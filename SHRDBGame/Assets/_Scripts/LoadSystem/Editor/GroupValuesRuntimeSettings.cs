using UnityEngine;

public class GroupValuesRuntimeSettings : ScriptableObject
{
    public bool encryptJson;
    public EncryptionMethod encryptionMethod;
    public string passwordSalt;
}