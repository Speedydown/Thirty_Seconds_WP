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
    public class GameHandler : DataHandler
    {
        public static readonly GameHandler instance = new GameHandler();

        private List<Game> Games = null;

        private GameHandler() : base()
        {
            CreateItemTable<Game>();
            Games = GetItems<Game>().ToList();
        }

        /// <summary>
        /// Adds the game to the database. Also saves child items such as teams and players
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public bool AddNewGame(Game game)
        {
            try
            {
                InsertItem(game);
                Games.Add(game);

                TeamHandler.instance.SaveTeams(game.Teams.ToList());

                return true;
            }
            catch (Exception e)
            {
                Task ExceptionTask = Task.Run(() => ExceptionHandler.instance.PostException(new AppException(e), (int)BaseLogic.ClientIDHandler.ClientIDHandler.AppName._30Seconds));
                return false;
            }
        }

        public Game GetGameByGameID(int GameID)
        {
            try
            {
                return Games.Single(g => g.InternalID == GameID);
            }
            catch
            {
                return null;
            }
        }

        public bool SaveCurrentGame()
        {
            try
            {
                SaveItem(GetCurrentGame());
                return true;
            }
            catch (Exception e)
            {
                new AppException(e);
                return false;
            }
        }

        public Game GetCurrentGame()
        {
            try
            {
                return Games.Single(g => !g.Finished);
            }
            catch
            {
                SetAllGamesToFinished();
                return null;
            }
        }

        public void SetAllGamesToFinished()
        {
            try
            {
                Game[] Games = this.Games.Where(g => !g.Finished).ToArray();

                foreach (Game g in Games)
                {
                    g.Finished = true;
                }

                SaveItems(Games);
            }
            catch (Exception e)
            {
                Task ExceptionTask = Task.Run(() => ExceptionHandler.instance.PostException(new AppException(e), (int)BaseLogic.ClientIDHandler.ClientIDHandler.AppName._30Seconds));
            }
        }
    }
}
