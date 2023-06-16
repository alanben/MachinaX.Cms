using System;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100925:	Started from EconoVault
	20140112:	Refactored constructor
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
    /// <summary>
    /// Description of the LoerieAdminCategories class.
    /// <para>Additional information about the class</para>
    /// </summary>
	public class CmsXBrokerLinkLog : CmsXBrokerBase {
		#region Invisible properties
        #endregion

        #region Constant name strings
		private const string logid = "CmsXBrokerLinkLog.";
		private const string PREFIX_SEARCH = "LINKLOG_SEARCH";
		private const string PREFIX_SEARCHITEM = "TS_LinkLog_";
		private const string PROFILE_LINKLOG_ID = "linklogID";
		private const string PROFILE_FILTER_PERSON_ID = "TS_Filter_PersonID";
		#endregion

        #region Constant error strings
		private const string error_linklogs_list = "Error listing the linklogs: ";
		private const string error_linklog_get = "Error getting the linklog: ";
		private const string error_linklog_select = "Error selecting a linklog: ";
		private const string error_linklog_submit = "Error updating a linklog: ";
		private const string error_linklog_delete = "Error deleting a linklog: ";
		#endregion

        #region Visible properties
        #endregion

        #region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerLinkLog(CmsX thispage) : base(thispage, typeof(CmsXBrokerLinkLog), logid) {
		}
        #endregion

        #region Public methods
        public override void Process(string type) {
			base.Process((type == "list"));
			xLogger.Info("Process:", type);
            switch (type) {
				case "search"			: search();					break;
                case "list"				: list();                	break;
				case "list_csv"			: list(true);				break;
                case "list_dd"			: list_admin(true);     	break;
                case "list_admin"		: list_admin();         	break;
                case "get"				: get();                 	break;
                case "select"			: select();               	break;
                case "add"				: submit(true);          	break;
                case "edit"				: submit();              	break;
                case "delete"			: delete();              	break;
				// alternative way to handle deletes and duplications...
				//case "edit"				: _GridEdit(UIPage.QueryParam("field", "none")); break;
			}
        }

		/// <summary>
		/// Add the current link to the link log for the user (if logged on)
		/// </summary>
		/// <param name="user">The current user</param>
		public void Add(CmsXUser user) {
			//xLogger.Debug("Add:");
			if (!String.IsNullOrEmpty(user.Token)) {
				xLogger.Debug("Add", "::PersonID:", user.PersonID);
				_LinkLog.AddLinkLog(UIPage.Parameters.LinkName, user.Token, user.PersonIID);
			}
			//xLogger.Debug("Add:ok");
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
				UserProfile.Clear(PROFILE_FILTER_PERSON_ID);
			}
			if (clearSearch) {
				xLogger.Debug("list_filter::clearSearch:");
				UserProfile.Clear(PREFIX_SEARCH);
				UserProfile.ClearAll(PREFIX_SEARCHITEM);
			}
			xLogger.Debug("list_filter::ok:");
		}

		/// <summary>List linklogs</summary>
		private void list_admin() {
			list_admin(false);
		}

		/// <summary>List linklogs</summary>
		private void list_admin(bool clearProfile) {
			Logger.Info(String.Concat(logid, "list_admin:"));

			Logger.Debug(String.Concat(logid, "list_admin:ok"));
		}

		/// <summary>List linklogs</summary>
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
					// Set search items...
					SearchItem logid = new SearchItem(PREFIX_SEARCHITEM, "PersonID", UserProfile, xLogger);
					SearchItem logname = new SearchItem(PREFIX_SEARCHITEM, "Name", UserProfile, xLogger);
					SearchItem logdesc = new SearchItem(PREFIX_SEARCHITEM, "Link", UserProfile, xLogger);
					SearchItem datestart = new SearchItem(PREFIX_SEARCHITEM, "DateStart", UserProfile, xLogger);
					SearchItem dateend = new SearchItem(PREFIX_SEARCHITEM, "DateEnd", UserProfile, xLogger);
					// Do search...
					_LinkLog.SearchLinkLogs(false,
							logid.Type, logid.Val,
							logname.Type, logname.Val,
							logdesc.Type, logdesc.Val,
							datestart.Type, datestart.Val,
							dateend.Type, dateend.Val,
							setting);
					if (is_csv) {
						CmsExport.ExportType = ExportType.XLSX;
						CmsExport.GetColumns(_LinkLog.ListItems);
					} else {
						UIPage.Content.AppendChild(UIPage.Document.ImportNode(_LinkLog.ListXmlRoot, true));
					}
				} else {
					xLogger.Debug("list::search:", setting.Column, ":", setting.Page.ToString(), ":", setting.Rows.ToString());
					if (is_csv)
						setting.MaxRows = true;
						int filterPerson = Int32.Parse(UserProfile.Value(PROFILE_FILTER_PERSON_ID, "0"));
						_LinkLog.ListLinkLogs(filterPerson, setting);
					if (is_csv) {
						CmsExport.ExportType = ExportType.XLSX;
						CmsExport.GetColumns(_LinkLog.ListItems);
					} else {
						UIPage.Content.AppendChild(UIPage.Document.ImportNode(_LinkLog.ListXmlRoot, true));
						xLogger.Debug("list:ok");
					}
				}
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("list::error_linklogs_list:", e.Message);
				throw (new x_exception("error_linklogs_list", String.Concat(error_linklogs_list, e.Message)));
			}
		}
		
		/// <summary>Get a linklog</summary> 
		private void get() {
			try {
				string linklogID = _GetQueryID(PROFILE_LINKLOG_ID);
				xLogger.Info("get::linklogID:", linklogID);

				_LinkLog.GetLinkLog(Int32.Parse(linklogID));
				UserProfile.Add(PROFILE_LINKLOG_ID, linklogID);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_LinkLog.ItemXmlRootNode, true));
				Logger.Debug(String.Concat(logid, "get:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_linklog_get", String.Concat(error_linklog_get, e.Message)));
			}
		}

		/// <summary>Select a linklog</summary>
		private void select() {
			try {
				xLogger.Info("select:");

				xLogger.Debug("select:ok");
			} catch (x_exception e) {
				throw (new x_exception("error_linklog_select", String.Concat(error_linklog_select, e.Code, " - ", e.Message)));
			} catch (Exception e) {
				throw (new x_exception("error_linklog_select", String.Concat(error_linklog_select, e.Message)));
			}
		}

		/// <summary>Add/Edit a linklog</summary> 
		private void submit(bool is_new) {
			try {
				Logger.Debug(String.Concat(logid, "submit:is_new:", is_new.ToString()));

				Logger.Debug(String.Concat(logid, "submit:ok"));
			} catch (x_exception e) {
				Logger.Debug(String.Concat(logid, "submit:xerror:", e.Code, "::", e.Message));
				throw e;
			} catch (Exception e) {
				Logger.Debug(String.Concat(logid, "submit:error:", e.Message));
				throw (new x_exception("error_linklog_submit", String.Concat(error_linklog_submit, e.Message)));
			} finally {
				UserProfile.Add("linklog_actions", "");
			}
		}

		/// <summary>List linklogs</summary>
		private void submit() {
			submit(false);
		}

		/// <summary>Delete a linklog</summary> 
		protected override void delete() {
			try {
				Logger.Info(String.Concat(logid, "delete:"));

				Logger.Debug(String.Concat(logid, "delete:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_linklog_delete", String.Concat(error_linklog_delete, e.Message)));
			}
		}
		#endregion

		#region Private utility methods
		#endregion
	}
}