using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Model.Utils;
using _30_Seconds_Windows.Pages.Settings;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.Settings
{
    public class SettingsPageViewModel : ViewModel
    {
        public static readonly SettingsPageViewModel instance = new SettingsPageViewModel();

        private bool _RemoveAdsButtonVisible;
        public bool RemoveAdsButtonVisible
        {
            get { return _RemoveAdsButtonVisible; }
            set
            {
                _RemoveAdsButtonVisible = value;
                NotifyPropertyChanged();
            }
        }


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
                return LanguageHandler.instance.Languages.OrderByDescending(l => l.LanguageID == LanguageHandler.instance.CurrentLanguage.LanguageID).ToArray();
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

        private SettingsPageViewModel()
            : base()
        {

        }

        public async Task LoadData()
        {
            IsLoading = true;
            RemoveAdsButtonVisible = !IAPHandler.instance.HasFeature(IAPHandler.RemoveAdsFeatureName);

            if (SettingsHandler.instance.UpdateTask != null)
            {
                await SettingsHandler.instance.UpdateTask;
                LanguageHandler.instance.ResetLanguages();
            }

            NotifyPropertyChanged("Languages");
            NotifyPropertyChanged("CurrentLanguage");
            IsLoading = false;
        }

        public async void WordPacksButton()
        {
            await Navigate(typeof(SettingsWordPackspage));
        }

        public async Task RemoveAdsButton()
        {
            if (!IAPHandler.instance.HasFeature(IAPHandler.RemoveAdsFeatureName))
            {
                RemoveAdsButtonVisible = false;
                IsLoading = true;
                RemoveAdsButtonVisible = !(await IAPHandler.instance.BuyProduct(IAPHandler.RemoveAdsFeatureName));

                if (!RemoveAdsButtonVisible)
                {
                    ////Create warning dialog:
                    var messageDialog = new MessageDialog(Utils.ResourceLoader.GetString("Purchase_completed_Text"), Utils.ResourceLoader.GetString("Purchase_completed_Header"));

                    messageDialog.Commands.Add(
                        new UICommand(
                            Utils.ResourceLoader.GetString("text_ok"),
                            null,
                            0));

                    IUICommand Command = await messageDialog.ShowAsync();
                }

                IsLoading = false;
            }
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

        public override void Unload()
        {
            
        }

        public override async Task Load()
        {
            
        }
    }
}
