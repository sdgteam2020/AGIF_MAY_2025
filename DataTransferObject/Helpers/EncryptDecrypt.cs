using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Helpers
{
    public class EncryptDecrypt
    {
        private const string key = "!QAZ2wsx!@#$1234";
        private const string iv = "HR$2pIjHR$2pIj12";
        public static string EncryptionData(string parameter)
        {
            byte[] parameterBytes = Encoding.UTF8.GetBytes(parameter);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                byte[] encryptedBytes = encryptor.TransformFinalBlock(parameterBytes, 0, parameterBytes.Length);

                string encryptedParameter = Convert.ToBase64String(encryptedBytes);

                return Uri.EscapeDataString(encryptedParameter);
            }
        }

        public static string DecryptionData(string encryptedParameter)
        {
            encryptedParameter = Uri.UnescapeDataString(encryptedParameter);

            byte[] encryptedBytes = Convert.FromBase64String(encryptedParameter);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                string decryptedParameter = Encoding.UTF8.GetString(decryptedBytes);

                return decryptedParameter;


            }
        }
    }
}
