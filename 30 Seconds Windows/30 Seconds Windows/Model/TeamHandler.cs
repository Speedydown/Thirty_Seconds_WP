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
    public class TeamHandler : DataHandler
    {
        public static readonly TeamHandler instance = new TeamHandler();

        private List<Team> Teams = null;

        private TeamHandler()
            : base()
        {
            CreateItemTable<Team>();
            Teams = GetItems<Team>().ToList();
        }

        public void DeleteTeam(Team team)
        {
            DeleteItem(team);
            Teams.Remove(team);
        }

        public List<Team> GetTeamsByGame(Game Game)
        {
            return Teams.Where(t => t.GameID == Game.InternalID).OrderBy(t => t.Name).ToList();
        }

        public Team GetTeamByID(int TeamID)
        {
            return Teams.Single(t => t.InternalID == TeamID);
        }

        public List<Team> GetTeamsFromDatabase()
        {
            return Teams.OrderBy(x => x.Name).ToList();
        }

        public bool SaveTeam(Team team)
        {
            try
            {
                if (!Teams.Contains(team))
                {
                    Teams.Add(team);
                }

                Task.Run(() =>
                {
                    SaveItem(team);
                });

                return true;
            }
            catch (Exception e)
            {
                Task ExceptionTask = Task.Run(() => ExceptionHandler.instance.PostException(new AppException(e), (int)BaseLogic.ClientIDHandler.ClientIDHandler.AppName._30Seconds));
                return false;
            }
        }

        public bool SetAllTeamsCurrentPlayerIDToNull()
        {
            try
            {
                Team[] MatchingTeams = Teams.Where(t => t.CurrentPlayerID != null || t.Round != 0 || t.Points != 0).ToArray();

                foreach (Team t in MatchingTeams)
                {
                    t.CurrentPlayerID = null;
                    t.Round = 0;
                    t.Points = 0;
                }

                Task.Run(() =>
                {
                    SaveItems(MatchingTeams);
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SaveTeams(List<Team> Teams)
        {
            try
            {
                bool Result = true;

                foreach (Team t in Teams)
                {
                    bool SaveResult = SaveTeam(t);

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
