using System.Collections.Generic;
using WarStarts.Enum;

namespace WarStarts.Models
{
    public class CharacterDeathsViewModel
    {
        public string Name { get; set; }
        public List<string> Killers { get; set; }
        public string TimeOfDeath { get; set; }
        public int Level { get; set; }
        public GuildEnum Guild { get; set; }
    }
}