using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Pages.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.Game
{
    public class PlayerReadyPageViewModel : GameViewModel
    {
        public static readonly PlayerReadyPageViewModel instance = new PlayerReadyPageViewModel();

        public Model.Game CurrentGame { get; private set; }
        public Team CurrentTeam { get; private set; }
        public Player CurrentPlayer { get; private set; }

        private Task loadTask = null;

        private PlayerReadyPageViewModel()
            : base()
        {

        }

        public async Task Load()
        {
            IsLoading = true;

            CurrentGame = GameHandler.instance.GetCurrentGame();
            CurrentTeam = TeamHandler.instance.GetTeamByID(CurrentGame.CurrentTeamID.Value);
            CurrentPlayer = PlayerHandler.instance.GetPlayerByID(CurrentTeam.CurrentPlayerID.Value);
            NotifyPropertyChanged("CurrentPlayer");
            NotifyPropertyChanged("CurrentTeam");

            loadTask = RoundPageViewModel.instance.Get5NewWords();
            NavigatedTo();
 
            IsLoading = false;

        }

        public async Task StartRoundButton()
        {
            if (loadTask == null)
            {
                return;
            }

            if (RoundPageViewModel.instance.GetNewWordsTask == null)
            {
                 await loadTask;
            }
            loadTask = null;
            
            (Window.Current.Content as Frame).Navigate(typeof(RoundPage));
            await ClearBackstack(0);

            Task t = Task.Run(() =>
            {
                CurrentPlayer.LastRound = DateTime.Now;
                CurrentPlayer.QuestionsAnswered = CurrentPlayer.QuestionsAnswered + 5;
                CurrentTeam.Round++;

                PlayerHandler.instance.SavePlayer(CurrentPlayer);
                TeamHandler.instance.SaveTeam(CurrentTeam);
            });
        }
    }
}
