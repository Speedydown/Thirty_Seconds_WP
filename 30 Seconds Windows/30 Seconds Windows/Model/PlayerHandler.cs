using _30_Seconds_Windows.Model.Utils;
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

        public void DeletePlayer(Player player)
        {
            DeleteItem(player);
            Players.Remove(player);
        }

        public Player GetPlayerByID(int ID)
        {
            return Players.SingleOrDefault(p => p.InternalID == ID);
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
                Task ExceptionTask = Task.Run(() => ExceptionHandler.instance.PostException(new AppException(e), (int)BaseLogic.ClientIDHandler.ClientIDHandler.AppName._30Seconds));
                return false;
            }
        }

        public bool SavePlayers(Player[] Players)
        {
            try
            {
                bool Result = true;

                foreach (Player p in Players)
                {
                    bool SaveResult = SavePlayer(p);

                    if (!SaveResult)
                    {
                        Result = !Result ? SaveResult : false;
                    }
                }

                return Result;
            }
            catch (Exception e)
            {
                Task ExceptionTask = Task.Run(() => ExceptionHandler.instance.PostException(new AppException(e), (int)BaseLogic.ClientIDHandler.ClientIDHandler.AppName._30Seconds));
                return false;
            }
        }
    }
}
