/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-12-06
	Status:		release	
	Version:	4.0.3
	Build:		20151223
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20101206:	Started from deprecated CmsXBrokerDisplay (for menus)
	20101218:	Updated contructor
	20101227:	Updated contructor
	20110426:	Split handleMenuItems in two passes, ie adedit~ and delete~
	20130418:	Added Process override to manage authentication
				(needed for Loeries as it does not use the same passport web service)
	20130513:	Added _IsDefault as test for DEFAULT_SUBITEM
	20140112:	Refactored constructor
	20151223:	Updated export to xlsx
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {

	using System;
	using System.Xml;

	/// <summary>
    /// Menus Broker
    /// </summary>
	public class CmsXBrokerMenus : CmsXBrokerBase {
		#region Invisible properties
        #endregion

        #region Constant name strings
		private const string logid = "CmsXBrokerMenus.";
		private const string PREFIX_SEARCH = "MENUS_SEARCH";
		private const string PREFIX_SEARCHITEM = "TS_Menus_";
		private const string PROFILE_MENUS_ID = "menuID";
		private const string PROFILE_FILTER_MENUS_ID = "TS_Filter_MenusID";
		private const string PROFILE_QUERY_MENUS_ID = "TS_Query_MenusID";
		private const string NO_PROFILE = "[none]";
		#endregion

        #region Constant error strings
		private const string error_menus_list = "Error listing the menus: ";
		private const string error_menu_get = "Error getting the menu: ";
		private const string error_menu_select = "Error selecting a menu: ";
		private const string error_menu_submit = "Error updating a menu: ";
		private const string error_menu_delete = "Error deleting a menu: ";
		//private const string error_menu_profile = "Error setting profile for an menu: ";
		#endregion

        #region Visible properties
        #endregion

        #region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerMenus(CmsX thispage, string blogxID) : base(thispage, blogxID, typeof(CmsXBrokerMenus), logid) {
		}
        #endregion

        #region Public methods
		public override void Process(string type) {
			Process(type, ProfilePage.Authenticate);
		}
        public override void Process(string type, bool authenticate) {
			xLogger.Info("Process::type:", type, "::authenticate:", authenticate.ToString());
			if (authenticate) {
				base.Process((type == "list"));
			}
			switch (type) {
				case "search":
					search();
					break;
				case "list_filter":
					list_filter(true, true);
					break;
				case "list":
					list();
					break;
				case "list_dd":
					list_admin(true);
					break;
				case "list_admin":
					list_admin();
					break;
				case "profiles_dd":
					profiles();
					break;
				case "get":
					get();
					break;
				case "getitems":
					getitems();
					break;
				case "list_items":
					list_items();
					break;
				case "select":
					select();
					break;
				case "add":
					submit(true);
					break;
				case "edit":
					submit();
					break;
				case "delete":
					delete();
					break;

				// alternative way to handle deletes and duplications...
				//case "edit"				: _GridEdit(UIPage.QueryParam("field", "none")); break;
			}
        }
        #endregion

        #region Protected methods
        #endregion

		#region Private process methods
		/// <summary>search</summary>
		private void search() {
			xLogger.Info("search:");
			UserProfile.Add(PREFIX_SEARCH, "yes");
			xLogger.Debug("search:ok");
		}

		/// <summary>Clear list search / filter</summary>
		private void list_filter(bool clearFilter, bool clearSearch) {
			xLogger.Debug("list_filter:");
			if (clearFilter) {
				xLogger.Debug("list_filter::clearFilter:");
				UserProfile.Clear(PROFILE_FILTER_MENUS_ID);
			}
			if (clearSearch) {
				xLogger.Debug("list_filter::clearSearch:");
				UserProfile.Clear(PREFIX_SEARCH);
				UserProfile.ClearAll(PREFIX_SEARCHITEM);
			}
			string MenusID = _GetQueryID(PROFILE_QUERY_MENUS_ID);
			xLogger.Debug("list_filter::seriesID:", MenusID);
		}

		/// <summary>List menus</summary>
		private void list_admin() {
			list_admin(false);
		}

		/// <summary>List menus</summary>
		private void list_admin(bool clearProfile) {
			xLogger.Info("list_admin:");
			_Menus.List();
			UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Menus.ListXmlRoot, true));
			xLogger.Debug("list_admin:ok");
		}

		/// <summary>List menus</summary>
		private void list() {
			xLogger.Info("list:");
			list(false);
		}

		/// <summary>Get list of series</summary>
		private void list(bool is_csv) {
			try {
				xLogger.Info("list:");
				SearchSettings setting = new SearchSettings(UserProfile, xLogger, is_csv);

				UserProfile.Add(PREFIX_SEARCH, (_IsSearchSet(PREFIX_SEARCHITEM, true)) ? "no" : "yes");
				xLogger.Debug("list::search:", UserProfile.Value(PREFIX_SEARCH));
				
				if (UserProfile.Value(PREFIX_SEARCH, "no") == "yes") {
					UserProfile.Clear(PROFILE_FILTER_MENUS_ID);
					UserProfile.Clear(PROFILE_QUERY_MENUS_ID);
					
					// Set search items...
					SearchItem menuid = new SearchItem(PREFIX_SEARCHITEM, "ID", UserProfile, xLogger);
					// Do search...
					//_Menus.SearchMenus(setting,
					//	,menuid.Type	,menuid.Val
					//);
					if (is_csv) {
						//CsvUtil.GetColumns(_Menus.ListItems);
					} else {
						//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Menus.ListXmlRoot, true));
					}
				} else {
					xLogger.Debug("list::search:", setting.Column, ":", setting.Page.ToString(), ":", setting.Rows.ToString());
					if (is_csv) {
						setting.MaxRows = true;
					}

					// Get filter(s) and call for list
					string filter = UserProfile.Value(PROFILE_FILTER_MENUS_ID, "");					// eg string filter
					xLogger.Debug("list::filter:", filter);
					_Menus.List(filter, setting);

					// Output list to csv or content page (for rendering to grid)
					if (is_csv) {
						CmsExport.ExportType = ExportType.XLSX;
						CmsExport.GetColumns(_Menus.ListItems);
					} else {
						UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Menus.ListXmlRoot, true));
						xLogger.Debug("list:ok");
					}
				}
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("list::error_menus_list:", e.Message);
				throw (new x_exception("error_menus_list", String.Concat(error_menus_list, e.Message)));
			}
		}
		
		/// <summary>Get a menu</summary> 
		private void get() {
			try {
				string menuID = _GetQueryID(PROFILE_MENUS_ID);
				xLogger.Info("get::menuID:", menuID);

				_Menus.GetMenu(Int32.Parse(menuID));
				UserProfile.Add(PROFILE_MENUS_ID, menuID);
				xLogger.Info("get::menuxml:", _Menus.ItemXmlRootNode.OuterXml);
				xLogger.Info("get::item:", _Menus.Item.OuterXml);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Menus.ItemXmlRootNode, true));
				xLogger.Debug("get:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_menu_get", String.Concat(error_menu_get, e.Message)));
			}
		}

		/// <summary>Get a menu's menuitems</summary> 
		private void getitems() {
			try {
				string menuID = _GetQueryID(PROFILE_MENUS_ID);
				xLogger.Debug("getmenuitems::menuID:", menuID);
				_Menus.GetMenuItems(Int32.Parse(menuID));

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Menus.ItemXmlRootNode, true));
				xLogger.Debug("getmenuitems:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_menu_get", String.Concat(error_menu_get, e.Message)));
			}
		}

		/// <summary>List menus</summary>
		private void list_items() {
			string menuID = UserProfile.Value(PROFILE_MENUS_ID);
			xLogger.Debug("list_items::menuID:", menuID);
			_Menus.ListMenuItems(Int32.Parse(menuID));

			UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Menus.ListXmlRoot, true));
			xLogger.Debug("list_items:ok");
		}

		/// <summary>Select a menu</summary>
		private void select() {
			try {
				xLogger.Info("select:");

				xLogger.Debug("select:ok");
			} catch (x_exception e) {
				throw (new x_exception("error_menu_select", String.Concat(error_menu_select, e.Code, " - ", e.Message)));
			} catch (Exception e) {
				throw (new x_exception("error_menu_select", String.Concat(error_menu_select, e.Message)));
			}
		}

		/// <summary>List menus</summary>
		private void submit() {
			submit(false);
		}

		/// <summary>Add/Edit a menu</summary> 
		private void submit(bool is_new) {
			try {
				xLogger.Debug("submit:is_new:", is_new.ToString());

				string menuid = UserProfile.Value("id", "0");
				menuid = (String.IsNullOrEmpty(menuid)) ? "0" : menuid;
				xLogger.Debug("submit:menuid:", menuid);

				string name = UserProfile.Value("name");
				string label = UserProfile.Value("label");
				string profile = UserProfile.Value("login");
				profile = (profile == NO_PROFILE || profile == "0") ? "" : profile;
				string display = UserProfile.Value("display");
				string parentid = UserProfile.Value("parentid");
				string order = UserProfile.Value("order");
				string position = UserProfile.Value("position");
				string logo = UserProfile.Value("logo");
				string link = UserProfile.Value("link");

				xLogger.Debug("submit::profile:", profile, "::display:", display, "::logo:", logo, "::link:", link);
				int menu_id = Int32.Parse(menuid);
				if (is_new) {
					_Menus.AddMenu(name, label, profile, display, parentid, logo, link);
					menuid = _Menus.ItemXml.SelectSingleNode("//menu/@id").InnerText;
					xLogger.Debug("submit:menuid:", menuid);
					menu_id = Int32.Parse(menuid);
				} else {
					_Menus.UpdateMenu(menu_id, name, label, profile, display, parentid, logo, link);
					_Menus.MoveMenu(menu_id, Int32.Parse(order));
				}
				xLogger.Debug("submit:menu_id:", menu_id.ToString());

				handleMenuItems("0_record_", menu_id, is_new);

				xLogger.Debug("submit:ok");
			} catch (x_exception e) {
				xLogger.Debug("submit:xerror:", e.Code, "::", e.Message);
				throw e;
			} catch (Exception e) {
				xLogger.Debug("submit:error:", e.Message);
				throw (new x_exception("error_menu_submit", String.Concat(error_menu_submit, e.Message)));
			} finally {
				UserProfile.Add("menu_actions", "");
			}
		}

		/// <summary>Add, update, remove subitems</summary>
		private void handleSubItems(string profilePrefix, string menu_id) {
			handleSubItems(profilePrefix, menu_id, false);
		}

		/// <summary>Add, update, remove credits</summary>
		private void handleSubItems(string profilePrefix, string menu_id, bool is_new) {
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string menuCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);

				if (!String.IsNullOrEmpty(menuCSV)) {
					string[] menu = menuCSV.Split(new char[] { '|' });
					xLogger.Debug("handleSubItems:", prof, ":", menuCSV, "; menu_id:", menu_id);
					// ie handleSubItems:0_record_0:default|default|1260|default|false|; menu_id:0

					string thismenu_id = menu[1];
					if (thismenu_id == "")
						break;
					else {
						string this_id = menu[2];
						string this_desc = menu[3];
						string this_remove = menu[4];
						if (this_remove == "true") {
							if (!(_IsDefault(thismenu_id ) || is_new)) {
								//_Yyyy.DeleteSubMenus(Int32.Parse(thismenu_id));
							}
						} else if (_IsDefault(this_id)) {
							// ignore this
						} else {
							if (_IsDefault(thismenu_id) || is_new) {
								xLogger.Debug("handleSubItems::add::menu_id:", menu_id, "::this_id:", this_id);
								//_Yyyy.AddSubMenus(Int32.Parse(menu_id), Int32.Parse(this_id));
							} else {
								xLogger.Debug("handleSubItems::update::thismenu_id:", thismenu_id, "::menu_id:", menu_id, "::this_id:", this_id);
								//_Yyyy.UpdateSubMenus(Int32.Parse(thismenu_id), Int32.Parse(menu_id), Int32.Parse(this_id));
							}
						}
					}
				}
			}
		}

		/// <summary>Add, update, remove menuitems</summary>
		private void handleMenuItems(string profilePrefix, int menu_id, bool is_new) {
			addeditMenuItems(profilePrefix, menu_id, is_new);
			deleteMenuItems(profilePrefix, menu_id, is_new);
		}

		/// <summary>Add, update, remove menuitems</summary>
		private void addeditMenuItems(string profilePrefix, int menu_id, bool is_new) {
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string menuitemsCSV = UserProfile.Value(prof, "");
				//UserProfile.Clear(prof);

				if (!String.IsNullOrEmpty(menuitemsCSV)) {
					string[] menuitems = menuitemsCSV.Split(new char[] { '|' });
					xLogger.Debug("addeditMenuItems:", prof, ":", menuitemsCSV);
					// ie handleMenuItems:0_record_2:105|105||service_contact_response|104||

					string thismenuitems_id = menuitems[0];
					if (thismenuitems_id == "")
						break;
					else {
						string this_id = menuitems[1];
						string this_lbl = menuitems[2];
						string this_lnk = menuitems[3];
						string this_dsp = menuitems[4];
						string this_ord = menuitems[5];
						string this_lgo = menuitems[6];
						string this_xtr = menuitems[7];
						// Used to have menuitem/menuitem, but now only menu/menuitem, so parent not relevant, but add order for ordering
						//string this_parid = menuitems[5];
						//string this_parnt = menuitems[6];
						string this_remove = menuitems[8];
						xLogger.Debug("addeditMenuItems::thismenuitems_id:", thismenuitems_id, "::this_id:", this_id, "::this_lbl:", this_lbl, "::this_lnk:", this_lnk, "::this_dsp:", this_dsp, "::this_ord:", this_ord, "::this_lgo:", this_lgo);
						xLogger.Debug("addeditMenuItems::this_remove:", this_remove);
						//string parid = (this_parid == "Select parent ID...") ? "" : this_parid;
						string lbl = (this_lbl == "Enter label...") ? "" : this_lbl.Trim();
						int order = 0;
						Int32.TryParse(this_ord, out order);
						if (this_remove == "true" || this_lnk == "Select link...") {
							// ignore this
						} else {
							if (_IsDefault(thismenuitems_id) || is_new) {
								xLogger.Debug("addeditMenuItems::add:");
								_Menus.AddMenuItem(menu_id, lbl, this_lnk, this_dsp, order, this_lgo, this_xtr);
							} else {
								xLogger.Debug("addeditMenuItems::update:");
								int id = Int32.Parse(this_id);
								_Menus.UpdateMenuItem(menu_id, id, lbl, this_lnk, this_dsp, order, this_lgo, this_xtr);
							}
						}

					}
				}
			}
		}

		/// <summary>Add, update, remove menuitems</summary>
		private void deleteMenuItems(string profilePrefix, int menu_id, bool is_new) {
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string menuitemsCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);

				if (!String.IsNullOrEmpty(menuitemsCSV)) {
					string[] menuitems = menuitemsCSV.Split(new char[] { '|' });
					xLogger.Debug("handleMenuItems:", prof, ":", menuitemsCSV);
					// ie handleMenuItems:0_record_2:105|105||service_contact_response|104||

					string thismenuitems_id = menuitems[0];
					if (thismenuitems_id == "")
						break;
					else {
						string this_id = menuitems[1];
						string this_lbl = menuitems[2];
						string this_lnk = menuitems[3];
						string this_dsp = menuitems[4];
						string this_ord = menuitems[5];
						string this_lgo = menuitems[6];
						// Used to have menuitem/menuitem, but now only menu/menuitem, so parent not relevant, but add order for ordering
						//string this_parid = menuitems[4];
						//string this_parnt = menuitems[5];
						string this_remove = menuitems[7];
						//xLogger.Debug("deleteMenuItems::thismenuitems_id:", thismenuitems_id, "::this_id:", this_id, "::this_lbl:", this_lbl, "::this_lnk:", this_lnk, "::this_ord:", this_ord);
						xLogger.Debug("deleteMenuItems::this_remove:", this_remove);
						//string parid = (this_parid == "Select parent ID...") ? "" : this_parid;
						//string lbl = (this_lbl == "Enter label...") ? "" : this_lbl.Trim();

						if (this_remove == "true") {
							if (!(_IsDefault(thismenuitems_id) || is_new)) {
								xLogger.Debug("deleteMenuItems::delete:");
								int id = Int32.Parse(this_id);
								_Menus.DeleteMenuItem(id);
							}
						} 

					}
				}
			}
		}

		/// <summary>Delete a menu</summary> 
		protected override void delete() {
			delete_menu();
		}

		/// <summary>Delete a menu</summary> 
		private void delete_menu() {
			try {
				xLogger.Info("delete_menu:");

				string menuid =  UIPage.QueryParam("id", "0");
				xLogger.Debug("delete_menu:menuid:", menuid);
				int menu_id = Int32.Parse(menuid);

				_Menus.DeleteMenu(menu_id);
				xLogger.Debug("delete_menu:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_menu_delete", String.Concat(error_menu_delete, e.Message)));
			}
		}
		#endregion

		#region Private menu dd methods
		/// <summary>Get list of profiles</summary>
		private void profiles() {
			try {
				xLogger.Info("profiles:");
				_Menus.ListProfiles();
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Menus.ListXmlRoot, true));
				xLogger.Debug("profiles:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("profiles::error_menus_list:", e.Message);
				throw (new x_exception("error_menus_list", String.Concat(error_menus_list, e.Message)));
			}
		}
		#endregion

		#region Private utility methods
		#endregion
	}
}