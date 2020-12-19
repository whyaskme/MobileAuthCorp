using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.Services;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
 
[System.Web.Script.Services.ScriptService]
public class ManageTypeDefsService: WebService {

    [WebMethod]
    public XmlDocument WsManageTypeDefsService(string data)
    {
        var mUtils = new Utils();

        var myData = new Dictionary<string, string> {{dk.ServiceName, "ManageTypeDefsService"}};

        //myData.Add(dk.Debug, "DisableBoth");
        //myData.Add(dk.Debug, "DisableUpdate");
        //myData.Add(dk.Debug, "DisableNew");

        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        var request = mUtils.GetIdDataFromRequest(data);
        if (mUtils.DecryptAndParseRequestData(request.Item1, request.Item2, myData, char.Parse(dk.KVSep)) == false)
            return mUtils.EventLogError_FinalizeXmlResponse(myData[dk.ServiceName],
                null, "Corrupt or bad request data!" + Environment.NewLine + data, null);
        if (myData.ContainsKey(dk.Request) == false)
            return mUtils.EventLogError_FinalizeXmlResponse(myData[dk.ServiceName],
                null, "No request!" + Environment.NewLine + data, null);

        // log request if debug set in web.config
        mUtils.LogRequest(myData, data, "MT");
 
        try
        {
            bool objChanged;
            switch (myData[dk.Request])
            {
                case dv.GetListOfTypes:

                    var myTypeDefs = mUtils.GetArrayOfTypeDefs();
                    if (myTypeDefs.Any())
                    {
                        myResponse.Append("<TypeDefinitions>");
                        foreach (var td in myTypeDefs)
                        {
                            myResponse.Append("<TypeDefinition>" + td + "</TypeDefinition>");
                        }
                        myResponse.Append("</TypeDefinitions>");
                        return mUtils.FinalizeXmlResponse(myResponse, "MT");
                    }
                    return mUtils.EventLogError_FinalizeXmlResponse(myData[dk.ServiceName],
                        null, "internal error", null);

                case dv.GetListByName:
                    mUtils.GetListByTypeDefName(myData[dk.Name], myResponse);
                    return mUtils.FinalizeXmlResponse(myResponse, "MT");

                case dv.UpdateTypeByName:

                    if (myData.ContainsKey(dk.Debug))
                    {
                        if ((myData[dk.Debug] == "DisableUpdate") || (myData[dk.Debug] == "DisableBoth"))
                        {
                            return mUtils.FinalizeXmlResponseWithError(myData[dk.ServiceName] + ".NewTypeByName: disabled!", "MT");
                        }
                    }
                    var resp = "";
                    switch (myData[dk.TypeDef])
                    {
                        case "Country":
                            var myC = (Country)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (myC == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            objChanged = UpdateCountryWithDict(myC, myData);
                            if (objChanged)
                                resp = mUtils.UpdateTypeDef(myC, myC._id.ToString(), myData[dk.TypeDef]);
                            break;

                        case "RegistrationType":
                            var myRt = (RegistrationType)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (myRt == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            objChanged = UpdateRegistrationTypeWithDict(myRt, myData);
                            if (objChanged)
                                resp = mUtils.UpdateTypeDef(myRt, myRt._id.ToString(), myData[dk.TypeDef]);
                            break;

                        case "State":
                            var mySt = (State)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (mySt == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            objChanged = UpdateStateWithDict(mySt, myData);
                            if (objChanged)
                                resp = mUtils.UpdateTypeDef(mySt, mySt._id.ToString(), myData[dk.TypeDef]);
                            break;

                        case "ProviderSms":
                            var mySms = (ProviderSms)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (mySms == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            objChanged = UpdateSmsWithDict(mySms, myData);
                            if (objChanged)
                                resp = mUtils.UpdateTypeDef(mySms, mySms._id.ToString(), myData[dk.TypeDef]);
                            break;

                        case "ProviderEmail":
                            var myEmail = (ProviderEmail)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (myEmail == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            objChanged = UpdateEmailWithDict(myEmail, myData);
                            if (objChanged)
                                resp = mUtils.UpdateTypeDef(myEmail, myEmail._id.ToString(), myData[dk.TypeDef]);
                            break;

                        case "ProviderVoice":
                            var myVoice = (ProviderVoice)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (myVoice == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            objChanged = UpdateVoiceWithDict(myVoice, myData);
                            if (objChanged)
                                resp = mUtils.UpdateTypeDef(myVoice, myVoice._id.ToString(), myData[dk.TypeDef]);
                            break;

                        case "VerificationProviders":
                            var myVp = (VerificationProvider)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (myVp == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            objChanged = UpdateUserVerificationProviderWithDict(myVp, myData);
                            if (objChanged)
                                resp = mUtils.UpdateTypeDef(myVp, myVp._id.ToString(), myData[dk.TypeDef]);
                            break;
                        default:
                            return mUtils.FinalizeXmlResponseWithError("Invalid TypeDefinitions " + myData[dk.TypeDef], "MT");
                    }
                    if (!resp.Contains("failed"))
                    {
                        mUtils.GetListByTypeDefName(myData[dk.TypeDefName], myResponse);
                    }
                    else
                    {
                        return mUtils.FinalizeXmlResponseWithError(resp, "MT");
                    }
                    return mUtils.FinalizeXmlResponse(myResponse, "MT");

                case dv.NewTypeByName:
                {
                    if (myData.ContainsKey(dk.Debug))
                    {
                        if ((myData[dk.Debug] == "DisableNew") || (myData[dk.Debug] == "DisableBoth"))
                        {
                            return mUtils.FinalizeXmlResponseWithError(myData[dk.ServiceName] + ".NewTypeByName: disabled!", "MT");
                        }
                    }
                    string result;
                    switch (myData[dk.TypeDef])
                    {
                        case "Country":
                            var myC = (Country)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (myC == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            // must change the name
                            if (myData.ContainsKey(dk.Name) == false)
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");
                            // Name must be different
                            if (myC.Name == HexAsciiConvert(myData[dk.Name]))
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");
                        
                            objChanged = UpdateCountryWithDict(myC, myData);
                            if (objChanged)
                            {
                                mUtils.TypeDefsSetNewObjectId(myC, myData[dk.TypeDef]);
                                result = mUtils.CreateTypeDef(myC, myData[dk.TypeDef]);
                                if (result == null)
                                    return mUtils.FinalizeXmlResponseWithError("Create new " + myData[dk.TypeDef] + "failed", "MT");
                            }
                            break;

                        case "RegistrationType":
                            var myRt = (RegistrationType)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (myRt == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            // must change the name
                            if (myData.ContainsKey(dk.Name) == false)
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");
                            // Name must be different
                            if (myRt.Name == HexAsciiConvert(myData[dk.Name]))
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");
                        
                            objChanged = UpdateRegistrationTypeWithDict(myRt, myData);
                            if (objChanged)
                            {
                                mUtils.TypeDefsSetNewObjectId(myRt, myData[dk.TypeDef]);
                                result = mUtils.CreateTypeDef(myRt, myData[dk.TypeDef]);
                                if (result == null)
                                    return mUtils.FinalizeXmlResponseWithError("Create new " + myData[dk.TypeDef] + "failed", "MT");
                            }
                            break;

                        case "State":
                            var mySt = (State)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (mySt == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            // must change the name
                            if (myData.ContainsKey(dk.Name) == false)
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");
                            // Name must be different
                            if (mySt.Name == HexAsciiConvert(myData[dk.Name]))
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");
                        
                            objChanged = UpdateStateWithDict(mySt, myData);
                            if (objChanged)
                            {
                                mUtils.TypeDefsSetNewObjectId(mySt, myData[dk.TypeDef]);
                                result = mUtils.CreateTypeDef(mySt, myData[dk.TypeDef]);
                                if (result == null)
                                    return mUtils.FinalizeXmlResponseWithError("Create new " + myData[dk.TypeDef] + "failed", "MT");
                            }
                            break;

                        case "ProviderSms":
                            var mySms = (ProviderSms)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (mySms == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            // must change the name
                            if (myData.ContainsKey(dk.Name) == false)
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");
                            // Name must be different
                            if (mySms.Name == HexAsciiConvert(myData[dk.Name]))
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");

                            objChanged = UpdateSmsWithDict(mySms, myData);
                            if (objChanged)
                            {
                                mUtils.TypeDefsSetNewObjectId(mySms, myData[dk.TypeDef]);
                                result = mUtils.CreateTypeDef(mySms, myData[dk.TypeDef]);
                                if (result == null)
                                    return mUtils.FinalizeXmlResponseWithError("Create new " + myData[dk.TypeDef] + "failed", "MT");
                            }
                            break;

                        case "ProviderEmail":
                            var myEmail = (ProviderEmail)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (myEmail == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            // must change the name
                            if (myData.ContainsKey(dk.Name) == false)
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");
                            // Name must be different
                            if (myEmail.Name == HexAsciiConvert(myData[dk.Name]))
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");

                            objChanged = UpdateEmailWithDict(myEmail, myData);
                            if (objChanged)
                            {
                                mUtils.TypeDefsSetNewObjectId(myEmail, myData[dk.TypeDef]);
                                result = mUtils.CreateTypeDef(myEmail, myData[dk.TypeDef]);
                                if (result == null)
                                    return mUtils.FinalizeXmlResponseWithError("Create new " + myData[dk.TypeDef] + "failed", "MT");
                            }
                            break;

                        case "ProviderVoice":
                            var myVoice = (ProviderVoice)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (myVoice == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            // must change the name
                            if (myData.ContainsKey(dk.Name) == false)
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");
                            // Name must be different
                            if (myVoice.Name == HexAsciiConvert(myData[dk.Name]))
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");

                            objChanged = UpdateVoiceWithDict(myVoice, myData);
                            if (objChanged)
                            {
                                mUtils.TypeDefsSetNewObjectId(myVoice, myData[dk.TypeDef]);
                                result = mUtils.CreateTypeDef(myVoice, myData[dk.TypeDef]);
                                if (result == null)
                                    return mUtils.FinalizeXmlResponseWithError("Create new " + myData[dk.TypeDef] + "failed", "MT");
                            }
                            break;

                        case "VerificationProviders":
                            var myVp = (VerificationProvider)mUtils.GetTypeDefByTypeAndName(myData[dk.TypeDef], myData[dk.TypeDefName]);
                            if (myVp == null)
                                return mUtils.FinalizeXmlResponseWithError("No TypeDefinitions " + myData[dk.TypeDef] + " with the name of " + myData[dk.TypeDefName], "MT");
                            // must change the name
                            if (myData.ContainsKey(dk.Name) == false)
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");
                            // Name must be different
                            if (myVp.Name == HexAsciiConvert(myData[dk.Name]))
                                return mUtils.FinalizeXmlResponseWithError("Error: can't create new " + myData[dk.TypeDefName] + " with the same name!", "MT");
                        
                            objChanged = UpdateUserVerificationProviderWithDict(myVp, myData);
                            if (objChanged)
                            {
                                mUtils.TypeDefsSetNewObjectId(myVp, myData[dk.TypeDef]);
                                result = mUtils.CreateTypeDef(myVp, myData[dk.TypeDef]);
                                if (result == null)
                                    return mUtils.FinalizeXmlResponseWithError("Create new " + myData[dk.TypeDef] + "failed", "MT");
                            }
                            break;
                        default:
                            return mUtils.FinalizeXmlResponseWithError("Invalid TypeDefinitions " + myData[dk.TypeDef], "MT");
                    }
                    mUtils.GetListByTypeDefName(myData[dk.TypeDefName], myResponse);
                    return mUtils.FinalizeXmlResponse(myResponse, "MT");
                }
                default:
                    return mUtils.EventLogError_FinalizeXmlResponse(myData[dk.ServiceName],
                        null,
                        "Invalid reqiest", null);
            }
        }
        catch (Exception ex)
        {
            var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
            var exceptionEvent = new Event
            {
                EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
            };
            exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

            return mUtils.EventLogError_FinalizeXmlResponse(myData[dk.ServiceName],
                null,
                "Exception: " + ex.Message, null);
        }
    }

    #region Object Updaters
    private bool UpdateCountryWithDict(Country ct, Dictionary<String, String> myData)
    {
        const string property = dk.Name;
        if (!myData.ContainsKey(property)) return false;
        ct.Name = HexAsciiConvert(myData[property]);
        return true;
    }

    private bool UpdateRegistrationTypeWithDict(RegistrationType rt, Dictionary<String, String> myData)
    {
        var objChanged = false;
        const string property = dk.Name;
        if (myData.ContainsKey(property))
        {
            rt.Name = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        if (myData.ContainsKey(dk.Description))
        {
            rt.Description = HexAsciiConvert(myData[dk.Description]);
            objChanged = true;
        }
        return objChanged;
    }

    private bool UpdateStateWithDict(State st, Dictionary<String, String> myData)
    {
        var objChanged = false;
        const string property = dk.Name;
        if (myData.ContainsKey(property))
        {
            st.Name = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        if (myData.ContainsKey(dk.Description))
        {
            st.Description = HexAsciiConvert(myData[dk.Description]);
            objChanged = true;
        }
        if (myData.ContainsKey(dk.Abbreviation))
        {
            st.Abbreviation = HexAsciiConvert(myData[dk.Abbreviation]);
            objChanged = true;
        }
        return objChanged;
    }

    private bool UpdateUserVerificationProviderWithDict(VerificationProvider myVp, Dictionary<String, String> myData)
    {
        bool objChanged = false;
        string property = dk.Name;
        if (myData.ContainsKey(property))
        {
            myVp.Name = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Enabled";
        if (myData.ContainsKey(property))
        {
            myVp.Enabled = bool.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "ApiKey";
        if (myData.ContainsKey(property))
        {
            myVp.ApiKey = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "ApiVersion";
        if (myData.ContainsKey(property))
        {
            myVp.ApiVersion = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "BaseUrl";
        if (myData.ContainsKey(property))
        {
            myVp.BaseUrl = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Login";
        if (myData.ContainsKey(property))
        {
            myVp.Login = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Password";
        if (myData.ContainsKey(property))
        {
            myVp.Password = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "ProcessingParameters";
        if (myData.ContainsKey(property))
        {
            myVp.ProcessingParameters = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Protocol";
        if (myData.ContainsKey(property))
        {
            myVp.Protocol = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "ProxyHost";
        if (myData.ContainsKey(property))
        {
            myVp.ProxyHost = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "ProxyPort";
        if (myData.ContainsKey(property))
        {
            myVp.ProxyPort = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "RequestParameters";
        if (myData.ContainsKey(property))
        {
            myVp.RequestParameters = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "SearchType";
        if (myData.ContainsKey(property))
        {
            myVp.SearchType = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = dk.ServiceName;
        if (myData.ContainsKey(property))
        {
            myVp.ServiceName = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        return objChanged;
    }

    private bool UpdateEmailWithDict(ProviderEmail myEmail, Dictionary<String, String> myData)
    {
        bool objChanged = false;
        string property = dk.Name;
        if (myData.ContainsKey(property))
        {
            myEmail.Name = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Enabled";
        if (myData.ContainsKey(property))
        {
            myEmail.Enabled = bool.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "ProviderCharge";
        if (myData.ContainsKey(property))
        {
            myEmail.ProviderCharge = Double.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "ClientCharge";
        if (myData.ContainsKey(property))
        {
            myEmail.ClientCharge = Double.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "FromEmail";
        if (myData.ContainsKey(property))
        {
            myEmail.FromEmail = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "LoginUsername";
        if (myData.ContainsKey(property))
        {
            myEmail.LoginUserName = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "LoginPassword";
        if (myData.ContainsKey(property))
        {
            myEmail.LoginPassword = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Port";
        if (myData.ContainsKey(property))
        {
            //myEmail.Port = int.Parse(HexAsciiConvert(myData[property]));
            myEmail.Port = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "NewlineReplacement";
        if (myData.ContainsKey(property))
        {
            myEmail.NewlineReplacement = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "RequiresSsl";
        if (myData.ContainsKey(property))
        {
            myEmail.RequiresSsl = bool.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "CredentialsRequired";
        if (myData.ContainsKey(property))
        {
            myEmail.CredentialsRequired = bool.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "IsBodyHtml";
        if (myData.ContainsKey(property))
        {
            myEmail.IsBodyHtml = bool.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "Server";
        if (myData.ContainsKey(property))
        {
            myEmail.Server = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "OtpSubject";
        if (myData.ContainsKey(property))
        {
            myEmail.OtpSubject = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "NotficationSubject";
        if (myData.ContainsKey(property))
        {
            myEmail.NotficationSubject = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "AdminNotificationOnFailure";
        if (!myData.ContainsKey(property)) return objChanged;
        myEmail.AdminNotificationOnFailure = bool.Parse(HexAsciiConvert(myData[property]));
        objChanged = true;
        return objChanged;
    }

    private bool UpdateSmsWithDict(ProviderSms mySms, Dictionary<String, String> myData)
    {
        var objChanged = false;
        var property = dk.Name;
        if (myData.ContainsKey(property))
        {
            mySms.Name = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Enabled";
        if (myData.ContainsKey(property))
        {
            mySms.Enabled = bool.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "ProviderCharge";
        if (myData.ContainsKey(property))
        {
            mySms.ProviderCharge = Double.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "ClientCharge";
        if (myData.ContainsKey(property))
        {
            mySms.ClientCharge = Double.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "Url";
        if (myData.ContainsKey(property))
        {
            mySms.Url = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Sid";
        if (myData.ContainsKey(property))
        {
            mySms.Sid = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "AuthToken";
        if (myData.ContainsKey(property))
        {
            mySms.AuthToken = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "ShortCodeFromNumber";
        if (myData.ContainsKey(property))
        {
            mySms.ShortCodeFromNumber = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "ApiVersion";
        if (myData.ContainsKey(property))
        {
            mySms.ApiVersion = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Key";
        if (myData.ContainsKey(property))
        {
            mySms.Key = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Server";
        if (myData.ContainsKey(property))
        {
            mySms.Server = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Port";
        if (myData.ContainsKey(property))
        {
            mySms.Port = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Protocol";
        if (myData.ContainsKey(property))
        {
            mySms.Protocol = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "RequiresSsl";
        if (myData.ContainsKey(property))
        {
            mySms.RequiresSsl = bool.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "LoginUsername";
        if (myData.ContainsKey(property))
        {
            mySms.LoginUsername = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "LoginPassword";
        if (myData.ContainsKey(property))
        {
            mySms.LoginPassword = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "NewlineReplacement";
        if (myData.ContainsKey(property))
        {
            mySms.NewlineReplacement = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "PhoneNumberFormat";
        if (myData.ContainsKey(property))
        {
            mySms.PhoneNumberFormat = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "VoiceToken";
        if (myData.ContainsKey(property))
        {
            mySms.VoiceToken = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        return objChanged;
    }

    private bool UpdateVoiceWithDict(ProviderVoice myVoice, Dictionary<String, String> myData)
    {
        bool objChanged = false;
        string property = dk.Name;
        if (myData.ContainsKey(property))
        {
            myVoice.Name = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Enabled";
        if (myData.ContainsKey(property))
        {
            myVoice.Enabled = bool.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "ProviderCharge";
        if (myData.ContainsKey(property))
        {
            myVoice.ProviderCharge = Double.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "ClientCharge";
        if (myData.ContainsKey(property))
        {
            myVoice.ClientCharge = Double.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "Url";
        if (myData.ContainsKey(property))
        {
            myVoice.Url = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Sid";
        if (myData.ContainsKey(property))
        {
            myVoice.Sid = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "AuthToken";
        if (myData.ContainsKey(property))
        {
            myVoice.AuthToken = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "ShortCodeFromNumber";
        if (myData.ContainsKey(property))
        {
            myVoice.ShortCodeFromNumber = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "FromPhoneNumber";
        if (myData.ContainsKey(property))
        {
            myVoice.FromPhoneNumber = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "ApiVersion";
        if (myData.ContainsKey(property))
        {
            myVoice.ApiVersion = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Key";
        if (myData.ContainsKey(property))
        {
            myVoice.Key = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Server";
        if (myData.ContainsKey(property))
        {
            myVoice.Server = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Port";
        if (myData.ContainsKey(property))
        {
            myVoice.Port = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "Protocol";
        if (myData.ContainsKey(property))
        {
            myVoice.Protocol = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "RequiresSsl";
        if (myData.ContainsKey(property))
        {
            myVoice.RequiresSsl = bool.Parse(HexAsciiConvert(myData[property]));
            objChanged = true;
        }
        property = "LoginUsername";
        if (myData.ContainsKey(property))
        {
            myVoice.LoginUsername = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "LoginPassword";
        if (myData.ContainsKey(property))
        {
            myVoice.LoginPassword = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "NewlineReplacement";
        if (myData.ContainsKey(property))
        {
            myVoice.NewlineReplacement = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "PhoneNumberFormat";
        if (myData.ContainsKey(property))
        {
            myVoice.PhoneNumberFormat = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        property = "VoiceToken";
        if (myData.ContainsKey(property))
        {
            myVoice.VoiceToken = HexAsciiConvert(myData[property]);
            objChanged = true;
        }
        return objChanged;
    }

    #endregion

    private string HexAsciiConvert(string hex)
    {
        var sb = new StringBuilder();
        for (var i = 0; i <= hex.Length - 2; i += 2)
        {
            sb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(hex.Substring(i, 2),
                        System.Globalization.NumberStyles.HexNumber))));
        }
        return sb.ToString();
    }
}
