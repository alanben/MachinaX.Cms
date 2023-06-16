using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;


/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	2.6.0
	Build:		20120316
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20120316:	Refactored from LoeriesAdmin
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
	/// <summary>
	/// This class is a functional wrapper of the PassportX web service
	/// </summary>
	public class PassportX : CmsWSBase {
		#region Private properties
		private string token;
		private x_config config;
		#endregion

		#region Constant name strings
		private const string logid = "PassportX.";
		private const string CONFIG_CONTENT_ID = "passportx_service";
		private const string ROOT_NAME = "PassportX";
		#endregion

		#region Visible properties
		private PassportWS.PassportXPassportServices passportWS;
		public PassportWS.PassportXPassportServices PassportWS {
			get { return passportWS; }
			set { passportWS = value; }
		}

		public string Token { 
			get { return String.IsNullOrEmpty(token)? login() : token;
			}
		}
		#endregion

		#region Constant error strings
		private const string ERROR_WS_RESULT = "Web service error : ";
		#endregion

		#region Constructors/Destructors
		///// <summary>Default constructor</summary>
		//public PassportX(displayX thispage, string award_id) : base(thispage, ROOT_NAME, award_id) {
		//    initialize();
		//}
		/// <summary>Default constructor</summary>
		public PassportX(displayX thispage) : base(thispage, ROOT_NAME, typeof(PassportX), logid) {
			initialize();
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize() {
			xLogger.Debug("initialize");
			config = new x_config();
			
			PassportWS = new PassportWS.PassportXPassportServices();
			PassportWS.Url = _GetUrl(CONFIG_CONTENT_ID);
			xLogger.Debug("initialize", "::PassportWS:", PassportWS.Url);

			xLogger.Debug("initialize::ok:");
		}

		#endregion

		#region Public Authentication PassportX Methods
		public void Login(string username, string password) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckPassportResult(PassportWS.Login(username, password, true), false)
					, true)
				);
		}
		/// <summary>DEPRECATED (was Validate)</summary>
		public void ValidateOld(string Token) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckPassportResult(PassportWS.Validate(Token), false)
					, true)
				);
		}
		public void ValidateToken(string Token, bool Remember) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckPassportResult(PassportWS.ValidateToken(Token, Remember), false)
					, true)
				);
		}
		public void ValidateUser(string Token) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckPassportResult(PassportWS.ValidateUser(Token, false), false)
					, true)
				);
		}
		public void GetUser(int UserID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckPassportResult(PassportWS.GetUser(UserID), false)
					, true)
				);
		}
		#endregion

		#region Private PassportX Methods
		private string login() {
			string username = config.Value(String.Concat(Cms.CONFIG_ROOT, "/Url[@id='", CONFIG_CONTENT_ID, "']/@username"));
			string password = config.Value(String.Concat(Cms.CONFIG_ROOT, "/Url[@id='", CONFIG_CONTENT_ID, "']/@password"));
			xLogger.Debug("login", "::Username:", username, "::Password:", password);
			XmlNode user = PassportWS.Login(username, password, true);
			xLogger.Debug("login", "::admin:", user.OuterXml);
			token = user.SelectSingleNode("//User/@token").InnerText;
			xLogger.Debug("login", "::token:", token);
			return token;
		}
		#endregion

	}
}

