VersionOne.Data.Generator
=========================

The VersionOne Data Generator is a command line tool used by the openAgile team for creating sample data for automated testing.

The tool was initially used by the Services team for creating training data for product training classes, and it created multiple member accounts and projects so that each student could practice what they were learning.

### System Requirements ###

- Visual Studio 2013
- SQL Server 2008/2012
- .NET Framework 4.5.1
- VersionOne


### Notes ###

While the tool does the majority of its work using the VersionOne .NET API Client, it can only be used with an on-premise instance of VersionOne as it makes some changes directly in the VersionOne database. Therefore, direct SQL Server database access is required which is not available when using a hosted instance of VersionOne.

The tool is controlled by the app.config file which contains configurations for the V1 instance, the SQL Server database, logging, and a few other configurations that toggle how the sample data is constructed.
