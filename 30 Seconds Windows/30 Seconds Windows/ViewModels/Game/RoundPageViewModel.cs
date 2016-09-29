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

        private bool _AdVisible;
        public bool AdVisible
        {
            get { return _AdVisible; }
            set
            {
                _AdVisible = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime? TimeStarted = null;
        private bool RoundFinished = false;
        private bool WarningSoundPlayed = false;
        private bool HourGlassAnimationReversed = false;
        public Task<Word[]> GetNewWordsTask = null;

        private RoundPageViewModel()
            : base()
        {

        }

        public async Task Get5NewWords()
        {
            CurrentGame = GameHandler.instance.GetCurrentGame();
            CurrentTeam = TeamHandler.instance.GetTeamByID(CurrentGame.CurrentTeamID.Value);
            CurrentPlayer = PlayerHandler.instance.GetPlayerByID(CurrentTeam.CurrentPlayerID.Value);
            NotifyPropertyChanged("CurrentTeam");
            NotifyPropertyChanged("CurrentPlayer");
            GetNewWordsTask = WordHandler.instance.Get5Words(CurrentPlayer.InternalID);
        }

        public async Task LoadData()
        {
            CurrentWords = null;
            IsLoading = true;

            NavigatedTo();
            CurrentWords = null;

            if (GetNewWordsTask == null)
            {
                await Get5NewWords();
            }

            CurrentWords = await GetNewWordsTask;
           
            IsLoading = false;

            await Task.Run(async() =>
            {
                TimeStarted = DateTime.Now;

                if (Timer == null)
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {

                        Timer = new DispatcherTimer()
                        {
                            Interval = new TimeSpan(0, 0, 0, 0, 100)
                        };

                        Timer.Start();
                    });
                }

            

                AdVisible = !IAPHandler.instance.HasRemoveAds;

                GetNewWordsTask = null;
            });

            Timer.Tick += Timer_Tick;
        }

        public override void NavigatedFrom()
        {
            base.NavigatedFrom();
            Task Stop = StopTimer();

            Task.Run(() =>
            {
                RoundFinished = false;
                EndOfRoundVisible = false;
                WarningSoundPlayed = false;
                HourGlassAnimationReversed = false;
                HourglassAngle = 0;
                StopwatchStream.Position = 0;
                AlarmStream.Position = 0;
            });
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
                        AdVisible = false;
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

                        NotifyPropertyChanged("HourglassAngle");
                    }
                }
                catch (Exception ex)
                {
                    Task PostExTask = ExceptionHandler.instance.PostException(new AppException(ex, (int)BaseLogic.ClientIDHandler.ClientIDHandler.AppName._30Seconds));
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

                if (Timer != null)
                {
                    Timer.Tick -= Timer_Tick;
                }
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
                await Navigate(typeof(VictoryAnimationPage));
            }
            else if (WordsCorrect == 0)
            {
               await Navigate(typeof(ZeroStarAnimationPage));
            }
            else if (WordsCorrect == 5)
            {
                FiveStarAnimationPageViewModel.instance.PlayedWords = CurrentWords;
                await Navigate(typeof(FiveStarAnimationPage));
            }
            else
            {
                await Navigate(typeof(NextPlayerPage));
            }

            await ClearBackStack(0);


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

        public override void Unload()
        {
          
        }

        public override async Task Load()
        {
            
        }
    }
}
