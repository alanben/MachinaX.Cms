
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
	20110902:	Updated _CustomerWS and passportAdminWS (related to name in namespace update)
	20111006:	Refactoring to bring CmsXBrokerPassport in line with CmsXBrokerAuth
	20120712:	Changed login so that summary (ie loading csv filers) is not controlled
	20131023:	Changed _User.SetName to _User.Set at login
	20140102:	Moved LINK_DESTINATION to Cms.LINK_DESTINATION
	20140112:	Refactored constructor
	20141023:	Added handling of account on-hold
	20151223:	Renamed CmsXCSV to CmsXExport 
	20160601:	Updated send to get default Email values from Config
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {

	using clickclickboom.machinaX.emailX;
	using clickclickboom.machinaX.recaptchaX;
	using System;
	using System.Xml;

	/// <summary>
	/// The CmsXBrokerPassport class implements an x_broker to connect to the CmsXBrokerPassport service
	/// </summary>
	public class CmsXBrokerPassport : CmsXBrokerBase {
		#region Invisible properties
		#endregion

		#region Constant names
		private const string logid = "CmsXBrokerPassport";
		private const string logsep = ",";
		private const string root = "<CmsXBrokerPassport/>";
		private const string EMAILTEMPLATE = "CmsXBrokerPassport.xsl";
		#endregion

		#region Constant error strings
		private const string error_no_error = "Not an error";
		private const string error_passport = "CmsXBrokerPassport error:: ";
		private const string error_passport_test = " test error";
		private const string error_passport_type = " No such passport type";
		private const string error_passport_example = " Example error";
		private const string error_passport_login = " Login error";
		private const string error_passport_summary = " Summary error";
		private const string error_passport_recaptcha = " ReCaptcha error";
		private const string error_passport_register = " Registration error";
		private const string error_passport_change = " Change password error";
		private const string error_passport_forgot = " Forgotten password error";
		private const string error_passport_locked = " Locked account error";
		private const string error_passport_unlock = " Unlock account error";
		private const string error_passport_email = " Sending error";
		private const string error_passport_gethint = " Hint retrieval error";
		private const string error_passport_checkhint = " Hint answer match error";
		private const string error_passport_result_null = "null result";
		private const string error_passport_result_error = "bad result";
		private const string error_passport_result_login = "Incorrect username or password";
		private const string error_passport_result_token = "Token invalid";
		private const string error_passport_result_usernot = "User not found";
		private const string error_passport_result_duplicate = "User already exists";
		private const string error_passport_result_onhold = "Account on-hold";
		private const string error_passport_template = " Template error for: ";
		#endregion

		#region Constants
		private const string QUERY_TOKEN = "token";
		private const string FIELD_HINT_ANSWER = "HintAnswerTest";
		private const string FIELD_HINT_QUESTION = "HintQuestion";
		
		private const string SELECT_RESULT_CODE = "/Result/ResultCode";
		private const string SELECT_ACCESS_CODE = "//ServicesX/Result/Result_Code";
		private const string SELECT_RESULT_DESC = "/Result/Description";
		private const string SELECT_USER_EMAIL = "//EMail";
		private const string SELECT_USER_PASSWORD = "//Password";
		private const string SELECT_USER_QUESTION = "//QuestionID";
		private const string SELECT_USER_ANSWER = "//Answer";
		private const string SELECT_IPASSPORT = "iPassport";

		private const string LINK_REGISTER = "passport_register";
		private const string LINK_FORGOT = "passport_forgot";
		private const string LINK_INFO = "passport_register_info";
		private const string LINK_LOGIN = "site_login";
		private const string LINK_LOCKED = "passport_locked";
		private const string LINK_HINTBAD = "passport_hintnotok";
		private const string LINK_NOTFOUND = "passport_notfound";
		private const string LINK_PROFILE = "passport_profile";
		private const string LINK_RECAPTCHA = "passport_recaptcha";
		private const string LINK_INVALID = "passport_invalid";

		private const int VERIFY_OK = 0;
		private const int VERIFY_LOGIN_BAD = 1;
		private const int VERIFY_LOGIN_LOCKED = 2;
		private const int VERIFY_TOKEN_INVALID = 3;
		private const int VERIFY_USER_NOTFOUND = 4;
		private const int VERIFY_USER_DUPLICATE = 5;
		private const int VERIFY_LOGIN_ONHOLD = 6;
		#endregion

		#region Visible properties
		private XmlNode result;
		/// <summary>The result node</summary>
		/// <value>The result of the web service call to PassportX</value>
		public XmlNode Result {
			get { return result; }
			set { result = value; }
		}
		/// <summary>The result document</summary>
		/// <value>The XmlDocument containing the result of the web service call to PassportX</value>
		private XmlDocument resultDoc;
		public XmlDocument ResultDoc {
			get {
				if (resultDoc == null) {
					resultDoc = new XmlDocument();
					resultDoc.LoadXml(result.OuterXml);
				//} else {
				//	resultDoc = result.OwnerDocument;
				}
				return resultDoc;
			}
		}
		public XmlElement Root {
			get { return result.SelectSingleNode("/") as XmlElement; }
		}
		#endregion

		#region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerPassport(CmsXProfileX thispage) : base(false, thispage, typeof(CmsXBrokerPassport), logid) {
			xLogger.Debug("CmsXBrokerPassport");
		}
		#endregion

		#region Public methods
		public override void Process(string type) {
			switch (type) {
				case "cmslogin":
					login(UserProfile.Value(Cms.LINK_DESTINATION), true);
					break;
				case "login":
					login(UserProfile.Value(Cms.LINK_DESTINATION), false);
					break;
				case "loginemail":
					login(UserProfile.Value(Cms.LINK_DESTINATION), false, true);
					break;
				case "loginonly":
					login();
					break;
				case "validate":
					validate();
					break;
				case "token":
					token();
					break;
				case "summary":
					summary();
					break;
				case "register":
					register();
					break;
				case "recaptcha":
					recaptcha();
					break;
				case "recaptcha2":
					recaptcha2();
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
				case "logout":
					logout();
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
					throw (new x_exception("error_passport_type", String.Concat(error_passport, error_passport_type)));
			}
		}
		#endregion

		#region Protected methods
		#endregion

		#region Private methods
		/// <summary>Login to Passport</summary>
		private void login(string link, bool wantSummary) {
			login(link, wantSummary, false);
		}
		/// <summary>Login to Passport</summary>
		private void login(string link, bool wantSummary, bool email2username) {
			xLogger.Debug("login", "::link:", link, "::email2username:", email2username.ToString());

			if (email2username) {
				xLogger.Debug("login", "::Email:", _User.Email, "::Username:", _User.Username);
				_User.Username = _User.Email;
			}
			login();
			if (wantSummary) {
				summary();
			}
			xLogger.Debug("login::link:", link);
			if (!String.IsNullOrEmpty(link)) {
				UserProfile.Clear(Cms.LINK_DESTINATION);
				throw (new displayException(link));
			}
		}

		/// <summary>Login to Passport</summary>
		private void login() {
			xLogger.Debug("_login:", _User.Username, "/", _User.Password);
			try {
				bool expireToken = !_User.Remember;
				xLogger.Debug("_login:", _User.Username, "/", _User.Password, "/", expireToken.ToString());

				result = _CustomerWS.Login(_User.Username, _User.Password, expireToken);
				int verifyResult = verify();
                xLogger.Info("_login:", _User.Username, "/", _User.Password, "::result:", verifyResult.ToString());

				if (verifyResult == VERIFY_OK) {	// ie is iPassport User
                    _User.Set(Root.GetAttribute("Token"), Root.GetAttribute("UserID"));
                    result = _CustomerWS.GetUserID(_User.PersonIID);
					//_User.SetName(Root.SelectSingleNode("FirstName").InnerText, Root.SelectSingleNode("Surname").InnerText);
					_User.Set(Root);
					xLogger.Debug("_login:", ":result:", result.OuterXml);
					xLogger.Info("_login:", _User.Token, "/", _User.UserID);
					
					_User.CheckCookieToken = false;
					getLoginProfile();

					// now set a cookie if it doesnt expire
					if (!expireToken) {
						ProfilePage.Cooker.Token = _User.Token;
						ProfilePage.Cooker.Write();
						xLogger.Info("_login:", ":cookie:", ProfilePage.Cooker.Token);
					}
				} else if (verifyResult == VERIFY_LOGIN_LOCKED) {	// Too many login attempts
					locked();
				} else if (verifyResult == VERIFY_USER_NOTFOUND) {	// User not found
					throw (new displayException(LINK_NOTFOUND));
				} else if (verifyResult == VERIFY_LOGIN_BAD) {
					if (_CustomerWS.VerifyUsername(_User.Username)) {	// ie username exists therefore must be bad password
						// send recruit email...
						throw (new displayException(LINK_FORGOT));
					} else {
						throw (new displayException(LINK_INFO));
					}
				} else {
					xLogger.Info("_login:", verifyResult.ToString());
				}
			}
			catch (displayException e) {
				throw e;
			}
			catch (WebsiteXPassportException e) {
				//throw (new x_exception(e.Code, e.Message));
				// Rather throw more specific so this can be handled by pages that call the base ProcessRequest and need to handle passport exceptions their own way - eg account on hold
				throw e;
			}
			catch (Exception e) {
				throw (new x_exception("error_passport_login", String.Concat(error_passport_login, "::", e.Message, " - ", e.StackTrace)));
			}
		}

		//OLD CODE- REPLACED BELLOW TO ADD ADMIN PATTERNS
		///// <summary>Call passport to establish a _User's login profile list</summary>
		//private void getLoginProfile() {
		//    xLogger.Debug("_getLoginProfile::userid:", _User.ID.ToString()));
		//    result = passportAdminWS.GetUserProfile(_User.ID);
		//    if (verify() != VERIFY_OK)
		//        logout(LINK_PROFILE);

		//    xLogger.Debug("_getLoginProfile::result:", result.OuterXml));
		//    XmlElement grp = result.SelectSingleNode("/Groups/item") as XmlElement;
		//    if (grp == null) {
		//        logout(LINK_PROFILE);
		//    } else {
		//        string grpid = grp.GetAttribute("GroupID");
		//        xLogger.Debug("_getLoginProfile::id:", grpid.ToString()));

		//        XmlElement subs = SiteProfile.GetSub("SiteProfiles");
		//        xLogger.Debug("_getLoginProfile::subs:", (subs == null) ? "null" : subs.OuterXml));

		//        XmlNode prompt = subs.SelectSingleNode(String.Format("subwf_obj[value='{0}']/prompt", grpid));
		//        string pattern = (prompt == null)? "" : prompt.InnerText;
		//        xLogger.Debug("_getLoginProfile::id:", grpid.ToString(), ":pattern:", pattern));
		//        UserProfile.SetPattern("login", pattern);
				
		//    }
		//}

		/*
		/// <summary>Call access to establish a _User's admin patterns- replaces above method</summary>
		private void getLoginProfile() {
			xLogger.Debug("_getLoginProfile::userid:", _User.ID.ToString()));
			_Access.GetUserRights(_User.ID);
			result = UIPage.Document.ImportNode(_Access.ItemXmlRootNode, true);
			xLogger.Debug("_getLoginProfile::result:", result.OuterXml));
			
			if (verifyAccess() != VERIFY_OK)
				logout(LINK_PROFILE);
			
			XmlElement categories = result.SelectSingleNode("//AwardsX/items") as XmlElement;
			if (categories == null) {
				logout(LINK_PROFILE);
			} else {
				UserProfile.SetPattern("login", "adminuser");
				UserProfile.AddNode("accessxml", categories);
				foreach (XmlNode item in categories) {
					Logger.Debug(String.Concat(logid, logsep, "Category:", item.SelectSingleNode("@category").InnerText, logsep, "Rights:", item.SelectSingleNode("@right").InnerText));
					int right = Convert.ToInt32(item.SelectSingleNode("@right").InnerText);
					if (right >= 1) {
						UserProfile.AddPattern("access", item.SelectSingleNode("@category").InnerText);
					}
				}
			}
		}
		*/

		/// <summary>Call access to establish a _User's admin patterns- replaces above method</summary>
		private void getLoginProfile() {
			xLogger.Debug("_getLoginProfile::userid:", _User.PersonID);
			try {
				_Access.GetUserRights(_User.PersonIID);
			} catch (Exception e) {
				xLogger.Debug("_getLoginProfile::error:", e.Message);
				logout(LINK_PROFILE);
			}
			//xLogger.Debug("_getLoginProfile::result:", _Access.ItemXml.OuterXml));

			// NB: This has been changed to account for website as well as admin use.
			//	- Authenticated users are given the customer pattern and then if they have access items are assumed to be admin users
			//	- also this profile might be further set / modified in a website (or admin site) specific login process
			if (_Access.Items == null) {
				logout(LINK_PROFILE);
			} else {
				UserProfile.SetPattern("login", Cms.PATTERN_CUSTOMER);
				if (_Access.ItemList.Count > 0) {
					UserProfile.SetPattern("login", Cms.PATTERN_ADMINUSER);

					UserProfile.AddNode("accessxml", _Access.Items);
					foreach (XmlNode item in _Access.ItemList) {
						xLogger.Debug("_getLoginProfile", "::Category:", item.SelectSingleNode("@category").InnerText, logsep, "Rights:", item.SelectSingleNode("@right").InnerText);
						int right = Convert.ToInt32(item.SelectSingleNode("@right").InnerText);
						if (right >= 1) {
							UserProfile.AddPattern("access", item.SelectSingleNode("@category").InnerText);
						}
					}
				}
			}
		}

		/// <summary>Validate token (pass if blank)</summary>
		private void validate() {
			xLogger.Debug("validate");
			
			//string tok = UserProfile.Value(CmsXUser.PROPERTY_TOKEN);
			//xLogger.Info("validate", "::Token:", tok);

			string token = _User.Token;
			xLogger.Debug("_doValidation", "::token:", token, "::remember:", _User.Remember.ToString());

			try {
				if (!String.IsNullOrEmpty(token)) {
					//result = _AdminWS.Validate(token);	// deprecated
					result = _PassportWS.ValidateToken(token, _User.Remember);
					xLogger.Debug("validate", "::Root:", Root.OuterXml);

					int verifyResult = verify();
					xLogger.Info("validate", "::verifyResult:", verifyResult.ToString());
					
					if (verifyResult == VERIFY_OK) {	// ie is iPassport User
						return;
					} else if (verifyResult == VERIFY_TOKEN_INVALID) {	// token invalid
						_User.Reset();
						throw new displayException(LINK_INVALID);
					} else {
						throw new Exception(String.Concat("Bad token validate result: ", verifyResult.ToString()));
					}
				}
			} catch (displayException e) {
				throw e;
			} catch (WebsiteXPassportException e) {
				throw (new x_exception("error_passport_login", String.Concat(error_passport_login, e.Message)));
			} catch (Exception e) {
				throw (new x_exception("error_passport_login", String.Concat(error_passport_login, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Login to Passport (using cookie token)</summary>
		private void token() {
			xLogger.Debug("_token:", ":CookieToken:", _User.CookieToken);
			token(_User.CookieToken, true);
		}
		/// <summary>Login to Passport (using token)</summary>
		private void token(string token, bool remember) {
			xLogger.Debug("_token:", ":token:", token, "::remember:", remember.ToString());
			try {
				if (String.IsNullOrEmpty(token)) {	// try Query parameter
					token = UIPage.GetQueryParam(QUERY_TOKEN);
				}
				if (String.IsNullOrEmpty(token)) {
					return;
				}
				xLogger.Debug("_token:", ":token:", token);

				//result = _AdminWS.Validate(token);	// deprecated
				result = _AdminWS.ValidateUser(token, remember);
				xLogger.Debug("_token:", ":result:", result.OuterXml);

				int verifyResult = verify();
				if (verifyResult == VERIFY_OK) {	// ie is iPassport User
					_User.Set(Root.GetAttribute("Token"), Root.GetAttribute("UserID"), Root.SelectSingleNode("UserName").InnerText, Root.SelectSingleNode("Password").InnerText, Root.SelectSingleNode("FirstName").InnerText, Root.SelectSingleNode("Surname").InnerText);
					_User.Remember = true;

					_User.CheckCookieToken = false;
					getLoginProfile();
					xLogger.Info("_token:", _User.Token, "/", _User.UserID);

					string link = UserProfile.Value(Cms.LINK_DESTINATION);
					xLogger.Debug("_token:", ":link:", link);

					if (!String.IsNullOrEmpty(link)) {
						UserProfile.Clear(Cms.LINK_DESTINATION);
						throw (new displayException(link));
					}
				} else if (verifyResult == VERIFY_TOKEN_INVALID) {	// token invalid
					xLogger.Info("_token:", verifyResult.ToString());
					ProfilePage.Cooker.Token = "";
					_User.Reset();
				} else {
					xLogger.Info("_token:", verifyResult.ToString());
					throw new Exception(String.Concat("Bad token validate result: ", verifyResult.ToString()));
				}
			} catch (displayException e) {
				throw e;
			} catch (WebsiteXPassportException e) {
				throw (new x_exception("error_passport_login", String.Concat(error_passport_login, e.Message)));
			} catch (Exception e) {
				throw (new x_exception("error_passport_login", String.Concat(error_passport_login, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Check recaptcha and challenge-response</summary>
		private void recaptcha() {
			xLogger.Info("_recaptcha:");
			try {
				if (_User.WantRecaptcha) {
					string challenge = _User.RecaptchaChallenge;
					string response = _User.RecaptchaResponse;
					xLogger.Info("_recaptcha", "::challenge:", challenge, "::response:", response);

					RecaptchaX recapx = new RecaptchaX();
					recapx.Check(challenge, response, UIPage.Request.UserHostAddress);
				}
			} catch (x_exception e) {
				xLogger.Info("_recaptcha", "::error:", e.Message);
				throw (new displayException(LINK_RECAPTCHA));
			} catch (Exception e) {
				throw (new x_exception("error_passport_recaptcha", String.Concat(error_passport_recaptcha, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Check recaptcha and challenge-response</summary>
		private void recaptcha2() {
			xLogger.Info("_recaptcha2:");
			try {
				if (_User.WantRecaptcha) {
					string response = UserProfile.Value("g-recaptcha-response");
					xLogger.Info("_recaptcha", "::response:", response);

					Recaptcha2X recapx = new Recaptcha2X();
					recapx.Check(response);
				}
			} catch (x_exception e) {
				xLogger.Info("_recaptcha2", "::error:", e.Message);
				throw (new displayException(LINK_RECAPTCHA));
			} catch (Exception e) {
				throw (new x_exception("error_passport_recaptcha", String.Concat(error_passport_recaptcha, e.Message, " - ", e.StackTrace)));
			}
		}
		
		/// <summary>Register _User (ie account holder)</summary>
		private void register() {
			//xLogger.Info("_register:", UIPage.UserProfile.ProfileXml.OuterXml);
			xLogger.Info("_register", "::Username:", _User.Username, "::Email:", _User.Email);
			try {
				// Use email as username, if no username
				_User.Username = String.IsNullOrEmpty(_User.Username) ? _User.Email : _User.Username;
				xLogger.Debug("_register", "::Credentials:", _User.Username, "|", _User.Password, "|", _User.Firstname, "|", _User.Surname, "|", _User.Email, "|", _User.Telephone, "|", _User.Cellphone);

				result = _CustomerWS.RegisterUser(_User.Username, _User.Password, _User.Firstname, _User.Surname, _User.Email, _User.Telephone, _User.Cellphone);
				xLogger.Debug("_register:", Root.OuterXml);

				int verifyResult = verify();
				if (verifyResult == VERIFY_OK) {	// ie is iPassport User
					result = _CustomerWS.GetUser(_User.Username);
					xLogger.Debug("_register:", Root.OuterXml);
					_User.Set(Root);

				} else if (verifyResult == VERIFY_USER_DUPLICATE) {
					throw new displayException("error_passport_duplicate", error_passport_register);
				}
			} catch (displayException e) {
				throw e;
			} catch (x_exception e) {
				throw (new x_exception("error_passport_register", String.Concat(error_passport_register, e.Message)));
			} catch (Exception e) {
				throw (new x_exception("error_passport_register", String.Concat(error_passport_register, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Change password</summary>
		private void change() {
			try {
				xLogger.Info("_change:");
			}
			catch (x_exception e) {
				throw (new x_exception("error_passport_change", String.Concat(error_passport_change, e.Message)));
			}
			catch (Exception e) {
				throw (new x_exception("error_passport_change", String.Concat(error_passport_change, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Get password and hint info</summary>
		private void hint() {
			try {
				xLogger.Info("_hint:", _User.Username);
				getHint();
			}
			catch (Exception e) {
				throw (new x_exception("error_passport_gethint", String.Concat(error_passport_gethint, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Check given hint against hint info</summary>
		private void checkhint() {
			try {
				xLogger.Info("_checkhint:", _User.Username);
				getHint();
				string answer = _User.Get(FIELD_HINT_ANSWER).ToLower();
				if (answer != _User.HintAnswer.ToLower()) {
					throw (new displayException(LINK_HINTBAD));
				}
			}
			catch (displayException e) {
				throw e;
			}
			catch (x_exception e) {
				throw (new x_exception("error_passport_checkhint", String.Concat(error_passport_checkhint, e.Message)));
			}
			catch (Exception e) {
				throw (new x_exception("error_passport_checkhint", String.Concat(error_passport_checkhint, e.Message, " - ", e.StackTrace)));
			}
		}

		private void getHint() {
			result = _CustomerWS.GetHint(_User.Username);
			int verifyResult = verify();
			xLogger.Debug("_getHint:", Root.OuterXml);
			if (verifyResult == VERIFY_OK) {
				_User.Password = Root.SelectSingleNode(SELECT_USER_PASSWORD).InnerText;
				_User.HintID = Root.SelectSingleNode(SELECT_USER_QUESTION).InnerText;
				UserProfile.Add(FIELD_HINT_QUESTION, String.Concat("Hint", _User.HintID));
				_User.HintAnswer = Root.SelectSingleNode(SELECT_USER_ANSWER).InnerText;
				xLogger.Debug("_getHint:", logsep, "id:", _User.HintID, logsep, "answer:", _User.HintAnswer, logsep, "password:", _User.Password);
			} else {
				throw new x_exception("error_passport_gethint", error_passport_gethint);
			}

		}

		/// <summary>Forgot password</summary>
		private void forgot() {
			xLogger.Info("_forgot", "Username", _User.Username);
			try {
				xLogger.Debug("_forgot", "::_CustomerWS:", _CustomerWS.Url);
				
				result = _CustomerWS.GetPassword(_User.Username);
				int verifyResult = verify();
				xLogger.Debug("_forgot:", Root.OuterXml);
				
				if (verifyResult == VERIFY_OK) {
					XmlNode email = Root.SelectSingleNode(SELECT_USER_EMAIL);
					xLogger.Info("_forgot", "::email:", (email == null) ? "null" : email.InnerText);
					send(email.InnerText);
				} else if (verifyResult == VERIFY_USER_NOTFOUND) { // User not found
					xLogger.Debug("_forgot", "::not_found:", verifyResult.ToString());
					throw (new displayException(LINK_NOTFOUND));
				}
			}
			catch (x_exception e) {
				throw (new x_exception("error_passport_forgot", String.Concat(error_passport_forgot, e.Message)));
			}
			catch (Exception e) {
				throw (new x_exception("error_passport_forgot", String.Concat(error_passport_forgot, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Get _User summary info</summary>
		private void summary() {
			xLogger.Debug("_summary:");
			try {
				CmsXExport exportutil = new CmsXExport(UIPage);
				exportutil.LoadFilters();
				xLogger.Debug("_summary:ok");
			}
			catch (x_exception e) {
				throw (new x_exception("error_passport_summary", String.Concat(error_passport_summary, e.Message)));
			}
			catch (Exception e) {
				throw (new x_exception("error_passport_summary", String.Concat(error_passport_summary, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Get _User summary info</summary>
		/// <remarks>This should be replaced by a single WS call that sets a new token for the user and gets user details.</remarks>
		private void locked() {
			try {
				result = _CustomerWS.GetUser(_User.Username);	// first get user details by username
				if (verify() == VERIFY_OK) {
					int userID = Int32.Parse(Root.GetAttribute("UserID"));	
					result = _CustomerWS.GetUserID(userID);	// now get details (incl password)
					if (verify() == VERIFY_OK) {
						string username = Root.SelectSingleNode("UserName").InnerText;
						string password = Root.SelectSingleNode("Password").InnerText;
						xLogger.Debug("_locked", "::username:", username, "::password:", password);

						// Need token, but the token can be set to null with a logout so....
						result = _CustomerWS.LoginBase(username, password);	// This sets the token even if the account is expired
						
						result = _CustomerWS.GetUserID(userID);	// Get user again with new token
						_User.Token = Root.GetAttribute("Token");
						Root.SetAttribute("type", "account_locked");
						Root.SetAttribute("url", String.Concat(UIPage.Config.Host, UIPage.Config.Path));
						XmlNode email = Root.SelectSingleNode(SELECT_USER_EMAIL);
						xLogger.Info("_locked", "::email:", (email == null) ? "" : email.InnerText);

						send(email.InnerText);
						_User.Reset();
						throw (new displayException(LINK_LOCKED));
					}
				} else {
					throw (new Exception("GetUser not verified"));
				}
			}
			catch (displayException e) {
				throw e;
			}
			catch (x_exception e) {
				throw (new x_exception("error_passport_locked", String.Concat(error_passport_locked, e.Message)));
			}
			catch (Exception e) {
				throw (new x_exception("error_passport_locked", String.Concat(error_passport_locked, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Unlock account</summary>
		private void unlock() {
			try {
				xLogger.Info("_unlock:", UIPage.Parameters.Token);
				if (!_CustomerWS.UnlockUser(UIPage.Parameters.Token))
					throw (new displayException(LINK_LOCKED));
			}
			catch (displayException e) {
				throw e;
			}
			catch (x_exception e) {
				throw (new x_exception("error_passport_unlock", String.Concat(error_passport_unlock, e.Message)));
			}
			catch (Exception e) {
				throw (new x_exception("error_passport_unlock", String.Concat(error_passport_unlock, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Logout of account</summary>
		private void logout() {
			logout("passport_login");
		}
		/// <summary>Logout of account</summary>
		private void logout(string link) {
			string token = UIPage.GetQueryParam(QUERY_TOKEN);
			token = (token == "")? _User.Token : token;
			xLogger.Info("_logout:", token, ":link:", link);

			_CustomerWS.Logout(token);
			_User.Reset();
			UserProfile.ClearItems("accessxml");

            throw (new displayException(link));
		}

		/// <summary>Retrieve _User token (after registration)</summary>
		private void share() {
			token();
			if (!String.IsNullOrEmpty(_User.FriendID) && !String.IsNullOrEmpty(_User.FileID))
				throw (new displayException("profile_share_thankyou_"));	// ie need to accept share invite
		}

		#endregion

		#region Private utility methods
		/// <summary>verifies the result from the web service call</summary>
		private int verify() {
			int returnCode = VERIFY_OK;
			XmlElement resltcode = Root.SelectSingleNode(SELECT_RESULT_CODE) as XmlElement;
			if (resltcode != null) {
				xLogger.Debug("_verify::resltcode:", resltcode.OuterXml);
				if (resltcode.InnerText != "0") {
					string excode = "error_passport_result_error";
					string exdesc = String.Concat(error_passport, error_passport_result_error, " code: '", resltcode.InnerText, "' description: '", Root.SelectSingleNode(SELECT_RESULT_DESC).InnerText, "'");
					bool throwException = true;
					switch(resltcode.InnerText) {
						case "1000":
							excode = "error_passport_result_login";
							exdesc = error_passport_result_login;
							returnCode = VERIFY_LOGIN_BAD;
							throwException = false;
							break;
						case "1001":	// code returned when lock occurs
						case "1003":	// code returned when already locked
							excode = "error_passport_result_login";
							exdesc = error_passport_result_login;
							returnCode = VERIFY_LOGIN_LOCKED;
							throwException = false;
							break;
						case "1006":	// code returned when On-hold
							excode = "error_passport_result_onhold";
							exdesc = error_passport_result_login;
							returnCode = VERIFY_LOGIN_ONHOLD;
							throwException = true;
							break;
						case "1004":
						case "2070":
						case "2101":
							excode = "error_passport_result_token";
							exdesc = error_passport_result_token;
							returnCode = VERIFY_TOKEN_INVALID;
							throwException = false;
							break;
						case "2053":
						case "2103":
							excode = "error_passport_result_usernot";
							exdesc = error_passport_result_usernot;
							returnCode = VERIFY_USER_NOTFOUND;
							throwException = false;
							break;
						case "2074":
						case "2109":
							excode = "error_passport_result_duplicate";
							exdesc = error_passport_result_duplicate;
							returnCode = VERIFY_USER_DUPLICATE;
							throwException = false;
							break;
						default:
							break;
					}
					if (throwException) 
						throw (new WebsiteXPassportException(excode, exdesc));
				}
			} else {
				xLogger.Debug("_verify::error_passport_result_null");
				throw (new WebsiteXPassportException("error_passport_result_null", String.Concat(error_passport, error_passport_result_null)));
			}
			return returnCode;
		}
		
		/// <summary>verifies the result from the web service call for getting the _User's rights</summary>
		private int verifyAccess() {
			int returnCode = VERIFY_OK;
			XmlElement resltcode = Root.SelectSingleNode(SELECT_ACCESS_CODE) as XmlElement;
			if (resltcode != null) {
				xLogger.Debug("_verifyAccess::resltcode:", resltcode.OuterXml);
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
							returnCode = VERIFY_LOGIN_LOCKED;
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
						throw (new WebsiteXPassportException(excode, exdesc));
				}
			} else {
				xLogger.Debug("_verify::error_passport_result_null");
				throw (new WebsiteXPassportException("error_passport_result_null", String.Concat(error_passport, error_passport_result_null)));
			}
			return returnCode;
		}

		/// <summary>Emails the _User details, including password</summary>
		private string send(string email) {
			string sendtext = "";
			x_email thisemail = new x_email();
			try {
				/* originally...
				thisemail.Bcc = "alan@clickclickBOOM.co.za";
				thisemail.To = email;
				*/
				thisemail.To = String.IsNullOrEmpty(email) ? Config.EmailTo : email;
				if (!String.IsNullOrEmpty(Config.EmailBcc)) {
					thisemail.Bcc = Config.EmailBcc;
				}
				if (!String.IsNullOrEmpty(Config.EmailFrom)) {
					thisemail.From = Config.EmailFrom;
				}
				thisemail.Type = x_emailtype.html;
				xLogger.Debug("_send", "::result:", result.OuterXml);
				xLogger.Debug("_send", "::ResultDoc:", ResultDoc.OuterXml);

				thisemail.Send(ResultDoc, _GetTemplate(EMAILTEMPLATE));
				sendtext = thisemail.Message;
				xLogger.Debug("_send", "::sendtext:", sendtext);
			}
			catch(System.Exception e) {
				string msg = String.Concat(error_passport, error_passport_email, e.Message);
				xLogger.Info("send(error):", "::Username:", _User.Username, "::Email:", email, "::Server:", thisemail.Server, "::error:", msg);
				throw(new WebsiteXPassportException("error_passport_email", msg));
			}
			return sendtext;
		}

		// Not required - use _User.Remember
		//private bool getRemember() {
		//	xLogger.Debug("_getRemember:", "RememberMe:", _User.RememberMe);
		//	bool noremember = (_User.RememberMe != "on");
		//	xLogger.Info("_getRemember:", "noremember:", noremember.ToString());
		//	_User.RememberMe = "off";	// set off automatically, for safety
		//	return noremember;
		//}

		#endregion
	}
	/// <summary>
	/// An exception class specific to iBurstPassport and derived classes
	/// </summary>
	public class WebsiteXPassportException : x_exception {
		/// <summary>Constructor</summary>
		/// <param name="cde">Error code</param>
		/// <param name="message">Error message</param>
		public WebsiteXPassportException(string cde, string message) : base(cde, message) {
		}
	}
}
