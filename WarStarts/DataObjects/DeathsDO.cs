using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarStarts.DataObjects
{
    public class DeathsDO
    {
        public class Guild
        {
            public string name { get; set; }
            public string rank { get; set; }
        }

        public class LastLogin
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }

        public class Data
        {
            public string name { get; set; }
            public string sex { get; set; }
            public string vocation { get; set; }
            public int level { get; set; }
            public int achievement_points { get; set; }
            public string world { get; set; }
            public string residence { get; set; }
            public string married_to { get; set; }
            public Guild guild { get; set; }
            public List<LastLogin> last_login { get; set; }
            public string comment { get; set; }
            public string account_status { get; set; }
            public string status { get; set; }
        }

        public class Achievement
        {
            public int stars { get; set; }
            public string name { get; set; }
        }

        public class Created
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }
        public class AccountInformation
        {
            public string loyalty_title { get; set; }
            public Created created { get; set; }
        }

        public class OtherCharacter
        {
            public string name { get; set; }
            public string world { get; set; }
            public string status { get; set; }
        }

        public class Characters
        {
            public Data data { get; set; }
            public List<Achievement> achievements { get; set; }
            public List<object> deaths { get; set; }
            [JsonIgnore]
            public AccountInformation account_information { get; set; }
            public List<OtherCharacter> other_characters { get; set; }
        }

        public class Information
        {
            public int api_version { get; set; }
            public double execution_time { get; set; }
            public string last_updated { get; set; }
            public string timestamp { get; set; }
        }

        public class RootObject
        {
            public Characters characters { get; set; }
            public Information information { get; set; }
        }
    }

    public class DeathDO
    {
        public class Date
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }

        public class Involved
        {
            public string name { get; set; }
        }

        public class RootObject
        {
            public Date date { get; set; }
            public int level { get; set; }
            public string reason { get; set; }
            public List<Involved> involved { get; set; }
        }
    }
}