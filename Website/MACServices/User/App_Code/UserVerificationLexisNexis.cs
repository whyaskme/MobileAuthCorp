using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.Services;
using System.Xml;
using System.IO;
using System.Net;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using sr = MACServices.Constants.ServiceResponse;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class UserVerificationLexisNexis : WebService
{
    public Object PropertyValue { get; set; }
    public virtual Type PropertyType { get; set; }

    private const string mSvcName = "UserVerificationLexisNexis";
    private const string mLogId = "LN";

    [WebMethod]
    public XmlDocument WsUserVerificationLexisNexis(string data)
    {
        var mUtils = new Utils();

        Client myClient;
        // request data Dictionary
        var myData = new Dictionary<string, string>();

        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        // check if direct call to service
        if (data.Length < 10)
        {
            #region Test
            myData.Add("File", "Both"); // Write to file(Request, Response, Both)
            // == debug data ==================================================
            data = data.ToLower();
            // set the client name
            if (data.StartsWith("b")) myData.Add(dk.ClientName,"Box Office");
            else if (data.StartsWith("c")) myData.Add(dk.ClientName,"Casino");
            else if (data.StartsWith("d")) myData.Add(dk.ClientName,"Discount Shoe Warehouse");
            else if (data.StartsWith("e")) myData.Add(dk.ClientName,"Email Only");
            else if (data.StartsWith("f")) myData.Add(dk.ClientName,"Food City");
            else if (data.StartsWith("k")) myData.Add(dk.ClientName,"Kohl's");
            else if (data.StartsWith("m")) myData.Add(dk.ClientName, "Mobile Authentication Corporation");
            else if (data.StartsWith("t")) myData.Add(dk.ClientName,"Target #222");
            else if (data.StartsWith("s")) myData.Add(dk.ClientName,"Sam's Club");
            else if (data.StartsWith("w"))
            {
                myData.Add(dk.ClientName, data.StartsWith("wo") ? "Walmart Online" : "Walmart #303");
            }
            // === Debug End User ====================================
            myData.Add("data", data);
            if (data.Contains("10"))
            {
                myData.Add(dkui.FirstName, "Don");
                myData.Add(dkui.LastName, "Gleason");
                myData.Add(dkui.PhoneNumber, "4803304246");
                myData.Add(dkui.DOB, "02/06/1948");
                myData.Add(dkui.EmailAddress, "dgleason@mobileauthcorp.com");
                myData.Add(dkui.UID, "dgleason@mobileauthcorp.com");
                myData.Add(dkui.SSN4, "0369");
            }
            else if (data.Contains("1")) 
            {
                myData.Add(dkui.FirstName, "Terry");
                myData.Add(dkui.LastName, "Davis");
                myData.Add(dkui.PhoneNumber, "4802684076");
                myData.Add(dkui.DOB, "7/31/1946");
                myData.Add(dkui.EmailAddress, "tdavis@mobileauthcorp.com");
                myData.Add(dkui.UID, "tdavis@mobileauthcorp.com");
                myData.Add(dkui.SSN4, "4479");
                myData.Add(dkui.Street, "10091 East Buckskin Trail");
                myData.Add(dkui.City, "Scottsdale");
                myData.Add(dkui.State, "AZ");
                myData.Add(dkui.ZipCode, "85255");
            }
            else if (data.Contains("2")) 
            {
                myData.Add(dkui.FirstName, "Terry");
                myData.Add(dkui.LastName, "Davis");
                myData.Add(dkui.PhoneNumber, "4802684076");
                myData.Add(dkui.DOB, "7/31/1946");
                myData.Add(dkui.EmailAddress, "tdavis@mobileauthcorp.com");
                myData.Add(dkui.UID, "tdavis@mobileauthcorp.com");
                myData.Add(dkui.SSN4, "4479");
                myData.Add(dkui.ZipCode, "85255");
            }
            else 
            {
                return mUtils.FinalizeXmlResponseWithError("Debug: input error", mLogId);
            }

            var dt = DateTime.UtcNow;
            try
            {
                dt = DateTime.ParseExact(myData[dkui.DOB], "d", null);
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError("Debug: Could not parse DOB " + myData[dkui.DOB] + ", " + ex.Message + ". Invalid: " + dt, mLogId);
            }
            // debug: get client by name
            myClient = mUtils.GetClientUsingClientName(myData[dk.ClientName]);
            if (myClient == null)
                return mUtils.FinalizeXmlResponseWithError("Debug: Could not get client by name: " + myData[dk.ClientName], mLogId);
            myResponse.Append("<Debug>Run - Client" + myData[dk.ClientName] + "</Debug>");
            #endregion
        }
        else
        {

            var request = mUtils.GetIdDataFromRequest(data);
            if (String.IsNullOrEmpty(request.Item1))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null,
                    "Corrupt data!" + Environment.NewLine + data, "1");
            try
            {
                var sd = Security.DecodeAndDecrypt(request.Item2, request.Item1);
                myData.Clear();
                myData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(sd);
            }
            catch
            {
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1,
                    "Corrupt data!" + Environment.NewLine + data, "2");
            }
            myClient = mUtils.ValidateClient(request.Item1);
            if (myClient == null) return mUtils.EmptyXml();
        }
        // Change service name in dictionary
        myData.Remove(dk.ServiceName);
        myData.Add(dk.ServiceName, mSvcName);

        // log request if debug set in web.config
        mUtils.LogRequest(myData, data, mLogId);

        //Check Ip
        var mResult = mUtils.CheckClientIp(myClient);
        if (mResult.Item1 == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], mResult.Item2, null);

        foreach (var mVerificationProvider in myClient.VerificationProviders)
        {
            if (mVerificationProvider.Name == myData[dk.VerificationProviderName])
            {
                if (mVerificationProvider.Enabled == false)
                {
                    myResponse.Append("<" + sr.Reply + ">Verification, Disabled!</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }

                var mLoopback = ConfigurationManager.AppSettings[cfg.LoopBackTest];
                if (!String.IsNullOrEmpty(mLoopback))
                {
                    if (mLoopback != cfg.Disabled)
                    {
                        myResponse.Append("<" + sr.Reply + ">Verification, Disabled!</" + sr.Reply + ">");
                        return mUtils.FinalizeXmlResponse(myResponse, "WP");
                    }
                }

                var myReqPara = new Dictionary<string, string>();
                if (String.IsNullOrEmpty(mVerificationProvider.RequestParameters) == false)
                {
                    if (mUtils.ParseIntoDictionary(mVerificationProvider.RequestParameters, myReqPara, char.Parse(dk.KVSep)) == false)
                        myReqPara.Clear();
                }
                if (myReqPara.ContainsKey(Constants.LexisNexis.GLBPurpose) == false) myReqPara.Add(Constants.LexisNexis.GLBPurpose, "5");
                if (myReqPara.ContainsKey(Constants.LexisNexis.DLPurpose) == false) myReqPara.Add(Constants.LexisNexis.DLPurpose, "3");
                if (myReqPara.ContainsKey(Constants.LexisNexis.ReturReturnCoun) == false) myReqPara.Add(Constants.LexisNexis.ReturReturnCoun, "1");
                if (myReqPara.ContainsKey(Constants.LexisNexis.StartingRecord) == false) myReqPara.Add(Constants.LexisNexis.StartingRecord, "1");
                if (myReqPara.ContainsKey(Constants.LexisNexis.UseDOBFilter) == false) myReqPara.Add(Constants.LexisNexis.UseDOBFilter, "1");
                if (myReqPara.ContainsKey(Constants.LexisNexis.DOBRadius) == false) myReqPara.Add(Constants.LexisNexis.DOBRadius, "1");

                //var myProcPara = new Dictionary<string, string>();
                //if (mUtils.ParseIntoDictionary(myClient.VerificationProviders.ProcessingParameters, myProcPara, '='))
                //    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myClient.ClientId.ToString(), 
                //        "No Request paramenters for " + myClient.VerificationProviders.Name + ", " + myClient.Name, "5");

                myData.Add(dk.Debug, ConfigurationManager.AppSettings["UserVerificationLexisNexisDebugOptions"]);
                myData.Add(Constants.LexisNexis.LogFile, ConfigurationManager.AppSettings["UserVerificationLexisNexisLogToFile"]);
                //var ClientId = myClient._id.ToString();
                //var myVerificationEvent = new EndUserEvent("");
                var soapRequest = new StringBuilder();
                /******* Soap xml format **************************************************
                <Envelope>
                    <Body>
                       <InstantIDRequest>
                         <User>	 
                            <!-- ReferenceCode is returned in your billing statement. -->
                            <!-- BillingCode replaces user’s login name in billing details -->
                            <!-- QueryId is returned in results. -->
                           <ReferenceCode>Ajax Corporation</ReferenceCode>
                           <BillingCode>Emily Kate</BillingCode>
                           <QueryId>Ajax123</QueryId>
                           <GLBPurpose>1</GLBPurpose>
                           <DLPurpose>1</DLPurpose>
                           <EndUser>
                             <CompanyName>Ajax Corporation</CompanyName>
                             <StreetAddress1>3003 NW Constance Rd</StreetAddress1>
                             <City>Cocoplum</City>
                             <State>FL</State>
                             <Zip5>33442</Zip5>
                           </EndUser>
                         </User>
                         <Options>
                           <Watchlists>
                              <Watchlist>OFAC</Watchlist>
                              <Watchlist>FBI</Watchlist>
                           </Watchlists>
                           <IncludeCLOverride>0</IncludeCLOverride>
                           <IncludeMSOverride>0</IncludeMSOverride>
                           <IncludeDLVerification>0<IncludeDLVerification>
                           <PoBoxCompliance>1</PoBoxCompliance>
                            <!-- Default threshold is .84. This example overrides the default. -->
                            <GlobalWatchlistThreshold>.85</GlobalWatchlistThreshold>
                           <DOBMatch>
                             <MatchType>FuzzyCCYYMM</MatchType>
                             <MatchYearRadius></MatchYearRadius>
                           </DOBMatch>
                           <IncludeModels>
                             <FraudPointModel>
                               <ModelName>FP1109_0</ModelName>
                               <IncludeRiskIndices>true</IncludeRiskIndices>
                             </FraudPointModel>
                            </IncludeModels>
                            <RedFlagsReport>Version1</RedFlagsReport>
                           <IncludeAllRiskIndicators>0</IncludeAllRiskIndicators>   

                    LexisNexis® Verification and Fraud Prevention Services Web Service Interface Guide  
                    17  This document contains Confidential and Proprietary Information of LexisNexis Risk Solutions
                    and shall not be used for purposes other than as intended and disclosed.

                          <RequireExactMatch>
                             <LastName>0</LastName>
                             <FirstName>0</FirstName>
                             <FirstNameAllowNickname>0</FirstNameAllowNickname>
                             <Address>0</Address>
                             <HomePhone>0</HomePhone>
                             <SSN>0</SSN>
                           </RequireExactMatch>
                         </Options>
                        <SearchBy>
                            <UseDOBFilter>1</UseDOBFilter>
                            <DOBRadius>3</DOBRadius>
                                    <!-- Use either unparsed format or parsed format to submit a Name  -->
                                    <!-- You shouldn’t use both. If you submit both, only unparsed form is considered -->
                                    <!-- unparsed format -->
                            <Name>
                                <Full>JOHN HENRY DOE JR</Full>
                            </Name>
                            <!-- parsed format -->
                            <Name>
                                    <First>JOHN</First>
                                    <Middle>HENRY</Middle>
                                    <Last>DOE</Last>
                                    <Suffix>JR</Suffix>
                            </Name> 
                                    <!-- Use either unparsed format or parsed format to submit an address --> 
                                    <!-- You can also use any combination that does not provide redundant input --> 
                                    <!-- unparsed format -->
                            <Address>
                                    <StreetAddress1>4711 NW BRONTE WAY</StreetAddress1>
                                    <StreetAddress2>APT B11</StreetAddress2>
                                    <StateCityZip>DEERFIELD BEACH, FL 33442</StateCityZip>
                            </Address> 
                            <!-- parsed format -->
                            <Address>
                                <StreetName>BRONTE</StreetName>
                                <StreetNumber>4711</StreetNumber>
                                <StreetPreDirection>NW</StreetPreDirection>
                                <StreetSuffix>WAY</StreetSuffix>
                                <UnitDesignation>APT</UnitDesignation>
                                <UnitNumber>B11</UnitNumber>
                                <State>FL</State>
                                <City>DEERFIELD BEACH</City>
                                <Zip5>33442</Zip5>
                            </Address>
                            <Age>55</Age>
                            <DOB>
                                <Year>1955</Year>
                                <Month>07</Month>
                                <Day>06</Day>
                            </DOB> 
                            <!-- use either SSN or SSNLast4, not both. SSN should not contain dashes -->
                            <SSN>000456789</SSN>
                            <SSNLast4>6789</SSNLast4>
                            <DriverLicenseNumber>D120240661060<DriverLicenseNumber>
                            <DriverLicenseState>FL<DriverLicenseState>
                            <HomePhone>9545552222</HomePhone>
                            <WorkPhone>5615559999</WorkPhone>

                        LexisNexis® Verification and Fraud Prevention Services Web Service Interface Guide
                        This document contains Confidential and Proprietary Information of LexisNexis Risk Solutions
                        and shall not be used for purposes other than as intended and disclosed.

                            <Passport>
                                <Number>01234567890</Number>
                                <ExpirationDate>
                                <Year>2012</Year>
                                <Month>03</Month>
                                <Day>30</Day>
                                </ExpirationDate>
                                <Country>SWEDEN</Country>
                                <MachineReadableLine1> 
                                    J&lt;IRLHENRY&lt;DOE&lt;ANGUS&lt;&lt;&lt;&lt;&lt;&lt;&lt;&lt;&lt;&lt;&lt;&lt;&lt;&lt;& lt;&lt;&lt;&lt;&lt;&lt;&lt;&lt;&lt;&lt;
                                </MachineReadableLine1>
                                <MachineReadableLine2>
                                    1234567897IRL1203309M2010245ABCD1234&lt;&lt;1A2B40
                                </MachineReadableLine2>
                            </Passport>
                            <Gender>M</Gender
                            <Email>JIMBO@EXAMPLE.COM</Email>
                                    <!-- Valid values for Channel are: -->
                                    <!-- Mail, PointOfSale,.Kiosk, Internet, Branch, Telephonic, or Other-->
                            <Channel>Internet</Channel>
                            <Income>76000</Income>
                                <!-- Valid values for OwnOrRent are: Own or Rent -->
                            <OwnOrRent>Own</OwnOrRent>
                            <ApplicationDateTime>
                                <Year>2012</Year>
                                <Month>03</Month>
                                <Day>30</Day>
                                <Hour24>04</Hour24>
                                <Minute>21</Minute>
                                <Second>23</Second>
                            </ApplicationDateTime>
                         </SearchBy>
                       </InstantIDRequest>
                    </Body> 
                </Envelope> 
                *******************************************************************/
                // Build the SOAP Request XML
                soapRequest.Append("<Envelope><Body><InstantIDRequest>");
                soapRequest.Append("<GLBPurpose>" + myReqPara[Constants.LexisNexis.GLBPurpose] + "</GLBPurpose>");
                soapRequest.Append("<DLPurpose>" + myReqPara[Constants.LexisNexis.DLPurpose] + "</DLPurpose>");

                soapRequest.Append("<Options>");
                soapRequest.Append("<IncludeAllRiskIndicators>1</IncludeAllRiskIndicators>");
                //soapRequest.Append("<RequireExactMatch>");
                //soapRequest.Append("<LastName>1</LastName>");
                //soapRequest.Append("<FirstName>1</FirstName>");
                //soapRequest.Append("<FirstNameAllowNickname>1</FirstNameAllowNickname>");
                //soapRequest.Append("</RequireExactMatch>");
                if ((myData.ContainsKey(dkui.DriverLic)) && (myData.ContainsKey(dkui.DriverLicSt)))
                    soapRequest.Append("<IncludeDLVerification>1</IncludeDLVerification>");
                //if (myData.ContainsKey(dk.DOB))
                //    soapRequest.Append("<MatchType>ExactCCYYMMDD</MatchType>");
                soapRequest.Append("</Options>");

                soapRequest.Append("<SearchBy>");

                //<Name>
                //  <First>JOHN</First>
                //  <Middle>HENRY</Middle>
                //  <Last>DOE</Last>
                //  <Suffix>JR</Suffix>
                //</Name>
                soapRequest.Append("<Name>");
                if (myData.ContainsKey(dkui.FirstName))
                    soapRequest.Append("<First>" + myData[dkui.FirstName] + "</First>");

                if (myData.ContainsKey(dkui.MiddleName))
                    soapRequest.Append("<Middle>" + myData[dkui.MiddleName] + "</Middle>");

                if (myData.ContainsKey(dkui.LastName))
                    soapRequest.Append("<Last>" + myData[dkui.LastName] + "</Last>");

                if (myData.ContainsKey(dkui.Suffix))
                    soapRequest.Append("<Suffix>" + myData[dkui.Suffix] + "</Suffix>");
                soapRequest.Append("</Name>");

                //<Address>
                //  <StreetAddress1>4711 NW BRONTE WAY</StreetAddress1>
                //  <StreetAddress2>APT B11</StreetAddress2>
                //  <StateCityZip>DEERFIELD BEACH, FL 33442</StateCityZip>
                //</Address> 
                soapRequest.Append("<Address>");
                if (myData.ContainsKey(dkui.Street))
                    soapRequest.Append("<StreetAddress1>" + myData[dkui.Street].ToUpper() + "</StreetAddress1>");

                if (myData.ContainsKey(dkui.Unit))
                    soapRequest.Append("<StreetAddress2>" + myData[dkui.Unit].ToUpper() + "</StreetAddress2>");

                if ((myData.ContainsKey(dkui.City)) && (myData.ContainsKey(dkui.State)) && (myData.ContainsKey(dkui.ZipCode)))
                {
                    soapRequest.Append("<StateCityZip>" + myData[dkui.City].ToUpper());
                    soapRequest.Append(", " + myData[dkui.State].ToUpper());
                    soapRequest.Append(" " + myData[dkui.ZipCode] + "</StateCityZip>");
                }
                else
                {   // <State>FL</State>
                    // <City>DEERFIELD BEACH</City>
                    // <Zip5>33442</Zip5>
                    if (myData.ContainsKey(dkui.State))
                        soapRequest.Append("<State>" + myData[dkui.State].ToUpper() + "</State>");
                    if (myData.ContainsKey(dkui.City))
                        soapRequest.Append("<City>" + myData[dkui.City].ToUpper() + "</City>");
                    if (myData.ContainsKey(dkui.ZipCode))
                        soapRequest.Append("<ZIP>" + myData[dkui.ZipCode].ToUpper() + "</ZIP>");
                }
                soapRequest.Append("</Address>");

                // <DOB>
                //  <Year>1955</Year>
                //  <Month>07</Month>
                //  <Day>06</Day>
                //</DOB>
                if (myData.ContainsKey(dkui.DOB))
                {
                    var dt = DateTime.ParseExact(myData[dkui.DOB], "d", null);
                    soapRequest.Append("<UseDOBFilter>" + myReqPara[Constants.LexisNexis.UseDOBFilter] + "</UseDOBFilter>");
                    soapRequest.Append("<DOBRadius>" + myReqPara[Constants.LexisNexis.DOBRadius] + "</DOBRadius>");
                    soapRequest.Append("<DOB><Year>" + dt.ToString("yyyy") + "</Year><Month>" + dt.ToString("MM") + "</Month><Day>" + dt.ToString("dd") + "</Day></DOB>");
                }

                if (myData.ContainsKey(dkui.SSN4))
                {
                    soapRequest.Append("<SSNLast4>" + myData[dkui.SSN4] + "</SSNLast4>");
                }

                // <DriverLicenseNumber>D120240661060</DriverLicenseNumber>
                // <DriverLicenseState>FL</DriverLicenseState> 
                if ((myData.ContainsKey(dkui.DriverLic)) && (myData.ContainsKey(dkui.DriverLicSt)))
                {
                    soapRequest.Append("<DriverLicenseNumber>" + myData[dkui.DriverLic].ToUpper() + "</DriverLicenseNumber>");
                    soapRequest.Append("<DriverLicenseState>" + myData[dkui.DriverLicSt].ToUpper() + "</DriverLicenseState>");
                }

                if (myData.ContainsKey(dkui.PhoneNumber))
                {
                    soapRequest.Append("<HomePhone>" + myData[dkui.PhoneNumber] + "</HomePhone>");
                }

                // <Email>JIMBO@EXAMPLE.COM</Email>
                // <Channel>Internet</Channel>
                if (myData.ContainsKey(dkui.EmailAddress))
                {
                    soapRequest.Append("<Email>" + myData[dkui.EmailAddress] + "</Email>");
                    soapRequest.Append("<Channel>Internet</Channel>");
                }

                soapRequest.Append("</SearchBy></InstantIDRequest></Body></Envelope>");

                if (myData.ContainsKey(dk.Debug))
                {
                    /*Debug*/
                    var sSoap = soapRequest.ToString();
                    /*Debug*/
                    //var dCred = myClient.VerificationProviders.Login + ":" + myClient.VerificationProviders.Password;
                    /*Debug*/
                    //var dUrl = myClient.VerificationProviders.BaseUrl + "?ver_=" + myClient.VerificationProviders.ApiVersion;

                    if (myData[dk.Debug].Contains(Constants.LexisNexis.LogReq))
                    {
                        if (myData.ContainsKey(Constants.LexisNexis.LogFile))
                        {
                            var xmldoc = new XmlDocument();
                            xmldoc.LoadXml(sSoap);
                            xmldoc.Save(myData[Constants.LexisNexis.LogFile] + "/" + mSvcName + "_Request_" + myData[dkui.FirstName] + myData[dkui.LastName] + ".xml");
                        }
                        //var logEvent = new Event
                        //{
                        //    ClientId = myClient._id
                        //    //EventTypeDesc = "User Verification (LexisNexis Request)",
                        //    //Details = "<![CDATA[" + sSoap + "]]>"
                        //};
                        //logEvent.Create();
                    }

                    if (myData[dk.Debug].ToLower().Contains("bypassissue"))
                    {
                        {
                            //var logEvent = new Event
                            //{
                            //    ClientId = myClient._id,
                            //    EventTypeDesc = "User Verification (LexisNexis Response)"
                            //    //Details = "Debug: Bypass Issue Set (" + myData[dk.Debug] + ")"
                            //};
                            //logEvent.Create();
                        }
                        return mUtils.FinalizeXmlResponseWithError("<Request>" + sSoap + "</Request>", mLogId);
                    }
                }

                // Create and make the request
                try
                {
                    var url = mVerificationProvider.BaseUrl + "?ver_=" + mVerificationProvider.ApiVersion;
                    var req = (HttpWebRequest)WebRequest.Create(url);
                    req.Headers.Add("SOAPAction", "\"http://tempuri.org/Register\"");
                    req.ContentType = "text/xml;charset=\"utf-8\"";
                    req.Accept = "text/xml";
                    req.Method = "POST";
                    req.PreAuthenticate = true;
                    req.UseDefaultCredentials = false;
                    req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(
                                    mVerificationProvider.Login + ":" + mVerificationProvider.Password));
                    using (var stm = req.GetRequestStream())
                    {
                        using (var stmw = new StreamWriter(stm))
                        {
                            stmw.Write(@"<?xml version=""1.0"" encoding=""utf-8"" ?>" + soapRequest);
                        }
                    }
                    var response = req.GetResponse();
                    var responseStream = response.GetResponseStream();
                    var xmlDoc = new XmlDocument();
                    if (responseStream != null) xmlDoc.Load(responseStream);
                    var resp = xmlDoc.InnerXml;
                    // <?xml version="1.0" encoding="utf-8"?>
                    var startposition = resp.IndexOf("?>", StringComparison.Ordinal) + 2;

                    if (myData.ContainsKey(dk.Debug))
                    {
                        if (myData[dk.Debug].Contains(Constants.LexisNexis.LogResp))
                        {
                            if (myData.ContainsKey(Constants.LexisNexis.LogFile))
                            {
                                xmlDoc.Save(myData[Constants.LexisNexis.LogFile] + "/" + mSvcName + "_Response_" + myData[dkui.FirstName] + myData[dkui.LastName] + ".xml");
                            }

                            var logEvent = new Event
                            {
                                ClientId = myClient._id
                                //EventTypeDesc = "User Verification (LexisNexis Response)",
                                //Details = "<![CDATA[" + resp.Substring(startposition, resp.Length - startposition) + "]]>"
                            };
                            logEvent.Create();
                        }
                    }

                    //TODO: need to add logic to return:
                    //  error reenter
                    //  need aditional info
                    //  ask questions

                    myResponse.Append("<" + sr.Reply + ">Verification, " + resp.Substring(startposition, resp.Length - startposition) + "<" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
                catch (Exception ex)
                {
                    var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    if (myData.ContainsKey(dk.ClientName))
                        exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                    myResponse.Append("<" + sr.Reply + ">Verification, Web Request failed, exception:" +
                        ex.Message + "!</" + sr.Reply + ">");   
                }
            }
        }
        myResponse.Append("<" + sr.Reply + ">Verification, Failed verification provider lookup /Reply>");
        return mUtils.FinalizeXmlResponse(myResponse, mLogId);

    }

    public static bool ValidateServerCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

}