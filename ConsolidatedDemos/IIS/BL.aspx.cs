using System;
using System.Configuration;

public partial class BL1 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // hide client logos
            divLogoMac.Visible = false;
            divLogoAvenueB.Visible = false;
            divLogoSts.Visible = false;
            divLogotns.Visible = false;
            divLogoGolfShop.Visible = false;
            divLogoGolfStore.Visible = false;

            // hide page titles
            divLoans.Visible = false;
            divCreditCards.Visible = false;
            divGeneric.Visible = false;

            // hide client profiles
            profileAvenueB.Visible = false;
            profileMac.Visible = false;
            profileSts.Visible = false;
            profileTns.Visible = false;
            profileGolfShop.Visible = false;
            profileGolfStore.Visible = false;

            // get client name
            var adClient = Request.QueryString["AdClient"].ToString();

            // get ad type
            var adType = Request.QueryString["AdType"].ToString();
            var adTypeSecondChar = adType[1].ToString().ToLower();

            if (String.IsNullOrEmpty(adClient) == false)
            {
                // determine which ad type heading to display (generic, discount or specials)
                if (String.IsNullOrEmpty(adType) == false)
                {
                    if (adTypeSecondChar == "l")
                        divLoans.Visible = true;
                    else if (adTypeSecondChar == "c")
                        divCreditCards.Visible = true;
                    else
                    {
                        foreach (var inputChar in adTypeSecondChar)
                        {
                            if (Char.IsNumber(inputChar))
                            {
                                spanTypeNum.InnerHtml = adTypeSecondChar;
                            }
                        }
                        divGeneric.Visible = true;
                    }
                }

                switch (adClient)
                {
                    case "MAC Test":
                        divLogoMac.Visible = true;
                        profileMac.Visible = true;
                        //spanClientLoans.InnerHtml = "MAC";
                        //spanClientCreditCards.InnerHtml = "MAC";
                        spanClientGeneric.InnerHtml = "MAC";
                        break;
                    case "Avenue B":
                        divLogoAvenueB.Visible = true;
                        profileAvenueB.Visible = true;
                        //spanClientLoans.InnerHtml = "Avenue B";
                        //spanClientCreditCards.InnerHtml = "Avenue B";
                        spanClientGeneric.InnerHtml = "Avenue B";
                        break;
                    case "STS":
                        divLogoSts.Visible = true;
                        profileSts.Visible = true;
                        //spanClientLoans.InnerHtml = "STS";
                        //spanClientCreditCards.InnerHtml = "STS";
                        spanClientGeneric.InnerHtml = "STS";
                        break;
                    case "TNS":
                        divLogotns.Visible = true;
                        profileTns.Visible = true;
                        //spanClientLoans.InnerHtml = "TNS";
                        //spanClientCreditCards.InnerHtml = "TNS";
                        spanClientGeneric.InnerHtml = "TNS";
                        break;
                    case "Scottsdale Golf Shop":
                        divLogoGolfShop.Visible = true;
                        profileGolfShop.Visible = true;
                        //spanClientLoans.InnerHtml = "Scottsdale Golf Shop";
                        //spanClientCreditCards.InnerHtml = "Scottsdale Golf Shop";
                        spanClientGeneric.InnerHtml = "Scottsdale Golf Shop";
                        break;
                    case "Scottsdale Golf Store":
                        divLogoGolfStore.Visible = true;
                        profileGolfStore.Visible = true;
                        //spanClientLoans.InnerHtml = "Scottsdale Golf Store";
                        //spanClientCreditCards.InnerHtml = "Scottsdale Golf Store";
                        spanClientGeneric.InnerHtml = "Scottsdale Golf Store";
                        break;
                    case "MAC Test Bank":
                        divLogoMac.Visible = true;
                        profileMac.Visible = true;
                        //spanClientLoans.InnerHtml = "MAC";
                        //spanClientCreditCards.InnerHtml = "MAC";
                        spanClientGeneric.InnerHtml = "MAC";
                        break;
                }
            }
        }
        catch
        {

        }

    }
}