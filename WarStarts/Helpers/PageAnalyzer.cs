using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarStarts.Models;
using static WarStarts.Controllers.HomeController;

namespace WarStarts.Helpers
{
    public class PageAnalyzer
    {
        public List<Character> AnalyzePage(HtmlNodeCollection collection, GuildEnum guild)
        {
            List<Character> list = new List<Character>();

            for (int j = 0; j < collection.Count(); j++)
            {
                Character character = new Character();

                if (collection[j].ChildNodes.Count.Equals(11))
                {
                    if (j == 10)
                    {
                        for (int i = 1; i < collection[j].ChildNodes.Count; i += 2)
                        {
                            switch (i)
                            {
                                case 1:
                                    var index = collection[j].ChildNodes[i].InnerText.Replace("&#160;", " ").LastIndexOf(" (");
                                    if (index > 0)
                                        character.CharacterName = collection[j].ChildNodes[i].InnerText.Replace("&#160;", " ").Substring(0, index);
                                    else
                                        character.CharacterName = collection[j].ChildNodes[i].InnerText.Replace("&#160;", " ");
                                    break;
                                case 3:
                                    character.Vocation = collection[j].ChildNodes[i].InnerText;
                                    break;
                                case 5:
                                    character.Level = int.Parse(collection[j].ChildNodes[i].InnerText);
                                    break;
                                case 9:
                                    character.Status = collection[j].ChildNodes[i].InnerText;
                                    break;
                            }
                        }

                        character.Guild = guild;

                        list.Add(character);
                    }
                    else
                    {
                        for (int i = 2; i < collection[j].ChildNodes.Count; i += 2)
                        {
                            switch (i)
                            {
                                case 2:
                                    var index = collection[j].ChildNodes[i].InnerText.Replace("&#160;", " ").LastIndexOf(" (");
                                    if (index > 0)
                                        character.CharacterName = collection[j].ChildNodes[i].InnerText.Replace("&#160;", " ").Substring(0, index);
                                    else
                                        character.CharacterName = collection[j].ChildNodes[i].InnerText.Replace("&#160;", " ");
                                    break;
                                case 4:
                                    character.Vocation = collection[j].ChildNodes[i].InnerText;
                                    break;
                                case 6:
                                    character.Level = int.Parse(collection[j].ChildNodes[i].InnerText);
                                    break;
                                case 10:
                                    character.Status = collection[j].ChildNodes[i].InnerText;
                                    break;
                            }
                        }

                        character.Guild = guild;

                        list.Add(character);
                    }
                }
            }

            return list;
        }

        // not in use yet, backup plan when an API goes down
        //public static List<Death> ParseCharacterHtmlPage(string responseString)
        //{
        //    HtmlDocument htmlDoc = new HtmlDocument();

        //    htmlDoc.LoadHtml(responseString);

        //    var htmlbody = htmlDoc.DocumentNode.SelectSingleNode("//body");

        //    var page = htmlbody.SelectNodes("//div[@class='BoxContent']//table")[2].SelectNodes("//tr");

        //    List<Death> deaths = new List<Death>();


        //    for (int i = 0; i < page.Count(); i++)
        //    {
        //        if (page[i].InnerText == "Character Deaths")
        //        {
        //            for (int j = i + 1; ; j++)
        //            {
        //                Death death = new Death();

        //                if (page[j].InnerText == "Search Character" || page[j].InnerText == "Account Information")
        //                    break;

        //                foreach (var item in page[j].ChildNodes)
        //                {
        //                    if (item.ChildNodes.Count == 1)
        //                    {
        //                        DateTime result;

        //                        if (DateTime.TryParse(item.ChildNodes[0].InnerText.Replace("&#160;", " ").Replace(" CEST", ""), out result))
        //                            death.TimeOfDeath = result;
        //                    }
        //                    else
        //                    {
        //                        for (int z = 1; z < item.ChildNodes.Count; z += 2)
        //                        {

        //                            Character car = new Character
        //                            {
        //                                CharacterName = item.ChildNodes[z].InnerText.Replace("&#160;", " ")
        //                            };

        //                            death.Killers.Add(car);
        //                        }
        //                    }
        //                }

        //                deaths.Add(death);
        //            };
        //        }
        //    }

        //    return deaths;
        //}

    }
}