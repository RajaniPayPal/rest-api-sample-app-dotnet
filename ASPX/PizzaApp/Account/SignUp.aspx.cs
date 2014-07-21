using System;
using System.Linq;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Web.Security;
using PayPal;
using PayPal.Api.Payments;

namespace PizzaApp
{
    public partial class SignUp : System.Web.UI.Page
    {
        #region Data
        private bool CheckIsExistingUser(string email)
        {
            bool isExistingUser = false;
            DataTable datTable = null;
            int rows = 0;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT ");
            selectQuery.Append("count(*) AS NumberOfUsers ");
            selectQuery.Append("FROM users ");
            selectQuery.Append("WHERE email = @email;");
            using (SQLiteCommand commandSQLite = new SQLiteCommand(selectQuery.ToString()))
            {
                commandSQLite.Parameters.AddWithValue("@email", email);
                DataAccessLayer dataAccessObject = new DataAccessLayer();
                datTable = dataAccessObject.Select(commandSQLite);
            }
            if (datTable != null && datTable.Rows.Count > 0)
            {
                var distinctRows = (from DataRow dRow in datTable.Rows
                                    select new { column1 = dRow["NumberOfUsers"] }).Distinct();
                if (distinctRows != null)
                {
                    foreach (var row in distinctRows)
                    {
                        rows = Convert.ToInt32(row.column1);
                        break;
                    }
                }
            }
            if (rows == 1)
            {
                isExistingUser = true;
            }
            return isExistingUser;
        }

        private bool Insert(string email, string password, string passwordConfirmation, string creditCardType, string creditCardNumber, string creditCardCVV2, string creditCardExpireMonth, string creditCardExpireYear)
        {
            bool isSuccess = false;
            int rowsAffected = 0;
            var encryptedPassword = Secure.Encrypt(password);
            var signInCount = 1;
            var dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.FFFFF");
            var currentSignInAt = dateTimeNow;
            var lastSignInAt = currentSignInAt;
            // Set first (current) signed in IP Address
            var signInIPAddress = string.Empty;
            var currentSignInIP = signInIPAddress;
            var lastSignInIP = signInIPAddress;
            var createdAt = dateTimeNow;
            var updatedAt = createdAt;
            CreditCard credCard = CreateCreditCard(creditCardType, creditCardNumber, creditCardCVV2, creditCardExpireMonth, creditCardExpireYear);
            var creditCardId = credCard.id;
            var creditCardDescription = credCard.number;
            StringBuilder insertQuery = new StringBuilder();
            insertQuery.Append("INSERT INTO users");
            insertQuery.Append("(");
            insertQuery.Append("email, ");
            insertQuery.Append("encrypted_password, ");
            insertQuery.Append("sign_in_count, ");
            insertQuery.Append("current_sign_in_at,");
            insertQuery.Append("last_sign_in_at, ");
            insertQuery.Append("current_sign_in_ip, ");
            insertQuery.Append("last_sign_in_ip, ");
            insertQuery.Append("created_at, ");
            insertQuery.Append("updated_at, ");
            insertQuery.Append("credit_card_id, ");
            insertQuery.Append("credit_card_description ");
            insertQuery.Append(") ");
            insertQuery.Append("VALUES ");
            insertQuery.Append("(");
            insertQuery.Append("@email, ");
            insertQuery.Append("@encrypted_password, ");
            insertQuery.Append("@sign_in_count, ");
            insertQuery.Append("@current_sign_in_at,");
            insertQuery.Append("@last_sign_in_at, ");
            insertQuery.Append("@current_sign_in_ip, ");
            insertQuery.Append("@last_sign_in_ip, ");
            insertQuery.Append("@created_at, ");
            insertQuery.Append("@updated_at, ");
            insertQuery.Append("@credit_card_id, ");
            insertQuery.Append("@credit_card_description ");
            insertQuery.Append(");");
            using (SQLiteCommand commandSQLite = new SQLiteCommand(insertQuery.ToString()))
            {
                commandSQLite.Parameters.AddWithValue("@email", email);
                commandSQLite.Parameters.AddWithValue("@encrypted_password", encryptedPassword);
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
            if (rowsAffected > 0)
            {
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
        #endregion PayPal
        
        #region Event Handlers
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonSignUp_Click(object sender, EventArgs e)
        {
            var email = TextBoxEmail.Text.Trim();

            bool isExistingUser = CheckIsExistingUser(email);

            if (isExistingUser)
            {
                divAlertMessage.Visible = true;
                divAlertMessage.Attributes["class"] = "alert fade in alert-error";
                LabelAlertMessage.Text = "Email already exists.";
                TextBoxEmail.Focus();
            }
            else
            {
                var password = TextBoxPassword.Text.Trim();
                var passwordConfirmation = TextBoxConfirmPassword.Text.Trim();
                var creditCardNumber = TextBoxCreditCardNumber.Text.Trim();
                var creditCardCVV2 = TextBoxCreditCardCVV2.Text.Trim();
                var creditCardType = DropDownListCreditCardType.SelectedValue.ToString().Trim();
                var creditCardExpireMonth = DropDownListCreditCardExpireMonth.SelectedValue.ToString().Trim();
                var creditCardExpireYear = DropDownListCreditCardExpireYear.SelectedValue.ToString().Trim();
                bool isSuccess = Insert(email, password, passwordConfirmation, creditCardType, creditCardNumber, creditCardCVV2, creditCardExpireMonth, creditCardExpireYear);
                if (isSuccess)
                {
                    FormsAuthentication.RedirectFromLoginPage(TextBoxEmail.Text.Trim(), false);
                }
            }
        }
        #endregion
    }
}