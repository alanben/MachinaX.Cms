using System;
using System.Web;
using System.Xml;

using XXBoom.MachinaX;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington	
	Started:	2009-11-09	
	Status:		release	
	Version:	2.2.0
	Build:		20100118
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100118:	Starting point from SpoefRecaptchaX
	-----------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.ReCaptchaX {
	/// <summary>This class provides the code-behind for an ajax aspx page returning an xml result</summary>
	public class RecaptchaAjaxX : System.Web.UI.Page {
        #region Invisible properties
        private XmlDocument recaptchaXml;
		private x_logger logger;
		#endregion

		#region Constants
		private const string DOCXML = "<recaptcha status='ok'></recaptcha>";
		#endregion

		#region Visible properties
		public XmlElement Recaptcha {
			get { return recaptchaXml.SelectSingleNode("/recaptcha") as XmlElement; }
		}
		public string Status {
			get { return Recaptcha.GetAttribute("status"); }
			set { Recaptcha.SetAttribute("status", value); }
		}
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public RecaptchaAjaxX() : base() {
			initialise();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Gets the xml containing advert references and writes to Response.
		/// </summary>
		public void DoRecaptcha() {
			logger.Info("Recaptcha:");
			try {
				check_recaptcha();
				Response.ContentType = "text/xml";
				Response.Write(recaptchaXml.DocumentElement.OuterXml);
			} catch (XException e) {
				Status = e.Code;
				Recaptcha.InnerText = e.Message;
				Response.ContentType = "text/xml";
				Response.Write(recaptchaXml.DocumentElement.OuterXml);
			} catch (Exception) {
				Response.StatusCode = 404;
				Response.StatusDescription = "Page not found";
			}
		}
		#endregion

		#region Protected methods
		#endregion

		#region Private methods
		private void initialise() {
			logger = new x_logger(typeof(RecaptchaX));
			recaptchaXml = new XmlDocument();
			recaptchaXml.LoadXml(DOCXML);
		}
		/// <summary>Recaptcha csv file and serve result</summary>
		private void check_recaptcha() {
			string challenge = Request.Form["recaptcha_challenge_field"];
			string response = Request.Form["recaptcha_response_field"];
			logger.Info(String.Concat("_check_recaptcha::challenge:", challenge, "::response:", response));

			RecaptchaX recapx = new RecaptchaX();
			recapx.Check(challenge, response, Request.UserHostAddress);
		}

		#endregion
	}
}