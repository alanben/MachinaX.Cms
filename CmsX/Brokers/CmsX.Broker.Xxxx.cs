using System;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2011-05-04
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20110504:	Started
	20130513:	Added _IsDefault as test for DEFAULT_SUBITEM
	20140112:	Refactored constructor
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
    /// <summary>
    /// Description of the class.
    /// <para>Additional information about the class</para>
    /// </summary>
	public class CmsXBrokerXxxx : CmsXBrokerBase {
		#region Invisible properties
        #endregion

        #region Constant name strings
		private const string logid = "CmsXBrokerXxxx.";
		private const string PREFIX_SEARCH = "XXX_SEARCH";
		private const string PREFIX_SEARCHITEM = "TS_Xxxx_";
		private const string PROFILE_XXX_ID = "xxxID";
		private const string PROFILE_FILTER_XXX_ID = "TS_Filter_XxxxID";
		private const string PROFILE_QUERY_XXX_ID = "TS_Query_XxxxID";
		#endregion

        #region Constant error strings
		private const string error_zzzs_list = "Error listing the xxxs: ";
		private const string error_zzzs_get = "Error getting the xxx: ";
		private const string error_zzzs_select = "Error selecting a xxx: ";
		private const string error_zzzs_submit = "Error updating a xxx: ";
		private const string error_zzzs_delete = "Error deleting a xxx: ";
		//private const string error_zzzs_profile = "Error setting profile for an xxx: ";
		#endregion

        #region Visible properties
        #endregion

        #region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerXxxx(CmsX thispage) : base(thispage, typeof(CmsXBrokerXxxx), logid) {
		}
        #endregion

        #region Public methods
        public override void Process(string type) {
			base.Process((type == "list"));
			xLogger.Info("Process:", type);
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
				case "get":
					get();
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
				UserProfile.Clear(PROFILE_FILTER_XXX_ID);
			}
			if (clearSearch) {
				xLogger.Debug("list_filter::clearSearch:");
				UserProfile.Clear(PREFIX_SEARCH);
				UserProfile.ClearAll(PREFIX_SEARCHITEM);
			}
			string XxxxID = _GetQueryID(PROFILE_QUERY_XXX_ID);
			xLogger.Debug("list_filter::seriesID:", XxxxID);
		}

		/// <summary>List xxxs</summary>
		private void list_admin() {
			list_admin(false);
		}

		/// <summary>List xxxs</summary>
		private void list_admin(bool clearProfile) {
			xLogger.Info("list_admin:");

			xLogger.Debug("list_admin:ok");
		}

		/// <summary>List xxxs</summary>
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
					UserProfile.Clear(PROFILE_FILTER_XXX_ID);
					UserProfile.Clear(PROFILE_QUERY_XXX_ID);
					
					// Set search items...
					SearchItem xxxid = new SearchItem(PREFIX_SEARCHITEM, "ID", UserProfile, xLogger);
					// Do search...
					//_XxxxWS.SearchXxxx(setting,
					//	,xxxid.Type	,xxxid.Val
					//);
					if (is_csv) {
						//CsvUtil.GetColumns(_XxxxWS.ListItems);
					} else {
						//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_XxxxWS.ListXmlRoot, true));
					}
				} else {
					xLogger.Debug("list::search:", setting.Column, ":", setting.Page.ToString(), ":", setting.Rows.ToString());
					if (is_csv) {
						setting.MaxRows = true;
					}

					// Get filter(s) and call for list
					string filter = UserProfile.Value(PROFILE_FILTER_XXX_ID, "");					// eg string filter
					int filterType = Int32.Parse(UserProfile.Value(PROFILE_FILTER_XXX_ID, "0"));	// eg int filter
					//_XxxxWS.ListXxxx(filterType, setting);

					// Output list to csv or content page (for rendering to grid)
					if (is_csv) {
						//CsvUtil.GetColumns(_XxxxWS.ListItems);
					} else {
						//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_XxxxWS.ListXmlRoot, true));
						xLogger.Debug("list:ok");
					}
				}
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("list::error_zzzs_list:", e.Message);
				throw (new x_exception("error_zzzs_list", String.Concat(error_zzzs_list, e.Message)));
			}
		}
		
		/// <summary>Get a xxx</summary> 
		private void get() {
			try {
				string xxxID = _GetQueryID(PROFILE_XXX_ID);
				xLogger.Info("get::xxxID:", xxxID);

				//_XxxxWS.GetXxxx(xxxID);
				UserProfile.Add(PROFILE_XXX_ID, xxxID);

				//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_XxxxWS.ItemXmlRootNode, true));
				xLogger.Debug("get:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_zzzs_get", String.Concat(error_zzzs_get, e.Message)));
			}
		}

		/// <summary>Select a xxx</summary>
		private void select() {
			try {
				xLogger.Info("select:");

				xLogger.Debug("select:ok");
			} catch (x_exception e) {
				throw (new x_exception("error_zzzs_select", String.Concat(error_zzzs_select, e.Code, " - ", e.Message)));
			} catch (Exception e) {
				throw (new x_exception("error_zzzs_select", String.Concat(error_zzzs_select, e.Message)));
			}
		}

		/// <summary>Add/Edit a xxx</summary> 
		private void submit(bool is_new) {
			try {
				xLogger.Debug("submit:is_new:", is_new.ToString());

				string xxxid = UserProfile.Value("id", "0");
				xxxid = (String.IsNullOrEmpty(xxxid)) ? "0" : xxxid;
				xLogger.Debug("submit:xxxid:", xxxid);

				int xxx_id = Int32.Parse(xxxid);
				if (is_new) {
					xLogger.Debug("submit:xxx_id:", xxx_id.ToString());
					//_XxxxWS.AddXxxx(name, desc, user_id);
					//xxx_id = Int32.Parse(_XxxxWS.ItemID);
				} else {
					//_XxxxWS.UpdateXxxx(extract_id, name, desc, user_id);
				}

				handleSubItems("0_record_", xxx_id.ToString(), is_new);

				xLogger.Debug("submit:ok");
			} catch (x_exception e) {
				xLogger.Debug("submit:xerror:", e.Code, "::", e.Message);
				throw e;
			} catch (Exception e) {
				xLogger.Debug("submit:error:", e.Message);
				throw (new x_exception("error_zzzs_submit", String.Concat(error_zzzs_submit, e.Message)));
			} finally {
				UserProfile.Add("xxx_actions", "");
			}
		}

		/// <summary>List xxxs</summary>
		private void submit() {
			submit(false);
		}

		/// <summary>Add, update, remove subitems</summary>
		private void handleSubItems(string profilePrefix, string xxx_id) {
			handleSubItems(profilePrefix, xxx_id, false);
		}

		/// <summary>Add, update, remove credits</summary>
		private void handleSubItems(string profilePrefix, string xxx_id, bool is_new) {
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string xxxCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);

				if (!String.IsNullOrEmpty(xxxCSV)) {
					string[] xxx = xxxCSV.Split(new char[] { '|' });
					xLogger.Debug("handleSubItems:", prof, ":", xxxCSV, "; xxx_id:", xxx_id);
					// ie handleSubItems:0_record_0:default|default|1260|default|false|; xxx_id:0

					string thisxxx_id = xxx[1];
					if (thisxxx_id == "")
						break;
					else {
						string this_id = xxx[2];
						string this_desc = xxx[3];
						string this_remove = xxx[4];
						if (this_remove == "true") {
							if (!(_IsDefault(thisxxx_id) || is_new)) {
								//_XxxxWS.DeleteSubXxxx(Int32.Parse(thisxxx_id));
							}
						} else if (_IsDefault(this_id)) {
							// ignore this
						} else {
							if (_IsDefault(thisxxx_id) || is_new) {
								xLogger.Debug("handleSubItems::add::xxx_id:", xxx_id, "::this_id:", this_id);
								//_XxxxWS.AddSubXxxx(Int32.Parse(xxx_id), Int32.Parse(this_id));
							} else {
								xLogger.Debug("handleSubItems::update::thisxxx_id:", thisxxx_id, "::xxx_id:", xxx_id, "::this_id:", this_id);
								//_XxxxWS.UpdateSubXxxx(Int32.Parse(thisxxx_id), Int32.Parse(xxx_id), Int32.Parse(this_id));
							}
						}
					}
				}
			}
		}

		/// <summary>Delete a xxx</summary> 
		protected override void delete() {
			delete_xxx();
		}

		/// <summary>Delete a xxx</summary> 
		private void delete_xxx() {
			try {
				xLogger.Info("delete_xxx:");

				xLogger.Debug("delete_xxx:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_zzzs_delete", String.Concat(error_zzzs_delete, e.Message)));
			}
		}
		#endregion

		#region Private utility methods
		#endregion
	}
}