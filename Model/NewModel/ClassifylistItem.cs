using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.NewModel
{
    public class ClassifylistItem
    {
        public T_Classify T_Classify { get; set; }
        public List<T_Classify> T_ClassifyItem { get; set; }
    }
}
