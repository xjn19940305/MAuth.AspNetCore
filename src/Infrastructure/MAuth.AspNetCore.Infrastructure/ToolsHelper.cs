using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Infrastructure
{
    public static class ToolsHelper
    {
        public static string GetDescription(this Enum enumName)
        {
            string description;
            FieldInfo fieldInfo = enumName.GetType().GetField(enumName.ToString());
            DescriptionAttribute[] attributes = fieldInfo.GetCustomAttributes<DescriptionAttribute>().ToArray();
            if (attributes != null && attributes.Length > 0)
                description = attributes[0].Description;
            else
                return string.Empty;
            return description;
        }

        public static string GenerateOrderNo(string Prefix = "mauth_")
        {
            string date = DateTime.Now.ToString("yyyyMMddHHmmss");
            string random = new Random(Guid.NewGuid().GetHashCode()).Next(10000, 99999).ToString();
            string orderNo = $"{Prefix}{date}{random}";
            return orderNo;
        }
        public static long GenerateTimeStamp()
        {
            return ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
        }
        public static string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString("n");
        }
        /// <summary>
        /// 进行MD5加密
        /// </summary>
        /// <param name="md5"></param>
        /// <returns></returns>
        public static string Get32Md5(this string md5)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] value, hash;
            value = System.Text.Encoding.UTF8.GetBytes(md5);
            hash = md.ComputeHash(value);
            md.Clear();
            string temp = "";
            for (int i = 0, len = hash.Length; i < len; i++)
            {
                temp += hash[i].ToString("X").PadLeft(2, '0');
            }
            return temp;
        }
    }
}
