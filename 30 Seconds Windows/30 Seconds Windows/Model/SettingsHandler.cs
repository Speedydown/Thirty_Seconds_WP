using _30_Seconds_Windows.Model.Utils;
using BaseLogic.DataHandler;
using BaseLogic.ExceptionHandler;
using BaseLogic.HtmlUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;

namespace _30_Seconds_Windows.Model
{
    public class SettingsHandler : DataHandler
    {
        public static readonly SettingsHandler instance = new SettingsHandler();

        public Settings CurrentSettings { get; set; }

        private SettingsHandler() : base()
        {
            CreateItemTable<Settings>();
            CurrentSettings = GetItems<Settings>().FirstOrDefault();

            if (CurrentSettings == null)
            {
                CurrentSettings = new Settings();

                //Default points
                CurrentSettings.RequiredPoints = 30;
            }
        }

        public bool SaveSettings()
        {
            try
            {
                SaveItem(CurrentSettings);
                return true;
            }
            catch (Exception e)
            {
                Task PostExTask = ExceptionHandler.instance.PostException(new AppException(e), (int)BaseLogic.ClientIDHandler.ClientIDHandler.AppName._30Seconds);
                return false;
            }
        }

        public async Task Update()
        {
            if (CurrentSettings.SettingsLastUpdated < DateTime.Now.AddDays(-7))
            {
                CurrentSettings.SettingsLastUpdated = DateTime.Now;
                Settings settingsFromServer = JsonConvert.DeserializeObject<Settings>(await HTTPGetUtil.GetDataAsStringFromURL(Constants.ServerAddress + "ThirtySeconds/getupdatesettings"));

                if (settingsFromServer.WordLastUpdated > CurrentSettings.WordLastUpdated)
                {
                    CurrentSettings.WordLastUpdated = DateTime.Now;
                    Task UpdateTask = Task.Run(() => WordHandler.instance.Update());
                }
                
                if (settingsFromServer.CategoryLastUpdated > CurrentSettings.CategoryLastUpdated)
                {
                    CurrentSettings.CategoryLastUpdated = DateTime.Now;
                    Task UpdateTask = Task.Run(() => CategoryHandler.instance.Update());
                }

                SaveSettings();
            }
        }
    }
}
