using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WarStarts.Classes;
using WarStarts.Enum;
using WarStarts.Helpers;
using WarStarts.Jobs;
using WarStarts.Models;

namespace WarStarts.Controllers
{
    public class HomeController : Base
    {
        private const string TibiaGuildUrl = "https://secure.tibia.com/community/?subtopic=guilds&page=view&order=level_desc&GuildName=";
        private const string TibiaCharacterUrl = "https://secure.tibia.com/community/?subtopic=characters&name=";


        // GET: Home
        public ActionResult Index()
        {
            //DeathListCounter death = new DeathListCounter();
            //death.Proceed();

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

            foreach (var guild in GuildEnum.GetValues(typeof(GuildEnum)))
            {
                var response = HttpStatusCode.NoContent;
                string responseString = null;

                response = RequestManager.SendGETRequest($"{ TibiaGuildUrl }{ guild.ToString() }", out responseString);

                if (!String.IsNullOrEmpty(responseString))
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




        private List<CharacterDeathsViewModel> GetLastKillsList()
        {

            string connetionString = "Server=tcp:showland.database.windows.net,1433;Initial Catalog=showland;Persist Security Info=False;User ID=showland;Password=kacperQ123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
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

            List<GuildMembers> newListOfMemberz = new List<GuildMembers>();

            foreach (var item in itemz)
            {
                var splittedList = item.Members.Split(';');

                foreach (var givenGuild in splittedList)
                {
                    newListOfMemberz.Add(new GuildMembers
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
    }
}
