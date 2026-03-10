using System.IO;
using System.Security.Cryptography;
using UnityEngine;


public static class DeviceKeyProvider
{
    public const int KEY_SIZE = 32;

    static string KeyPath =>
        Path.Combine(Application.persistentDataPath, "device.key");

    public static byte[] GetOrCreateDeviceKey()
    {
        if (File.Exists(KeyPath))
        {
            Debug.Log("[DeviceKeyProvider] File found in:"+KeyPath);
            return File.ReadAllBytes(KeyPath);
        }

        byte[] key = GenerateNewPassword();

        File.WriteAllBytes(KeyPath, key);
        Debug.Log("[DeviceKeyProvider] Created file");
        return key;
    }
    public static void GenerateNewDeviceKey(){
        File.WriteAllBytes(KeyPath,GenerateNewPassword());
        Debug.Log("[DeviceKeyProvider]New password generated");
    }
     public static byte[] GenerateNewPassword()
    {
        byte[] randomBytes = new byte[KEY_SIZE];
        RandomNumberGenerator.Fill(randomBytes);
        return randomBytes;
    }
    
}