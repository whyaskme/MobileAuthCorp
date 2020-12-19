using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;

using Newtonsoft.Json.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using STLib;
using Cst = STLib.STConstants;

public partial class Default : System.Web.UI.Page
{

    MongoDBData myMongoUtil = new MongoDBData();
    MongoDatabase mongoDB;

    string returnCodeColor = "#ff0000";

    protected void Page_Load(object sender, EventArgs e)
    {
        SetServerInfoHeader();

        if (IsPostBack) { return; }

        divDeleteResult.InnerHtml = "";

        divJsonResponseData.InnerHtml = "";
        divPlayerOperationsContainer.Visible = false;

        divActionButtons.Visible = false;
        divPlayerContainer.Visible = false;
        //divPlayerCardInfo.Visible = false;
        //divPlayerPrePaidAccountInfo.Visible = false;

        divQuestionsContainer.Visible = false;

        divStatus.InnerHtml = "";

        rbst.Checked = true;
        cbLogToFile.Checked = true;
        dlPlayer.Visible = false;

        hiddenLoopbackEnabled.Value = "0";
        CheckExistingSitesAndCreateMissingSites();
        CreateSitesList();
        CreatePlayersList();

        // Only used for seeding cards in central database
        //CreateTestCards();

        //GetAvailableCreditCards();
        //GetAllRegisteredCreditCards();

        //WrapperService("Ping", "PingData");
    }

    private void ShowAll()
    {
        //divPlayerContainer.Visible = true;
        //divPlayerCardInfo.Visible = true;
        //divPlayerPrePaidAccountInfo.Visible = true;
        //divOtpValidate.Visible = true;
        //divPlayerOperationsContainer.Visible = true;

    }

    //private void GetAvailableCreditCards()
    //{
    //    dlAvailableCards.Items.Clear();

    //    // Create root list item
    //    var defaultItem = new ListItem();
    //    defaultItem.Text = "Select an Available Card";
    //    defaultItem.Value = defaultItem.Text;

    //    dlAvailableCards.Items.Add(defaultItem);
    //    var myDatabase = new MongoDBData();
    //    var cardCollection = myDatabase.GetAvailableCreditCards();
    //    if (cardCollection.Any())
    //    {
    //        foreach (TestCreditCard currentCard in cardCollection)
    //        {
    //            var cardItem = new ListItem();
    //            cardItem.Text = currentCard.cardNumber + " - " + currentCard.cardType;
    //            cardItem.Value = currentCard.cardNumber;

    //            dlAvailableCards.Items.Add(cardItem);
    //        }
    //    }
    //}

    //private void GetAllRegisteredCreditCards()
    //{
    //    dlAssignedCards.Items.Clear();

    //    // Create root list item
    //    var defaultItem = new ListItem();
    //    defaultItem.Text = "Select a Registered Card";
    //    defaultItem.Value = defaultItem.Text;

    //    dlAssignedCards.Items.Add(defaultItem);
    //    var myDatabase = new MongoDBData();

    //    List<TestCreditCard> cardCollection = new List<TestCreditCard>();

    //    if(dlPlayer.SelectedIndex == 0)
    //        cardCollection = myDatabase.GetAllRegisteredCreditCards("");
    //    else
    //        cardCollection = myDatabase.GetAllRegisteredCreditCards(dlPlayer.SelectedValue);

    //    if (cardCollection.Any())
    //    {
    //        foreach (TestCreditCard currentCard in cardCollection)
    //        {
    //            var cardItem = new ListItem();
    //            cardItem.Text = currentCard.cardNumber + " - " + currentCard.cardType + " (" + currentCard.stiPlayer.PCINameOnCard + ")";
    //            cardItem.Value = currentCard.cardNumber;

    //            dlAssignedCards.Items.Add(cardItem);
    //        }
    //    }
    //}

    private void SetServerInfoHeader()
    {
        var dbConnectionString = ConfigurationManager.ConnectionStrings["MongoServer"].ConnectionString.Split('@');

        var databaseName = ConfigurationManager.AppSettings["MongoDbName"];

        var serverInfo = "";
        serverInfo += "<i>Database Info:</i> ";
        serverInfo += "<b>" + databaseName + "</b> @ ";
        serverInfo += "<b>" + dbConnectionString[1] + "</b>";

        divServerInfo.InnerHtml = serverInfo;
    }

    #region ST Direct Flows

    #region ST Direct 1 stapiRegisterPlayer (done)
    private void ST_stapiRegisterPlayerFlow()
    {
        var stiOperations = new StiOperations();
        var mRegisterPlayer_RequestJson = Construct_RegisterPlayer_RequestJson();
        AddToLogAndDisplay(hiddenFlow.Value, "stapiRegisterPlayer Request", mRegisterPlayer_RequestJson);

        var mResponseData = stiOperations.stapiRegisterPlayer(mRegisterPlayer_RequestJson);
        
        AddToLogAndDisplay(hiddenFlow.Value, "stapiRegisterPlayer Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);

        string returnMsg = mResponseJson.returnMsg.ToString();
        string returnCode = mResponseJson.returnCode.ToString();
        switch (returnCode)
        {
            case "ERR_PFO_00291": // Username already exists for the given site.
                returnCodeColor = "#ff0000";
                break;

            case "ERR_PFO_00068": // What is this error?
            case "ERR_PFO_00069": // KYC Check Failure - No questions returned.
                returnCodeColor = "#ff0000";
                break;

            case "INFO_PFO_00021": // Please answer personal verification questions.
                returnCodeColor = "#068415";
                //var rt = mResponseJson.responseToken.ToString();
                ParseJsonQuestionToForm(mResponseData);
                break;

            case "INFO_PFO_00023":
                returnCodeColor = "#068415";
                // We have a valid registration response from ST
                if (mResponseJson.playerDetails != null)
                {
                    var myDatabase = new MongoDBData();
                    var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
                    txtPlayerPlayerId.Value = myPlayer.stiPlayerId = mResponseJson.playerDetails.stPlayerId;
                    txtPlayerReferenceNumber.Value = myPlayer.stiReferenceNumber = mResponseJson.playerDetails.stReferenceNo;
                    myDatabase.Save(myPlayer);
                }
                divPlayerContainer.Visible = false;
                divPlayerOperationsContainer.Visible = true;
                break;

            default:
                returnCodeColor = "#ff0000";
                //divOperatorContainer.Visible = true;
                divPlayerContainer.Visible = true;
                divPlayerOperationsContainer.Visible = false;
                break;
        }
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region ST Direct 2 stapiPlayerLogin (done)
    private void ST_stapiPlayerLoginFlow()
    {
        try
        {
            var mRequestData = Construct_OperatorInfo_RequestJson();
            AddToLogAndDisplay(hiddenFlow.Value, "stapiPreCheckSiteValidatePlayerRequest Request", mRequestData);
            var stiOperations = new StiOperations();
            var mResponseData = stiOperations.stapiPreCheckSiteValidatePlayerRequest(mRequestData);

            AddToLogAndDisplay(hiddenFlow.Value, "stapiPreCheckSiteValidatePlayerRequest Response", mResponseData);
            dynamic mResponseJson = JObject.Parse(mResponseData);
            string returnCode = mResponseJson.returnCode.ToString();
            string returnMsg = mResponseJson.returnMsg.ToString();

            var myDatabase = new MongoDBData();
            var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
            switch (returnCode)
            {
                case "INFO_PFO_00040":
                //Withdrawal Only - The player is on the self exclusion list and is only allowed to withdrawal.
                //todo: set player status to Withdrawal Only
                case "INFO_PFO_00041":
                //Not legal for wagering – The players location is outside the permitted state borders and therefore can only perform a deposit,card and account management.
                //todo: set player status to Not legal for wagering
                case "INFO_PFO_00002": //Player successfully validated.    
                    var diff = false;
                    if (myPlayer.stiPlayerId != mResponseJson.validationDetails.stPlayerId.ToString())
                    {
                        diff = true;
                        myPlayer.stiPlayerId = mResponseJson.validationDetails.stPlayerId.ToString();
                    }
                    txtPlayerPlayerId.Value = myPlayer.stiPlayerId;

                    if (myPlayer.stiReferenceNumber != mResponseJson.validationDetails.stReferenceNo.ToString())
                    {
                        diff = true;
                        myPlayer.stiReferenceNumber = mResponseJson.validationDetails.stReferenceNo.ToString();
                    }
                    txtPlayerReferenceNumber.Value = myPlayer.stiReferenceNumber;

                    if (myPlayer.connectionToken != mResponseJson.validationDetails.connectionToken.ToString())
                    {
                        diff = true;
                        myPlayer.connectionToken = mResponseJson.validationDetails.connectionToken.ToString();
                    }
                    txtPlayerConnectionToken.Value = myPlayer.connectionToken;

                    if (myPlayer.sessionToken != mResponseJson.validationDetails.stReferenceNo.ToString())
                    {
                        diff = true;
                        myPlayer.sessionToken = mResponseJson.validationDetails.stReferenceNo.ToString();
                    }
                    txtPlayerSessionToken.Value = myPlayer.sessionToken;
                    if (diff) myDatabase.Save(myPlayer);
                    return;

                case "INFO_PFO_00001":
                //Player was previously Self Excluded, please display the screen with the option to continue Self Exclusion.
                case "ERR_PFO_00008": //Site authentication failure.
                case "ERR_PFO_00178": //Player is self-excluded from the system until {DATE}.
                    txtPlayerPlayerId.Value = myPlayer.stiPlayerId = string.Empty;
                    txtPlayerReferenceNumber.Value = myPlayer.stiReferenceNumber = string.Empty;
                    myPlayer.connectionToken = mResponseJson.validationDetails.connectionToken.ToString();
                    myPlayer.sessionToken = mResponseJson.validationDetails.stReferenceNo.ToString();
                    myDatabase.Save(myPlayer);
                    break;
            }
            ShowCodeAndMsg(returnCode, returnMsg);
        }
        catch (Exception ex)
        {
            showError("Exception: " + hiddenService.Value + "." + dlWorkflows.SelectedValue + ", " + ex.Message);
        }
    }
    #endregion

    #region ST Direct 3 stapiPreCheckSiteValidatePlayerRequest (done)
    private void ST_stapiPreCheckSiteValidatePlayerRequestFlow()
    {
        // Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        ShowCodeAndMsg(rtn.Item2, rtn.Item3);
    }
    #endregion

    #region ST Direct 4 stapiPreCheckSiteCardRequest (done)
    private void ST_stapiPreCheckSiteCardRequestFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

        //Step 2 Register the Card
        var mRequestData = Construct_PreCheckSiteCardRequest_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteCardRequest Request", mRequestData);

        var mResponseData = stiOperations.stapiPreCheckSiteCardRequest(mRequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteCardRequest Response", mResponseData);
        dynamic mPreCheckSiteCardRequest_ResponseData = JObject.Parse(mResponseData);
        string mReturnCode = mPreCheckSiteCardRequest_ResponseData.returnCode.ToString();
        string mReturnMsg = mPreCheckSiteCardRequest_ResponseData.returnMsg.ToString();
        if (mReturnCode == "ERR_PFO_00016")
        {
            // Successfully Registered card, save card token
            txtPCICardToken.Value = myPlayer.PCICardToken =
                mPreCheckSiteCardRequest_ResponseData.cardRegistrationDetails.cardDetails.cardToken.ToString();
        }
        else
        {
            txtPCICardToken.Value = myPlayer.PCICardToken = string.Empty;
        }
        myDatabase.Save(myPlayer);
        ShowAll();
        ShowCodeAndMsg(mReturnCode, mReturnMsg);
    }
    #endregion

    #region ST Direct 5 Delete Registered Card (done)
    private void ST_DeleteRegisteredCardFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }

        if (String.IsNullOrEmpty(txtPCIDefaultCard.Value))
        {
            showError("DeleteRegisteredCardFlow: txtPCIDefaultCard.Value not set!");
            return;
        }
        //Step 2 Delete Registered Card using 
        var mRequestData = Construct_PreCheckSiteModifyCardRequest_Json(txtPCIDefaultCard.Value, "Y");
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteModifyCardRequest Request", mRequestData);
        var stiOperations = new StiOperations();
        var mResponseData = stiOperations.stapiPreCheckSiteModifyCardRequest(mRequestData);
        
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteModifyCardRequest Response", mResponseData);
        dynamic ResponseJson = JObject.Parse(mResponseData);
        string returnCode = ResponseJson.returnCode.ToString();
        string returnMsg = ResponseJson.returnMsg.ToString();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        if (returnCode == "INFO_PFO_00038")
        {
            // card successfull deleted
            returnCodeColor = "#068415";
            returnMsg += ", " + ResponseJson.recordStatus.ToString();
            txtPCICardToken.Value = myPlayer.PCICardToken = ResponseJson.cardToken.ToString();
        }
        else
        {
            returnCodeColor = "#ff0000";
            txtPCICardToken.Value = myPlayer.PCICardToken = string.Empty;
        }
        myDatabase.Save(myPlayer);
        ShowAll();
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region ST Direct 6 stapiRegisterPrePaidAccount (done)
    private void ST_stapiRegisterPrePaidAccountFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

        //Step 2 Register Pre Paid Account
        var mRequestData = Construct_RegisterPrePaidAccount_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiRegisterPrePaidAccount Request", mRequestData);
        
        var mResponseData = stiOperations.stapiRegisterPrePaidAccount(mRequestData);
        
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiRegisterPrePaidAccount Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        
        
        //prepaidaccounttoken

        // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 7 stapiLoadPrePaidFunds (not tested)
    private void ST_stapiLoadPrePaidFundsFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

        //Step 2 Load PrePaid Funds Flow
        var mRequestData = Construct_RegisterPrePaidAccount_RequestJson(myPlayer);
        //--- call ST-1 direct
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiLoadPrePaidFundsFlow Request", mRequestData);

        var mResponseData = stiOperations.stapiLoadPrePaidFunds(mRequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiLoadPrePaidFundsFlow Response", mResponseData);
        dynamic mLoadPrePaidFunds_ResponseData = JObject.Parse(mResponseData);
        string returnCode = mLoadPrePaidFunds_ResponseData.returnCode.ToString();
        string returnMsg = mLoadPrePaidFunds_ResponseData.returnMsg.ToString();

        // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 8 stapiSubmitPrePaidDeposit Flow (not tested)
    private void ST_stapiSubmitPrePaidDepositFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        var stiOperations = new StiOperations();
        //var myDatabase = new MongoDBData();
        //var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

        //Step 2 Submit Pre Paid Deposit
        var SubmitPrePaidDeposit_RequestData = Construct_SubmitPrePaidDeposit_RequestJson();
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitPrePaidDeposit Request", SubmitPrePaidDeposit_RequestData);

        var mResponseData = stiOperations.stapiSubmitPrePaidDeposit(SubmitPrePaidDeposit_RequestData);
        
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitPrePaidDeposit Response", mResponseData);
        dynamic SubmitPrePaidDeposit_ResponseData = JObject.Parse(mResponseData);
        string mReturnCode = SubmitPrePaidDeposit_ResponseData.returnCode.ToString();
        string mReturnMsg = SubmitPrePaidDeposit_ResponseData.returnMsg.ToString();

        ShowCodeAndMsg(mReturnCode, mReturnMsg);
    }
    #endregion

    #region ST Direct 9 stapiSubmitDepositAccountRequest (not tested)
    private void ST_stapiSubmitDepositAccountRequestFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

        //Step 2 Pre Paid Withdrawl Request
        var mRequestData = Construct_SubmitDepositAccountRequest_RequestJson(myPlayer);
        //--- call ST-1 direct
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitDepositAccountRequest Request", mRequestData);
        var mResponseData = stiOperations.stapiSubmitDepositAccountRequest(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitDepositAccountRequest Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 10 stapiSubmitPrePaidWithdrawal (not tested)
    private void ST_stapiSubmitPrePaidWithdrawalFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

        //Step 2 Pre Paid Withdrawl Request
        var mRequestData = Construct_SubmitPrePaidWithdrawal_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitPrePaidWithdrawal Request", mRequestData);
        
        var mResponseData = stiOperations.stapiSubmitPrePaidWithdrawal(mRequestData);
        
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitPrePaidWithdrawal Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 11 stapiGetTransactionDetails (not tested)
    private void ST_stapiGetTransactionDetailsFlow()
    {
        var stiOperations = new StiOperations();
        var mRequestData = Construct_GetTransactionDetails_RequestJson();
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetTransactionDetails Request", mRequestData);
        var mResponseData = stiOperations.stapiGetTransactionDetails(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetTransactionDetails Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();

    }
    #endregion
    
    #region ST Direct 12 stapiUpdateTransaction 282 (not tested)
    private void ST_stapiUpdateTransactionFlow()
    {
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_UpdateTransaction_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetTransactionDetails Request", mRequestData);
        var mResponseData = stiOperations.stapiUpdateTransaction(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetTransactionDetails Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 13 stapiGetRegisteredAccounts (not tested)
    private void ST_stapiGetRegisteredAccountsFlow()
    {
        var stiOperations = new StiOperations();
        var mRequestData = Construct_GetRegisteredAccounts_RequestJson();
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetRegisteredAccounts Request", mRequestData);
        var mResponseData = stiOperations.stapiGetRegisteredAccounts(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetRegisteredAccounts Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 14 stapiGetRegisteredCards (done)
    private void ST_stapiGetRegisteredCardsFlow()
    {
        var stiOperations = new StiOperations();
        var mRequestData = Construct_GetRegisteredCards_RequestJson();
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetRegisteredCards_RequestData Request", mRequestData);

        var mResponseData = stiOperations.stapiGetRegisteredCards(MapPathSecure(mRequestData));

        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetRegisteredCards_RequestData Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 15 stapiGetPlayerSSN (done)
    private void ST_stapiGetPlayerSSNFlow()
    {
        var stiOperations = new StiOperations();
        var mRequestData = Construct_GetPlayerSSN_RequestJson();
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetPlayerSSN Request", mRequestData);
        
        var mResponseData = stiOperations.stapiGetPlayerSSN(mRequestData);
        
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetPlayerSSN Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 16 stapiUpdatePlayerSSN (not tested)
    private void ST_stapiUpdatePlayerSSNFlow()
    {
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_UpdatePlayerSSN_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiUpdatePlayerSSN Request", mRequestData);
        var mResponseData = stiOperations.stapiUpdateTransaction(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiUpdatePlayerSSN Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 17 stapiGetPrePaidAccountHolderInfo (not tested)
    private void ST_stapiGetPrePaidAccountHolderInfoFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Get PrePaid Account Holder Info
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestJson = Construct_GetPrePaidAccountHolderInfo_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiGetPrePaidAccountHolderInfo Request", mRequestJson);

        var mResponseData = stiOperations.stapiGetPrePaidAccountHolderInfo(mRequestJson);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiGetPrePaidAccountHolderInfo Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 18 stapiGetPrePaidAccountBalance  (not tested)
    private void ST_stapiGetPrePaidAccountBalanceFlow()
    {
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_GetPrePaidAccountBalance_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetPrePaidAccountBalance Request", mRequestData);
        
        var mResponseData = stiOperations.stapiGetPrePaidAccountBalance(mRequestData);
        
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetPrePaidAccountBalance Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 19 stapiGetPrePaidAccountCVV2  (not tested)
    private void ST_stapiGetPrePaidAccountCVV2Flow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Get PrePaid Account CVV2
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_GetPrePaidAccountCVV2_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiGetPrePaidAccountCVV2 Request", mRequestData);
        var mResponseData = stiOperations.stapiUpdateTransaction(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiGetPrePaidAccountCVV2 Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 20 stapiPreCheckSiteAccountRequest (Not tested ??check service)
    private void ST_stapiPreCheckSiteAccountRequestFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Pre Check Site Account Request
        var stiOperations = new StiOperations();
        var mRequestData = Construct_PreCheckSiteAccountRequest_RequestJson();
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteAccountRequest Request", mRequestData);
        var mResponseData = stiOperations.stapiPreCheckSiteAccountRequest(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteAccountRequest Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00033" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 21 stapiPrePaidRegisterAndLoad (not tested)
    private void ST_stapiPrePaidRegisterAndLoadFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 PrePaid Register And Load
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_PrePaidRegisterAndLoad_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPrePaidRegisterAndLoad Request", mRequestData);
        var mResponseData = stiOperations.stapiPrePaidRegisterAndLoad(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPrePaidRegisterAndLoad Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 22 stapiUpdatePrePaidAccountStatus (Not tested)
    private void ST_stapiUpdatePrePaidAccountStatusFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        // Step 2 Update PrePaid AccountStatus
        var mRequestData = Construct_UpdatePrePaidAccountStatus_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiUpdatePrePaidAccountStatus Request", mRequestData);
        var mResponseData = stiOperations.stapiUpdatePrePaidAccountStatus(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiUpdatePrePaidAccountStatus Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 23 stapiSubmitDepositRequest (not tested)
    private void ST_stapiSubmitDepositRequestFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Submit Deposit Request
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_stapiSubmitDepositRequest_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitDepositRequest Request", mRequestData);
        var mResponseData = stiOperations.stapiSubmitDepositRequest(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitDepositRequest Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 24 stapiAccountWithdrawal  (not tested)
    private void ST_stapiAccountWithdrawalFlow()
    {
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_AccountWithdrawal_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiAccountWithdrawal Request", mRequestData);
        var mResponseData = stiOperations.stapiAccountWithdrawal(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiAccountWithdrawal Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


       // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 25 stapiSelfExcludePlayer  (not tested)
    private void ST_stapiSelfExcludePlayerFlow()
    {
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_SelfExcludePlayer_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiSelfExcludePlayer Request", mRequestData);
        var mResponseData = stiOperations.stapiSelfExcludePlayer(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiSelfExcludePlayer Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 26 stapiModifyPlayerRequest  (not tested)
    private void ST_stapiModifyPlayerRequestFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Modify Player Request
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_ModifyPlayerRequest_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiModifyPlayerRequest Request", mRequestData);
        var mResponseData = stiOperations.stapiModifyPlayerRequest(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiModifyPlayerRequest Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00021" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 27 stapiAuthDepositRequest  (not tested)
    private void ST_stapiAuthDepositRequestFlow()
    {
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_AuthDepositRequest_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiAuthDepositRequest Request", mRequestData);
        var mResponseData = stiOperations.stapiAuthDepositRequest(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiAuthDepositRequest Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();

        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 28 stapiSubmitRefund  (not tested)
    private void ST_stapiSubmitRefundFlow()
    {
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_SubmitRefund_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiSubmitRefund Request", mRequestData);
        var mResponseData = stiOperations.stapiSubmitRefund(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiSubmitRefund Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 29 stapiAddSelfExcludePlayer  (not tested)
    private void ST_stapiAddSelfExcludePlayerFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Add Self Exclude Player
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_AddSelfExcludePlayer_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiAddSelfExcludePlayer Request", mRequestData);
        var mResponseData = stiOperations.stapiAddSelfExcludePlayer(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiAddSelfExcludePlayer Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00004" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region ST Direct 30 stapiWithdrawal  (not tested)
    private void ST_stapiWithdrawalFlow()
    {
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_Withdrawal_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiWithdrawal Request", mRequestData);
        var mResponseData = stiOperations.stapiWithdrawal(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiWithdrawal Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion   
      
    #region ST Direct 31 stapiSubmitReversal  (not tested)
    private void ST_stapiSubmitReversalFlow()
    {
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_SubmitReversal_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiSubmitReversal Request", mRequestData);
        var mResponseData = stiOperations.stapiSubmitReversal(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiSubmitReversal Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        // returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion   

    #region ST Direct 32 stapiPreCheckSiteModifyAccountRequest Passthru (not tested)
    private void ST_stapiPreCheckSiteModifyAccountRequestFlow()
    {
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_PreCheckSiteModifyAccountRequest_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteModifyAccountRequest Request", mRequestData);
        var mResponseData = stiOperations.stapiPreCheckSiteModifyAccountRequest(mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteModifyAccountRequest Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        myDatabase.Save(myPlayer);
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region ST Direct - Combined Player Registration & Pre Paid Deposit Flow (not done)
    private void ST_CombinedPlayerRegistrationPrePaidDepositFlow()
    {
        var stiOperations = new StiOperations();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

        // Step 1 Validate the Player, if valid update player info. else quit
        var Step1 = ValidatePlayerAndUpdateInfo("1");

        if (Step1.Item1 == false)
        {   //------------- player is not registeres,
            // step A1 -----------  register player
            var mRequestJson = Construct_RegisterPlayer_RequestJson();
            AddToLogAndDisplay(hiddenFlow.Value, "stapiRegisterPlayer Request", mRequestJson);

            var mResponseData = stiOperations.stapiRegisterPlayer(mRequestJson);

            AddToLogAndDisplay(hiddenFlow.Value, "stapiRegisterPlayer Response", mResponseData);
            dynamic mResponseJson = JObject.Parse(mResponseData);
            string returnMsg = mResponseJson.returnCode.ToString();
            string returnCode = mResponseJson.returnCode.ToString();
            switch (returnCode)
            {
                case "ERR_PFO_00068": // What is this error?
                case "ERR_PFO_00291": // Username already exists for the given site.
                    returnCodeColor = "#ff0000";
                    break;

                case "ERR_PFO_00069": // KYC Check Failure - No questions returned.
                    returnCodeColor = "#ff0000";
                    break;

                case "INFO_PFO_00021": // Please answer personal verification questions.
                    returnCodeColor = "#068415";
                    //var rt = mResponseJson.responseToken.ToString();
                    ParseJsonQuestionToForm(mResponseJson);
                    break;

                case "INFO_PFO_00023":
                    returnCodeColor = "#068415";
                    // We have a valid registration response from ST
                    if (mResponseJson.playerDetails != null)
                    {
                        txtPlayerPlayerId.Value = myPlayer.stiPlayerId = mResponseJson.playerDetails.stPlayerId;
                        txtPlayerReferenceNumber.Value = myPlayer.stiReferenceNumber = mResponseJson.playerDetails.stReferenceNo;
                        myDatabase.Save(myPlayer);
                    }
                    divPlayerContainer.Visible = false;
                    divPlayerOperationsContainer.Visible = true;
                    ShowCodeAndMsg(returnCode, returnMsg);
                    return;

                default:
                    returnCodeColor = "#ff0000";
                    //divOperatorContainer.Visible = true;
                    divPlayerContainer.Visible = true;
                    divPlayerOperationsContainer.Visible = false;
                    ShowCodeAndMsg(returnCode, returnMsg);
                    return;
            }
            // step A2 -----------  validate player, after register (to get connectionToken and sessionToken
            var ValidatePlayerAndUpdateInfo_rtn = ValidatePlayerAndUpdateInfo("2A");
            if (ValidatePlayerAndUpdateInfo_rtn.Item1 == false)
            {
                ShowCodeAndMsg(ValidatePlayerAndUpdateInfo_rtn.Item2, ValidatePlayerAndUpdateInfo_rtn.Item3);
                return;
            }
            // step A3 ----------- combined pre paid registration and load

            var mRequestData3 = Construct_PrePaidRegisterAndLoad_RequestJson(myPlayer);

            AddToLogAndDisplay(hiddenFlow.Value, "3A) stapiPrePaidRegisterAndLoad Request", mRequestData3);
            var mResponseData3 = stiOperations.stapiPrePaidRegisterAndLoad(mRequestData3);
            AddToLogAndDisplay(hiddenFlow.Value, "3A) stapiPrePaidRegisterAndLoad Response", mResponseData3);
            dynamic mResponseJson3 = JObject.Parse(mResponseData3);
            returnMsg = mResponseJson3.returnMsg.ToString();
            returnCode = mResponseJson3.returnCode.ToString();

            ShowAll();
            ShowCodeAndMsg(returnCode, returnMsg);

        }
        else
        { //-------------- player is already registered, do balance check
            // Step B1 -------------- get pre paid balance
            myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
            var GetPrePaidAccountBalance_RequestData = Construct_GetPrePaidAccountBalance_RequestJson(myPlayer);
            AddToLogAndDisplay(hiddenFlow.Value, "B1) SubmitPrePaidBalanceRequest Request", GetPrePaidAccountBalance_RequestData);

            var GetPrePaidAccountBalance_ResponseData = stiOperations.stapiGetPrePaidAccountBalance(GetPrePaidAccountBalance_RequestData);

            AddToLogAndDisplay(hiddenFlow.Value, "B1) SubmitPrePaidBalanceRequest Response", GetPrePaidAccountBalance_ResponseData);
            //dynamic GetPrePaidAccountBalance_ResponseJson = JObject.Parse(GetPrePaidAccountBalance_ResponseData);
            //string GetPrePaidAccountBalance_ReturnMsg = GetPrePaidAccountBalance_ResponseJson.returnMsg.ToString();
            //string GetPrePaidAccountBalance_ReturnCode = GetPrePaidAccountBalance_ResponseJson.returnCode.ToString();
            //if (GetPrePaidAccountBalance_ReturnMsg != "INFO_PFO_00023")
            //{
            //    ShowCodeAndMsg(GetPrePaidAccountBalance_ReturnCode, GetPrePaidAccountBalance_ReturnMsg);
            //    return;
            //}
            //// successful
            //string GetPrePaidAccountToken = GetPrePaidAccountBalance_ResponseJson.prePaidAccountDetails.prePaidAccountToken.ToString();
            //string mGetPrePaidAccountCurrency = GetPrePaidAccountBalance_ResponseJson.prePaidAccountDetails.prePaidAccountCurrency.ToString();
            //string mGetPrePaidAccountBalance = GetPrePaidAccountBalance_ResponseJson.prePaidAccountDetails.prePaidAccountBalance.ToString();
            //// get requested load amount 
            //dynamic mPrePaidRegisterAndLoad_RequestJson = JObject.Parse(mPrePaidRegisterAndLoad_RequestData);
            //string LoadAmpunt = mPrePaidRegisterAndLoad_RequestJson.depositDetails.amount.ToString();
            //// check funds
            //if (ToNumeric(mGetPrePaidAccountBalance) < ToNumeric(LoadAmpunt))
            //{ //insuffience prepaid funds to load account

            //}
            //else
            //{  // suffience funds in prepaid account to cover load



            //}


        }
        // Step B2 -------------- get pre paid balance

        ShowAll();
        ShowCodeAndMsg("000000", "NI");
    }
    #endregion

    #endregion

    #region Wrapper Flows

    #region Wrapper 1 stapiRegisterPlayer OTP (loopback tested)
    private void stapiRegisterPlayerFlow()
    {
        var mRegisterPlayer_RequestData = Construct_RegisterPlayer_RequestJson();
        AddToLogAndDisplay(hiddenFlow.Value, "stapiRegisterPlayer Request", mRegisterPlayer_RequestData);

        var mResponseData = WrapperService("stapiRegisterPlayer", mRegisterPlayer_RequestData);
        
        AddToLogAndDisplay(hiddenFlow.Value, "stapiRegisterPlayer Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnMsg = mResponseJson.returnMsg.ToString();
        string returnCode = mResponseJson.returnCode.ToString();
        switch (returnCode)
        {
            case "ERR_PFO_00068": // What is this error?
            case "ERR_MAC_20045": // MAC EndUser already registered in system
                returnCodeColor = "#ff0000";
                break;

            case "INFO_MAC_20021": // Validate OTP                    
                returnCodeColor = "#068415";
                // Update the Player's username in central seed database
                spanOtpMessage.InnerHtml = returnMsg;
                hiddenPageAction.Value = "ValidateOtp";

                hiddenRequestToken.Value = "";
                var requestId = mResponseJson.responseToken;
                if (requestId != null)
                    hiddenRequestToken.Value = requestId.Value.ToString();

                // otp to form if included in response
                txtOtp.Value = "";
                var OTPProperty = mResponseJson.Property("OTP");
                if (OTPProperty != null) //check if OTPProperty exists
                {
                    var mOTP = OTPProperty.Value.ToString();
                    txtOtp.Value = mOTP.Trim();
                }

                // ad to form if included in response
                divImageAdContainer.InnerHtml = string.Empty;
                var OTPEnterOTPAd = mResponseJson.Property("EnterOTPAd");
                if (OTPEnterOTPAd != null) //check if OTPProperty exists
                {
                    var adInfo = OTPEnterOTPAd.Value.ToString();
                    divImageAdContainer.InnerHtml = HexToString(adInfo.Trim());
                }

                // If successfull, hide PlayerContainer and show PlayerOperations
                //divOperatorContainer.Visible = false;
                divPlayerContainer.Visible = false;
                divPlayerOperationsContainer.Visible = true;
                divOtpValidate.Visible = true;
                // Reinit this
                txtOtp.Focus();
                break;

            default:
                returnCodeColor = "#ff0000";
                //divOperatorContainer.Visible = true;
                divPlayerContainer.Visible = true;
                divPlayerOperationsContainer.Visible = false;
                break;
        }
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region Wrapper 2 stapiPlayerLogin OTP (loopback tested)
    private void stapiPlayerLoginFlow()
    {
        var mRequestData = Construct_OperatorInfo_RequestJson();
        try
        {
            AddToLogAndDisplay(hiddenFlow.Value, "stapiPlayerLogin Request", mRequestData);
            var mResponseData = WrapperService("stapiPlayerLogin", mRequestData);
            AddToLogAndDisplay(hiddenFlow.Value, "stapiPlayerLogin Response", mResponseData);
            dynamic mPlayerLogin_ResponseJson = JObject.Parse(mResponseData);
            string returnMsg = mPlayerLogin_ResponseJson.returnMsg.ToString();
            string returnCode = mPlayerLogin_ResponseJson.returnCode.ToString();
            switch (returnCode)
            {
                case "INFO_MAC_20021": // Validate OTP
                    returnCodeColor = "#068415";
                    spanOtpMessage.InnerHtml = returnMsg;
                    hiddenPageAction.Value = "ValidateOtp";

                    hiddenRequestToken.Value = "";
                    var requestId = mPlayerLogin_ResponseJson.responseToken;
                    if (requestId != null)
                        hiddenRequestToken.Value = requestId.Value.ToString();

                    // otp to form if included in response
                    txtOtp.Value = "";
                    var OTPProperty = mPlayerLogin_ResponseJson.Property("OTP");
                    if (OTPProperty != null) //check if OTPProperty exists
                    {
                        var mOTP = OTPProperty.Value.ToString();
                        txtOtp.Value = mOTP.Trim();
                    }

                    // ad to form if included in response
                    divImageAdContainer.InnerHtml = string.Empty;
                    var OTPEnterOTPAd = mPlayerLogin_ResponseJson.Property("EnterOTPAd");
                    if (OTPEnterOTPAd != null) //check if OTPProperty exists
                    {
                        var adInfo = OTPEnterOTPAd.Value.ToString();
                        divImageAdContainer.InnerHtml = HexToString(adInfo.Trim());
                    }

                    // If successfull, hide PlayerContainer and show PlayerOperations
                    divPlayerContainer.Visible = false;
                    divPlayerOperationsContainer.Visible = true;
                    divOtpValidate.Visible = true;
                    // Reinit this
                    txtOtp.Focus();
                    break;

                default:
                    returnCodeColor = "#ff0000";
                    //divOperatorContainer.Visible = true;
                    divPlayerContainer.Visible = true;
                    divPlayerOperationsContainer.Visible = false;
                    break;
            }
            ShowCodeAndMsg(returnCode, returnMsg);
        }
        catch (Exception ex)
        {
           showError("Exception: " + hiddenService.Value + "." + dlWorkflows.SelectedValue + ", " + ex.Message);
        }

    }
    #endregion

    #region Wrapper 3 stapiPreCheckSiteValidatePlayerRequest - Passthru (loopback tested)
    private void stapiPreCheckSiteValidatePlayerRequestFlow()
    {
        // Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        returnCodeColor = "#068415";
        if (rtn.Item1 == false)
        {
            returnCodeColor = "#ff0000";
        }
        ShowCodeAndMsg(rtn.Item2, rtn.Item3);
    }
    #endregion

    #region Wrapper 4 stapiPreCheckSiteCardRequest Passthru (loopback tested)
    private void stapiPreCheckSiteCardRequestFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }

        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

        //Step 2 Register the Card
        var mRequestData = Construct_PreCheckSiteCardRequest_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteCardRequest Request", mRequestData);

        var mResponseData = WrapperService("stapiPreCheckSiteCardRequest", mRequestData);
        
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteCardRequest Response", mResponseData);
        dynamic mPreCheckSiteCardRequest_ResponseData = JObject.Parse(mResponseData);
        string mReturnMsg = mPreCheckSiteCardRequest_ResponseData.returnMsg.ToString();
        string mReturnCode = mPreCheckSiteCardRequest_ResponseData.returnCode.ToString();
        if (mReturnCode == "INFO_PFO_00016")
        {
            returnCodeColor = "#068415";
            // Successfully Registered card, save card token
            txtPCICardToken.Value = myPlayer.PCICardToken =
                mPreCheckSiteCardRequest_ResponseData.cardRegistrationDetails.cardDetails.cardToken.ToString();
        }
        else
        {
            returnCodeColor = "#ff0000";
            txtPCICardToken.Value = myPlayer.PCICardToken = string.Empty;
        }
        myDatabase.Save(myPlayer);
        ShowCodeAndMsg(mReturnCode, mReturnMsg);
    }
    #endregion

    #region Wrapper 5 DeleteRegisteredCard Passthru (loopback tested)
    private void DeleteRegisteredCardFlow()
    {
        // Step 1 Validate the Player
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }

        if (String.IsNullOrEmpty(txtPCIDefaultCard.Value))
        {
            showError("DeleteRegisteredCardFlow: txtPCIDefaultCard.Value not set!");
            return;
        }

        //Step 2 Delete Registered Card using stapiPreCheckSiteModifyCardRequest method
        var mRequestData = Construct_PreCheckSiteModifyCardRequest_Json(txtPCIDefaultCard.Value, "Y");
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteModifyCardRequest.Delete Request", mRequestData);

        var ResponseData = WrapperService("stapiPreCheckSiteModifyCardRequest", mRequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteModifyCardRequest.Delete Response", ResponseData);
        dynamic ResponseJson = JObject.Parse(ResponseData);
        string returnCode = ResponseJson.returnCode.ToString();
        string returnMsg = ResponseJson.returnMsg.ToString();
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        if (returnCode == "INFO_PFO_00038")
        {
            // card successfull deleted
            returnCodeColor = "#068415";
            returnMsg += ", " + ResponseJson.recordStatus.ToString();
            txtPCICardToken.Value = myPlayer.PCICardToken = ResponseJson.cardToken.ToString();
        }
        else
        {
            returnCodeColor = "#ff0000";
            txtPCICardToken.Value = myPlayer.PCICardToken = string.Empty;        
        }
        myDatabase.Save(myPlayer);
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region Wrapper 6 stapiRegisterPrePaidAccount (loopback tested)
    private void stapiRegisterPrePaidAccountFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

        //Step 2 Register Pre Paid Account
        var mRequestData = Construct_RegisterPrePaidAccount_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiRegisterPrePaidAccount Request", mRequestData);
        var mResponseData = WrapperService("stapiRegisterPrePaidAccount", mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiRegisterPrePaidAccount Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        switch (returnCode)
        {
            case "INFO_PFO_00042":
                returnCodeColor = "#068415";
                txtPPANumber.Value = myPlayer.PPANumber = mResponseJson.prePaidAccountRegistrationDetails.prePaidAccountNumber.ToString();
                txtPPAToken.Value = myPlayer.PPAToken = mResponseJson.prePaidAccountRegistrationDetails.prePaidAccountToken.ToString();
                txtPPAVirtualCardNumber.Value = myPlayer.PPAVirtualCardNumber = mResponseJson.prePaidAccountRegistrationDetails.virtualCardNumber.ToString();
                /*
                { 
                "returnCode":"INFO_PFO_00042", 
                "returnMsg":"Pre Paid Account registered successfully.", 
                "stTimeStamp":"2012-08-13T11:54:21.804+05:30", 
                "recordStatus":"Created", 
                "prePaidAccountRegistrationDetails": 
                    { 
                    "operatorId":"000", 
                    "siteId":"000", 
                    "userName":"Johnsmith1", 
                    "prePaidAccountNumber":"51############01",  
                    "prePaidAccountToken":"IpFlsWGtfoP379vDAVsMi-g=", 
                    "virtualCardNumber":"0000000000000000", 
                    "idCheckStatus":"PASSED", 
                    "supportPhoneNumber":"1234567895", 
                    "expiryDate":"mm/yyyy", 
                    "receiptHtml":"<value>" 
                    } 
                } 

                 * */
                break;
            case "INFO_PFO_00043":  // ask questions response
                returnCodeColor = "#068415";
                ParseJsonQuestionToForm(mResponseData);
                break;
            default:
                returnCodeColor = "#ff0000";
                break;
        }

        ShowAll();
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion
    
    #region Wrapper 7 stapiLoadPrePaidFunds OTP (Loopback tested)
    private void stapiLoadPrePaidFundsFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }

        //Step 2 Load PrePaid Funds, uses Authorization OTP
        var mRequestData = Construct_LoadFundstoPrePaid_RequestJson(); //uses info from form
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiLoadPrePaidFunds Request", mRequestData);

        var mResponseData = WrapperService("stapiLoadPrePaidFunds", mRequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiLoadPrePaidFunds Response", mResponseData);
        dynamic mLoadPrePaidFunds_ResponseJson = JObject.Parse(mResponseData);
        string returnCode = mLoadPrePaidFunds_ResponseJson.returnCode.ToString();
        string returnMsg = mLoadPrePaidFunds_ResponseJson.returnMsg.ToString();
        switch (returnCode)
        {
            case "INFO_MAC_20021": // Validate OTP
                returnCodeColor = "#068415";
                spanOtpMessage.InnerHtml = returnMsg;
                hiddenPageAction.Value = "ValidateOtp";

                var mOtpRequestId = mLoadPrePaidFunds_ResponseJson.responseToken;
                if (mOtpRequestId == null)
                {
                    hiddenRequestToken.Value = "";
                    returnMsg = returnMsg + ", Missing OTP requestId!";
                    break;
                }
                hiddenRequestToken.Value = mOtpRequestId;

                // otp to form if included in response
                txtOtp.Value = "";
                var OTPProperty = mLoadPrePaidFunds_ResponseJson.Property("OTP");
                if (OTPProperty != null) //check if OTPProperty exists
                {
                    var mOTP = OTPProperty.Value.ToString();
                    txtOtp.Value = mOTP.Trim();
                }

                // ad to form if included in response
                divImageAdContainer.InnerHtml = string.Empty;
                var OTPEnterOTPAd = mLoadPrePaidFunds_ResponseJson.Property("EnterOTPAd");
                if (OTPEnterOTPAd != null) //check if OTPProperty exists
                {
                    var adInfo = OTPEnterOTPAd.Value.ToString();
                    divImageAdContainer.InnerHtml = HexToString(adInfo.Trim());
                }

                // If successfull, hide PlayerContainer and show PlayerOperations
                //divOperatorContainer.Visible = false;
                divPlayerContainer.Visible = false;
                divPlayerOperationsContainer.Visible = true;
                divOtpValidate.Visible = true;

                // Reinit this
                txtOtp.Focus();
                return;

            default:
                returnCodeColor = "#ff0000";
                //divOperatorContainer.Visible = true;
                divPlayerContainer.Visible = true;
                divPlayerOperationsContainer.Visible = false;
                divOtpValidate.Visible = false;
                return;
        }

        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region Wrapper 8 stapiSubmitPrePaidDeposit OTP (loopback tested)
    private void stapiSubmitPrePaidDepositFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        //Step 2 Pre Paid Account Deposit, uses Authorization OTP
        var mRequestData = Construct_SubmitPrePaidDeposit_RequestJson(); //uses info from form
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitPrePaidDeposit Request", mRequestData);
        var mResponseData = WrapperService("stapiSubmitPrePaidDeposit", mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitPrePaidDeposit Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        switch (returnCode)
        {
            case "INFO_MAC_20021": // Validate OTP
                spanOtpMessage.InnerHtml = returnMsg;
                hiddenPageAction.Value = "ValidateOtp";

                var mOtpRequestId = mResponseJson.responseToken;
                if (mOtpRequestId == null)
                {
                    hiddenRequestToken.Value = "";
                    returnMsg = returnMsg + ", Missing OTP requestId!";
                    break;
                }
                hiddenRequestToken.Value = mOtpRequestId;

                // otp to form if included in response
                txtOtp.Value = "";
                var OTPProperty = mResponseJson.Property("OTP");
                if (OTPProperty != null) //check if OTPProperty exists
                {
                    var mOTP = OTPProperty.Value.ToString();
                    txtOtp.Value = mOTP.Trim();
                }

                // ad to form if included in response
                divImageAdContainer.InnerHtml = string.Empty;
                var OTPEnterOTPAd = mResponseJson.Property("EnterOTPAd");
                if (OTPEnterOTPAd != null) //check if OTPProperty exists
                {
                    var adInfo = OTPEnterOTPAd.Value.ToString();
                    divImageAdContainer.InnerHtml = HexToString(adInfo.Trim());
                }

                // If successfull, hide PlayerContainer and show PlayerOperations
                //divOperatorContainer.Visible = false;
                divPlayerContainer.Visible = false;
                divPlayerOperationsContainer.Visible = true;
                divOtpValidate.Visible = true;

                // Reinit this
                txtOtp.Focus();
                return;

            default:
                //divOperatorContainer.Visible = true;
                divPlayerContainer.Visible = true;
                divPlayerOperationsContainer.Visible = false;
                divOtpValidate.Visible = false;
                return;
        }

        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region Wrapper 9 stapiSubmitDepositAccountRequest OTP (loopback tested)
    private void stapiSubmitDepositAccountRequestFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

        //Step 2 Pre Paid Withdrawl Request
        var mRequestData = Construct_SubmitDepositAccountRequest_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitDepositAccountRequest Request", mRequestData);

        var mResponseData = WrapperService("stapiSubmitDepositAccountRequest", mRequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitDepositAccountRequest Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        switch (returnCode)
        {
            case "INFO_MAC_20021": // Validate OTP
                spanOtpMessage.InnerHtml = returnMsg;
                hiddenPageAction.Value = "ValidateOtp";

                var mOtpRequestId = mResponseJson.responseToken;
                if (mOtpRequestId == null)
                {
                    hiddenRequestToken.Value = "";
                    returnMsg = returnMsg + ", Missing OTP requestId!";
                    break;
                }
                hiddenRequestToken.Value = mOtpRequestId;

                // otp to form if included in response
                txtOtp.Value = "";
                var OTPProperty = mResponseJson.Property("OTP");
                if (OTPProperty != null) //check if OTPProperty exists
                {
                    var mOTP = OTPProperty.Value.ToString();
                    txtOtp.Value = mOTP.Trim();
                }

                // ad to form if included in response
                divImageAdContainer.InnerHtml = string.Empty;
                var OTPEnterOTPAd = mResponseJson.Property("EnterOTPAd");
                if (OTPEnterOTPAd != null) //check if OTPProperty exists
                {
                    var adInfo = OTPEnterOTPAd.Value.ToString();
                    divImageAdContainer.InnerHtml = HexToString(adInfo.Trim());
                }

                // If successfull, hide PlayerContainer and show PlayerOperations
                //divOperatorContainer.Visible = false;
                divPlayerContainer.Visible = false;
                divPlayerOperationsContainer.Visible = true;
                divOtpValidate.Visible = true;

                // Reinit this
                txtOtp.Focus();
                return;

            default:
                //divOperatorContainer.Visible = true;
                divPlayerContainer.Visible = true;
                divPlayerOperationsContainer.Visible = false;
                divOtpValidate.Visible = false;
                return;
        }
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region Wrapper 10 stapiSubmitPrePaidWithdrawal OTP (loopback tested)
    private void stapiSubmitPrePaidWithdrawalFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

        //Step 2 Pre Paid Withdrawl Request
        var mRequestData = Construct_SubmitPrePaidWithdrawal_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitPrePaidWithdrawal Request", mRequestData);
        var mResponseData = WrapperService("stapiSubmitPrePaidWithdrawal", mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitPrePaidWithdrawal Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        switch (returnCode)
        {
            case "ERR_PFO_00068": // What is this error?
            case "ERR_MAC_20045": 
                returnCodeColor = "#ff0000";
                break;

            case "INFO_MAC_20021": // Validate OTP                    
                returnCodeColor = "#068415";
                spanOtpMessage.InnerHtml = returnMsg;
                hiddenPageAction.Value = "ValidateOtp";

                hiddenRequestToken.Value = "";
                var requestId = mResponseJson.responseToken;
                if (requestId != null)
                    hiddenRequestToken.Value = requestId.Value.ToString();

                // otp to form if included in response
                txtOtp.Value = "";
                var OTPProperty = mResponseJson.Property("OTP");
                if (OTPProperty != null) //check if OTPProperty exists
                {
                    var mOTP = OTPProperty.Value.ToString();
                    txtOtp.Value = mOTP.Trim();
                }

                // ad to form if included in response
                divImageAdContainer.InnerHtml = string.Empty;
                var OTPEnterOTPAd = mResponseJson.Property("EnterOTPAd");
                if (OTPEnterOTPAd != null) //check if OTPProperty exists
                {
                    var adInfo = OTPEnterOTPAd.Value.ToString();
                    divImageAdContainer.InnerHtml = HexToString(adInfo.Trim());
                }

                // If successfull, hide PlayerContainer and show PlayerOperations
                //divOperatorContainer.Visible = false;
                divPlayerContainer.Visible = false;
                divPlayerOperationsContainer.Visible = true;
                divOtpValidate.Visible = true;
                // Reinit this
                txtOtp.Focus();
                break;

            default:
                returnCodeColor = "#ff0000";
                //divOperatorContainer.Visible = true;
                divPlayerContainer.Visible = true;
                divPlayerOperationsContainer.Visible = false;
                break;
        }
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region Wrapper 11 stapiGetTransactionDetails Passthru (loopback tested)
    private void stapiGetTransactionDetailsFlow()
    {
        var mRequestData = Construct_GetTransactionDetails_RequestJson();
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetTransactionDetails Request", mRequestData);

        var mResponseData = WrapperService("stapiGetTransactionDetails", mRequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetTransactionDetails Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();

    }
    #endregion

    #region Wrapper 12 stapiUpdateTransaction Passthru (loopback tested)
    private void stapiUpdateTransactionFlow()
    {
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_UpdateTransaction_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiUpdateTransaction Request", mRequestData);

        var mResponseData = WrapperService("stapiUpdateTransaction", mRequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiUpdateTransaction Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region Wrapper 13 stapiGetRegisteredAccounts (loopback tested)
    private void stapiGetRegisteredAccountsFlow()
    {
        var mRequestData = Construct_GetRegisteredAccounts_RequestJson();
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetRegisteredAccounts Request", mRequestData);

        var mResponseData = WrapperService("stapiGetRegisteredAccounts", mRequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetRegisteredAccounts Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }

    #endregion

    #region Wrapper 14 stapiGetRegisteredCards Passthru (loopback tested)
    private void stapiGetRegisteredCardsFlow()
    {
        var mRequestData = Construct_GetRegisteredCards_RequestJson();
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetRegisteredCards_RequestData Request", mRequestData);
        
        var mResponseData = WrapperService("stapiGetRegisteredCards", mRequestData);
        
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetRegisteredCards_RequestData Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region Wrapper 15 stapiGetPlayerSSN Passthru (loopback tested)
    private void stapiGetPlayerSSNFlow()
    {
        var mRequestData = Construct_GetPlayerSSN_RequestJson();
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiGetPlayerSSN Request", mRequestData);
        var mResponseData = WrapperService("stapiGetPlayerSSN", mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiGetPlayerSSN Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";

        ShowCodeAndMsg(returnCode, returnMsg); 
        ShowAll();
    }
    #endregion

    #region Wrapper 16 stapiUpdatePlayerSSN Passthru (loopback tested)
    private void stapiUpdatePlayerSSNFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }

        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

        //Step 2 UpdatePlayerSSN
        var RequestData = Construct_UpdatePlayerSSN_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiUpdatePlayerSSN Request", RequestData);

        var mResponseData = WrapperService("stapiUpdatePlayerSSN", RequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiUpdatePlayerSSN Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";

        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region Wrapper 17 stapiGetPrePaidAccountHolderInfo Passthru (loopback tested)
    private void stapiGetPrePaidAccountHolderInfoFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Get PrePaid Account Holder Info
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestJson = Construct_GetPrePaidAccountHolderInfo_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiGetPrePaidAccountHolderInfo Request", mRequestJson);
        var mResponseData = WrapperService("stapiGetPrePaidAccountHolderInfo", mRequestJson);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiGetPrePaidAccountHolderInfo Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";

        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region Wrapper 18 stapiGetPrePaidAccountBalance Passthru (not tested) - needs account token
    private void stapiGetPrePaidAccountBalanceFlow()
    {
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_GetPrePaidAccountBalance_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetPrePaidAccountBalance Request", mRequestData);
        var mResponseData = WrapperService("stapiGetPrePaidAccountBalance", mRequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "1) stapiGetPrePaidAccountBalance Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnMsg = mResponseJson.returnMsg.ToString();
        string returnCode = mResponseJson.returnCode.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region Wrapper 19 stapiGetPrePaidAccountCVV2 Passthru (Not tested)
    private void stapiGetPrePaidAccountCVV2Flow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Get PrePaid Account CVV2
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var RequestData = Construct_GetPrePaidAccountCVV2_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiGetPrePaidAccountCVV2 Request", RequestData);
        var mResponseData = WrapperService("stapiGetPrePaidAccountCVV2", RequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiGetPrePaidAccountCVV2 Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";

        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region Wrapper 20 stapiPreCheckSiteAccountRequest Passthru (Not tested)
    private void stapiPreCheckSiteAccountRequestFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Pre Check Site Account Request
        var RequestData = Construct_PreCheckSiteAccountRequest_RequestJson();
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteAccountRequest Request", RequestData);

        var mResponseData = WrapperService("stapiPreCheckSiteAccountRequest", RequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteAccountRequest Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00033" ? "#068415" : "#ff0000";

        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region Wrapper 21 stapiPrePaidRegisterAndLoad - Passthru (Not tested)
    private void stapiPrePaidRegisterAndLoadFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Pre Paid Register And Load
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var RequestData = Construct_PrePaidRegisterAndLoad_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPrePaidRegisterAndLoad Request", RequestData);

        var mResponseData = WrapperService("stapiPrePaidRegisterAndLoad", RequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPrePaidRegisterAndLoad Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";

        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region Wrapper 22 stapiUpdatePrepPaidAccountStatus (loopback tested)
    private void stapiUpdatePrePaidAccountStatusFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Update PrepPaid Account Status
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var RequestData = Construct_UpdatePrePaidAccountStatus_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiUpdatePrepPaidAccountStatus Request", RequestData);
        var mResponseData = WrapperService("stapiUpdatePrePaidAccountStatus", RequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiUpdatePrepPaidAccountStatus Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        if (returnCode == "INFO_PFO_00023")
        {
            returnCodeColor = "#068415";
            // Successfully Registered card, save card token
            txtPPAToken.Value = myPlayer.PPAToken =
                mResponseJson.prePaidAccountDetails.prePaidAccountToken.ToString();
        }
        else
        {
            returnCodeColor = "#ff0000";
            txtPPAToken.Value = myPlayer.PPAToken = string.Empty;
        }
        myDatabase.Save(myPlayer);
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region Wrapper 23 stapiSubmitDepositRequest OTP (loopback tested)
    private void stapiSubmitDepositRequestFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Submit Deposit Request
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var RequestData = Construct_stapiSubmitDepositRequest_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitDepositRequest Request", RequestData);

        var mResponseData = WrapperService("stapiSubmitDepositRequest", RequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitDepositRequest Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        switch (returnCode)
        {
            case "ERR_PFO_00068": // What is this error?
            case "ERR_MAC_20045": 
                returnCodeColor = "#ff0000";
                break;

            case "INFO_MAC_20021": // Validate OTP                    
                returnCodeColor = "#068415";
                spanOtpMessage.InnerHtml = returnMsg;
                hiddenPageAction.Value = "ValidateOtp";

                hiddenRequestToken.Value = "";
                var requestId = mResponseJson.responseToken;
                if (requestId != null)
                    hiddenRequestToken.Value = requestId.Value.ToString();

                // otp to form if included in response
                txtOtp.Value = "";
                var OTPProperty = mResponseJson.Property("OTP");
                if (OTPProperty != null) //check if OTPProperty exists
                {
                    var mOTP = OTPProperty.Value.ToString();
                    txtOtp.Value = mOTP.Trim();
                }

                // ad to form if included in response
                divImageAdContainer.InnerHtml = string.Empty;
                var OTPEnterOTPAd = mResponseJson.Property("EnterOTPAd");
                if (OTPEnterOTPAd != null) //check if OTPProperty exists
                {
                    var adInfo = OTPEnterOTPAd.Value.ToString();
                    divImageAdContainer.InnerHtml = HexToString(adInfo.Trim());
                }

                // If successfull, hide PlayerContainer and show PlayerOperations
                //divOperatorContainer.Visible = false;
                divPlayerContainer.Visible = false;
                divPlayerOperationsContainer.Visible = true;
                divOtpValidate.Visible = true;
                // Reinit this
                txtOtp.Focus();
                break;

            default:
                returnCodeColor = "#ff0000";
                //divOperatorContainer.Visible = true;
                divPlayerContainer.Visible = true;
                divPlayerOperationsContainer.Visible = false;
                break;
        }
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region Wrapper 24 stapiAccountWithdrawal OTP (loopback tested)
    private void stapiAccountWithdrawalFlow()
    {
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var RequestData = Construct_AccountWithdrawal_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiAccountWithdrawal Request", RequestData);
        var mResponseData = WrapperService("stapiAccountWithdrawal", RequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiAccountWithdrawal Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        switch (returnCode)
        {
            case "ERR_PFO_00068": // What is this error?
            case "ERR_MAC_20045":
                returnCodeColor = "#ff0000";
                break;

            case "INFO_MAC_20021": // Validate OTP                    
                returnCodeColor = "#068415";
                spanOtpMessage.InnerHtml = returnMsg;
                hiddenPageAction.Value = "ValidateOtp";

                hiddenRequestToken.Value = "";
                var requestId = mResponseJson.responseToken;
                if (requestId != null)
                    hiddenRequestToken.Value = requestId.Value.ToString();

                // otp to form if included in response
                txtOtp.Value = "";
                var OTPProperty = mResponseJson.Property("OTP");
                if (OTPProperty != null) //check if OTPProperty exists
                {
                    var mOTP = OTPProperty.Value.ToString();
                    txtOtp.Value = mOTP.Trim();
                }

                // ad to form if included in response
                divImageAdContainer.InnerHtml = string.Empty;
                var OTPEnterOTPAd = mResponseJson.Property("EnterOTPAd");
                if (OTPEnterOTPAd != null) //check if OTPProperty exists
                {
                    var adInfo = OTPEnterOTPAd.Value.ToString();
                    divImageAdContainer.InnerHtml = HexToString(adInfo.Trim());
                }

                // If successfull, hide PlayerContainer and show PlayerOperations
                //divOperatorContainer.Visible = false;
                divPlayerContainer.Visible = false;
                divPlayerOperationsContainer.Visible = true;
                divOtpValidate.Visible = true;
                // Reinit this
                txtOtp.Focus();
                break;

            default:
                returnCodeColor = "#ff0000";
                //divOperatorContainer.Visible = true;
                divPlayerContainer.Visible = true;
                divPlayerOperationsContainer.Visible = false;
                break;
        }
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region Wrapper 25 stapiSelfExcludePlayer Passthru (loopback tested)
    private void stapiSelfExcludePlayerFlow()
    {
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var RequestData = Construct_SelfExcludePlayer_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiAccountWithdrawal Request", RequestData);

        var mResponseData = WrapperService("stapiSelfExcludePlayer", RequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSelfExcludePlayer Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00002" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region Wrapper 26 stapiModifyPlayerRequest Passthru (Not  tested)
    private void stapiModifyPlayerRequestFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Modify Player Request
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var RequestData = Construct_ModifyPlayerRequest_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiModifyPlayerRequest Request", RequestData);

        var mResponseData = WrapperService("stapiModifyPlayerRequest", RequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiModifyPlayerRequest Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00021" ? "#068415" : "#ff0000";

        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region Wrapper 27 stapiAuthDepositRequest Passthru (loopback tested)
    private void stapiAuthDepositRequestFlow()
    {
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var RequestData = Construct_AuthDepositRequest_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiAuthDepositRequest Request", RequestData);

        var mResponseData = WrapperService("stapiAuthDepositRequest", RequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiAuthDepositRequest Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnMsg = mResponseJson.returnMsg.ToString();
        string returnCode = mResponseJson.returnCode.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region Wrapper 28 stapiSubmitRefund Passthru (loopback  tested)
    private void stapiSubmitRefundFlow()
    {
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var RequestData = Construct_SubmitRefund_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitRefund Request", RequestData);

        var mResponseData = WrapperService("stapiSubmitRefund", RequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitRefund Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region Wrapper 29 stapiAddSelfExcludePlayer Passthru (loopback tested)
    private void stapiAddSelfExcludePlayerFlow()
    {
        // Step 1 Validate the Player, if valid update player info. else quit
        var rtn = ValidatePlayerAndUpdateInfo("1");
        if (rtn.Item1 == false)
        {
            ShowCodeAndMsg(rtn.Item2, rtn.Item3);
            return;
        }
        // Step 2 Add Self Exclude Player
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var RequestData = Construct_AddSelfExcludePlayer_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiAddSelfExcludePlayer Request", RequestData);

        var mResponseData = WrapperService("stapiAddSelfExcludePlayer", RequestData);

        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiAddSelfExcludePlayer Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();


        returnCodeColor = returnCode == "INFO_PFO_00004" ? "#068415" : "#ff0000";
        ShowCodeAndMsg(returnCode, returnMsg);
        ShowAll();
    }
    #endregion

    #region Wrapper 30 stapiWithdrawal Passthru (loopback tested)
    private void stapiWithdrawalFlow()
    {
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var RequestData = Construct_Withdrawal_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiWithdrawal Request", RequestData);
        var mResponseData = WrapperService("stapiWithdrawal", RequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiWithdrawal Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        switch (returnCode)
        {
            case "ERR_PFO_00068": // What is this error?
            case "ERR_MAC_20045":
                returnCodeColor = "#ff0000";
                break;

            case "INFO_MAC_20021": // Validate OTP                    
                returnCodeColor = "#068415";
                spanOtpMessage.InnerHtml = returnMsg;
                hiddenPageAction.Value = "ValidateOtp";

                hiddenRequestToken.Value = "";
                var requestId = mResponseJson.responseToken;
                if (requestId != null)
                    hiddenRequestToken.Value = requestId.Value.ToString();

                // otp to form if included in response
                txtOtp.Value = "";
                var OTPProperty = mResponseJson.Property("OTP");
                if (OTPProperty != null) //check if OTPProperty exists
                {
                    var mOTP = OTPProperty.Value.ToString();
                    txtOtp.Value = mOTP.Trim();
                }

                // ad to form if included in response
                divImageAdContainer.InnerHtml = string.Empty;
                var OTPEnterOTPAd = mResponseJson.Property("EnterOTPAd");
                if (OTPEnterOTPAd != null) //check if OTPProperty exists
                {
                    var adInfo = OTPEnterOTPAd.Value.ToString();
                    divImageAdContainer.InnerHtml = HexToString(adInfo.Trim());
                }

                // If successfull, hide PlayerContainer and show PlayerOperations
                //divOperatorContainer.Visible = false;
                divPlayerContainer.Visible = false;
                divPlayerOperationsContainer.Visible = true;
                divOtpValidate.Visible = true;
                // Reinit this
                txtOtp.Focus();
                break;

            default:
                returnCodeColor = "#ff0000";
                //divOperatorContainer.Visible = true;
                divPlayerContainer.Visible = true;
                divPlayerOperationsContainer.Visible = false;
                break;
        }
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region Wrapper 31 stapiSubmitReversal Passthru (loopback tested)
    private void stapiSubmitReversalFlow()
    {
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var RequestData = Construct_SubmitReversal_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitReversal Request", RequestData);
        var mResponseData = WrapperService("stapiSubmitReversal", RequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiSubmitReversal Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        myDatabase.Save(myPlayer);
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region Wrapper 32 stapiPreCheckSiteModifyAccountRequest Passthru (loopback tested)
    private void stapiPreCheckSiteModifyAccountRequestFlow()
    {
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var RequestData = Construct_PreCheckSiteModifyAccountRequest_RequestJson(myPlayer);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteModifyAccountRequest Request", RequestData);
        var mResponseData = WrapperService("stapiPreCheckSiteModifyAccountRequest", RequestData);
        AddToLogAndDisplay(hiddenFlow.Value, "2) stapiPreCheckSiteModifyAccountRequest Response", mResponseData);
        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnCode = mResponseJson.returnCode.ToString();
        string returnMsg = mResponseJson.returnMsg.ToString();
        returnCodeColor = returnCode == "INFO_PFO_00023" ? "#068415" : "#ff0000";
        myDatabase.Save(myPlayer);
        ShowCodeAndMsg(returnCode, returnMsg);
    }
    #endregion

    #region Wrapper - CombinedPlayerRegistrationPrePaidDeposit  OTP  (Not fully  implemented)
    private void CombinedPlayerRegistrationPrePaidDepositFlow()
    {
        //var myDatabase = new MongoDBData();
        //var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        //var CombinedPlayerRegistrationPrePaidDeposit_RequestJson = Construct_CombinedPlayerRegistrationPrePaidDeposit_RequestJson(myPlayer);
        divOtpValidate.Visible = true;

        ShowCodeAndMsg("99999", "Not implemented");
    }
    #endregion

    #endregion

    #region Wrapper - Common flow Methods
    private Tuple<bool, string, string> ValidatePlayerAndUpdateInfo(string pStep)
    {
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());
        var mRequestData = Construct_OperatorInfo_RequestJson();
        string mResponseData;
        // Validate the Player
        if (hiddenService.Value == "Wrapper")
        {
            AddToLogAndDisplay(hiddenFlow.Value, pStep + ") stapiPreCheckSiteValidatePlayerRequest Request", mRequestData);
            //--- call the wrapper service
            mResponseData = WrapperService("stapiPreCheckSiteValidatePlayerRequest", mRequestData);
            AddToLogAndDisplay(hiddenFlow.Value, pStep + ") stapiPreCheckSiteValidatePlayerRequest Response", mResponseData);
        }
        else
        {
            var stiOperations = new StiOperations();
            AddToLogAndDisplay(hiddenFlow.Value, pStep + ") stapiPreCheckSiteValidatePlayerRequest Request", mRequestData);
            // direct call to ST-1 Server
            mResponseData = stiOperations.stapiPreCheckSiteValidatePlayerRequest(mRequestData);
            AddToLogAndDisplay(hiddenFlow.Value, pStep + ") stapiPreCheckSiteValidatePlayerRequest Response", mResponseData);
        }

        dynamic mPreCheckSiteValidatePlayerResponseData = JObject.Parse(mResponseData);
        var mReturnMsg = mPreCheckSiteValidatePlayerResponseData.returnMsg.ToString();
        var mReturnCode = mPreCheckSiteValidatePlayerResponseData.returnCode.ToString();
        bool mSuccessFlag;
        if (mReturnCode == "INFO_PFO_00002")
        {
            mSuccessFlag = true;
            txtPlayerPlayerId.Value =
                myPlayer.stiPlayerId = mPreCheckSiteValidatePlayerResponseData.validationDetails.stPlayerId.ToString();
            txtPlayerReferenceNumber.Value =
                myPlayer.stiReferenceNumber =
                    mPreCheckSiteValidatePlayerResponseData.validationDetails.stReferenceNo.ToString();
            txtPlayerConnectionToken.Value =
                myPlayer.connectionToken =
                    mPreCheckSiteValidatePlayerResponseData.validationDetails.connectionToken.ToString();
            txtPlayerSessionToken.Value =
                myPlayer.sessionToken =
                    mPreCheckSiteValidatePlayerResponseData.validationDetails.sessionToken.ToString();
            myDatabase.Save(myPlayer);
        }
        else
        {
            mSuccessFlag = false;
        }
        return new Tuple<bool, string, string>(mSuccessFlag, mReturnCode, mReturnMsg);
    }

    private void ShowCodeAndMsg(string pCode, string pMessage)
    {
        divStatus.InnerHtml = "<span style='font-weight: bold;'>Return Code: </span><span style='color: "
        + returnCodeColor + ";'>"
        + pCode
        + "</span>"
        + "<span style='font-weight: bold;'> Status: </span><span style='font-weight: normal; color: "
        + returnCodeColor + ";'>"
        + pMessage
        + "</span>";
    }

    #endregion

    #region Button Events
    protected void btnSubmitSTDirect_Click(object sender, EventArgs e)
    {
        hiddenService.Value = "ST Direct";
        lbError.Text = "";

        try
        {
            switch (hiddenSelectedWorkFlow.Value)
            {
                case "Select":
                    return;

                case "RegisterPlayer":
                    hiddenFlow.Value = "ST Direct 1 stapiRegisterPlayer Flow";
                    ST_stapiRegisterPlayerFlow();
                    break;

                case "PlayerLogin":
                    hiddenFlow.Value = "ST Direct 2 stapiPlayerLogin Flow";
                    ST_stapiPlayerLoginFlow();
                    break;

                case "PreCheckSiteValidatePlayerRequest":
                    hiddenFlow.Value = "ST Direct 3 stapiPreCheckSiteValidatePlayerRequest Flow";
                    ST_stapiPreCheckSiteValidatePlayerRequestFlow();
                    break;

                case "PreCheckSiteCardRequest":
                    hiddenFlow.Value = "ST Direct 4 stapiPreCheckSiteCardRequest Flow";
                    ST_stapiPreCheckSiteCardRequestFlow();
                    break;

                case "DeleteRegisteredCard":
                    hiddenFlow.Value = "ST Direct 5 DeleteRegisteredCard Flow";
                    ST_DeleteRegisteredCardFlow();
                    break;

                case "RegisterPrePaidAccount":
                    hiddenFlow.Value = "ST Direct 6 stapiRegisterPrePaidAccount Flow";
                    ST_stapiRegisterPrePaidAccountFlow();
                    break;

                case "LoadPrePaidFunds":
                    hiddenFlow.Value = "ST Direct 7 stapiLoadPrePaidFunds Flow";
                    ST_stapiLoadPrePaidFundsFlow();
                    break;

                case "SubmitPrePaidDeposit":
                    hiddenFlow.Value = "ST Direct 8 stapiSubmitPrePaidDeposit Flow";
                    ST_stapiSubmitPrePaidDepositFlow();
                    break;

                case "SubmitDepositAccountRequest":
                    hiddenFlow.Value = "ST Direct 9 stapiSubmitDepositAccountRequestFlow";
                    ST_stapiSubmitDepositAccountRequestFlow();
                    break;

                case "SubmitPrePaidWithdrawal":
                    hiddenFlow.Value = "ST Direct 10 stapiSubmitPrePaidWithdrawal Flow";
                    ST_stapiSubmitPrePaidWithdrawalFlow();
                    break;

                case "GetTransactionDetails":
                    hiddenFlow.Value = "ST Direct 11 stapiGetTransactionDetails Flow";
                    ST_stapiGetTransactionDetailsFlow();
                    break;

                case "UpdateTransaction":
                    hiddenFlow.Value = "ST Direct 12 stapiUpdateTransaction Flow";
                    ST_stapiUpdateTransactionFlow();
                    break;

                case "GetRegisteredAccounts":
                    hiddenFlow.Value = "ST Direct 13 stapiGetRegisteredAccounts Flow";
                    ST_stapiGetRegisteredAccountsFlow();
                    break;

                case "GetRegisteredCards":
                    hiddenFlow.Value = "ST Direct 14 stapiGetRegisteredCards Flow";
                    ST_stapiGetRegisteredCardsFlow();
                    break;

                case "GetPlayerSSN":
                    hiddenFlow.Value = "ST Direct 15 stapiGetPlayerSSN Flow";
                    ST_stapiGetPlayerSSNFlow();
                    break;

                case "UpdatePlayerSSN":
                    hiddenFlow.Value = "ST Direct 16 stapiUpdatePlayerSSN Flow";
                    ST_stapiUpdatePlayerSSNFlow();
                    break;

                case "GetPrePaidAccountHolderInfo":
                    hiddenFlow.Value = "ST Direct 17 stapiGetPrePaidAccountHolderInfo Flow";
                    ST_stapiGetPrePaidAccountHolderInfoFlow();
                    break;

                case "GetPrePaidAccountBalance":
                    hiddenFlow.Value = "ST Direct 18 stapiGetPrePaidAccountBalance Flow";
                    ST_stapiGetPrePaidAccountBalanceFlow();
                    break;

                case "GetPrePaidAccountCVV2":
                    hiddenFlow.Value = "ST Direct 19 stapiGetPrePaidAccountCVV2 Flow";
                    ST_stapiGetPrePaidAccountCVV2Flow();
                    break;

                case "PreCheckSiteAccountRequest":
                    hiddenFlow.Value = "ST Direct 20 stapiPreCheckSiteAccountRequest Flow";
                    ST_stapiPreCheckSiteAccountRequestFlow();
                    break;

                case "PrePaidRegisterAndLoad":
                    hiddenFlow.Value = "ST Direct 21 stapiPrePaidRegisterAndLoadFlow";
                    ST_stapiPrePaidRegisterAndLoadFlow();
                    break;

                case "UpdatePrePaidAccountStatus":
                    hiddenFlow.Value = "ST Direct 22 stapiUpdatePrePaidAccountStatusFlow";
                    ST_stapiUpdatePrePaidAccountStatusFlow();
                    break;

                case "SubmitDepositRequest":
                    hiddenFlow.Value = "ST Direct 23 stapiSubmitDepositRequestFlow";
                    ST_stapiSubmitDepositRequestFlow();
                    break;

                case "AccountWithdrawal":
                    hiddenFlow.Value = "ST Direct 24 stapiAccountWithdrawalFlow";
                    ST_stapiAccountWithdrawalFlow();
                    break;

                case "SelfExcludePlayer":
                    hiddenFlow.Value = "ST Direct 25 stapiSelfExcludePlayerFlow";
                    ST_stapiSelfExcludePlayerFlow();
                    break;

                case "ModifyPlayerRequest":
                    hiddenFlow.Value = "ST Direct 26 stapiModifyPlayerRequestFlow";
                    ST_stapiModifyPlayerRequestFlow();
                    break;
                    
                case "AuthDepositRequest":
                    hiddenFlow.Value = "ST Direct 27 stapiAuthDepositRequestFlow";
                    ST_stapiAuthDepositRequestFlow();
                    break;
                    
                case "SubmitRefund":
                    hiddenFlow.Value = "ST Direct 28 stapiSubmitRefundFlow";
                    ST_stapiSubmitRefundFlow();
                    break;
                    
                case "AddSelfExcludePlayer":
                    hiddenFlow.Value = "ST Direct 29 stapiAddSelfExcludePlayerFlow";
                    ST_stapiAddSelfExcludePlayerFlow();
                    break;
                    
                case "Withdrawal":
                    hiddenFlow.Value = "ST Direct 30 stapiWithdrawal";
                    ST_stapiWithdrawalFlow();
                    break;

                case "SubmitReversal":
                    hiddenFlow.Value = "ST Direct 31 stapiSubmitReversal";
                    ST_stapiSubmitReversalFlow();
                    break;

                case "stapiPreCheckSiteModifyAccountRequest":
                    hiddenFlow.Value = "ST Direct 32 stapiPreCheckSiteModifyAccountRequest";
                    ST_stapiPreCheckSiteModifyAccountRequestFlow();
                    break;                    
 
                case "CombinedPlayerRegistrationPrePaidDeposit":
                    hiddenFlow.Value = "ST Direct - CombinedPlayerRegistrationPrePaidDeposit Flow";
                    ST_CombinedPlayerRegistrationPrePaidDepositFlow();
                    break;

            }
        }
        catch (Exception ex)
        {
            showError("Exception: " + hiddenService.Value + "." + dlWorkflows.SelectedValue + ", " + ex.Message);
        }
    }

    protected void btnSubmitToWrapperService_Click(object sender, EventArgs e)
    {
        hiddenService.Value = "Wrapper";
        hiddenPageAction.Value = "";
        divOtpControls.Visible = true;
        lbError.Text = "";
        try
        {
            switch (hiddenSelectedWorkFlow.Value)
            {
                case "Select":
                    return;

                case "RegisterPlayer":
                    hiddenFlow.Value = "Wrapper 1 stapiRegisterPlayerFlow";
                    stapiRegisterPlayerFlow();
                    break;

                case "PlayerLogin":
                    hiddenFlow.Value = "Wrapper 2 stapiPlayerLoginFlow";
                    stapiPlayerLoginFlow();
                    break;

                case "PreCheckSiteValidatePlayerRequest":
                    hiddenFlow.Value = "Wrapper 3 stapiPreCheckSiteValidatePlayerRequestFlow";
                    stapiPreCheckSiteValidatePlayerRequestFlow();
                    break;

                case "PreCheckSiteCardRequest":
                    hiddenFlow.Value = "Wrapper 4 stapiPreCheckSiteCardRequest Flow";
                    stapiPreCheckSiteCardRequestFlow();
                    break;

                case "DeleteRegisteredCard":
                    hiddenFlow.Value = "Wrapper 5 DeleteRegisteredCardFlow Flow";
                    DeleteRegisteredCardFlow();
                    break;
                    
                case "RegisterPrePaidAccount":
                    hiddenFlow.Value = "Wrapper 6 stapiRegisterPrePaidAccountFlow";
                    stapiRegisterPrePaidAccountFlow();
                    break;

                case "LoadPrePaidFunds":
                    hiddenFlow.Value = "Wrapper 7 stapiLoadPrePaidFundsFlow";
                    stapiLoadPrePaidFundsFlow();
                    break;

                case "SubmitPrePaidDeposit":
                    hiddenFlow.Value = "Wrapper 8 stapiSubmitPrePaidDepositFlow";
                    stapiSubmitPrePaidDepositFlow();
                    break;

                case "SubmitDepositAccountRequest":
                    hiddenFlow.Value = "Wrapper 9 stapiSubmitDepositAccountRequestFlow";
                    stapiSubmitDepositAccountRequestFlow();
                    break;

                case "SubmitPrePaidWithdrawal":
                    hiddenFlow.Value = "Wrapper 10 stapiSubmitPrePaidWithdrawalFlow";
                    stapiSubmitPrePaidWithdrawalFlow();
                    break;

                case "GetTransactionDetails":
                    hiddenFlow.Value = "Wrapper 11 stapiGetTransactionDetailsFlow";
                    stapiGetTransactionDetailsFlow();
                    break;

                case "UpdateTransaction":
                    hiddenFlow.Value = "Wrapper 12 stapiUpdateTransaction Flow";
                    stapiUpdateTransactionFlow();
                    break;

                case "GetRegisteredAccounts":
                    hiddenFlow.Value = "Wrapper 13 stapiGetRegisteredAccountsFlow";
                    stapiGetRegisteredAccountsFlow();
                    break;

                case "GetRegisteredCards":
                    hiddenFlow.Value = "Wrapper 14 stapiGetRegisteredCardsFlow";
                    stapiGetRegisteredCardsFlow();
                    break;

                case "GetPlayerSSN":
                    hiddenFlow.Value = "Wrapper 15 stapiGetPlayerSSNFlow";
                    stapiGetPlayerSSNFlow();
                    break;

                case "UpdatePlayerSSN":
                    hiddenFlow.Value = "Wrapper 16 stapiUpdatePlayerSSNFlow";
                    stapiUpdatePlayerSSNFlow();
                    break;

                case "GetPrePaidAccountHolderInfo":
                    hiddenFlow.Value = "Wrapper 17 stapiGetPrePaidAccountHolderInfoFlow";
                    stapiGetPrePaidAccountHolderInfoFlow();
                    break;

                case "GetPrePaidAccountBalance":
                    hiddenFlow.Value = "Wrapper 18 stapiGetPrePaidAccountBalanceFlow";
                    stapiGetPrePaidAccountBalanceFlow();
                    break;

                case "GetPrePaidAccountCVV2":
                    hiddenFlow.Value = "Wrapper 19 stapiGetPrePaidAccountCVV2Flow";
                    stapiGetPrePaidAccountCVV2Flow();
                    break;

                case "PreCheckSiteAccountRequest":
                    hiddenFlow.Value = "Wrapper 20 stapiPreCheckSiteAccountRequestFlow";
                    stapiPreCheckSiteAccountRequestFlow();
                    break;

                    case "PrePaidRegisterAndLoad":
                    hiddenFlow.Value = "Wrapper 21 stapiPrePaidRegisterAndLoadFlow";
                    stapiPrePaidRegisterAndLoadFlow();
                    break;

                case "UpdatePrePaidAccountStatus":
                    hiddenFlow.Value = "ST Direct 22 stapiUpdatePrePaidAccountStatusFlow";
                    stapiUpdatePrePaidAccountStatusFlow();
                    break;

                case "SubmitDepositRequest":
                    hiddenFlow.Value = "Wrapper 23 stapiSubmitDepositRequestFlow";
                    stapiSubmitDepositRequestFlow();
                    break;

                case "AccountWithdrawal":
                    hiddenFlow.Value = "Wrapper 24 stapiAccountWithdrawalFlow";
                    stapiAccountWithdrawalFlow();
                    break;

                case "SelfExcludePlayer":
                    hiddenFlow.Value = "Wrapper 25 stapiSelfExcludePlayerFlow";
                    stapiSelfExcludePlayerFlow();
                    break;

                case "ModifyPlayerRequest":
                    hiddenFlow.Value = "Wrapper 26 stapiModifyPlayerRequestFlow";
                    stapiModifyPlayerRequestFlow();
                    break;

                case "AuthDepositRequest":
                    hiddenFlow.Value = "Wrapper 27 stapiAuthDepositRequestFlow";
                    stapiAuthDepositRequestFlow();
                    break;

                case "SubmitRefund":
                    hiddenFlow.Value = "Wrapper 28 stapiSubmitRefundFlow";
                    stapiSubmitRefundFlow();
                    break;

                case "AddSelfExcludePlayer":
                    hiddenFlow.Value = "Wrapper 29 stapiAddSelfExcludePlayerFlow";
                    stapiAddSelfExcludePlayerFlow();
                    break;

                case "Withdrawal":
                    hiddenFlow.Value = "Wrapper 30 stapiWithdrawal";
                    stapiWithdrawalFlow();
                    break;

                case "SubmitReversal":
                    hiddenFlow.Value = "Wrapper 31 stapiSubmitReversal";
                    stapiSubmitReversalFlow();
                    break;

                case "stapiPreCheckSiteModifyAccountRequest":
                    hiddenFlow.Value = "Wrapper 32 stapiPreCheckSiteModifyAccountRequest";
                    stapiPreCheckSiteModifyAccountRequestFlow();
                    break;

                case "CombinedPlayerRegistrationPrePaidDeposit":
                    hiddenFlow.Value = "Wrapper - CombinedPlayerRegistrationPrePaidDepositFlow";
                    CombinedPlayerRegistrationPrePaidDepositFlow();
                    break;


                //case "GetPrePaidAccountStatus":
                //    hiddenFlow.Value = "Wrapper - stapiGetPrePaidAccountStatusFlow";
                //    stapiGetPrePaidAccountStatusFlow();
                //    break;

            }
        }
        catch (Exception ex)
        {
            showError("Exception: " + hiddenService.Value + "." + dlWorkflows.SelectedValue + ", " + ex.Message);
        }
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        // Reset UI
        ClearOperatorInfo();
        ClearPlayerInfo();

        rblt1.Checked = false;
        rblt2.Checked = false;
        rblt3.Checked = false;
        rblt4.Checked = false;
        rbst.Checked = true;

        divWorkflows.Visible = true;

        cbLogToFile.Checked = true;

        lbError.Text = string.Empty;
        divStatus.InnerHtml = String.Empty;
        divJsonResponseData.InnerHtml = String.Empty;
        divDeleteResult.InnerHtml = String.Empty;

        hiddenSelectedWorkFlow.Value = "Select";
        dlWorkflows.SelectedIndex = 0;
        dlPlayer.SelectedIndex = 0;
        dlPlayer.Visible = false;
        //divOperatorContainer.Visible = false;
        divPlayerContainer.Visible = false;
        divPlayerOperationsContainer.Visible = false;
    }

    protected void btnDeleteStateObjects_Click(object sender, EventArgs e)
    {
        MongoDBData myDatabase = new MongoDBData();
        // Delete all state objects in database
        var deleteResult = myDatabase.DeleteStateObjects();

        if (deleteResult.Contains("FAILED"))
            divDeleteResult.InnerHtml = "<span style='color: #ff0000;'>" + deleteResult + "</span>";
        else
            divDeleteResult.InnerHtml = "<span style='color: #068415;'>" + deleteResult + "</span>";
    }

    protected void btnClearLogFile_Click(object sender, EventArgs e)
    {
        var mLogfilename = "ST_OperLog-" + DateTime.Now.ToString("MMddyy");
        var mRoot = ConfigurationManager.AppSettings["LogFileRootPath"];
        if (mRoot == null) return;
        var path = Path.Combine(mRoot, mLogfilename + ".txt");
        File.Delete(path);
    }

    protected void btnValidateOtp_Click(object sender, EventArgs e)
    {
        //only called if wrapper being used
        var sbJsonInput = new StringBuilder();
        sbJsonInput.Append("{");
        sbJsonInput.Append("\"operatorId\":\"" + hiddenOperatorId.Value + "\",");
        sbJsonInput.Append("\"siteId\":\"" + hiddenSiteId.Value + "\",");
        sbJsonInput.Append("\"requestToken\":\"" + hiddenRequestToken.Value + "\",");
        sbJsonInput.Append("\"OTP\":\"" + txtOtp.Value + "\"");
        sbJsonInput.Append("}");
        // what service method needs to be called
        string servicemethod;
        if (hiddenFlow.Value.Contains("Login"))
            servicemethod = "stapiValidateAuthenticationOtp"; // player login
        else if (hiddenFlow.Value.Contains("Player"))
            servicemethod = "stapiValidateRegistrationOtp";  // player registration
        else
            servicemethod = "stapiValidateAuthorizationOtp"; // all funds movement flows

        // Call the correct OTP verification method in wrapped
        AddToLogAndDisplay(hiddenFlow.Value, servicemethod + " Request", sbJsonInput.ToString());
        //--- call the wrapper service
        var mResponseData = WrapperService(servicemethod, sbJsonInput.ToString());
        AddToLogAndDisplay(hiddenFlow.Value, servicemethod + " Response", mResponseData);

        dynamic mResponseJson = JObject.Parse(mResponseData);
        string returnMsg = mResponseJson.returnMsg.ToString();
        string returnCode = mResponseJson.returnCode.ToString();
        switch (returnCode)
        {
            case "INFO_PFO_00002": // Player login successful
                var myDatabase = new MongoDBData();
                var myPlayer = myDatabase.GetPlayerByUsername(txtPlayerUsername.Value.Trim());
                txtPlayerReferenceNumber.Value = myPlayer.stiReferenceNumber = txtPlayerReferenceNumber.Value = mResponseJson.validationDetails.stReferenceNo.ToString();
                txtPlayerConnectionToken.Value = myPlayer.connectionToken = mResponseJson.validationDetails.connectionToken.ToString();
                txtPlayerSessionToken.Value = myPlayer.sessionToken = mResponseJson.validationDetails.sessionToken.ToString();
                myDatabase.Save(myPlayer);
                returnCodeColor = "#068415";
                divOtpValidate.Visible = false;
                break;

            case "INFO_PFO_00021": // Please answer personal verification questions
                returnCodeColor = "#068415";
                ParseJsonQuestionToForm(mResponseData);
                break;

            case "INFO_PFO_00023":
                returnCodeColor = "#068415";
                txtOtp.Value = "";
                hiddenPageAction.Value = "";
                hiddenRequestToken.Value = "";

                //divOperatorContainer.Visible = false;
                divPlayerContainer.Visible = false;
                divPlayerOperationsContainer.Visible = true;
                divOtpControls.Visible = false;
                divOtpValidate.Visible = false;
                break;

            case "INFO_MAC_30021": // OTP Invalid, Please re-enter!
                returnCodeColor = "#ff0000";
                divPlayerContainer.Visible = false;
                divPlayerOperationsContainer.Visible = false;
                divOtpValidate.Visible = true;

                txtOtp.Value = "";
                txtOtp.Focus();
                break;

            case "ERR_MAC_13045":
            case "ERR_MAC_14045": // OTP expired
                returnCodeColor = "#ff0000";
                returnMsg += ", Please re-register!";
                txtOtp.Value = "";
                btnSubmitWCFService.Focus();

                hiddenPageAction.Value = "";
                hiddenRequestToken.Value = "";
                divPlayerContainer.Visible = false;
                divPlayerOperationsContainer.Visible = true;
                divOtpControls.Visible = false;
                divOtpValidate.Visible = false;
                break;
        }
        ShowCodeAndMsg(returnCode, returnMsg);
    }

    protected void btnResend_Click(object sender, EventArgs e)
    {
        //only called if wrapper being used
        var sbJsonInput = new StringBuilder();

        sbJsonInput.Append("{");
        sbJsonInput.Append("\"operatorId\":\"" + hiddenOperatorId.Value + "\",");
        sbJsonInput.Append("\"siteId\":\"" + hiddenSiteId.Value + "\",");
        sbJsonInput.Append("\"requestToken\":\"" + hiddenRequestToken.Value + "\"");
        sbJsonInput.Append("}");

        //--- call the wrapper service
        AddToLogAndDisplay(hiddenFlow.Value, "stapiResendOtp Request", sbJsonInput.ToString());
        //--- call the wrapper service
        var mResponseJson = WrapperService("stapiResendOtp", sbJsonInput.ToString());
        AddToLogAndDisplay(hiddenFlow.Value, "stapiResendOtp Response", mResponseJson);
        divStatus.InnerHtml = "<span style='color: #ff0000;'>" + mResponseJson + "</span>";
    }

    protected void btnSubmitAnswers_Click(object sender, EventArgs e)
    {
        var Question1AnswerId = Convert.ToInt16(hiddenQuestion1Answer.Value);
        var Question2AnswerId = Convert.ToInt16(hiddenQuestion2Answer.Value);
        var Question3AnswerId = Convert.ToInt16(hiddenQuestion3Answer.Value);
        var Question4AnswerId = Convert.ToInt16(hiddenQuestion4Answer.Value);

        var sbJsonSubmitAnswers = new StringBuilder();
        sbJsonSubmitAnswers.Append("{");
        sbJsonSubmitAnswers.Append("\"operatorId\":\"" + hiddenOperatorId.Value + "\",");
        sbJsonSubmitAnswers.Append("\"siteId\":\"" + hiddenSiteId.Value + "\",");
        sbJsonSubmitAnswers.Append("\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\",");
        sbJsonSubmitAnswers.Append("\"sitePwd\":\"" + txtSiteUserPassword.Value + "\",");
        sbJsonSubmitAnswers.Append("\"requestToken\":\"" + hiddenQuestionsResponseToken.Value + "\",");
        sbJsonSubmitAnswers.Append("\"answer\":");
        sbJsonSubmitAnswers.Append(" [");
        sbJsonSubmitAnswers.Append("{\"questionId\":1,\"answerId\":" + Question1AnswerId + "},");
        sbJsonSubmitAnswers.Append("{\"questionId\":2,\"answerId\":" + Question2AnswerId + "},");
        sbJsonSubmitAnswers.Append("{\"questionId\":3,\"answerId\":" + Question3AnswerId + "},");
        sbJsonSubmitAnswers.Append("{\"questionId\":4,\"answerId\":" + Question4AnswerId + "}");
        sbJsonSubmitAnswers.Append("]");
        sbJsonSubmitAnswers.Append("}");

        divQuestions.InnerHtml = String.Empty;
        divQuestionsContainer.Visible = false;
        var mRequestJson = sbJsonSubmitAnswers.ToString();
        string mResponseData;
        if (hiddenService.Value == "Wrapper")
        {
            AddToLogAndDisplay(hiddenFlow.Value, "stapiSubmitPlayerKBA Request", mRequestJson);
            //----- Call Wrapper -----
            mResponseData = WrapperService("stapiSubmitPlayerKBA", mRequestJson);
            AddToLogAndDisplay(hiddenFlow.Value, "stapiSubmitPlayerKBA Response", mResponseData);
        }
        else
        {
            AddToLogAndDisplay(hiddenFlow.Value, "stapiSubmitPlayerKBA Request", mRequestJson);
            //----- Call ST Direct -----
            var stiOperations = new StiOperations();
            mResponseData = stiOperations.stapiSubmitPlayerKBA(mRequestJson);
            AddToLogAndDisplay(hiddenFlow.Value, "stapiSubmitPlayerKBA Response", mResponseData);
        }
        dynamic mAnswerResponseJson = JObject.Parse(mResponseData);
        string returnMsg = mAnswerResponseJson.returnMsg.ToString();
        string returnCode = mAnswerResponseJson.returnCode.ToString();
        switch (returnCode)
        {
            case "INFO_PFO_00023": // Validation and registration successfull
                returnCodeColor = "#068415";
                txtOtp.Value = "";
                hiddenPageAction.Value = "";
                hiddenRequestToken.Value = "";
                divPlayerContainer.Visible = false;
                divPlayerOperationsContainer.Visible = true;
                divOtpControls.Visible = false;
                break;
            default:
                returnCodeColor = "#ff0000";
                break;
        }
        ShowCodeAndMsg(returnCode, returnMsg);
    }

    protected void btnUpdatePlayerFromForm_Click(object sender, EventArgs e)
    {
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayerByFirstNameLastName(txtPlayerFirstName.Value, txtPlayerLastName.Value);
        if (myPlayer == null)
        {
            showError("No player in db with name " + txtPlayerFirstName.Value + " " + txtPlayerLastName.Value);
            return;
        }
        UpdatePlayerFromForm(myPlayer);
    }

    protected void btnUpdatePlayerUserName_Click(object sender, EventArgs e)
    {
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

        // Increment this in the Player form and Player document for next registration request for selected user.
        txtPlayerUsername.Value = myPlayer.userName = RandomizeUserName(myPlayer.firstName);
        txtPlayerSessionToken.Value = myPlayer.sessionToken = string.Empty;
        txtPlayerConnectionToken.Value = myPlayer.connectionToken = string.Empty;
        txtPlayerPlayerId.Value = myPlayer.stiPlayerId = string.Empty;
        txtPlayerReferenceNumber.Value = myPlayer.stiReferenceNumber = string.Empty;
        txtPCICardToken.Value = myPlayer.PCICardToken = string.Empty;
        myDatabase.Save(myPlayer);
    }

    private void UpdatePlayerFromForm(StiPlayer myPlayer)
    {
        myPlayer.stiPlayerId = txtPlayerPlayerId.Value;
        myPlayer.userName = txtPlayerUsername.Value;
        myPlayer.firstName = txtPlayerFirstName.Value;
        myPlayer.middleInitial = txtPlayerMiddleInitial.Value;
        myPlayer.lastName = txtPlayerLastName.Value;
        myPlayer.gender = txtPlayerGender.Value;
        myPlayer.dob = txtPlayerDoB.Value;
        myPlayer.emailAddress = txtPlayerEmail.Value = myPlayer.emailAddress;
        myPlayer.playerAddress1 = txtPlayerAddress1.Value;
        myPlayer.playerAddress2 = txtPlayerAddress2.Value;
        myPlayer.city = txtPlayerCity.Value;
        myPlayer.county = txtPlayerCounty.Value;
        myPlayer.state = txtPlayerState.Value;
        myPlayer.zipCode = txtPlayerZip.Value;
        myPlayer.country = txtPlayerCountry.Value;
        myPlayer.mobileNo = txtPlayerMobilePhone.Value;
        myPlayer.landLineNo = txtPlayerHomePhone.Value;
        myPlayer.ssn = txtPlayerSSN.Value;
        myPlayer.dlNumber = txtPlayerDriversLicenseNumber.Value;
        myPlayer.dlIssuingState = txtPlayerDriversLicenseIssueState.Value;

        // divPlayerCardInfo
        myPlayer.PCICardNumber = txtPCICardNumber.Value = myPlayer.PCICardNumber;
        myPlayer.PCIAccountType = txtPCIAccountType.Value;
        myPlayer.PCINameOnCard = txtPCINameOnCard.Value;
        myPlayer.PCICVV = txtPCICVV.Value;
        myPlayer.PCIStartDate = txtPCIStartDate.Value;
        myPlayer.PCIExpiryDate = txtPCIExpiryDate.Value;
        myPlayer.PCIIssueNumber = txtPCIIssueNumber.Value;
        myPlayer.PCIDefaultCard = txtPCIDefaultCard.Value;
        myPlayer.PCIAddress1 = txtPCIAddress1.Value;
        myPlayer.PCIAddress2 = txtPCIAddress2.Value;
        myPlayer.PCICity = txtPCICity.Value;
        myPlayer.PCICountry = txtPCICounty.Value;
        myPlayer.PCIState = txtPCIState.Value;
        myPlayer.PCIZipCode = txtPCIZipCode.Value;
        myPlayer.PCICountry = txtPCICountry.Value;

        //PrePaid Account Info in divPlayerPrePaidAccountInfo
        myPlayer.PPANumber = txtPPANumber.Value;
        myPlayer.PPANumber = txtPPAVirtualCardNumber.Value;
        myPlayer.PPATandCAccepted = txtPPAtandcAccepted.Value;
        myPlayer.PPATandCAcceptedTimestampUTC = txtPPAtandcAcceptedTimestampUTC.Value;
        myPlayer.PPABalance = txtPPABalance.Value;
        
        var myDatabase = new MongoDBData();
        myDatabase.Save(myPlayer);
    }

    protected void btnUpdateOperatorSettings_Click(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(hiddenOperatorId.Value)) return;
        if (hiddenOperatorId.Value == "0") return;

        if (String.IsNullOrEmpty(hiddenSiteId.Value)) return;
        if (hiddenSiteId.Value == "0") return;

        var myDatabase = new MongoDBData();
        var mStiOperatorSite = myDatabase.GetStiOperatorSiteUsingOperatorIdAndSiteId(hiddenOperatorId.Value,
            hiddenSiteId.Value);
        if (mStiOperatorSite == null) return;

        var changed = false;
        if (mStiOperatorSite.AuthorizedIp != txtAuthorizedIP.Value)
        {
            mStiOperatorSite.AuthorizedIp = txtAuthorizedIP.Value;
            changed = true;
        }
        if (mStiOperatorSite.SiteUserName != txtSiteUsername.Value)
        {
            mStiOperatorSite.SiteUserName = txtSiteUsername.Value;
            changed = true;
        }
        if (mStiOperatorSite.SitePassword != txtSiteUserPassword.Value)
        {
            mStiOperatorSite.SitePassword = txtSiteUserPassword.Value;
            changed = true;
        }
        if (mStiOperatorSite.GeoInfo != txtGeoInfo.Value)
        {
            mStiOperatorSite.GeoInfo = txtGeoInfo.Value;
            changed = true;
        }
        if (changed)
        {
            myDatabase.UpdateOperatorSite(mStiOperatorSite);
        }
        CreateSitesList();
    }

    protected void btnUpdateCard_Click(object sender, EventArgs e)
    {
        var myDatabase = new MongoDBData();
        var myPlayer = myDatabase.GetPlayerByUsername(txtPlayerUsername.Value);
        if (myPlayer == null)
        {
            showError("Could not get player using userName:" + txtPlayerUsername.Value);
            return;
        }
        if (txtPCICardNumber.Value == myPlayer.PCICardNumber)
        {
            showError("You must change the card number!");
            return;
        }
        myPlayer.PCICardNumber = txtPCICardNumber.Value;
        myPlayer.PCICardType = txtPCICardType.Value;
        myDatabase.Save(myPlayer);
    }

    #endregion

    #region ddl Methods

    private void CreateSitesList()
    {
        dlOperatorSites.Items.Clear();

        // Create root list item
        var defaultItem = new ListItem();
        defaultItem.Text = "Select an Operator Site";
        defaultItem.Value = defaultItem.Text;

        dlOperatorSites.Items.Add(defaultItem);

        // Get sites from db
        var myDatabase = new MongoDBData();
        var mySites = myDatabase.GetOperatorSites();

        foreach (var currentSite in mySites)
        {
            var reply = MAC_ClientService(currentSite.OperatorId, currentSite.SiteId);
            // Only process and add list item if siteid is assigned to a valid client
            if (reply.Item1)
            {
                var siteItem = new ListItem();

                var delimitedAttrbutes = "";
                delimitedAttrbutes += "operatorId:" + currentSite.OperatorId + "|";
                delimitedAttrbutes += "siteId:" + currentSite.SiteId + "|";
                delimitedAttrbutes += "siteUserName:" + currentSite.SiteUserName + "|";
                delimitedAttrbutes += "sitePassword:" + currentSite.SitePassword + "|";
                delimitedAttrbutes += "geoInfo:" + currentSite.GeoInfo + "|";
                delimitedAttrbutes += "authorizedIp:" + currentSite.AuthorizedIp + "|";
                delimitedAttrbutes += "clientId:" + reply.Item2 + "|";
                delimitedAttrbutes += "clientName:" + reply.Item3;

                siteItem.Text = currentSite.SiteId + " - " + reply.Item3;
                siteItem.Value = delimitedAttrbutes;

                dlOperatorSites.Items.Add(siteItem);
            }
        }
    }

    protected void dlWorkflows_SelectedIndexChanged(object sender, EventArgs e)
    {
        ClearPlayerInfo();
        divActionButtons.Visible = false;
        var value = dlWorkflows.SelectedValue;
        if (value.StartsWith("--"))
        {
            dlPlayer.SelectedIndex = 0;
            dlPlayer.Visible = false;
            return;
        }

        //dlPlayer.SelectedIndex = 0;

        if (dlWorkflows.SelectedIndex > 0)
            dlPlayer.Visible = true;
        else
            dlPlayer.Visible = false;

        hiddenSelectedWorkFlow.Value = value;

        ShowAll();
    }

    private void CreatePlayersList()
    {
        dlPlayer.Items.Clear();

        // Create root list item
        var defaultItem = new ListItem();
        defaultItem.Text = "Select a Player";
        defaultItem.Value = defaultItem.Text;

        dlPlayer.Items.Add(defaultItem);
        var myDatabase = new MongoDBData();
        var playerCollection = myDatabase.GetAllPlayers();
        if (playerCollection.Any())
        {
            foreach (var currentPlayer in playerCollection)
            {
                var userItem = new ListItem();
                userItem.Text = currentPlayer.firstName + " " + currentPlayer.lastName;
                userItem.Value = currentPlayer._id.ToString();
                userItem.Attributes.Add("stPlayerId", currentPlayer.stiPlayerId);

                dlPlayer.Items.Add(userItem);
            }
        }
        else
        {
            if (playerCollection.Count == 0)
            {
                // Change database to central db
                mongoDB = myMongoUtil.ConfigDB("SecureTrading");

                var query = Query.EQ("_t", "StiPlayer");
                MongoCollection stCollection = myMongoUtil.GetSTCollection();

                var SeedPlayerCollection = stCollection.FindAs<StiPlayer>(query).ToList();

                // Switch back to user's database
                mongoDB = myMongoUtil.ConfigDB("");

                // Copy players from there to the user's database
                foreach (StiPlayer currentPlayer in SeedPlayerCollection)
                {
                    myMongoUtil.Save(currentPlayer);

                    var userItem = new ListItem();
                    userItem.Text = currentPlayer.firstName + " " + currentPlayer.lastName;
                    userItem.Value = currentPlayer._id.ToString();

                    userItem.Attributes.Add("stPlayerId", currentPlayer.stiPlayerId);

                    dlPlayer.Items.Add(userItem);
                }
            }
        }
    }

    protected void dlPlayer_SelectedIndexChanged(object sender, EventArgs e)
    {
        var mOperation = dlWorkflows.SelectedItem.Text;
        //ClearPlayerInfo();
        if (mOperation == "Select an Operation")
        {
            divPlayerContainer.Visible = false;
            divActionButtons.Visible = false;
            return;
        }

        if (dlPlayer.SelectedIndex > 0)
        {
            divActionButtons.Visible = true;
            if (mOperation == "ST-1 Player Registration")
                divPlayerContainer.Visible = true;
            //else if (mOperation == "ST-1 Card Registration")
                //divPlayerCardInfo.Visible = true;
            //else if (mOperation == "ST-1 Pre Paid Card Registration")
            //    divPlayerPrePaidAccountInfo.Visible = true;

            // Load player info from database
            var myDatabase = new MongoDBData();
            var myPlayer = myDatabase.GetPlayer(dlPlayer.SelectedValue.Trim());

            txtPlayerPlayerId.Value = String.IsNullOrEmpty(myPlayer.stiPlayerId) ? Cst.Strings.DefaultSTPlayerId : myPlayer.stiPlayerId;

            txtPlayerUsername.Value = myPlayer.userName;
            txtPlayerFirstName.Value = myPlayer.firstName;
            txtPlayerMiddleInitial.Value = myPlayer.middleInitial;
            txtPlayerLastName.Value = myPlayer.lastName;
            txtPlayerGender.Value = myPlayer.gender;
            txtPlayerDoB.Value = myPlayer.dob;
            txtPlayerEmail.Value = myPlayer.emailAddress;
            txtPlayerAddress1.Value = myPlayer.playerAddress1;
            txtPlayerAddress2.Value = myPlayer.playerAddress2;
            txtPlayerCity.Value = myPlayer.city;
            txtPlayerCounty.Value = myPlayer.county;
            txtPlayerState.Value = myPlayer.state;
            txtPlayerZip.Value = myPlayer.zipCode;
            txtPlayerCountry.Value = myPlayer.country;
            txtPlayerMobilePhone.Value = myPlayer.mobileNo;
            txtPlayerHomePhone.Value = myPlayer.landLineNo;
            txtPlayerSSN.Value = myPlayer.ssn;
            txtPlayerDriversLicenseNumber.Value = myPlayer.dlNumber;
            txtPlayerDriversLicenseIssueState.Value = myPlayer.dlIssuingState;

            // divPlayerCardInfo
            txtPCICardToken.Value = myPlayer.PCICardToken;
            txtPCICardType.Value = myPlayer.PCICardType;
            txtPCICardNumber.Value = myPlayer.PCICardNumber;
            txtPCIAccountType.Value = String.IsNullOrEmpty(myPlayer.PCIAccountType) ? "ECOM" : myPlayer.PCIAccountType;
            txtPCINameOnCard.Value = String.IsNullOrEmpty(myPlayer.PCINameOnCard) ? myPlayer.userName + " " + myPlayer.lastName : myPlayer.PCINameOnCard;
            txtPCICVV.Value = String.IsNullOrEmpty(myPlayer.PCICVV) ? "123" : myPlayer.PCICVV;
            txtPCIStartDate.Value = String.IsNullOrEmpty(myPlayer.PCIStartDate) ? "10/2014" : myPlayer.PCIStartDate;
            txtPCIExpiryDate.Value = String.IsNullOrEmpty(myPlayer.PCIExpiryDate) ? "10/2016" : myPlayer.PCIExpiryDate;
            txtPCIIssueNumber.Value = String.IsNullOrEmpty(myPlayer.PCIIssueNumber) ? "21" : myPlayer.PCIIssueNumber;
            txtPCIDefaultCard.Value = String.IsNullOrEmpty(myPlayer.PCIDefaultCard) ? "Y" : myPlayer.PCIDefaultCard;
            txtPCIAddress1.Value = String.IsNullOrEmpty(myPlayer.PCIAddress1) ? myPlayer.playerAddress1 : myPlayer.PCIAddress1;
            txtPCIAddress2.Value = String.IsNullOrEmpty(myPlayer.PCIAddress2) ? myPlayer.playerAddress2 : myPlayer.PCIAddress2;
            txtPCICity.Value = String.IsNullOrEmpty(myPlayer.PCICity) ? myPlayer.city : myPlayer.PCICity;
            txtPCICounty.Value = String.IsNullOrEmpty(myPlayer.PCICountry) ? myPlayer.country : myPlayer.PCICountry;
            txtPCIState.Value = String.IsNullOrEmpty(myPlayer.PCIState) ? myPlayer.state : myPlayer.PCIState;
            txtPCIZipCode.Value = String.IsNullOrEmpty(myPlayer.PCIZipCode) ? myPlayer.zipCode : myPlayer.PCIZipCode;
            txtPCICountry.Value = String.IsNullOrEmpty(myPlayer.PCICountry) ? myPlayer.country : myPlayer.PCICountry;

            //PrePaid Account Info in divPlayerPrePaidAccountInfo
            txtPPANumber.Value = myPlayer.PPANumber;
            txtPPAVirtualCardNumber.Value = myPlayer.PPANumber;
            txtPPAtandcAccepted.Value = myPlayer.PPATandCAccepted;
            txtPPAtandcAcceptedTimestampUTC.Value = myPlayer.PPATandCAcceptedTimestampUTC;
            txtPPABalance.Value = myPlayer.PPABalance;

            // show divs
            divPlayerContainer.Visible = true;
            divActionButtons.Visible = true;
        }
        else
        {
            divPlayerContainer.Visible = false;
            divActionButtons.Visible = false;
        }

        ShowAll();
    }

    #endregion

    #region Service Calls
    private string WrapperService(string pMethod, string pData)
    {
        if (string.IsNullOrEmpty(pData))
        {
            showError("WrapperService, null data");
            return null;
        }
        
        var mUrl = ConfigurationManager.AppSettings[Cst.Config.StWrapperServiceUrl] + pMethod;
        // no newlines it makes the WCF unhappy
        var mData = pData.Replace(Environment.NewLine, "");
        var dataStream = Encoding.UTF8.GetBytes(mData);
        try
        {
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(mUrl);
            myHttpWebRequest.Method = "POST";

            // Set the content type of the data being posted.
            myHttpWebRequest.ContentType = "application/json;charset=utf-8";
            //myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.ContentLength = dataStream.Length;
            var newStream = myHttpWebRequest.GetRequestStream();
            newStream.Write(dataStream, 0, dataStream.Length);
            newStream.Close();

            using (var response = (HttpWebResponse)myHttpWebRequest.GetResponse())
            {
                var header = response.GetResponseStream();
                if (header == null) throw new Exception("stapiRequest returned null header!");
                //var encode = Encoding.GetEncoding("utf-8");
                var readStream = new StreamReader(header);
                string mResponseData1 = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                return mResponseData1;
            }
        }
        catch (Exception ex)
        {
            showError(mUrl + ".." + ex.Message);
            return "{"
                + "\"Exception\":\"" + ex.Message + ""
                + "}";
        }

    }
    private Tuple<bool, string, string> MAC_ClientService(string pOperatorId, string pSiteId)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(
                ConfigurationManager.AppSettings[Cst.Config.MacServicesUrl]
                + "/AdminServices/ClientServices.asmx/WsGetMACClientIdBySTSiteId");

            var postData = "data=" + pOperatorId + "|" + pSiteId;
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            // ReSharper disable once AssignNullToNotNullAttribute
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd().ToString();
            var myDoc = new XmlDocument();
            myDoc.LoadXml(responseString);
            var clientId = "";
            var clientName = "";
            var hasClientInfo = false;
            if (myDoc.GetElementsByTagName("clientid")[0] != null)
            {
                clientId = myDoc.GetElementsByTagName("clientid")[0].InnerText;
                hasClientInfo = true;
            }
            if (myDoc.GetElementsByTagName("clientname")[0] != null)
            {
                clientName = myDoc.GetElementsByTagName("clientname")[0].InnerText;
            }
            if (hasClientInfo)
                return new Tuple<bool, string, string>(true, clientId, clientName);
            return new Tuple<bool, string, string>(false, string.Empty, string.Empty);
        }
        catch (Exception ex)
        {
            AddToLogAndDisplay("MAC_GetMACClientWithSTSiteIds", "Exception", ex.Message);
            return new Tuple<bool, string, string>(false, string.Empty, ex.Message);
        }
    }
    private XmlDocument MAC_GetMACClientWithSTSiteIds()
    {
        var myDoc = new XmlDocument();
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(
                ConfigurationManager.AppSettings[Cst.Config.MacServicesUrl]
                + "/AdminServices/ClientServices.asmx/WsGetMACClientWithSTSiteIds");
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            var response = (HttpWebResponse)request.GetResponse();
            // ReSharper disable once AssignNullToNotNullAttribute
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd().ToString();
            myDoc.LoadXml(responseString);
        }
        catch (Exception ex)
        {
            AddToLogAndDisplay("MAC_GetMACClientWithSTSiteIds", "Exception", ex.Message);
        }
        return myDoc;
    }
    #endregion

    #region Json Builders

    private string Construct_SubmitDepositAccountRequest_RequestJson(StiPlayer pPlayer)
    {
      var mJson =
            "{"
                + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
                + "\"siteId\":\"" + hiddenSiteId.Value + "\","
                + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
                + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
                + "\"geoComplyEncryptedPacket\":\"" + txtGeoInfo.Value + "\","
                + "\"depositDetails\":"
                + "{"
                    + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                    + "\"connectionToken\":\"" + txtPlayerConnectionToken.Value + "\","
                    + "\"sessionToken\":\"" + txtPlayerSessionToken.Value + "\","
                    + "\"transactionType\":\"DEPOSIT\","
                    + "\"orderReference\":\"1234\","
                    + "\"accountType\":\"ACH\","
                    + "\"currency\":\"" + "USD" + "\","                        //todo add to player object and PCI form
                    + "\"amount\":\"" + "1000" + "\","
                    + "\"atmVerify\":\"" + "Y" + "\","
                    + "\"accountDetails\":"
                    + "{"
                        + "\"bankName\":\"" + "Y" + "\","
                        + "\"bankAccountType\":\"" + "Y" + "\","
                        + "\"accountToken\":\"" + "null" + "\","
                        + "\"abaNumber\":\"" + "111111111" + "\""
                    + "}"
                + "}"
            + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
      return mJson;
    }

    private string Construct_SubmitPrePaidDeposit_RequestJson()
    {
        var mJson =
            "{"
                + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
                + "\"siteId\":\"" + hiddenSiteId.Value + "\","
                + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
                + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
                + "\"geoComplyEncryptedPacket\":\"" + txtGeoInfo.Value + "\","
                + "\"depositDetails\":"
                + "{"
                    + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                    + "\"connectionToken\":\"" + txtPlayerConnectionToken.Value + "\","
                    + "\"sessionToken\":\"" + txtPlayerSessionToken.Value + "\","
                    + "\"transactionType\":\"DEPOSIT\","
                    + "\"orderReference\":\"1234\","
                    + "\"accountType\":\"" + txtPCIAccountType.Value + "\","
                    + "\"prePaidAccountToken\":\"" + "1234" + "\","             //todo add to player object and PCI form
                    + "\"currency\":\"" + "USD" + "\","                        //todo add to player object and PCI form
                    + "\"amount\":\"" + "1000" + "\"" 
                + "}"
            + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_SubmitPrePaidWithdrawal_RequestJson(StiPlayer pPlayer)
    {
        var mJson =            
            "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"withdrawalDetails\":"
            + "{"
            +   "\"userName\":\"" + txtPlayerUsername.Value + "\","
            +   "\"transactionType\":\"WITHDRAWAL\","
            +   "\"orderReference\":\"1234\","                        //todo random
            +   "\"prePaidAccountToken\":\"1234\","                   //todo add to player object and PCI form
            +   "\"accountType\":\"" + txtPCIAccountType.Value + "\","
            +   "\"currency\":\"" + "USD" + "\","                     //todo add to player object and PCI form
            +   "\"amount\":\"" + "1000" + "\""                     //todo add to  PCI form
            + "}"
            + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_LoadFundstoPrePaid_RequestJson()
    {
        var mJson =
            "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"geoComplyEncryptedPacket\":\"" + txtGeoInfo.Value + "\","
            + "\"depositDetails\":"
            + "{"
            + "\"userName\":\"" + txtPlayerUsername.Value + "\","
            + "\"connectionToken\":\"" + txtPlayerConnectionToken.Value + "\","
            + "\"sessionToken\":\"" + txtPlayerSessionToken.Value + "\","
            + "\"orderReference\":\"1234\","
            + "\"accountType\":\"" + txtPCIAccountType.Value + "\","
            + "\"currency\":\"" + "USD" + "\","                       //todo add to player object and PCI form
            + "\"transactionType\":\"DEPOSIT\","
            + "\"threeDFlag\":\"FALSE\","                             //todo add to player object and PCI form
            + "\"amount\":\"" + "1000" + "\","                        //todo add to  PCI form
            + "\"cardDetails\":"
            + "{"
            + "\"nameOnCard\":\"" + txtPCINameOnCard.Value + "\","
            + "\"cardType\":\"" + txtPCICardType.Value + "\","
            + "\"cardNumber\":\"" + txtPCICardNumber.Value + "\","
            + "\"cardToken\":\"null\","
            + "\"startDate\":\"" + txtPCIStartDate.Value + "\","
            + "\"expiryDate\":\"" + txtPCIExpiryDate.Value + "\","
            + "\"issueNumber\":\"" + txtPCIIssueNumber.Value + "\","
            + "\"defaultCard\":\"" + txtPCIDefaultCard.Value + "\","
            + "\"cvv\":\"" + txtPCICVV.Value + "\","
            + "\"cardBillingInfo\":"
            + "{"
            + "\"playerAddress1\":\"" + txtPCIAddress1.Value + "\","
            + "\"playerAddress2\":\"" + txtPCIAddress2.Value + "\","
            + "\"city\":\"" + txtPCICity.Value + "\","
            + "\"county\":\"" + txtPCICounty.Value + "\","
            + "\"state\":\"" + txtPCIState.Value + "\","
            + "\"zipCode\":\"" + txtPCIZipCode.Value + "\","
            + "\"country\":\"" + txtPCICountry.Value + "\""
        + "}"
        + "}"
        + "}"
        + "}";

        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_PreCheckSiteModifyCardRequest_Json(String pDefaultCardFlag, String pDeleteCardFlag)
    {
        var mDefaultCardFlag = "Y";
        if (pDefaultCardFlag != "Y")
            mDefaultCardFlag = "N";

        var mDeleteCardFlag = "N";
        if (pDeleteCardFlag != "N")
            mDeleteCardFlag = "Y";


        // check flow to determain how to construct the json
        var mJson =
            "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"userName\":\"" + txtPlayerUsername.Value + "\","
            + "\"accountType\":\"" + txtPCIAccountType.Value + "\","
            + "\"connectionToken\":\"" + txtPlayerConnectionToken.Value + "\","
            + "\"sessionToken\":\"" + txtPlayerSessionToken.Value + "\","
            + "\"cardDetails\":"
            + "{"
            + "\"nameOnCard\":\"" + txtPCINameOnCard.Value + "\","
            + "\"cardType\":\"" + txtPCICardType.Value + "\","
            + "\"cardNumber\":\"" + txtPCICardNumber.Value + "\","
            + "\"cardToken\":\"null\","
            + "\"startDate\":\"" + txtPCIStartDate.Value + "\","
            + "\"expiryDate\":\"" + txtPCIExpiryDate.Value + "\","
            + "\"issueNumber\":\"" + txtPCIIssueNumber.Value + "\","
            + "\"defaultCard\":\"" + mDefaultCardFlag + "\","
            + "\"deleteFlag\":\"" + mDeleteCardFlag + "\","
            + "\"cvv\":\"" + txtPCICVV.Value + "\","
                + "\"cardBillingInfo\":"
                + "{"
                + "\"playerAddress1\":\"" + txtPCIAddress1.Value + "\","
                + "\"playerAddress2\":\"" + txtPCIAddress2.Value + "\","
                + "\"city\":\"" + txtPCICity.Value + "\","
                + "\"county\":\"" + txtPCICounty.Value + "\","
                + "\"state\":\"" + txtPCIState.Value + "\","
                + "\"zipCode\":\"" + txtPCIZipCode.Value + "\","
                + "\"country\":\"" + txtPCICountry.Value + "\""
            + "}}}";

        try
        {
            dynamic text = JObject.Parse(mJson);
            lbError.Text = string.Empty;
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_OperatorInfoRegister_RequestJson()
    {
        var sbJsonObject = new StringBuilder();
        sbJsonObject.Append("\"operatorId\":\"" + hiddenOperatorId.Value + "\",");
        sbJsonObject.Append("\"siteId\":\"" + hiddenSiteId.Value + "\",");
        sbJsonObject.Append("\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\",");
        sbJsonObject.Append("\"sitePwd\":\"" + txtSiteUserPassword.Value + "\",");
        sbJsonObject.Append("\"geoComplyEncryptedPacket\":\"" + txtGeoInfo.Value + "\"");
        return sbJsonObject.ToString();
    }

    private string Construct_OperatorInfo_RequestJson()
    {
        var sbJsonObject = new StringBuilder();
        sbJsonObject.Append("{");
        sbJsonObject.Append("\"operatorId\":\"" + hiddenOperatorId.Value + "\",");
        sbJsonObject.Append("\"siteId\":\"" + hiddenSiteId.Value + "\",");
        sbJsonObject.Append("\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\",");
        sbJsonObject.Append("\"sitePwd\":\"" + txtSiteUserPassword.Value + "\",");
        sbJsonObject.Append("\"userName\":\"" + txtPlayerUsername.Value + "\",");
        sbJsonObject.Append("\"ipAddress\":\"" + txtAuthorizedIP.Value + "\",");
        sbJsonObject.Append("\"geoComplyEncryptedPacket\":\"" + txtGeoInfo.Value + "\",");
        sbJsonObject.Append("\"deviceFingerPrint\":\" \"");
        sbJsonObject.Append("}");
        return sbJsonObject.ToString();
    }

    private string Construct_UpdateTransaction_RequestJson(StiPlayer pPlayer)
    {
        /*
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"123456789545", 
            "sitePwd":")*(&^%$#", 
            "stTransactionNo":"1234567890", 
            "orderReference":"45587", 
            "settledDate":"2000-02-19", 
            "status":"Awaiting Settlement", 
            "flag":"update" 
        } 
        */
        var sbJsonObject = new StringBuilder();
        sbJsonObject.Append("{");
        sbJsonObject.Append("\"operatorId\":\"" + hiddenOperatorId.Value + "\",");
        sbJsonObject.Append("\"siteId\":\"" + hiddenSiteId.Value + "\",");
        sbJsonObject.Append("\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\",");
        sbJsonObject.Append("\"sitePwd\":\"" + txtSiteUserPassword.Value + "\",");
        sbJsonObject.Append("\"stTransactionNo\":\"" + "1234567890" + "\",");
        sbJsonObject.Append("\"orderReference\":\"" + "45587" + "\",");
        sbJsonObject.Append("\"settledDate\":\"" + "2000-02-19" + "\",");
        sbJsonObject.Append("\"status\":\"" + "Awaiting Settlement" + "\",");
        sbJsonObject.Append("\"flag\":\"" + "update" + "\"");
        sbJsonObject.Append("}");
       
        try
        {
            dynamic text = JObject.Parse(sbJsonObject.ToString());
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return sbJsonObject.ToString();
    }

    private string Construct_RegisterPlayer_RequestJson()
    {
        var mJson = "{" + Construct_OperatorInfoRegister_RequestJson() + ", " + Construct_GetPlayerInfo_RequestJson() + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }
    
    private string Construct_GetPlayerInfo_RequestJson()
    {
        var sbJsonObject = new StringBuilder();

        sbJsonObject.Append("\"playerDetails\":");
        sbJsonObject.Append("{");
        sbJsonObject.Append("\"userName\":\"" + txtPlayerUsername.Value + "\",");
        sbJsonObject.Append("\"firstName\":\"" + txtPlayerFirstName.Value + "\",");
        if (!string.IsNullOrEmpty(txtPlayerMiddleInitial.Value))
            sbJsonObject.Append("\"middleInitial\":\"" + txtPlayerMiddleInitial.Value + "\",");
        sbJsonObject.Append("\"lastName\":\"" + txtPlayerLastName.Value + "\",");
        sbJsonObject.Append("\"gender\":\"" + txtPlayerGender.Value + "\",");
        sbJsonObject.Append("\"dob\":\"" + txtPlayerDoB.Value + "\",");
        sbJsonObject.Append("\"emailAddress\":\"" + txtPlayerEmail.Value + "\",");
        sbJsonObject.Append("\"playerAddress1\":\"" + txtPlayerAddress1.Value + "\",");
        if (!string.IsNullOrEmpty(txtPlayerAddress2.Value))
            sbJsonObject.Append("\"playerAddress2\":\"" + txtPlayerAddress2.Value + "\",");
        sbJsonObject.Append("\"city\":\"" + txtPlayerCity.Value + "\",");
        if (!string.IsNullOrEmpty(txtPlayerCounty.Value))
            sbJsonObject.Append("\"county\":\"" + txtPlayerCounty.Value + "\",");
        sbJsonObject.Append("\"state\":\"" + txtPlayerState.Value + "\",");
        sbJsonObject.Append("\"zipCode\":\"" + txtPlayerZip.Value + "\",");
        sbJsonObject.Append("\"country\":\"" + txtPlayerCountry.Value + "\",");
        sbJsonObject.Append("\"mobileNo\":\"" + txtPlayerMobilePhone.Value + "\",");
        sbJsonObject.Append("\"landLineNo\":\"" + txtPlayerHomePhone.Value + "\",");

        sbJsonObject.Append("\"ssn\":\"" + txtPlayerSSN.Value + "\",");
        sbJsonObject.Append("\"dlNumber\":\"" + txtPlayerDriversLicenseNumber.Value + "\",");
        sbJsonObject.Append("\"dlIssuingState\":\"" + txtPlayerDriversLicenseIssueState.Value + "\",");
        sbJsonObject.Append("\"ipAddress\":\"" + txtAuthorizedIP.Value + "\"");
        sbJsonObject.Append("}");
        return sbJsonObject.ToString();
    }

    private string Construct_RegisterPrePaidAccount_RequestJson(StiPlayer pPlayer)
    {
        string mtandcAcceptedTimestampUTC = pPlayer.PPATandCAccepted;
        if (String.IsNullOrEmpty(mtandcAcceptedTimestampUTC))
        {
            mtandcAcceptedTimestampUTC = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            mtandcAcceptedTimestampUTC = mtandcAcceptedTimestampUTC + "+00:00";
        }

        var mJson =
        "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"userName\":\"" + pPlayer.userName + "\","
            + "\"connectionToken\":\"" + pPlayer.connectionToken + "\","
            + "\"sessionToken\":\"" + pPlayer.sessionToken + "\","
            + "\"tandcAccepted\":\"Y\","
            + "\"tandcAcceptedTimestampUTC\":\"" + mtandcAcceptedTimestampUTC + "\""
        + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_PreCheckSiteCardRequest_RequestJson(StiPlayer pPlayer)
    {
        var mJson =
           "{"
               + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
               + "\"siteId\":\"" + hiddenSiteId.Value + "\","
               + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
               + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
               + "\"userName\":\"" + pPlayer.userName + "\","
               + "\"accountType\":\"" + txtPCIAccountType.Value + "\","
               + "\"connectionToken\":\"" + pPlayer.connectionToken + "\","
               + "\"sessionToken\":\"" + pPlayer.sessionToken + "\","
               + "\"cardDetails\":"
               + "{"
                   + "\"nameOnCard\":\"" + txtPCINameOnCard.Value + "\","
                   + "\"cardType\":\"" + txtPCICardType.Value + "\","
                   + "\"cardNumber\":\"" + txtPCICardNumber.Value + "\","
                   + "\"cardToken\":\"\","
                   + "\"startDate\":\"" + txtPCIStartDate.Value + "\","
                   + "\"expiryDate\":\"" + txtPCIExpiryDate.Value + "\","
                   + "\"issueNumber\":\"" + txtPCIIssueNumber.Value + "\","
                   + "\"defaultCard\":\"" + txtPCIDefaultCard.Value + "\","
                   + "\"cvv\":\"" + txtPCICVV.Value + "\","
                   + "\"cardBillingInfo\":"
                   + "{"
                       + "\"playerAddress1\":\"" + txtPCIAddress1.Value + "\","
                       + "\"playerAddress2\":\"" + txtPCIAddress2.Value + "\","
                       + "\"city\":\"" + txtPCICity.Value + "\","
                       + "\"county\":\"" + txtPCICounty.Value + "\","
                       + "\"state\":\"" + txtPCIState.Value + "\","
                       + "\"zipCode\":\"" + txtPCIZipCode.Value + "\","
                       + "\"country\":\"" + txtPCICountry.Value + "\""
                   + "}"
               + "}"
           + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_GetPrePaidAccountBalance_RequestJson(StiPlayer pPlayer)
    {
        var mJson =
            "{"
                + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
                + "\"siteId\":\"" + hiddenSiteId.Value + "\","
                + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
                + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
                + "\"prePaidAccountDetails\":"
                    + "{"
                        + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                        + "\"prePaidAccountToken\":\"" + txtPlayerUsername.Value + "\""
                    + "}"
                + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_GetRegisteredCards_RequestJson()
    {
        var mJson =
            "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"userName\":\"" + txtPlayerUsername.Value + "\""
            + "}";

        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_GetPlayerSSN_RequestJson()
    {
        var mJson =
            "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"userName\":\"" + txtPlayerUsername.Value + "\","
            + "\"flag\":\"ssnLookup\""
            + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_GetTransactionDetails_RequestJson()
    {
        var mJson =
            "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"stTransactionNo\":\"1234567890\"," //todo: where to get transaction number
            + "\"orderReference\":\"123456\"," //todo: where to get order reference
            + "\"flag\":\"retrieve\""
            + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_GetRegisteredAccounts_RequestJson()
    {
        var mJson =
            "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"userName\":\"" + txtPlayerUsername.Value + "\""
            + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_GetPrePaidAccountHolderInfo_RequestJson(StiPlayer pPlayer)
    {
        var mJson =
        "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"playerDetails\":"
            + "{"
                + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                + "\"connectionToken\":\"" + txtPlayerConnectionToken.Value + "\","
                + "\"sessionToken\":\"" + txtPlayerSessionToken.Value + "\","
               + "\"prePaidAccountDetails\":"
                + "{"
                    + "\"prePaidAccountToken\":\"" + txtPPAToken.Value + "\""
                + "}"
            + "}"
        + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_PreCheckSiteModifyCardRequest_RequestJson(string pDeleteFlag)
    {
        var mJson =
        "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"userName\":\"" + txtPlayerUsername.Value + "\","
           + "\"accountType\":\"ECOM\","
           + "\"connectionToken\":\"" + txtPlayerConnectionToken.Value + "\","
           + "\"sessionToken\":\"" + txtPlayerSessionToken.Value + "\","
           + "\"cardDetails\":"
           + "{"
               + "\"nameOnCard\":\"" + txtPCINameOnCard.Value + "\","
               + "\"cardType\":\"" + txtPCICardType.Value + "\","
               + "\"cardNumber\":\"" + txtPCICardNumber.Value + "\","
               + "\"cardToken\":null,"
               + "\"startDate\":\"" + txtPCIStartDate.Value + "\","
               + "\"expiryDate\":\"" + txtPCIExpiryDate.Value + "\","
               + "\"issueNumber\":\"" + txtPCIIssueNumber.Value + "\","
               + "\"defaultCard\":\"" + txtPCIDefaultCard.Value + "\","
               + "\"deleteFlag\":\"" + pDeleteFlag + "\","
               + "\"cvv\":\"" + txtPCICVV.Value + "\","
               + "\"cardBillingInfo\":"
               + "{"
                   + "\"playerAddress1\":\"" + txtPCIAddress1.Value + "\","
                   + "\"playerAddress2\":\"" + txtPCIAddress2.Value + "\","
                   + "\"city\":\"" + txtPCICity.Value + "\","
                   + "\"county\":\"" + txtPCICounty.Value + "\","
                   + "\"state\":\"" + txtPCIState.Value + "\","
                   + "\"zipCode\":\"" + txtPCIZipCode.Value + "\","
                   + "\"country\":\"" + txtPCICountry.Value + "\""
               + "}"
           + "}"
       + "}";

        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_PreCheckSiteAccountRequest_RequestJson()
    {
        var mJson =
            "{"
                + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
                + "\"siteId\":\"" + hiddenSiteId.Value + "\","
                + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
                + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
                + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                + "\"accountType\":\"ACH\","
                + "\"connectionToken\":\"" + txtPlayerConnectionToken.Value + "\","
                + "\"sessionToken\":\"" + txtPlayerSessionToken.Value + "\","
                + "\"geoComplyEncryptedPacket\":\"" + txtGeoInfo.Value + "\","
                + "\"atmVerify\":\"Y\","
                + "\"accountDetails\":"
                + "{"
                    + "\"bankName\":\"" + txtBankName.Value + "\","
                    + "\"bankAccountType\":\"" + txtBAAccountType.Value + "\","
                    + "\"accountNumber\":\"" + txtBAAccountNumber.Value + "\","
                    + "\"abaNumber\":\"" + txtabaNumber.Value + "\""
                + "}"
            + "}";

        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_stapiSubmitDepositRequest_RequestJson(StiPlayer pPlayer)
    {
        /*
        { 
        "operatorId":"000", 
        "siteId":"000", 
        "siteUsername":"123456789545", 
        "sitePwd":")*(&^%$#", 
        "depositDetails": 
            { 
            "userName":"johnsmith1", 
            "connectionToken":"gnsekjgnlkjfdgnethtkfgnkjgfn=", 
            "sessionToken":"52445", 
            "transactionType":"DEPOSIT", 
            "orderReference":"578954", 
            "threeDFlag":"TRUE", 
            "accountType":"ECOM", 
            "currency":"USD", 
            "amount":"1000",   	
            "cardDetails": 
            { 
                "nameOnCard":"John Smith", 
                "cardType":"VISA", 
                "cardNumber":"4111110000000211", 
                "cardToken":null, 
                "startDate":"02/2011", 
                "expiryDate":"04/2016", 
                "issueNumber":"20", 
                "defaultCard":"Y", 
                "cvv":"123", 
                "cardBillingInfo": 
                { 
                    "playerAddress1":"123 LORDS STREET", 
                    "playerAddress2":"123 SECOND STREET", 
                    "city":"ANNEMANIE", 
                    "county":"RUSSELL", 
                    "state":"ALABAMA", 
                    "zipCode":"36721", 
                    "country":"UNITED STATES" 
                } 	
            }
        }
        */
        var mJson =
        "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"depositDetails\":"
            + "{"
                + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                + "\"accountType\":\"ACH\","
                + "\"connectionToken\":\"" + txtPlayerConnectionToken.Value + "\","
                + "\"sessionToken\":\"" + txtPlayerSessionToken.Value + "\","
                + "\"transactionType\":\"DEPOSIT" + "\","
                + "\"orderReference\":\"578954" + "\","
                + "\"threeDFlag\":\"TRUE" + "\","
                + "\"accountType\":\"" + txtPCIAccountType.Value + "\","
                + "\"currency\":\"" + "USD" + "\","
                + "\"amount\":\"" + "1000" + "\","
                + "\"cardDetails\":"
                + "{"
                    + "\"nameOnCard\":\"" + txtPCINameOnCard.Value + "\","
                    + "\"cardType\":\"" + txtPCICardType.Value + "\","
                    + "\"cardNumber\":\"" + txtPCICardNumber.Value + "\","
                    + "\"cardToken\":\"null\","
                    + "\"startDate\":\"" + txtPCIStartDate.Value + "\","
                    + "\"expiryDate\":\"" + txtPCIExpiryDate.Value + "\","
                    + "\"issueNumber\":\"" + txtPCIIssueNumber.Value + "\","
                    + "\"defaultCard\":\"" + txtPCIDefaultCard.Value + "\","
                    + "\"cvv\":\"" + txtPCICVV.Value + "\","
                    + "\"cardBillingInfo\":"
                    + "{"
                        + "\"playerAddress1\":\"" + txtPCIAddress1.Value + "\","
                        + "\"playerAddress2\":\"" + txtPCIAddress2.Value + "\","
                        + "\"city\":\"" + txtPCICity.Value + "\","
                        + "\"county\":\"" + txtPCICounty.Value + "\","
                        + "\"state\":\"" + txtPCIState.Value + "\","
                        + "\"zipCode\":\"" + txtPCIZipCode.Value + "\","
                        + "\"country\":\"" + txtPCICountry.Value + "\""
                    + "}"
                + "}"
            + "}"
        + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_PrePaidRegisterAndLoad_RequestJson(StiPlayer pPlayer)
    {
/*
{ 
 	"operatorId":"000", 
 	"siteId":"000", 
 	"siteUsername":"123789456245", 
 	"sitePwd":")(*&^%", 
 	"geoComplyEncryptedPacket":"ZsUiDymAiyVr/CGEDzr7tveVE=", 
 	"tandcAccepted":"Y", 
 	"tandcAcceptedTimestampUTC":"2014-07-28T10:48:07.530+00:00",  	
    "depositDetails": 
    { 
 	 	"userName":"johnsmith1", 
 	 	"connectionToken":"6789gfgdfhgdfgdfghhtr4", 
 	 	"sessionToken":"200446", 
 	 	"orderReference":"1234", 
 	 	"accountType":"ECOM", 
 	 	"currency":"USD",  
 	 	"transactionType":"DEPOSIT", 
 	 	"threeDFlag":"TRUE",  	 	
        "amount":"10000",  	 	
        "cardDetails": 
 	    { 
 	 	 	"nameOnCard":"John Smith", 
 	 	 	"cardType":"VISA", 
 	 	 	"cardNumber":"5100000000002301", 
 	 	 	"cardToken":null, 
 	 	 	"startDate":"02/2011", 
 	 	 	"expiryDate":"04/2014", 
 	 	 	"issueNumber":"20", 
 	 	 	"defaultCard":"Y", 
 	 	 	"cvv":"123", 
 	 	 	"cardBillingInfo": 		 	
            { 
 	 	 	 	"playerAddress1":"123 LORDS STREET", 
 	 	 	 	"playerAddress2":"123 SECOND STREET", 
 	 	 	 	"city":"ANNEMANIE", 
 	 	 	 	"county":"RUSSELL", 
 	 	 	 	"state":"ALABAMA", 
 	 	 	 	"zipCode":"36721", 
 	 	 	 	"country":"UNITED STATES" 
 	 	 	} 
 	 	} 
 	} 
} 
*/
        var mtandcAcceptedTimestampUTC = pPlayer.PPATandCAccepted;
        if (String.IsNullOrEmpty(mtandcAcceptedTimestampUTC))
        {
            mtandcAcceptedTimestampUTC = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            mtandcAcceptedTimestampUTC = mtandcAcceptedTimestampUTC + "+00:00";
        }
        var mJson =
            "{"
                + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
                + "\"siteId\":\"" + hiddenSiteId.Value + "\","
                + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
                + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
                + "\"geoComplyEncryptedPacket\":\"ZsUiDymAiyVr/CGEDzr7tveVE=\"," 
 	            + "\"tandcAccepted\":\"Y\","
                + "\"tandcAcceptedTimestampUTC\":\"" + mtandcAcceptedTimestampUTC + "\""
                + "\"depositDetails\":"
                + "{"
                    + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                    + "\"accountType\":\"ACH\","
                    + "\"connectionToken\":\"" + txtPlayerConnectionToken.Value + "\","
                    + "\"sessionToken\":\"" + txtPlayerSessionToken.Value + "\","
                    + "\"transactionType\":\"DEPOSIT" + "\","
                    + "\"orderReference\":\"578954" + "\","
                    + "\"threeDFlag\":\"TRUE" + "\","
                    + "\"accountType\":\"" + txtPCIAccountType.Value + "\","
                    + "\"currency\":\"" + "USD" + "\","
                    + "\"amount\":\"" + "1000" + "\","
                    + "\"cardDetails\":"
                    + "{"
                        + "\"nameOnCard\":\"" + txtPCINameOnCard.Value + "\","
                        + "\"cardType\":\"" + txtPCICardType.Value + "\","
                        + "\"cardNumber\":\"" + txtPCICardNumber.Value + "\","
                        + "\"cardToken\":\"null\","
                        + "\"startDate\":\"" + txtPCIStartDate.Value + "\","
                        + "\"expiryDate\":\"" + txtPCIExpiryDate.Value + "\","
                        + "\"issueNumber\":\"" + txtPCIIssueNumber.Value + "\","
                        + "\"defaultCard\":\"" + txtPCIDefaultCard.Value + "\","
                        + "\"cvv\":\"" + txtPCICVV.Value + "\","
                        + "\"cardBillingInfo\":"
                        + "{"
                            + "\"playerAddress1\":\"" + txtPCIAddress1.Value + "\","
                            + "\"playerAddress2\":\"" + txtPCIAddress2.Value + "\","
                            + "\"city\":\"" + txtPCICity.Value + "\","
                            + "\"county\":\"" + txtPCICounty.Value + "\","
                            + "\"state\":\"" + txtPCIState.Value + "\","
                            + "\"zipCode\":\"" + txtPCIZipCode.Value + "\","
                            + "\"country\":\"" + txtPCICountry.Value + "\""
                        + "}"
                    + "}"
                + "}"
            + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_GetPrePaidAccountCVV2_RequestJson(StiPlayer pPlayer)
    {
    /*
    { 
        "operatorId":"000", 
        "siteId":"000", 
        "siteUsername":"123789456245", 
        "sitePwd":")(*&^%",
        "playerDetails": 
 	    { 
 	        "userName":"johnsmith1", 
 	        "connectionToken":"6789gfgdfhgdfgdfghhtr4", 
 	        "sessionToken":"200446",  	
            "prePaidAccountDetails": 
 	 	    { 
 	 	        "virtualCardNumber":"000000000000000000" 
 	 	    } 
 	    } 
    } 
    */
        var mJson =
            "{"
                + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
                + "\"siteId\":\"" + hiddenSiteId.Value + "\","
                + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
                + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
                + "\"playerDetails\":"
                + "{"
                    + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                    + "\"connectionToken\":\"" + txtPlayerConnectionToken.Value + "\","
                    + "\"sessionToken\":\"" + txtPlayerSessionToken.Value + "\","
                    + "\"prePaidAccountDetails\":"
                    + "{"
                        + "\"virtualCardNumber\":\"" + "000000000000000000" + "\""
                    + "}"
                + "}"
            + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        } 
        return mJson;
    }
    
    private string Construct_UpdatePlayerSSN_RequestJson(StiPlayer pPlayer)
    {
        /*
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"123456789545", 
            "sitePwd":")*(&^%$#", 
            "userName":"johnsmith1", 
            "flag":"ssnUpdateFinal", 
            "iqRequest": 
 	         {  
 	            "requestToken":"1gNXr12wnctP8Om3zRSdmW8xHgAlooWJNXo", 
 	            "interactiveQueryId":1,  	
                "answer": 
 	 	         [ 
 	 	 	        { 
 	 	 	            "answerId":1, 
 	 	 	            "questionId":1 
 	 	 	        }, 
 	 	 	        { 
 	 	 	            "answerId":2, 
 	 	 	            "questionId":2 
 	 	 	        }, 
 	 	 	        { 
 	 	 	            "answerId":1, 
 	 	 	            "questionId":3 
 	 	 	        }, 
 	 	 	        { 
 	 	 	            "answerId":1, 
 	 	 	            "questionId":4 
 	 	 	        } 
 	 	        ] 	 
 	        } 
        } 
        */

        var mJson =
            "{"
                + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
                + "\"siteId\":\"" + hiddenSiteId.Value + "\","
                + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
                + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
                + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                + "\"flag\":\"" + "ssnUpdateFinal" + "\","
                + "\"iqRequest\":"
                + "{"
                    + "\"requestToken\":\"" + hiddenRequestToken.Value + "\","
                    + "\"interactiveQueryId\":1,"
                    + "\"answer\":" 
 	 	            + "["
 	 	 	            + "{"
 	 	 	                + "\"answerId\":1,"
 	 	 	                + "\"questionId\":1," 
 	 	 	            + "},"
 	 	 	            + "{"
 	 	 	                + "\"answerId\":2,"
 	 	 	                + "\"questionId\":2," 
 	 	 	            + "},"
 	 	 	            + "{"
 	 	 	                + "\"answerId\":3,"
 	 	 	                + "\"questionId\":3," 
 	 	 	            + "},"
 	 	 	            + "{"
 	 	 	                + "\"answerId\":4,"
 	 	 	                + "\"questionId\":4," 
 	 	 	            + "},"
 	 	 	        + "]"
                + "}"
            + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_AccountWithdrawal_RequestJson(StiPlayer pPlayer)
    {
        /*
        { 
        "operatorId":"000", 
        "siteId":"000", 
        "siteUsername":"457985621458", 
        "sitePwd":"(*^*%^$#", "withdrawalDetails":  
 	        { 
 	        "userName":"johnsmith1", 
 	        "transactionType":"WITHDRAWAL", 
 	        "orderReference":"3213444", 
 	        "accountToken":"7DHV_UZt3v30RXZXkv2D_e87y0cppadz_wBnM0=",
 	        "accountType":"ACH", 
 	        "currency":"USD", 
 	        "amount":"1000" 
 	        } 
        }
         */             
        var mJson = 
            "{"
                + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
                + "\"siteId\":\"" + hiddenSiteId.Value + "\","
                + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
                + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
                + "\"withdrawalDetails\":"
                + "{"
                    + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                    + "\"transactionType\":\"" + "WITHDRAWAL" + "\","
                    + "\"orderReference\":\"" + "3213444" + "\","
                    + "\"accountToken\":\"" + txtPPAToken.Value + "\","
                    + "\"accountType\":\"" + "ACH" + "\","
                    + "\"currency\":\"" + "USD" + "\","
                    + "\"amount\":\"" + "1000" + "\""
                + "}"
            + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_UpdatePrePaidAccountStatus_RequestJson(StiPlayer pPlayer)
    {
        /*
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"123789456245", 
            "sitePwd":")(*&^%", 
            "playerDetails": 
            { 
                "userName":"johnsmith1", 
                "connectionToken":"6789gfgdfhgdfgdfghhtr4", 
                "sessionToken":"200446",
                "prePaidAccountDetails": 
                { 
                    "prePaidAccountToken":"IpFlmsW9vWXpH3DAVsMi-g=", 
                    "status":"Active"      
                } 
            } 
        } 
        */

        var mJson =
        "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"playerDetails\":"
            + "{"
                + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                + "\"connectionToken\":\"" + txtPlayerConnectionToken.Value + "\","
                + "\"sessionToken\":\"" + txtPlayerSessionToken.Value + "\","
                + "\"prePaidAccountDetails\":"
                + "{"
                    + "\"prePaidAccountToken\":\"" + txtPPAToken.Value + "\","
                    + "\"status\":\"" + "Active" + "\""
                + "}"
            + "}"
        + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_SubmitRefund_RequestJson(StiPlayer pPlayer)
    {
        /*
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"123456789545", 
            "sitePwd":")*(&^%$#", 
            "refundDetails": 
 	        { 
 	            "userName":"johnsmith1", 
 	            "transactionType":"REFUND", 
 	            "depositSTTransactionNo":"1234567890", 
 	            "expiryDate":"", 
                "accountType":"ECOM", 
                "currency":"USD", 
 	            "amount":"1000" 
 	        } 
        } 

         * */
        var mJson =
        "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"refundDetails\":"
            + "{"
                + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                + "\"transactionType\":\"" + "REFUND" + "\","
                + "\"depositSTTransactionNo\":\"" + "3213444" + "\","
                + "\"expiryDate\":\"\","
                + "\"accountType\":\"" + "ECOM" + "\","
                + "\"currency\":\"" + "USD" + "\","
                + "\"amount\":\"" + "1000" + "\""
            + "}"
        + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_AddSelfExcludePlayer_RequestJson(StiPlayer pPlayer)
    {
        /*
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"123456789545", 
            "sitePwd":")*(&^%$#", 
            "exclusionDetails": 
            { 
 	            "userName":"johnsmith1",  
 	            "fromDate":"03/26/2012", 
 	            "toDate":"06/05/2012", 
 	            "requestHelp":"Y", 
 	            "sessionToken":"233900", 
 	            "connectionToken":"653be279f22dede9310237db59525bd778523e429b7cdd965f092bba916a0ca1" 
 	        } 
        } 
        */
        var formDate = DateTime.Now.AddDays(-1).ToString("d");
        var toDate = DateTime.Now.AddMonths(6).ToString("d");
        var mJson =
            "{"
                + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
                + "\"siteId\":\"" + hiddenSiteId.Value + "\","
                + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
                + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
                + "\"exclusionDetails\":" 
                + "{"
                    + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                    + "\"fromDate\":\"" + formDate + "\","
                    + "\"toDate\":\"" + toDate + "\","
                    + "\"sessionToken\":\"" + txtPlayerSessionToken.Value + "\","
                    + "\"connectionToken\":\"" + txtPlayerConnectionToken.Value + "\""
                + "}"
            + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_SelfExcludePlayer_RequestJson(StiPlayer pPlayer)
    {
        /*
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"123456789545", 
            "sitePwd":")*(&^%$#",
            "userName":"johnsmith1",
            "requestToken":"KLvYTGHhvmIoys6qF_HVH_sJbJXJ5ECfpDnzwRVtpWx=",  
            "exclusionDetails": 
 	        { 
 	            "fromDate":"03/26/2012", 
 	            "toDate":"06/05/2012", 
 	            "requestHelp":"Y", 
 	            "requestExtension":"N", 
 	            "screenShown":"Y"
 	        } 
        } 
     */
        var mJson =
        "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"userName\":\"" + txtPlayerUsername.Value + "\","
            + "\"requestToken\":\"" + "KLvYTGHhvmIoys6qF_HVH_sJbJXJ5ECfpDnzwRVtpWx=" + "\","
            + "\"exclusionDetails\":"
            + "{"
                + "\"fromDate\":\"" + "07/05/2015" + "\","
                + "\"toDate\":\"" + "08/05/2015" + "\","
                + "\"requestHelp\":\"" + "Y" + "\","
                + "\"requestExtension\":\"" + "N" + "\","
                + "\"screenShown\":\"" + "Y" + "\""
            + "}"
       + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_ModifyPlayerRequest_RequestJson(StiPlayer pPlayer)
    {
        /*
        { 
            "operatorId":"123", 
            "siteId":"121", 
            "siteUsername":"123456789545", 
            "sitePwd":")*(&^%$#", 
            "playerDetails": 
 	        { 
 	            "userName":"johnsmith1", 
 	            "connectionToken":"a8af858803cc44ce114ac1a78137c18", 
 	            "sessionToken":"233936", 
 	            "firstName":"John",  
 	            "middleInitial":"M", 
 	            "lastName":"Smith", 
 	            "gender":"MALE", 
 	            "dob":"02/01/1969", 
 	            "emailAddress":"john.smith@example.com", 
 	            "playerAddress1":"123 LORDS STREET", 
 	            "playerAddress2":"123 SECOND STREET", 
 	            "city":"ANNEMANIE", 
 	            "county":"RUSSELL", 
 	            "state":"ALABAMA", 
 	            "zipCode":"36721", 
 	            "country":"UNITED STATES", 
 	            "mobileNo":"+1-5648956248", 
                "landLineNo":"+1-1234567895", 
                "ssn":"000000000", 
                "dlNumber":"", 
                "dlIssuingState":"", 
                "ipAddress":"209.237.227.195", 
                "deviceFingerPrint":""
            }
        } 
         */
        var mJson =
        "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + Construct_GetPlayerInfo_RequestJson() 
        + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_AuthDepositRequest_RequestJson(StiPlayer pPlayer)
    {
        /*
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"123456789545", 
            "sitePwd":")*(&^%$#", 
            "md":"kdgfkdhsgfkdshgf", //Merchant session tracking identifier
            "paRes":"jdsfjdsgfdgb48743jdbf" //It is a code sent by the 3 - D secure third - 
                    party to identify if 3 - Dauthentication has been successful or failed. 
                    The possible values are: Y - The 3 - D secure authentication is successful. 
                    N- 3 - D secure authentication failed.  
        } 

        */
        var mJson =
        "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"md\":\"" + "kdgfkdhsgfkdshgf" + "\","
            + "\"paRes\":\"" + "jdsfjdsgfdgb48743jdbf" + "\""
        + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_Withdrawal_RequestJson(StiPlayer pPlayer)
    {
    /*
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"457985621458", 
            "sitePwd":"(*^*%^$#", 
            "withdrawalDetails":  
 	        {
 	            "userName":"johnsmith1", 
 	            "transactionType":"WITHDRAWAL", 
 	            "orderReference":"3213444", 
 	            "cardToken":"7DHV_UZt3v30RXZXkv2D_e87y0cppadz_wBnM0=", 
 	            "cvv":"129", 
 	            "accountType":"CFT", 
 	            "currency":"USD", 
 	            "amount":"1000" 
 	        }
        }
    */
        var mJson =
        "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"withdrawalDetails\":"
            + "{"
                + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                + "\"transactionType\":\"" + "WITHDRAWAL" + "\","
                + "\"orderReference\":\"" + "3213444" + "\","
                + "\"cardToken\":\"" + "Y" + "\","
                + "\"cvv\":\"" + txtPCICVV.Value + "\","
                + "\"accountType\":\"" + "CFT" + "\","
                + "\"currency\":\"" + "USD" + "\","
                + "\"amount\":\"" + "1000" + "\""
            + "}"
       + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_SubmitReversal_RequestJson(StiPlayer pPlayer)
    {
        /*
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"123456789545", 
            "sitePwd":")*(&^%$#", 
            "reversalDetails": 
            { 
 	            "userName":"johnsmith1", 
 	            "transactionType":"REVERSAL", 
 	            "depositSTTransactionNo":"1234567890", 
 	            "accountType":"ECOM", 
 	            "currency":"USD", 
 	            "amount":"1000"  
            } 
        } 
        */
        var mJson =
        "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"reversalDetails\":"
            + "{"
                + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                + "\"transactionType\":\"" + "REVERSAL" + "\","
                + "\"depositSTTransactionNo\":\"" + "1234567890" + "\","
                + "\"accountType\":\"" + "ECOM" + "\","
                + "\"currency\":\"" + "USD" + "\","
                + "\"amount\":\"" + "1000" + "\""
            + "}"
       + "}";
        try
        {
            dynamic text = JObject.Parse(mJson);
        }
        catch
        {
            showError("Bad Json in " + GetCurrentMethod());
        }
        return mJson;
    }

    private string Construct_PreCheckSiteModifyAccountRequest_RequestJson(StiPlayer pPlayer)
    {
        var mJson =
        "{"
        + "\"operatorId\":\"000\","
        + "\"siteId\":\"000\","
        + "\"siteUsername\":\"123789456245\","
        + "\"sitePwd\":\")(*&^%\","
        + "\"userName\":\"johnsmith1\","
        + "\"accountType\":\"ACH\","
        + "\"connectionToken\":\"65325bd778523e429b7cdd965f092bba916a0ca1\","
        + "\"sessionToken\":\"200446\","
        + "\"accountDetails\":"
            + "{"
            + "\"bankName\":\"FIRSTBANK\","
            + "\"bankAccountType\":\"SAVINGS\","
            + "\"accountNumber\":\"1111111111111\","
            + "\"accountToken\":\"null\","
            + "\"deleteFlag\":\"Y\","
            + "\"abaNumber\":\"111111111"
            + "}"
        + "}";
        return mJson;
    }
    #endregion

    private void ParseJsonQuestionToForm(string sj)
    {
        var form = new StringBuilder();
        dynamic questions = JObject.Parse(sj);
        hiddenQuestionsResponseToken.Value = questions.responseToken.ToString();
        form.Append("<label>responseToken:</label>&nbsp;<span id=\"lbresponseToken\">" + questions.responseToken.ToString() + "</span>");
        form.Append("&nbsp;-&nbsp;");
        form.Append("<label>returnCode:</label>&nbsp;<span id=\"returnCode\">" + questions.returnCode.ToString() + "</span>");
        form.Append("&nbsp;-&nbsp;");
        form.Append("<label>" + questions.stTimeStamp.ToString() + "</label>");
        form.Append("<br /><br />");
        form.Append("<span style=\"font-weight:bold\" id=\"returnMsg\">" + questions.returnMsg.ToString() + "</span>");
        form.Append("<br />");
        form.Append("____ kycDetails interactiveQueryId - transactionKey _____________");
        form.Append("<br />");
        var kycDetails = questions.kycDetails;
        form.Append("<label>interactiveQueryId:</label>&nbsp;<span id=\"interactiveQueryId\">" + kycDetails.interactiveQueryId.ToString() + "</span>");
        form.Append("&nbsp;&nbsp;");
        form.Append("<label>transactionKey:</label>&nbsp;<span id=\"transactionKey\">" + kycDetails.transactionKey.ToString() + "</span>");
        form.Append("<br />");
        form.Append("______ question interactiveQueryId _______________________________________");
        form.Append("<br />");
        var questionl1 = kycDetails.question;
        form.Append("<label>question interactiveQueryId:</label>&nbsp;<span id=\"lbQuestionInteractiveQueryId:\">" + questionl1.interactiveQueryId.ToString() + "</span>");
        form.Append("<br />");
        form.Append("____ questions ____________________________________________________________");
        form.Append("<br />");
        var questionl2 = questionl1.question;
        foreach (var thequestions in questionl2)
        {
            var questionId = thequestions.questionId.ToString();
            form.Append("- - -&nbsp;" + thequestions.questionText.ToString() + "&nbsp;- - - - - - - - - - - - - -");
            form.Append("<br />");
            foreach (var answerChoise in thequestions.answerChoice)
            {
                var value = answerChoise.value.ToString();
                var answerId = answerChoise.answerId;
                form.Append("&nbsp;&nbsp;<input type=\"radio\" name=\"" + questionId + "\" value=\"" + answerId + "\" onclick=\"AnswerSelected(" + questionId + "," + answerId + ");\">" + value + "<br>");
            }
        }
        divQuestions.InnerHtml = form.ToString();
        divQuestionsContainer.Visible = true;
        divOtpValidate.Visible = false;
    }

    #region Helper Methods

    private void showError(string pErrMsg)
    {
        if (String.IsNullOrEmpty(lbError.Text))
        {
            lbError.Text =  pErrMsg;
            return;
        }
        lbError.Text = lbError.Text + "<br />" + pErrMsg;
    }
    
    private int ToNumeric(string pNumber)
    {
        int x;
        Int32.TryParse(pNumber.Trim(), out x);
        return x;
    }

    private string GetSiteUserNameOrLoopbackTestCase()
    {
        if (hiddenService.Value == "Wrapper")
        {
            if (String.IsNullOrEmpty(hiddenLoopbackEnabled.Value) == false)
            {
                if (hiddenLoopbackEnabled.Value != "0")
                    return "Loopback Test Case=" + hiddenLoopbackEnabled.Value;
            }
        }
        return txtSiteUsername.Value;
    }

    private string RandomizeUserName(string userFirstName)
    {
        var mFN = userFirstName;
        if (mFN.Length > 12)
            mFN = mFN.Substring(0, 10) + "..";
        var formattedDateTime = DateTime.Now.ToString("yymmddHHmmss");
        return mFN + formattedDateTime;
    }
    
    private void ClearOperatorInfo()
    {
        dlOperatorSites.SelectedIndex = 0;
        lbOperatorId.Text = "";
        lbSiteId.Text = "";
        txtSiteUsername.Value = "";
        txtSiteUserPassword.Value = "";
        txtPlayerUsername.Value = "";
        txtGeoInfo.Value = "";
        txtAuthorizedIP.Value = "";
    }

    private void ClearPlayerInfo()
    {
        dlPlayer.SelectedIndex = 0;
        txtPlayerUsername.Value = "";
        txtPlayerFirstName.Value = "";
        txtPlayerMiddleInitial.Value = "";
        txtPlayerLastName.Value = "";
        txtPlayerGender.Value = "";
        txtPlayerDoB.Value = "";
        txtPlayerEmail.Value = "";
        txtPlayerAddress1.Value = "";
        txtPlayerAddress2.Value = "";
        txtPlayerCity.Value = "";
        txtPlayerCounty.Value = "";
        txtPlayerState.Value = "";
        txtPlayerZip.Value = "";
        txtPlayerCountry.Value = "";
        txtPlayerMobilePhone.Value = "";
        txtPlayerHomePhone.Value = "";
        txtPlayerSSN.Value = "";
        txtPlayerDriversLicenseNumber.Value = "";
        txtPlayerDriversLicenseIssueState.Value = "";
        // divPlayerCardInfo
        //txtPCIUserName.Value = "";
        txtPCIAccountType.Value = "";
        txtPCINameOnCard.Value = "";
        txtPCICardType.Value = "";
        txtPCICardNumber.Value = "";
        txtPCICVV.Value = "";
        txtPCIStartDate.Value = "";
        txtPCIExpiryDate.Value = "";
        txtPCIIssueNumber.Value = "";
        txtPCIDefaultCard.Value = "";
        txtPCIAddress1.Value = "";
        txtPCIAddress2.Value = "";
        txtPCICity.Value = "";
        txtPCICounty.Value = "";
        txtPCIState.Value = "";
        txtPCIZipCode.Value = "";
        txtPCICountry.Value = "";

        //divPlayerPrePaidAccountInfo
        //txtPPAUserName.Value = "";
        txtPPANumber.Value = "";
        txtPPAVirtualCardNumber.Value = "";
        txtPPAtandcAccepted.Value = "";
        txtPPAtandcAcceptedTimestampUTC.Value = "";

        divPlayerContainer.Visible = false;
        //divPlayerCardInfo.Visible = false;
        //divPlayerPrePaidAccountInfo.Visible = false;
        divPlayerOperationsContainer.Visible = false;
    }

    private void CheckExistingSitesAndCreateMissingSites()
    {
        // Checks the ST Integration database to see if all the operator / sites exists
        // if any are missing create them.
        var myDatabase = new MongoDBData();
        var mSites = myDatabase.GetOperatorSites();
        var xmlDoc = MAC_GetMACClientWithSTSiteIds();
        var elemList = xmlDoc.GetElementsByTagName("Error");
        if (elemList.Count != 0)
        {
            showError(elemList[0].InnerXml);
            return;
        }
        elemList = xmlDoc.GetElementsByTagName("STClient");
        if (elemList.Count != 0)
        {
            foreach (XmlElement mSTClientElement in elemList)
            {
                var SiteId = mSTClientElement.GetAttribute("SiteId");
                var OperatorId = mSTClientElement.GetAttribute("OperatorId");
                var ClientId = mSTClientElement.GetAttribute("ClientId");
                var ClientName = mSTClientElement.GetAttribute("ClientName");
                var found = false;
                foreach (var mSite in mSites)
                {
                    if ((mSite.OperatorId == OperatorId) && (mSite.SiteId == SiteId))
                    {
                        // found
                        found = true;
                        if ((mSite.macClientId != ClientId) || (mSite.macClientName != ClientName))
                        {
                            mSite.macClientId = ClientId;
                            mSite.macClientName = ClientName;
                            myDatabase.UpdateOperatorSite(mSite);
                        }
                    }
                }
                if (found == false)
                {
                    var mNewSite = new StiOperatorSite();
                    mNewSite.OperatorId = OperatorId;
                    mNewSite.SiteId = SiteId;
                    mNewSite.macClientId = ClientId;
                    mNewSite.macClientName = ClientName;
                    if ((OperatorId == "188") && (SiteId == "229"))
                    {
                        mNewSite.SiteUserName = "1886232015229";
                        mNewSite.SitePassword = "1H#k9rr4ocReKfSt";
                    }
                    else if ((OperatorId == "188") && (SiteId == "227"))
                    {
                        mNewSite.SiteUserName = "188642015227";
                        mNewSite.SitePassword = "@8cIa6NBTdTZjzb7";
                    }
                    myDatabase.Save(mNewSite);
                }
            }
        }
    }

    private void AddToLogAndDisplay(string serviceType, string header, string textToAdd)
    {
        var logData = "<div style='border-bottom: dashed 1px #c0c0c0; margin-bottom: 15px; padding-bottom: 15px;'>";
        logData += "    <div style='font-weight: bold;'>";
        logData += serviceType + " " + header;
        logData += "    </div>";
        logData += "    <div>";
        logData += textToAdd;
        logData += "    </div>";
        logData += "</div>";

        divJsonResponseData.InnerHtml += logData;
        LogToFile(serviceType + " " + header, textToAdd);
    }

    private void LogToFile(string pHeader, string pLogentry)
    {
        if (!cbLogToFile.Checked) return;
        // compute file name using mmddyy
        var mLogfilename = "ST_OperLog-" + DateTime.Now.ToString("MMddyy");
        var mRoot = ConfigurationManager.AppSettings["LogFileRootPath"];
        if (mRoot == null)
        {
            showError("No LogFileRootPath in web.config file, can't log to file.");
            return;
        }
       // create the log file directory is it doesn not exists
        if (Directory.Exists(mRoot) == false)
        {
            try
            {
                Directory.CreateDirectory(mRoot);
            }
            catch (Exception ex)
            {
                showError("Could not create directory for log files: " + ex.Message);
                return; 
            }
        }

        try
        {
            var path = Path.Combine(mRoot, mLogfilename + ".txt");
            if (File.Exists(path))
            {
                using (var sw = File.AppendText(path))
                {
                    sw.WriteLine("- - -" + DateTime.Now.ToString("HH:mm:sss") + "- - - - - - - - -");
                    if (!string.IsNullOrEmpty(pHeader))
                        sw.WriteLine(pHeader);
                    sw.WriteLine(pLogentry);
                }
                return;
            }

            // Create a file to write to. 
            using (var sw = File.CreateText(path))
            {
                sw.WriteLine("- - -" + DateTime.Now.ToString("HH:mm:sss") + "- - - - - - - - -");
                if (!string.IsNullOrEmpty(pHeader))
                    sw.WriteLine(pHeader);
                sw.WriteLine(pLogentry);
            }
        }
        catch (Exception ex)
        {
            showError("LogToFile Exception: " + ex.Message);
        }
    }

    public string HexToString(String pInput)
    { // data is encoded in hex, convert it back to a string
        if (String.IsNullOrEmpty(pInput)) return string.Empty;
        var mInput = pInput.Trim();
        try
        {
            var sb = new StringBuilder();
            for (var i = 0; i < mInput.Length; i += 2)
            {
                var hs = mInput.Substring(i, 2);
                sb.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            showError(ex.Message);
            return string.Empty;
        }
    }

    public string GetCurrentMethod()
    {
        var st = new StackTrace();
        var sf = st.GetFrame(1);
        return sf.GetMethod().Name;
    }

    #endregion

    //private void CreateTestCards()
    //{
    //    // If TestCards collection exists in database just return
    //    // create instance of new TestCards object
    //    // read text card file one line at a time using strean read
    //    // add card to TestCard list in TestCard collection
    //    // at end file save TestCards object to database

    //    // need to creat new class for TestCards that contains list of TestCard see StiWrapperState.cs for example(events is a list)
    //    // need to create a new class for TestCard, see StiEvent.cs for an example, this should have all the registred props 
    //    // need to add methods to MongoDBData.cs:
    //    //    to save TestCards
    //    //    get current card by userName
    //    //    get next available TestCard in list
    //    //    get list of TestCard by userName

    //    var cardCount = 0;

    //    var mappedPath = Server.MapPath("~");

    //    var folderName = "Visa";

    //    var cardsDir = mappedPath + "TestData\\CreditCards\\" + folderName;

    //    var cardFiles = Directory.GetFiles(cardsDir, "*.txt");
    //    foreach (var currentFile in cardFiles)
    //    {
    //        // Load player info from txt file
    //        using (var reader = new StreamReader(currentFile))
    //        {
    //            string line;
    //            while ((line = reader.ReadLine()) != null)
    //            {
    //                if (line.Length == 16)
    //                {
    //                    TestCreditCard currentCard = new TestCreditCard();

    //                    currentCard.cardNumber = line;

    //                    Random rnd = new Random();
    //                    int ccvDigit = rnd.Next(000, 999);

    //                    currentCard.cardCCV = ccvDigit.ToString();

    //                    currentCard.cardExpires = DateTime.Now.AddDays(365).ToShortDateString();
    //                    currentCard.cardType = folderName;

    //                    // Set internal properties to the player
    //                    currentCard.stiPlayer.PCICardType = folderName;
    //                    currentCard.stiPlayer.PCICardNumber = currentCard.cardNumber;
    //                    currentCard.stiPlayer.PCICVV = currentCard.cardCCV;
    //                    currentCard.stiPlayer.PCIExpiryDate = currentCard.cardExpires;

    //                    StiEvent cardEvent = new StiEvent();
    //                    cardEvent.stRecordStatus = "Card created in database";

    //                    currentCard.events.Add(cardEvent);

    //                    currentCard.Save(currentCard);

    //                    cardCount++;

    //                    Response.Write("<div>" + folderName + " - " + cardCount + ") Card number: <b>" + currentCard.cardNumber + "</b></div>");
    //                }
    //            }
    //        }
    //    }

    //}

    //protected void dlAvailableCards_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    // Get card info
    //    TestCreditCard selectedCard = myMongoUtil.GetCreditCard(dlAvailableCards.SelectedValue);

    //    txtPCIUserName.Value = selectedCard.stiPlayer.userName;
    //    txtPCIAccountType.Value = selectedCard.stiPlayer.PCIAccountType;
    //    txtPCINameOnCard.Value = selectedCard.stiPlayer.PCINameOnCard;
    //    txtPCICardType.Value = selectedCard.cardType;
    //    txtPCICardNumber.Value = selectedCard.cardNumber;
    //    txtPCICVV.Value = selectedCard.cardCCV;
    //    txtPCIStartDate.Value = selectedCard.stiPlayer.PCIStartDate;
    //    txtPCIexpiryDate.Value = selectedCard.cardExpires;
    //    txtPCIIssueNumber.Value = selectedCard.stiPlayer.PCIIssueNumber;
    //    txtPCIDefaultCard.Value = selectedCard.stiPlayer.PCIDefaultCard;

    //    // Billing address info
    //    txtPCIAddress1.Value = selectedCard.stiPlayer.PCIAddress1;
    //    txtPCIAddress2.Value = selectedCard.stiPlayer.PCIAddress2;
    //    txtPCICity.Value = selectedCard.stiPlayer.PCICity;
    //    txtPCICounty.Value = selectedCard.stiPlayer.PCICounty;
    //    txtPCIState.Value = selectedCard.stiPlayer.PCIState;
    //    txtPCIZipCode.Value = selectedCard.stiPlayer.PCIZipCode;
    //    txtPCICountry.Value = selectedCard.stiPlayer.PCICountry;

    //    var registeredStatus = "<span style='color: #ff0000;'>Card not registered yet</span>";
    //    var registeredDate = "n/a";

    //    if (!string.IsNullOrEmpty(selectedCard.registeredDate))
    //    {
    //        registeredDate = selectedCard.registeredDate;
    //        registeredStatus = "<span style='color: #048413;'>Card registered</span>";
    //    }

    //    spanCardStatus.InnerHtml = registeredStatus;
    //    spanCardRegisteredDate.InnerHtml = registeredDate;
    //}

    //protected void dlAssignedCards_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    // Get card info

    //}
}