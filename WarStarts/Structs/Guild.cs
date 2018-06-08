using System.Collections.Generic;
using static WarStarts.Controllers.HomeController;

namespace WarStarts.Classes
{
    public struct Guild
    {
        public string[] Members { get; set; }
        public GuildEnum GuildType { get; set; }
    }
}