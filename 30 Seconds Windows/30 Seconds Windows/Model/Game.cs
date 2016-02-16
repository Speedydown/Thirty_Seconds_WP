using BaseLogic.DataHandler;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.Model
{
    public class Game : DataObject
    {
        public DateTime TimeStarted { get; set; }
        public bool Finished { get; set; }

        private ObservableCollection<Team> _Teams;
        [Ignore]
        public ObservableCollection<Team> Teams
        {
            get
            {
                if (_Teams == null)
                {
                    if (InternalID != 0)
                    {
                        Teams = new ObservableCollection<Team>(TeamHandler.instance.GetTeamsByGame(this));
                    }
                    else
                    {
                        Teams = new ObservableCollection<Team>();
                    }
                }

                return _Teams;
            }
            set { _Teams = value; }
        }

        public Game()
        {
            
        }
    }
}
