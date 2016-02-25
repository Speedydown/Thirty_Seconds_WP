using _30_Seconds_Windows.ViewModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.ViewModels.GameAnimations
{
    public class FiveStarAnimationPageViewModel : GameViewModel
    {
        public static readonly FiveStarAnimationPageViewModel instance = new FiveStarAnimationPageViewModel();

        private FiveStarAnimationPageViewModel() : base()
        {

        }

        public async Task Load()
        {
            IsLoading = true;
            NavigatedTo();
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
            
        }
    }
}
