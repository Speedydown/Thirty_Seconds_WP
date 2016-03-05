using BaseLogic.DataHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.Model
{
    public class Settings : DataObject
    {
        public DateTime CategoryLastUpdated { get; set; }
        public DateTime WordLastUpdated { get; set; }
        public DateTime SettingsLastUpdated { get; set; }
        public DateTime WordPackLastUpdated { get; set; }
        public int CurrentLanguageID { get; set; }
        public int RequiredPoints { get; set; }
    }
}
