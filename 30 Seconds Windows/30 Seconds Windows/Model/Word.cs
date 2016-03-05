using BaseLogic.DataHandler;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.Model
{
    public enum Languages { Neutral = 0, Dutch = 1, English = 2, German = 3}

    public class Word : DataObject
    {
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public int WordPackID { get; set; }
        public bool Guessed { get; set; }
        public DateTime Retrieved { get; set; }
        public DateTime LastPlayed { get; set; }
        public int LastPlayedByPlayerID { get; set; }
        public bool Deleted { get; set; }

        private bool _CurrentGameCorrect;
        [Ignore]
        public bool CurrentGameCorrect
        {
            get { return _CurrentGameCorrect; }
            set { _CurrentGameCorrect = value;
            NotifyPropertyChanged();
            }
        }

        [Ignore]
        public Player LastPlayedByPlayer
        {
            get
            {
                return PlayerHandler.instance.GetPlayerByID(LastPlayedByPlayerID);
            }
        }

        public int NumberOfTimesTimesPlayed { get; set; }
        public int NumberOfTimesCorrect { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
