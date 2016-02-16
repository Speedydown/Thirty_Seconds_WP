using _30_Seconds_Windows.Model;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private PlayersPageViewModel()
            : base()
        {

        }

        public async Task LoadData()
        {
            IsLoading = true;
            ExistingPlayers = new ObservableCollection<Player>(PlayerHandler.instance.GetPlayersFromDatabase());
            ExistingPlayers.CollectionChanged += ExistingPlayers_CollectionChanged;
            CurrentTeam.Players.CollectionChanged += ExistingPlayers_CollectionChanged;
        }

        void ExistingPlayers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("CurrentTeam");
            NotifyPropertyChanged("ExistingPlayersVisible");
        }

        public void AddNewPlayerButton()
        {
            Player NewPlayer = new Player()
            {
                Name = "Player" + (PlayerHandler.instance.GetPlayersFromDatabase().Max(p => p.InternalID) + 1),
                GameID = GameHandler.instance.GetCurrentGame().InternalID,
                TeamID = CurrentTeam.InternalID,
                GamesPlayed = 0,
            };

            PlayerHandler.instance.SavePlayer(NewPlayer);

            CurrentTeam.Players.Add(NewPlayer);
        }
    }
}
