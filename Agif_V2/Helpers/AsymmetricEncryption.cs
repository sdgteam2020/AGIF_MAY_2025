using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Agif_V2.Helpers
{
    public class AsymmetricEncryption
    {
        // Generate RSA key pair and return PEM formatted public key
        public (string publicKeyPem, string privateKeyXml) GenerateKeyPair()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                // Get private key in XML format for server-side storage
                string privateKeyXml = rsa.ToXmlString(true);

                // Export public key in PEM format for JavaScript
                string publicKeyPem = ExportPublicKeyToPem(rsa);

                return (publicKeyPem, privateKeyXml);
            }
        }

        // Convert RSA public key to PEM format (compatible with JSEncrypt)
        private string ExportPublicKeyToPem(RSACryptoServiceProvider rsa)
        {
            var publicKey = rsa.ExportParameters(false);

            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);

                // Write the RSA public key in PKCS#1 format
                writer.Write((byte)0x30); // SEQUENCE

                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);

                    // Write modulus
                    EncodeIntegerBigEndian(innerWriter, publicKey.Modulus);

                    // Write exponent
                    EncodeIntegerBigEndian(innerWriter, publicKey.Exponent);

                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);

                // Format as PEM
                var sb = new StringBuilder();
                sb.AppendLine("-----BEGIN PUBLIC KEY-----");

                // Split into 64-character lines
                for (int i = 0; i < base64.Length; i += 64)
                {
                    sb.AppendLine(base64.Substring(i, Math.Min(64, base64.Length - i)));
                }

                sb.AppendLine("-----END PUBLIC KEY-----");

                return sb.ToString();
            }
        }

        private void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0x80)
            {
                stream.Write((byte)length);
            }
            else if (length < 0x100)
            {
                stream.Write((byte)0x81);
                stream.Write((byte)length);
            }
            else if (length < 0x8000)
            {
                stream.Write((byte)0x82);
                stream.Write((byte)(length >> 8));
                stream.Write((byte)(length & 0xFF));
            }
        }

        private void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;

            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }

            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }

                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }

        // Decrypt string using private key in XML format
        public string DecryptString(string encryptedText, string privateKeyXml)
        {
            try
            {
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(privateKeyXml);
                    var data = Convert.FromBase64String(encryptedText);
                    var decryptedByte = rsa.Decrypt(data, false);
                    return Encoding.UTF8.GetString(decryptedByte);
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Decryption failed: {ex.Message}");
                return null;
            }
        }

        public string GetSalt(int sizeInBytes = 16)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(sizeInBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }
}