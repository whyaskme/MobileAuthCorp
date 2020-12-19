
function InitDisplay()
{
    var operationsList = document.getElementById("dlWorkflows");
    var operatorSitesList = document.getElementById("dlOperatorSites");

    if (operatorSitesList.selectedIndex > 0) {
        document.getElementById("divServiceOperationsContainer").style.display = "block";
        document.getElementById("divOperatorInfo").style.display = "block";

        // Only hide if a valid operation is selected
        if (operationsList.selectedIndex > 0) {
            if (document.getElementById("divPlayerContainer") != null)
                document.getElementById("divPlayerContainer").style.display = "block";
        }
        else {
            if (document.getElementById("divPlayerContainer") != null)
                document.getElementById("divPlayerContainer").style.display = "none";
        }
    }
    else {
        document.getElementById("divServiceOperationsContainer").style.display = "none";
        document.getElementById("divOperatorInfo").style.display = "none";

        // Only hide if a valid operation is selected
        if (operationsList.selectedIndex > 0) {
            if (document.getElementById("divPlayerContainer") != null)
                document.getElementById("divPlayerContainer").style.display = "block";
        }
        else {
            if (document.getElementById("divPlayerContainer") != null)
                document.getElementById("divPlayerContainer").style.display = "none";
        }
    }
}

function SetOperatorSiteInfo()
{
    var operatorSitesList = document.getElementById("dlOperatorSites");
    if (operatorSitesList.selectedIndex > 0) {

        document.getElementById("divOperatorInfo").style.display = "block";
        document.getElementById("divServiceOperationsContainer").style.display = "block";

        var selectedOperatorSite = operatorSitesList.options[operatorSitesList.selectedIndex];
        var tmpAttributeCollection = selectedOperatorSite.value.split('|');
        for (var i = 0; i < tmpAttributeCollection.length; i++)
        {
            var tmpAttributes = tmpAttributeCollection[i].split(':');
            for(var j = 0; j < tmpAttributes.length; j++)
            {
                var attrName = tmpAttributes[0];
                var attrValue = tmpAttributes[1];

                switch (attrName)
                {
                    case "operatorId":
                        $('#hiddenOperatorId').val(attrValue);
                        if (attrValue != "")
                            $('#lbOperatorId').html(attrValue);
                        break;

                    case "siteId":
                        $('#hiddenSiteId').val(attrValue);
                        $('#lbSiteId').html(attrValue);
                        break;

                    case "siteUserName":
                        $('#txtSiteUsername').val(attrValue);
                        break;

                    case "sitePassword":
                        $('#txtSiteUserPassword').val(attrValue);
                        break;

                    case "geoInfo":
                        $('#txtGeoInfo').val(attrValue);
                        break;

                    case "authorizedIp":
                        $('#txtAuthorizedIP').val(attrValue);
                        break;

                    case "clientId":
                        $('#clientId').val(attrValue);
                        break;
                }
            }
        }
        if (document.getElementById("divPlayerContainer") != null)
            document.getElementById("divPlayerContainer").style.display = "block";
    }
    else
    {
        document.getElementById("divServiceOperationsContainer").style.display = "none";
        document.getElementById("divOperatorInfo").style.display = "none";
        document.getElementById("divActionButtons").style.display = "none";
        document.getElementById("txtOperatorId").value = "";
        document.getElementById("txtSiteUsername").value = "";
        document.getElementById("txtSiteUserPassword").value = "";
        document.getElementById("txtGeoInfo").value = "";
        document.getElementById("txtAuthorizedIP").value = "";

        if (document.getElementById("divPlayerContainer") != null)
            document.getElementById("divPlayerContainer").style.display = "none";
    }
}

function SetLoopbackOption(value)
{
    $('#hiddenLoopbackEnabled').val(value);
}