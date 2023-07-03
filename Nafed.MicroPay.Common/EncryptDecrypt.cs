using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Nafed.MicroPay.Common
{
    public class EncryptDecrypt
    {
       public static string cryptographyKey = "j2b1nr1x";  //===== same key used for both encyption and decryption of data.

        private EncryptDecrypt()
        {

        }
        public static string Decrypt(string stringToDecrypt, string sEncryptionKey)
        {
            try
            {
                stringToDecrypt = stringToDecrypt.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(stringToDecrypt);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(sEncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        stringToDecrypt = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return stringToDecrypt;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string Encrypt(string stringToEncrypt, string sEncryptionKey)
        {
            try
            {
                byte[] clearBytes = Encoding.Unicode.GetBytes(stringToEncrypt);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(sEncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        stringToEncrypt = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return stringToEncrypt;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
