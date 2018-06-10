using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using WarStarts.DataObjects;
using WarStarts.Enum;
using WarStarts.Helpers;
using WarStarts.Transformators;
using static WarStarts.DataObjects.DeathDO;

namespace WarStarts.Jobs
{
    public class DeathListCounter : Base
    {
        public void Proceed()
        {
            var guildMembersFromDatabase = GetMembersFromDb();
            var transformatedMembers = MembersList
                .StringMembersListToListObject(guildMembersFromDatabase);

            foreach (var character in transformatedMembers)
            {
                bool isRequestOk = RequestManager
                    .SendGETRequest($"{TibiaAPICharacterUrl}{character.Name}.json",
                    out var responseString) == HttpStatusCode.OK
                    && !String.IsNullOrEmpty(responseString);

                if (isRequestOk)
                    ProcessResponse(responseString, character.Name, character.Guild);
            }
        }

        private void ProcessResponse(string responseString, string characterNameInProcess, GuildEnum playerGuild)
        {
            var deaths = JsonConvert
                .DeserializeObject<DeathsDO.RootObject>
                (responseString);

            foreach (var item in deaths.characters.deaths)
            {
                var death = JsonConvert
                    .DeserializeObject<RootObject>
                    (item.ToString());

                bool isOwnDeath = death.involved.Count == 1 
                    && death.involved[0].name == characterNameInProcess;
                bool hasAnyDeaths = death.involved.Count() > 0;

                if (isOwnDeath)
                {
                    continue;
                }

                if (hasAnyDeaths)
                {
                    string deathString = BuildDeathString(death.involved);

                    bool isInDatabase = DoesDeathExistInDatabase(
                        characterNameInProcess, death.date.date);

                    if (!isInDatabase)
                    {
                        StoreDeathInDatabase(death, characterNameInProcess, deathString, playerGuild.ToString());
                    }
                }
            }
        }

        private string BuildDeathString(List<Involved> involved)
        {
            StringBuilder sb = 
                new StringBuilder();

            foreach (var playerInvolved in involved)
            {
                sb.Append($"{playerInvolved.name};");
            }

            return sb.ToString().TrimEnd(';');
        }

        private bool DoesDeathExistInDatabase(string characterName, string deathDate)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                string sqlCheck = $"SELECT COUNT(*) FROM [dbo].[DeathList]" +
                                  $"WHERE CharacterName = @CharacterName AND Date = @Date";
                    
                using (SqlCommand cmd = new SqlCommand(sqlCheck, cnn))
                {
                    cnn.Open();

                    cmd.Parameters.AddWithValue("@CharacterName", OleDbType.VarWChar).Value = characterName;
                    cmd.Parameters.AddWithValue("@Date", OleDbType.DBDate).Value = DateTime.Parse(deathDate);
                    int count = (int)cmd.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        private void StoreDeathInDatabase(RootObject death, string characterName, string deathString, string characterGuild)
        {
            string sql = $"INSERT INTO [dbo].[DeathList] ([CharacterName], [Reason], [Guild], [Killers], [Level], [Date]) VALUES" +
                         $"(@CharacterName, @Reason, @Guild, @Killers, @Level, @Date)";

            var guildEnumAsInt = (int)GuildEnum.Parse(typeof(GuildEnum), characterGuild);

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, cnn))
            {
                cnn.Open();
                cmd.Parameters.AddWithValue("@CharacterName", OleDbType.VarWChar).Value = characterName;
                cmd.Parameters.AddWithValue("@Reason", OleDbType.VarWChar).Value = death.reason;
                cmd.Parameters.AddWithValue("@Guild", OleDbType.Integer).Value = guildEnumAsInt;
                cmd.Parameters.AddWithValue("@Killers", OleDbType.VarWChar).Value = deathString;
                cmd.Parameters.AddWithValue("@Level", OleDbType.Integer).Value = death.level;
                cmd.Parameters.AddWithValue("@Date", OleDbType.DBDate).Value = DateTime.Parse(death.date.date);
                cmd.ExecuteNonQuery();
            }
        }
    }
}