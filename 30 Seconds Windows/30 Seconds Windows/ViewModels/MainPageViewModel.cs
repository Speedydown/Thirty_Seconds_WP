using _30_Seconds_Windows.Model;
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
            HasCurrentGame = GameHandler.instance.GetCurrentGame() != null;
            IsLoading = false;
        }

        public async Task CurrentGameButton()
        {

        }

        public void NewGameButton()
        {
            GameHandler.instance.SetAllGamesToFinished();

            Game NewGame = new Game();
            NewGame.Teams = new List<Team>();
            NewGame.Finished = false;

            for (int i = 0; i < 2; i++)
            {
                NewGame.Teams.Add(new Team()
                {
                    Name = "Team" + (NewGame.Teams.Count + 1),
                    Players = new List<Player>(),
                    Points = 0
                });
            }

            GameHandler.instance.AddNewGame(NewGame);

            (Window.Current.Content as Frame).Navigate(typeof(TeamsPage));
        }

        public async Task Rules()
        {

        }
    }
}
