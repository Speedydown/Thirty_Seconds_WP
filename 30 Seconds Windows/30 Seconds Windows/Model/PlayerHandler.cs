using BaseLogic.DataHandler;
using BaseLogic.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.Model
{
    public class PlayerHandler : DataHandler
    {
        public static readonly PlayerHandler instance = new PlayerHandler();

        private List<Player> Players = null;

        private PlayerHandler() : base()
        {
            CreateItemTable<Player>();
            Players = GetItems<Player>().ToList();
        }

        public Player[] GetPlayersByTeam(Team Team)
        {
            return Players.Where(p => p.TeamID == Team.InternalID).ToArray();
        }

        public bool SaveTeams(Player[] Players)
        {
            try
            {
                this.Players.AddRange(Players);
                SaveItems(Players);

                return true;
            }
            catch (Exception e)
            {
                Task ExceptionTask = Task.Run(() => ExceptionHandler.instance.PostException(new AppException(e), 10));
                return false;
            }
        }
    }
}
