//using Microsoft.AspNetCore.Mvc;
//using System.Security.Cryptography;
//using System.Text;

//namespace Agif_V2.Helpers
//{
//    public static class AESEncrytDecry
//    {
//        private static Random random = new Random();
//        public static string GetSaltLegacy()
//        {
//            var builder = new StringBuilder();
//            while (builder.Length < 16)
//            {
//                builder.Append(random.Next(10).ToString());
//            }
//            return builder.ToString();
//        }
//        public static string GetKeyLegacy()
//        {
//            var builder = new StringBuilder();
//            while (builder.Length < 16)
//            {
//                builder.Append(random.Next(10).ToString());
//            }
//            return builder.ToString();
//        }

//        public static string DecryptAESLegacy(string cipherText, string? key)
//        {
//            var iv = Encoding.UTF8.GetBytes(key.Substring(0, 16));
//            var keyBytes = Encoding.UTF8.GetBytes(key);

//            var buffer = Convert.FromBase64String(cipherText);

//            using Aes aes = Aes.Create();
//            aes.Mode = CipherMode.CBC;
//            aes.Padding = PaddingMode.PKCS7;
//            aes.Key = keyBytes;
//            aes.IV = iv;

//            using var decryptor = aes.CreateDecryptor();
//            var result = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
//            return Encoding.UTF8.GetString(result);
//        }

//        public static string GetKey(int sizeInBytes = 32) // 32 bytes = AES-256
//        {
//            byte[] randomBytes = new byte[sizeInBytes];
//            using (var rng = RandomNumberGenerator.Create())
//            {
//                rng.GetBytes(randomBytes);
//            }
//            return Convert.ToBase64String(randomBytes);
//        }

//        public static string GetSalt(int sizeInBytes = 16)
//        {
//            byte[] randomBytes = new byte[sizeInBytes];
//            using (var rng = RandomNumberGenerator.Create())
//            {
//                rng.GetBytes(randomBytes);
//            }
//            return Convert.ToBase64String(randomBytes);
//        }

//        // 2. Modern Decryption
//        // Assumes your key is now Base64 encoded, and the IV is attached to the ciphertext
//        public static string DecryptAES(string cipherTextBase64, string keyBase64)
//        {
//            if (string.IsNullOrEmpty(cipherTextBase64)) throw new ArgumentNullException(nameof(cipherTextBase64));
//            if (string.IsNullOrEmpty(keyBase64)) throw new ArgumentNullException(nameof(keyBase64));

//            byte[] fullCipher = Convert.FromBase64String(cipherTextBase64);
//            byte[] keyBytes = Convert.FromBase64String(keyBase64);

//            using Aes aes = Aes.Create();
//            aes.Mode = CipherMode.CBC;
//            aes.Padding = PaddingMode.PKCS7;
//            aes.Key = keyBytes;

//            // Extract the 16-byte IV from the beginning of the ciphertext package
//            byte[] iv = new byte[16];
//            byte[] cipher = new byte[fullCipher.Length - 16];

//            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
//            Buffer.BlockCopy(fullCipher, 16, cipher, 0, cipher.Length);

//            aes.IV = iv;

//            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
//            byte[] result = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

//            return Encoding.UTF8.GetString(result);
//        }

//        public static string EncryptAES(string plainText, string keyBase64)
//        {
//            if (string.IsNullOrEmpty(plainText)) throw new ArgumentNullException(nameof(plainText));
//            if (string.IsNullOrEmpty(keyBase64)) throw new ArgumentNullException(nameof(keyBase64));

//            byte[] keyBytes = Convert.FromBase64String(keyBase64);

//            using Aes aes = Aes.Create();
//            aes.Mode = CipherMode.CBC;
//            aes.Padding = PaddingMode.PKCS7;
//            aes.Key = keyBytes;
//            aes.GenerateIV(); // Generates a random, secure 16-byte IV

//            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
//            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
//            byte[] cipherText = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

//            // Combine IV and Ciphertext into one array
//            byte[] result = new byte[aes.IV.Length + cipherText.Length];
//            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
//            Buffer.BlockCopy(cipherText, 0, result, aes.IV.Length, cipherText.Length);

//            // Return the combined package as Base64
//            return Convert.ToBase64String(result);
//        }


//    }

//}

using System.Security.Cryptography;
using System.Text;

namespace Agif_V2.Helpers
{
    public static class AESEncrytDecry
    {
        // Removed the static `Random` instance to fix thread-safety issues.

        public static string GetSaltLegacy()
        {
            var builder = new StringBuilder();
            while (builder.Length < 16)
            {
                // Use Random.Shared for thread-safe random number generation
                builder.Append(Random.Shared.Next(10).ToString());
            }
            return builder.ToString();
        }

        public static string GetKeyLegacy()
        {
            var builder = new StringBuilder();
            while (builder.Length < 16)
            {
                builder.Append(Random.Shared.Next(10).ToString());
            }
            return builder.ToString();
        }

        public static string DecryptAESLegacy(string cipherText, string? key)
        {
            // 1. Added validation to prevent NullReference or ArgumentOutOfRange exceptions
            if (string.IsNullOrEmpty(cipherText)) throw new ArgumentNullException(nameof(cipherText));
            if (string.IsNullOrEmpty(key) || key.Length < 16)
                throw new ArgumentException("Legacy key must be at least 16 characters long.", nameof(key));

            var keyBytes = Encoding.UTF8.GetBytes(key);

            // 2. AES requires strictly 16, 24, or 32 byte keys. 
            if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32)
                throw new CryptographicException("Legacy key length is invalid. Must be exactly 16, 24, or 32 characters.");

            var iv = Encoding.UTF8.GetBytes(key.Substring(0, 16));
            var buffer = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = keyBytes;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var result = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(result);
        }

        public static string GetKey(int sizeInBytes = 32) // 32 bytes = AES-256
        {
            // 3. Simplified using modern .NET crypto methods
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(sizeInBytes));
        }

        public static string GetSalt(int sizeInBytes = 16)
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(sizeInBytes));
        }

        // 2. Modern Decryption
        public static string DecryptAES(string cipherTextBase64, string keyBase64)
        {
            if (string.IsNullOrEmpty(cipherTextBase64)) throw new ArgumentNullException(nameof(cipherTextBase64));
            if (string.IsNullOrEmpty(keyBase64)) throw new ArgumentNullException(nameof(keyBase64));

            byte[] fullCipher = Convert.FromBase64String(cipherTextBase64);
            byte[] keyBytes = Convert.FromBase64String(keyBase64);

            // 4. Prevent crashes if the ciphertext is corrupted or too short
            if (fullCipher.Length < 16)
                throw new CryptographicException("Ciphertext is too short to contain a valid IV.");

            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = keyBytes;

            byte[] iv = new byte[16];
            byte[] cipher = new byte[fullCipher.Length - 16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, 16, cipher, 0, cipher.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] result = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(result);
        }

        public static string EncryptAES(string plainText, string keyBase64)
        {
            if (string.IsNullOrEmpty(plainText)) throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrEmpty(keyBase64)) throw new ArgumentNullException(nameof(keyBase64));

            byte[] keyBytes = Convert.FromBase64String(keyBase64);

            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = keyBytes;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherText = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            byte[] result = new byte[aes.IV.Length + cipherText.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherText, 0, result, aes.IV.Length, cipherText.Length);

            return Convert.ToBase64String(result);
        }
    }
}
