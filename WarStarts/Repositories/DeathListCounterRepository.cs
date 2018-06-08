using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WarStarts.Classes;
using WarStarts.Jobs;
using static WarStarts.Controllers.HomeController;

namespace WarStarts.Repositories
{
    public class DeathListCounterRepository : Base
    {
        public List<Guild> GetAllMembersFromDatabase()
        {
            List<Guild> guildMembersList = new List<Guild>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT TOP 2 * FROM [DB_A3C068_showland].[dbo].[MembersList] ORDER BY DATE DESC";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            guildMembersList.Add(new Guild
                            {
                                Members = reader.GetString(reader.GetOrdinal("Members")).Split(';'),
                                GuildType = (GuildEnum)reader.GetInt32(reader.GetOrdinal("Guild"))
                            });
                        }
                    }
                }
            }

            return guildMembersList;
        }
    }


}