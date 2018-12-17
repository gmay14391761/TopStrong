using Easy4net.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Entity
{
    [Table(Name = "T_Classify")]
    public class T_Classify
    {
        [Column(Name = "ID")]
        public string ID { get; set; }

        [Id(Name = "AID", Strategy = GenerationType.INDENTITY)]
        public int? AID { get; set; }

        [Column(Name = "DELETED")]
        public int? DELETED { get; set; }

        [Column(Name = "CREATEDATE")]
        public DateTime CREATEDATE { get; set; }

        [Column(Name = "ClassifyName")]
        public string ClassifyName { get; set; }

        [Column(Name = "NewsType")]
        public string NewsType { get; set; }

        [Column(Name = "Pid")]
        public int Pid { get; set; }

        [Column(Name = "PidItem")]
        public int PidItem { get; set; }

        [Column(Name = "SORT")]
        public int? SORT { get; set; }

    }
}

