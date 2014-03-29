using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using VersionOne.SDK.APIClient;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace VersionOne.Data.Generator
{
    internal class Program
    {
        private readonly static string _v1Url = ConfigurationManager.AppSettings["v1Url"];
        private readonly static string _v1Username = ConfigurationManager.AppSettings["v1Username"];
        private readonly static string _v1Password = ConfigurationManager.AppSettings["v1Password"];
        private readonly static string _v1ConnectionString = ConfigurationManager.AppSettings["v1ConnectionString"];
        private readonly static int _v1ProjectSetCount = Convert.ToInt32(ConfigurationManager.AppSettings["v1ProjectSetCount"]);
        internal readonly static string _v1ClientTarget = ConfigurationManager.AppSettings["v1ClientTarget"];

        private static bool _isUltimate = false;
        private static bool _isCatalyst = false;
        private static bool _isEnteprisePlus = false;
        private static bool _useTeamRoom = false;

        private const string DATA_API = "/rest-1.v1/";
        private const string META_API = "/meta.v1/";
        public static Services Services;
        public static MetaModel MetaModel;

        public static void Main()
        {
            try
            {
                Console.WriteLine("**** VERSIONONE DATA GENERATOR ****\n");
                Utils.Logger.Info(String.Format("Client target: {0}", _v1ClientTarget));
                Run();
            }
            catch (Exception ex)
            {
                Utils.Logger.Error("An error occurred: " + ex.Message);
                Utils.Logger.Error(ex.StackTrace);
            }
            finally
            {
                Console.Write("\nHave a nice day!\n\n");
                Console.Write("Press ENTER to close: ");
                Console.ReadLine();
            }
        }

        private static void Run()
        {
            V1APIConnector dataConnector = new V1APIConnector(_v1Url + DATA_API, _v1Username, _v1Password);
            V1APIConnector metaConnector = new V1APIConnector(_v1Url + META_API);

            MetaModel = new MetaModel(metaConnector);
            Services = new Services(MetaModel, dataConnector);

            //Check if we need to add TeamRooms.
            if (ConfigurationManager.AppSettings["UseTeamRoom"] == "true")
                _useTeamRoom = true;

            Utils.Logger.Info("Verifying configuration...");
            VerifyConfiguration();

            Project.HideFutureStatus();

            if (_isCatalyst == false) 
                Team.SaveTeams(Team.GetAllTeams());

            //Use for loop to create multiple project sets based on config value. Each project set contains 40 distinct projects.
            for (int i = 0; i < _v1ProjectSetCount; i++)
            {
                Utils.Logger.Info("Creating projects...");
                IList<Project> projects = Project.GetProjects(i);

                Utils.Logger.Info("Saving projects...");
                Project.SaveProjects(projects, "Scope:0", null, null, _isUltimate, _isCatalyst, _isEnteprisePlus, _useTeamRoom);

                Utils.Logger.Info("Training data saved. Rolling back dates.");
                RollDatesBackOneDay();

                Utils.Logger.Info("Dates rolled back. Saving second day of data.");

                SaveSecondDay(projects);
                Utils.Logger.Info("Second day of data saved. Rolling back dates.");

                RollDatesBackOneDay();
                Utils.Logger.Info("Dates rolled back. Saving third day of data.");

                SaveThirdDay(projects);
                Utils.Logger.Info("Third day of data saved. Rolling back dates.");

                RollDatesBackOneDay();
                Utils.Logger.Info("Dates rolled back. Saving fourth day of data.");

                SaveFourthDay(projects);
                Utils.Logger.Info("Fourth day of data saved. Rolling back dates.");

                RollDatesBackOneDay();
                Utils.Logger.Info("Dates rolled back. Saving fifth day of data.");

                SaveFifthDay(projects);
                Utils.Logger.Info("Fifth day of data saved. Rolling back dates.");

                RollDatesBackOneDay();
                Utils.Logger.Info("Dates rolled back. Saving sixth day of data.");

                SaveSixthDay(projects);
                Utils.Logger.Info("Sixth day of data saved. Rolling back dates.");

                RollDatesBackOneDay();
                Utils.Logger.Info("Dates rolled back. Saving seventh day of data.");

                SaveSeventhDay(projects);
                Utils.Logger.Info("Seventh day of data saved.");

                ProcessClientSpecificData(projects);

                Utils.Logger.Info("Training data generation completed.");
            }
        }

        private static void VerifyConfiguration()
        {
            VerifyV1Url();
            VerifyDatabase();
            VerifyLicense();
            VerifyVersion();
        }

        private static void VerifyVersion()
        {
            try
            {
                string edition = String.Empty;

                //First check the config for Enterprise plus (not in database); otherwise check database.
                if (ConfigurationManager.AppSettings["IsEnteprisePlus"] == "true")
                    edition = "EnterprisePlus";
                else
                {
                    using (SqlConnection conn = new SqlConnection(_v1ConnectionString))
                    {
                        string SQL = "SELECT Value FROM SystemConfig WHERE Name = 'Product';";
                        SqlCommand cmd = new SqlCommand(SQL, conn);
                        conn.Open();
                        edition = (string)cmd.ExecuteScalar();
                    }
                }

                if (edition == "Ultimate")
                {
                    TryCreatingEnvironment();
                    Utils.Logger.Info("VersionOne edition: ULTIMATE");
                    _isUltimate = true;
                    _isCatalyst = false;
                }
                else if (edition == "Enterprise")
                {
                    Utils.Logger.Info("VersionOne edition: ENTERPRISE");
                    _isUltimate = false;
                    _isCatalyst = false;
                }
                else if (edition == "EnterprisePlus")
                {
                    Utils.Logger.Info("VersionOne edition: ENTERPRISE_PLUS");
                    _isUltimate = false;
                    _isCatalyst = false;
                    _isEnteprisePlus = true;
                }
                else if (edition == "Catalyst")
                {
                    Utils.Logger.Info("VersionOne edition: CATALYST");
                    _isUltimate = false;
                    _isCatalyst = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void TryCreatingEnvironment()
        {
            IAssetType environmentType = MetaModel.GetAssetType("Environment");
            IAttributeDefinition environmentName = environmentType.GetAttributeDefinition("Name");
            IAttributeDefinition environmenteScope = environmentType.GetAttributeDefinition("Scope");
            Asset asset = Services.New(environmentType, Oid.FromToken("Scope:0", MetaModel));
            asset.SetAttributeValue(environmentName, "TEST ENVIRONMENT NAME" );
            asset.SetAttributeValue(environmenteScope, Oid.FromToken("Scope:0", MetaModel));
            Services.Save(asset);
            IOperation inactivate = MetaModel.GetOperation("Environment.Inactivate");
            Services.ExecuteOperation(inactivate, Oid.FromToken(asset.Oid.Momentless.Token, MetaModel));
        }

        private static void VerifyV1Url()
        {
            try
            {
                Version ver = MetaModel.Version;
                Utils.Logger.Info("Using V1 API: " + ver.ToString());
            }
            catch (Exception e)
            {
                Utils.Logger.Error("Could not connect to the specified V1 instance: " + _v1Url + ". Check your configuration.");
                throw new ApplicationException("Could not connect to the specified V1 instance: " + _v1Url + ". Check your configuration.", e);
            }
        }

        private static void VerifyDatabase()
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(_v1ConnectionString);
                conn.Open();
                Utils.Logger.Info(String.Format("Connected to database: {0}", conn.ConnectionString));
            }
            catch (Exception ex)
            {
                Utils.Logger.Error("Could not connect to the specified database: " + _v1ConnectionString + ". Check your configuration.");
                throw new ApplicationException("Could not connect to the specified database: " + _v1ConnectionString + ". Check your configuration." + ex.Message, ex);
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        private static void VerifyLicense()
        {
            Project project = new Project("System", DateTime.Now, DateTime.Now);
            project.Id = "Scope:0";
            IList<Member> members = new List<Member>();
            for (int i = 0; i < 6; i++)
            {
                Member member = new Member("test", "test", Guid.NewGuid().ToString().Substring( 0, 16 ), "test", "test@versionone.com", Utils.TeamMember);
                members.Add(member);
            }
            project.Members = members;
            try
            {
                Member.SaveMembers(project);
                Utils.Logger.Info("VersionOne license verified.");
            }
            catch (Exception ex)
            {
                Utils.Logger.Info("You need to place the customer license file in the bin directory of the configured v1 instance: " + _v1Url);
                throw new ApplicationException("You need to place the customer license file in the bin directory of the configured v1 instance: " + _v1Url, ex);
            }
            finally
            {
                IOperation delete = MetaModel.GetOperation("Member.Delete");
                foreach (Member member in members)
                {
                    if (member.Id != null)
                    {
                        Services.ExecuteOperation(delete, Oid.FromToken(member.Id, MetaModel));
                    }
                }
            }
        }

        private static void RollDatesBackOneDay()
        {
            //Determine days to roll based on ClientTarget and if it is the last day.
            int daysToAdd = 0;
            if (_v1ClientTarget == "CapitalOne")
            {
                daysToAdd = -2;
            }
            else
            {
                daysToAdd = -1;
            }

            using (SqlConnection conn = new SqlConnection(_v1ConnectionString))
            {
                DBStore store = new DBStore(conn);
                conn.Open();
                IDictionary<string, StringCollection> tables = store.GetDateTables();
                StringCollection sqlStatements = store.GenerateSQL(tables, daysToAdd);
                store.UpdateDates(sqlStatements);
            }
        }

        private static void SaveSecondDay(IEnumerable<Project> projects)
        {
            foreach (Project project in projects)
            {
                Project release = project.Releases[0];
                Story searchAppts = release.Stories[Utils.SearchAvailable];
                Story.UpdateStory(searchAppts.Id, Utils.StatusInProgress);
                Task.UpdateTask(searchAppts.Tasks[0].Id, Utils.TaskStatusInProgress, -1);
                Task.UpdateTask(searchAppts.Tasks[1].Id, Utils.TaskStatusCompleted, 0);
                Story reserveTime = release.Stories[Utils.ReserveTime];
                Story.UpdateStory(reserveTime.Id, Utils.StatusInProgress);
                Task.UpdateTask(reserveTime.Tasks[2].Id, Utils.TaskStatusCompleted, -1);
                Task.UpdateTask(reserveTime.Tasks[3].Id, Utils.TaskStatusInProgress, 0);
                Task.UpdateTask(reserveTime.Tasks[4].Id, Utils.TaskStatusCompleted, 0);
                Task.UpdateTask(reserveTime.Tasks[5].Id, Utils.TaskStatusCompleted, 0);
            }
        }

        private static void SaveThirdDay(IEnumerable<Project> projects)
        {
            foreach (Project project in projects)
            {
                Project release = project.Releases[0];
                Story searchAppts = release.Stories[Utils.SearchAvailable];
                Task.UpdateTask(searchAppts.Tasks[0].Id, Utils.TaskStatusCompleted, 0);
                Story.UpdateStory(searchAppts.Id, Utils.StatusDone);
                Test.UpdateTest(searchAppts.Tests[0].Id, Utils.TestStatusFailed, 6);
                Story reserveTime = release.Stories[Utils.ReserveTime];
                Task.UpdateTask(reserveTime.Tasks[0].Id, Utils.TaskStatusInProgress, reserveTime.Tasks[0].Todo + 6);
                Task.UpdateTask(reserveTime.Tasks[1].Id, Utils.TaskStatusInProgress, reserveTime.Tasks[1].Todo + 6);
                Task.UpdateTask(reserveTime.Tasks[2].Id, Utils.TaskStatusInProgress, reserveTime.Tasks[2].Todo + 8);
            }
        }

        private static void SaveFourthDay(IEnumerable<Project> projects)
        {
            foreach (Project project in projects)
            {
                Project release = project.Releases[0];
                Story searchAppts = release.Stories[Utils.SearchAvailable];
                Test.UpdateTest(searchAppts.Tests[0].Id, Utils.TestStatusPassed, 0);
                Story.UpdateStory(searchAppts.Id, Utils.StatusAccepted);
                Story.CloseStory(searchAppts.Id);
                Story reserveTime = release.Stories[Utils.ReserveTime];
                Task.UpdateTask(reserveTime.Tasks[1].Id, Utils.TaskStatusCompleted, 0);
            }
        }

        private static void SaveFifthDay(IEnumerable<Project> projects)
        {
            foreach (Project project in projects)
            {
                Project release = project.Releases[0];
                Story searchStyles = release.Stories[Utils.SearchStyle];
                Story.UpdateStory(searchStyles.Id, Utils.StatusInProgress);
                Task.UpdateTask(searchStyles.Tasks[0].Id, Utils.TaskStatusCompleted, 0);
                Task.UpdateTask(searchStyles.Tasks[1].Id, Utils.TaskStatusInProgress, -1);
                Task.UpdateTask(searchStyles.Tasks[2].Id, Utils.TaskStatusCompleted, 0);
                Story reserveTime = release.Stories[Utils.ReserveTime];
                Task.UpdateTask(reserveTime.Tasks[2].Id, Utils.TaskStatusInProgress, 6);
            }
        }

        private static void SaveSixthDay(IEnumerable<Project> projects)
        {
            foreach (Project project in projects)
            {
                Project release = project.Releases[0];
                Story reserveTime = release.Stories[Utils.ReserveTime];
                Task.UpdateTask(reserveTime.Tasks[0].Id, Utils.TaskStatusCompleted, 0);
                Task.UpdateTask(reserveTime.Tasks[2].Id, Utils.TaskStatusCompleted, 0);
                Test.UpdateTest(reserveTime.Tests[0].Id, Utils.TestStatusPassed, 0);
                Story.UpdateStory(reserveTime.Id, Utils.StatusAccepted);
            }
        }

        private static void SaveSeventhDay(IEnumerable<Project> projects)
        {
            foreach (Project project in projects)
            {
                Project release = project.Releases[0];
                Story searchStyles = release.Stories[Utils.SearchStyle];

                Task.UpdateTask(searchStyles.Tasks[1].Id, Utils.TaskStatusCompleted, 0);
                Task.UpdateTask(searchStyles.Tasks[3].Id, Utils.TaskStatusCompleted, 0);
                Task.UpdateTask(searchStyles.Tasks[4].Id, Utils.TaskStatusCompleted, 0);
                Test.UpdateTest(searchStyles.Tests[0].Id, Utils.TestStatusPassed, 0);
                Story.UpdateStory(searchStyles.Id, Utils.StatusAccepted);
                Story.CloseStory(searchStyles.Id);

                Story cancelRes = release.Stories[Utils.CancelReservation];
                Story.UpdateStory(cancelRes.Id, Utils.StatusInProgress);
                Task.UpdateTask(cancelRes.Tasks[2].Id, Utils.TaskStatusCompleted, 0);
            }
        }

        private static void ProcessClientSpecificData(IEnumerable<Project> projects)
        {
            switch (_v1ClientTarget)
            {
                case "CapitalOne":
                {
                    Utils.Logger.Info("Processing client specific data for CapitalOne...");
                    foreach (Project project in projects)
                    {
                        if (project.Iterations.Count > 0)
                        {
                            //Close all items in the first sprint.
                            Oid sprintID = Oid.FromToken(project.Iterations[0].Id, MetaModel);
                            IAssetType workItemType = MetaModel.GetAssetType("Workitem");

                            Query query = new Query(workItemType);
                            IAttributeDefinition timeboxAttribute = workItemType.GetAttributeDefinition("Timebox");
                            query.Selection.Add(timeboxAttribute);

                            FilterTerm term = new FilterTerm(timeboxAttribute);
                            term.Equal(sprintID.Token);
                            query.Filter = term;
                            QueryResult result = Services.Retrieve(query);

                            IOperation operation = MetaModel.GetOperation("Story.QuickClose");
                            foreach (Asset workitem in result.Assets)
                            {
                                try
                                {
                                    //Only close stories and defects.
                                    string[] assetType = workitem.Oid.ToString().Split(':');
                                    if (assetType[0] == "Story" || assetType[0] == "Defect")
                                        Services.ExecuteOperation(operation, workitem.Oid);
                                }
                                catch
                                {
                                    //Skip items that are closed.
                                    continue;
                                }
                            }

                            //Close the first sprint.
                            operation = MetaModel.GetOperation("Timebox.Close");
                            Services.ExecuteOperation(operation, sprintID);
                        }
                    }
                    break;
                }
                default:
                    break;
            }
        }
    }
}