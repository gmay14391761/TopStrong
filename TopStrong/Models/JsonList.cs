using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TopStrong.Models
{
    public class JsonList<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// JSON集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> list { get; set; }
    }
}