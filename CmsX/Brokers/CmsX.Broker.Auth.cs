/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.3
	Build:		20151223
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Started
	20101206:	Changed LINK_LOGIN
	20110505:	Changed user to _User
	20140102:	Moved LINK_DESTINATION to common Cms.LINK_DESTINATION
	20140112:	Refactored constructor
	20151223:	Renamed CmsXCSV to CmsXExport 
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {

	using System;
	using System.Xml;

	/// <summary>
	/// The CmsXBrokerAuth class implements an x_broker to connect to the CmsXBrokerAuth service
	/// </summary>
	public class CmsXBrokerAuth : CmsXBrokerBase {
		#region Invisible properties
		#endregion

		#region Constant names
		private const string logid = "CmsXBrokerAuth.";
		private const string emailTemplate = "CmsXBrokerAuth.xsl";
		#endregion

		#region Constant error strings
		private const string error_no_error = "Not an error";
		private const string error_passport = "CmsXBrokerAuth error:: ";
		private const string error_passport_test = " test error";
		private const string error_passport_type = " No such passport type";
		private const string error_passport_example = " Example error";
		private const string error_passport_login = " Login error";
		private const string error_passport_summary = " Summary error";
		private const string error_passport_register = " Registration error";
		private const string error_passport_change = " Change password error";
		private const string error_passport_forgot = " Forgotten password error";
		private const string error_passport_locked = " Locked account error";
		private const string error_passport_unlock = " Unlock account error";
		private const string error_passport_email = " _User.Email sending error";
		private const string error_passport_gethint = " Hint retrieval error";
		private const string error_passport_checkhint = " Hint answer match error";
		private const string error_passport_result_null = "null result";
		private const string error_passport_result_error = "bad result";
		private const string error_passport_result_login = "Incorrect username or password";
		private const string error_passport_result_token = "Token invalid";
		private const string error_passport_result_usernot = "User not found";
		private const string error_passport_template = " Template error for: ";
		#endregion

		#region Constants
		private const string QUERY_TOKEN = "token";
		private const string FIELD_HINT_ANSWER = "HintAnswerTest";
		private const string FIELD_HINT_QUESTION = "HintQuestion";

		private const string LINK_REGISTER = "cmsauth_register";
		private const string LINK_FORGOT = "cmsauth_forgot";
		private const string LINK_INFO = "cmsauth_register_info";
		private const string LINK_LOGIN = "site_login";
		private const string LINK_LOCKED = "cmsauth_locked";
		private const string LINK_HINTBAD = "cmsauth_hintnotok";
		private const string LINK_NOTFOUND = "cmsauth_notfound";
		private const string LINK_PROFILE = "cmsauth_profile";

		private const int VERIFY_OK = 0;
		private const int VERIFY_LOGIN_BAD = 1;
		private const int VERIFY_TOKEN_EXPIRED = 2;
		private const int VERIFY_TOKEN_INVALID = 3;
		private const int VERIFY_USER_NOTFOUND = 4;
		#endregion

		#region Visible properties
		#endregion

		#region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerAuth(CmsX thispage) : base(thispage, typeof(CmsXBrokerAuth), logid) {
		}
		#endregion

		#region Public methods
		public override void Process(string type) {
			switch (type) {
				case "login":
					login(UserProfile.Value(Cms.PROFILE_LINK_DESTINATION));
					break;
				case "logout":
					logout();
					break;

				// following not implemented / tested
				case "token":
					token();
					break;
				case "summary":
					summary();
					break;
				case "register":
					register();
					break;
				case "change":
					change();
					break;
				case "forgot":
					forgot();
					break;
				case "unlock":
					unlock();
					break;
				case "hint":
					hint();
					break;
				case "checkhint":
					checkhint();
					break;
				case "share":
					share();
					break;
				default:
					throw (new XException("error_passport_type", String.Concat(error_passport, error_passport_type)));
			}
		}
		#endregion

		#region Protected methods
		#endregion

		#region Private methods
		/// <summary>Login to Passport</summary>
		private void login(string link) {
			login();
            summary();
			if (link != "" && link != null) {
				UserProfile.Clear(Cms.PROFILE_LINK_DESTINATION);
				throw (new displayException(link));
			}
		}
		/// <summary>Login to Passport</summary>
		private void login() {
			try {
				xLogger.Debug("login:", _User.Username, "/", _User.Password);
				bool expireToken = getRemember();
				xLogger.Debug("login:", _User.Username, "/", _User.Password, "/", expireToken.ToString());
				_AdminX.Login(_User.Username, _User.Password);
				xLogger.Debug("login::result:", _AdminX.ItemXml.OuterXml);

				int verifyResult = verify(_AdminX.ItemXml);
				if (verifyResult == VERIFY_OK) {
					iBurstCmsXAuthUser usr = new iBurstCmsXAuthUser(_AdminX.ItemXml.SelectSingleNode("//user") as XmlElement);
					xLogger.Debug("login", "::usr.Firstname:", usr.Firstname, "::usr.Surname:", usr.Surname);
					_User.Set(usr.Token, usr.ID, usr.Username, usr.Password, usr.Firstname, usr.Surname);
					xLogger.Info("login:", _User.Token, "/", _User.UserID);

					getLoginProfile(usr);

					// now set a cookie if it doesnt expire
					if (!expireToken)
						((CmsXProfileX)UIPage).Cooker.Token = _User.Token;
				} else if (verifyResult == VERIFY_TOKEN_EXPIRED) {	// Too many login attempts
					locked();
				} else if (verifyResult == VERIFY_USER_NOTFOUND) {	// User not found
					throw (new displayException(LINK_NOTFOUND));
				} else if (verifyResult == VERIFY_LOGIN_BAD) {
					throw (new displayException(LINK_FORGOT));
				} else {
					xLogger.Info("login:", verifyResult.ToString());
				}
			} catch (displayException e) {
				throw e;
			} catch (iBurstCmsXAuthException e) {
				throw (new XException("error_passport_login", String.Concat(error_passport_login, "::", e.Code, ": '", e.Message, "'")));
			} catch (Exception e) {
				throw (new XException("error_passport_login", String.Concat(error_passport_login, "::", e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Call access to establish a user's admin patterns</summary>
		private void getLoginProfile(iBurstCmsXAuthUser usr) {
			xLogger.Debug("getLoginProfile::userid:", usr.ID, "::usergroup:", usr.Group);
			_AdminX.Rights(_User.Token);
			int verifyResult = verify(_AdminX.ItemXml);
			if (verifyResult != VERIFY_OK) {
				logout(LINK_PROFILE);
			} else {
				xLogger.Debug("login::result:", _AdminX.ItemXml.OuterXml);
				XmlElement rights = _AdminX.ItemXml.SelectSingleNode("//rights") as XmlElement;
				if (rights == null) {
					logout(LINK_PROFILE);
				} else {
					/* Need to figure how blogx.AdminX rights can be used...
					UserProfile.AddNode("accessxml", rights);
					foreach (XmlNode right in rights.ChildNodes) {
						xLogger.Debug("right:", right.Name);
						UserProfile.AddPattern("access", right.Name);
					}
					*/
					// for now use Group as access right
					UserProfile.SetPattern("login", usr.Group);
				}
			}
		}

		/// <summary>Logout of account</summary>
		private void logout() {
			logout(LINK_LOGIN);
		}
		/// <summary>Logout of account</summary>
		private void logout(string link) {
			string token = UIPage.GetQueryParam(QUERY_TOKEN);
			token = (token == "") ? _User.Token : token;
			xLogger.Info("logout:", token, ":link:", link);
			_AdminX.Logout(token);
			_User.Reset();
			UserProfile.ClearItems("accessxml");
			throw (new displayException(link));
		}
		#endregion

		#region Methods not implemented or tested
		/// <summary>Login to Passport (using cookie token)</summary>
		private void token() {
			xLogger.Debug("CookieToken:", _User.CookieToken);
			token(_User.CookieToken);
		}
		/// <summary>Login to Passport (using token)</summary>
		private void token(string token) {
			try {
				if (String.IsNullOrEmpty(token))	// try Query parameter
					token = UIPage.GetQueryParam(QUERY_TOKEN);
				xLogger.Debug("token:", token);
				_AdminX.Validate(token);
				xLogger.Debug("token:", _AdminX.ItemXml.OuterXml);
/* originally...
				int verifyResult = verify();
				if (verifyResult == VERIFY_OK) {	// ie is iPassport User
					_User.Set(Root.GetAttribute("Token"), Root.GetAttribute("UserID"), Root.SelectSingleNode("UserName").InnerText, Root.SelectSingleNode("Password").InnerText, Root.SelectSingleNode("FirstName").InnerText, Root.SelectSingleNode("Surname").InnerText);
					xLogger.Info("login:", _User.Token, "/", _User.UserID));
				} else if (verifyResult == VERIFY_TOKEN_INVALID) {	// token invalid
					xLogger.Info("token:", verifyResult.ToString()));
					((WebsiteXProfileX)UIPage).Cooker.Token = "";
					_User.Reset();
				} else {
					xLogger.Info("token:", verifyResult.ToString()));
					throw new Exception(String.Concat("Bad token validate result: ", verifyResult.ToString()));
				}
*/
			} catch (displayException e) {
				throw e;
			} catch (iBurstCmsXAuthException e) {
				throw (new XException("error_passport_login", String.Concat(error_passport_login, e.Message)));
			} catch (Exception e) {
				throw (new XException("error_passport_login", String.Concat(error_passport_login, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Register user (ie account holder)</summary>
		private void register() {
			try {
				//xLogger.Info("register:"));
				//xLogger.Debug("register:", UIPage.UserProfile.ProfileXml.OuterXml));
				//result = _AdminX.RegisterUser(_User.Username, _User.Password, _User.Firstname, _User.Surname, _User.Email, "", _User.Cellno);
				//xLogger.Debug("register:", Root.OuterXml));
				//int verifyResult = verify();
				//if (verifyResult == VERIFY_OK) {	// ie is iPassport User
				//}
			}
			catch (XException e) {
				throw (new XException("error_passport_register", String.Concat(error_passport_register, e.Message)));
			}
			catch (Exception e) {
				throw (new XException("error_passport_register", String.Concat(error_passport_register, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Change password</summary>
		private void change() {
			try {
				xLogger.Info("change:");
			}
			catch (XException e) {
				throw (new XException("error_passport_change", String.Concat(error_passport_change, e.Message)));
			}
			catch (Exception e) {
				throw (new XException("error_passport_change", String.Concat(error_passport_change, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Get password and hint info</summary>
		private void hint() {
			try {
				xLogger.Info("hint:", _User.Username);
				getHint();
			}
			catch (Exception e) {
				throw (new XException("error_passport_gethint", String.Concat(error_passport_gethint, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Check given hint against hint info</summary>
		private void checkhint() {
			try {
				xLogger.Info("checkhint:", _User.Username);
				getHint();
				string answer = _User.Get(FIELD_HINT_ANSWER).ToLower();
				if (answer != _User.HintAnswer.ToLower()) {
					throw (new displayException(LINK_HINTBAD));
				}
			}
			catch (displayException e) {
				throw e;
			}
			catch (XException e) {
				throw (new XException("error_passport_checkhint", String.Concat(error_passport_checkhint, e.Message)));
			}
			catch (Exception e) {
				throw (new XException("error_passport_checkhint", String.Concat(error_passport_checkhint, e.Message, " - ", e.StackTrace)));
			}
		}

		private void getHint() {
			//result = _AdminX.GetHint(_User.Username);
			//int verifyResult = verify();
			//xLogger.Debug("getHint:", Root.OuterXml));
			//if (verifyResult == VERIFY_OK) {
			//    _User.Password = Root.SelectSingleNode(SELECT_USER_PASSWORD).InnerText;
			//    _User.HintID = Root.SelectSingleNode(SELECT_USER_QUESTION).InnerText;
			//    UserProfile.Add(FIELD_HINT_QUESTION, String.Concat("Hint", _User.HintID));
			//    _User.HintAnswer = Root.SelectSingleNode(SELECT_USER_ANSWER).InnerText;
			//    xLogger.Debug("getHint:", logsep, "id:", _User.HintID, logsep, "answer:", _User.HintAnswer, logsep, "password:", _User.Password));
			//} else {
			//    throw new XException("error_passport_gethint", error_passport_gethint);
			//}
		}

		/// <summary>Forgot password</summary>
		private void forgot() {
			try {
				//xLogger.Info("forgot:"));
				//result = _AdminX.GetPassword(_User.Username);
				//int verifyResult = verify();
				//xLogger.Debug("forgot:", Root.OuterXml));
				//if (verifyResult == VERIFY_OK) {
				//    XmlNode email = Root.SelectSingleNode(SELECT_USER_EMAIL);
				//    xLogger.Info("forgot:", logsep, (email == null) ? "" : email.InnerText));
				//    send(email.InnerText);
				//}
			}
			catch (XException e) {
				throw (new XException("error_passport_forgot", String.Concat(error_passport_forgot, e.Message)));
			}
			catch (Exception e) {
				throw (new XException("error_passport_forgot", String.Concat(error_passport_forgot, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Get user summary info</summary>
		private void summary() {
			xLogger.Debug("summary:");
			try {
				CmsXExport exportutil = new CmsXExport(UIPage);
				exportutil.LoadFilters();
				xLogger.Debug("summary:ok");
			}
			catch (XException e) {
				throw (new XException("error_passport_summary", String.Concat(error_passport_summary, e.Message)));
			}
			catch (Exception e) {
				throw (new XException("error_passport_summary", String.Concat(error_passport_summary, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Get user summary info</summary>
		private void locked() {
			try {
				//result = _AdminX.GetUser(_User.Username);
				//if (verify() == VERIFY_OK) {
				//    _User.Token = Root.GetAttribute("Token");
				//    Root.SetAttribute("type", "account_locked");
				//    Root.SetAttribute("url", String.Concat(config.Host, config.Path));
				//    XmlNode email = Root.SelectSingleNode(SELECT_USER_EMAIL);
				//    xLogger.Info("locked:", logsep, (email == null) ? "" : email.InnerText));
				//    send(email.InnerText);
				//    _User.Reset();
				//    throw (new displayException(LINK_LOCKED));
				//} else {
				//    throw (new Exception("GetUser not verified"));
				//}
			}
			catch (displayException e) {
				throw e;
			}
			catch (XException e) {
				throw (new XException("error_passport_locked", String.Concat(error_passport_locked, e.Message)));
			}
			catch (Exception e) {
				throw (new XException("error_passport_locked", String.Concat(error_passport_locked, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Unlock account</summary>
		private void unlock() {
			try {
				//xLogger.Info("unlock:", UIPage.Parameters.Token));
				//if (!_AdminX.UnlockUser(UIPage.Parameters.Token))
				//    throw (new displayException(LINK_LOCKED));
			}
			catch (displayException e) {
				throw e;
			}
			catch (XException e) {
				throw (new XException("error_passport_unlock", String.Concat(error_passport_unlock, e.Message)));
			}
			catch (Exception e) {
				throw (new XException("error_passport_unlock", String.Concat(error_passport_unlock, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Retrieve user token (after registration)</summary>
		private void share() {
			token();
			if (!String.IsNullOrEmpty(_User.FriendID) && !String.IsNullOrEmpty(_User.FileID))
				throw (new displayException("profile_share_thankyou_"));	// ie need to accept share invite
		}

		#endregion

		#region Private utility methods
		/// <summary>verifies the result from the web service call</summary>
		private int verify(XmlDocument ResultDoc) {
			int returnCode = VERIFY_OK;
			XmlElement Root = ResultDoc.SelectSingleNode("//blogX") as XmlElement;
			XmlElement resltcode = Root.SelectSingleNode(Cms.SELECT_RESULT_CODE) as XmlElement;
			if (resltcode != null) {
				xLogger.Debug("verify::resltcode:", resltcode.OuterXml);
				if (resltcode.InnerText != "0") {
					string excode = "error_passport_result_error";
					string exdesc = String.Concat(error_passport, error_passport_result_error, " code: '", resltcode.InnerText, "' description: '", Root.SelectSingleNode(Cms.SELECT_RESULT_DESC).InnerText, "'");
					bool throwException = true;
					switch(resltcode.InnerText) {
						case "error_password":
							excode = "error_passport_result_login";
							exdesc = error_passport_result_login;
							returnCode = VERIFY_LOGIN_BAD;
							throwException = false;
							break;
						case "error_expired":
							excode = "error_passport_result_login";
							exdesc = error_passport_result_login;
							returnCode = VERIFY_TOKEN_EXPIRED;
							throwException = false;
							break;
						case "error_notoken":
							excode = "error_passport_result_token";
							exdesc = error_passport_result_token;
							returnCode = VERIFY_TOKEN_INVALID;
							throwException = false;
							break;
						case "error_notexist":
							excode = "error_passport_result_usernot";
							exdesc = error_passport_result_usernot;
							returnCode = VERIFY_USER_NOTFOUND;
							throwException = false;
							break;
						default:
							break;
					}
					if (throwException) 
						throw (new iBurstCmsXAuthException(excode, exdesc));
				}
			} else {
				xLogger.Debug("verify::error_passport_result_null");
				throw (new iBurstCmsXAuthException("error_passport_result_null", String.Concat(error_passport, error_passport_result_null)));
			}
			return returnCode;
		}

/*
		/// <summary>verifies the result from the web service call for getting the user's rights</summary>
		private int verifyAccess() {
			int returnCode = VERIFY_OK;
			XmlElement resltcode = Root.SelectSingleNode(SELECT_ACCESS_CODE) as XmlElement;
			if (resltcode != null) {
				xLogger.Debug("verify::resltcode:", resltcode.OuterXml));
				if (resltcode.InnerText != "0") {
					string excode = "error_passport_result_error";
					string exdesc = String.Concat(error_passport, error_passport_result_error, " code: '", resltcode.InnerText, "' description: '", Root.SelectSingleNode(SELECT_RESULT_DESC).InnerText, "'");
					bool throwException = true;
					switch (resltcode.InnerText) {
						case "1000":
							excode = "error_passport_result_login";
							exdesc = error_passport_result_login;
							returnCode = VERIFY_LOGIN_BAD;
							throwException = false;
							break;
						case "1003":
							excode = "error_passport_result_login";
							exdesc = error_passport_result_login;
							returnCode = VERIFY_TOKEN_EXPIRED;
							throwException = false;
							break;
						case "2070":
							excode = "error_passport_result_token";
							exdesc = error_passport_result_token;
							returnCode = VERIFY_TOKEN_INVALID;
							throwException = false;
							break;
						case "2053":
							excode = "error_passport_result_usernot";
							exdesc = error_passport_result_usernot;
							returnCode = VERIFY_USER_NOTFOUND;
							throwException = false;
							break;
						default:
							break;
					}
					if (throwException)
						throw (new iBurstCmsXAuthException(excode, exdesc));
				}
			} else {
				xLogger.Debug("verify::error_passport_result_null"));
				throw (new iBurstCmsXAuthException("error_passport_result_null", String.Concat(error_passport, error_passport_result_null)));
			}
			return returnCode;
		}

		/// <summary>Emails the user details, including password</summary>
		private string send(string email) {
			string sendtext = "";
			x_email thisemail = new x_email();
			try {
				thisemail.Bcc = "alanben@clickclickBOOM.com";
				thisemail.To = email;
				thisemail.Type = x_emailtype.html;
				thisemail.Send(Root, _GetTemplate(emailTemplate));
				sendtext = thisemail.Message;
			}
			catch(System.Exception e) {
				string msg = String.Concat(error_passport, error_passport_email, e.Message);
				xLogger.Info("email:", _User.Username, logsep, "Email_NotOK", logsep, email, logsep, thisemail.Server, logsep, msg));
				throw(new iBurstCmsXAuthException("error_passport_email", msg));
			}
			return sendtext;
		}
*/

		private bool getRemember() {
			xLogger.Debug("getRemember:", "RememberMe:", _User.RememberMe);
			bool noremember = (_User.RememberMe != "on");
			xLogger.Info("getRemember:", "noremember:", noremember.ToString());
			_User.RememberMe = "off";	// set off automatically, for safety
			return noremember;
		}

		#endregion
	}

	/// <summary>
	/// An exception class specific to iBurstPassport and derived classes
	/// </summary>
	public class iBurstCmsXAuthException : XException {
		/// <summary>Constructor</summary>
		/// <param name="cde">Error code</param>
		/// <param name="message">Error message</param>
		public iBurstCmsXAuthException(string cde, string message) : base(cde, message) {
		}
	}

	/// <summary>
	/// A utility class to hold the user info from the AdminX login (move to own file at later stage)
	/// </summary>
	public class iBurstCmsXAuthUser {
		#region Invisible properties
		#endregion

		#region Constants
		#endregion

		#region Visible properties
		private XmlElement user;
		/// <summary>
		/// 
		/// </summary>
		public XmlElement UserElement {
			get { return user; }
			set { user = value; }
		}
		private string token;
		/// <summary>
		/// 
		/// </summary>
		public string Token {
			get { return token; }
			set { token = value; }
		}
		private string group;
		/// <summary>
		/// 
		/// </summary>
		public string Group {
			get { return group; }
			set { group = value; }
		}
		private string id;
		/// <summary>
		/// 
		/// </summary>
		public string ID {
			get { return id; }
			set { id = value; }
		}
		private string username;
		/// <summary>
		/// 
		/// </summary>
		public string Username {
			get { return username; }
			set { username = value; }
		}
		private string password;
		/// <summary>
		/// 
		/// </summary>
		public string Password {
			get { return password; }
			set { password = value; }
		}
		private string firstname;
		/// <summary>
		/// 
		/// </summary>
		public string Firstname {
			get { return firstname; }
			set { firstname = value; }
		}
		private string surname;
		/// <summary>
		/// 
		/// </summary>
		public string Surname {
			get { return surname; }
			set { surname = value; }
		}
		private string email;
		/// <summary>
		/// 
		/// </summary>
		public string Email {
			get { return email; }
			set { email = value; }
		}
		private string telno;
		/// <summary>
		/// 
		/// </summary>
		public string Telno {
			get { return telno; }
			set { telno = value; }
		}
		#endregion

		#region Constructors/Destructors
		/// <summary>Constructor</summary>
		public iBurstCmsXAuthUser(XmlElement userel) {
			user = userel;

			Username = userel.GetAttribute("name");
			ID = userel.GetAttribute("id");
			Group = userel.GetAttribute("group");
			Token = userel.GetAttribute("token");
			Password = userel.GetAttribute("password");
			
			Firstname = userel.SelectSingleNode("firstname").InnerText;
			Surname = userel.SelectSingleNode("surname").InnerText;
			Email = userel.SelectSingleNode("email").InnerText;
			Telno = userel.SelectSingleNode("telno").InnerText;
		}
		#endregion

		#region Public methods
		#endregion
	}
}
