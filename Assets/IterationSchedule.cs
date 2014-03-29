using System;
using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class IterationSchedule
    {
        #region "PROPERTIES"
        public int Length { get; private set; }
        public int Gap { get; private set; }
        #endregion

        #region "CONSTRUCTORS"
        public IterationSchedule(int length, int gap)
        {
            Length = length;
            Gap = gap;
        }
        #endregion

        #region "STATIC METHODS"

        public static Asset CreateSchedule(string name, IterationSchedule schedule)
        {
            IAssetType scheduleType = Program.MetaModel.GetAssetType("Schedule");
            IAttributeDefinition scheduleName = scheduleType.GetAttributeDefinition("Name");
            IAttributeDefinition scheduleLength = scheduleType.GetAttributeDefinition("TimeboxLength");
            IAttributeDefinition scheduleGap = scheduleType.GetAttributeDefinition("TimeboxGap");

            Asset newSchedule = Program.Services.New(scheduleType, null);
            newSchedule.SetAttributeValue(scheduleName, name + " Schedule");
            newSchedule.SetAttributeValue(scheduleLength, schedule.Length);
            newSchedule.SetAttributeValue(scheduleGap, schedule.Gap);

            Program.Services.Save(newSchedule);
            return newSchedule;
        }

        public static Asset GetDefaultSchedule()
        {
            IAssetType assetType = Program.MetaModel.GetAssetType("Schedule");
            Query query = new Query(assetType);
            IAttributeDefinition nameAttribute = assetType.GetAttributeDefinition("Name");
            query.Selection.Add(nameAttribute);
            query.Find = new QueryFind("Default Schedule", new AttributeSelection(nameAttribute));
            QueryResult result = Program.Services.Retrieve(query);
            return result.Assets[0];
        }
        #endregion
    }
}
