using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SurvayArm.Utility
{
    public class Encryption
    {
        private readonly Byte[] IV_64 = { 8, 7, 6, 5, 4, 3, 2, 1 };
        private readonly Byte[] KEY_64 = { 1, 2, 3, 4, 5, 6, 7, 8 };


        /// <summary>
        /// This method will encrypt a string value
        /// </summary>
        /// <param name="value">The string value to encrypt</param>
        /// <returns>Returns DES encrypted string</returns>
        public string DoEncrypt(string sValue)
        {
            try
            {
                var cryptoProvider = new DESCryptoServiceProvider();
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(KEY_64, IV_64), CryptoStreamMode.Write);
                var sw = new StreamWriter(cs);

                sw.Write(sValue);
                sw.Flush();
                cs.FlushFinalBlock();
                ms.Flush();

                // convert back to a string
                return Convert.ToBase64String(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }


        }

        /// <summary>
        /// This method will decrypt a DES encrypted string value
        /// </summary>
        /// <param name="strValue">The encrypted string value to decrypt</param>
        /// <returns>Returns DES decrypted string</returns>
        public string DoDecrypt(string sValue)
        {
            try
            {
                var cryptoProvider = new DESCryptoServiceProvider();
                Byte[] buffer = Convert.FromBase64String(sValue);
                var ms = new MemoryStream(buffer);
                var cs = new CryptoStream(ms, cryptoProvider.CreateDecryptor(KEY_64, IV_64), CryptoStreamMode.Read);
                var sr = new StreamReader(cs);

                return sr.ReadToEnd();
            }
            catch (NullReferenceException)
            {
                // log the error
                //  _logger.Error("A null reference error occurred while Decrypting value.", e);
            }
            catch (Exception)
            {
                // log the error
                // _logger.Error("An error occurred while Decrypting value.", e);
            }

            return string.Empty;
        }
    }
}
