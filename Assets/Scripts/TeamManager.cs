using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TeamManager
    {
        public static Dictionary<string, Team> Teams
        {
            get
            {
                if (instance == null) instance = new TeamManager();
                return instance.teams;
            }
        }

        private static TeamManager instance;

        private Dictionary<string, Team> teams;

        private TeamManager() 
        {
            teams = new Dictionary<string, Team>
            {
                { nameof(Race.Neutral), new Team(nameof(Race.Neutral), Color.gray, Race.Neutral) }
            };
        }

        public static Team CreateTeam(Race race)
        {
            var name = string.Empty;
            var color = Color.white;

            if (race == Race.Pepl)
            {
                if (!Teams.ContainsKey("Green"))
                {
                    name = "Green";
                    color = Color.green;
                }
                else if (!Teams.ContainsKey("Cyan"))
                {
                    name = "Cyan";
                    color = Color.cyan;
                }
                else if (!Teams.ContainsKey("Blue"))
                {
                    name = "Blue";
                    color = Color.blue;
                }
                else
                {
                    name = GetRandomName();
                    color = UnityEngine.Random.ColorHSV(0.2f, 0.8f, 0.2f, 0.8f, 0.2f, 0.8f);
                }
            }
            else if (race == Race.Dimn)
            {
                if (!Teams.ContainsKey("Red"))
                {
                    name = "Red";
                    color = Color.red;
                }
                else if (!Teams.ContainsKey("Yellow"))
                {
                    name = "Yellow";
                    color = Color.yellow;
                }
                else if (!Teams.ContainsKey("Magenta"))
                {
                    name = "Magenta";
                    color = Color.magenta;
                }
                else
                {
                    name = GetRandomName();
                    color = UnityEngine.Random.ColorHSV(0.2f, 0.8f, 0.2f, 0.8f, 0.2f, 0.8f);
                }
            }
            else
            {
                throw new InvalidOperationException($"Race {race} unregisted!");
            }

            var team = new Team(name, color, race);
            foreach (var item in Teams)
            {
                if (item.Value.Race == Race.Neutral || item.Value.Race == race) continue;
                StartWar(team, item.Value);
            }
            Teams.Add(name, team);

            return team;
        }

        public static void StartWar(Team firstTeam, Team secondTeam)
        {
            secondTeam.AddEnemy(firstTeam); // Запомнили друг друга =_=
            firstTeam.AddEnemy(secondTeam);
        }

        public static Team GetOrCreate(string name, Race race)
        {
            return Teams.ContainsKey(name) ? Teams[name] : CreateTeam(race);
        }

        private static string GetRandomName()
        {
            throw new NotImplementedException();
        }
    }
}
