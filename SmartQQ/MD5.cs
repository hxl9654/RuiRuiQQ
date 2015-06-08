using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
///******************************************************
///本文作者：凌晨的搜索者
///博客地址:http://www.cnblogs.com/uu102
///本文属于原创，如果有您需要的内容想要转载，请保留此段签名注释，否则作者将保留追究法律责任的权利。
///******************************************************
namespace SmartQQ
{
    class MD5
    {
        public static string MD5_QQ_2_Encrypt(string uin, string password, string verifyCode)
        {

            ByteBuffer buffer = new ByteBuffer();
            buffer.Put(MD5_GetBytes(password));
            //buffer.Put(Encoding.UTF8.GetBytes(password));
            //buffer.PutInt(0);
            buffer.Put(MD5_GetBytes(uin));
            byte[] bytes = buffer.ToByteArray();
            string md5_1 = MD5_Encrypt(bytes);//将混合后的字节流进行一次md5加密
            string result = MD5_Encrypt(md5_1 + verifyCode.ToUpper());//再用加密后的结果与大写的验证码一起加密一次
            return result;

        }
        public static string MD5_QQ_2_Encrypt(long uin, string password, string verifyCode)
        {

            ByteBuffer buffer = new ByteBuffer();
            buffer.Put(MD5_GetBytes(password));
            //buffer.Put(Encoding.UTF8.GetBytes(password));
            buffer.PutInt(0);
            buffer.PutInt((uint)uin);
            byte[] bytes = buffer.ToByteArray();
            string md5_1 = MD5_Encrypt(bytes);//将混合后的字节流进行一次md5加密
            string result = MD5_Encrypt(md5_1 + verifyCode.ToUpper());//再用加密后的结果与大写的验证码一起加密一次
            return result;

        }
        public class ByteBuffer
        {
            private byte[] _buffer;
            /// <summary>
            /// 获取同后备存储区连接的基础流
            /// </summary>
            public Stream BaseStream;

            /// <summary>
            /// 构造函数
            /// </summary>
            public ByteBuffer()
            {
                this.BaseStream = new MemoryStream();
                this._buffer = new byte[0x10];
            }

            /// <summary>
            /// 设置当前流中的位置
            /// </summary>
            /// <param name="offset">相对于origin参数字节偏移量</param>
            /// <param name="origin">System.IO.SeekOrigin类型值,指示用于获取新位置的参考点</param>
            /// <returns></returns>
            public virtual long Seek(int offset, SeekOrigin origin)
            {
                return this.BaseStream.Seek((long)offset, origin);
            }



            /// <summary>
            /// 检测是否还有可用字节
            /// </summary>
            /// <returns></returns>
            public bool Peek()
            {
                return BaseStream.Position >= BaseStream.Length ? false : true;
            }

            /// <summary>
            /// 将整个流内容写入字节数组，而与 Position 属性无关。
            /// </summary>
            /// <returns></returns>
            public byte[] ToByteArray()
            {
                long org = BaseStream.Position;
                BaseStream.Position = 0;
                byte[] ret = new byte[BaseStream.Length];
                BaseStream.Read(ret, 0, ret.Length);
                BaseStream.Position = org;
                return ret;
            }


            #region "写流方法"
            /// <summary>
            /// 压入一个布尔值,并将流中当前位置提升1
            /// </summary>
            /// <param name="value"></param>
            public void Put(bool value)
            {
                this._buffer[0] = value ? (byte)1 : (byte)0;
                this.BaseStream.Write(_buffer, 0, 1);
            }

            /// <summary>
            /// 压入一个Byte,并将流中当前位置提升1
            /// </summary>
            /// <param name="value"></param>
            public void Put(Byte value)
            {
                this.BaseStream.WriteByte(value);
            }
            /// <summary>
            /// 压入Byte数组,并将流中当前位置提升数组长度
            /// </summary>
            /// <param name="value">字节数组</param>
            public void Put(Byte[] value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.BaseStream.Write(value, 0, value.Length);
            }
            /// <summary>
            /// Puts the int.
            /// </summary>
            /// <param name="value">The value.</param>
            public void PutInt(int value)
            {
                PutInt((uint)value);
            }
            /// <summary>
            /// 压入一个int,并将流中当前位置提升4
            /// </summary>
            /// <param name="value"></param>
            public void PutInt(uint value)
            {
                this._buffer[0] = (byte)(value >> 0x18);
                this._buffer[1] = (byte)(value >> 0x10);
                this._buffer[2] = (byte)(value >> 8);
                this._buffer[3] = (byte)value;
                this.BaseStream.Write(this._buffer, 0, 4);
            }
            /// <summary>
            /// Puts the int.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="value">The value.</param>
            public void PutInt(int index, uint value)
            {
                int pos = (int)this.BaseStream.Position;
                Seek(index, SeekOrigin.Begin);
                PutInt(value);
                Seek(pos, SeekOrigin.Begin);
            }

            #endregion

            #region "读流方法"

            /// <summary>
            /// 读取Byte值,并将流中当前位置提升1
            /// </summary>
            /// <returns></returns>
            public byte Get()
            {
                return (byte)BaseStream.ReadByte();
            }

            #endregion


        }
        /// <summary>
        /// 加密成md5字节流之后转换成文本
        /// </summary>
        /// <param name="md5_str"></param>
        /// <returns></returns>
        private static string Encrypt_1(string md5_str)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(md5_str);
            bytes = md5.ComputeHash(bytes);
            System.Text.StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in bytes)
            {
                stringBuilder.Append(@"\x");
                stringBuilder.Append(item.ToString("x2"));
            }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// 获取文本的md5字节流
        /// </summary>
        /// <param name="md5_str">需要加密成Md5d的文本</param>
        /// <returns></returns>
        private static byte[] MD5_GetBytes(string md5_str)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(md5_str);
            return md5.ComputeHash(bytes);


        }
        /// <summary>
        /// 将字节流加密
        /// </summary>
        /// <param name="md5_bytes">需要加密的字节流</param>
        /// <returns></returns>
        private static string MD5_Encrypt(byte[] md5_bytes)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();

            byte[] bytes1 = md5.ComputeHash(md5_bytes);
            System.Text.StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in bytes1)
            {
                stringBuilder.Append(item.ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString().ToUpper();

        }
        /// <summary>
        /// 一次md5加密
        ///http://www.cnblogs.com/uu102
        /// </summary>
        /// <param name="md5_str">需要加密的文本</param>
        /// <returns></returns>
        public static string MD5_Encrypt(string md5_str)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(md5_str);
            byte[] bytes1 = md5.ComputeHash(bytes);

            System.Text.StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in bytes1)
            {
                stringBuilder.Append(item.ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString().ToUpper();
        }
    }
}
