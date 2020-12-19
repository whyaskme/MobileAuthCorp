<%@ Page Title="MAC Otp System Administration" Language="C#" 
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true" 
    CodeFile="Encrypt.aspx.cs" 
    Inherits="MACUserApps.Web.Tests.Encrypt.MacUserAppsWebTestsWebConfigEncrypt" 
    validateRequest="false" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="BodyContent" Runat="Server">
    <script src="aes.js"></script>

    <div class="row">
        <div class="large-3 medium-3 small-12 columns hide-for-small">
            &nbsp;
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <uc1:MenuTest runat="server" ID="MenuTest" />
        </div>
        <div class="large-3 medium-3 small-12 columns hide-for-small">
            &nbsp;
        </div>
    </div>

    <div style="padding: 0.75rem;"></div>

    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            <h3>AES Encrypt/Decrypt & Hex Encode/Decode</h3>
        </div>
    </div>

    <div class="row">
        <div class="large-12 columns">
            <fieldset><legend></legend>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Data
                            <asp:TextBox runat="server" ID="txtClearData" />                                                                
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Key
                            <asp:TextBox runat="server" ID="txtKey" />                                                                
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="large-12 medium-12 small-12 columns">
                        <label>Encrypted/Encoded Data
                            <asp:TextBox runat="server" ID="txtEncryptedData" />                                                                
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="large-12 medium-12 small-12 columns">
                        <label>Decrypted/Decoded Data
                            <asp:TextBox runat="server" ID="txtDecryptedData" />                                                                
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="large-12 medium-12 small-12 columns">
                        <label>Hashed Data
                            <asp:TextBox runat="server" ID="tstHashedData" />                                                                
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="large-12 columns">
                        <asp:Button CssClass="button tiny radius" ID="btnMSEncrypt" runat="server" Text="MS Encrypt" OnClick="btnMSEncrypt_Click" />
                        <asp:Button CssClass="button tiny radius" ID="btnMSDecrypt" runat="server" Text="MS Decrypt" OnClick="btnMSDecrypt_Click"/>
                        
                        <input class="button tiny radius" style="width: 100px;margin:0 0 0.75rem;" id="btnJSEncrypt" type="button" value="JS Encrypt" 
                            onclick="javascript: TestEncrypt();" />

                        <input class="button tiny radius" style="width: 100px;margin:0 0 0.75rem;" id="btnJSDecrypt" type="button" value="JS Decrypt" 
                            onclick="javascript: TestDecrypt();" />
                        
                        <asp:Button CssClass="button tiny radius" ID="btnHexEncode" runat="server" Text="Hex Encode" OnClick="btnHexEncode_Click" />
                        <asp:Button CssClass="button tiny radius" ID="btnHexDecode" runat="server" Text="Hex Decode" OnClick="btnHexDecode_Click"/>
                        <asp:Button CssClass="button tiny radius" ID="bthHash" runat="server" Text="Hash Data" OnClick="btnHashData_Click"/>
                    </div>
                </div>
            </fieldset>
        </div>
    </div>
    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Label ID="lbLineNumber" runat="server" Font-Bold="false" />
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Label ID="lbError" runat="server" Font-Bold="false" ForeColor="Red" />
        </div>
    </div>
    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
        </div>
    </div>
</asp:Content>
