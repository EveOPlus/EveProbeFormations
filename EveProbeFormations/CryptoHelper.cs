using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EveProbeFormations
{
    public static class CryptoHelper
    {
        // We're going to slap security in the face instead with a "super secure" hard coded value, that is totally not a no no.
        private const string KeyPlainText = "CCPlease give in game export....";
        private const string IVPlainText = "I love patterns.";

        private static byte[] Key => Encoding.UTF8.GetBytes(KeyPlainText);
        private static byte[] IV => Encoding.UTF8.GetBytes(IVPlainText);

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) throw new ArgumentNullException(nameof(plainText));

            using (var aes = Aes.Create()) 
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                    sw.Flush();
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) throw new ArgumentNullException(nameof(cipherText));

            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                { 
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
