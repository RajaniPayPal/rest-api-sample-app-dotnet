using System;
using System.Web.Security;

namespace PizzaApp
{
    public partial class SignOut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("~/Default.aspx?SignedIn=False");
        }
    }
}