using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Pages.Game;
using _30_Seconds_Windows.Pages.GameAnimations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.Game
{
    public class RoundPageViewModel : GameViewModel
    {
        public static readonly RoundPageViewModel instance = new RoundPageViewModel();

        public Word[] CurrentWords { get; private set; }
        public Model.Game CurrentGame { get; private set; }
        public Team CurrentTeam { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public bool RoundFinishedAnimationVisible { get; private set; }
        public bool EndOfRoundVisible { get; private set; }
        public int HourglassAngle { get; private set; }

        private DateTime? TimeStarted = null;
        private bool RoundFinished = false;
        private bool WarningSoundPlayed = false;
        private bool HourGlassAnimationReversed = false;

        private RoundPageViewModel()
            : base()
        {

        }

        public async Task Load()
        {
            IsLoading = true;
            NavigatedTo();
            CurrentGame = GameHandler.instance.GetCurrentGame();
            CurrentTeam = TeamHandler.instance.GetTeamByID(CurrentGame.CurrentTeamID.Value);
            CurrentPlayer = PlayerHandler.instance.GetPlayerByID(CurrentTeam.CurrentPlayerID.Value);
            CurrentWords = await WordHandler.instance.Get5Words(CurrentPlayer.InternalID);
            IsLoading = false;
            NotifyPropertyChanged("CurrentWords");
            NotifyPropertyChanged("CurrentGame");
            NotifyPropertyChanged("CurrentTeam");
            NotifyPropertyChanged("CurrentPlayer");

            TimeStarted = DateTime.Now;

            Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            Timer.Tick += Timer_Tick;
            Timer.Start();
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
        }

        private void Timer_Tick(object sender, object e)
        {
            int SecondsElapsed = (int)DateTime.Now.Subtract(TimeStarted.Value).TotalSeconds;

            if (!RoundFinished && !WarningSoundPlayed && SecondsElapsed > 20)
            {
                WarningSoundPlayed = true;
                //TODO:  Play warning sound
            }
            else if (!RoundFinished && SecondsElapsed > 30)
            {
                RoundFinished = true;
                RoundFinishedAnimationVisible = true;
                NotifyPropertyChanged("RoundFinishedAnimationVisible");
                //TODO: Play end of round sound
            }
            else if (RoundFinished && SecondsElapsed > 35)
            {
                EndOfRoundVisible = true;
                NotifyPropertyChanged("EndOfRoundVisible");
                RoundFinishedAnimationVisible = false;
                NotifyPropertyChanged("RoundFinishedAnimationVisible");
                Timer.Tick -= Timer_Tick;
                Timer.Stop();
            }

            //Animate hourglass
            if (RoundFinishedAnimationVisible)
            {
                HourglassAngle = HourGlassAnimationReversed ? HourglassAngle + 2 : HourglassAngle - 2;

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

        public async Task NextRoundButton()
        {
            foreach (Word w in CurrentWords.Where(w => w.CurrentGameCorrect))
            {
                w.NumberOfTimesCorrect++;
            }

            WordHandler.instance.SaveWords(CurrentWords);

            int WordsCorrect = CurrentWords.Count(w => w.CurrentGameCorrect);

            CurrentPlayer.QuestionsCorrect += WordsCorrect;
            CurrentPlayer.PointsThisGame += WordsCorrect == 5 ? WordsCorrect + 1 : WordsCorrect;
            CurrentPlayer.QuestionsCorrectThisGame += WordsCorrect;

            PlayerHandler.instance.SavePlayer(CurrentPlayer);

            CurrentTeam.Points += WordsCorrect == 5 ? WordsCorrect + 1 : WordsCorrect;

            TeamHandler.instance.SaveTeam(CurrentTeam);

            //GameWon condition
            if (CurrentTeam.Points > SettingsHandler.instance.CurrentSettings.RequiredPoints)
            {
                Task FinishGameTask = Task.Run(() =>
                {
                    CurrentGame.Finished = true;
                    GameHandler.instance.SaveCurrentGame();

                    foreach (Player p in CurrentTeam.Players)
                    {
                        p.GamesWon++;
                    }

                    PlayerHandler.instance.SavePlayers(CurrentTeam.Players.ToArray());
                });

                (Window.Current.Content as Frame).Navigate(typeof(VictoryAnimationPage));
            }
            else if (WordsCorrect == 0)
            {
                (Window.Current.Content as Frame).Navigate(typeof(ZeroStarAnimationPage));
            }
            else if (WordsCorrect == 5)
            {
                (Window.Current.Content as Frame).Navigate(typeof(FiveStarAnimationPage));
            }
            else
            {
                (Window.Current.Content as Frame).Navigate(typeof(NextPlayerPage));
            }

            await ClearBackstack(0);
        }
    }
}
