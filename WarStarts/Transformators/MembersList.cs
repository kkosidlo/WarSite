using System.Collections.Generic;
using WarStarts.Classes;

namespace WarStarts.Transformators
{
    public static class MembersList
    {
        public static List<Player> StringMembersListToListObject(List<GuildMembers> membersAsString)
        {
            List<Player> playersList = 
                new List<Player>();

            foreach (var guildMembers in membersAsString)
            {
                foreach (var member in guildMembers.Members.Split(';'))
                {
                    playersList.Add(
                        new Player
                        {
                            Name = member,
                            Guild = guildMembers.Guild
                        });
                }
            }

            return playersList;
        }
    }
}