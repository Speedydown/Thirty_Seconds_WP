using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Model.Utils;
using _30_Seconds_Windows.Pages.GameSetup;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.GameSetup
{
    public class PlayersPageViewModel : ViewModel
    {
        public static readonly PlayersPageViewModel instance = new PlayersPageViewModel();

        private Team _CurrentTeam;
        public Team CurrentTeam
        {
            get { return _CurrentTeam; }
            set { _CurrentTeam = value; }
        }

        private ObservableCollection<Player> _ExistingPlayers;
        public ObservableCollection<Player> ExistingPlayers
        {
            get { return _ExistingPlayers; }
            set
            {
                _ExistingPlayers = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ExistingPlayersVisible");
            }
        }

        public bool ExistingPlayersVisible
        {
            get
            {
                return ExistingPlayers != null && ExistingPlayers.Count > 0;
            }
        }

        public bool NoPlayersInTeamMessageVisiable
        {
            get
            {
                return CurrentTeam == null || CurrentTeam.Players.Count == 0;
            }
        }

        private PlayersPageViewModel()
            : base()
        {

        }

        public async Task LoadData()
        {
            IsLoading = true;
            ExistingPlayers = new ObservableCollection<Player>(PlayerHandler.instance.GetPlayersFromDatabase());

            //Delete players in teams from existing player list
            foreach (Team t in TeamHandler.instance.GetTeamsFromDatabase())
            {
                foreach (Player p in t.Players)
                {
                    if (ExistingPlayers.SingleOrDefault(ep => ep == p) != null)
                    {
                        ExistingPlayers.Remove(p);
                    }
                }
            }

            ExistingPlayers.CollectionChanged += ExistingPlayers_CollectionChanged;
            CurrentTeam.Players.CollectionChanged += ExistingPlayers_CollectionChanged;
        }

        void ExistingPlayers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("CurrentTeam");
            NotifyPropertyChanged("ExistingPlayersVisible");
            NotifyPropertyChanged("NoPlayersInTeamMessageVisiable");
        }

        public void AddNewPlayerButton()
        {
            List<Player> Players = PlayerHandler.instance.GetPlayersFromDatabase();

            Player NewPlayer = new Player()
            {
                Name = "Player" + (Players.Count > 0 ? Players.Max(p => p.InternalID) + 1 : 1),
                GameID = GameHandler.instance.GetCurrentGame().InternalID,
                TeamID = CurrentTeam.InternalID,
                GamesPlayed = 0,
            };

            PlayerHandler.instance.SavePlayer(NewPlayer);

            CurrentTeam.Players.Add(NewPlayer);
        }

        public void EditPlayerButton(Player player)
        {
            PlayerPageViewModel.instance.CurrentPlayer = player;
            (Window.Current.Content as Frame).Navigate(typeof(PlayerPage));
        }

        public void AddExistingPlayerButton(Player ExistingPlayer)
        {
            CurrentTeam.Players.Add(ExistingPlayer);
            ExistingPlayers.Remove(ExistingPlayer);

            ExistingPlayer.TeamID = CurrentTeam.InternalID;
            ExistingPlayer.GameID = GameHandler.instance.GetCurrentGame().InternalID;
            Task.Run(() =>
                {
                    PlayerHandler.instance.SavePlayer(ExistingPlayer);
                });
        }

        public async Task DeletePlayerButton(Player PlayerToDelete)
        {
            if (PlayerToDelete.GameID != CurrentTeam.GameID)
            {
                ////Create warning dialog:
                var messageDialog = new MessageDialog(string.Format(Utils.ResourceLoader.GetString("text_DeletePlayerQuestion_Body"), PlayerToDelete.Name), Utils.ResourceLoader.GetString("text_DeletePlayerQuestion_Title"));

                messageDialog.Commands.Add(
                    new UICommand(
                        Utils.ResourceLoader.GetString("text_Delete"),
                        null,
                        0));
                messageDialog.Commands.Add(
                     new UICommand(
                        Utils.ResourceLoader.GetString("text_Cancel"),
                        null,
                        1));

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 0;

                // Set the command to be invoked when escape is pressed
                messageDialog.CancelCommandIndex = 1;

                IUICommand Command = await messageDialog.ShowAsync();

                if ((int)Command.Id == 0)
                {
                    PlayerHandler.instance.DeletePlayer(PlayerToDelete);
                    ExistingPlayers.Remove(PlayerToDelete);
                }
            }
            else
            {
                CurrentTeam.Players.Remove(PlayerToDelete);
                ExistingPlayers.Add(PlayerToDelete);

                PlayerToDelete.GameID = 0;
                PlayerToDelete.TeamID = 0;
                PlayerHandler.instance.SavePlayer(PlayerToDelete);
            }
        }

        public async Task SaveTeam()
        {
            Team CurrentTeam = this.CurrentTeam;

            if (CurrentTeam.Name.Length == 0)
            {
                CurrentTeam.Name = "Team" + CurrentTeam.InternalID;
            }

            await Task.Run(() =>
                {
                    TeamHandler.instance.SaveTeam(CurrentTeam);
                });
        }
    }
}
