using System;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-09-27
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100927:	Starting point
	20101020:	Implemented list_admin
	20101227:	Updated contructor
	20140112:	Refactored constructor
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
    /// <summary>
    /// Description of the class.
    /// <para>Additional information about the class</para>
    /// </summary>
	public class CmsXBrokerTopics : CmsXBrokerBase {
		#region Invisible properties
        #endregion

        #region Constant name strings
		private const string logid = "CmsXBrokerTopics.";
		private const string PREFIX_SEARCH = "TOPICS_SEARCH";
		private const string PREFIX_SEARCHITEM = "TS_Topics_";
		private const string PROFILE_TOPICS_ID = "topicID";
		private const string PROFILE_FILTER_TOPICS_ID = "TS_Filter_TopicsID";
		private const string PROFILE_QUERY_TOPICS_ID = "TS_Query_TopicsID";
		#endregion

        #region Constant error strings
		private const string error_list = "Error listing the topics: ";
		private const string error_get = "Error getting the topic: ";
		private const string error_select = "Error selecting a topic: ";
		private const string error_submit = "Error updating a topic: ";
		private const string error_delete = "Error deleting a topic: ";
		//private const string error_profile = "Error setting profile for an topic: ";
		#endregion

        #region Visible properties
        #endregion

        #region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerTopics(CmsX thispage, string blogxID) : base(thispage, blogxID, typeof(CmsXBrokerTopics), logid) {
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
				UserProfile.Clear(PROFILE_FILTER_TOPICS_ID);
			}
			if (clearSearch) {
				xLogger.Debug("list_filter::clearSearch:");
				UserProfile.Clear(PREFIX_SEARCH);
				UserProfile.ClearAll(PREFIX_SEARCHITEM);
			}
			string TopicsID = _GetQueryID(PROFILE_QUERY_TOPICS_ID);
			xLogger.Debug("list_filter::seriesID:", TopicsID);
		}

		/// <summary>List topics</summary>
		private void list_admin() {
			list_admin(false);
		}

		/// <summary>List topics</summary>
		private void list_admin(bool clearProfile) {
			xLogger.Debug("list_admin:");
			xLogger.Info("list_admin", "::blogspace:", Cms.PROFILE_TOPICS_BLOGSPACE, "::default:", UserProfile.Value(Cms.PROFILE_SITE_SPACE));
			
			string blogspace = UserProfile.Value(Cms.PROFILE_TOPICS_BLOGSPACE, UserProfile.Value(Cms.PROFILE_SITE_SPACE));
			xLogger.Info("list_admin::blogspace:", blogspace);

			_AdminX.ListTopics(blogspace);
			UIPage.Content.AppendChild(UIPage.Document.ImportNode(_AdminX.ListXmlRoot, true));

			xLogger.Debug("list_admin:ok");
		}

		/// <summary>List topics</summary>
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
					UserProfile.Clear(PROFILE_FILTER_TOPICS_ID);
					UserProfile.Clear(PROFILE_QUERY_TOPICS_ID);
					
					// Set search items...
					SearchItem topicid = new SearchItem(PREFIX_SEARCHITEM, "ID", UserProfile, xLogger);
					// Do search...
					//_TopicsWS.SearchTopics(false,
					//        topicid.Type, topicid.Val,
					//        setting);
					if (is_csv) {
						//CsvUtil.GetColumns(_TopicsWS.ListItems);
					} else {
						//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_TopicsWS.ListXmlRoot, true));
					}
				} else {
					xLogger.Debug("list::search:", setting.Column, ":", setting.Page.ToString(), ":", setting.Rows.ToString());
					if (is_csv)
						setting.MaxRows = true;
						// Set filters
						int filterType = Int32.Parse(UserProfile.Value("XX_TypeID", "0"));
						// Do list...
						//_TopicsWS.ListTopics(filterType, setting);
					if (is_csv) {
						//CsvUtil.GetColumns(_TopicsWS.ListItems);
					} else {
						//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_TopicsWS.ListXmlRoot, true));
						xLogger.Debug("list:ok");
					}
				}
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("list::error_list:", e.Message);
				throw (new XException("error_list", String.Concat(error_list, e.Message)));
			}
		}
		
		/// <summary>Get a topic</summary> 
		private void get() {
			try {
				xLogger.Info(logid, "get:");
 				
				xLogger.Debug(logid, "get:ok");
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				throw (new XException("error_get", String.Concat(error_get, e.Message)));
			}
		}

		/// <summary>Select a topic</summary>
		private void select() {
			try {
				xLogger.Info("select:");

				xLogger.Debug("select:ok");
			} catch (XException e) {
				throw (new XException("error_select", String.Concat(error_select, e.Code, " - ", e.Message)));
			} catch (Exception e) {
				throw (new XException("error_select", String.Concat(error_select, e.Message)));
			}
		}

		/// <summary>Add/Edit a topic</summary> 
		private void submit(bool is_new) {
			try {
				xLogger.Debug(String.Concat(logid, "submit:is_new:", is_new.ToString()));

				xLogger.Debug(logid, "submit:ok");
			} catch (XException e) {
				xLogger.Debug(logid, "submit:xerror:", e.Code, "::", e.Message);
				throw e;
			} catch (Exception e) {
				xLogger.Debug(logid, "submit:error:", e.Message);
				throw (new XException("error_submit", String.Concat(error_submit, e.Message)));
			} finally {
				UserProfile.Add("topic_actions", "");
			}
		}

		/// <summary>List topics</summary>
		private void submit() {
			submit(false);
		}

		/// <summary>Delete a topic</summary> 
		protected override void delete() {
			delete_topic();
		}

		/// <summary>Delete a topic</summary> 
		private void delete_topic() {
			try {
				xLogger.Info(logid, "delete:");

				xLogger.Debug(logid, "delete:ok");
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				throw (new XException("error_delete", String.Concat(error_delete, e.Message)));
			}
		}
		#endregion

		#region Private utility methods
		#endregion
	}
}