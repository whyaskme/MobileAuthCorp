using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace RemovingWhiteSpacesAspNet
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public class WebForm1 : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.Label lblSum;
		protected System.Web.UI.WebControls.RadioButtonList rblSecond;
		protected System.Web.UI.WebControls.Button btCalc;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator1;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator2;
		protected System.Web.UI.WebControls.RadioButtonList rblFirst;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btCalc.Click += new System.EventHandler(this.btCalc_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btCalc_Click(object sender, System.EventArgs e)
		{
			if (this.IsValid)
				lblSum.Text = (rblFirst.SelectedIndex + rblSecond.SelectedIndex + 2).ToString();
			else
				lblSum.Text = "[Sum]";
		}
	}
}
