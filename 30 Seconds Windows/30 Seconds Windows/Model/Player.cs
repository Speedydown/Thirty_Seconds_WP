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
    }
}
