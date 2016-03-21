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
using Windows.Storage;
using Windows.UI.ViewManagement;

namespace _30_Seconds_Windows.ViewModels.GameAnimations
{
    public class FiveStarAnimationPageViewModel : GameViewModel
    {
        public static readonly FiveStarAnimationPageViewModel instance = new FiveStarAnimationPageViewModel();

        private DateTime? TimeStarted = null;
        public Word[] PlayedWords { get; set; }
        public double AnimationFontSize { get; set; }
  
        private FiveStarAnimationPageViewModel() : base()
        {

        }

        public async Task Load()
        {
            IsLoading = true;
           // await StatusBar.GetForCurrentView().HideAsync();
            NavigatedTo();
            TimeStarted = DateTime.Now;
            AnimationFontSize = 300;
            IsLoading = false;

            Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            Timer.Tick += Timer_Tick; ;
            Timer.Start();

            Task t = Task.Run(async () =>
            {
                await LoadFileTask;

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CheerStream.Position = 0;
                    MediaPlayer.SetSource(CheerStream.AsRandomAccessStream(), CheerFile.ContentType);
                    MediaPlayer.Play();
                });
            });
        }

        public override void NavigatedFrom()
        {
           // StatusBar.GetForCurrentView().ShowAsync();
            base.NavigatedFrom();
            Timer.Tick -= Timer_Tick;
            Timer.Stop();
        }

        void Timer_Tick(object sender, object e)
        {
            int MilliSecondsElapsed = (int)DateTime.Now.Subtract(TimeStarted.Value).TotalMilliseconds;

            AnimationFontSize += 10;

            NotifyPropertyChanged("AnimationFontSize");

            if (MilliSecondsElapsed > 5500)
            {
                MediaPlayer.Stop();
                Timer.Tick -= Timer_Tick;
                Timer.Stop();
                (Window.Current.Content as Frame).Navigate(typeof(NextPlayerPage));

                Task t = ClearBackstack(0);
            }
        }
    }
}
