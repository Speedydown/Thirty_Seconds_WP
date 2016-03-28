using _30_Seconds_Windows.Model;
using BaseLogic;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.ViewModels.GameSetup
{
    public class PlayerPageViewModel : ViewModel
    {
        public static readonly PlayerPageViewModel instance = new PlayerPageViewModel();

        private Player _CurrentPlayer;
        public Player CurrentPlayer
        {
            get { return _CurrentPlayer; }
            set { _CurrentPlayer = value; }
        }
        

        private PlayerPageViewModel() : base()
        {

        }

        public async Task LoadData()
        {
            IsLoading = true;
            IsLoading = false;
        }

        public void SavePlayer()
        {
            Player CurrentPlayer = this.CurrentPlayer;

            if (CurrentPlayer.Name.Length < 3)
            {
                CurrentPlayer.Name = "Player" + CurrentPlayer.InternalID;
            }

            Task.Run(() =>
                {
                    PlayerHandler.instance.SavePlayer(CurrentPlayer);
                });
        }
    }
}
