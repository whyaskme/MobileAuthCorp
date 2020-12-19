using System;
using System.Windows.Forms;
using MongoDB.Bson;

public class StiPlayer
{
	public StiPlayer()
	{
        _id = ObjectId.GenerateNewId();
        _t = "StiPlayer";

        // Secure Trading Info
        stiPlayerId = "000000000000000000000";
        stiReferenceNumber = "Not Registered";

        // Personal Info
        firstName = "";
        middleInitial = "";
        lastName = "";

        // Address Info
        playerAddress1 = "";
        playerAddress2 = "";
        city = "";
        county = "";
        state = "";
        zipCode = "";
        country = "";

        // Contact Info
        emailAddress = "";
        mobileNo = "";
        landLineNo = "";

        // Demographic Info
        gender = "";
        dob = "";

        // Security Info
        userName = "";
        ipAddress = "";

        // Identification Info
        ssn = "";
        dlNumber = "";
        dlIssuingState = "";

        // st connection and session
        connectionToken = "";
        sessionToken = "";

        currency = "USD";
        threeDFlag = "FALSE";

        // Player Card Info
        PCIAccountType = "";
        PCINameOnCard = "";
        PCICardType = "";
        PCICardNumber = "";
        PCICVV = "123";
        PCIStartDate = "01/2015";
        PCIExpiryDate = "12/2016";
        PCIIssueNumber = "";
        PCIDefaultCard = "";
        PCIAddress1 = "";
        PCIAddress2 = "";
        PCICity = "";
        PCICounty = "";
        PCIState = "";
        PCIZipCode = "";
        PCICountry = "";
	    PCICardToken = "";

        // banking info
	    BAInstitutionName = "";
	    BAAccountType = "";
	    BAToken = "";
	    BAABANumber = "";

        //PrePaid Account Info
        PPANumber = "51############01";
        PPAVirtualCardNumber = "0000000000000000";
        PPAToken = "ABCDEFGHIJKLMNO=";
	    PPABalance = "100000";
        PPATandCAccepted = "Y";
        PPATandCAcceptedTimestampUTC = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
	}

    public ObjectId _id { get; set; }
    public string _t { get; set; }

    // Secure Trading Info
    public string stiPlayerId { get; set; }
    public string stiReferenceNumber { get; set; }

    // Personal Info
    public string firstName { get; set; }
    public string middleInitial { get; set; }
    public string lastName { get; set; }

    // Address Info
    public string playerAddress1 { get; set; }
    public string playerAddress2 { get; set; }
    public string city { get; set; }
    public string county { get; set; }
    public string state { get; set; }
    public string zipCode { get; set; }
    public string country { get; set; }

    // Contact Info
    public string emailAddress { get; set; }
    public string mobileNo { get; set; }
    public string landLineNo { get; set; }

    // Demographic Info
    public string gender { get; set; }
    public string dob { get; set; }

    // Security Info
    public string userName { get; set; }
    public string ipAddress { get; set; }

    // Identification Info
    public string ssn { get; set; }
    public string dlNumber { get; set; }
    public string dlIssuingState { get; set; }

    // Connection and Session tokens
    public string connectionToken { get; set; }
    public string sessionToken { get; set; }
    public string threeDFlag { get; set; }

    public string currency { get; set; }

    // Card information
    public string PCIAccountType { get; set; }
    public string PCINameOnCard { get; set; }
    public string PCICardType { get; set; }
    public string PCICardNumber { get; set; }
    public string PCICVV { get; set; }
    public string PCIStartDate { get; set; }
    public string PCIExpiryDate { get; set; }
    public string PCIIssueNumber { get; set; }
    public string PCIDefaultCard { get; set; }
    public string PCIAddress1 { get; set; }
    public string PCIAddress2 { get; set; }
    public string PCICity { get; set; }
    public string PCICounty { get; set; }
    public string PCIState { get; set; }
    public string PCIZipCode { get; set; }
    public string PCICountry { get; set; }
    public string PCICardToken { get; set; }

    // banking info
    public string BAInstitutionName { get; set; }
    public string BAAccountType { get; set; }
    public string BAToken { get; set; }
    public string BAABANumber { get; set; }
    
    // prepaid account info
    public string PPAVirtualCardNumber { get; set; }
    public string PPATandCAccepted { get; set; }
    public string PPATandCAcceptedTimestampUTC { get; set; }
    public string PPANumber { get; set; }
    public string PPAToken { get; set; }
    public string PPABalance { get; set; }
}