using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Pages.Game;
using _30_Seconds_Windows.ViewModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.GameAnimations
{
    public class VictoryAnimationPageViewModel : GameViewModel
    {
        public static readonly VictoryAnimationPageViewModel instance = new VictoryAnimationPageViewModel();

        public Team WinningTeam { get; set; }
        public DateTime StartTime { get; set; }

        private VictoryAnimationPageViewModel()
            : base()
        {

        }

        public async Task Load()
        {
            IsLoading = true;
            StartTime = DateTime.Now;
            NavigatedTo();
            IsLoading = false;

            Timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            Timer.Tick += Timer_Tick; ;
            Timer.Start();

            NotifyPropertyChanged("WinningTeam");
        }

        public override void NavigatedFrom()
        {
            base.NavigatedFrom();
            Timer.Tick -= Timer_Tick;
            Timer.Stop();
        }

        void Timer_Tick(object sender, object e)
        {
            if (DateTime.Now.Subtract(StartTime).TotalMilliseconds > 2500)
            {
                //TODO play sound
                Timer.Tick -= Timer_Tick;
                Timer.Stop();
                (Window.Current.Content as Frame).Navigate(typeof(EndGamePage));

                Task t = ClearBackstack(0);
            }
        }
    }
}
