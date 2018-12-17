using Easy4net.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Entity
{
    [Table(Name = "T_News")]
    public class T_News
    {
        [Column(Name = "ID")]
        public string ID { get; set; }

        [Id(Name = "AID", Strategy = GenerationType.INDENTITY)]
        public int? AID { get; set; }

        [Column(Name = "DELETED")]
        public int? DELETED { get; set; }

        [Column(Name = "CREATEDATE")]
        public DateTime CREATEDATE { get; set; }

        [Column(Name = "NewsTitle")]
        public string NewsTitle { get; set; }

        [Column(Name = "NewsSmallTitle")]
        public string NewsSmallTitle { get; set; }

        [Column(Name = "Classifylist")]
        public string Classifylist { get; set; }

        [Column(Name = "NewsImg")]
        public string NewsImg { get; set; }

        [Column(Name = "NewsType")]
        public string NewsType { get; set; }

        [Column(Name = "SORT")]
        public int? SORT { get; set; }

        [Column(Name = "NewsDetail")]
        public string NewsDetail { get; set; }

        [Column(Name = "NewsClickNum")]
        public int NewsClickNum { get; set; }

        [Column(Name = "Classify",IsInsert=false,IsUpdate=false)]
        public List<T_Classify> Classify { get; set; }
    }
}

