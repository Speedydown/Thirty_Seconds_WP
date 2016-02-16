using _30_Seconds_Windows.Model;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.ViewModels.GameSetup
{
    public class TeamsPageViewModel : ViewModel
    {
        public static readonly TeamsPageViewModel instance = new TeamsPageViewModel();

        private Game _CurrentGame;
        public Game CurrentGame
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

        private List<Team> _ExistingTeams;
        public List<Team> ExistingTeams
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
                return CurrentGame != null && CurrentGame.Teams.Count > 2 && CurrentGame.Teams.Where(t => t.Players.Count > 2).Count() > 2;
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
            ExistingTeams = TeamHandler.instance.GetTeamsFromDatabase();

            if (ExistingTeams.Count == 0)
            {
                List<Team> NewTeams = new List<Team>();

                for (int i = 0; i < 2; i++)
                {
                    NewTeams.Add(new Team()
                    {
                        Name = "Team" + (i + 1),
                        Players = new List<Player>(),
                        Points = 0
                    });
                }

                TeamHandler.instance.SaveTeams(NewTeams);
                ExistingTeams = NewTeams;
            }

            IsLoading = false;
        }
    }
}
