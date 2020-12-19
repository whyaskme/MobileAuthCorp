using System;
using System.Web.Services;
using System.Text;
using System.Xml;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using sr = MACServices.Constants.ServiceResponse;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]

public class EncryptDecrypt : WebService
{
    [WebMethod]
    public XmlDocument WsEncrypt(string data)
    {
        var mUtils = new Utils();
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        var mValueToReturn = "0";
        if (data.Contains(dk.ItemSep))
        {
            var mData = getValues(data);
            mValueToReturn = MACSecurity.Security.EncryptAndEncode(mData.Item1, mData.Item2);
        }

        myResponse.Append("<" + sr.Reply + ">" + mValueToReturn + "</" + sr.Reply + ">");
        var rply = mUtils.FinalizeXmlResponse(myResponse, "");
        return rply;
    }

    [WebMethod]
    public XmlDocument WsDecrypt(string data)
    {
        var mUtils = new Utils();
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);
        var mDecryptedData = String.Empty;
        if (data.Contains(dk.ItemSep))
        {
            var mData = getValues(data);
            mDecryptedData = MACSecurity.Security.DecodeAndDecrypt(mData.Item1, mData.Item2);

        }
        myResponse.Append("<" + sr.Reply + ">" + mUtils.StringToHex(mDecryptedData) + "</" + sr.Reply + ">");
        var rply = mUtils.FinalizeXmlResponse(myResponse, "");
        return rply;
    }

    public XmlDocument WsHash(string data)
    {
        var mUtils = new Utils();
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);
        var mValueToReturn = "0";
        string mStringToHash;
        if (data.StartsWith("value:"))
        {
            mStringToHash = mUtils.HexToString(data.Replace("value:", ""));
            mValueToReturn = MACSecurity.Security.GetHashString(mStringToHash);
        }
        myResponse.Append("<" + sr.Reply + ">" + mValueToReturn + "</" + sr.Reply + ">");
        var rply = mUtils.FinalizeXmlResponse(myResponse, "");
        return rply;
    }

    protected Tuple<string, string> getValues(string pData)
    {
        var mUtils = new Utils();
        var mValue = string.Empty;
        var mKey = string.Empty;
        var mParas = pData.Split(char.Parse(dk.ItemSep));
        foreach (var mPara in mParas)
        {
            if (mPara.StartsWith("value"))
                mValue = mUtils.HexToString(mPara.Replace("value:", ""));
            if (mPara.StartsWith("key"))
                mKey = mUtils.HexToString(mPara.Replace("key:", ""));
        }
        return new Tuple<string, string>(mValue, mKey);
    }

}
