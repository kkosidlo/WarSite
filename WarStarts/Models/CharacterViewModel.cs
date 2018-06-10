using System;
using System.Collections.Generic;
using WarStarts.Enum;

namespace WarStarts.Models
{
    public class CharactersList
    {
        public CharactersList()
        {
            Character = new List<Character>();
        }

        public List<Character> Character { get; set; }
    }

    public class Character
    {
        public Character()
        {
            Deaths = new List<Death>();
        }

        public string CharacterName { get; set; }
        public int Level { get; set; }
        public string Vocation { get; set; }
        public string Status { get; set; }
        public GuildEnum Guild { get; set; }
        public List<Death> Deaths { get; set; }

    }

    public class Death
    {
        public string Killers { get; set; }
        public int Level { get; set; }
        public DateTime Date { get; set; }
        public int Guild { get; set; }
    }
}