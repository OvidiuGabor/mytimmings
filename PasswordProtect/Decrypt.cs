using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordProtect
{
    public class Decrypt
    {
        private static string SecurityKey = "Ocean8!#";
        public static string DecryptText(string textToDecrypt,  bool useHasing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(textToDecrypt);

            string key = SecurityKey;

            if (useHasing)
            {
                //if hashing was used get the hash code with regards to your key  
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider  

                hashmd5.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm  
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.   
            //We choose ECB(Electronic code Book)  

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)  
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                  
            tdes.Clear();
            //return the Clear decrypted TEXT  
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
