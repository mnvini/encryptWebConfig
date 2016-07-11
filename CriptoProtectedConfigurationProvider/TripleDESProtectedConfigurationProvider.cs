using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace CriptoProtectedConfigurationProvider
{
    public class TripleDESProtectedConfigurationProvider : ProtectedConfigurationProvider
    {
        private TripleDESCryptoServiceProvider tripleDesCripto = new TripleDESCryptoServiceProvider();
        private string key = "Sgo0k8dv112TCU0BiMNOo0tfR5AYmXeC";
        private string IV = "mmVbNIIj8k4=";

        public TripleDESProtectedConfigurationProvider()
        {
            //Read the key and IV
            tripleDesCripto.Key = Convert.FromBase64String(key);
            tripleDesCripto.IV = Convert.FromBase64String(IV);
            //ECB(Electronic code Book)

            tripleDesCripto.Mode = CipherMode.ECB;
            //padding mode if have any additional byte to be added
            tripleDesCripto.Padding = PaddingMode.PKCS7;
        }


        public override XmlNode Encrypt(XmlNode node)
        {

            string encryptedData = EncryptString(node.OuterXml);

            //Create section, or Web Config Encrypted
                        XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml("<EncryptedData>" + encryptedData + "</EncryptedData>");
            return xmlDoc.DocumentElement;
        }


        public override XmlNode Decrypt(XmlNode encryptedNode)
        {
            string decryptedData = DecryptString(encryptedNode.InnerText);

            // Decrypts the section or the Web Config Encrypted
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(decryptedData);

            return xmlDoc.DocumentElement;
        }

        /// <summary>
        /// Method for Encrypting
        /// </summary>
        private string EncryptString(string encryptValue)
        {

            byte[] valBytes = Encoding.Unicode.GetBytes(encryptValue);

            ICryptoTransform transform = tripleDesCripto.CreateEncryptor();

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
            cs.Write(valBytes, 0, valBytes.Length);
            cs.FlushFinalBlock();

            byte[] returnBytes = ms.ToArray();
            cs.Close();

            return Convert.ToBase64String(returnBytes);
        }

        /// <summary>
        /// Method for Decrypt
        /// </summary>
        private string DecryptString(string encryptedValue)
        {
            byte[] valBytes = Convert.FromBase64String(encryptedValue);

            ICryptoTransform transform = tripleDesCripto.CreateDecryptor();

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
            cs.Write(valBytes, 0, valBytes.Length);
            cs.FlushFinalBlock();
            byte[] returnBytes = ms.ToArray();
            cs.Close();

            return Encoding.Unicode.GetString(returnBytes);
        }
    }
}
