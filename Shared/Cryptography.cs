using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace KiAcorp.Shared;

public class Cryptography
{

    // Key generate
    public static ICipherParameters KeyParameterGeneration(int keySize)
    {
        CipherKeyGenerator keyGen = new();
        SecureRandom secureRandom = new();
        keyGen.Init(new KeyGenerationParameters(secureRandom, 128));
        KeyParameter keyParam = keyGen.GenerateKeyParameter();
        return keyParam;
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

    // AES ECB Encryption
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

        return finalCipherTextData;
    }

    // AES ECB Decryption
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

        return finalPlainTextData;
    }

    // DES ECB Encryption
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
        byte[] finalCipherTextData = new byte[cipherTextData.Length - (blockSize - finalLength)];
        Array.Copy(cipherTextData, 0, finalCipherTextData, 0, finalCipherTextData.Length);

        return finalCipherTextData;
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
        byte[] finalPlainTextData = new byte[plainTextData.Length - (blockSize - finalLength)];
        Array.Copy(plainTextData, 0, finalPlainTextData, 0, finalPlainTextData.Length);

        return finalPlainTextData;
    }

    // RC4 Encryption
    public static byte[] Rc4Encrypt(ICipherParameters keyParam, byte[] plainTextData)
    {
        IStreamCipher rc4Engine = new RC4Engine();
        rc4Engine.Init(true, keyParam);
        var cipherText = new byte[plainTextData.Length];
        rc4Engine.ProcessBytes(plainTextData, 0, plainTextData.Length, cipherText, 0);
        return cipherText;
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