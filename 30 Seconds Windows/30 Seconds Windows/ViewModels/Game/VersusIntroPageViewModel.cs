using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Pages.Game;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.Game
{
    public class VersusIntroPageViewModel : GameViewModel
    {
        public static readonly VersusIntroPageViewModel instance = new VersusIntroPageViewModel();

        private DispatcherTimer Timer = null;
        private bool AnimationReversed = false;

        public Model.Game CurrentGame { get; private set; }

        private Thickness _AnimationMargin;

        public Thickness AnimationMargin
        {
            get { return _AnimationMargin; }
            set
            {
                _AnimationMargin = value;
                NotifyPropertyChanged();
            }
        }


        public Team Team1
        {
            get
            {
                if (CurrentGame.Teams.Count > 0)
                {
                    return CurrentGame.Teams.First();
                }
                else
                {
                    return null;
                }
            }
        }

        public Team Team2
        {
            get
            {
                if (CurrentGame.Teams.Count > 1)
                {
                    return CurrentGame.Teams[1];
                }
                else
                {
                    return null;
                }
            }
        }

        public Team Team3
        {
            get
            {
                if (CurrentGame.Teams.Count > 2)
                {
                    return CurrentGame.Teams[2];
                }
                else
                {
                    return null;
                }
            }
        }

        private bool _TwoPlayer;
        public bool TwoPlayer
        {
            get { return _TwoPlayer; }
            set
            {
                _TwoPlayer = value;
                NotifyPropertyChanged();
            }
        }

        private bool _ThreePlayer;
        public bool ThreePlayer
        {
            get { return _ThreePlayer; }
            set
            {
                _ThreePlayer = value;
                NotifyPropertyChanged();
            }
        }

        private bool _FourPlayer;
        public bool FourPlayer
        {
            get { return _FourPlayer; }
            set
            {
                _FourPlayer = value;
                NotifyPropertyChanged();
            }
        }

        private VersusIntroPageViewModel()
            : base()
        {

        }

        public async Task LoadData()
        {
            IsLoading = true;
            NavigatedTo();
            AnimationReversed = false;
            StatusBar.GetForCurrentView().ForegroundColor = Colors.White;

            AnimationMargin = new Thickness(0, -600, -600, 0);

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Timer = new DispatcherTimer();
                Timer.Tick += DispatcherTimer_Tick;
                Timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
                Timer.Start();
            });

            CurrentGame = GameHandler.instance.GetCurrentGame();
            NotifyPropertyChanged("CurrentGame");

            TwoPlayer = CurrentGame.Teams.Count == 2;
            ThreePlayer = CurrentGame.Teams.Count == 3;
            FourPlayer = CurrentGame.Teams.Count > 3;

            IsLoading = false;
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            int currentMargin = (int)AnimationMargin.Top;

            if (AnimationReversed)
            {
                if (currentMargin <= 0)
                {
                    Timer.Tick -= DispatcherTimer_Tick;
                    Timer.Stop();
                    Timer = null;
                    AnimationMargin = new Thickness(0, 0, 0, 0);
                    Task t = NavigateToGame();
                }
                else
                {
                    AnimationMargin = new Thickness(0, currentMargin - 17, 0, currentMargin - 17);
                }
            }
            else
            {
                if (currentMargin >= 100)
                {
                    AnimationReversed = true;
                }
                else
                {
                    AnimationMargin = new Thickness(0, currentMargin + 17, 0, currentMargin + 17);
                }
            }
        }

        private async Task NavigateToGame()
        {
            await Task.Delay(750);
            (Window.Current.Content as Frame).Navigate(typeof(NextPlayerPage));
            await ClearBackstack(0);
        }

    }
}
