using BaseLogic.DataHandler;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.Model
{
    public class Team : DataObject
    {
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
        public string NumberOfPlayersString
        {
            get
            {
                return string.Format("{0} {1}", Players.Count, Utils.Utils.ResourceLoader.GetString("Text_Players"));
            }
        }

        public List<Player> Players { get; set; }
        public int Points { get; set; }

    }
}
