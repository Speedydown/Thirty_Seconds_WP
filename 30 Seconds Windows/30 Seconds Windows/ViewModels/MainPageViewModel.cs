using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Pages;
using _30_Seconds_Windows.Pages.Game;
using _30_Seconds_Windows.Pages.GameSetup;
using _30_Seconds_Windows.Pages.Settings;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task LoadData()
        {
            IsLoading = true;
            await StatusBar.GetForCurrentView().ShowAsync();
            StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
            HasCurrentGame = GameHandler.instance.GetCurrentGame() != null && GameHandler.instance.GetCurrentGame().TimeStarted != DateTime.MinValue;
            IsLoading = false;
        }

        public void CurrentGameButton()
        {
            (Window.Current.Content as Frame).Navigate(typeof(VersusIntroPage));
        }

        public void NewGameButton()
        {
            if (SettingsHandler.instance.UpdateTask == null || (SettingsHandler.instance.UpdateTask.IsCompleted && WordHandler.instance.DatabaseHasWords()))
            {
                GameHandler.instance.StartNewGame();

                (Window.Current.Content as Frame).Navigate(typeof(TeamsPage));
            }
            else
            {
                (Window.Current.Content as Frame).Navigate(typeof(SplashPage));
            }
        }

        public void RulesButton()
        {
            (Window.Current.Content as Frame).Navigate(typeof(RulesPage));
        }

        public void SettingsButton()
        {
            (Window.Current.Content as Frame).Navigate(typeof(SettingsPage));
        }

        public void PrivacyPolicyButton()
        {
            (Window.Current.Content as Frame).Navigate(typeof(Privacy_Policy));
        }
    }
}
