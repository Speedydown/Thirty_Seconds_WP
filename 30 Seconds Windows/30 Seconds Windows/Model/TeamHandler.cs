﻿using BaseLogic.DataHandler;
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

        public void DeleteTeam(Team team)
        {
            DeleteItem(team);
            Teams.Remove(team);
        }

        public List<Team> GetTeamsByGame(Game Game)
        {
            return Teams.Where(t => t.GameID == Game.InternalID).ToList();
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
                if (!this.Teams.Contains(team))
                {
                    this.Teams.Add(team);
                }

                SaveItem(team);

                return true;
            }
            catch (Exception e)
            {
                Task ExceptionTask = Task.Run(() => ExceptionHandler.instance.PostException(new AppException(e), 10));
                return false;
            }
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
