using Microsoft.Xna.Framework.Input;
using System;
#if WINDOWS_UAP || WP81
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
#else
using System.Security.Cryptography;
using System.Text;
#endif

namespace Inlumino_SHARED
{
    // Warning, Encryption results are different on UAP and Android. Probably due to a different default vector.
    class SecurityProvider
    {
        static byte[] Default = new byte[] { 44, 58, 66, 92, 16, 32, 14, 182, 250, 26, 188, 164, 12, 74, 78, 92, 220, 34, 70, 210, 144, 180, 120, 64 };
#if WINDOWS_UAP || WP81               
        public static string GetMD5Hash(string data)
        {
            // Convert the message string to binary data.        
            IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8);

            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);

            // Hash the message.
            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

            // Verify that the hash length equals the length specified for the algorithm.
            if (buffHash.Length != objAlgProv.HashLength)
            {
                throw new Exception("There was an error creating the hash");
            }

            return CryptographicBuffer.EncodeToBase64String(buffHash);
        }
        public static string Encrypt(string toEncrypt, byte[] key = null)
        {
            if (toEncrypt == "") return "";
            if (key == null || key.Length == 0) key = Default;
            var keyHash = key.AsBuffer();

            // Create a buffer that contains the encoded message to be encrypted.
            var toEncryptBuffer = CryptographicBuffer.ConvertStringToBinary(toEncrypt, BinaryStringEncoding.Utf8);

            // Open a symmetric algorithm provider for the specified algorithm.
            var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.TripleDesEcbPkcs7);

            // Create a symmetric key.
            var symetricKey = aes.CreateSymmetricKey(keyHash);

            // The input key must be securely shared between the sender of the cryptic message
            // and the recipient. The initialization vector must also be shared but does not
            // need to be shared in a secure manner. If the sender encodes a message string
            // to a buffer, the binary encoding method must also be shared with the recipient.
            var buffEncrypted = CryptographicEngine.Encrypt(symetricKey, toEncryptBuffer, null);
            // We are using Base64 to convert bytes to string since you might get unmatched characters
            // in the encrypted buffer that we cannot convert to string with UTF8.
            var strEncrypted = CryptographicBuffer.EncodeToBase64String(buffEncrypted);

            return strEncrypted;
        }
        public static string Decrypt(string cipherString, byte[] key = null)
        {
            if (cipherString == "") return "";
            if (key == null || key.Length == 0) key = Default;
            var keyHash = key.AsBuffer();

            // Create a buffer that contains the encoded message to be decrypted.
            IBuffer toDecryptBuffer = CryptographicBuffer.DecodeFromBase64String(cipherString);

            // Open a symmetric algorithm provider for the specified algorithm.
            SymmetricKeyAlgorithmProvider aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.TripleDesEcbPkcs7);

            // Create a symmetric key.
            var symetricKey = aes.CreateSymmetricKey(keyHash);

            var buffDecrypted = CryptographicEngine.Decrypt(symetricKey, toDecryptBuffer, null);

            string strDecrypted = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffDecrypted);

            return strDecrypted;
        }
#else
        public static string GetMD5Hash(string data)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            return Convert.ToBase64String(hashmd5.ComputeHash(Encoding.UTF8.GetBytes(data)));
        }
        public static string Encrypt(string toEncrypt, byte[] key = null)
        {
            if (toEncrypt == "") return "";
            if (key == null || key.Length == 0) key = Default;

            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
            byte[] keyArray = key;

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray);
        }
        public static string Decrypt(string cipherString, byte[] key = null)
        {
            if (cipherString == "") return "";
            if (key == null || key.Length == 0) key = Default;

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            byte[] keyArray = key;

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return Encoding.UTF8.GetString(resultArray, 0, resultArray.Length);
        }
#endif
    }
}
