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
            }
        }

        private TeamsPageViewModel() : base()
        {

        }

        public async Task LoadData()
        {
            IsLoading = true;
            CurrentGame = GameHandler.instance.GetCurrentGame();
            IsLoading = false;
        }
    }
}
