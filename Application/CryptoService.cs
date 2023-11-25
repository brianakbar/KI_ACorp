namespace ACorp.Application;

using System.Security.Cryptography;
using System.Text;
using ACorp.Shared;
using Org.BouncyCastle.Crypto;

public class CryptoService
{
    static readonly int keySize = 128;
    static readonly char[] keyChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    const string AESKey = "ThisIsAKeyForAES";
    const string AESIV = "ThisIVAKeyForAES";
    const string DESKey = "KyForDES";
    const string DESIV = "IvForDES";
    const string RC4Key = "ThisIsMyRC4Key";

    public static string EncryptAESFromString(string data)
    {
        byte[] stringByte = Encoding.UTF8.GetBytes(data);
        string base64String = Convert.ToBase64String(stringByte);
        byte[] base64Byte = Convert.FromBase64String(base64String);
        return Convert.ToBase64String(EncryptAES(base64Byte));
    }

    public static string DecryptAESFromString(string data)
    {
        string base64String = Convert.ToBase64String(DecryptAES(Convert.FromBase64String(data)));
        string stringReal = Encoding.UTF8.GetString(Convert.FromBase64String(base64String)).TrimEnd('\0');
        return stringReal;
    }

    public static string EncryptDESFromString(string data)
    {
        byte[] stringByte = Encoding.UTF8.GetBytes(data);
        string base64String = Convert.ToBase64String(stringByte);
        byte[] base64Byte = Convert.FromBase64String(base64String);
        return Convert.ToBase64String(EncryptDES(base64Byte));
    }

    public static string DecryptDESFromString(string data)
    {
        string base64String = Convert.ToBase64String(DecryptDES(Convert.FromBase64String(data)));
        string stringReal = Encoding.UTF8.GetString(Convert.FromBase64String(base64String)).TrimEnd('\0');
        return stringReal;
    }

    public static string EncryptRC4FromString(string data)
    {
        byte[] stringByte = Encoding.UTF8.GetBytes(data);
        string base64String = Convert.ToBase64String(stringByte);
        byte[] base64Byte = Convert.FromBase64String(base64String);
        return Convert.ToBase64String(EncryptRC4(base64Byte));
    }

    public static string DecryptRC4FromString(string data)
    {
        string base64String = Convert.ToBase64String(DecryptRC4(Convert.FromBase64String(data)));
        string stringReal = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));
        return stringReal;
    }

    public static byte[] EncryptAES(byte[] data)
    {
        ICipherParameters myAeskeyParam = CreateKeyWithIV(AESKey, AESIV);

        return Cryptography.AesCbcPaddedEncrypt(myAeskeyParam, data);
    }

    public static byte[] DecryptAES(byte[] encryptedData)
    {
        ICipherParameters myAeskeyParam = CreateKeyWithIV(AESKey, AESIV);

        return Cryptography.AesCbcPaddedDecrypt(myAeskeyParam, encryptedData);
    }

    public static byte[] EncryptDES(byte[] data)
    {
        ICipherParameters myDeskeyParam = CreateKeyWithIV(DESKey, DESIV);

        return Cryptography.DesCbcPaddedEncrypt(myDeskeyParam, data);
    }

    public static byte[] DecryptDES(byte[] encryptedData)
    {
        ICipherParameters myDeskeyParam = CreateKeyWithIV(DESKey, DESIV);

        return Cryptography.DesCbcPaddedDecrypt(myDeskeyParam, encryptedData);
    }

    public static byte[] EncryptRC4(byte[] data)
    {
        ICipherParameters myRc4keyParam = CreateKey(RC4Key);

        return Cryptography.Rc4Encrypt(myRc4keyParam, data);
    }

    public static byte[] DecryptRC4(byte[] encryptedData)
    {
        ICipherParameters myRc4keyParam = CreateKey(RC4Key);

        return Cryptography.Rc4Decrypt(myRc4keyParam, encryptedData);
    }

    public static string GetUniqueKey()
    {
        return GetUniqueKey(keySize);
    }

    public static string GetUniqueKey(int size)
    {
        byte[] data = new byte[4 * size];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(data);
        }
        StringBuilder result = new(size);
        for (int i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % keyChars.Length;

            result.Append(keyChars[idx]);
        }

        return result.ToString();
    }


    static ICipherParameters CreateKeyWithIV(string key, string iv)
    {
        var myKey = Encoding.UTF8.GetBytes(key);
        var myIv = Encoding.UTF8.GetBytes(iv);
        return Cryptography.KeyParameterGenerationWithKeyAndIV(myKey, myIv);
    }

    static ICipherParameters CreateKey(string key)
    {
        var myKey = Encoding.UTF8.GetBytes(key);
        return Cryptography.KeyParameterGenerationWithKey(myKey);
    }
}