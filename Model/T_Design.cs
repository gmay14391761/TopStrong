using Easy4net.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Entity
{
    [Table(Name = "T_Design")]
    public class T_Design
    {
        [Column(Name = "ID")]
        public string ID { get; set; }

        [Id(Name = "AID", Strategy = GenerationType.INDENTITY)]
        public int? AID { get; set; }

        [Column(Name = "DELETED")]
        public int? DELETED { get; set; }

        [Column(Name = "CREATEDATE")]
        public DateTime CREATEDATE { get; set; }

        /// <summary>
        /// 提交单号
        /// </summary>
        [Column(Name = "DesignNo")]
        public string DesignNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Column(Name = "UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Column(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 选择的项目
        /// </summary>
        [Column(Name = "Sele_Item")]
        public string Sele_Item { get; set; }

        /// <summary>
        /// 是否回访
        /// </summary>
        [Column(Name = "IsReturn")]
        public int IsReturn { get; set; }
    }
}

