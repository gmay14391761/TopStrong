using Easy4net.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Entity
{
    [Table(Name = "T_Adv")]
    public class T_Adv
    {
        [Column(Name = "ID")]
        public string ID { get; set; }

        [Id(Name = "AID", Strategy = GenerationType.INDENTITY)]
        public int? AID { get; set; }

        [Column(Name = "DELETED")]
        public int? DELETED { get; set; }

        [Column(Name = "CREATEDATE")]
        public DateTime CREATEDATE { get; set; }

        [Column(Name = "AdvTitle")]
        public string AdvTitle { get; set; }

        [Column(Name = "AdvImg")]
        public string AdvImg { get; set; }

        [Column(Name = "AdvType")]
        public int AdvType { get; set; }

        [Column(Name = "SORT")]
        public int? SORT { get; set; }

        [Column(Name = "AdvDetail")]
        public string AdvDetail { get; set; }

    }
}

