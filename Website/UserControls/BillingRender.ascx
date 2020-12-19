<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BillingRender.ascx.cs" Inherits="UserControls_BillingRender" %>

<div style="padding: 1.0rem;"></div>

<div id="divBillDocumentContainer" runat="server" style="border: solid 0px #0000ff; margin: 0 auto; min-height: 792pt; width: 612pt;">

    <div id="PrintedPageContainer" class="PrintedPageContainer">

        <div id="divPrintPageWithMargin" class="divPrintPageWithMargin">

            <!-- Bill Header -->
            <div class="row">
                <table style="border: solid 0px #ff0000;">
                    <tr>
                        <td style="border: solid 0px #ff0000; text-align: center !important; vertical-align: middle !important;">
                            <img id="imgOwnerLogo" runat="server" src="/Images/OwnerLogos/!Empty-Placeholder.png" class="OwnerLogo" style="border: none;" />
                        </td>
                        <td style="border: solid 0px #ff0000; text-align: center !important; vertical-align: middle !important;">
                            <span id="spanIncludedInGroupBillLabel" runat="server">Billed direct to Client.</span>
                        </td>
                        <td style="border: solid 0px #ff0000; text-align: center !important; vertical-align: middle !important;">
                            <span id="txtClientMemberNumber" runat="server" style="color: #808080; white-space: nowrap;">Client Member #0</span>
                        </td>
                    </tr>
                </table>
            </div>
            <!-- Bill Header -->

            <div style="padding: 0.50rem; border-top: solid 1px #c0c0c0;"></div>

            <!-- Organization Info -->
            <div class="row">
                <table style="width: 100%; border: solid 0px #0000ff;">
                    <tr>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            <span id="divClientNameLabel" runat="server">Client</span>
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtClientName" runat="server"></span>
                        </td>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            TaxId
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtTaxId" runat="server"></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            Street 1
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtStreet1" runat="server"></span>
                        </td>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            Street 2
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtStreet2" runat="server"></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            City
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtCity" runat="server"></span>
                        </td>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            State
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtState" runat="server"></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            Zip
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtZipCode" runat="server"></span>
                        </td>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            Phone
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtPhone" runat="server"></span>
                        </td>
                    </tr>
                </table>
            </div>
            <!-- Organization Info -->

            <div style="padding: 0.50rem;"></div>

            <!-- Bill Status -->
            <div class="row">
                <table style="width: 100%; border: solid 0px #0000ff;">
                    <tr>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            BillId
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtBillNumber" runat="server"></span>
                        </td>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            <span id="divClientIdLabel" runat="server" class="BillingLabel">ClientID</span>
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtClientID" runat="server"></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            Created
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtDateCreated" runat="server"></span>
                        </td>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            Due
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtDateDue" runat="server"></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            Sent
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtDateSent" runat="server"></span>
                        </td>
                        <td style="border: solid 0px #ff0000; width: 100px; text-align: right !important; font-style: italic; color: #808080;">
                            Paid
                        </td>
                        <td style="border: solid 0px #ff0000; width: auto; text-align: left !important;">
                            <span id="txtDatePaid" runat="server"></span>
                        </td>
                    </tr>

                </table>
            </div>
            <!-- Bill Status -->

            <div style="padding: 0.50rem; border-bottom: solid 1px #c0c0c0; margin-bottom: 25px;"></div>

            <!-- OTP Info -->
            <div class="row">
                <!-- OTP row -->
                <div id="divBillingResultsClient" visible="true" runat="server" class="large-12 columns">
                    <div class="billingCategory" style="color: #808080; font-weight: bold; text-align: left;">OTP Sent</div>
                    <table style='width: 100%;border-collapse: collapse;margin-bottom:0;'>
                        <thead>
                            <tr>
                                <th class="text-left small-only-text-center" style="text-align: left !important;"><span id="legendMessaging" runat="server" class="billingData" style="color: #808080; font-style: italic;">Messaging</span></th>
                                <th><span id="legendMessagingCount" runat="server" class="billingData" style="color: #808080; font-style: italic;">Count</span></th>
                                <th><span id="legendMessagingCost" runat="server" class="billingData" style="color: #808080; font-style: italic;">Client Price</span></th>
                                <th><span id="legendMessagingCharge" runat="server" class="billingData" style="color: #808080; font-style: italic;">Charge</span></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td class="text-left small-only-text-center"><span class="billingCategory">Email</span></td>
                                <td><span id="spanOtpEmailCount" runat="server" class="billingData small-only-text-center">0</span></td>
                                <td><span id="spanOtpEmailCost" runat="server" class="billingData small-only-text-center">0.00</span></td>
                                <td><span id="spanOtpEmailCharge" runat="server" class="billingData small-only-text-center" style="font-weight: normal;">$0.00</span></td>
                            </tr>
                            <tr>
                                <td class="text-left small-only-text-center"><span class="billingCategory">Sms</span></td>
                                <td><span id="spanOtpSmsCount" runat="server" class="billingData small-only-text-center">0</span></td>
                                <td><span id="spanOtpSmsCost" runat="server" class="billingData small-only-text-center">0.00</span></td>
                                <td><span id="spanOtpSmsCharge" runat="server" class="billingData small-only-text-center" style="font-weight: normal;">$0.00</span></td>
                            </tr>
                            <tr>
                                <td class="text-left small-only-text-center"><span class="billingCategory">Voice</span></td>
                                <td><span id="spanOtpVoiceCount" runat="server" class="billingData small-only-text-center">0</span></td>
                                <td><span id="spanOtpVoiceCost" runat="server" class="billingData small-only-text-center">0.00</span></td>
                                <td><span id="spanOtpVoiceCharge" runat="server" class="billingData small-only-text-center" style="font-weight: normal;">$0.00</span></td>
                            </tr>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td colspan="4"><span style="font-weight: normal; font-style: italic;">&nbsp;</span> <span class="billingData" style="font-weight: bold;" runat="server" id="spanOtpTotal">$0.00</span></td>
                            </tr>
                        </tfoot>
                    </table>
                </div>
            </div>
            <!-- OTP Info -->

            <div style="padding: 0.50rem;"></div>

            <!-- AdPass Info -->
            <div class="row">
                <!-- AdPass row -->
                <div id="div7" visible="true" runat="server" class="large-12 columns">
                    <div class="billingCategory" style="color: #808080; font-weight: bold; text-align: left;">AdPass</div>
                    <table style='width: 100%;border-collapse: collapse;margin-bottom:0;'>
                        <thead>
                            <tr>
                                <th class="text-left small-only-text-center" style="text-align: left !important;"><span id="Span1" runat="server" class="billingData" style="color: #808080; font-style: italic;">Ad Types</span></th>
                                <th><span id="legendAdsCount" runat="server" class="billingData" style="color: #808080; font-style: italic;">Count</span></th>
                                <th><span id="legendAdsCost" runat="server" class="billingData" style="color: #808080; font-style: italic;">Client Price</span></th>
                                <th><span id="legendAdsCharge" runat="server" class="billingData" style="color: #808080; font-style: italic;">Charge</span></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td class="text-left small-only-text-center"><span class="billingCategory">Message</span></td>
                                <td><span id="spanAdMessageSentCount" runat="server" class="billingData small-only-text-center">0</span></td>
                                <td><span id="spanAdMessageSentCost" runat="server" class="billingData small-only-text-center">0.00</span></td>
                                <td><span id="spanAdMessageSentAmount" runat="server" class="billingData small-only-text-center" style="font-weight: normal;">$0.00</span></td>
                            </tr>
                            <tr>
                                <td class="text-left small-only-text-center"><span class="billingCategory">Enter OTP</span></td>
                                <td><span id="spanAdEnterOtpScreenSentCount" runat="server" class="billingData small-only-text-center">0</span></td>
                                <td><span id="spanAdEnterOtpScreenSentCost" runat="server" class="billingData small-only-text-center">0.00</span></td>
                                <td><span id="spanAdEnterOtpScreenSentAmount" runat="server" class="billingData small-only-text-center" style="font-weight: normal;">$0.00</span></td>
                            </tr>
                            <tr>
                                <td class="text-left small-only-text-center"><span class="billingCategory">Content Ad</span></td>
                                <td><span id="spanAdVerificationScreenSentCount" runat="server" class="billingData small-only-text-center">0</span></td>
                                <td><span id="spanAdVerificationScreenSentCost" runat="server" class="billingData small-only-text-center">0.00</span></td>
                                <td><span id="spanAdVerificationScreenSentAmount" runat="server" class="billingData small-only-text-center" style="font-weight: normal;">$0.00</span></td>
                            </tr>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td colspan="4"><span style="font-weight: normal; font-style: italic;">&nbsp;</span> <span class="billingData" style="font-weight: bold;" runat="server" id="spanAdPassTotal">$0.00</span></td>
                            </tr>
                        </tfoot>
                    </table>
                </div>
            </div>
            <!-- AdPass Info -->

            <div style="padding: 0.50rem;"></div>

            <!-- End User Info -->
            <div class="row">
                <!-- Users row -->
                <div id="div1" visible="true" runat="server" class="large-12 columns">
                    <div class="billingCategory" style="color: #808080; font-weight: bold; text-align: left;">Users</div>
                    <table style='width: 100%;border-collapse: collapse;margin-bottom:0;'>
                        <thead>
                            <tr>
                                <th class="text-left small-only-text-center" style="text-align: left !important;"><span id="Span5" runat="server" class="billingData" style="color: #808080; font-style: italic;">Action</span></th>
                                <th><span id="legendEndUserCount" runat="server" class="billingData" style="color: #808080; font-style: italic;">Count</span></th>
                                <th><span id="legendEndUserCost" runat="server" class="billingData" style="color: #808080; font-style: italic;">Client Price</span></th>
                                <th><span id="legendEndUserCharge" runat="server" class="billingData" style="color: #808080; font-style: italic;">Charge</span></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td class="text-left small-only-text-center"><span class="billingCategory">Register</span></td>
                                <td><span id="spanEndUserRegistrationCount" runat="server" class="billingData small-only-text-center">0</span></td>
                                <td><span id="spanEndUserRegistrationCost" runat="server" class="billingData small-only-text-center">0.00</span></td>
                                <td><span id="spanEndUserRegistrationAmount" runat="server" class="billingData small-only-text-center" style="font-weight: normal;">$0.00</span></td>
                            </tr>
                            <tr>
                                <td class="text-left small-only-text-center"><span class="billingCategory">Verify</span></td>
                                <td><span id="spanEndUserVerificationCount" runat="server" class="billingData small-only-text-center">0</span></td>
                                <td><span id="spanEndUserVerificationCost" runat="server" class="billingData small-only-text-center">0.00</span></td>
                                <td><span id="spanEndUserVerificationAmount" runat="server" class="billingData small-only-text-center" style="font-weight: normal;">$0.00</span></td>
                            </tr>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td colspan="4"><span style="font-weight: normal; font-style: italic;">&nbsp;</span> <span class="billingData" style="font-weight: bold;" runat="server" id="spanEndUserTotal">$0.00</span></td>
                            </tr>
                        </tfoot>
                    </table>
                </div>
            </div>
            <!-- End User Info -->

            <!-- Miscellaneous/Addendums -->
            <div class="row" id="divMiscellaneousBilling" runat="server">
                <div style="padding: 0.50rem;"></div>
                <div id="div2" visible="true" runat="server" class="large-12 columns">
                    <div class="billingCategory" style="color: #808080; font-weight: bold; text-align: left;">Miscellaneous</div>
                    <table style='width: 100%;border-collapse: collapse;margin-bottom:0;'>
                        <thead>
                            <tr>
                                <th class="tdMisc50 miscDesc"><span id="Span10" runat="server" class="billingData" style="color: #808080; font-style: italic;">Addendum</span></th>
                                <th class="tdMisc25"><span id="Span12" runat="server" class="billingData" style="color: #808080; font-style: italic;">Credit</span></th>
                                <th class="tdMisc25"><span id="Span11" runat="server" class="billingData" style="color: #808080; font-style: italic;">Charge</span></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td runat="server" colspan="4" class="text-right small-only-text-center" style="border-top: 0 !important;padding:0 !important;">
                                    <div id="divAddendumsContainer" runat="server" style=""></div>
                                </td>
                            </tr>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td colspan="4" class="text-right small-only-text-center"><span style="font-weight: normal; font-style: italic;">&nbsp;</span> <span class="billingData" style="font-weight: bold;" runat="server" id="spanMiscTotal">$0.00</span></td>
                            </tr>
                        </tfoot>    
                    </table>
                </div>
            </div>
            <!-- Miscellaneous/Addendums -->

            <div style="padding: 0.50rem;"></div>

            <!-- Sub/Totals -->
            <div class="row">
                <!-- Billing Details -->
                <div id="div3" visible="true" runat="server" class="large-12 columns">
                    <div class="billingCategory" style="color: #808080; font-weight: bold; text-align: left;">Summary</div>
                    <table style='width: 100%;border-collapse: collapse;margin-bottom:0;'>
                        <tbody>
                            <tr>
                                <td class="tdMisc75">
                                    <div class="text-right small-only-text-center" id="divSubtotal" visible="true" runat="server" style="text-align: right; color: #808080;"><em>Subtotal</em></div>
                                </td>
                                <td class="tdMisc25">
                                    <span id="spanSubtotal" runat="server" class="billingData">$0.00</span>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdMisc75">
                                    <div class="text-right small-only-text-center" id="legendTaxRate" visible="true" runat="server" style="text-align: right; color: #808080;"><em>Client Tax Rate</em></div>
                                </td>
                                <td class="tdMisc25">
                                    <span id="spanTaxRate" runat="server" class="billingData">0 %</span>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdMisc75">
                                    <div class="text-right small-only-text-center" id="legendTaxes" visible="true" runat="server" style="text-align: right; color: #808080;"><em>Taxes</em></div>
                                </td>
                                <td class="tdMisc25">
                                    <span id="spanSalesTax" runat="server" class="billingData">$0.00</span>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdMisc75">
                                    <div class="text-right small-only-text-center" id="div33" visible="true" runat="server" style="text-align: right; color: #808080;"><em>Total</em></div>
                                </td>
                                <td class="tdMisc25">
                                   <span id="spanTotal" runat="server" class="billingData" style="font-weight: bold;">$0.00</span>
                                </td>
                            </tr>

                        </tbody>
                    </table>
                </div>  
                          
                </div>

            <div style="padding: 0.50rem;"></div>   
                
            <!-- Footnotes -->
            <div class="row">
                <div id="divClientSummaryContainer" runat="server">
                    <div id="divClientSummary" runat="server" class="large-12 medium-12 small-6 columns">
                        FootNotes
                    </div>
                </div>                  
            </div>
            <!-- Footnotes -->

            <div style="padding: 0.50rem;"></div>

            <!-- Page Number -->
            <div class="row">
                <div id="divPageNumber" runat="server" class="large-12 medium-12 small-6 columns" style="border-top: solid 1px #c0c0c0; color: #808080; position: relative; top: 0px; padding-top: 15px; padding-bottom: 15px; text-align: right;">
                    Page Number
                </div>                   
            </div>
            <!-- Page Number -->                      

        </div>

    </div>

</div>