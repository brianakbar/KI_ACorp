namespace ACorp.Application;

using System.Text;
using KiAcorp.Shared;
using Org.BouncyCastle.Crypto;

public class CryptoService
{
    const string AESKey = "ThisIsAKeyForAES";
    const string AESIV = "ThisIVAKeyForAES";
    const string DESKey = "KyForDES";
    const string DESIV = "IvForDES";
    const string RC4Key = "ThisIsMyRC4Key";

    public string EncryptAESFromString(string data)
    {
        return Encoding.UTF8.GetString(EncryptAES(Encoding.UTF8.GetBytes(data)));
    }

    public string DecryptAESFromString(string data)
    {
        return Encoding.UTF8.GetString(DecryptAES(Encoding.UTF8.GetBytes(data)));
    }

    public string EncryptDESFromString(string data)
    {
        return Encoding.UTF8.GetString(EncryptDES(Encoding.UTF8.GetBytes(data)));
    }

    public string DecryptDESFromString(string data)
    {
        return Encoding.UTF8.GetString(DecryptDES(Encoding.UTF8.GetBytes(data)));
    }

    public string EncryptRC4FromString(string data)
    {
        return Encoding.UTF8.GetString(EncryptRC4(Encoding.UTF8.GetBytes(data)));
    }

    public string DecryptRC4FromString(string data)
    {
        return Encoding.UTF8.GetString(DecryptRC4(Encoding.UTF8.GetBytes(data)));
    }

    public byte[] EncryptAES(byte[] data)
    {
        ICipherParameters myAeskeyParam = CreateKeyWithIV(AESKey, AESIV);

        return Cryptography.AesCbcPaddedEncrypt(myAeskeyParam, data);
    }

    public byte[] DecryptAES(byte[] encryptedData)
    {
        ICipherParameters myAeskeyParam = CreateKeyWithIV(AESKey, AESIV);

        return Cryptography.AesCbcPaddedDecrypt(myAeskeyParam, encryptedData);
    }

    public byte[] EncryptDES(byte[] data)
    {
        ICipherParameters myDeskeyParam = CreateKeyWithIV(DESKey, DESIV);

        return Cryptography.DesCbcPaddedEncrypt(myDeskeyParam, data);
    }

    public byte[] DecryptDES(byte[] encryptedData)
    {
        ICipherParameters myDeskeyParam = CreateKeyWithIV(DESKey, DESIV);

        return Cryptography.DesCbcPaddedDecrypt(myDeskeyParam, encryptedData);
    }

    public byte[] EncryptRC4(byte[] data)
    {
        ICipherParameters myRc4keyParam = CreateKey(RC4Key);

        return Cryptography.Rc4Encrypt(myRc4keyParam, data);
    }

    public byte[] DecryptRC4(byte[] encryptedData)
    {
        ICipherParameters myRc4keyParam = CreateKey(RC4Key);

        return Cryptography.Rc4Decrypt(myRc4keyParam, encryptedData);
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