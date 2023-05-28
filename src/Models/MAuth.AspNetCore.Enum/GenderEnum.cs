using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Enum
{
   public enum GenderEnum
    {
        /// <summary>
        /// 男性
        /// </summary>
        [Description("男性")]
        MALE =0,
        /// <summary>
        /// 女性
        /// </summary>
        [Description("女性")]
        FEMALE =1,
        /// <summary>
        /// 保密
        /// </summary>
        [Description("保密")]
        SECRET = 2,
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        UNKNOWN =3
    }
}
