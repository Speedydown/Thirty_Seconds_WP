using BaseLogic.DataHandler;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.Model
{
    public class Game : DataObject
    {
        public DateTime TimeStarted { get; set; }
        public bool Finished { get; set; }

        [Ignore]
        public List<Team> Teams { get; set; }

        public Game()
        {
            if (InternalID != 0)
            {
                Teams = TeamHandler.instance.GetTeamsByGame(this);
            }
            else
            {
                Teams = new List<Team>();
            }
        }
    }
}
