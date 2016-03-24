using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Model.Utils;
using _30_Seconds_Windows.Pages.Game;
using _30_Seconds_Windows.Pages.GameSetup;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.GameSetup
{
    public class TeamsPageViewModel : ViewModel
    {
        public static readonly TeamsPageViewModel instance = new TeamsPageViewModel();

        private Model.Game _CurrentGame;
        public Model.Game CurrentGame
        {
            get { return _CurrentGame; }
            set
            {
                _CurrentGame = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("NoTeamsInGameMessageVisible");
                NotifyPropertyChanged("PlayGameButtonEnabled");
            }
        }

        private ObservableCollection<Team> _ExistingTeams;
        public ObservableCollection<Team> ExistingTeams
        {
            get { return _ExistingTeams; }
            set
            {
                _ExistingTeams = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ExistingTeamsVisible");
            }
        }

        public bool NoTeamsInGameMessageVisible
        {
            get
            {
                return CurrentGame == null || CurrentGame.Teams.Count == 0;
            }
        }

        public bool PlayGameButtonEnabled
        {
            get
            {
                return CurrentGame != null &&
                    CurrentGame.Teams.Count > 1 &&
                    CurrentGame.Teams.Where(t => t.Players.Count > 1).Count() > 1 &&
                    CurrentGame.Teams.Where(t => t.Players.Count < 2).Count() == 0;
            }

        }

        public bool ExistingTeamsVisible
        {
            get
            {
                return ExistingTeams != null && ExistingTeams.Count > 0;
            }
        }

        private TeamsPageViewModel()
            : base()
        {

        }

        public async Task LoadData()
        {
            IsLoading = true;
            CurrentGame = GameHandler.instance.GetCurrentGame();
            ExistingTeams = new ObservableCollection<Team>(TeamHandler.instance.GetTeamsFromDatabase());
            ExistingTeams.CollectionChanged += Teams_CollectionChanged;
            CurrentGame.Teams.CollectionChanged += Teams_CollectionChanged;

            if (ExistingTeams.Count == 0 && CurrentGame.Teams.Count == 0)
            {
                List<Team> NewTeams = new List<Team>();

                for (int i = 0; i < 2; i++)
                {
                    NewTeams.Add(new Team()
                    {
                        Name = "Team" + (i + 1),
                        Players = new ObservableCollection<Player>(),
                        Points = 0
                    });
                }

                TeamHandler.instance.SaveTeams(NewTeams);
                ExistingTeams = new ObservableCollection<Team>(NewTeams);
            }

            for (int i = 0; i < ExistingTeams.Count; i++)
            {
                if (CurrentGame.Teams.Where(team => ExistingTeams[i].InternalID == team.InternalID).Count() > 0)
                {
                    ExistingTeams.RemoveAt(i);
                    i--;
                }
            }

            IsLoading = false;
        }

        void Teams_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("ExistingTeamsVisible");
            NotifyPropertyChanged("NoTeamsInGameMessageVisible");
            NotifyPropertyChanged("PlayGameButtonEnabled");
        }

        public void AddTeamToGameButton(Team team)
        {
            CurrentGame.Teams.Add(team);
            ExistingTeams.Remove(team);

            team.GameID = CurrentGame.InternalID;

            foreach (Player p in team.Players)
            {
                p.GameID = CurrentGame.InternalID;
            }

            Task.Run(() =>
            {
                PlayerHandler.instance.SavePlayers(team.Players.ToArray());
                TeamHandler.instance.SaveTeam(team);
            });
        }

        public void AddNewTeamToGameButton()
        {
            int TeamCount = TeamHandler.instance.GetTeamsFromDatabase().Count > 0 ? (TeamHandler.instance.GetTeamsFromDatabase().Max(t => t.InternalID) + 1) : 1;

            Team NewTeam = new Team()
            {
                GameID = CurrentGame.InternalID,
                Players = new ObservableCollection<Player>(),
                Points = 0,
                Name = "Team" + TeamCount
            };

            CurrentGame.Teams.Add(NewTeam);
            TeamHandler.instance.SaveTeam(NewTeam);
        }

        public async Task StartGameButton()
        {
            CurrentGame.TimeStarted = DateTime.Now;
            Task task = Task.Run(() =>
            {
                GameHandler.instance.SaveGame(CurrentGame);
            });

            List<Player> playersToSave = new List<Player>();

            foreach (Team t in CurrentGame.Teams)
            {
                foreach (Player p in t.Players)
                {
                    playersToSave.Add(p);
                    p.GamesPlayed++;
                }
            }

            (Window.Current.Content as Frame).Navigate(typeof(VersusIntroPage));
            task = Task.Run(() =>
            {
                PlayerHandler.instance.SavePlayers(playersToSave.ToArray());
            });
        }

        public async Task DeleteTeamButton(Team TeamToDelete)
        {
            if (TeamToDelete.GameID != CurrentGame.InternalID)
            {
                ////Create warning dialog:
                var messageDialog = new MessageDialog(string.Format(Utils.ResourceLoader.GetString("text_DeleteTeamQuestion_Body"), TeamToDelete.Name), Utils.ResourceLoader.GetString("text_DeleteTeamQuestion_Title"));

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
                    TeamHandler.instance.DeleteTeam(TeamToDelete);
                    ExistingTeams.Remove(TeamToDelete);
                }
            }
            else
            {
                CurrentGame.Teams.Remove(TeamToDelete);
                ExistingTeams.Add(TeamToDelete);

                TeamToDelete.GameID = 0;
                TeamHandler.instance.SaveTeam(TeamToDelete);
            }
        }

        public void EditTeamButton(Team team)
        {
            PlayersPageViewModel.instance.CurrentTeam = team;
            (Window.Current.Content as Frame).Navigate(typeof(PlayersPage));
        }
    }
}
