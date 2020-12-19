using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using STLib;

public class MongoDBData
{
	public MongoDBData()
	{
        mongoDB = ConfigDB("");
	}

    MongoClient mongoClient;
    MongoServer mongoServer;
    MongoDatabase mongoDB;
    MongoCollection stCollection;

    public MongoDatabase ConfigDB(string databaseName)
    {
        if (string.IsNullOrEmpty(databaseName))
            databaseName = ConfigurationManager.AppSettings["MongoDbName"];

        // Parse dbserver credentials
        string[] dbCredentials;
        // Parse dbserver settings
        string[] dbServerPort;
        var dbConnectionString = ConfigurationManager.ConnectionStrings["MongoServer"].ConnectionString;

        var connectionMode = ConfigurationManager.AppSettings["ConnectionMode"];
        var readPreference = ConfigurationManager.AppSettings["ReadPreference"];
        var writeConcern = ConfigurationManager.AppSettings["WriteConcern"];

        var mongoClientSettings = new MongoClientSettings();

        var dbConnectionTimeout = new TimeSpan(0, 0, 0, Convert.ToInt16(ConfigurationManager.AppSettings["ConnectionTimeoutSeconds"]), 0);
        mongoClientSettings.ConnectTimeout = dbConnectionTimeout;

        mongoClientSettings.MinConnectionPoolSize = Convert.ToInt16(ConfigurationManager.AppSettings["MinDBConnections"]);
        mongoClientSettings.MaxConnectionPoolSize = Convert.ToInt16(ConfigurationManager.AppSettings["MaxDBConnections"]);

        var dbConnectionSettings = dbConnectionString.Split('@');

        // Server credentials
        dbCredentials = dbConnectionSettings[0].Split(':');
        var dbUserName = dbCredentials[1].Replace("//", "");
        var dbPassword = dbCredentials[2];

        // Server settings
        dbServerPort = dbConnectionSettings[1].Split(':');
        var dbServer = dbServerPort[0];
        var dbPort = dbServerPort[1];

        //mongoClientSettings.Credentials = new[] { MongoCredential.CreateMongoCRCredential(ConfigurationManager.AppSettings["MongoDbName"], dbUserName, dbPassword) };
        mongoClientSettings.Credentials = new[] { MongoCredential.CreateMongoCRCredential(databaseName, dbUserName, dbPassword) };
        mongoClientSettings.Server = new MongoServerAddress(dbServer, Convert.ToInt16(dbPort));

        mongoClient = new MongoClient(mongoClientSettings);
        mongoServer = mongoClient.GetServer();

        //mongoDB = mongoServer.GetDatabase(ConfigurationManager.AppSettings["MongoDbName"]);
        mongoDB = mongoServer.GetDatabase(databaseName);

        return mongoDB;
    }

    public MongoCollection GetSTCollection()
    {
        // Initialize the collection
        stCollection = mongoDB.GetCollection("SecureTrading");

        return stCollection;
    }

    public List<StiOperatorSite> GetOperatorSites()
    {
        // Initialize the collection if not already done so
        if (stCollection == null)
            stCollection = this.GetSTCollection();

        var query = Query.EQ("_t", "StiOperatorSite");
        var sortBy = SortBy.Ascending("SiteId");

        var siteCollection = stCollection.FindAs<StiOperatorSite>(query).SetSortOrder(sortBy).ToList();

        return siteCollection;
    }

    public StiOperatorSite GetStiOperatorSiteUsingOperatorIdAndSiteId(string pOperatorId, String pSiteId)
    {
        if (String.IsNullOrEmpty(pOperatorId)) return null;
        if (String.IsNullOrEmpty(pSiteId)) return null;
        // Initialize the collection if not already done so
        if (stCollection == null)
            stCollection = this.GetSTCollection();
        var query = Query.And(Query.EQ("OperatorId", pOperatorId), Query.EQ("SiteId", pSiteId));
        var mStiOperatorSite = stCollection.FindOneAs<StiOperatorSite>(query);
        return mStiOperatorSite;
    }

    public StiPlayer GetPlayer(string playerId)
    {
        // Initialize the collection if not already done so
        if (stCollection == null) stCollection = this.GetSTCollection();
        var query = Query.EQ("_id", ObjectId.Parse(playerId));
        StiPlayer myPlayer = stCollection.FindOneAs<StiPlayer>(query);
        return myPlayer;
    }

    public TestCreditCard GetCreditCard(string cardNumber)
    {
        // Initialize the collection if not already done so
        //if (stCollection == null) stCollection = this.GetSTCollection();

        // Switch to central database
        mongoDB = ConfigDB("SecureTrading");

        var mongoCollection = mongoDB.GetCollection("SecureTrading");

        var query = Query.EQ("cardNumber", cardNumber);

        TestCreditCard myCreditCard = mongoCollection.FindOneAs<TestCreditCard>(query);

        // Switch back to user database
        mongoDB = ConfigDB("");

        return myCreditCard;
    }

    public StiPlayer GetPlayerByUsername(string userName)
    {
        // Initialize the collection if not already done so
        if (stCollection == null) stCollection = this.GetSTCollection();
        var query = Query.EQ("userName", userName);
        StiPlayer myPlayer = stCollection.FindOneAs<StiPlayer>(query);
        return myPlayer;
    }

    public StiPlayer GetPlayerByFirstNameLastName(string pFirstName, string pLastName)
    {
        // Initialize the collection if not already done so
        if (stCollection == null) stCollection = this.GetSTCollection();
        var query = Query.And(Query.EQ("firstName", pFirstName), Query.EQ("lastName", pLastName));
        try
        {
            var myPlayer = stCollection.FindOneAs<StiPlayer>(query);
            return myPlayer;  
        }
        catch (Exception)
        {
            return null;
        }
    }

    public List<StiPlayer> GetAllPlayers()
    {
        // Initialize the collection if not already done so
        if (stCollection == null)
            stCollection = this.GetSTCollection();

        var query = Query.EQ("_t", "StiPlayer");

        var playerCollection = stCollection.FindAs<StiPlayer>(query).ToList();

        return playerCollection;
    }

    public List<TestCreditCard> GetAvailableCreditCards()
    {
        // Switch to central database
        mongoDB = ConfigDB("SecureTrading");

        var mongoCollection = mongoDB.GetCollection("SecureTrading");

        var query = Query.And(Query.EQ("_t", "TestCreditCard"), Query.EQ("macPlayerId", ObjectId.Parse("000000000000000000000000")));
        var sortBy = SortBy.Descending("cardNumber");

        var testCreditCardCollection = mongoCollection.FindAs<TestCreditCard>(query).SetSortOrder(sortBy).ToList();

        // Switch back to user database
        mongoDB = ConfigDB("");

        return testCreditCardCollection;
    }

    public List<TestCreditCard> GetAllRegisteredCreditCards(string playerId)
    {
        // Switch to central database
        mongoDB = ConfigDB("SecureTrading");

        var mongoCollection = mongoDB.GetCollection("SecureTrading");

        if (string.IsNullOrEmpty(playerId))
        {
            playerId = ObjectId.Empty.ToString();

            var query = Query.And(Query.EQ("_t", "TestCreditCard"), Query.NE("macPlayerId", ObjectId.Parse(playerId)));
            var sortBy = SortBy.Descending("cardNumber");

            var testCreditCardCollection = mongoCollection.FindAs<TestCreditCard>(query).SetSortOrder(sortBy).ToList();

            // Switch back to user database
            mongoDB = ConfigDB("");

            return testCreditCardCollection;
        }
        else
        {
            var query = Query.And(Query.EQ("_t", "TestCreditCard"), Query.EQ("macPlayerId", ObjectId.Parse(playerId)));
            var sortBy = SortBy.Descending("cardNumber");

            var testCreditCardCollection = mongoCollection.FindAs<TestCreditCard>(query).SetSortOrder(sortBy).ToList();

            // Switch back to user database
            mongoDB = ConfigDB("");

            return testCreditCardCollection;
        }
    }

    // Overloaded save method
    public void Save(StiOperatorSite myOperatorSite)
    {
        // Initialize the collection if not already done so
        if (stCollection == null)
            stCollection = this.GetSTCollection();

        stCollection.Save(myOperatorSite);

        // Switch to central database for storage
        mongoDB = ConfigDB("SecureTrading");

        var mongoCollection = mongoDB.GetCollection("SecureTrading");
        mongoCollection.Save(myOperatorSite);

        // Switch back to user database
        mongoDB = ConfigDB("");
    }

    public void Save(TestCreditCard myCreditCard)
    {
        // Switch to central database
        mongoDB = ConfigDB("SecureTrading");

        if (stCollection == null) stCollection = this.GetSTCollection();
        var mongoCollection = mongoDB.GetCollection("SecureTrading");
        mongoCollection.Save(myCreditCard);

        // Switch back to user database
        mongoDB = ConfigDB("");
    }

    public bool UpdateOperatorSite(StiOperatorSite pOperatorSite)
    {
        try
        {
            // Initialize the collection if not already done so
            if (stCollection == null) stCollection = this.GetSTCollection();
            var mongoCollection = mongoDB.GetCollection("SecureTrading");
            mongoCollection.Save(pOperatorSite);

            // Switch to central database for storage
            mongoDB = ConfigDB("SecureTrading");

            mongoCollection = mongoDB.GetCollection("SecureTrading");
            mongoCollection.Save(pOperatorSite);

            // Switch back to user database
            mongoDB = ConfigDB("");

        }
        catch (Exception ex)
        {
            return false;
        }
        return true;
    }
    // Overloaded save method
    public void Save(StiPlayer myPlayer)
    {
        // Initialize the collection if not already done so
        //if (stCollection == null)
            stCollection = this.GetSTCollection();

        stCollection.Save(myPlayer);
    }

    // Overloaded save method
    public void Save(StiWrapperState myState)
    {
        // Initialize the collection if not already done so
        if (stCollection == null)
            stCollection = this.GetSTCollection();

        stCollection.Save(myState);

        // Switch to central database for storage
        mongoDB = ConfigDB("SecureTrading");

        var mongoCollection = mongoDB.GetCollection("SecureTrading");
        mongoCollection.Save(myState);

        // Switch back to user database
        mongoDB = ConfigDB("");
    }

    public StiWrapperState Read(string stateId)
    {
        // Initialize the collection if not already done so
        if (stCollection == null)
            stCollection = this.GetSTCollection();

        StiWrapperState myState;

        var query = Query.EQ("_id", ObjectId.Parse(stateId.Trim()));

        myState = stCollection.FindOneAs<StiWrapperState>(query);

        return myState;
    }

    public void Delete(StiWrapperState myState)
    {
        // Initialize the collection if not already done so
        if (stCollection == null)
            stCollection = this.GetSTCollection();

        var query = Query.EQ("_id", myState._id);
        var sortBy = SortBy.Descending("_id");
        stCollection.FindAndRemove(query, sortBy);
    }

    public string DeleteStateObjects()
    {
        var deleteResult = "Successfuly deleted all [0] WrapperState objects in database";

        var itemCount = 0;

        try
        {
            // Initialize the collection if not already done so
            if (stCollection == null)
                stCollection = this.GetSTCollection();

            var query = Query.EQ("_t", "WrapperState");
            var sortBy = SortBy.Descending("_id");

            var stateCollection = stCollection.FindAs<StiWrapperState>(query).ToList();
            foreach (var currentState in stateCollection)
            {
                query = Query.EQ("_id", currentState._id);
                stCollection.FindAndRemove(query, sortBy);
                itemCount++;
            }

            deleteResult = deleteResult.Replace("[0]", "[" + itemCount.ToString() + "]");
        }
        catch(Exception ex)
        {
            deleteResult = "FAILED to delete all WrapperState objects in database.";
        }

        return deleteResult;
    }

}