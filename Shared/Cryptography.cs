using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace ACorp.Shared;

public class Cryptography
{
    // Key generate
    public static byte[] KeyParameterGeneration(int keySize)
    {
        CipherKeyGenerator keyGen = new();
        SecureRandom secureRandom = new();
        keyGen.Init(new KeyGenerationParameters(secureRandom, 128));
        KeyParameter keyParam = keyGen.GenerateKeyParameter();
        byte[] keyBytes = keyParam.GetKey();
        return keyBytes;
    }

    public static ICipherParameters KeyParameterGenerationWithKey(byte[] myKey)
    {
        ICipherParameters keyParam = new KeyParameter(myKey);
        return keyParam;
    }

    public static ICipherParameters KeyParameterGenerationWithKeyAndIV(byte[] myKey, byte[] myIV)
    {
        ICipherParameters keyParam = new ParametersWithIV(new KeyParameter(myKey), myIV);
        return keyParam;
    }

    public static (string publicKey, string privateKey) GenerateRSAKeys()
    {
        using RSACryptoServiceProvider rsa = new(2048); // 2048 is the key size
        var publicKey = rsa.ToXmlString(false); // false to get the public key
        var privateKey = rsa.ToXmlString(true); // true to get the private key
        return (publicKey, privateKey);
    }

    public static byte[] SignWithRSA(byte[] hashData, string privateKey)
    {
        using RSACryptoServiceProvider rsa = new();

        rsa.FromXmlString(privateKey);

        return rsa.SignHash(hashData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public static bool VerifyWithRSA(byte[] hashData, byte[] signature, string publicKey)
    {
        using RSACryptoServiceProvider rsa = new();

        rsa.FromXmlString(publicKey);

        return rsa.VerifyHash(hashData, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public static string EncryptWithRSA(string dataToEncrypt, string publicKey)
    {
        using RSACryptoServiceProvider rsa = new();
        // Import the RSA public key
        rsa.FromXmlString(publicKey);
        RSAParameters publicKeyParams = rsa.ExportParameters(false);

        // Encrypt the data
        byte[] encryptedData = rsa.Encrypt(Encoding.UTF8.GetBytes(dataToEncrypt), true);

        // Convert encrypted data to Base64 string
        return Convert.ToBase64String(encryptedData);
    }

    public static string DecryptWithRSA(string dataToDecrypt, string privateKey)
    {
        using RSACryptoServiceProvider rsa = new();

        // Import the RSA private key
        rsa.FromXmlString(privateKey);
        RSAParameters privateKeyParams = rsa.ExportParameters(true);

        // Decrypt the data
        byte[] decryptedData = rsa.Decrypt(Convert.FromBase64String(dataToDecrypt), true);

        // Convert decrypted data to string
        return Encoding.UTF8.GetString(decryptedData);
    }

    // AES CBC Encryption
    public static byte[] AesCbcPaddedEncrypt(ICipherParameters keyParamWithIV, byte[] plainTextData)
    {
        IBlockCipher symmetricBlockCipher = new AesEngine();
        IBlockCipher symmetricBlockMode = new CbcBlockCipher(symmetricBlockCipher);
        IBlockCipherPadding padding = new Pkcs7Padding();

        PaddedBufferedBlockCipher cbcChiper = new(symmetricBlockMode, padding);

        cbcChiper.Init(true, keyParamWithIV);
        int blockSize = cbcChiper.GetBlockSize();
        byte[] cipherTextData = new byte[cbcChiper.GetOutputSize(plainTextData.Length)];
        int processLength = cbcChiper.ProcessBytes(plainTextData, 0, plainTextData.Length, cipherTextData, 0);
        int finalLength = cbcChiper.DoFinal(cipherTextData, processLength);
        byte[] finalCipherTextData = new byte[cipherTextData.Length - (blockSize - finalLength)];
        Array.Copy(cipherTextData, 0, finalCipherTextData, 0, finalCipherTextData.Length);

        return cipherTextData;
    }

    // AES CBC Decryption
    public static byte[] AesCbcPaddedDecrypt(ICipherParameters keyParamWithIV, byte[] cipherTextData)
    {
        IBlockCipher symmetricBlockCipher = new AesEngine();
        IBlockCipher symmetricBlockMode = new CbcBlockCipher(symmetricBlockCipher);
        IBlockCipherPadding padding = new Pkcs7Padding();

        PaddedBufferedBlockCipher cbcChiper = new(symmetricBlockMode, padding);

        cbcChiper.Init(false, keyParamWithIV);
        int blockSize = cbcChiper.GetBlockSize();
        byte[] plainTextData = new byte[cbcChiper.GetOutputSize(cipherTextData.Length)];
        int processLength = cbcChiper.ProcessBytes(cipherTextData, 0, cipherTextData.Length, plainTextData, 0);
        int finalLength = cbcChiper.DoFinal(plainTextData, processLength);
        byte[] finalPlainTextData = new byte[plainTextData.Length - (blockSize - finalLength)];
        Array.Copy(plainTextData, 0, finalPlainTextData, 0, finalPlainTextData.Length);

        return plainTextData;
    }

    // DES CBC Encryption
    public static byte[] DesCbcPaddedEncrypt(ICipherParameters keyParamWithIV, byte[] plainTextData)
    {
        IBlockCipher symmetricBlockCipher = new DesEngine();
        IBlockCipher symmetricBlockMode = new CbcBlockCipher(symmetricBlockCipher);
        IBlockCipherPadding padding = new Pkcs7Padding();

        PaddedBufferedBlockCipher cbcChiper = new(symmetricBlockMode, padding);

        cbcChiper.Init(true, keyParamWithIV);
        int blockSize = cbcChiper.GetBlockSize();
        byte[] cipherTextData = new byte[cbcChiper.GetOutputSize(plainTextData.Length)];
        int processLength = cbcChiper.ProcessBytes(plainTextData, 0, plainTextData.Length, cipherTextData, 0);
        int finalLength = cbcChiper.DoFinal(cipherTextData, processLength);
        byte[] finalCipherTextData = new byte[processLength + finalLength];
        Array.Copy(cipherTextData, 0, finalCipherTextData, 0, finalCipherTextData.Length);

        return cipherTextData;
    }

    // DES CBC Decryption
    public static byte[] DesCbcPaddedDecrypt(ICipherParameters keyParamWithIV, byte[] cipherTextData)
    {
        IBlockCipher symmetricBlockCipher = new DesEngine();
        IBlockCipher symmetricBlockMode = new CbcBlockCipher(symmetricBlockCipher);
        IBlockCipherPadding padding = new Pkcs7Padding();

        PaddedBufferedBlockCipher cbcChiper = new(symmetricBlockMode, padding);

        cbcChiper.Init(false, keyParamWithIV);
        int blockSize = cbcChiper.GetBlockSize();
        byte[] plainTextData = new byte[cbcChiper.GetOutputSize(cipherTextData.Length)];
        int processLength = cbcChiper.ProcessBytes(cipherTextData, 0, cipherTextData.Length, plainTextData, 0);
        int finalLength = cbcChiper.DoFinal(plainTextData, processLength);
        byte[] finalPlainTextData = new byte[processLength + finalLength];
        Array.Copy(plainTextData, 0, finalPlainTextData, 0, finalPlainTextData.Length);

        return plainTextData;
    }

    // RC4 Encryption
    public static byte[] Rc4Encrypt(ICipherParameters keyParam, byte[] plainTextData)
    {
        IStreamCipher rc4Engine = new RC4Engine();
        rc4Engine.Init(true, keyParam);
        var cipherText = new byte[plainTextData.Length];
        rc4Engine.ProcessBytes(plainTextData, 0, plainTextData.Length, cipherText, 0);
        int finalLength = plainTextData.Length;
        byte[] finalCiphertext = new byte[finalLength];
        Array.Copy(cipherText, finalCiphertext, finalLength);

        return finalCiphertext;
    }

    // RC4 Decryption
    public static byte[] Rc4Decrypt(ICipherParameters keyParam, byte[] cipherTextData)
    {
        IStreamCipher rc4cipher = new RC4Engine();
        rc4cipher.Init(false, keyParam);
        byte[] plainText = new byte[cipherTextData.Length];
        rc4cipher.ProcessBytes(cipherTextData, 0, cipherTextData.Length, plainText, 0);

        return plainText;
    }
}