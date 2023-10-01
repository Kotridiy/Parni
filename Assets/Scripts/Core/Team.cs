using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public struct Team
    {
        public string Name { get; private set; }
        public Color Color { get; private set; }
        public Race Race { get; private set; }
        
        private List<Team> enemies;

        public Team(string name, Color color, Race race)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Color = color;
            Race = race;
            enemies = new List<Team>();
        }

        public bool IsEnemy(Team team)
        {
            return enemies.Contains(team);
        }

        public void AddEnemy(Team team)
        {
            if (IsEnemy(team)) return; 
            enemies.Add(team);
        }

        public override bool Equals(object obj)
        {
            return obj is Team team &&
                   string.Equals(Name, team.Name);
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public override string ToString()
        {
            return Name + " team";
        }
    }

    public enum Race
    {
        Pepl,
        Dimn,
        Neutral
    }
}
