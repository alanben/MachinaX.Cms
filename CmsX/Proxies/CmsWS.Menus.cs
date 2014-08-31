using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-12-06
	Status:		release	
	Version:	4.0.2
	Build:		20140120
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20101206:	Started.
	20140120:	Changed constructor to take CmsXProfileX (was displayX)
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>This class is a functional wrapper of the DisplayXMenus web service</summary>
	public class Menus : CmsWSSite {
		#region Private properties
		#endregion

		#region Constant name strings
		private const string logid = "Menus.";
		private const string CONFIG_ID = "menus_service";
		private const string ROOT_NAME = "Menus";
		#endregion

		#region Visible properties
		private DisplayXMenusWS.DisplayXMenus menusService;
		public DisplayXMenusWS.DisplayXMenus MenusService {
			get { return menusService; }
		}
		#endregion

		#region Constant error strings
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public Menus(CmsXProfileX thispage, string blogx_id) : base(thispage, ROOT_NAME, blogx_id, typeof(Menus), logid) {
			initialize();
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize() {
			xLogger.Debug("initialize::");
			menusService = new DisplayXMenusWS.DisplayXMenus();
			menusService.Url = _GetUrl(CONFIG_ID);
			xLogger.Debug("initialize:menusService:", menusService.Url);
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Get a list of all menus
		/// </summary>
		public void List() {
			xLogger.Debug("List", "::BlogxID:", BlogxID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							menusService.ListDD(BlogxID)
						   )
					, true)
				);
		}

		/// <summary>
		/// Get a list of all menus
		/// </summary>
		/// <param name="Settings">Page/Order settings</param>
		public void ListAll(string Profile, SearchSettings Settings) {
			xLogger.Debug("List", "::BlogxID:", BlogxID, "::Profile:", Profile, "::Column:", Settings.Column, "::Descending:", Settings.Descending.ToString(), "::Number:", Settings.Number.ToString());
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							menusService.ListAll(BlogxID, Profile, Settings.Column, !Settings.Descending)
						   )
					, true)
				);
		}

		/// <summary>
		/// Get a list of all menus
		/// </summary>
		/// <param name="Settings">Page/Order settings</param>
		public void List(string Profile, SearchSettings Settings) {
			xLogger.Debug("List", "::BlogxID:", BlogxID, "::Profile:", Profile, "::Column:", Settings.Column, "::Descending:", Settings.Descending.ToString(), "::Number:", Settings.Number.ToString(), "::Page:", Settings.Page.ToString(), "::Rows:", Settings.Rows.ToString());
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							menusService.List(BlogxID, Profile, Settings.Column, !Settings.Descending, Settings.Number, Settings.Page, Settings.Rows)
						   )
					, true)
				);
		}

		/// <summary>
		/// Get a list of all menu profiles
		/// </summary>
		public void ListProfiles() {
			xLogger.Debug("ListProfiles", "::BlogxID:", BlogxID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							menusService.ListProfiles(BlogxID)
						   )
					, true)
				);
		}

		public void GetMenu(int MenuID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(menusService.Get(BlogxID, MenuID))
					, true)
				);
		}

		public void DeleteMenu(int MenuID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(menusService.Delete(BlogxID, MenuID))
					, true)
				);
		}

		public void AddMenu(string Name, string Label, string Profile, string Display, string ParentID, string Logo) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(menusService.Add(BlogxID, Name, Label, Profile, Display, ParentID, Logo))
					, true)
				);
		}

		public void UpdateMenu(int MenuID, string Name, string Label, string Profile, string Display, string ParentID, string Logo) {
			xLogger.Debug("UpdateMenu", "::BlogxID:", BlogxID, "::Profile:", Profile);
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(menusService.Edit(BlogxID, MenuID, Name, Label, Profile, Display, ParentID, Logo))
					, true)
				);
		}

		public void MoveMenu(int MenuID, int Increment) {
			xLogger.Debug("MoveMenu", "::BlogxID:", BlogxID, "::Increment:", Increment.ToString());
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(menusService.Order(BlogxID, MenuID, Increment))
					, true)
				);
		}

		/*
				/// <summary>
				/// Get a list of all menus by search
				/// </summary>
				public void Search(SearchSettings Settings, int ProfileSrchType, string Profile, int NameSrchType, string Name, int LabelSrchType, string Label) {
					xLogger.Debug("List", "::BlogxID:", BlogxID, "::Profile:", Profile, "::Name:", Name, "::Label:", Label);
					ListXmlRoot.RemoveAll();
					ListXmlRoot.AppendChild(
							ListXml.ImportNode(
								_CheckWSResult(
									menusService.Search(BlogxID, ProfileSrchType, Profile, NameSrchType, Name, LabelSrchType, Label, Settings.Column, !Settings.Descending, Settings.Number, Settings.Page, Settings.Rows)
									)
								, true)
						);
				}

		*/
		#endregion

		#region Public MenuItems Methods
		/// <summary>
		/// Get a list of all menuitems for a menu
		/// </summary>
		public void ListMenuItems(int MenuID) {
			xLogger.Debug("ListMenuItems", "::MenuID:", MenuID.ToString());
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							menusService.ListMenuItems(BlogxID, MenuID)
						   )
					, true)
				);
		}

		/// <summary>
		/// Get the menu's flags
		/// </summary>
		public void GetMenuItems(int MenuID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(menusService.GetMenuItems(BlogxID, MenuID))
					, true)
				);
		}

		public void DeleteMenuItem(int MenuItemID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(menusService.DeleteMenuItem(BlogxID, MenuItemID))
					, true)
				);
		}

		public void AddMenuItem(int MenuID, string Label, string Link, string Display, int Order, string Logo) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(menusService.AddMenuItem(BlogxID, MenuID, Label, Link, Display, Order, Logo))
					, true)
				);
		}

		public void UpdateMenuItem(int MenuID, int MenuItemID, string Label, string Link, string Display, int Order, string Logo) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(menusService.EditMenuItem(BlogxID, MenuID, MenuItemID, Label, Link, Display, Order, Logo))
					, true)
				);
		}
		#endregion

	}
}

