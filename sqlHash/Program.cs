using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace sqlHash
{
    class Program
    {
        const string HashableObject_TABLE = "TABLE";
        const string HashableObject_VIEW = "VIEW";
        public static SqlConnectionStringBuilder thisBuilder; // todo: determine if global is most efficient way to to this
        public static SqlConnection thisConnection;
        public static string thisConnectString = "";
        // NOTE: This has not been tested with named instances. 
        // todo: nice but not necessary - add timers to show time run when in verbose mode
        // todo: long term consideration - hash the contents as well - or as a half measure - the number of records?? Note: Hashing an entire db is not trivial - good reason to do it?

        static void Main(string[] args)
        {
            bool VerboseOutput = ConsoleHelper.deriveParameter(args, "/v", "False").ToUpper() == "";
            bool ExtraStructure = ConsoleHelper.deriveParameter(args, "/s", "False").ToUpper() == "";
            thisConnectString = ConsoleHelper.deriveParameter(args, "/c", "");
            if (string.IsNullOrEmpty(thisConnectString))
            {
                Console.Write("Must provide a connect string - for examples:\nServer=ServerName;Database=DatabaseName;User Id=myLocalUserID;Password=*********;\nServer=ServerName;Database=DatabaseName;Trusted_Connection=True;");
                Environment.ExitCode = 1;
                return;
            }
            if (VerboseOutput) { Console.WriteLine($"Connection String:[{thisConnectString}]"); }

            SqlDataReader tables2Hash = openSqlReader(thisConnectString, "sp_tables;");
            var AllTableHashes = new List<string>();
            while (tables2Hash.Read())
            {
                if (notSys(tables2Hash))
                {
                    //todo - add switch to also list the fields - nice to see for humans reading
                    var thisTableStructures = getStructureSummary(tables2Hash);
                    //var thisTableValues = $"{getStructureSummary(tables2Hash)} {tables2Hash.GetValue(2)}";
                    var thisTableValues = $"{CryptoHelper.HashStringMD5(thisTableStructures)} {tables2Hash.GetValue(2)}";
                    if (ExtraStructure) thisTableValues += $" [{thisTableStructures}]";
                    if (VerboseOutput) { AllTableHashes.Add(thisTableValues); } // only do this if we're hashing all and displaying after
                    Console.WriteLine(thisTableValues); 
                }
            }
            if (VerboseOutput) { Console.WriteLine($"{CryptoHelper.HashStringMD5(String.Join("", AllTableHashes))} ** Hash of Hashes **"); }
        }

        private static bool notSys(SqlDataReader tablesToIterate)
        {
            try
            {
                var theType = $"{tablesToIterate.GetString(1)}";
                return !(theType == "sys" || theType == "INFORMATION_SCHEMA"); // todo: make this into an array - and a switch to use it or not - user should have choice..
            }
            catch { return false; }
        }

        private static SqlDataReader openSqlReader(string givenConnectString, string givenSQL)
        {
            thisBuilder = new SqlConnectionStringBuilder(givenConnectString);
            thisConnection = new SqlConnection(thisBuilder.ConnectionString);
            thisConnection.Open();
            SqlCommand command = new SqlCommand(givenSQL, thisConnection);
            SqlDataReader result = command.ExecuteReader();
            return result;
        }

        static string getStructureSummary(SqlDataReader givenObj)
        {
            var result = "";
            // name and type
            var theName = givenObj.GetString(2);
            var theType = givenObj.GetString(3);            
            switch (theType)
            {
                case (HashableObject_TABLE):
                case (HashableObject_VIEW):
                    result = getStructureSummary4Table(theName);
                    break;
                // further room for other things like indexes and such..
                default:
                    result = $"Object:{theName},Type:{theType} NOT Hashable";
                    break;
            }
            return result;
        }

        public static string getStructureSummary4Table(string theName)
        {
            string result = "";
            // open table 0 records - just to get the structure in our reader
            SqlDataReader tableToHash = openSqlReader(thisConnectString , $"SELECT TOP 0 * FROM dbo.{theName}");
            tableToHash.Read();
            // iterate through fields collecting name, type and length into a string
            var fields = new List<string>();
            for (int fieldIndex = 0; fieldIndex < tableToHash.FieldCount; fieldIndex++)
            {
                fields.Add($"{tableToHash.GetName(fieldIndex)}({tableToHash.GetDataTypeName(fieldIndex)})"); //todo - length & nullable (Note: will change hash)
            }
            //result = CryptoHelper.HashStringMD5(string.Join("", fields));
            result = string.Join("", fields);

            return result;
        }

    }
}
