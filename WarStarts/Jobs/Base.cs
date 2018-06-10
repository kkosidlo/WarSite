using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using WarStarts.Classes;
using WarStarts.Enum;

namespace WarStarts.Jobs
{
    public class Base : Controller
    {
        public string ConnectionString => "Server=tcp:showland.database.windows.net,1433;Initial Catalog=showland;Persist Security Info=False;User ID=showland;Password=kacperQ123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public string ConnectionStringOld => "Data Source=SQL6003.site4now.net;Initial Catalog=DB_A3C068_showland;User Id=DB_A3C068_showland_admin;Password=kacperQ123;";


        public string TibiaSiteGuildUrl = "https://secure.tibia.com/community/?subtopic=guilds&page=view&order=level_desc&GuildName=";
        public string TibiaAPICharacterUrl => "https://api.tibiadata.com/v2/characters/";
        public string TibiaAPIGuildUrl => "https://api.tibiadata.com/v2/guild/";

        public List<GuildMembers> GetMembersFromDb()
        {
            string sqlQuery = $"SELECT TOP 2 [Members], [Guild] from [dbo].[MembersList] ORDER BY DATE DESC";

            List<string> memberAsString = new List<string>();

            List<GuildMembers> allMembers = new List<GuildMembers>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
            {
                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            allMembers.Add(new GuildMembers
                            {
                                Members = reader.GetString(reader.GetOrdinal("Members")),
                                Guild = (GuildEnum)reader.GetInt32(reader.GetOrdinal("Guild"))
                            });
                        }
                    }
                }
            }
            return allMembers;
        }
    }
}