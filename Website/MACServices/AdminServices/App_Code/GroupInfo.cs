using System;
using System.Web.Services;
using Newtonsoft.Json;

using MACServices;
using ds = MACServices.Constants.Strings;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using sr = MACServices.Constants.ServiceResponse;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]

public class GroupInfo : WebService
{

    // ReSharper disable once EmptyConstructor
    public GroupInfo()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string WsGetGroupInfo(string groupId)
    {
        // ReSharper disable once RedundantAssignment
        var tmpResponse = "Valid groupId";

        if (String.IsNullOrEmpty((groupId)))
            groupId = ds.DefaultGroupId;

        // Connect to the db and get the populated group object
        var myGroup = new Group(groupId);

        tmpResponse = JsonConvert.SerializeObject(myGroup);

        return tmpResponse;
    }

    [WebMethod]
    public string WsUpdateGroupInfo(string groupId, string groupProperties)
    {
        // If sucessfully updated, return true
        var updateStatus = "Successfully updated";

        return updateStatus;
    }
}
