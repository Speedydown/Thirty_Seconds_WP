using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Pages.Settings;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.Settings
{
    public class SettingsPageViewModel : ViewModel
    {
        public static readonly SettingsPageViewModel instance = new SettingsPageViewModel();

        public Language CurrentLanguage
        {
            get
            {
                return LanguageHandler.instance.CurrentLanguage;
            }
        }

        public Language[] Languages
        {
            get
            {
                return LanguageHandler.instance.Languages;
            }
        }

        public int CurrentPointsIndex
        {
            get
            {
                return PossiblePoints.ToList().IndexOf(SettingsHandler.instance.CurrentSettings.RequiredPoints);
            }
        }

        public int[] PossiblePoints
        {
            get
            {
                return new int[] { 20, 30, 40, 50, 60 };
            }
        }

        private SettingsPageViewModel() : base()
        {

        }

        public async Task load()
        {
            IsLoading = true;
            if (SettingsHandler.instance.UpdateTask != null)
            {
                await SettingsHandler.instance.UpdateTask;
                LanguageHandler.instance.ResetLanguages();
            }

            NotifyPropertyChanged("Languages");
            NotifyPropertyChanged("CurrentLanguage");
            IsLoading = false;
        }

        public void WordPacksButton()
        {
            (Window.Current.Content as Frame).Navigate(typeof(SettingsWordPackspage));
        }

        public void RemoveAdsButton()
        {

        }

        public void NavigateFrom(Language SelectedLanguage, int PointsIndex)
        {
            if (SelectedLanguage != null && SelectedLanguage != CurrentLanguage)
            {
                LanguageHandler.instance.ChangeLanguage(SelectedLanguage.LanguageID);
            }

            if (PointsIndex != CurrentPointsIndex)
            {
                SettingsHandler.instance.CurrentSettings.RequiredPoints = PossiblePoints[PointsIndex];
                SettingsHandler.instance.SaveSettings();
            }
        }

    }
}
