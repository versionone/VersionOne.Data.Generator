using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace VersionOne.Data.Generator
{
    static class Utils
    {
        #region "WORKITEM CONSTANTS"
        public const string PriorityHigh = "WorkitemPriority:140";
        public const string PriorityMedium = "WorkitemPriority:139";
        public const string PriorityLow = "WorkitemPriority:138";
        public static readonly string StatusNone = String.Empty;
        public const string StatusFuture = "StoryStatus:133";
        public const string StatusInProgress = "StoryStatus:134";
        public const string StatusDone = "StoryStatus:135";
        public const string StatusAccepted = "StoryStatus:137";    
        #endregion

        #region "TASK CONSTANTS"
        public const string TaskStatusInProgress = "TaskStatus:123";
        public const string TaskStatusCompleted = "TaskStatus:125";
        #endregion

        #region "ACCEPTANCE TEST CONSTANTS"
        public const string TestStatusPassed = "TestStatus:129";
        public const string TestStatusFailed = "TestStatus:155";
        #endregion

        #region "GOAL CONSTANTS"
        public const string GoalPriorityHigh = "GoalPriority:189";
        public const string GoalPriorityMedium = "GoalPriority:188";
        public const string GoalPriorityLow = "GoalPriority:187";
        public const string GoalCategoryCompetitive = "GoalCategory:190";
        public const string GoalCategoryFinancial = "GoalCategory:191";
        public const string GoalCategoryMarket = "GoalCategory:192";
        public const string GoalCategoryOperations = "GoalCategory:193";
        public const string GoalCategoryStrategic = "GoalCategory:194";
        #endregion

        #region "REQUEST CONSTANTS"
        public const string RequestPriorityHigh = "RequestPriority:169";
        public const string RequestPriorityMedium = "RequestPriority:168";
        public const string RequestPriorityLow = "RequestPriority:167";
        public const string RequestStatusReviewed = "RequestStatus:170";
        public const string RequestStatusApproved = "RequestStatus:171";
        public const string RequestStatusRejected = "RequestStatus:172";
        #endregion

        #region "ISSUE CONSTANTS"
        public const string IssuePriorityHigh = "IssuePriority:161";
        public const string IssuePriorityMedium = "IssuePriority:160";
        public const string IssuePriorityLow = "IssuePriority:159";
        public const string IssueTypeImpediment = "IssueCategory:164";
        #endregion

        #region "BUILDRUN CONSTANTS"
        public const string BuildRunStatusPassed = "BuildStatus:195";
        public const string BuildRunStatusFailed = "BuildStatus:196";
        public const string BuildRunSourceTrigger = "BuildSource:197";
        public const string BuildRunSourceForced = "BuildSource:198";
        #endregion

        #region "PROJECT CONSTANTS"
        public const string TeamMember = "Role:4";
        public const string ProjectAdmin = "Role:2";
        public const string ProjectLead = "Role:3";
        #endregion

        #region "MISC CONSTANTS"
        public static int SearchAvailable = 8;
        public static int SearchStyle = 9;
        public static int ReserveTime = 10;
        public static int CancelReservation = 11;
        public static int ModifyReservation = 12;
        #endregion

        public static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static DateTime GetTodaysDateNoTime()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

    }
}
