
/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.3
	Build:		20161017
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Refactored from LoeriesAdmin
	20130926:	Updated to use reference to web service class
	20161017:	Added virtual to Start methods
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {

	using System;
	using System.Web;
	using System.Xml;
	using XXBoom.MachinaX.BlogX.TemplateX;
	
	public class CmsXStartup {

		#region Invisible properties
		public XLogger xLogger;
		private XConfig webconfig;
		#endregion

		#region Constant name strings
		private const string logid = "CmsXStartup.";
		private const string CONFIG_DISPLAY_SITE_ID = "displayxsite_service";
		private const string CONFIG_ROOT = "CmsX";
		private const string SELECT_ITEMS = "//items";
		private const string SELECT_ITEMSITEM = "//items/item";



		public HttpApplicationState Application { get; set; }
		public x_siteprofile SiteProfile { get; set; }

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
			xLogger = new XLogger(typeof(CmsXStartup), logid, false, true);
		}

		/// <summary>Normal constructor</summary>
		public CmsXStartup(HttpApplicationState application, string configID, x_siteprofile siteProfile) {
			initialise(application, new XConfig(configID), siteProfile);
		}

		/// <summary>Normal constructor</summary>
		public CmsXStartup(HttpApplicationState application, XConfig config) {
			initialise(application, config, (x_siteprofile)application.Get(BlogTemplateX.SITEPROFILE));
		}

		/// <summary>Normal constructor</summary>
		public CmsXStartup(HttpApplicationState application, string BlogID) {
			XConfig config = new XConfig(BlogID);
			initialise(application, config, (x_siteprofile)application.Get(BlogTemplateX.SITEPROFILE));
		}

		/// <summary>Normal constructor</summary>
		public CmsXStartup(HttpApplicationState application, XConfig config, x_siteprofile siteProfile) {
			initialise(application, config, siteProfile);
		}
		#endregion

		#region Private methods
		private void initialise(HttpApplicationState application, XConfig config, x_siteprofile siteProfile) {
			xLogger = new XLogger(typeof(CmsXStartup), logid, false, true);

			Application = application;
			SiteProfile = siteProfile;
			webconfig = config;
		}

		/// <summary>get the WS config URL</summary>
		private string getUrl(string config_id) {
			xLogger.Debug("getUrl", "::config_id:", config_id);

			string url = webconfig.Value(String.Concat(Cms.CONFIG_ROOT, "/", "Url[@id='", config_id, "']"));
			xLogger.Debug("getUrl", "::url:", url);
			
			return url;
        }
		#endregion

		#region Public methods
		/// <summary>Start the entire application - typically called from global.asax's Application OnStart event</summary>
		/// <remarks>Use for backward compatibility for sites that have admin/control separate from www</remarks>
		public virtual void Start() {
			Start(true);
		}
		/// <summary>Start the entire application - typically called from global.asax's Application OnStart event</summary>
		/// <param name="WantSiteWS">Flag to indicate use of web service or internal reference</param>
		public virtual void Start(bool WantSiteWS) {
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

			XmlElement profileSites = SiteProfile.AddNode(Cms.PROFILE_SITES_ALL, true);
			profileSites.AppendChild(SiteProfile.ProfileXml.ImportNode(sites.SelectSingleNode(SELECT_ITEMS), true));
		}
		#endregion

		/// <summary>get the WS config URL</summary>
		protected string _GetUrl(string ConfigID) {
			return getUrl(ConfigID);
		}
		protected string _GetUrl(string ConfigRoot, string ConfigID) {
			return webconfig.Value(String.Concat(ConfigRoot, "/", "Url[@id='", ConfigID, "']"));
		}


	}
}
