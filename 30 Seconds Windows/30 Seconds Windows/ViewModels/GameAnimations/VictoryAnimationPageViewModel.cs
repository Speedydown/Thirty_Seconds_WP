using _30_Seconds_Windows.Model;
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

            Task t = Task.Run(async () =>
            {
                await LoadFileTask;
                VictoryStream.Position = 0;

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MediaPlayer.SetSource(VictoryStream.AsRandomAccessStream(), VictoryFile.ContentType);
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
            if (DateTime.Now.Subtract(StartTime).TotalMilliseconds > 3000)
            {
                Timer.Tick -= Timer_Tick;
                Timer.Stop();
                (Window.Current.Content as Frame).Navigate(typeof(EndGamePage));

                Task t = ClearBackstack(0);
            }
        }
    }
}
