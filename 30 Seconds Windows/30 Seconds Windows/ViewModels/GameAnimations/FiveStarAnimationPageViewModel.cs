using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Pages.Game;
using _30_Seconds_Windows.ViewModels.Game;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.IO;

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

        public async Task LoadData()
        {
            IsLoading = true;
            NavigatedTo();
            TimeStarted = DateTime.Now;
            AnimationFontSize = 300;
            IsLoading = false;

            Timer.Tick += Timer_Tick;

            await Task.Run(async () =>
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
            base.NavigatedFrom();
            Timer.Tick -= Timer_Tick;
        }

        async void Timer_Tick(object sender, object e)
        {
            int MilliSecondsElapsed = (int)DateTime.Now.Subtract(TimeStarted.Value).TotalMilliseconds;

            AnimationFontSize += 15;

            NotifyPropertyChanged("AnimationFontSize");

            if (MilliSecondsElapsed > 5500)
            {
                MediaPlayer.Stop();
                await Navigate(typeof(NextPlayerPage), true);
            }
        }

        public override void Unload()
        {
            
        }

        public async override Task Load()
        {
            
        }
    }
}
