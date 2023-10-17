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


    // AES ECB Encryption
    public static byte[] AesEcbPaddedEncrypt(ICipherParameters keyParam, byte[] plainTextData)
    {
        IBlockCipher symmetricBlockCipher = new AesEngine();
        IBlockCipher symmetricBlockMode = new EcbBlockCipher(symmetricBlockCipher);
        IBlockCipherPadding padding = new Pkcs7Padding();

        PaddedBufferedBlockCipher ecbChiper = new(symmetricBlockMode, padding);

        ecbChiper.Init(true, keyParam);
        int blockSize = ecbChiper.GetBlockSize();
        byte[] cipherTextData = new byte[ecbChiper.GetOutputSize(plainTextData.Length)];
        int processLength = ecbChiper.ProcessBytes(plainTextData, 0, plainTextData.Length, cipherTextData, 0);
        int finalLength = ecbChiper.DoFinal(cipherTextData, processLength);
        byte[] finalCipherTextData = new byte[cipherTextData.Length - (blockSize - finalLength)];
        Array.Copy(cipherTextData, 0, finalCipherTextData, 0, finalCipherTextData.Length);

        return finalCipherTextData;
    }

    // AES ECB Decryption
    public static byte[] AesEcbPaddedDecrypt(ICipherParameters keyParam, byte[] cipherTextData)
    {
        IBlockCipher symmetricBlockCipher = new AesEngine();
        IBlockCipher symmetricBlockMode = new EcbBlockCipher(symmetricBlockCipher);
        IBlockCipherPadding padding = new Pkcs7Padding();

        PaddedBufferedBlockCipher ecbChiper = new(symmetricBlockMode, padding);

        ecbChiper.Init(false, keyParam);
        int blockSize = ecbChiper.GetBlockSize();
        byte[] plainTextData = new byte[ecbChiper.GetOutputSize(cipherTextData.Length)];
        int processLength = ecbChiper.ProcessBytes(cipherTextData, 0, cipherTextData.Length, plainTextData, 0);
        int finalLength = ecbChiper.DoFinal(plainTextData, processLength);
        byte[] finalPlainTextData = new byte[plainTextData.Length - (blockSize - finalLength)];
        Array.Copy(plainTextData, 0, finalPlainTextData, 0, finalPlainTextData.Length);

        return finalPlainTextData;
    }

    // DES ECB Encryption
    public static byte[] DesEcbPaddedEncrypt(ICipherParameters keyParam, byte[] plainTextData)
    {
        IBlockCipher symmetricBlockCipher = new DesEngine();
        IBlockCipher symmetricBlockMode = new EcbBlockCipher(symmetricBlockCipher);
        IBlockCipherPadding padding = new Pkcs7Padding();

        PaddedBufferedBlockCipher ecbChiper = new(symmetricBlockMode, padding);

        ecbChiper.Init(true, keyParam);
        int blockSize = ecbChiper.GetBlockSize();
        byte[] cipherTextData = new byte[ecbChiper.GetOutputSize(plainTextData.Length)];
        int processLength = ecbChiper.ProcessBytes(plainTextData, 0, plainTextData.Length, cipherTextData, 0);
        int finalLength = ecbChiper.DoFinal(cipherTextData, processLength);
        byte[] finalCipherTextData = new byte[cipherTextData.Length - (blockSize - finalLength)];
        Array.Copy(cipherTextData, 0, finalCipherTextData, 0, finalCipherTextData.Length);

        return finalCipherTextData;
    }

    // DES ECB Decryption
    public static byte[] DesEcbPaddedDecrypt(ICipherParameters keyParam, byte[] cipherTextData)
    {
        IBlockCipher symmetricBlockCipher = new DesEngine();
        IBlockCipher symmetricBlockMode = new EcbBlockCipher(symmetricBlockCipher);
        IBlockCipherPadding padding = new Pkcs7Padding();

        PaddedBufferedBlockCipher ecbChiper = new(symmetricBlockMode, padding);

        ecbChiper.Init(false, keyParam);
        int blockSize = ecbChiper.GetBlockSize();
        byte[] plainTextData = new byte[ecbChiper.GetOutputSize(cipherTextData.Length)];
        int processLength = ecbChiper.ProcessBytes(cipherTextData, 0, cipherTextData.Length, plainTextData, 0);
        int finalLength = ecbChiper.DoFinal(plainTextData, processLength);
        byte[] finalPlainTextData = new byte[plainTextData.Length - (blockSize - finalLength)];
        Array.Copy(plainTextData, 0, finalPlainTextData, 0, finalPlainTextData.Length);

        return finalPlainTextData;
    }
}