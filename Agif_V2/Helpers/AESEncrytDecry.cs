using System.Security.Cryptography;
using System.Text;

namespace Agif_V2.Helpers
{
    public static class AESEncrytDecry
    {
        public static string GetKey(int sizeInBytes = 32)
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(sizeInBytes));
        }

        public static string GetSalt(int sizeInBytes = 16)
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(sizeInBytes));
        }
        public static string DecryptAES(string cipherTextBase64, string keyBase64)
        {
            if (string.IsNullOrEmpty(cipherTextBase64)) throw new ArgumentNullException(nameof(cipherTextBase64));
            if (string.IsNullOrEmpty(keyBase64)) throw new ArgumentNullException(nameof(keyBase64));

            byte[] fullData = Convert.FromBase64String(cipherTextBase64);
            byte[] keyBytes = Convert.FromBase64String(keyBase64);

            if (fullData.Length < 48) // 16 IV + 32 HMAC minimum
                throw new CryptographicException("Invalid encrypted data.");

            // 🔥 Extract parts
            byte[] iv = new byte[16];
            byte[] hmac = new byte[32];
            byte[] cipherText = new byte[fullData.Length - 16 - 32];

            Buffer.BlockCopy(fullData, 0, iv, 0, 16);
            Buffer.BlockCopy(fullData, 16, cipherText, 0, cipherText.Length);
            Buffer.BlockCopy(fullData, 16 + cipherText.Length, hmac, 0, 32);

            // 🔥 Recompute HMAC
            byte[] ivAndCipher = new byte[16 + cipherText.Length];
            Buffer.BlockCopy(iv, 0, ivAndCipher, 0, 16);
            Buffer.BlockCopy(cipherText, 0, ivAndCipher, 16, cipherText.Length);

            using var hmacSha = new HMACSHA256(keyBytes);
            byte[] computedHmac = hmacSha.ComputeHash(ivAndCipher);

            // 🔥 Compare HMAC securely
            if (!CryptographicOperations.FixedTimeEquals(hmac, computedHmac))
                throw new CryptographicException("Data tampered or invalid key.");

            // 🔥 Decrypt
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = keyBytes;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] result = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

            return Encoding.UTF8.GetString(result);
        }

        public static string EncryptAES(string plainText, string keyBase64)
        {
            if (string.IsNullOrEmpty(plainText)) throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrEmpty(keyBase64)) throw new ArgumentNullException(nameof(keyBase64));

            byte[] keyBytes = Convert.FromBase64String(keyBase64);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = keyBytes;
            aes.GenerateIV();

            byte[] cipherText;
            using (var encryptor = aes.CreateEncryptor())
            {
                cipherText = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }

            // 🔥 Step 1: Combine IV + CipherText
            byte[] ivAndCipher = new byte[aes.IV.Length + cipherText.Length];
            Buffer.BlockCopy(aes.IV, 0, ivAndCipher, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherText, 0, ivAndCipher, aes.IV.Length, cipherText.Length);

            // 🔥 Step 2: Generate HMAC
            byte[] hmac;
            using (var hmacSha = new HMACSHA256(keyBytes))
            {
                hmac = hmacSha.ComputeHash(ivAndCipher);
            }

            // 🔥 Step 3: Final = IV + Cipher + HMAC
            byte[] finalData = new byte[ivAndCipher.Length + hmac.Length];
            Buffer.BlockCopy(ivAndCipher, 0, finalData, 0, ivAndCipher.Length);
            Buffer.BlockCopy(hmac, 0, finalData, ivAndCipher.Length, hmac.Length);

            return Convert.ToBase64String(finalData);
        }
    }
}
