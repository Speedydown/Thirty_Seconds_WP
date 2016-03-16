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
using Windows.ApplicationModel;

namespace _30_Seconds_Windows.Model
{
    public class SettingsHandler : DataHandler
    {
        public static readonly SettingsHandler instance = new SettingsHandler();

        public Settings CurrentSettings { get; set; }
        public Task UpdateTask { get; set; }

        private SettingsHandler()
            : base()
        {
            CreateItemTable<Settings>();
            CurrentSettings = GetItems<Settings>().FirstOrDefault();

            if (CurrentSettings == null)
            {
                CurrentSettings = new Settings();

                //Default points
                CurrentSettings.RequiredPoints = Constants.RequiredPoints;
                CurrentSettings.CurrentLanguageID = Constants.DefaultLanguageID;

                var version = Package.Current.Id.Version;

                CurrentSettings.CurrentAppVersion = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Revision, version.Build);
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

        public void Update(bool ForceWordUpdate = false)
        {
            UpdateTask = Task.Run(async () =>
            {
                if (ForceWordUpdate)
                {
                    CurrentSettings.SettingsLastUpdated = DateTime.MinValue;
                    CurrentSettings.WordLastUpdated = DateTime.MinValue;
                }

                if ( WordPackHandler.instance.GetWordPacks().Count() == 0)
                {
                    CurrentSettings.WordPackLastUpdated = DateTime.MinValue;
                    CurrentSettings.SettingsLastUpdated = DateTime.MinValue;
                    CurrentSettings.WordLastUpdated = DateTime.MinValue;
                }

                if (CurrentSettings.SettingsLastUpdated < DateTime.Now.AddDays(-7))
                {
                    List<Task> UpdateTasks = new List<Task>();

                    CurrentSettings.SettingsLastUpdated = DateTime.Now;

                    Settings settingsFromServer = null;

                    try
                    {
                        settingsFromServer = JsonConvert.DeserializeObject<Settings>(await HTTPGetUtil.GetDataAsStringFromURL(Constants.ServerAddress + "ThirtySeconds/getupdatesettings"));
                    }
                    catch
                    {

                    }

                    if (settingsFromServer != null)
                    {
                        if (settingsFromServer.WordPackLastUpdated > CurrentSettings.WordPackLastUpdated)
                        {
                            CurrentSettings.WordPackLastUpdated = DateTime.Now;
                            await WordPackHandler.instance.Update();
                        }

                        if (settingsFromServer.WordLastUpdated > CurrentSettings.WordLastUpdated)
                        {
                            CurrentSettings.WordLastUpdated = DateTime.Now;
                            UpdateTasks.Add(Task.Run(() => WordHandler.instance.Update())); 
                        }

                        if (settingsFromServer.CategoryLastUpdated > CurrentSettings.CategoryLastUpdated)
                        {
                            CurrentSettings.CategoryLastUpdated = DateTime.Now;
                            UpdateTasks.Add(Task.Run(() => CategoryHandler.instance.Update()));
                        }

                        SaveSettings();

                        Task.WaitAll(UpdateTasks.ToArray());
                    }
                }
            });
        }

        public async Task WaitForUpdate()
        {
            if (UpdateTask != null && !UpdateTask.IsCompleted)
            {
                await UpdateTask;
            }
        }
    }
}
