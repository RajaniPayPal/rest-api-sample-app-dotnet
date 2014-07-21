using PayPal;
using PayPal.Api.Payments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PizzaAppMvc4Razor
{
    public class PlaceOrdersController : Controller
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
            selectQuery.Append("credit_card_id ");
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

        private DataTable GetOrders(int userId)
        {
            DataTable datTable = null;
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SELECT ");
            selectQuery.Append("DISTINCT id, ");
            selectQuery.Append("user_id, ");
            selectQuery.Append("payment_id, ");
            selectQuery.Append("state, ");
            selectQuery.Append("amount, ");
            selectQuery.Append("description, ");
            selectQuery.Append("created_at, ");
            selectQuery.Append("updated_at ");
            selectQuery.Append("FROM orders ");
            selectQuery.Append("WHERE user_id = @user_id ORDER BY id DESC LIMIT 1;");
            using (SQLiteCommand commandSQLite = new SQLiteCommand(selectQuery.ToString()))
            {
                commandSQLite.Parameters.AddWithValue("@user_id", userId);
                DataAccessLayer dataAccessObject = new DataAccessLayer();
                datTable = dataAccessObject.Select(commandSQLite);
            }
            return datTable;
        }

        private int GetSignedInUserId(string email)
        {
            int userId = 0;
            DataTable datTable = GetUser(email);
            if (datTable != null && datTable.Rows.Count > 0)
            {
                var distinctRows = (from DataRow dRow in datTable.Rows
                                    where dRow.Field<string>("email") == email
                                    select new { column1 = dRow["id"] }).Distinct();
                if (distinctRows != null)
                {
                    foreach (var row in distinctRows)
                    {
                        userId = Convert.ToInt32(row.column1);
                        break;
                    }
                }
            }
            return userId;
        }

        private string GetSignedInUserCreditCardId(string email)
        {
            string creditCardId = string.Empty;

            CreditCard crdtCard = new CreditCard();
            DataTable datTable = GetUser(email);
            if (datTable != null && datTable.Rows.Count > 0)
            {
                var distinctRows = (from DataRow dRow in datTable.Rows
                                    where dRow.Field<string>("email") == email
                                    select new { column1 = dRow["credit_card_id"] }).Distinct();
                if (distinctRows != null)
                {
                    foreach (var row in distinctRows)
                    {
                        creditCardId = Convert.ToString(row.column1);
                        break;
                    }
                }
            }
            return creditCardId;
        }

        private int GetSignedInUserLastInsertedOrderId(int userId)
        {
            int orderId = 0;
            CreditCard crdtCard = new CreditCard();
            DataTable datTable = GetOrders(userId);
            if (datTable != null && datTable.Rows.Count > 0)
            {
                var distinctRows = (from DataRow dRow in datTable.Rows
                                    select new { column1 = dRow["id"] }).Distinct();
                if (distinctRows != null)
                {
                    foreach (var row in distinctRows)
                    {
                        orderId = Convert.ToInt32(row.column1);
                        break;
                    }
                }
            }
            return orderId;
        }

        private bool Insert(int userId, string payId, string state, string amount, string description, string createdAt, string updatedAt)
        {
            bool isSuccess = false;
            int rowsAffected = 0;
            StringBuilder insertQuery = new StringBuilder();
            insertQuery.Append("INSERT INTO orders");
            insertQuery.Append("(");
            insertQuery.Append("user_id, ");
            insertQuery.Append("payment_id, ");
            insertQuery.Append("state, ");
            insertQuery.Append("amount, ");
            insertQuery.Append("description, ");
            insertQuery.Append("created_at, ");
            insertQuery.Append("updated_at ");
            insertQuery.Append(") ");
            insertQuery.Append("VALUES ");
            insertQuery.Append("(");
            insertQuery.Append("@user_id, ");
            insertQuery.Append("@payment_id, ");
            insertQuery.Append("@state, ");
            insertQuery.Append("@amount,");
            insertQuery.Append("@description, ");
            insertQuery.Append("@created_at, ");
            insertQuery.Append("@updated_at ");
            insertQuery.Append(");");
            using (SQLiteCommand commandSQLite = new SQLiteCommand(insertQuery.ToString()))
            {
                commandSQLite.Parameters.AddWithValue("@user_id", userId);
                commandSQLite.Parameters.AddWithValue("@payment_id", payId);
                commandSQLite.Parameters.AddWithValue("@state", state);
                commandSQLite.Parameters.AddWithValue("@amount", amount);
                commandSQLite.Parameters.AddWithValue("@description", description);
                commandSQLite.Parameters.AddWithValue("@created_at", createdAt);
                commandSQLite.Parameters.AddWithValue("@updated_at", updatedAt);
                DataAccessLayer dataAccessObject = new DataAccessLayer();
                rowsAffected = dataAccessObject.Execute(commandSQLite);
            }
            if (rowsAffected > 0)
            {
                isSuccess = true;
            }
            return isSuccess;
        }

        private bool Insert(int userId, string createdAt, string updatedAt)
        {
            bool isSuccess = false;
            int rowsAffected = 0;
            StringBuilder insertQuery = new StringBuilder();
            insertQuery.Append("INSERT INTO orders");
            insertQuery.Append("(");
            insertQuery.Append("user_id, ");
            insertQuery.Append("created_at, ");
            insertQuery.Append("updated_at ");
            insertQuery.Append(") ");
            insertQuery.Append("VALUES ");
            insertQuery.Append("(");
            insertQuery.Append("@user_id, ");
            insertQuery.Append("@created_at, ");
            insertQuery.Append("@updated_at ");
            insertQuery.Append(");");
            using (SQLiteCommand commandSQLite = new SQLiteCommand(insertQuery.ToString()))
            {
                commandSQLite.Parameters.AddWithValue("@user_id", userId);
                commandSQLite.Parameters.AddWithValue("@created_at", createdAt);
                commandSQLite.Parameters.AddWithValue("@updated_at", updatedAt);
                DataAccessLayer dataAccessObject = new DataAccessLayer();
                rowsAffected = dataAccessObject.Execute(commandSQLite);
            }
            if (rowsAffected > 0)
            {
                isSuccess = true;
            }
            return isSuccess;
        }

        private bool Update(int orderId, string payId, string state, string amount, string description, string updatedAt)
        {
            bool isSuccess = false;
            int rowsAffected = 0;
            StringBuilder updateQuery = new StringBuilder();
            updateQuery.Append("UPDATE orders ");
            updateQuery.Append("SET ");
            updateQuery.Append("payment_id = @payment_id, ");
            updateQuery.Append("state = @state, ");
            updateQuery.Append("amount = @amount, ");
            updateQuery.Append("description = @description, ");
            updateQuery.Append("updated_at = @updated_at ");
            updateQuery.Append("WHERE ");
            updateQuery.Append("id = @id;");
            using (SQLiteCommand commandSQLite = new SQLiteCommand(updateQuery.ToString()))
            {
                commandSQLite.Parameters.AddWithValue("@payment_id", payId);
                commandSQLite.Parameters.AddWithValue("@state", state);
                commandSQLite.Parameters.AddWithValue("@amount", amount);
                commandSQLite.Parameters.AddWithValue("@description", description);
                commandSQLite.Parameters.AddWithValue("@updated_at", updatedAt);
                commandSQLite.Parameters.AddWithValue("@id", orderId);
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

        private string GetApprovalURL(Payment payment)
        {
            string redirectUrl = null;
            List<Links> links = payment.links;
            foreach (Links lnk in links)
            {
                if (lnk.rel.ToLower().Equals("approval_url"))
                {
                    redirectUrl = Server.UrlDecode(lnk.href);
                    break;
                }
            }
            return redirectUrl;
        }

        public Payment CreatePayment(string email, PaymentMethod payMethod, string orderAmount, string orderDescription, string returnUrl, string cancelUrl)
        {
            Payment pay = null;

            Details amountDetails = new Details();
            amountDetails.shipping = "2";
            amountDetails.tax = "1";
            amountDetails.subtotal = orderAmount;

            Amount amount = new Amount();
            amount.currency = "USD";
            int total = Convert.ToInt32(amountDetails.tax) + Convert.ToInt32(amountDetails.shipping) + Convert.ToInt32(orderAmount);
            amount.total = total.ToString();
            amount.details = amountDetails;

            RedirectUrls redirectUrls = new RedirectUrls();
            redirectUrls.return_url = returnUrl;
            redirectUrls.cancel_url = cancelUrl;

            Transaction transaction = new Transaction();
            transaction.amount = amount;
            transaction.description = orderDescription;
            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(transaction);

            Payer payr = new Payer();
            payr.payment_method = payMethod.ToString();

            Payment paymnt = new Payment();
            paymnt.intent = "sale";
            paymnt.payer = payr;
            paymnt.transactions = transactions;
            paymnt.redirect_urls = redirectUrls;

            pay = paymnt.Create(Api);
            return pay;
        }

        public Payment CreatePayment(string email, PaymentMethod payMethod, string orderAmount, string orderDescription)
        {
            Payment pay = null;

            Amount amount = new Amount();
            amount.currency = "USD";
            amount.total = orderAmount;

            Transaction transaction = new Transaction();
            transaction.amount = amount;
            transaction.description = orderDescription;

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(transaction);

            FundingInstrument fundingInstrument = new FundingInstrument();
            CreditCardToken creditCardToken = new CreditCardToken();
            creditCardToken.credit_card_id = GetSignedInUserCreditCardId(email);
            fundingInstrument.credit_card_token = creditCardToken;

            List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
            fundingInstrumentList.Add(fundingInstrument);

            Payer payr = new Payer();
            payr.funding_instruments = fundingInstrumentList;
            payr.payment_method = payMethod.ToString();

            Payment paymnt = new Payment();
            paymnt.intent = "sale";
            paymnt.payer = payr;
            paymnt.transactions = transactions;
            pay = paymnt.Create(Api);
            return pay;
        }        
        
        #endregion

        #region Register
        private SelectListItem[] RegisterPaymentTypes()
        {
            var model = new PlaceOrdersModels();
            model.PaymentTypes = new[]
            {
                new SelectListItem { Text = "credit_card", Value = "credit_card" }, 
                new SelectListItem { Text = "paypal", Value = "paypal" },
            };
            return model.PaymentTypes;
        }
        #endregion

        #region ActionResult

        //
        // GET: /PlaceOrders/

        [Authorize]
        public ActionResult PlaceOrders()
        {
            var model = new PlaceOrdersModels();
            model.PaymentTypes = RegisterPaymentTypes();
            model.PaymentType = string.Empty;

            if (Request.QueryString["order[amount]"] != null && Request.QueryString["order[description]"] != null)
            {
                model.Amount = Request.QueryString["order[amount]"];
                model.Description = Request.QueryString["order[description]"];
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        //
        // POST: /PlaceOrders/

        [Authorize]
        [HttpPost]
        public ActionResult PlaceOrders(PlaceOrdersModels model)
        {
            if (ModelState.IsValid)
            {
                if (Request.QueryString["order[amount]"] != null && Request.QueryString["order[description]"] != null)
                {
                    model.Amount = Request.QueryString["order[amount]"];
                    model.Description = Request.QueryString["order[description]"];

                    var email = User.Identity.Name.Trim();
                    var userId = GetSignedInUserId(email);

                    if (model.PaymentType != null)
                    {
                        if (model.PaymentType.Trim() == "credit_card")
                        {
                            var amount = model.Amount.Trim();
                            var description = model.Description.Trim();
                            Payment pay = null;
                            pay = CreatePayment(email, PaymentMethod.credit_card, amount, description);
                            if (pay != null)
                            {
                                var payId = pay.id;
                                var state = pay.state;
                                DateTime createdDateTime = Convert.ToDateTime(pay.create_time);
                                var createdAt = createdDateTime.ToString("yyyy-MM-dd hh:mm:ss.FFFFF");
                                var updatedAt = createdDateTime.ToString("yyyy-MM-dd hh:mm:ss.FFFFF");
                                bool isSuccess = Insert(userId, payId, state, amount, description, createdAt, updatedAt);
                                if (isSuccess)
                                {
                                    if (state.Trim().ToLower().Equals("approved"))
                                    {
                                        string requestUrl = Request.Url.OriginalString;
                                        string authority = Request.Url.Authority;
                                        string dnsSafeHost = Request.Url.DnsSafeHost;

                                        if (Request.UrlReferrer != null && Request.UrlReferrer.Scheme == "https")
                                        {
                                            requestUrl = requestUrl.Replace("http://", "https://");
                                            requestUrl = requestUrl.Replace(authority, dnsSafeHost);
                                        }
                                        return new RedirectResult("~/Orders/Orders?Status=approved");
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, "Order failed.");
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Order failed.");
                            }
                        }
                        else if (model.PaymentType.Trim() == "paypal")
                        {
                            DateTime createdDateTime = DateTime.Now;
                            var createdAt = createdDateTime.ToString("yyyy-MM-dd hh:mm:ss.FFFFF");
                            var updatedAt = createdDateTime.ToString("yyyy-MM-dd hh:mm:ss.FFFFF");
                            bool isSuccess = Insert(userId, createdAt, updatedAt);
                            if (isSuccess)
                            {
                                int orderId = GetSignedInUserLastInsertedOrderId(userId);
                                string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Orders/Orders?";
                                string requestUrl = Request.Url.OriginalString;
                                string returnUrl = baseURI + "Success=True&OrderID=" + orderId;
                                string cancelUrl = baseURI + "Success=False&OrderID=" + orderId;
                                var amount = model.Amount.Trim();
                                var description = model.Description.Trim();
                                Payment pay = null;

                                pay = CreatePayment(email, PaymentMethod.paypal, amount, description, returnUrl, cancelUrl);
                                if (pay != null)
                                {
                                    var payId = pay.id;
                                    var state = pay.state;
                                    var updatedAtDateTime = Convert.ToDateTime(pay.create_time);
                                    var payUpdatedAt = updatedAtDateTime.ToString("yyyy-MM-dd hh:mm:ss.FFFFF");
                                    bool isUpdateSuccess = Update(orderId, payId, state, amount, description, payUpdatedAt);
                                    if (isUpdateSuccess)
                                    {
                                        string dredirectUrl = GetApprovalURL(pay);
                                        return new RedirectResult(dredirectUrl);
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, "Order failed.");
                                }
                            }
                        }
                    }
                }

            }

            if (model.PaymentTypes == null)
            {
                model.PaymentTypes = RegisterPaymentTypes();
            }
            return View(model);
        }

        #endregion
    }
}
