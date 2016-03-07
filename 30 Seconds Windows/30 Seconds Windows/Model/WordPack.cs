using BaseLogic.DataHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.Model
{
    public class WordPack : DataObject
    {
        public string Name { get; set; }
        public int LanguageID { get; set; }
        public bool Free { get; set; }
        public DateTime Added { get; set; }
        public DateTime Retrieved { get; set; }

        private bool _Enabled;
        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                _Enabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsDefault { get; set; }
        public int WordCount { get; set; }
    }
}
