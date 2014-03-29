using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;

namespace VersionOne.Data.Generator
{
    public class DBStore
    {
        #region "FIELDS"
        private readonly SqlConnection _conn;
        #endregion

        #region "CONSTRUCTORS"
        public DBStore(SqlConnection conn)
        {
            _conn = conn;
        }
        #endregion

        #region "METHODS"
        public IDictionary<string, StringCollection> GetDateTables()
        {
            const string sql = @"SELECT sysobjects.name as TableName, syscolumns.name AS ColumnName, systypes.name AS Datatype 
	                           FROM sysobjects, syscolumns, systypes 
	                           WHERE sysobjects.id = syscolumns.id 
	                           AND syscolumns.xtype = systypes.xtype 
	                           AND sysobjects.type = 'U' 
	                           AND systypes.name = 'datetime'";
            SqlCommand command = new SqlCommand(sql, _conn);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            IDictionary<string, StringCollection> tables = new Dictionary<string, StringCollection>();
            while (reader.Read())
            {
                string tableName = reader.GetString(0);
                if (!tables.ContainsKey(tableName))
                {
                    tables.Add(tableName, new StringCollection());
                }
                tables[tableName].Add(reader.GetString(1));
            }
            reader.Close();
            return tables;
        }

        public StringCollection GenerateSQL(IDictionary<string, StringCollection> dateTables, int daysToAdd)
        {
            StringCollection sqlStatements = new StringCollection();
            IEnumerator<KeyValuePair<string, StringCollection>> enumer = dateTables.GetEnumerator();
            while (enumer.MoveNext())
            {
                string sql = "UPDATE " + enumer.Current.Key + " SET ";
                StringCollection columnNames = enumer.Current.Value;
                for (int i = 0; i < columnNames.Count; i++)
                {
                    sql += columnNames[i] + " = DATEADD(DAY, " + daysToAdd + ", " + columnNames[i] + ")";
                    if (i != columnNames.Count - 1)
                    {
                        sql += ", ";
                    }
                }
                sqlStatements.Add(sql);
            }
            return sqlStatements;
        }

        public void UpdateDates(StringCollection sqlStatements)
        {
            foreach (string sql in sqlStatements)
            {
                SqlCommand command = new SqlCommand(sql, _conn);
                command.ExecuteNonQuery();
            }
        }
        #endregion
    }
}