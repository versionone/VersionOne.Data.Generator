using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class Team
    {
        #region "FIELDS"
        public static readonly Team TeamAlpha = new Team("Alpha");
        public static readonly Team TeamBeta = new Team("Beta");
        #endregion

        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public Team(string name)
        {
            Name = name;
        }
        #endregion

        #region "STATIC METHODS"
        public static IList<Team> GetAllTeams()
        {
            return new List<Team> {TeamAlpha, TeamBeta};
        }

        public static void SaveTeams(IList<Team> teams)
        {
            IAssetType teamType = Program.MetaModel.GetAssetType("Team");
            IAttributeDefinition teamName = teamType.GetAttributeDefinition("Name");

            foreach (Team team in teams)
            {
                Asset asset = Program.Services.New(teamType, Oid.Null);
                asset.SetAttributeValue(teamName, team.Name);
                Program.Services.Save(asset);
                team.Id = asset.Oid.Momentless.Token;
            }
        }
        #endregion
    }
}
