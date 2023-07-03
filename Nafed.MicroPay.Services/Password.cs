using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Nafed.MicroPay.Services
{
    public class Password
    {
        //For Password Encryption
        public static readonly int PASSWORD_SALT = 16;

        public Password()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static string CreateSalt(int size)
        {
            // Generate a cryptographic random number using the cryptographic
            // service provider
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size]; // Size taken 16 bytes
            rng.GetBytes(buff);
            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }

        public static string CreatePasswordHash(string pwd, string salt)
        {
            string saltAndPwd = String.Concat(pwd, salt);
            HashAlgorithm hashAlgorithm = SHA512.Create();
            List<byte> pass = new List<byte>(Encoding.Unicode.GetBytes(saltAndPwd));
            string hashedPwd = Convert.ToBase64String(hashAlgorithm.ComputeHash(pass.ToArray()));
            hashedPwd = String.Concat(hashedPwd, salt);
            return hashedPwd;
        }

        public static bool VerifyPassword(string userPassword, string dbUserPassword, int saltSize)
        {
            bool passwordMatch = false;
            if (string.IsNullOrEmpty(dbUserPassword))
                return false;
            string salt = dbUserPassword.Substring(dbUserPassword.Length - CreateSalt(saltSize).Length);
            string hashedPasswordAndSalt = CreatePasswordHash(userPassword, salt);
            passwordMatch = hashedPasswordAndSalt.Equals(dbUserPassword);
            return passwordMatch;
        }

        public static bool CheckPasswordAgainstPolicy(string username, string password)
        {
            bool passwordValid = true;
            //string patthern = @"^.*(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*[\d]).*$";
            string patthern = @"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%]).{8,50})";
            //Password should contain atleast one upper case letter, one lower case letter, one digit, six characters long and should not contain username or 
            // parts of username that exceed two or more than two consecutive characters
            if (!Regex.IsMatch(password, patthern))
                passwordValid = false;
            else if (password.ToLower().Contains(username.ToLower()) || CheckPatterInString(username, password))
                passwordValid = false;
            return passwordValid;
        }

        private static bool CheckPatterInString(string username, string password)
        {
            bool passwordValid = false;

            for (int i = 0; i < username.Length - 3; i++)
            {
                if (password.ToLower().Contains(username.ToLower().Substring(i, 3)))
                {
                    passwordValid = true;
                    break;
                }
            }

            return passwordValid;
        }

        public static string DecryptStringAES(string cipherText)
        {
            var keybytes = Encoding.UTF8.GetBytes("8080808080808080");
            var iv = Encoding.UTF8.GetBytes("8080808080808080");

            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return string.Format(decriptedFromJavascript);
        }
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold  
            // the decrypted text.  
            string plaintext = null;

            // Create an RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings  
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.  
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream  
                                // and place them in a string.  
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }
    }
    public class EncryptDecryptQueryString
    {
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
