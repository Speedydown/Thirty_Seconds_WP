using BaseLogic.DataHandler;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.Model
{
    public class Player : DataObject
    {
        public int TeamID { get; set; }
        private Team _Team = null;
        [Ignore]
        public Team Team
        {
            get {
                if (_Team == null)
                {
                    _Team = TeamHandler.instance.GetTeamByID(TeamID);
                }
                return _Team; }
        }



        public int GameID { get; set; }
        private Game _Game = null;
        [Ignore]
        public Game Game
        {
            get
            {
                if (_Game == null)
                {
                    _Game = GameHandler.instance.GetGameByGameID(GameID);
                }

                return _Game;
            }
        }

        public string Name { get; set; }
        public DateTime? LastRound { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public int QuestionsAnswered { get; set; }
        public int QuestionsCorrect { get; set; }
        public int QuestionsCorrectThisGame { get; set; }
        public int PointsThisGame { get; set; } //TODO punten resetten bij nieuw spel
        [Ignore]
        public string NumberOfTimesPlayedString
        {
            get
            {
                return string.Format("{0} {1}", GamesPlayed, Utils.Utils.ResourceLoader.GetString("text_TimesPlayed"));
            }
        }

        [Ignore]
        public string NumberTimesWonString
        {
            get
            {
                return string.Format("{0} {1}", GamesWon, Utils.Utils.ResourceLoader.GetString("text_TimesWon"));
            }
        }

        [Ignore]
        public string QuestionsAnsweredString
        {
            get
            {
                return string.Format("{0} {1}", QuestionsAnswered, Utils.Utils.ResourceLoader.GetString("text_QuestionsAnswered"));
            }
        }

        [Ignore]
        public string QuestionsCorrectString
        {
            get
            {
                return string.Format("{0} {1}", QuestionsCorrect, Utils.Utils.ResourceLoader.GetString("text_QuestionsCorrect"));
            }
        }
    }
}
