using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class TeamRoom
    {
        #region "STATIC METHODS"

        public static Asset CreateTeamRoom(Project project, Asset schedule)
        {
            IAssetType assetType = Program.MetaModel.GetAssetType("TeamRoom");
            IAttributeDefinition nameAttribute = assetType.GetAttributeDefinition("Name");
            IAttributeDefinition scheduleAttribute = assetType.GetAttributeDefinition("Schedule");
            IAttributeDefinition scopeAttribute = assetType.GetAttributeDefinition("Scope");
            IAttributeDefinition participantAttribute = assetType.GetAttributeDefinition("Participants");
            
            Asset newTeamRoom = Program.Services.New(assetType, null);
            newTeamRoom.SetAttributeValue(nameAttribute, project.Name + " TeamRoom");
            newTeamRoom.SetAttributeValue(scheduleAttribute, schedule.Oid);
            newTeamRoom.SetAttributeValue(scopeAttribute, project.Id);

            foreach (Member member in project.Members)
            {
                newTeamRoom.AddAttributeValue(participantAttribute, member.Id);
            }

            Program.Services.Save(newTeamRoom);
            return newTeamRoom;
        }
        #endregion
    }
}
