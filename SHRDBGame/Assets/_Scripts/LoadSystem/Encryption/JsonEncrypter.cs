using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public enum EncryptionMethod
{
    None,
    Aes256_HmacSha256,
    Aes256_Only,
    Xor
}

public static class JsonEncrypter
{
    //TODO: añadir extension encriptedmethod antes del archivo, hacerlo paralelizable
    private const int SaltSize = 16;
    private const int IvSize = 16;
    private static int KeySize => DeviceKeyProvider.KEY_SIZE;
    private const int HmacSize = 32;
    private const int Iterations = 100_000;

    public static void EncryptToFile(
        string path,
        string json,
        string password,
        EncryptionMethod method)
    {
        if (Directory.Exists(path))
        {
            Debug.LogError("Your are trying to write on a directory");
        }
        Debug.Log("[JsonEncrypter]File saved with encryption method:" + method.ToString());
        byte[] data = method switch
        {
            EncryptionMethod.None => Encoding.UTF8.GetBytes(json),
            EncryptionMethod.Aes256_HmacSha256 => EncryptAesHmac(json, password),
            EncryptionMethod.Aes256_Only => EncryptAesOnly(json, password),
            EncryptionMethod.Xor => EncryptXor(json, password),
            _ => throw new NotSupportedException()
        };

        File.WriteAllBytes(path, data);
    }

    public static string DecryptFromFile(
        string path,
        string password,
        EncryptionMethod method)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Debug.Log("[JsonEncrypter]File loaded with encryption method:" + method.ToString());
        return method switch
        {
            EncryptionMethod.None => Encoding.UTF8.GetString(fileData),
            EncryptionMethod.Aes256_HmacSha256 => DecryptAesHmac(fileData, password),
            EncryptionMethod.Aes256_Only => DecryptAesOnly(fileData, password),
            EncryptionMethod.Xor => DecryptXor(fileData, password),
            _ => throw new NotSupportedException()
        };
    }

    // ===============================
    // AES-256 + HMAC SHA256
    // ===============================

    private static byte[] EncryptAesHmac(string json, string password)
    {
        byte[] salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt);
        using var kdf = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256);

        byte[] aesKey = kdf.GetBytes(KeySize);
        byte[] hmacKey = kdf.GetBytes(KeySize);

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = aesKey;
        aes.GenerateIV();

        byte[] plainBytes = Encoding.UTF8.GetBytes(json);
        byte[] cipherBytes;

        using (var encryptor = aes.CreateEncryptor())
        {
            cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        byte[] dataToAuth = Combine(salt, aes.IV, cipherBytes);

        byte[] hmac;
        using (var hmacSha = new HMACSHA256(hmacKey))
        {
            hmac = hmacSha.ComputeHash(dataToAuth);
        }

        return Combine(dataToAuth, hmac);
    }
    private static string DecryptAesHmac(byte[] fileData, string password)
    {
        if (fileData.Length < SaltSize + IvSize + HmacSize)
            throw new CryptographicException("Encrypted file is too small or corrupted.");

        byte[] salt = fileData[..SaltSize];
        byte[] iv = fileData[SaltSize..(SaltSize + IvSize)];
        byte[] hmacStored = fileData[^HmacSize..];
        byte[] cipherBytes = fileData[(SaltSize + IvSize)..^HmacSize];

        using var kdf = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256);

        byte[] aesKey = kdf.GetBytes(KeySize);
        byte[] hmacKey = kdf.GetBytes(KeySize);

        byte[] dataToAuth = Combine(salt, iv, cipherBytes);

        using (var hmacSha = new HMACSHA256(hmacKey))
        {
            byte[] computedHmac = hmacSha.ComputeHash(dataToAuth);

            if (!CryptographicOperations.FixedTimeEquals(hmacStored, computedHmac))
                throw new CryptographicException("HMAC validation failed. File may be corrupted or password incorrect.");
        }

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = aesKey;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();

        byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    // ===============================
    // AES only (sin HMAC)
    // ===============================

    private static byte[] EncryptAesOnly(string json, string password)
    {
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using var kdf = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] key = kdf.GetBytes(KeySize);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();

        byte[] plainBytes = Encoding.UTF8.GetBytes(json);

        using var encryptor = aes.CreateEncryptor();
        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return Combine(salt, aes.IV, cipherBytes);
    }

    private static string DecryptAesOnly(byte[] fileData, string password)
    {
        byte[] salt = fileData[..SaltSize];
        byte[] iv = fileData[SaltSize..(SaltSize + IvSize)];
        byte[] cipherBytes = fileData[(SaltSize + IvSize)..];

        using var kdf = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] key = kdf.GetBytes(KeySize);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    // ===============================
    // XOR simple (ofuscación)
    // ===============================

    private static byte[] EncryptXor(string json, string password)
    {
        byte[] data = Encoding.UTF8.GetBytes(json);
        byte[] key = Encoding.UTF8.GetBytes(password);

        for (int i = 0; i < data.Length; i++)
            data[i] ^= key[i % key.Length];

        return data;
    }

    private static string DecryptXor(byte[] fileData, string password)
    {
        byte[] key = Encoding.UTF8.GetBytes(password);

        for (int i = 0; i < fileData.Length; i++)
            fileData[i] ^= key[i % key.Length];

        return Encoding.UTF8.GetString(fileData);
    }

    private static byte[] Combine(params byte[][] arrays)
    {
        int length = 0;
        foreach (var arr in arrays)
            length += arr.Length;

        byte[] result = new byte[length];
        int offset = 0;

        foreach (var arr in arrays)
        {
            Buffer.BlockCopy(arr, 0, result, offset, arr.Length);
            offset += arr.Length;
        }

        return result;
    }
}
