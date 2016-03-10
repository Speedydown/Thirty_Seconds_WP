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
    public class ZeroStarAnimationPageViewModel : GameViewModel
    {
        public static readonly ZeroStarAnimationPageViewModel instance = new ZeroStarAnimationPageViewModel();

        private DateTime? TimeStarted = null;
        public int AnimationAngle { get; set; }

        private ZeroStarAnimationPageViewModel() : base()
        {

        }

        public async Task Load()
        {
            IsLoading = true;
            NavigatedTo();
            TimeStarted = DateTime.Now;
            AnimationAngle = 0;
            IsLoading = false;

            Timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            Timer.Tick += Timer_Tick; ;
            Timer.Start();
        }

        public override void NavigatedFrom()
        {
            base.NavigatedFrom();
            Timer.Tick -= Timer_Tick;
            Timer.Stop();
        }

        void Timer_Tick(object sender, object e)
        {
            int MilliSecondsElapsed = (int)DateTime.Now.Subtract(TimeStarted.Value).TotalMilliseconds;

            AnimationAngle += 15;

            if (AnimationAngle > 360)
            {
                AnimationAngle -= 360;
            }

            NotifyPropertyChanged("AnimationAngle");

            if (MilliSecondsElapsed > 1250)
            {
                //TODO play sound
                Timer.Tick -= Timer_Tick;
                Timer.Stop();
                (Window.Current.Content as Frame).Navigate(typeof(NextPlayerPage));

                Task t = ClearBackstack(0);
            }
        }
    }
}
