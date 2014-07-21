using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Data;
using System.Data.SQLite;
using PayPal;
using PayPal.Api.Payments;

namespace PizzaApp
{
    public partial class Profile : System.Web.UI.Page
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
            selectQuery.Append("sign_in_count, ");
            selectQuery.Append("current_sign_in_at, ");
            selectQuery.Append("last_sign_in_at, ");
            selectQuery.Append("last_sign_in_ip, ");
            selectQuery.Append("created_at, ");
            selectQuery.Append("updated_at, ");
            selectQuery.Append("credit_card_id, ");
            selectQuery.Append("credit_card_description ");
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
                        string encryptedPassword = row.column1.ToString();
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

        private bool Update(string email, string newPassword, string confirmNewPassword, string newCreditCardType, string newCreditCardNumber, string newCreditCardCVV2, string newCreditCardExpireMonth, string newCreditCardExpireYear)
        {
            bool isSuccess = false;

            int rowsAffected = 0;
            var encryptedNewPassword = Secure.Encrypt(newPassword);
            var signInCount = 0;
            var dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.FFFFF");
            var currentSignInAt = dateTimeNow;

            // Set last signed in IP Address from database
            var lastSignInAt = string.Empty;

            // Set first signed in IP Address from database
            var signInIPAddress = string.Empty;

            // Set current signed in IP Address
            var currentSignInIP = string.Empty;

            // Set last signed in IP Address from database
            var lastSignInIP = string.Empty;

            var createdAt = string.Empty;
            var updatedAt = dateTimeNow;
            DataTable datTable = GetUser(email);
            if (datTable != null && datTable.Rows.Count > 0)
            {
                var distinctRows = (from DataRow dRow in datTable.Rows
                                   where dRow.Field<string>("email") == email
                                   select new { column1 = dRow["sign_in_count"], 
                                   column2 = dRow["last_sign_in_at"], 
                                   column3 = dRow["last_sign_in_ip"],
                                   column4 = dRow["created_at"]
                                   }).Distinct();
                if (distinctRows != null)
                {
                    foreach (var row in distinctRows)
                    {
                        signInCount = Convert.ToInt32(row.column1.ToString());
                        signInCount++;
                        DateTime lastSignInAtDateTime = Convert.ToDateTime(row.column2);
                        lastSignInAt = lastSignInAtDateTime.ToString("yyyy-MM-dd hh:mm:ss.FFFFF");
                        lastSignInIP = Convert.ToString(row.column3);
                        DateTime createdAtDateTime = Convert.ToDateTime(row.column4);
                        createdAt = createdAtDateTime.ToString("yyyy-MM-dd hh:mm:ss.FFFFF");
                        break;
                    }
                }
                CreditCard credCard = CreateCreditCard(newCreditCardType, newCreditCardNumber, newCreditCardCVV2, newCreditCardExpireMonth, newCreditCardExpireYear);
                var creditCardId = credCard.id;
                var creditCardDescription = credCard.number;
                StringBuilder updateQuery = new StringBuilder();
                updateQuery.Append("UPDATE Users ");
                updateQuery.Append("SET ");
                updateQuery.Append("encrypted_password = @encrypted_password, ");
                updateQuery.Append("sign_in_count = @sign_in_count, ");
                updateQuery.Append("current_sign_in_at = @current_sign_in_at, ");
                updateQuery.Append("last_sign_in_at = @last_sign_in_at, ");
                updateQuery.Append("current_sign_in_ip = @current_sign_in_ip, ");
                updateQuery.Append("last_sign_in_ip = @last_sign_in_ip, ");
                updateQuery.Append("created_at = @created_at, ");
                updateQuery.Append("credit_card_id = @credit_card_id, ");
                updateQuery.Append("credit_card_description = @credit_card_description ");
                updateQuery.Append("WHERE ");
                updateQuery.Append("email = @email;");
                using (SQLiteCommand commandSQLite = new SQLiteCommand(updateQuery.ToString()))
                {
                    commandSQLite.Parameters.AddWithValue("@email", email);
                    commandSQLite.Parameters.AddWithValue("@encrypted_password", encryptedNewPassword);
                    commandSQLite.Parameters.AddWithValue("@sign_in_count", signInCount);
                    commandSQLite.Parameters.AddWithValue("@current_sign_in_at", currentSignInAt);
                    commandSQLite.Parameters.AddWithValue("@last_sign_in_at", lastSignInAt);
                    commandSQLite.Parameters.AddWithValue("@current_sign_in_ip", currentSignInIP);
                    commandSQLite.Parameters.AddWithValue("@last_sign_in_ip", lastSignInIP);
                    commandSQLite.Parameters.AddWithValue("@created_at", createdAt);
                    commandSQLite.Parameters.AddWithValue("@updated_at", updatedAt);
                    commandSQLite.Parameters.AddWithValue("@credit_card_id", creditCardId);
                    commandSQLite.Parameters.AddWithValue("@credit_card_description", creditCardDescription);
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

        private bool DataBind(string email)
        {
            bool isSuccess = false;
            DataTable datTable = GetUser(email);
            if (datTable != null && datTable.Rows.Count > 0)
            {
                if (datTable.Rows[0]["email"] != DBNull.Value)
                {
                    TextBoxEmail.Text = Convert.ToString(datTable.Rows[0]["email"]);
                }
                string creditCardId = string.Empty;
                if (datTable.Rows[0]["credit_card_id"] != DBNull.Value)
                {
                    creditCardId = Convert.ToString(datTable.Rows[0]["credit_card_id"]);
                }
                CreditCard crdtCard = CreditCard.Get(Api, creditCardId);
                TextBoxCurrentCreditCardNumber.Text = crdtCard.number.Trim();
                isSuccess = true;
            }
            return isSuccess;
        }
        #endregion

        #region PayPal
        private string AccessToken
        {
            get
            {
                string token = new OAuthTokenCredential
                                (
                                   "EBWKjlELKMYqRNQ6sYvFo64FtaRLRR5BdHEESmha49TM",
                                    "EO422dn3gQLgDbuwqTjzrFgFtaRLRR5BdHEESmha49TM",
                                    Configuration.GetConfiguration()
                                ).GetAccessToken();
                return token;
            }
        }

        private APIContext Api
        {
            get
            {
                APIContext context = new APIContext(AccessToken);
                context.Config = Configuration.GetConfiguration();
                return context;
            }
        }

        public CreditCard CreateCreditCard(string creditCardType, string creditCardNumber, string creditCardCVV2, string creditCardExpireMonth, string creditCardExpireYear)
        {
            CreditCard card = null;
            CreditCard credCard = new CreditCard();
            credCard.type = creditCardType;
            credCard.number = creditCardNumber;
            credCard.cvv2 = creditCardCVV2;
            credCard.expire_month = System.Convert.ToInt32(creditCardExpireMonth);
            credCard.expire_year = System.Convert.ToInt32(creditCardExpireYear);
            card = credCard.Create(Api);
            return card;
        }
        #endregion
                
        #region Event Handlers
        protected void Page_Init(Object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var email = HttpContext.Current.User.Identity.Name.Trim();
                bool isSuccess = DataBind(email);
            }
            else
            {
                Response.Redirect("~/Account/SignIn.aspx");
            }    
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonUpdate_Click(object sender, EventArgs e)
        {
            var email = TextBoxEmail.Text.Trim();
            var currentPassword = TextBoxCurrentPassword.Text.Trim();
            bool isValid = IsPasswordValid(email, currentPassword);
            if (isValid)
            {
                var newPassword = TextBoxNewPassword.Text.Trim();
                var confirmNewPassword = TextBoxConfirmNewPassword.Text.Trim();
                var newCreditCardType = DropDownListNewCreditCardType.SelectedValue.ToString().Trim();
                var newCreditCardNumber = TextBoxNewCreditCardNumber.Text.Trim();
                var newCreditCardCVV2 = TextBoxNewCreditCardCVV2.Text.Trim();
                var newCreditCardExpireMonth = DropDownListNewCreditCardExpireMonth.SelectedValue.ToString().Trim();
                var newCreditCardExpireYear = DropDownListNewCreditCardExpireYear.SelectedValue.ToString().Trim();
                bool isSuccess = Update(email, newPassword, confirmNewPassword, newCreditCardType, newCreditCardNumber, newCreditCardCVV2, newCreditCardExpireMonth, newCreditCardExpireYear);
                if (isSuccess)
                {
                    FormsAuthentication.RedirectFromLoginPage(TextBoxEmail.Text.Trim(), false);
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