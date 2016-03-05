using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WZWVAPI;

namespace _30SecondsCore.Model
{
    public class SettingsHandler : DataHandler
    {
        private static readonly Field CategoryLastUpdatedField = new Field("CategoryLastUpdated", typeof(DateTime), 1);
        private static readonly Field WordLastUpdatedField = new Field("WordLastUpdated", typeof(DateTime), 1);
        private static readonly Field SettingsLastUpdatedField = new Field("SettingsLastUpdated", typeof(DateTime), 1);
        private static readonly Field WordPackLastUpdatedField = new Field("WordPackLastUpdated", typeof(DateTime), 1);

        public static readonly SettingsHandler instance = new SettingsHandler();

        public Settings CurrentSettings { get; set; }

        private SettingsHandler() : base("Settings", new Field[]
            {
                CategoryLastUpdatedField,
                WordLastUpdatedField,
                SettingsLastUpdatedField,
                WordPackLastUpdatedField
            }, typeof(Settings))
        {
            CurrentSettings = base.GetObjectList(0, OrderBy.ASC, IDField).Cast<Settings>().FirstOrDefault();

            if (CurrentSettings == null)
            {
                CurrentSettings = new Settings(0, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);

                if (CurrentSettings.ID == 0)
                {
                    try
                    {
                        AddObject(CurrentSettings);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public bool SaveSettings()
        {
            try
            {
                CurrentSettings.SettingsLastUpdated = DateTime.Now;
                UpdateObject(CurrentSettings);
                return true;
            }
            catch (Exception e)
            {
                new WebsiteException(e, ErrorOrigin.Website, this.ToString());
                return false;
            }
        }
    }
}
