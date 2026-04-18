using System;
using System.Security.Cryptography;
using System.Text;

namespace SmartColor.My_Tool
{
    /// <summary>
    /// 提供基于 AES 算法的加密与解密功能（ECB 模式，PKCS7 填充）。
    /// </summary>
    public class AES
    {
        // 加密密钥，长度需为 16 字节（128 位），与 AES-128 匹配
        private static  readonly string code = "20230610HelloDog";

        /// <summary>
        /// 使用 AES 算法对字符串进行加密，返回 Base64 编码的密文。
        /// </summary>
        /// <param name="str">待加密的明文字符串</param>
        /// <returns>加密后的 Base64 字符串，若输入为空则返回 null</returns>
        public static  string AesEncrypt(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;

            // 将明文字符串转换为字节数组
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            // 创建 RijndaelManaged（AES）对象并设置参数
            using (RijndaelManaged rm = new RijndaelManaged())
            {
                rm.Key = Encoding.UTF8.GetBytes(code);
                rm.Mode = CipherMode.ECB; // 电子密码本模式（不安全，建议实际项目使用 CBC）
                rm.Padding = PaddingMode.PKCS7;

                // 创建加密器并执行加密
                ICryptoTransform encryptor = rm.CreateEncryptor();
                byte[] resultArray = encryptor.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                // 返回 Base64 编码的密文
                return Convert.ToBase64String(resultArray);
            }
        }

        /// <summary>
        /// 使用 AES 算法对 Base64 编码的密文进行解密，返回明文字符串。
        /// </summary>
        /// <param name="str">待解密的 Base64 字符串</param>
        /// <returns>解密后的明文字符串，若输入为空则返回 null</returns>
        public static  string AesDecrypt(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;

            // 将 Base64 字符串转换为字节数组
            byte[] toDecryptArray = Convert.FromBase64String(str);

            // 创建 RijndaelManaged（AES）对象并设置参数
            using (RijndaelManaged rm = new RijndaelManaged())
            {
                rm.Key = Encoding.UTF8.GetBytes(code);
                rm.Mode = CipherMode.ECB;
                rm.Padding = PaddingMode.PKCS7;

                // 创建解密器并执行解密
                ICryptoTransform decryptor = rm.CreateDecryptor();
                byte[] resultArray = decryptor.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);

                // 返回解密后的明文字符串
                return Encoding.UTF8.GetString(resultArray);
            }
        }
    }
}