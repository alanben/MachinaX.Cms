using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.2
	Build:		20140120
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Refactored from LoeriesAdmin
	20101020:	Added GetSite
	20140120:	Changed constructor to take CmsXProfileX (was displayX)
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>
	/// This class is a functional wrapper of the displayx web service
	/// </summary>
	public class Displayx : CmsWSSite {
		#region Private properties
		#endregion

		#region Constant name strings
		private const string logid = "Displayx.";
		private const string CONFIG_DISPLAY_SITE_ID = "displayxsite_service";
		private const string ROOT_NAME = "Displayx";
		#endregion

		#region Visible properties
		private DisplayXSiteWS.SiteDisplayxServiceX displayxSiteService;
		public DisplayXSiteWS.SiteDisplayxServiceX DisplayxSiteService {
			get { return displayxSiteService; }
		}
		#endregion

		#region Constant error strings
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public Displayx(CmsXProfileX thispage) : base(thispage, ROOT_NAME, "", typeof(Displayx), logid) {
		    initialize(true);
		}
		/// <summary>Common constructor</summary>
		public Displayx(CmsXProfileX thispage, string blogx_id) : base(thispage, ROOT_NAME, blogx_id, typeof(Displayx), logid) {
			initialize(false);
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize(bool showflag) {
			displayxSiteService = new DisplayXSiteWS.SiteDisplayxServiceX();
			displayxSiteService.Url = _GetUrl(CONFIG_DISPLAY_SITE_ID);
		}

		#endregion

		#region Public ShowDisplayx Methods
		public void GetMenus(SearchSettings Settings, bool IsDropDown) {
			xLogger.Debug("GetMenus::settings:", Settings.Page.ToString(), ":", Settings.Column, ":", Settings.Descending.ToString(), ":", Settings.Rows.ToString());
			XmlNode result;
			result = displayxSiteService.GetMenus(BlogxID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(ListXml.ImportNode(_CheckWSResult(result), true));
		}
		public void GetParents(SearchSettings Settings, bool IsDropDown) {
			xLogger.Debug("GetParents::settings:", Settings.Page.ToString(), ":", Settings.Column, ":", Settings.Descending.ToString(), ":", Settings.Rows.ToString());
			XmlNode result;
			result = displayxSiteService.GetParents(BlogxID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(ListXml.ImportNode(_CheckWSResult(result), true));
		}
		public void GetPages(SearchSettings Settings, bool IsDropDown) {
			xLogger.Debug("GetPages::settings:", Settings.Page.ToString(), ":", Settings.Column, ":", Settings.Descending.ToString(), ":", Settings.Rows.ToString());
			XmlNode result;
			result = displayxSiteService.GetPages(BlogxID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(ListXml.ImportNode(_CheckWSResult(result), true));
		}
		public void GetMenus(int menuID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(displayxSiteService.GetMenus(BlogxID))
					, true)
				);
		}
		public void GetMenu(int menuID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(displayxSiteService.GetMenu(BlogxID, menuID))
					, true)
				);
		}
		public void GetPage(int pageID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(displayxSiteService.GetPage(BlogxID, pageID))
					, true)
				);
		}
		public void AddMenu(string MenuName, string PageName, string DisplayValue, string Pattern, string ConcatId) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(displayxSiteService.AddMenu(BlogxID, MenuName, PageName, DisplayValue, Pattern, ConcatId)) 
					, true)
				);
		}
		public void DeleteMenu(int menuID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(displayxSiteService.DeleteMenu(BlogxID, menuID))
					, true)
				);
		}
		public void DeletePage(int pageID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(displayxSiteService.DeletePage(BlogxID, pageID))
					, true)
				);
		}

		public void EditMenu(int menuId, string menuName, string pageName, string displayValue, string pattern, string concatId) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				ItemXml.ImportNode(
					_CheckWSResult(displayxSiteService.EditMenu(BlogxID, menuId, menuName, pageName, displayValue, pattern, concatId)) 
					, true)
				);
		}

		public void DeleteMenuItem(int menuId, int mItemId) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				ItemXml.ImportNode(
					_CheckWSResult(displayxSiteService.DeleteMenuItem(BlogxID, menuId, mItemId))
					, true)
				);
		}

		public void AddMenuItem(int menuId,string mItemName,string page,string displayValue) { 
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				ItemXml.ImportNode(
					_CheckWSResult(displayxSiteService.AddMenuItem(BlogxID, menuId, mItemName, page, displayValue))
					, true)
				);
		}

		public void EditMenuItem(int menuId, int mItemId, string mItemName, string page, string displayValue) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				ItemXml.ImportNode(
					_CheckWSResult(displayxSiteService.EditMenuItem(BlogxID, menuId, mItemId, mItemName, page, displayValue))
					, true)
				);
		}

		public void AddPage(string name, string space, string topic, string blog) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				ItemXml.ImportNode(
					_CheckWSResult(displayxSiteService.AddPage(BlogxID, name, space, topic, blog))
					, true)
				);
		}

		public void GetThoughtspace() {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				ItemXml.ImportNode(
					_CheckWSResult(displayxSiteService.GetThoughtspace(BlogxID))
					, true)
				);
		}
		#endregion

		#region Public SiteDisplayx Methods
		public void GetSites() {
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
				ListXml.ImportNode(
					_CheckWSResult(displayxSiteService.GetSites())
					, true)
				);
		}

		public void GetSite(string blogxID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				ItemXml.ImportNode(
					_CheckWSResult(displayxSiteService.GetSite(blogxID))
					, true)
				);
		}
		#endregion
	}
}

