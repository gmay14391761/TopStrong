using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.NewModel
{
    public class ThemeNews
    {
        public string ThemeName { get; set; }
        public List<T_News> News { get; set; }
    }
}
