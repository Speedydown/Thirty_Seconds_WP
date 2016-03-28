using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Pages.Game;
using _30_Seconds_Windows.Pages.GameAnimations;
using _30_Seconds_Windows.ViewModels.GameAnimations;
using BaseLogic.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.Game
{
    public class RoundPageViewModel : GameViewModel
    {
        public static readonly RoundPageViewModel instance = new RoundPageViewModel();

        private DispatcherTimer Timer = null;
        private Word[] _CurrentWords;

        public Word[] CurrentWords
        {
            get { return _CurrentWords; }
            set
            {
                _CurrentWords = value;
                NotifyPropertyChanged("CurrentWords");
            }
        }
        public Model.Game CurrentGame { get; private set; }
        public Team CurrentTeam { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public bool RoundFinishedAnimationVisible { get; private set; }
        public bool EndOfRoundVisible { get; private set; }
        public int HourglassAngle { get; private set; }
        public bool AdVisible { get; set; }

        private DateTime? TimeStarted = null;
        private bool RoundFinished = false;
        private bool WarningSoundPlayed = false;
        private bool HourGlassAnimationReversed = false;
        private Task<Word[]> GetNewWordsTask = null;

        private RoundPageViewModel()
            : base()
        {

        }

        public void Get5NewWords()
        {
            CurrentGame = GameHandler.instance.GetCurrentGame();
            CurrentTeam = TeamHandler.instance.GetTeamByID(CurrentGame.CurrentTeamID.Value);
            CurrentPlayer = PlayerHandler.instance.GetPlayerByID(CurrentTeam.CurrentPlayerID.Value);
            GetNewWordsTask = WordHandler.instance.Get5Words(CurrentPlayer.InternalID);
        }

        public async Task Load()
        {
            CurrentWords = null;
            IsLoading = true;
            Task AdsTask = Task.Run(() =>
                {
                    AdVisible = false;
                    NotifyPropertyChanged("AdVisible");
                    AdVisible = !IAPHandler.instance.HasFeature(IAPHandler.RemoveAdsFeatureName);
                    NotifyPropertyChanged("AdVisible");
                });
            StopwatchStream.Position = 0;
            AlarmStream.Position = 0;
            NavigatedTo();
            CurrentWords = null;

            if (GetNewWordsTask != null)
            {
                CurrentWords = await GetNewWordsTask;
                GetNewWordsTask = null;
            }

            if (CurrentWords == null)
            {
                Get5NewWords();
                CurrentWords = await GetNewWordsTask;
            }

            IsLoading = false;
            NotifyPropertyChanged("CurrentGame");
            NotifyPropertyChanged("CurrentTeam");
            NotifyPropertyChanged("CurrentPlayer");

            TimeStarted = DateTime.Now;

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        Timer = new DispatcherTimer();
                        Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                        Timer.Tick += Timer_Tick;
                        Timer.Start();
                    }
                    catch (Exception)
                    {

                    }
                });
        }

        public override void NavigatedFrom()
        {
            RoundFinished = false;
            EndOfRoundVisible = false;
            WarningSoundPlayed = false;
            HourGlassAnimationReversed = false;
            HourglassAngle = 0;
            base.NavigatedFrom();
            Timer.Tick -= Timer_Tick;
            Timer.Stop();
            Timer = null;
        }

        private void Timer_Tick(object sender, object e)
        {
            Task.Run(async () =>
            {
                try
                {
                    int SecondsElapsed = (int)DateTime.Now.Subtract(TimeStarted.Value).TotalSeconds;

                    if (!RoundFinished && !WarningSoundPlayed && SecondsElapsed > 26)
                    {
                        WarningSoundPlayed = true;

                        await PlayFile(StopwatchStream, StopwatchFile);
                    }
                    else if (!RoundFinished && SecondsElapsed > 29)
                    {
                        await StopMedia();

                        RoundFinished = true;
                        RoundFinishedAnimationVisible = true;
                        NotifyPropertyChanged("RoundFinishedAnimationVisible");

                        await PlayFile(AlarmStream, AlarmFile);
                    }
                    else if (RoundFinished && SecondsElapsed > 32)
                    {
                       
                        EndOfRoundVisible = true;
                        NotifyPropertyChanged("EndOfRoundVisible");
                        RoundFinishedAnimationVisible = false;
                        NotifyPropertyChanged("RoundFinishedAnimationVisible");
                        await StopTimer();
                    }

                    //Animate hourglass
                    if (RoundFinishedAnimationVisible)
                    {
                        HourglassAngle = HourGlassAnimationReversed ? HourglassAngle + 11 : HourglassAngle - 11;

                        if (HourglassAngle < -25)
                        {
                            HourGlassAnimationReversed = true;
                        }
                        else if (HourglassAngle > 25)
                        {
                            HourGlassAnimationReversed = false;
                        }
                    }

                    NotifyPropertyChanged("HourglassAngle");
                }
                catch (Exception ex)
                {
                    Task PostExTask = ExceptionHandler.instance.PostException(new AppException(ex), (int)BaseLogic.ClientIDHandler.ClientIDHandler.AppName._30Seconds);
                }
            });
        }

        private async Task PlayFile(MemoryStream stream, StorageFile file)
        {
            Task MediaPlayerTask = Task.Run(async () =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MediaPlayer.SetSource(stream.AsRandomAccessStream(), file.ContentType);
                    MediaPlayer.Play();
                });
            });
        }

        private async Task StopMedia()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MediaPlayer.Stop();
            });
        }

        private async Task StopTimer()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MediaPlayer.Stop();
                Timer.Tick -= Timer_Tick;
                Timer.Stop();
            });
        }

        public async Task NextRoundButton()
        {
            foreach (Word w in CurrentWords.Where(w => w.CurrentGameCorrect))
            {
                w.NumberOfTimesCorrect++;
            }

            int WordsCorrect = CurrentWords.Count(w => w.CurrentGameCorrect);

            CurrentPlayer.QuestionsCorrect += WordsCorrect;
            CurrentPlayer.PointsThisGame += WordsCorrect == 5 ? WordsCorrect + 1 : WordsCorrect;
            CurrentPlayer.QuestionsCorrectThisGame += WordsCorrect;

            CurrentTeam.Points += WordsCorrect == 5 ? WordsCorrect + 1 : WordsCorrect;

            if (CheckWinCondition())
            {
                Team WinningTeam = CurrentGame.Teams.OrderByDescending(t => t.Points).First();

                Task FinishGameTask = Task.Run(() =>
                {
                    CurrentGame.Finished = true;
                    Task t4 = Task.Run(() =>
                {
                    GameHandler.instance.SaveGame(CurrentGame);
                });

                    foreach (Player p in WinningTeam.Players)
                    {
                        p.GamesWon++;
                    }

                    PlayerHandler.instance.SavePlayers(WinningTeam.Players.ToArray());
                });

                //Current game does not exist in db anymore -> prepare data in viewmodels
                EndGamePageViewModel.instance.CurrentGame = CurrentGame;
                VictoryAnimationPageViewModel.instance.WinningTeam = WinningTeam;
                (Window.Current.Content as Frame).Navigate(typeof(VictoryAnimationPage));
            }
            else if (WordsCorrect == 0)
            {
                (Window.Current.Content as Frame).Navigate(typeof(ZeroStarAnimationPage));
            }
            else if (WordsCorrect == 5)
            {
                FiveStarAnimationPageViewModel.instance.PlayedWords = CurrentWords;
                (Window.Current.Content as Frame).Navigate(typeof(FiveStarAnimationPage));
            }
            else
            {
                (Window.Current.Content as Frame).Navigate(typeof(NextPlayerPage));
            }

            await ClearBackstack(0);


            Task SaveTask = Task.Run(async () =>
            {
                await Task.Delay(750);
                WordHandler.instance.SaveWords(CurrentWords);
                PlayerHandler.instance.SavePlayer(CurrentPlayer);
                TeamHandler.instance.SaveTeam(CurrentTeam);
            });
        }

        private bool CheckWinCondition()
        {
            Team LeadTeam = CurrentGame.Teams.OrderByDescending(t => t.Points).First();

            if (LeadTeam.Points >= SettingsHandler.instance.CurrentSettings.RequiredPoints)
            {
                if (!(CurrentGame.Teams.Count(t => t.Round < LeadTeam.Round &&
                SettingsHandler.instance.CurrentSettings.RequiredPoints - t.Points < 7) > 0))
                {
                    if (CurrentGame.Teams.Count(t => t.Round == LeadTeam.Round && t.Points == LeadTeam.Points) > 1)
                    {
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
