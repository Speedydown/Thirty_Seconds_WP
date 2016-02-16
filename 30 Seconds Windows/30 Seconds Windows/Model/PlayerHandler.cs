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

        public List<Player> GetPlayersByTeam(Team Team)
        {
            return Players.Where(p => p.TeamID == Team.InternalID).ToList();
        }

        public List<Player> GetPlayersFromDatabase()
        {
            return Players.OrderBy(p => p.Name).ToList();
        }

        public bool SavePlayer(Player player)
        {
            try
            {
                if (!this.Players.Contains(player))
                {
                    this.Players.Add(player);
                }

                SaveItem(player);

                return true;
            }
            catch (Exception e)
            {
                Task ExceptionTask = Task.Run(() => ExceptionHandler.instance.PostException(new AppException(e), 10));
                return false;
            }
        }

        public bool SavePlayers(Player[] Players)
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
