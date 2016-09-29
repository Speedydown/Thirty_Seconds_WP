using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Pages;
using _30_Seconds_Windows.Pages.Game;
using _30_Seconds_Windows.Pages.GameSetup;
using _30_Seconds_Windows.Pages.Settings;
using BaseLogic;
using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels
{
    public class MainPageViewModel : ViewModel
    {
        public static readonly MainPageViewModel instance = new MainPageViewModel();

        private bool _HasCurrentGame = false;
        public bool HasCurrentGame
        {
            get { return _HasCurrentGame; }
            set
            {
                _HasCurrentGame = value;
                NotifyPropertyChanged();
            }
        }


        private MainPageViewModel() : base()
        {

        }

        public async void CurrentGameButton()
        {
            await Navigate(typeof(VersusIntroPage));
        }

        public async void NewGameButton()
        {
            if (SettingsHandler.instance.UpdateTask == null || (SettingsHandler.instance.UpdateTask.IsCompleted && WordHandler.instance.DatabaseHasWords()))
            {
                GameHandler.instance.StartNewGame();

                await Navigate(typeof(TeamsPage));
            }
            else
            {
                await Navigate(typeof(SplashPage));
            }
        }

        public async void RulesButton()
        {
            await Navigate(typeof(RulesPage));
        }

        public async void SettingsButton()
        {
            await Navigate(typeof(SettingsPage));
        }

        public async void PrivacyPolicyButton()
        {
            await Navigate(typeof(Privacy_Policy));
        }

        public async Task LoadData()
        {
            StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
            HasCurrentGame = GameHandler.instance.GetCurrentGame() != null && GameHandler.instance.GetCurrentGame().TimeStarted != DateTime.MinValue;
        }

        public override void Unload()
        {

        }

        public async override Task Load()
        {
            
        }
    }
}
