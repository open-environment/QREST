using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;

namespace QREST.App_Logic.BusinessLogicLayer
{
    /// <summary>
    /// This class provides encryption/decryption of string-based messages using the standard AES symmetric key encryption algorithm (Rijndeel).
    /// </summary>
    public class EncryptionAES
    {
        //get key value from config file 
        private static byte[] key = Convert.FromBase64String("e9kTCxgaVS1yuBuiJXDe0fEYr5CtNcQdGBoR2oPsNdE=");
        private static byte[] vector = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 221, 112, 79, 32, 114, 156 };
        private readonly ICryptoTransform encryptor, decryptor;
        private UTF8Encoding encoder;


        public EncryptionAES()
        {
            RijndaelManaged rm = new RijndaelManaged();
            encryptor = rm.CreateEncryptor(key, vector);
            decryptor = rm.CreateDecryptor(key, vector);
            encoder = new UTF8Encoding();
        }

        public string Encrypt(string unencrypted)
        {
            return Convert.ToBase64String(Encrypt(encoder.GetBytes(unencrypted)));
        }

        public string Decrypt(string encrypted)
        {
            return encoder.GetString(Decrypt(Convert.FromBase64String(encrypted)));
        }

        public byte[] Encrypt(byte[] buffer)
        {
            return Transform(buffer, encryptor);
        }

        public byte[] Decrypt(byte[] buffer)
        {
            return Transform(buffer, decryptor);
        }

        protected byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            MemoryStream stream = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }
            return stream.ToArray();
        }
    }
}