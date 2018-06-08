using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using WarStarts.Helpers;
using WarStarts.Models;
using static WarStarts.Controllers.HomeController;

namespace WarStarts.Jobs
{
    public class MembersListCounter
    {
        private const string TibiaGuildUrl = "https://secure.tibia.com/community/?subtopic=guilds&page=view&GuildName=";

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

        public void Proceed()
        {
            string connetionString = "Data Source=SQL6003.site4now.net;Initial Catalog=DB_A3C068_showland;User Id=DB_A3C068_showland_admin;Password=kacperQ123;";
            var allMembers = GetMembers().Character;
            var showlandMembers = allMembers.Where(x => x.Guild == GuildEnum.Showland);
            var reapersMembers = allMembers.Where(x => x.Guild == GuildEnum.Reapers);

            string allShowlandMembers = null;
            string allReapersMembers = null;

            foreach (var member in showlandMembers)
            {
                allShowlandMembers += member.CharacterName + ";";
            }

            string newStringShowland = allShowlandMembers.Substring(0, allShowlandMembers.Length - 1);


            foreach (var member in reapersMembers)
            {
                allReapersMembers += member.CharacterName + ";";
            }

            string newStringReapers = allReapersMembers.Substring(0, allReapersMembers.Length - 1);

            using (SqlConnection cnn = new SqlConnection(connetionString))
            {
                string sql = $"insert into [dbo].[MembersList] ([Members], [Guild], [Date]) values(@MembersList, @Guild, @Date)";

                using (SqlCommand cmd = new SqlCommand(sql, cnn))
                {
                    cnn.Open();

                    cmd.Parameters.AddWithValue("@MembersList", OleDbType.VarWChar).Value = newStringShowland;
                    cmd.Parameters.AddWithValue("@Guild", OleDbType.VarWChar).Value = (int)GuildEnum.Showland;
                    cmd.Parameters.AddWithValue("@Date", OleDbType.DBDate).Value = DateTime.Now;

                    cmd.ExecuteNonQuery();
                }
            }

            using (SqlConnection cnn = new SqlConnection(connetionString))
            {
                string sql = $"insert into [dbo].[MembersList] ([Members], [Guild], [Date]) values(@MembersList, @Guild, @Date)";

                using (SqlCommand cmd = new SqlCommand(sql, cnn))
                {
                    cnn.Open();

                    cmd.Parameters.AddWithValue("@MembersList", OleDbType.VarWChar).Value = newStringReapers;
                    cmd.Parameters.AddWithValue("@Guild", OleDbType.VarWChar).Value = (int)GuildEnum.Reapers;
                    cmd.Parameters.AddWithValue("@Date", OleDbType.DBDate).Value = DateTime.Now;

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}