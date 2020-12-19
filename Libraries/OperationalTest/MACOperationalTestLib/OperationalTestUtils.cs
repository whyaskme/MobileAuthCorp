using System;
using System.Configuration;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using MACSecurity;
using MACServices;
using cs = MACServices.Constants.Strings;
using dk = MACServices.Constants.Dictionary.Keys;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using ot = MACOperationalTestLib.OperationalTestConstants;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;

namespace MACOperationalTestLib
{
    public class OperationalTestUtils
    {
        private MongoClient mongoClient;
        private MongoServer mongoServer;
        private MongoDatabase mongoDBConnectionPool;
        private MongoCollection mongoCollection;

        public OperationalTestUtils()
        {
            mongoDBConnectionPool = CreateApplicationDBConnectionPool();
        }

        #region Connection Pool Methods
        public MongoDatabase CreateApplicationDBConnectionPool()
        {
            // Parse dbserver credentials
            string[] dbCredentials;
            // Parse dbserver settings
            string[] dbServerPort;
            var dbConnectionString = ConfigurationManager.ConnectionStrings[cfgcs.OperationalTestServer].ConnectionString;

            var connectionMode = ConfigurationManager.AppSettings[cs.ConnectionMode];
            var readPreference = ConfigurationManager.AppSettings[cs.ReadPreference];
            var writeConcern = ConfigurationManager.AppSettings[cs.WriteConcern];

            var mongoClientSettings = new MongoClientSettings();

            var dbConnectionTimeout = new TimeSpan(0, 0, 0,
                Convert.ToInt16(ConfigurationManager.AppSettings[cs.ConnectionTimeoutSeconds]), 0);
            mongoClientSettings.ConnectTimeout = dbConnectionTimeout;

            mongoClientSettings.MinConnectionPoolSize =
                Convert.ToInt16(ConfigurationManager.AppSettings[cs.MinDBConnections]);
            mongoClientSettings.MaxConnectionPoolSize =
                Convert.ToInt16(ConfigurationManager.AppSettings[cs.MaxDBConnections]);

            if (dbConnectionString.Contains(cs.ReplicaSetFlag))
            {
                // MongoS service connecting to a ReplicaSet running on AWS cluster
                const string dbServer = "localhost";
                const string dbPort = "27019";

                mongoClientSettings.Server = new MongoServerAddress(dbServer, Convert.ToInt16(dbPort));
            }
            else
            {
                var dbConnectionSettings = dbConnectionString.Split('@');

                // Server credentials
                dbCredentials = dbConnectionSettings[0].Split(':');
                var dbUserName = dbCredentials[1].Replace("//", "");
                var dbPassword = dbCredentials[2];

                // Server settings
                dbServerPort = dbConnectionSettings[1].Split(':');
                var dbServer = dbServerPort[0];
                var dbPort = dbServerPort[1];

                mongoClientSettings.Credentials = new[]
                {
                    MongoCredential.CreateMongoCRCredential(
                        ConfigurationManager.AppSettings[Constants.WebConfig.AppSettingsKeys.MongoDbOperationalTestDBName],
                        dbUserName, dbPassword)
                };
                mongoClientSettings.Server = new MongoServerAddress(dbServer, Convert.ToInt16(dbPort));
            }

            switch (connectionMode)
            {
                case cs.Automatic:
                    mongoClientSettings.ConnectionMode = ConnectionMode.Automatic;
                    break;
                case cs.Direct:
                    mongoClientSettings.ConnectionMode = ConnectionMode.Direct;
                    break;
                case cs.ReplicaSet:
                    mongoClientSettings.ConnectionMode = ConnectionMode.ReplicaSet;
                    break;
                case cs.ShardRouter:
                    mongoClientSettings.ConnectionMode = ConnectionMode.ShardRouter;
                    break;
            }

            switch (readPreference)
            {
                case cs.Nearest:
                    mongoClientSettings.ReadPreference = ReadPreference.Nearest;
                    break;
                case cs.Primary:
                    mongoClientSettings.ReadPreference = ReadPreference.Primary;
                    break;
                case cs.PrimaryPreferred:
                    mongoClientSettings.ReadPreference = ReadPreference.PrimaryPreferred;
                    break;
                case cs.Secondary:
                    mongoClientSettings.ReadPreference = ReadPreference.Secondary;
                    break;
                case cs.SecondaryPreferred:
                    mongoClientSettings.ReadPreference = ReadPreference.SecondaryPreferred;
                    break;
            }

            switch (writeConcern)
            {
                case cs.Acknowledged:
                    mongoClientSettings.WriteConcern = WriteConcern.Acknowledged;
                    break;
                case cs.Unacknowledged:
                    mongoClientSettings.WriteConcern = WriteConcern.Unacknowledged;
                    break;
            }

            mongoClient = new MongoClient(mongoClientSettings);
            mongoServer = mongoClient.GetServer();
            mongoDBConnectionPool =
                mongoServer.GetDatabase(ConfigurationManager.AppSettings[Constants.WebConfig.AppSettingsKeys.MongoDbOperationalTestDBName]);

            return mongoDBConnectionPool;
        }
        #endregion

        public OperationalTest Read()
        {
            try
            {
                mongoDBConnectionPool = CreateApplicationDBConnectionPool();
                var query = Query.EQ("_t", ot.OperationalTestCollectionName);
                MongoCollection mCollection = mongoDBConnectionPool.GetCollection(ot.OperationalTestCollectionName);
                var mOperationalTest = mCollection.FindOneAs<OperationalTest>(query);
                if (mOperationalTest != null) return mOperationalTest;
                var mOperationalTestNew = new OperationalTest();
                mOperationalTestNew._id = ObjectId.GenerateNewId();
                mOperationalTestNew._t = ot.OperationalTestCollectionName;
                mOperationalTestNew.TestSystemName = "Opts Test";
                mOperationalTestNew.TestSystemIp = "127.0.0.1";
                mOperationalTestNew.TestSystemHost = "localhost";
                mOperationalTestNew.LastTimeTestRan = mOperationalTestNew.LastUpdate = DateTime.UtcNow;
                mOperationalTestNew.ResultsLookupDays = 10;
                mOperationalTestNew.RunInterval = 60;
                mOperationalTestNew.SmsSettings = new OperationalTestSmsSettings();
                mOperationalTestNew.EmailSettings = new OperationalTestEmailSettings();
                mOperationalTestNew.SystemsUnderTestList = new List<OperationalTestSystemUnderTest>();
                mCollection.Insert(mOperationalTestNew);
                return mOperationalTestNew;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string Update(OperationalTest pOperationalTestObject)
        {
            try
            {
                mongoDBConnectionPool = CreateApplicationDBConnectionPool();
                mongoCollection = mongoDBConnectionPool.GetCollection(ot.OperationalTestCollectionName);
                //var query = Query.EQ("_id", pOperationalTestObject._id);
                //var sortBy = SortBy.Descending("_id");

                mongoCollection.Save(pOperationalTestObject);

                //mongoCollection.FindAndRemove(query, sortBy);
                //mongoCollection.Insert(pOperationalTestObject);
            }
            catch (Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.ToString();
                return null;
            }
            return "update succeeded";
        }

        public Dictionary<String, String> GetTestRunsBySystemUnderTestNameAndDateRange(String pSUTName, DateTime pStartDateTime, DateTime pEndDateTime, String pPassOrFail)
        {
            var mDict = new Dictionary<String, String>();
            try
            {
                // all for a given system under test in a given date range
                var query = Query.And(
                    Query.GT("RunDateTime", pStartDateTime),
                    Query.LT("RunDateTime", pEndDateTime),
                    Query.EQ("SystemUnderTestName", pSUTName)
                    );
                mongoDBConnectionPool = CreateApplicationDBConnectionPool();
                var db = mongoDBConnectionPool;
                var OperationalTestResults =
                    db.GetCollection(ot.OperationalTestResultsCollectionName)
                        .Find(query)
                        .SetFields(Fields.Include("ResultsStatement", "RunDateTime", "_id"));
                if (OperationalTestResults == null) return null;
                foreach (var OperationalTestResult in OperationalTestResults)
                {
                    var mIgnore = false;
                    var mRunTime = string.Empty;
                    var mRunId = string.Empty;
                    var mResultsStatement = string.Empty;
                    foreach (var field in OperationalTestResult)
                    {
                        switch (field.Name)
                        {
                            case "ResultsStatement":
                                mResultsStatement = field.Value.ToString();
                                if (!String.IsNullOrEmpty(pPassOrFail))
                                {
                                    if (pPassOrFail == "Failed")
                                    {
                                        if (mResultsStatement.Contains("Failed=0"))
                                            mIgnore = true;
                                    }
                                    else
                                    {
                                        if (mResultsStatement.Contains("Failed=0") == false)
                                            mIgnore = true;
                                    }
                                }
                                break;
                            case "RunDateTime":
                                var sdt = field.Value.ToString();
                                var dt = DateTime.Parse(sdt);
                                mRunTime = dt.ToString("MM/dd/yy HH:mm:ss");
                                break;
                            case "_id":
                                mRunId = field.Value.ToString();
                                break;
                        }
                    }
                    if (!mIgnore)
                    {
                        var mRun = "Run " + mRunTime + " - " + mResultsStatement;
                        // avoid dup keys
                        for (var x = 0;; ++x)
                        {
                            if (mDict.ContainsKey(mRun) == false) break;
                            if (x < 10)
                            {
                                mIgnore = true;
                                break;
                            }
                            mRun = string.Format("Run({0}){1} - {2}", x, mRunTime, mResultsStatement);

                        }
                        if (!mIgnore)
                            mDict.Add(mRun, mRunId);
                    }
                }
                return mDict;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public Dictionary<String, String> xGetFilesForTestRunUsingTestRunId(String pTestRunId)
        {
            var mResultFiles = new Dictionary<String, String>();

            mongoDBConnectionPool = CreateApplicationDBConnectionPool();
            var query = Query.EQ("_id", ObjectId.Parse(pTestRunId));
            MongoCollection mCollection = mongoDBConnectionPool.GetCollection(ot.OperationalTestResultsCollectionName);
            var mOperationalTestResults = mCollection.FindOneAs<OperationalTestResults>(query);
            if (mOperationalTestResults == null) return null;
            foreach (var mResultFile in mOperationalTestResults.ResultFileList)
            {
                var mIgnore = false;
                var mFileName = mResultFile.Name;
                // avoid dup keys
                for (var x = 0; ; ++x)
                {
                    if (mResultFiles.ContainsKey(mFileName) == false) break;
                    if (x < 10)
                    {
                        mIgnore = true;
                        break;
                    }
                    mFileName = string.Format("{0}({1})", mResultFile.Name, x);

                }
                if (!mIgnore)
                    mResultFiles.Add(mResultFile.Name, mResultFile.Type);
            }
            return mResultFiles;
        }
        public OperationalTestResults GetOperationalTestResultsById(String pTestRunId)
        {
            try
            {
                mongoDBConnectionPool = CreateApplicationDBConnectionPool();
                var query = Query.EQ("_id", ObjectId.Parse(pTestRunId));
                MongoCollection mCollection =
                    mongoDBConnectionPool.GetCollection(ot.OperationalTestResultsCollectionName);
                return mCollection.FindOneAs<OperationalTestResults>(query);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string GetResultsFileContent(String pTestRunId, string pFileName)
        {
            try
            {
                mongoDBConnectionPool = CreateApplicationDBConnectionPool();
                var query = Query.EQ("_id", ObjectId.Parse(pTestRunId));
                MongoCollection mCollection =
                    mongoDBConnectionPool.GetCollection(ot.OperationalTestResultsCollectionName);
                var mTestResults = mCollection.FindOneAs<OperationalTestResults>(query);
                foreach (OperationalTestResultFile mFile in mTestResults.ResultFileList)
                {
                    if (mFile.Name == pFileName)
                    {
                        return Security.DecodeAndDecrypt(mFile.eContent, cs.DefaultClientId);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Add & delete system under test
        public string AddSystemUnderTest(OperationalTestSystemUnderTest pSystemUnderTest)
        {
            var mOperationalTest = Read();
            foreach (var mSystemUnderTest in mOperationalTest.SystemsUnderTestList)
            {
                if (mSystemUnderTest.SystemName == pSystemUnderTest.SystemName)
                    return "Dup name.";
            }
            mOperationalTest.SystemsUnderTestList.Add(pSystemUnderTest);
            Update(mOperationalTest);
            return "System Under Test Added";
        }
        public string DeleteSystemUnderTestByName(String pSystemUnderTestName)
        {
            var mDeleted = false;
            var mOperationalTest = Read();
            var mNewList = new List<OperationalTestSystemUnderTest>();
            foreach (var mSUT in mOperationalTest.SystemsUnderTestList)
            {
                if (mSUT.SystemName != pSystemUnderTestName)
                    mNewList.Add(mSUT);
                else
                    mDeleted = true;
            }

            if (mDeleted)
            {
                mOperationalTest.SystemsUnderTestList.Clear();
                foreach (var mSUT in mNewList)
                {
                    mOperationalTest.SystemsUnderTestList.Add(mSUT);
                }
                Update(mOperationalTest);
            }
            return String.Empty;
        }

        // Add & delete contacts
        public string AddContactToSystemUnderTest(String pSystemUnderTestName, OperationalTestContact pContact)
        {
            var mOperationalTest = Read();
            foreach (var mSUT in mOperationalTest.SystemsUnderTestList)
            {
                if (mSUT.SystemName == pSystemUnderTestName)
                {
                    foreach (var mContact in mSUT.ContactList)
                    {
                        if ((mContact.eFirstName == pContact.eFirstName) && (mContact.eLastName == pContact.eLastName))
                        {
                            return "Dup contact";
                        }
                    }
                    mSUT.ContactList.Add(pContact);
                    Update(mOperationalTest);
                    return "Contact Added";
                }
            }
            return "Could not add";
        }
        public string UpdateContactForSystemUnderTest(String pSystemUnderTestName, OperationalTestContact pContact)
        {
            DeleteContactFromSystemUnderTest(pSystemUnderTestName, pContact.eFirstName, pContact.eLastName);
            AddContactToSystemUnderTest(pSystemUnderTestName, pContact);
            return "Could not update";
        }
        public string DeleteContactFromSystemUnderTest(String pSystemUnderTestName, String pContactName)
        {
            var mContactNames = pContactName.Split(' ');
            var mEFN = Security.EncryptAndEncode(mContactNames[0], cs.DefaultClientId);
            var mELN = Security.EncryptAndEncode(mContactNames[1], cs.DefaultClientId);
            return DeleteContactFromSystemUnderTest(pSystemUnderTestName, mEFN, mELN);
        }
        public string DeleteContactFromSystemUnderTest(String pSystemUnderTestName, String pEFirstName, String pELastName)
        {
            var mDeleted = false;
            var mOperationalTest = Read();
            var mNewList = new List<OperationalTestContact>();
            foreach (var mSUT in mOperationalTest.SystemsUnderTestList)
            {
                if (mSUT.SystemName == pSystemUnderTestName)
                {
                    foreach (var mContact in mSUT.ContactList)
                    {
                        if ((mContact.eFirstName == pEFirstName) && (mContact.eLastName == pELastName))
                            mDeleted = true;
                        else
                            mNewList.Add(mContact);
                    }
                    if (mDeleted)
                    {
                        mSUT.ContactList.Clear();
                        foreach (var mContact in mNewList)
                        {
                            mSUT.ContactList.Add(mContact);
                        }
                        Update(mOperationalTest);
                        return "Deleted";
                    }
                }
            }
            return "Could not delete!";
        }

        // Add & Delete Tests
        public string AddTestFromSystemUnderTest(string pSystemUnderTestName, OperationalTestConfig pTest)
        {
            var mOperationalTest = Read();
            foreach (var mSUT in mOperationalTest.SystemsUnderTestList)
            {
                if (mSUT.SystemName == pSystemUnderTestName)
                {
                    foreach (var mTest in mSUT.TestConfigList)
                    {
                        if (mTest.eTestName == pTest.eTestName)
                            return "Dup. test";
                    }
                    mSUT.TestConfigList.Add(pTest);
                    Update(mOperationalTest);
                    return "Test Added";
                }
            }
            return "Could not add";
        }
        public string DeleteTestFromSystemUnderTest(string pSystemUnderTestName, String pTestName)
        {
            var mDeleted = false;
            var mOperationalTest = Read();
            var mKeepList = new List<OperationalTestConfig>();
            foreach (var mSUT in mOperationalTest.SystemsUnderTestList)
            {
                if (mSUT.SystemName == pSystemUnderTestName)
                {
                    var en = Security.EncryptAndEncode(pTestName, cs.DefaultClientId);
                    foreach (var mTest in mSUT.TestConfigList)
                    {
                        if (mTest.eTestName == en)
                            mDeleted = true;
                        else // don't add to keep list
                            mKeepList.Add(mTest);
                    }
                    if (mDeleted)
                    {
                        // delete all current test
                        mSUT.TestConfigList.Clear();
                        // add keepers
                        foreach (var mTest in mKeepList)
                        {
                            mSUT.TestConfigList.Add(mTest);
                        }
                        Update(mOperationalTest);
                        return "Deleted";
                    }
                    return "Could not find!";
                }
            }
            return "Could not delete!";
        }

        // Delete All Methods
        public void DeleteAllSystemsUnderTest()
        {
            var mOperationalTest = Read();
            mOperationalTest.SystemsUnderTestList.Clear();
            Update(mOperationalTest);
        }
        public void DeleteAllContactFromSystemUnderTest(string pSystemUnderTestName)
        {
            var mOperationalTest = Read();
            foreach (var mSUT in mOperationalTest.SystemsUnderTestList)
            {
                if (mSUT.SystemName == pSystemUnderTestName)
                {
                    mSUT.ContactList.Clear();
                    Update(mOperationalTest);
                }
            }
        }
        public void DeleteAllTestFromSystemUnderTest(string pSystemUnderTestName)
        {
            var mOperationalTest = Read();

            foreach (var mSUT in mOperationalTest.SystemsUnderTestList)
            {
                if (mSUT.SystemName == pSystemUnderTestName)
                {
                    mSUT.TestConfigList.Clear();
                    Update(mOperationalTest);
                }
            }
        }
    }
}
