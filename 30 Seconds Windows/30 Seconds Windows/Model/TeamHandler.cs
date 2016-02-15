using BaseLogic.DataHandler;
using BaseLogic.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.Model
{
    public class TeamHandler : DataHandler
    {
        public static readonly TeamHandler instance = new TeamHandler();

        private List<Team> Teams = null;

        private TeamHandler() : base()
        {
            CreateItemTable<Team>();
            Teams = GetItems<Team>().ToList();
        }

        public Team[] GetTeamsByGame(Game Game)
        {
            return Teams.Where(t => t.GameID == Game.InternalID).ToArray();
        }

        public Team GetTeamByID(int TeamID)
        {
            return Teams.Single(t => t.InternalID == TeamID);
        }

        public bool SaveTeams(List<Team> Teams)
        {
            try
            {
                this.Teams.AddRange(Teams);
                SaveItems(Teams);

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
