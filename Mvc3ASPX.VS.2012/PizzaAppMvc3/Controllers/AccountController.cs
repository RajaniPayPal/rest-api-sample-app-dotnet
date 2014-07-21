using System;
using System.Linq;
using System.Web.Mvc;
using PayPal;
using System.Data;
using System.Text;
using System.Data.SQLite;
using System.Web.Security;
using PayPal.Api.Payments;

namespace PizzaAppMvc3
{
    public class AccountController : Controller
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

        private bool Insert(string email, string password, string passwordConfirmation,
            string creditCardType, string creditCardNumber, string creditCardCVV2, string creditCardExpireMonth, string creditCardExpireYear)
        {
            bool isSuccess = false;
            int rowsAffected = 0;      
            var encryptedPassword = Secure.Encrypt(password);
            var signInCount = 1;
            var dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.FFFFF");
            var currentSignInAt = dateTimeNow;
            var lastSignInAt = dateTimeNow;
            var signInIPAddress = string.Empty;
            var currentSignInIP = signInIPAddress;
            var lastSignInIP = signInIPAddress;
            var createdAt = dateTimeNow;
            var updatedAt = dateTimeNow;
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

        private bool Update(string email, string newPassword, string confirmNewPassword,
            string newCreditCardType, string newCreditCardNumber, string newCreditCardCVV2, string newCreditCardExpireMonth, string newCreditCardExpireYear)
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
                                    select new
                                    {
                                        column1 = dRow["sign_in_count"],
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
        #endregion

        #region Paypal
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
        
        #region Register
        private SelectListItem[] RegisterCreditCardTypes(bool isValid)
        {
            var model = new SignUpModel();
            model.CreditCardTypes = new[]
            {
                new SelectListItem { Selected = isValid, Text = "--Select--", Value = string.Empty }, 
                new SelectListItem { Text = "visa", Value = "visa" }, 
                new SelectListItem { Text = "mastercard", Value = "mastercard" },
                new SelectListItem { Text = "discover", Value = "discover" },
                new SelectListItem { Text = "amex", Value = "amex" },
            };
            return model.CreditCardTypes;
        }

        private SelectListItem[] RegisterCreditCardExpireMonths(bool isValid)
        {
            var model = new SignUpModel();
            model.CreditCardExpireMonths = new[]
            {
                new SelectListItem { Selected = isValid, Text = "--Select--", Value = string.Empty }, 
                new SelectListItem { Text = "01", Value = "01" }, 
                new SelectListItem { Text = "02", Value = "02" },
                new SelectListItem { Text = "03", Value = "03" },
                new SelectListItem { Text = "04", Value = "04" },
                new SelectListItem { Text = "05", Value = "05" }, 
                new SelectListItem { Text = "06", Value = "06" },
                new SelectListItem { Text = "07", Value = "07" },
                new SelectListItem { Text = "08", Value = "08" },
                new SelectListItem { Text = "09", Value = "09" }, 
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "11", Value = "11" },
                new SelectListItem { Text = "12", Value = "12" },
            };
            return model.CreditCardExpireMonths;
        }

        private SelectListItem[] RegisterCreditCardExpireYears(bool isValid)
        {
            var model = new SignUpModel();
            model.CreditCardExpireYears = new[]
            {
                new SelectListItem { Selected = isValid, Text = "--Select--", Value = string.Empty }, 
                new SelectListItem { Text = "2013", Value = "2013" }, 
                new SelectListItem { Text = "2014", Value = "2014" }, 
                new SelectListItem { Text = "2015", Value = "2015" }, 
                new SelectListItem { Text = "2016", Value = "2016" }, 
                new SelectListItem { Text = "2017", Value = "2017" }, 
                new SelectListItem { Text = "2018", Value = "2018" }, 
                new SelectListItem { Text = "2019", Value = "2019" }, 
                new SelectListItem { Text = "2020", Value = "2020" }, 
                new SelectListItem { Text = "2021", Value = "2021" }, 
                new SelectListItem { Text = "2022", Value = "2022" }, 
                new SelectListItem { Text = "2023", Value = "2023" }, 
            };
            return model.CreditCardExpireYears;
        }

        private SelectListItem[] RegisterNewCreditCardTypes(bool isValid)
        {
            var model = new ProfileModel();

            model.NewCreditCardTypes = new[]
            {
                new SelectListItem { Selected = isValid, Text = "--Select--", Value = string.Empty }, 
                new SelectListItem { Text = "visa", Value = "visa" }, 
                new SelectListItem { Text = "mastercard", Value = "mastercard" },
                new SelectListItem { Text = "discover", Value = "discover" },
                new SelectListItem { Text = "amex", Value = "amex" },
            };
            return model.NewCreditCardTypes;
        }

        private SelectListItem[] RegisterNewCreditCardExpireMonths(bool isValid)
        {
            var model = new ProfileModel();

            model.NewCreditCardExpireMonths = new[]
            {
                new SelectListItem { Selected = isValid, Text = "--Select--", Value = string.Empty }, 
                new SelectListItem { Text = "01", Value = "01" }, 
                new SelectListItem { Text = "02", Value = "02" },
                new SelectListItem { Text = "03", Value = "03" },
                new SelectListItem { Text = "04", Value = "04" },
                new SelectListItem { Text = "05", Value = "05" }, 
                new SelectListItem { Text = "06", Value = "06" },
                new SelectListItem { Text = "07", Value = "07" },
                new SelectListItem { Text = "08", Value = "08" },
                new SelectListItem { Text = "09", Value = "09" },
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "11", Value = "11" },
                new SelectListItem { Text = "12", Value = "12" },
            };
            return model.NewCreditCardExpireMonths;
        }

        private SelectListItem[] RegisterNewCreditCardExpireYears(bool isValid)
        {
            var model = new ProfileModel();

            model.NewCreditCardExpireYears = new[]
            {
                new SelectListItem { Selected = isValid, Text = "--Select--", Value = string.Empty }, 
                new SelectListItem { Text = "2013", Value = "2013" }, 
                new SelectListItem { Text = "2014", Value = "2014" }, 
                new SelectListItem { Text = "2015", Value = "2015" }, 
                new SelectListItem { Text = "2016", Value = "2016" }, 
                new SelectListItem { Text = "2017", Value = "2017" }, 
                new SelectListItem { Text = "2018", Value = "2018" }, 
                new SelectListItem { Text = "2019", Value = "2019" }, 
                new SelectListItem { Text = "2020", Value = "2020" }, 
                new SelectListItem { Text = "2021", Value = "2021" }, 
                new SelectListItem { Text = "2022", Value = "2022" }, 
                new SelectListItem { Text = "2023", Value = "2023" }, 
            };
            return model.NewCreditCardExpireYears;
        }      
        #endregion

        #region ActionResult

        //
        // GET: /Account/

        public ActionResult SignIn()
        {
            return View();
        }

        //
        // GET: /Account/
        [HttpPost]
        public ActionResult SignIn(SignInModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var email = model.Email.Trim();
                var password = model.Password.Trim();
                bool isValid = IsPasswordValid(email, password);

                if (isValid)
                {
                    int signInCount = GetSignedInUserSignInCount(email);
                    if (signInCount > 0)
                    {
                        bool isSuccess = Update(email);
                    }
                    FormsAuthentication.SetAuthCookie(model.Email.Trim(), model.Remember);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The email or password provided is incorrect.");
                }
            }

            return View(model);
        }

        //
        // GET: /Account/

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/

        public ActionResult SignUp()
        {
            var model = new SignUpModel();
            model.CreditCardTypes = RegisterCreditCardTypes(false);
            model.CreditCardType = string.Empty;
            model.CreditCardExpireMonths = RegisterCreditCardExpireMonths(false);
            model.CreditCardExpireMonth = string.Empty;
            model.CreditCardExpireYears = RegisterCreditCardExpireYears(false);
            model.CreditCardExpireYear = string.Empty;
            return View(model);
        }

        //
        // GET: /Account/

        [HttpPost]
        public ActionResult SignUp(SignUpModel model)
        {            
            if (ModelState.IsValid)
            {
                var email = model.Email.Trim();
                bool isExistingUser = CheckIsExistingUser(email);
                if (isExistingUser)
                {
                    ModelState.AddModelError(string.Empty, "Email already exists.");
                }
                else
                {
                    var password = model.Password.Trim();
                    var passwordConfirmation = model.ConfirmPassword.Trim();
                    var creditCardNumber = model.CreditCardNumber.Trim();
                    var creditCardCVV2 = model.CreditCardCVV2.Trim();
                    var creditCardType = model.CreditCardType.Trim();
                    var creditCardExpireMonth = model.CreditCardExpireMonth.Trim();
                    var creditCardExpireYear = model.CreditCardExpireYear.Trim();
                    bool isSuccess = Insert(email, password, passwordConfirmation, creditCardType, creditCardNumber, creditCardCVV2, creditCardExpireMonth, creditCardExpireYear);
                    if (isSuccess)
                    {
                        FormsAuthentication.SetAuthCookie(model.Email, false /* createPersistentCookie */);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Registration failed.");
                    }
                }
            }                      

            if (model.CreditCardTypes == null)
            {
                model.CreditCardTypes = RegisterCreditCardTypes(true);
            }        

            if (model.CreditCardExpireMonths == null)
            {
                model.CreditCardExpireMonths = RegisterCreditCardExpireMonths(true);
            }

            if (model.CreditCardExpireYears == null)
            {
                model.CreditCardExpireYears = RegisterCreditCardExpireYears(true);
            }
            return View(model);
        }

        private bool DataBind(ProfileModel model, string email)
        {
            bool isSuccess = false;
            model.Email = email;
            DataTable datTable = GetUser(email);
            if (datTable != null && datTable.Rows.Count > 0)
            {
                string creditCardId = string.Empty;
                if (datTable.Rows[0]["credit_card_id"] != DBNull.Value)
                {
                    creditCardId = Convert.ToString(datTable.Rows[0]["credit_card_id"]);
                }
                CreditCard crdtCard = CreditCard.Get(Api, creditCardId);
                model.CurrentCreditCardNumber = crdtCard.number.Trim();
                isSuccess = true;
            }
            return isSuccess;
        }

        //
        // GET: /Account/
        
        [Authorize]
        public ActionResult Profile()
        {
            var model = new ProfileModel();
            model.NewCreditCardTypes = RegisterNewCreditCardTypes(false);
            model.NewCreditCardType = string.Empty;
            model.NewCreditCardExpireMonths = RegisterNewCreditCardExpireMonths(false);
            model.NewCreditCardExpireMonth = string.Empty;
            model.NewCreditCardExpireYears = RegisterNewCreditCardExpireYears(false);
            model.NewCreditCardExpireYear = string.Empty;

            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name.Trim();
                bool isSuccess = DataBind(model, email);
            }
            else
            {
                RedirectToAction("Index", "Home");
            }    

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Profile(ProfileModel model)
        {
            if (ModelState.IsValid)
            {
                bool changeProfileSucceeded = false;
                try
                {
                    var email = User.Identity.Name.Trim();
                    bool isValid = IsPasswordValid(email, model.CurrentPassword.Trim());
                    if (isValid)
                    {
                        var newPassword = model.NewPassword.Trim();
                        var confirmNewPassword = model.ConfirmNewPassword.Trim();
                        var newCreditCardType = model.NewCreditCardType.Trim();
                        var newCreditCardNumber = model.NewCreditCardNumber.Trim();
                        var newCreditCardCVV2 = model.NewCreditCardCVV2.Trim();
                        var newCreditCardExpireMonth = model.NewCreditCardExpireMonth.Trim();
                        var newCreditCardExpireYear = model.NewCreditCardExpireYear.Trim();

                        bool isSuccess = Update(email, newPassword, confirmNewPassword,
                            newCreditCardType, newCreditCardNumber, newCreditCardCVV2, newCreditCardExpireMonth, newCreditCardExpireYear);
                        if (isSuccess)
                        {
                            changeProfileSucceeded = true;                           
                        }                        
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The current password provided is incorrect.");
                    }                    
                }
                catch (Exception)
                {
                    changeProfileSucceeded = false;
                }

                if (changeProfileSucceeded)
                {               
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Profile update failed.");
                }
            }

            if (model.NewCreditCardTypes == null)
            {
                model.NewCreditCardTypes = RegisterNewCreditCardTypes(true);
            }

            if (model.NewCreditCardExpireMonths == null)
            {
                model.NewCreditCardExpireMonths = RegisterNewCreditCardExpireMonths(true);
            }

            if (model.NewCreditCardExpireYears == null)
            {
                model.NewCreditCardExpireYears = RegisterNewCreditCardExpireYears(true);
            }
            return View(model);
        }

        #endregion
    }
}
