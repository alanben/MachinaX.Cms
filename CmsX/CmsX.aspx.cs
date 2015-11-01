using System;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.1
	Build:		20120712
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Started
	20101214:	Added call to CmsXBrokerSlides
	20101218:	Added call to CmsXBrokerSpaces and CmsXBrokerBlogs
	20101228:	Added check for linklog flag in config
	20110905:	Updated _CheckPattern call
	20111010:	Added load_sites process
	20111117:	Added setCSVFilters method (called in _ProcessContent)
	20120712:	Changed passportlogin so that summary (ie loading csv filers) is not called
				Added passportcmslogin (ie summary is called)
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>
	/// Description of the classX class.
	/// <para>Additional information about the class</para>
	/// </summary>
	public class CmsX : CmsXProfileX {
		#region Invisible properties
		#endregion

		#region Constants
		private const string CONFIG_ROOT = "CmsX";
		private const string logid = "CmsX.";
		private const string DEFAULT_GRID_WIDTH = "970";
		#endregion

		#region Visible properties
		private x_broker broker;
		/// <summary>The broker to be used to process request and content</summary>
		public x_broker Broker {
			get { return broker; }
			set { broker = value; }
		}
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public CmsX() : base(CONFIG_ROOT, typeof(CmsX), logid) {
		}
		/// <summary>Constructor for derived classes</summary>
		public CmsX(string label) : base(label, typeof(CmsX), logid) {
		}
		/// <summary>Constructor for derived classes</summary>
		public CmsX(string label, Type type, string loggerID) : base(label, type, loggerID) {
		}
		#endregion

		#region Public methods
		#endregion

		#region Protected methods
		/// <summary>
		/// Does the pre-processing of the page request. 
		///	This method is overriden in order to:
		///	- Add form fields to the profile
		/// </summary>
		protected override void _ProcessRequest() {
			_ProcessRequest(false);
		}
		protected override void _ProcessRequest(bool CheckPassportToken) {
			xLogger.Debug("ProcessRequest", "::CheckPassportToken:", CheckPassportToken);
			xLogger.Debug("ProcessRequest", "::CheckCookieToken:", WebsiteUser.CheckCookieToken);

			Cooker.Read(WebsiteUser.CheckCookieToken);

			_CheckPattern(true, true);
			if (CheckPassportToken) {
				CmsXBrokerPassport pbroker = new CmsXBrokerPassport(this);
				pbroker.Process("validate");
			}

			UserProfile.Add(Request);
			string siteid = UserProfile.Value(Cms.PROFILE_SITE_ID);
			xLogger.Debug("ProcessRequest::siteid:", siteid);

			// Add linklog entry
			if (Config.Value(String.Concat(CONFIG_ROOT, "/@linklog"), "no") == "yes") {
				CmsXBrokerLinkLog log = new CmsXBrokerLinkLog(this);
				log.Add(WebsiteUser);
			}

			xLogger.Debug("ProcessRequest::Parameters.Broker:", Parameters.Broker);
			switch (Parameters.Broker) {
				// CmsXBrokerAuth
				case "cmsauthlogin":			broker = new CmsXBrokerAuth(this);				broker.Process("login");			break;
				case "cmsauthlogout":			broker = new CmsXBrokerAuth(this);				broker.Process("logout");			break;

				// CmsXBrokerPassport
                case "passportcmslogin":		broker = new CmsXBrokerPassport(this);			broker.Process("cmslogin");			break;
                case "passportloginemail":		broker = new CmsXBrokerPassport(this);			broker.Process("loginemail");		break;
                case "passportlogin":			broker = new CmsXBrokerPassport(this);			broker.Process("login");			break;
                case "passportregister":		broker = new CmsXBrokerPassport(this);			broker.Process("recaptcha");			
																								broker.Process("register");			break;
                case "passportchange":			broker = new CmsXBrokerPassport(this);			broker.Process("change");			break;
                case "passportforgot":			broker = new CmsXBrokerPassport(this);			broker.Process("forgot");			break;
                case "passportsummary":			broker = new CmsXBrokerPassport(this);			broker.Process("summary");			break;
                case "passportunlock":			broker = new CmsXBrokerPassport(this);			broker.Process("unlock");			break;
                case "passportlogout":			broker = new CmsXBrokerPassport(this);			broker.Process("logout");			break;
                case "passporttoken":			broker = new CmsXBrokerPassport(this);			broker.Process("token");			break;
                case "passporthint":			broker = new CmsXBrokerPassport(this);			broker.Process("hint");				break;
                case "passportcheckhint":		broker = new CmsXBrokerPassport(this);			broker.Process("checkhint");		break;
                case "passportshare":			broker = new CmsXBrokerPassport(this);			broker.Process("share");			break;

				// Sites
				case "load_sites":				broker = new CmsXBrokerSites(this);				broker.Process("load_sites");		break;
				case "list_sites":				broker = new CmsXBrokerSites(this);				broker.Process("list_sites");		break;
				case "select_site":				broker = new CmsXBrokerSites(this);				broker.Process("select_site");		break;

				// Check logged in state...
				case "list_pages":				broker = new CmsXBrokerBase(this);				broker.Process("default");			break;

				// Access
				case "user_unlock":				broker = new CmsXBrokerAccess(this);			broker.Process("user_unlock");		break;

				// LinkLogs
				case "list_linklogs":			broker = new CmsXBrokerLinkLog(this);			broker.Process("list_filter");		break;
				case "list_linklogs_csv":		broker = new CmsXBrokerLinkLog(this);			broker.Process("list_csv");			break;

				// Links
				case "list_links":				broker = new CmsXBrokerLinks(this, siteid);		broker.Process("list_filter");		break;
				case "edit_link_blog":			broker = new CmsXBrokerLinks(this, siteid);		broker.Process("getblog");			break;
				case "edit_link_blog_update":	broker = new CmsXBrokerLinks(this, siteid);		broker.Process("saveblog");			break;
				case "edit_link_page":			broker = new CmsXBrokerLinks(this, siteid);		broker.Process("getpage");			break;
				case "edit_link_page_update":	broker = new CmsXBrokerLinks(this, siteid);		broker.Process("savepage");			break;

				// Spaces
				case "edit_space_blog":			broker = new CmsXBrokerSpaces(this, siteid);	broker.Process("getblog");			break;
				case "edit_space_blog_update":	broker = new CmsXBrokerSpaces(this, siteid);	broker.Process("saveblog");			break;
				case "space_topic_blog":		broker = new CmsXBrokerSpaces(this, siteid);	broker.Process("gettopicblog");		break;
				case "space_topic_blog_update":	broker = new CmsXBrokerSpaces(this, siteid);	broker.Process("savetopicblog");	break;

				// Blogs
				case "list_blogs":				broker = new CmsXBrokerBlogs(this, siteid);		broker.Process("list_filter");		break;
				case "edit_blogs_blog":			broker = new CmsXBrokerBlogs(this, siteid);		broker.Process("getblog");			break;
				case "edit_blogs_blog_update":	broker = new CmsXBrokerBlogs(this, siteid);		broker.Process("saveblog");			break;
				case "get_blog_history_view":	broker = new CmsXBrokerBlogs(this, siteid);		broker.Process("view_history");		break;

				// Media
				case "media_upload":			broker = new CmsXBrokerMedia(this, siteid);		broker.Process("upload");			break;
				case "media_doupload":			broker = new CmsXBrokerMedia(this, siteid);		broker.Process("doupload");			break;

				// Files
				case "file_upload":				broker = new CmsXBrokerFile(this, siteid);		broker.Process("upload");			break;
				case "file_doupload":			broker = new CmsXBrokerFile(this, siteid);		broker.Process("doupload");			break;

				// News
				case "edit_news_blog":			broker = new CmsXBrokerNews(this, siteid);		broker.Process("getblog");			break;
				case "edit_news_blog_update":	broker = new CmsXBrokerNews(this, siteid);		broker.Process("saveblog");			break;
				
				// Tests
				case "errortest":				throw new x_exception("error_test", "This is an error test");
				case null:
				case "": break;
				// use base broker for default (ie sends email on submission)
				//default:				broker = new x_broker(this);			broker.Process();	break;
			}
		}
		/// <summary>
		/// Post-process page content to: 
		///	- Save CSV settings (on lists)
		/// </summary>
		protected override void _ProcessContent() {
			xLogger.Debug("_ProcessContent", "::Parameters.Broker:", Parameters.Broker);
			//xLogger.Debug("_ProcessContent", "::UserProfile:", (UserProfile== null)? "null" : UserProfile.ProfileXml.OuterXml);

			string siteid = UserProfile.Value(Cms.PROFILE_SITE_ID);
			xLogger.Debug("_ProcessContent", "::siteid:", siteid);

			switch (Parameters.Broker) {
                case "list_pages":				Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("pages_list");		break;
				case "csv_settings_save":		Broker = new CmsXBrokerCSV(this);				Broker.Process("settings_save");	break;
				case "get_blog_comment_view":	Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("view_comment");		break;
			}
			//xLogger.Debug("_ProcessContent:.");
			
			setGridWidth();
			//xLogger.Debug("_ProcessContent:..");

			setCSVFilters();
			//xLogger.Debug("_ProcessContent:...");
			

			Cooker.Token = WebsiteUser.Token;
			Cooker.Write(!WebsiteUser.Remember);
			//xLogger.Debug("_ProcessContent:ok.");
		}
        #endregion
				
		#region Private methods
		private void setGridWidth() {
			XmlElement gridconfig = Content.SelectSingleNode("//grid/config") as XmlElement;
			if (gridconfig != null) {
				string width = gridconfig.GetAttribute("grid_width");
				width = (String.IsNullOrEmpty(width)) ? DEFAULT_GRID_WIDTH : width;
				gridconfig.SetAttribute("grid_width", width);
			}
		}

		private void setCSVFilters() {
			if (Content.SelectSingleNode("//csvfilter[@type='itemselector']") != null) {
				CmsXCSV csvutil = new CmsXCSV(this);
				csvutil.SetFilters();
			}
		}
		#endregion
	}
}
