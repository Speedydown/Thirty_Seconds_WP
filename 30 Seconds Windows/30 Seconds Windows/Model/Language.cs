using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.Model
{
    public class Language
    {
        public int LanguageID { get; set; }
        public string Name { get; set; }
        public int NumberOfWordPacks { get; set; }
        public bool Enabled { get; set; }
    }
}
