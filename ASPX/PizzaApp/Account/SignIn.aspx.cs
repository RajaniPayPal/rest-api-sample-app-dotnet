using System;
using System.Linq;
using System.Data;
using System.Text;
using System.Data.SQLite;
using System.Web.Security;

namespace PizzaApp
{
    public partial class SignIn : System.Web.UI.Page
    {
        #region Data
        private DataTable GetUser(string email)
        {
            DataTable datTable = null;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT ");
            selectQuery.Append("id, ");
            selectQuery.Append("email, ");
            selectQuery.Append("encrypted_password, ");
            selectQuery.Append("sign_in_count ");
            selectQuery.Append("FROM users ");
            selectQuery.Append("WHERE email = @email;");
            using (SQLiteCommand commandSQLite = new SQLiteCommand(selectQuery.ToString()))
            {
                commandSQLite.Parameters.AddWithValue("@email", email);
                DataAccessLayer dataAccessObject = new DataAccessLayer();
                datTable = dataAccessObject.Select(commandSQLite);                    
            }           
            return datTable;      
        }

        private bool IsPasswordValid(string email, string password)
        {
            bool isValid = false;
            string decryptedPassword = null;
            DataTable datTable = GetUser(email);
            if (datTable != null && datTable.Rows.Count > 0)
            {
                var distinctRows = (from DataRow dRow in datTable.Rows
                                   where dRow.Field<string>("email") == email
                                    select new { column1 = dRow["encrypted_password"] }).Distinct();
                if (distinctRows != null)
                {
                    foreach (var row in distinctRows)
                    {
                        string encryptedPassword = Convert.ToString(row.column1);
                        decryptedPassword = Secure.Decrypt(encryptedPassword);
                        break;
                    }
                }
                if (password.Trim().Equals(decryptedPassword.Trim()))
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        private int GetSignedInUserSignInCount(string email)
        {
            int signInCount = 0;
            DataTable datTable = GetUser(email);
            if (datTable != null && datTable.Rows.Count > 0)
            {
                var distinctRows = (from DataRow dRow in datTable.Rows
                                   where dRow.Field<string>("email") == email
                                    select new { column1 = dRow["sign_in_count"] }).Distinct();
                if (distinctRows != null)
                {
                    foreach (var row in distinctRows)
                    {
                        signInCount = Convert.ToInt32(row.column1);
                        break;
                    }
                }
            }
            return signInCount;
        }

        private bool Update(string email)
        {
            bool isSuccess = false;
            int rowsAffected = 0;
            int signInCount = 0;
            DataTable datTable = GetUser(email);
            if (datTable != null && datTable.Rows.Count > 0)
            {
                var distinctRows = (from DataRow dRow in datTable.Rows
                                   where dRow.Field<string>("email") == email
                                    select new { column1 = dRow["sign_in_count"] }).Distinct();
                if (distinctRows != null)
                {
                    foreach (var row in distinctRows)
                    {
                        signInCount = Convert.ToInt32(row.column1.ToString());
                        signInCount++;
                        break;
                    }
                }
                StringBuilder updateQuery = new StringBuilder();
                updateQuery.Append("UPDATE Users ");
                updateQuery.Append("SET ");
                updateQuery.Append("sign_in_count = @sign_in_count ");
                updateQuery.Append("WHERE ");
                updateQuery.Append("email = @email;");
                using (SQLiteCommand commandSQLite = new SQLiteCommand(updateQuery.ToString()))
                {
                    commandSQLite.Parameters.AddWithValue("@sign_in_count", signInCount);
                    commandSQLite.Parameters.AddWithValue("@email", email);
                    DataAccessLayer dataAccessObject = new DataAccessLayer();
                    rowsAffected = dataAccessObject.Execute(commandSQLite);
                }
            }
            if (rowsAffected > 0)
            {
                isSuccess = true;
            }
            return isSuccess;
        }
        #endregion

        #region Event Handlers
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void ButtonSignIn_Click(object sender, EventArgs e)
        {
            var email = TextBoxEmail.Text.Trim();
            var password = TextBoxPassword.Text.Trim();
            bool isValid = IsPasswordValid(email, password);
            if (isValid)
            {
                int signInCount = GetSignedInUserSignInCount(email);
                if (signInCount > 0)
                {
                    bool isSuccess = Update(email);
                } 

                if (Request.QueryString["order[amount]"] != null && Request.QueryString["order[description]"] != null)
                {
                    FormsAuthentication.SetAuthCookie(TextBoxEmail.Text.Trim(), CheckBoxPersist.Checked);
                    Response.Redirect("~/PlaceOrders/PlaceOrders.aspx" + Request.Url.Query);
                }
                else
                {
                    FormsAuthentication.RedirectFromLoginPage(TextBoxEmail.Text.Trim(), CheckBoxPersist.Checked);
                }
            }
            else
            {
                divAlertMessage.Visible = true;
                divAlertMessage.Attributes["class"] = "alert fade in alert-error";
                LabelAlertMessage.Text = "Invalid Email or Password.";
            }
        }
        #endregion
    }
}