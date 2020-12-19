using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using MACSecurity;
using MACServices;

namespace MACUserApps.Web.Tests.Encrypt
{
    public partial class MacUserAppsWebTestsWebConfigEncrypt : System.Web.UI.Page
    {
        HiddenField _hiddenW;

        private const string Test = "AES";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Master != null)
            {
                _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                _hiddenW.Value = "54a83c89ead6362034d04bd4";
            }
        }
    
        protected void btnMSEncrypt_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnEncrypt");
            lbError.Text = "";
            if (String.IsNullOrWhiteSpace(txtKey.Text))
            {
                lbError.Text = @"No Key Selected!";
                return;
            }

            if (txtKey.Text.Length < 16)
            {
                lbError.Text = @"Short key!";
                return;
            }
            if (String.IsNullOrWhiteSpace(txtClearData.Text))
            {
                lbError.Text = @"No value to encrypt!";
                return;
            }
            try
            {
                txtEncryptedData.Text = Security.EncryptAndEncode(txtClearData.Text, txtKey.Text);
            }
            catch (Exception ex)
            {
                txtEncryptedData.Text = @"Exception";
                AddToLogAndDisplay(txtEncryptedData.Text + ":|" + ex.ToString());
            }
        }

        protected void btnMSDecrypt_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnDecrypt");
            lbError.Text = "";
            if (String.IsNullOrWhiteSpace(txtKey.Text))
            {
                lbError.Text = @"No Key Selected!";
                return;
            } 
            if (txtKey.Text.Length < 16)
            {
                lbError.Text = @"Short key!";
                return;
            }
            if (String.IsNullOrWhiteSpace(txtEncryptedData.Text))
            {
                lbError.Text = @"No value to decrypt!";
                return;
            }
            try
            {
                txtDecryptedData.Text = Security.DecodeAndDecrypt(txtEncryptedData.Text, txtKey.Text);
            }
            catch (Exception ex)
            {
                txtDecryptedData.Text = @"Exception";
                AddToLogAndDisplay(txtDecryptedData.Text + ":|" + ex.ToString());
            }
        }

        protected void btnHexDecode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEncryptedData.Text))
            {
                lbError.Text = @"No encoded data to decocode!";
                return;  
            }
            var mUtils = new Utils();
            txtDecryptedData.Text = mUtils.HexToString(txtEncryptedData.Text);
            AddToLogAndDisplay("Decoded Data " + txtDecryptedData.Text.Replace("|", "(Pipe)"));
        }

        protected void btnHexEncode_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtClearData.Text))
            {
                lbError.Text = @"No data to encode!";
                return;
            }
            var mUtils = new Utils();
            txtEncryptedData.Text = mUtils.StringToHex(txtClearData.Text);
            AddToLogAndDisplay("Encoded Data " + txtEncryptedData.Text);
        }

        protected void btnHashData_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtClearData.Text))
            {
                lbError.Text = @"No data to hash!";
                return;
            }
            tstHashedData.Text = Security.GetHashString(txtClearData.Text);
        }

        private void AddToLogAndDisplay(string textToAdd)
        {
            var newlog = String.Format("{0}|{1}.{2}", Session["LogText"], Test, textToAdd);
            Session["LogText"] = newlog;
            tbLog.Text = newlog.Replace("|", Environment.NewLine);
        }
    }
}