using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WZWVAPI;

namespace _30SecondsCore.Model
{
    public class Settings : DataObject
    {
        public DateTime CategoryLastUpdated { get; set; }
        public DateTime WordLastUpdated { get; set; }
        public DateTime SettingsLastUpdated { get; set; }
        public DateTime WordPackLastUpdated { get; set; }

        public Settings(int ID, DateTime CategoryLastUpdated, DateTime WordLastUpdated, DateTime SettingsLastUpdated, DateTime WordPackLastUpdated)
            : base(ID)
        {
            this.CategoryLastUpdated = CategoryLastUpdated;
            this.WordLastUpdated = WordLastUpdated;
            this.SettingsLastUpdated = SettingsLastUpdated;
            this.WordPackLastUpdated = WordPackLastUpdated;
        }
    }
}
