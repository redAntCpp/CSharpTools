using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HOTApi.Lib
{

    //加密类
    public class Encrypt
    {

        //----------------------------------3DES BEGIN----------------------------------------------------
        /// <summary>
        /// 3DES加密算法
        /// </summary>
        /// <param name="strContent">要加密的文本</param>
        /// <param name="strKey">对称密钥</param>
        /// <param name="encoding">编码方式，默认default</param>
        /// <returns></returns>
        public static string T_DESEEncrypt(string strContent, string strKey, Encoding encoding)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();
            DES.Key = hashMD5.ComputeHash(encoding.GetBytes(strKey));
            DES.Mode = CipherMode.ECB;
            //创建加密器
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            byte[] Buffer = encoding.GetBytes(strContent);
            //转为64为的字符串
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        //-----------------------------3DES END----------------------------------------------------------




        //-----------------------------MD5 BEGIN---------------------------------------------------------
        /// <summary>
        /// md5 加密，常常用于用户密码加密， 由于MD5是不可逆的，所以加密之后就无法解密，取用户名和密码时候，
        /// 需要再加密一边用户输入的数据与数据库中已加密的数据进行比对。如果比对结果一致，则可以判定登陆成功！
        /// </summary>
        /// <param name="strContent">待加密文本</param>
        /// <returns>返回</returns>
        public static string MD5EncryptFor16(string strContent)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(strContent)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }
        public static string MD5EncryptFor32(string strContent)
        {
            string result = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(strContent));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                result = result + s[i].ToString("X");
            }
            return result;
        }
        public static string MD5EncryptFor64(string strContent)
        {
            MD5 md5 = MD5.Create(); //实例化一个md5对像
                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(strContent));
            return Convert.ToBase64String(s);
        }
        //---------------------------------------MD5  END--------------------------------------------

        //---------------------------------------RSA  BEGIN--------------------------------------------
        //RSA加密算法，第一个参数为公钥，第二个参数为要加密的数据
        public static string RSAEncrypt(string PublicKey, string EncryptString)
        {
            string str2;
            try
            {

                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                provider.FromXmlString(PublicKey);
                byte[] bytes = Encoding.Default.GetBytes(EncryptString);
                str2 = Convert.ToBase64String(provider.Encrypt(bytes, false));
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str2;
        }

        /// <summary>
        /// 该函数用来生成RSA加密算法的私钥和公钥，两个参数分别表示私钥和公钥文件存放的路径
        /// </summary>
        /// <param name="PrivateKeyPath">私钥存放路径</param>
        /// <param name="PublicKeyPath">公钥存放路径</param>
        public void RSAKey(string PrivateKeyPath, string PublicKeyPath)
        {
            try
            {
                //首先使用RSACryptoServiceProvider provider = new RSACryptoServiceProvider();来生成RSA实现类的实例provider
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                //provider.ToXmlString()函数导出包含当前RSA对象密钥的XML字符串。true表示同时包含RSA公钥和私钥；false表示仅包含公钥。
                //下面两个自定义函数分别用来创建存放私钥和公钥的文件               
                CreateKeyXML(PrivateKeyPath, provider.ToXmlString(true));
                CreateKeyXML(PublicKeyPath, provider.ToXmlString(false));
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// 将秘钥写入到文件
        /// </summary>
        /// <param name="path">存放路径</param>
        /// <param name="ppkey">秘钥（含公钥私钥）</param>
        public void CreateKeyXML(string path, string ppkey)
        {

            try
            {

                //FileStream 构造函数 (String, FileMode) 使用指定的路径和创建模式初始化 FileStream 类的新实例
                //而FileMode有Open、Append、Create等模式
                FileStream ppkeyxml = new FileStream(path, FileMode.Create);
                //StreamWriter 构造函数 (Stream) 用 UTF-8 编码及默认缓冲区大小，为指定的流初始化 StreamWriter 类的一个新实例。
                StreamWriter sw = new StreamWriter(ppkeyxml);
                //FileStream对象表示在磁盘或网络路径上指向文件的流。这个类提供了在文件中读写字节的方法，但经常使用StreamReader
                //或StreamWriter执行这些功能。这是因为FileStream类操作的是字节和字节数组，而Stream类操作的是字符数据。
                sw.WriteLine(ppkey);
                sw.Close();
                ppkeyxml.Close();
            }
            catch (Exception exception)
            {
                throw exception;
            }

        }
        /*---------------------------------------------------------------------------*/

        /// <summary>

        /// 读文件

        /// </summary>

        /// <param name="path"></param>

        /// <returns></returns>

        public static string ReadKey(string path)
        {

            StreamReader reader = new StreamReader(path);

            string key = reader.ReadToEnd();

            reader.Close();

            return key;

        }

        //--------------------------------------RSA  END---------------------------------------


    }

    //解密类
    public class Decrypt
    {
        /// <summary>
        /// 3DES解密
        /// </summary>
        /// <param name="strContent">被加密文本</param>
        /// <param name="strKey">加密秘钥</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public static string T_DESDecrypt(string strContent, string strKey, Encoding encoding)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();

            DES.Key = hashMD5.ComputeHash(encoding.GetBytes(strKey));
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESEncrypt = DES.CreateDecryptor();
            byte[] Buffer = Convert.FromBase64String(strContent);
            return encoding.GetString(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        //RSA解密算法，第一个参数为私钥，第二个参数为要解密的数据
        public static string RSADecrypt(string PrivateKey, string DecryptString)
        {
            string str2;
            try
            {

                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                provider.FromXmlString(PrivateKey);
                //FromBase64String( ) 将 Base64 数字编码的等效字符串轮换为8位无符号整数的数组。
                byte[] rgb = Convert.FromBase64String(DecryptString);
                //Decrypt（）方法是使用 RSA 算法对数据进行解密。
                byte[] buffer2 = provider.Decrypt(rgb, false);
                str2 = Encoding.Default.GetString(buffer2);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str2;
        }


    }
}