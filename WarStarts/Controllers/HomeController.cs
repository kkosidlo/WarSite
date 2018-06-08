using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WarStarts.Helpers;
using WarStarts.Jobs;
using WarStarts.Models;

namespace WarStarts.Controllers
{
    public class HomeController : Controller
    {
        private const string TibiaGuildUrl = "https://secure.tibia.com/community/?subtopic=guilds&page=view&GuildName=";
        private const string TibiaCharacterUrl = "https://secure.tibia.com/community/?subtopic=characters&name=";


        // GET: Home
        public ActionResult Index()
        {
            return View("CharactersView", GetMembers());
        }

        public ActionResult Kills()
        {
            return View("KillsView", GetLastKillsList());
        }

        public ActionResult KillsTotal()
        {
            return View("KillsViewRest", GetLastKillsList());
        }

        private CharactersList GetMembers()
        {
            CharactersList result = new CharactersList();

            foreach (var guild in Enum.GetValues(typeof(GuildEnum)))
            {
                var response = HttpStatusCode.NoContent;
                string responseString = null;

                response = RequestManager.CallTibiaSite($"{ TibiaGuildUrl }{ guild.ToString() }", out responseString);

                var parsedHtmlPage = ParseHtmlPage(responseString);

                PageAnalyzer page = new PageAnalyzer();

                result.Character.AddRange(page.AnalyzePage(parsedHtmlPage, (GuildEnum)guild));
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




        private List<CharacterDeathsViewModel> GetLastKillsList()
        {

            string connetionString = "Data Source=SQL6003.site4now.net;Initial Catalog=DB_A3C068_showland;User Id=DB_A3C068_showland_admin;Password=kacperQ123;";
            string sql = $"select [CharacterName], [Guild], [Killers], [Level], [Date] from[dbo].[DeathList]";

            CharactersList list = new CharactersList();

            using (SqlConnection connection = new SqlConnection(connetionString))
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Character character = new Character();
                            Death death = new Death();

                            character.CharacterName = reader.GetString(reader.GetOrdinal("CharacterName"));
                            death.Killers = reader.GetString(reader.GetOrdinal("Killers"));
                            death.Level = reader.GetInt32(reader.GetOrdinal("Level"));
                            death.Guild = reader.GetInt32(reader.GetOrdinal("Guild"));
                            death.Date = reader.GetDateTime(reader.GetOrdinal("Date"));


                            character.Deaths.Add(death);
                            list.Character.Add(character);
                        }
                    }
                }
            }


            List<CharacterDeathsViewModel> model = new List<CharacterDeathsViewModel>();

            List<CharacterDeathsViewModel> newModel = new List<CharacterDeathsViewModel>();

            foreach (var item in list.Character)
            {
                foreach (var item1 in item.Deaths)
                {
                    model.Add(
                        new CharacterDeathsViewModel
                        {
                            Name = item.CharacterName,
                            Killers = item1.Killers.Split(';').ToList(),
                            Level = item1.Level,
                            TimeOfDeath = item1.Date.ToString(),
                            Guild = (GuildEnum)item1.Guild
                        });
                }
            }

            var itemz = GetMembersFromDb();

            List<Member> newListOfMemberz = new List<Member>();

            foreach (var item in itemz)
            {
                var splittedList = item.Members.Split(';');

                foreach (var givenGuild in splittedList)
                {
                    newListOfMemberz.Add(new Member
                    {
                        Members = givenGuild,
                        Guild = item.Guild
                    });
                }
            }

            foreach (var characz in model)
            {
                int counter = 0;

                bool isSameGuild = false;

                foreach (var pi in characz.Killers)
                {
                    isSameGuild = false;
                    var killer = newListOfMemberz.FirstOrDefault(x => x.Members.Equals(pi));

                    if (characz.Killers.Count() == 1)
                    {
                        if (killer != null && killer.Guild == characz.Guild)
                        {
                            isSameGuild = true;
                            break;
                        }

                        if (killer == null)
                            isSameGuild = true;
                    }
                    else
                    {
                        if (killer != null && killer.Guild == characz.Guild)
                        {
                            continue;
                        }
                    }

                    if (newListOfMemberz.FirstOrDefault(x => x.Members.Equals(pi)) != null)
                    {
                        counter += 1;
                    }
                }

                if (!isSameGuild && counter >= characz.Killers.Count / 2)
                {
                    newModel.Add(characz);
                }
            }

            return newModel;
        }

        private List<Member> GetMembersFromDb()
        {
            string connetionString = "Data Source=SQL6003.site4now.net;Initial Catalog=DB_A3C068_showland;User Id=DB_A3C068_showland_admin;Password=kacperQ123;";

            string sql = $"select top 2 [Members], [Guild] from [dbo].[MembersList] order by date desc";

            List<string> memberAsString = new List<string>();

            List<Member> allMembers = new List<Member>();

            using (SqlConnection connection = new SqlConnection(connetionString))
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            allMembers.Add(new Member
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
    public class Member
    {
        public string Members { get; set; }
        public GuildEnum Guild { get; set; }
    }

    public enum GuildEnum
    {
        Showland,   
        Reapers
    }
}
}
