using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SharpAESCrypt;

namespace CryptShareAPI.Services
{
    public static class EncryptionService
    {
        #region Settings

        private static int _iterations = 2;
        private static int _keySize = 256;

        private static string _hash = "SHA1";
        private static string _salt = "aselrias38490a32"; // Random
        private static string _vector = "8947az34awl34kjq"; // Random

        #endregion

        public static string Encrypt(string value, string password)
        {
            return Encrypt<AesManaged>(value, password);
        }
        public static string Encrypt<T>(string value, string password)
                where T : SymmetricAlgorithm, new()
        {
            byte[] vectorBytes = Encoding.ASCII.GetBytes(_vector);
            byte[] saltBytes = Encoding.ASCII.GetBytes(_salt);
            byte[] valueBytes = Encoding.UTF8.GetBytes(value);

            byte[] encrypted;
            using (T cipher = new T())
            {
                PasswordDeriveBytes _passwordBytes =
                    new PasswordDeriveBytes(password, saltBytes, _hash, _iterations);
                byte[] keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
                {
                    using (MemoryStream to = new MemoryStream())
                    {
                        using (CryptoStream writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
                        {
                            writer.Write(valueBytes, 0, valueBytes.Length);
                            writer.FlushFinalBlock();
                            encrypted = to.ToArray();
                        }
                    }
                }
                cipher.Clear();
            }
            return Convert.ToBase64String(encrypted);
        }

        //public static string GenerateKey()
        //{
        //    var desCrypto = DES.Create();

        //    return Encoding.ASCII.GetString(desCrypto.Key);
        //}

        public static string Decrypt(string value, string password)
        {
            return Decrypt<AesManaged>(value, password);
        }
        public static string Decrypt<T>(string value, string password) where T : SymmetricAlgorithm, new()
        {
            byte[] vectorBytes = Encoding.ASCII.GetBytes(_vector);
            byte[] saltBytes = Encoding.ASCII.GetBytes(_salt);
            byte[] valueBytes = Convert.FromBase64String(value);

            byte[] decrypted;
            int decryptedByteCount = 0;

            using (T cipher = new T())
            {
                PasswordDeriveBytes _passwordBytes = new PasswordDeriveBytes(password, saltBytes, _hash, _iterations);
                byte[] keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                try
                {
                    using (ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
                    {
                        using (MemoryStream from = new MemoryStream(valueBytes))
                        {
                            using (CryptoStream reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
                            {
                                decrypted = new byte[valueBytes.Length];
                                decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return String.Empty;
                }

                cipher.Clear();
            }
            return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
        }

        //public static MemoryStream Encrypt(Stream fsInput, string sKey)
        //{
        //    var fsEncrypted = new MemoryStream();

        //    var des = new DESCryptoServiceProvider
        //    {
        //        Key = Encoding.ASCII.GetBytes(sKey),
        //        IV = Encoding.ASCII.GetBytes(sKey)
        //    };
        //    var desencrypt = des.CreateEncryptor();
        //    var cryptostream = new CryptoStream(fsEncrypted, desencrypt, CryptoStreamMode.Write);

        //    var bytearrayinput = new byte[fsInput.Length];
        //    fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
        //    cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
        //    cryptostream.FlushFinalBlock();
        //    fsInput.Close();

        //    fsEncrypted.Flush();
        //    fsEncrypted.Position = 0;
        //    return fsEncrypted;
        //}

        //public static MemoryStream Decrypt(Stream fsread, string sKey)
        //{
        //    var des = new DESCryptoServiceProvider
        //    {
        //        Key = Encoding.ASCII.GetBytes(sKey),
        //        IV = Encoding.ASCII.GetBytes(sKey)
        //    };

        //    //des.Padding = PaddingMode.Zeros;

        //    var sOutputFilename = new MemoryStream();
        //    var desdecrypt = des.CreateDecryptor();
        //    var cryptostreamDecr = new CryptoStream(fsread, desdecrypt, CryptoStreamMode.Read);

        //    var bytearrayinput = new byte[fsread.Length];
        //    sOutputFilename.Read(bytearrayinput, 0, bytearrayinput.Length);
        //    cryptostreamDecr.Write(bytearrayinput, 0, bytearrayinput.Length);
        //    cryptostreamDecr.FlushFinalBlock();
        //    sOutputFilename.Close();

        //    sOutputFilename.Flush();
        //    sOutputFilename.Position = 0;
        //    return sOutputFilename;
        //}


        public static MemoryStream Encrypt(Stream sInputFilename,
           string sKey)
        {
            //FileStream fsInput = new FileStreasm(sInputFilename,
            //   FileMode.Open,
            //   FileAccess.Read);

            //FileStream fsEncrypted = new FileStream(sOutputFilename,
            //   FileMode.Create,
            //   FileAccess.Write);

            var fsEncrypted = new MemoryStream();

            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = DES.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(fsEncrypted,
               desencrypt,
               CryptoStreamMode.Write);

            byte[] bytearrayinput = new byte[sInputFilename.Length];
            sInputFilename.Read(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
            //cryptostream.Close();
            //sInputFilename.Close();
            //fsEncrypted.Close();
            //fsEncrypted.Position = 0;
            //return fsEncrypted;

            cryptostream.FlushFinalBlock();
            //    fsInput.Close();

            fsEncrypted.Flush();
            fsEncrypted.Position = 0;
            return fsEncrypted;
        }

        public static MemoryStream Decrypt(Stream fsread,
           string sKey)
        {
            var DES = new DESCryptoServiceProvider();
            var sOutputFilename = new MemoryStream();
            //A 64 bit key and IV is required for this provider.
            //Set secret key For DES algorithm.
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            //Set initialization vector.
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

            ////Create a file stream to read the encrypted file back.
            //FileStream fsread = new FileStream(sInputFilename,
            //   FileMode.Open,
            //   FileAccess.Read);
            //Create a DES decryptor from the DES instance.
            ICryptoTransform desdecrypt = DES.CreateDecryptor();
            //Create crypto stream set to read and do a 
            //DES decryption transform on incoming bytes.
            CryptoStream cryptostreamDecr = new CryptoStream(fsread,
               desdecrypt,
               CryptoStreamMode.Read);
            //Print the contents of the decrypted file.
            StreamWriter fsDecrypted = new StreamWriter(sOutputFilename, Encoding.ASCII);
            fsDecrypted.Write(new StreamReader(cryptostreamDecr, Encoding.ASCII).ReadToEnd());
            fsDecrypted.Flush();
            //fsDecrypted.Close();
            sOutputFilename.Position = 0;
            return sOutputFilename;
        }
    }
}
