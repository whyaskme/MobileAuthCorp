<%@ WebHandler Language="C#" Class="FileUpload" %>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Xml;
using System.ServiceModel;

using MACServices;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;

public class FileUpload : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        
        HttpPostedFile uploads = context.Request.Files["upload"];

        string CKEditorFuncNum = context.Request["CKEditorFuncNum"];

        string file = System.IO.Path.GetFileName(uploads.FileName);

        var fileExtension = file.Split('.');
        var fileName = ObjectId.GenerateNewId() + "." + fileExtension[1];

        uploads.SaveAs(context.Server.MapPath(".") + "\\Images\\" + fileName);

        string url = "/Admin/Help/Images/" + fileName;

        context.Response.Write("<script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + url + "\");</script>");
        context.Response.End();          
        
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}