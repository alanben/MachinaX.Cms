using System;
using System.Web;
using System.Xml;

using clickclickboom.machinaX.blogX.templateX;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.2
	Build:		20130926
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Refactored from LoeriesAdmin
	20130926:	Updated to use reference to web service class
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	public class CmsXStartup {

		#region Invisible properties
		public x_logger xLogger;
		private HttpApplicationState Application;
		private x_siteprofile siteprofile;
		private x_config webconfig;
		#endregion

		#region Constant name strings
		private const string logid = "CmsXStartup.";
		private const string CONFIG_DISPLAY_SITE_ID = "displayxsite_service";
		private const string CONFIG_ROOT = "CmsX";
		private const string SELECT_ITEMS = "//items";
		private const string SELECT_ITEMSITEM = "//items/item";
		#endregion

		#region Constant error strings
		#endregion

		#region Visible properties
		private DisplayXSiteWS.SiteDisplayxServiceX siteServiceWS;
		public DisplayXSiteWS.SiteDisplayxServiceX SiteServiceWS {
			get {
				if (siteServiceWS == null) {
					siteServiceWS = new DisplayXSiteWS.SiteDisplayxServiceX();
					siteServiceWS.Url = getUrl(CONFIG_DISPLAY_SITE_ID);
				}
				return siteServiceWS;
			}
		}

		private DisplayXSites siteService;
		public DisplayXSites SiteService {
			get {
				if (siteService == null) {
					siteService = new DisplayXSites();
				}
				return siteService;
			}
		}
		#endregion

		#region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		public CmsXStartup() {
			xLogger = new x_logger(typeof(CmsXStartup), logid, false, true);
		}

		/// <summary>Normal constructor</summary>
		public CmsXStartup(HttpApplicationState application, x_config config) {
			initialise(application, config, (x_siteprofile)application.Get(Profile.SITEPROFILE));
		}

		/// <summary>Normal constructor</summary>
		public CmsXStartup(HttpApplicationState application, string BlogID) {
			x_config config = new x_config(BlogID);
			initialise(application, config, (x_siteprofile)application.Get(Profile.SITEPROFILE));
		}

		/// <summary>Normal constructor</summary>
		public CmsXStartup(HttpApplicationState application, x_config config, x_siteprofile siteProfile) {
			initialise(application, config, siteProfile);
		}
		#endregion

		#region Private methods
		private void initialise(HttpApplicationState application, x_config config, x_siteprofile siteProfile) {
			xLogger = new x_logger(typeof(CmsXStartup), logid, false, true);

			Application = application;
			siteprofile = siteProfile;
			webconfig = config;
		}

		/// <summary>get the WS config URL</summary>
		private string getUrl(string config_id) {
			xLogger.Debug("getUrl", "::config_id:", config_id);

			string url = webconfig.Value(String.Concat(CONFIG_ROOT, "/", "Url[@id='", config_id, "']"));
			xLogger.Debug("getUrl", "::url:", url);
			
			return url;
        }
		#endregion

		#region Public methods
		/// <summary>Start the entire application - typically called from global.asax's Application OnStart event</summary>
		/// <remarks>Use for backward compatibility for sites that have admin/control separate from www</remarks>
		public void Start() {
			Start(true);
		}
		/// <summary>Start the entire application - typically called from global.asax's Application OnStart event</summary>
		/// <param name="WantSiteWS">Flag to indicate use of web service or internal reference</param>
		public void Start(bool WantSiteWS) {
			loadSites(WantSiteWS);
		}
		#endregion

		#region Private methods
		private void loadSites(bool WantSiteWS) {
			xLogger.Debug("loadSites");

			XmlNode sites;
			if (WantSiteWS) {
				sites = SiteServiceWS.GetSites();
			} else {
				sites = SiteService.GetSites();
			}
			xLogger.Debug("loadSites::sites:", sites.OuterXml);
			
			XmlElement profileSites = siteprofile.AddNode(Cms.PROFILE_SITES_ALL, true);
			profileSites.AppendChild(siteprofile.ProfileXml.ImportNode(sites.SelectSingleNode(SELECT_ITEMS), true));
		}
		#endregion

	}
}
