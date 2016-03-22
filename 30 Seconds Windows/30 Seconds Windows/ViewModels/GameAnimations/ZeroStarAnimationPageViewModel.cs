using _30_Seconds_Windows.Pages.Game;
using _30_Seconds_Windows.ViewModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.IO;
using Windows.UI.ViewManagement;

namespace _30_Seconds_Windows.ViewModels.GameAnimations
{
    public class ZeroStarAnimationPageViewModel : GameViewModel
    {
        public static readonly ZeroStarAnimationPageViewModel instance = new ZeroStarAnimationPageViewModel();

        private DateTime? TimeStarted = null;
        public int AnimationAngle { get; set; }

        private ZeroStarAnimationPageViewModel()
            : base()
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

            Task t = Task.Run(async () =>
            {
                await LoadFileTask;

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    AwwStream.Position = 0;
                    MediaPlayer.SetSource(AwwStream.AsRandomAccessStream(), AwwFile.ContentType);
                    MediaPlayer.Play();
                });
            });
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

            if (MilliSecondsElapsed > 3120)
            {
                Timer.Tick -= Timer_Tick;
                Timer.Stop();
                MediaPlayer.Stop();
                (Window.Current.Content as Frame).Navigate(typeof(NextPlayerPage));

                Task t = ClearBackstack(0);
            }
        }
    }
}
