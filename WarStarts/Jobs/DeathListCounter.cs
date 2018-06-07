using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using WarStarts.DataObjects;
using WarStarts.Helpers;
using WarStarts.Models;

using static WarStarts.Controllers.HomeController;

namespace WarStarts.Jobs
{
    public class DeathListCounter
    {
        private const string TibiaGuildUrl = "https://secure.tibia.com/community/?subtopic=guilds&page=view&GuildName=";
        public void Proceed()
        {
            string connetionString = "Data Source=SQL6003.site4now.net;Initial Catalog=DB_A3C068_showland;User Id=DB_A3C068_showland_admin;Password=kacperQ123;";
            string sql = null;

            var allMembers = GetMembers().Character;


            var sortedCharacters = allMembers.Where(x => x.Level >= 250);


            foreach (var character in sortedCharacters)
            {
                if (RequestManager.CallTibiaSite($"https://api.tibiadata.com/v2/characters/{character.CharacterName}.json", out var responseString) == HttpStatusCode.OK)
                {
                    var deaths = JsonConvert.DeserializeObject<DeathsDO.RootObject>(responseString);

                    foreach (var item in deaths.characters.deaths.ToList())
                    {
                        var death = JsonConvert.DeserializeObject<DeathDO.RootObject>(item.ToString());

                        if (death.involved.Count == 1 && death.involved[0].name == character.CharacterName)
                            continue;

                        if (death.involved.Count() > 0 && DateTime.Parse(death.date.date.ToString()) >= new DateTime(2018, 5, 4))
                        {
                            string killers = null;

                            foreach (var kill in death.involved)
                            {
                                killers += kill.name + ";";
                            }

                            string newString = killers.Substring(0, killers.Length - 1);
                            int count;

                            using (SqlConnection cnn = new SqlConnection(connetionString))
                            {
                                string sqlCheck = $"select count(*) from[dbo].[DeathList] where CharacterName = @CharacterName and Date = @Date";

                                using (SqlCommand cmd = new SqlCommand(sqlCheck, cnn))
                                {
                                    cnn.Open();

                                    cmd.Parameters.AddWithValue("@CharacterName", OleDbType.VarWChar).Value = character.CharacterName;
                                    cmd.Parameters.AddWithValue("@Date", OleDbType.DBDate).Value = DateTime.Parse(death.date.date);
                                    count = (int)cmd.ExecuteScalar();
                                }


                                if (count == 0)
                                {
                                    sql = $"insert into [dbo].[DeathList] ([CharacterName], [Reason], [Guild], [Killers], [Level], [Date]) values(@CharacterName, @Reason, @Guild, @Killers, @Level, @Date)";

                                    var guildEnumAsInt = (int)Enum.Parse(typeof(GuildEnum), character.Guild.ToString());

                                    using (SqlCommand cmd = new SqlCommand(sql, cnn))
                                    {
                                        cmd.Parameters.AddWithValue("@CharacterName", OleDbType.VarWChar).Value = character.CharacterName;
                                        cmd.Parameters.AddWithValue("@Reason", OleDbType.VarWChar).Value = death.reason;
                                        cmd.Parameters.AddWithValue("@Guild", OleDbType.Integer).Value = guildEnumAsInt;
                                        cmd.Parameters.AddWithValue("@Killers", OleDbType.VarWChar).Value = newString;
                                        cmd.Parameters.AddWithValue("@Level", OleDbType.Integer).Value = death.level;
                                        cmd.Parameters.AddWithValue("@Date", OleDbType.DBDate).Value = DateTime.Parse(death.date.date);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private CharactersList GetMembers()
        {
            CharactersList result = new CharactersList();

            foreach (var guild in Enum.GetValues(typeof(GuildEnum)))
            {
                if (RequestManager.CallTibiaSite($"{ TibiaGuildUrl }{ guild.ToString() }", out var responseString) == HttpStatusCode.OK)
                {
                    var parsedHtmlPage = ParseHtmlPage(responseString);

                    PageAnalyzer page = new PageAnalyzer();
                    result.Character.AddRange(page.AnalyzePage(parsedHtmlPage, (GuildEnum)guild));
                }
            }

            return result;
        }

        private HtmlNodeCollection ParseHtmlPage(string responseString)
        {
            HtmlDocument htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(responseString);

            var htmlbody = htmlDoc.DocumentNode.SelectSingleNode("//body");

            var page = htmlbody.SelectNodes("//div[@class='InnerTableContainer']//div[@class='TableContentContainer']//table[@class='TableContent']")[0];

            return page.SelectNodes("//tr");
        }
    }
}