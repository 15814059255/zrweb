using System;
using System.Web.UI;
using System.Configuration;

public partial class UserControls_bottom : UserControl
{
    protected string VersionTag = DateTime.Now.ToString("yyyyMMdd");

    protected void Page_Load(object sender, EventArgs e)
    {
    }
}