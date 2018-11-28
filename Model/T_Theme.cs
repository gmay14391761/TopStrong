using Easy4net.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Entity
{
    [Table(Name = "T_Theme")]
    public class T_Theme
    {
        [Column(Name = "ID")]
        public string ID { get; set; }

        [Id(Name = "AID", Strategy = GenerationType.INDENTITY)]
        public int? AID { get; set; }

        [Column(Name = "DELETED")]
        public int? DELETED { get; set; }

        [Column(Name = "CREATEDATE")]
        public DateTime CREATEDATE { get; set; }

        [Column(Name = "ThemeName")]
        public string ThemeName { get; set; }

        [Column(Name = "ThemeImg")]
        public string ThemeImg { get; set; }

        [Column(Name = "ThemeType")]
        public int ThemeType { get; set; }

        [Column(Name = "ThemeSort")]
        public int? ThemeSort { get; set; }

    }
}

