using System;
using System.Web;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.3
	Build:		20200218
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Refactored from WebMail
	20121116:	Refactored and updated
	20200218:	Renamed checkToken to _CheckToken and readTokenCookie to _ReadTokenCookie
				+ make protected, virtual ie overridable
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>
	/// The CmsXCookies class is a utility class for handling cookies on Website
	/// </summary>
	public class CmsXCookies {

		private const string logid = "CmsXCookies";

		public CmsXProfileX UIPage;
		public CmsXUser WebsiteUser;
		public x_logger xLogger;

		public bool Active	{ get; set; }
		public string Skin		{ get; set; }
		public string Token		{ get; set; }
		public string Wizard	{ get; set; }

		public bool IsCookies	{ get { return (UIPage.Request.Cookies[Cms.COOKIES] != null); } }
		public bool IsSkin		{ get { return (UIPage.Request.Cookies[Cms.COOKIE_SKIN] != null); } }
		public bool IsToken		{ get { return (UIPage.Request.Cookies[Cms.COOKIE_TOKEN] != null); } }
		public bool IsWizard	{ get { return (UIPage.Request.Cookies[Cms.COOKIE_WIZARD] != null); } }

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public CmsXCookies(CmsXProfileX thispage) {
			UIPage = thispage;
			xLogger = thispage.xLogger;
			WebsiteUser = thispage.WebsiteUser;
			Skin = Cms.DEFAULT_SKIN;
			Wizard = Cms.DEFAULT_WIZARD;
			Active = true;
		}
		#endregion

		/// <summary>
		/// Reads the cookies from the request
		/// </summary>
		/// <param name="check">Flag to indicate if token cookie check is to be done</param>
		public void Read(bool check) {
			xLogger.Debug("Read", "::Active:", Active, "::IsCookies:", IsCookies);

			if (!Active) {
				return;
			}

			readSkinCookie();
			readWizardCookie();
			if (check) {
				if (_ReadTokenCookie()) {
					_CheckToken();
				}
			}
		}
		/// <summary>
		/// Reads the wizard cookie from the request
		/// </summary>
		public string ReadWizard() {
			xLogger.Debug(logid, "ReadWizard", IsCookies.ToString());
			readWizardCookie();
			return Wizard;
		}
		/// <summary>
		/// Writes the cookies to the response
		/// </summary>
		public void Write(bool resetToken) {
			xLogger.Debug("Write", "::Active:", Active, "::resetToken:", resetToken);

			if (!Active) {
				return;
			}
			if (resetToken) {
				Token = "";
			}
			Write();
		}
		/// <summary>
		/// Writes the cookies to the response
		/// </summary>
		public void Write() {
			xLogger.Debug("Write", "::Active:", Active);

			if (!Active) {
				return;
			}

			HttpCookie cmsXcookie = new HttpCookie(Cms.COOKIES);
			cmsXcookie.Value = UIPage.Label;
			cmsXcookie.Expires = DateTime.Now.AddYears(Cms.COOKIE_EXPIRY_YEARS);
			UIPage.Response.Cookies.Add(cmsXcookie);

			HttpCookie skinCookie = new HttpCookie(Cms.COOKIE_SKIN);
			skinCookie.Value = Skin;
			skinCookie.Expires = DateTime.Now.AddYears(Cms.COOKIE_EXPIRY_YEARS);
			UIPage.Response.Cookies.Add(skinCookie);

			xLogger.Debug(logid, "Write", "::Token:", Token);

			HttpCookie tokenCookie = new HttpCookie(Cms.COOKIE_TOKEN);
			tokenCookie.Value = Token;
			tokenCookie.Expires = DateTime.Now.AddYears(Cms.COOKIE_EXPIRY_YEARS);
			UIPage.Response.Cookies.Add(tokenCookie);

			WriteWizard();
		}
		/// <summary>
		/// Writes the wizard cookie to the response
		/// </summary>
		public void WriteWizard() {
			HttpCookie wizardCookie = new HttpCookie(Cms.COOKIE_WIZARD);
			wizardCookie.Value = Wizard;
			wizardCookie.Expires = DateTime.Now.AddYears(Cms.COOKIE_EXPIRY_YEARS);
			UIPage.Response.Cookies.Add(wizardCookie);
		}

		private void readSkinCookie() {
			//xLogger.Debug(logid, "readSkinCookie::", "IsCookies:", IsCookies.ToString(), "::IsSkin:", IsSkin.ToString());
			if (IsCookies && IsSkin) {
				Skin = UIPage.Request.Cookies[Cms.COOKIE_SKIN].Value;
				WebsiteUser.CookieSkin = Skin;
			}
			xLogger.Debug(logid, "readSkinCookie::", "Skin:", Skin);
		}

		protected virtual bool _ReadTokenCookie() {
			//xLogger.Debug(logid, "readTokenCookie::", "IsCookies:", IsCookies.ToString(), "::IsToken:", IsToken.ToString());
			if (IsCookies && IsToken) {
				Token = UIPage.Request.Cookies[Cms.COOKIE_TOKEN].Value;
				WebsiteUser.CookieToken = Token;
			}
			xLogger.Debug(logid, "readTokenCookie::", "Token:", Token);
			return (Token != "");
		}

		private void readWizardCookie() {
			//xLogger.Debug(logid, "readWizardCookie::", "IsCookies:", IsCookies.ToString(), "::IsWizard:", IsWizard.ToString());
			if (IsCookies && IsWizard) {
				Wizard = UIPage.Request.Cookies[Cms.COOKIE_WIZARD].Value;
				WebsiteUser.CookieWizard = Wizard;
			}
			Wizard = (!String.IsNullOrEmpty(Wizard))? Wizard : Cms.DEFAULT_WIZARD;
			xLogger.Debug(logid, "readWizardCookie::", "wizard:", Wizard);
		}

		protected virtual void _CheckToken() {
			CmsXBrokerPassport broker = new CmsXBrokerPassport(UIPage); 
			broker.Process("token");
		}
	}
}
