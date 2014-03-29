using System.Collections.Generic;
using System.Configuration;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class Member
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string DefaultRoleId { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public Member(string name, string shortName, string userName, string password, string email, string defaultRoleId)
        {
            Name = name;
            ShortName = shortName;
            UserName = userName;
            Password = password;
            Email = email;
            DefaultRoleId = defaultRoleId;
        }
        #endregion

        #region "STATIC METHODS"

        public static void AddMembers(string name, Project project)
        {
            project.Members.Add(new Member(name + " Admin", name + "Admin", name + "Admin", "password", name + "Admin@versionone.com", Utils.ProjectAdmin));
            project.Members.Add(new Member(name + " Team Member 1", name + "TM1", name + "TM1", "password", "tm1@versionone.com", Utils.TeamMember));
            project.Members.Add(new Member(name + " Team Member 2", name + "TM2", name + "TM2", "password", "tm2@versionone.com", Utils.TeamMember));
        }

        public static void SaveMembers(Project project)
        {
            IAssetType memberType = Program.MetaModel.GetAssetType("Member");
            IAttributeDefinition memberName = memberType.GetAttributeDefinition("Name");
            IAttributeDefinition memberUserName = memberType.GetAttributeDefinition("Username");
            IAttributeDefinition memberPassword = memberType.GetAttributeDefinition("Password");
            IAttributeDefinition memberNickname = memberType.GetAttributeDefinition("Nickname");
            IAttributeDefinition memberEmail = memberType.GetAttributeDefinition("Email");
            IAttributeDefinition memberDefaultRole = memberType.GetAttributeDefinition("DefaultRole");
            IAttributeDefinition memberScopes = memberType.GetAttributeDefinition("Scopes");
            IList<Member> members = project.Members;

            foreach (Member member in members)
            {
                Asset asset = Program.Services.New(memberType, Oid.Null);
                asset.SetAttributeValue(memberName, member.Name);
                asset.SetAttributeValue(memberUserName, member.UserName);
                asset.SetAttributeValue(memberPassword, member.Password);
                asset.SetAttributeValue(memberNickname, member.ShortName);
                asset.SetAttributeValue(memberEmail, member.Email);
                asset.SetAttributeValue(memberDefaultRole, Oid.FromToken(member.DefaultRoleId, Program.MetaModel));
                asset.AddAttributeValue(memberScopes, Oid.FromToken(project.Id, Program.MetaModel));

                try
                {
                    Program.Services.Save(asset);
                    member.Id = asset.Oid.Momentless.Token;
                }
                catch
                {
                    Utils.Logger.Error("ERROR SAVING MEMBER: " + member.Name);
                    Utils.Logger.Error("ATTEMPTING TO LOOKUP");

                    if ("TRUE".Equals( ConfigurationManager.AppSettings["IfExistingMembersThenMatch"]))
                    {
                        QueryToFindExistingMember(member);
                    }
                    else
                    {
                        Utils.Logger.Error("REMOVING MEMBERS CREATED WITH THIS RUN");
                        RemoveCreatedMembers(members);
                        break;
                    }
                }
            }
        }

        private static void QueryToFindExistingMember(Member member)
        {
            IAssetType memberType = Program.MetaModel.GetAssetType("Member");
            IAttributeDefinition memberUserName = memberType.GetAttributeDefinition("Username");
            Query query = new Query(memberType);
            query.Selection.Add(memberUserName);

            FilterTerm term = new FilterTerm(memberUserName);
            term.Equal(member.UserName);
            query.Filter = term;

            QueryResult result = Program.Services.Retrieve(query);
            member.Id = result.Assets[0].Oid.Momentless.Token;
        }

        public static void RemoveCreatedMembers(IList<Member> members)
        {
            IOperation delete = Program.MetaModel.GetOperation("Member.Delete");

            foreach (Member member in members)
            {
                if (member.Id != null)
                {
                    Program.Services.ExecuteOperation(delete, Oid.FromToken(member.Id, Program.MetaModel));
                }
            }
        }
        #endregion
    }
}
