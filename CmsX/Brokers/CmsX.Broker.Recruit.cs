using System;
using System.Xml;
using Facebook;
using Microsoft.CSharp.RuntimeBinder;

using XXBoom.MachinaX.EmailX;
using XXBoom.MachinaX.ReCaptchaX;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2012-11-06
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20121106:	Started
	20140112:	Refactored constructor
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
    /// <summary>
    /// Description of the class.
    /// <para>Additional information about the class</para>
    /// </summary>
	public class CmsXBrokerRecruit : CmsXBrokerBase {

		private CmsXRecruit recruit;

		private const string logid = "CmsXBrokerRecruit.";
		private const string error_recruit = "CmsXBrokerRecruit error:: ";
		private const string error_recruit_recaptcha = " ReCaptcha error";
		private const string error_recruit_add = "Error adding the recruit: ";
		private const string error_recruit_get = "Error getting the recruit: ";
		private const string error_recruit_check = "Error checking a recruit: ";
		private const string error_recruit_remove = "Error deleting a recruit: ";
		private const string error_recruit_facebook = "Error deleting a recruit: ";
		private const string error_recruit_result_error = "bad result";
		private const string error_recruit_result_null = "null result";
		private const string error_recruit_email = " Email sending error";
		
		private const string LINK_RECAPTCHA = "recruit_recaptcha";
		private const string EMAILTEMPLATE = "CmsXBrokerRecruit.xsl";

		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerRecruit(CmsX thispage) : base(thispage, typeof(CmsXBrokerRecruit), logid) {
			xLogger.Debug("CmsXBrokerRecruit");
			recruit = new CmsXRecruit(UserProfile);
		}

		public override void Process(string type) {
			xLogger.Info("Process:", type);
			switch (type) {
				case "recaptcha":
					recaptcha();
					break;
				case "add":
					add();
					break;
				case "get":
					get();
					break;
				case "check":
					check();
					break;
				case "remove":
					remove();
					break;
				case "facebook":
					facebook();
					break;
			}
        }

		public override void Process(string type, XmlElement content) {
			xLogger.Info("Process:", type);
			switch (type) {

				// template methods
				case "check":
					check(content);
					break;
			}
		}

		/// <summary>Check recaptcha and challenge-response</summary>
		private void recaptcha() {
			xLogger.Info("_recaptcha:");
			try {
				if (recruit.WantRecaptcha) {
					string challenge = recruit.RecaptchaChallenge;
					string response = recruit.RecaptchaResponse;
					xLogger.Info("_recaptcha", "::challenge:", challenge, "::response:", response);

					RecaptchaX recapx = new RecaptchaX();
					recapx.Check(challenge, response, UIPage.Request.UserHostAddress);
				}
			} catch (XException e) {
				xLogger.Info("_recaptcha", "::error:", e.Message);
				throw (new displayException(LINK_RECAPTCHA));
			} catch (Exception e) {
				throw (new XException("error_recruit_recaptcha", String.Concat(error_recruit_recaptcha, e.Message, " - ", e.StackTrace)));
			}
		}

		/// <summary>Recruit.add</summary>
		private void add() {
			xLogger.Info("add:");
			try {
				XmlNode result = _CustomerWS.AddRecruit(recruit.Username, recruit.Password, recruit.Firstname, recruit.Surname, recruit.Email, recruit.Telephone, recruit.Cellphone, recruit.TypeIID, recruit.PersonIID);
				verify(result);
				recruit.Set(result as XmlElement);
				send(result as XmlElement);

				xLogger.Debug("add:ok");
			} catch (WebsiteXPassportException e) {
				throw e;
			} catch (Exception e) {
				throw (new XException("error_recruit_add", String.Concat(error_recruit_add, e.Message)));
			}
		}

		/// <summary>Recruit.get</summary>
		private void get() {
			xLogger.Info("get:");
			try {
				XmlNode result = _CustomerWS.GetRecruit(recruit.ID);
				verify(result);
				recruit.Set(result as XmlElement);

				xLogger.Debug("get:ok");
			} catch (WebsiteXPassportException e) {
				throw e;
			} catch (Exception e) {
				throw (new XException("error_recruit_get", String.Concat(error_recruit_get, e.Message)));
			}
		}

		/// <summary>Recruit.check</summary>
		private void check() {
			xLogger.Info("check:");
			try {
				string token = UIPage.QueryParam("token");

				XmlNode result = _CustomerWS.GetRecruitToken(token);
				verify(result);
				recruit.Set(result as XmlElement);

				xLogger.Debug("check:ok");
			} catch (WebsiteXPassportException e) {
				//throw e;
				throw (new XException("error_recruit_failed", String.Concat(error_recruit_check, e.Message)));
			} catch (Exception e) {
				throw (new XException("error_recruit_check", String.Concat(error_recruit_check, e.Message)));
			}
		}

		/// <summary>Recruit.check</summary>
		private void check(XmlElement content) {
			xLogger.Info("check:");
			try {

				xLogger.Debug("check:ok");
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				throw (new XException("error_recruit_check", String.Concat(error_recruit_check, e.Message)));
			}
		}

		/// <summary>Recruit.remove</summary>
		private void remove() {
			xLogger.Info("remove:");
			try {
				if (!_CustomerWS.DeleteRecruit(recruit.ID)) {
					throw (new XException("error_recruit_failed", String.Concat(error_recruit_check, " not deleted")));
				} else {
					recruit.Reset();
				}
				xLogger.Debug("remove:ok");
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				throw (new XException("error_recruit_remove", String.Concat(error_recruit_remove, e.Message)));
			}
		}

		/// <summary>Recruit.facebook</summary>
		private void facebook() {
			xLogger.Info("facebook:");
			try {
				string fbtoken = UIPage.QueryParam("FacebookToken");

				var client = new FacebookClient(fbtoken);
				dynamic result = client.Get("me", new { fields = "first_name, middle_name, last_name, name, username, id, email" });
				xLogger.Debug("facebook", "::first_name:", result.first_name, "::middle_name:", result.middle_name, "::last_name:", result.last_name, "::name:", result.name, "::username:", result.username, "::id:", result.id, "::email:", result.email);
				recruit.Set(result.first_name, result.middle_name, result.last_name, result.name, result.username, result.id, result.email);

				xLogger.Debug("facebook:ok");
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				throw (new XException("error_recruit_facebook", String.Concat(error_recruit_facebook, e.Message)));
			}
		}

		/// <summary>verifies the result from the web service call</summary>
		private void verify(XmlNode result) {
			xLogger.Debug("_verify::result:", result.OuterXml);
			
			XmlElement resltcode = result.SelectSingleNode(SELECT_PASSPORT_RESULT_CODE) as XmlElement;
			if (resltcode != null) {
				xLogger.Debug("_verify::resltcode:", resltcode.OuterXml);
				if (resltcode.InnerText != "0") {
					string excode = "error_recruit_result_error";
					string exdesc = String.Concat(error_recruit, error_recruit_result_error, " code: '", resltcode.InnerText, "' description: '", result.SelectSingleNode(SELECT_PASSPORT_RESULT_DESC).InnerText, "'");
					throw (new WebsiteXPassportException(excode, exdesc));
				}
			} else {
				xLogger.Debug("_verify::error_recruit_result_null");
				throw (new WebsiteXPassportException("error_recruit_result_null", String.Concat(error_recruit, error_recruit_result_null)));
			}
		}

		/// <summary>Emails the _User details, including password</summary>
		/// <remarks>
		///		result should contain the following:
		///		<Recruit ID="1" Token="9c806852-5a59-4ef6-903c-b0975da6596c" UserName="al@xxboom.com" Password="" FirstName="" Surname="" EMail="al@xxboom.com" TelNo="" CellPhone="" SignupDate="2012/11/06 11:20:55 PM" Type="Subscriber" TypeID="1" UserID="0">
		///		  <Result>
		///		    <ResultCode>0</ResultCode>
		///		    <Description />
		///		  </Result>
		///		</Recruit>
		///	</remarks>
		private void send(XmlElement result) {
			xLogger.Info("send::Email:", recruit.Email);
			
			try {
				x_email thisemail = new x_email();
				thisemail.Bcc = Config.EmailBcc;
				thisemail.To = recruit.Email;
				thisemail.Type = x_emailtype.html;
				thisemail.Send(result, _GetTemplate(EMAILTEMPLATE));
				xLogger.Info("send::Message:", thisemail.Message);
			} catch (System.Exception e) {
				string msg = String.Concat(error_recruit, error_recruit_email, e.Message);
				xLogger.Info("send:::Email_NotOK:", msg);
				throw (new WebsiteXPassportException("error_recruit_email", msg));
			}
		}

	}
}





