﻿using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Model.Utils;
using _30_Seconds_Windows.Pages.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.Game
{
    public class NextPlayerPageViewModel : GameViewModel
    {
        public static readonly NextPlayerPageViewModel instance = new NextPlayerPageViewModel();

        private Random Randomizer = new Random();

        public Model.Game CurrentGame { get; private set; }

        private Team _CurrentTeam;
        public Team CurrentTeam
        {
            get { return _CurrentTeam; }
            set
            {
                _CurrentTeam = value;
                NotifyPropertyChanged();
            }
        }

        private Player _CurrentPlayer;
        public Player CurrentPlayer
        {
            get { return _CurrentPlayer; }
            set
            {
                _CurrentPlayer = value;
                NotifyPropertyChanged();
            }
        }

        public string PassTroughText
        {
            get
            {
                if (CurrentPlayer != null)
                {
                    return string.Format(Utils.ResourceLoader.GetString("NextPlayerPagePassPhoneToText"), CurrentPlayer.Name);
                }
                else
                {
                    return null;
                }

            }
        }

        public Team[] TeamsSortedByScore
        {
            get
            {
                return CurrentGame.Teams.OrderByDescending(t => t.Points).ToArray();
            }
        }

        private NextPlayerPageViewModel()
            : base()
        {

        }

        public async Task LoadData()
        {
            IsLoading = true;
            NavigatedTo();
            CurrentGame = GameHandler.instance.GetCurrentGame();
            GetNextTeam();
            GetNextPlayer();
            Task t = Task.Run(() => RoundPageViewModel.instance.Get5NewWords());
            IsLoading = false;
        }

        private void GetNextTeam()
        {
            if (CurrentGame != null)
            {
                if (CurrentGame.CurrentTeamID == null)
                {
                    //Get random team for the game start
                    CurrentTeam = CurrentGame.Teams[Randomizer.Next(0, CurrentGame.Teams.Count - 1)];
                    CurrentGame.CurrentTeamID = CurrentTeam.InternalID;
                }
                else
                {
                    if (CurrentGame.Teams.Count(t => t.Round < CurrentGame.Teams.Max(te => te.Round)) > 0)
                    {
                        CurrentTeam = CurrentGame.Teams.OrderBy(t => t.Round).First();
                    }
                    else
                    {
                        //Get Next Team based on team index
                        Team CurrentTeam = TeamHandler.instance.GetTeamByID(CurrentGame.CurrentTeamID.Value);
                        int TeamIndex = CurrentGame.Teams.IndexOf(CurrentTeam);

                        TeamIndex++;

                        if (TeamIndex + 1 > CurrentGame.Teams.Count)
                        {
                            TeamIndex = TeamIndex - CurrentGame.Teams.Count;
                        }

                        this.CurrentTeam = CurrentGame.Teams[TeamIndex];
                    }

                    CurrentGame.CurrentTeamID = this.CurrentTeam.InternalID;
                }

                Task.Run(() =>
                {
                    GameHandler.instance.SaveGame(CurrentGame);
                });
            }
        }

        private void GetNextPlayer()
        {
            if (CurrentTeam == null)
            {
                return;
            }

            if (CurrentTeam.CurrentPlayerID == null)
            {
                CurrentPlayer = CurrentTeam.Players[Randomizer.Next(0, CurrentTeam.Players.Count - 1)];
                CurrentTeam.CurrentPlayerID = CurrentPlayer.InternalID;
            }
            else
            {
                //Get Next Team based on team index
                Player CurrentPlayer = PlayerHandler.instance.GetPlayerByID(CurrentTeam.CurrentPlayerID.Value);
                int PlayerIndex = CurrentTeam.Players.IndexOf(CurrentPlayer);

                PlayerIndex++;

                if (PlayerIndex + 1 > CurrentTeam.Players.Count)
                {
                    PlayerIndex = PlayerIndex - CurrentTeam.Players.Count;
                }

                this.CurrentPlayer = CurrentTeam.Players[PlayerIndex];
                CurrentTeam.CurrentPlayerID = this.CurrentPlayer.InternalID;
            }

            NotifyPropertyChanged("PassTroughText");
            Task.Run(() =>
            {
                TeamHandler.instance.SaveTeam(CurrentTeam);
            });
        }

        public async Task ContinueButton()
        {
            await Navigate(typeof(PlayerReadyPage), true);
        }

        public override void Unload()
        {
            
        }

        public async override Task Load()
        {
            
        }
    }
}
