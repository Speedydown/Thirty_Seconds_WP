using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Pages.Game;
using _30_Seconds_Windows.Pages.GameSetup;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            HasCurrentGame = GameHandler.instance.GetCurrentGame() != null && GameHandler.instance.GetCurrentGame().TimeStarted != DateTime.MinValue;
            IsLoading = false;
        }

        public async Task CurrentGameButton()
        {
            (Window.Current.Content as Frame).Navigate(typeof(VersusIntroPage));
        }

        public void NewGameButton()
        {
            Model.Game CurrentGame = GameHandler.instance.GetCurrentGame();

            if (CurrentGame == null || CurrentGame.TimeStarted != DateTime.MinValue)
            {
                GameHandler.instance.SetAllGamesToFinished();
                TeamHandler.instance.SetAllTeamsCurrentPlayerIDToNull();
                Model.Game NewGame = new Model.Game();

                GameHandler.instance.AddNewGame(NewGame);
            }

            (Window.Current.Content as Frame).Navigate(typeof(TeamsPage));
        }

        public async Task Rules()
        {

        }
    }
}
