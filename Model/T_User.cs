using Easy4net.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Table(Name = "T_User")]
    public class T_User
    {
        [Column(Name = "ID")]
        public string ID { get; set; }

        [Id(Name = "AID", Strategy = GenerationType.INDENTITY)]
        public int? AID { get; set; }

        [Column(Name = "Deleted")]
        public int? Deleted { get; set; }

        [Column(Name = "CreateDate")]
        public string CreateDate { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Column(Name = "UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [Column(Name = "Pwd")]
        public string Pwd { get; set; }

        /// <summary>
        /// 用户状态 0=可用 1=禁用
        /// </summary>
        [Column(Name = "Status")]
        public int? Status { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Column(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 登陆次数
        /// </summary>
        [Column(Name = "LoginCount")]
        public int LoginCount { get; set; }

        /// <summary>
        /// 最后登陆时间
        /// </summary>
        [Column(Name = "LastLoginDate")]
        public DateTime LastLoginDate { get; set; }

        /// <summary>
        /// 最后登陆IP
        /// </summary>
        [Column(Name = "LastLoginIp")]
        public string LastLoginIp { get; set; }
    }
}

