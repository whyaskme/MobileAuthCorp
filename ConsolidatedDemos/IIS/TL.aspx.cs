using System;
using System.Configuration;

public partial class TL1 : System.Web.UI.Page
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
            divDiscount25.Visible = false;
            divSpecialOffer.Visible = false;
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
                    if (adTypeSecondChar == "d")
                        divDiscount25.Visible = true;
                    else if (adTypeSecondChar == "s")
                        divSpecialOffer.Visible = true;
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
                        //spanClientDiscount25.InnerHtml = "MAC";
                        //spanClientSpecialOffer.InnerHtml = "MAC";
                        spanClientGeneric.InnerHtml = "MAC";                        
                        break;
                    case "Avenue B":
                        divLogoAvenueB.Visible = true;
                        profileAvenueB.Visible = true;
                        //spanClientDiscount25.InnerHtml = "Avenue B";
                        //spanClientSpecialOffer.InnerHtml = "Avenue B";
                        spanClientGeneric.InnerHtml = "Avenue B";
                        break;
                    case "STS":
                        divLogoSts.Visible = true;
                        profileSts.Visible = true;
                        //spanClientDiscount25.InnerHtml = "STS";
                        //spanClientSpecialOffer.InnerHtml = "STS";
                        spanClientGeneric.InnerHtml = "STS";
                        break;
                    case "TNS":
                        divLogotns.Visible = true;
                        profileTns.Visible = true;
                        //spanClientDiscount25.InnerHtml = "TNS";
                        //spanClientSpecialOffer.InnerHtml = "TNS";
                        spanClientGeneric.InnerHtml = "TNS";
                        break;
                    case "Scottsdale Golf Shop":
                        divLogoGolfShop.Visible = true;
                        profileGolfShop.Visible = true;
                        //spanClientDiscount25.InnerHtml = "Scottsdale Golf Shop";
                        //spanClientSpecialOffer.InnerHtml = "Scottsdale Golf Shop";
                        spanClientGeneric.InnerHtml = "Scottsdale Golf Shop";
                        break;
                    case "Scottsdale Golf Store":
                        divLogoGolfStore.Visible = true;
                        profileGolfStore.Visible = true;
                        //spanClientDiscount25.InnerHtml = "Scottsdale Golf Store";
                        //spanClientSpecialOffer.InnerHtml = "Scottsdale Golf Store";
                        spanClientGeneric.InnerHtml = "Scottsdale Golf Store";
                        break;
                    case "MAC Test Bank":
                        divLogoMac.Visible = true;
                        profileMac.Visible = true;
                        //spanClientDiscount25.InnerHtml = "MAC";
                        //spanClientSpecialOffer.InnerHtml = "MAC";
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