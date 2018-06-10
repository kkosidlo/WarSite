using Newtonsoft.Json;
using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using WarStarts.DataObjects;
using WarStarts.Enum;
using WarStarts.Helpers;
using static WarStarts.DataObjects.GuildDO;

namespace WarStarts.Jobs
{
    public class MembersListCounter : Base
    {
        public void Proceed()
        {
            foreach (var guild in GuildEnum.GetValues(typeof(GuildEnum)))
            {
                bool isRequestOk = RequestManager.SendGETRequest(
                    $"{ TibiaAPIGuildUrl }{ guild.ToString() }.json",
                    out var responseString) == HttpStatusCode.OK
                    && !String.IsNullOrEmpty(responseString);

                if (isRequestOk)
                    ProcessResponse(responseString);
            }
        }

        private void ProcessResponse(string responseString)
        {
            var guild = JsonConvert
                .DeserializeObject<GuildDO.RootObject>
                (responseString);

            string membersListAsString = BuildStringMembers(guild);
            GuildEnum guildTypeEnum = (GuildEnum)System.Enum.Parse
                (typeof(GuildEnum), guild.guild.data.name);

            StoreGuildMembersInDb(membersListAsString, guildTypeEnum);
        }

        private void StoreGuildMembersInDb(string members, GuildEnum guild)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string sql = $"INSERT INTO [dbo].[MembersList] ([Members], [Guild], [Date]) " +
                             $"VALUES (@MembersList, @Guild, @Date)";

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    cmd.Parameters.AddWithValue("@MembersList", OleDbType.VarWChar).Value = members;
                    cmd.Parameters.AddWithValue("@Guild", OleDbType.VarWChar).Value = (int)guild;
                    cmd.Parameters.AddWithValue("@Date", OleDbType.DBDate).Value = DateTime.Now;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string BuildStringMembers(RootObject guildMembers)
        {
            StringBuilder sb =
                new StringBuilder();

            foreach (var member in guildMembers.guild.members)
            {
                foreach (var memberCharacter in member.characters)
                {
                    sb.Append($"{memberCharacter.name};");
                }
            }

            return sb.ToString().TrimEnd(';');
        }

    }
}