using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static WarStarts.Controllers.HomeController;

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