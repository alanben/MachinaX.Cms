/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-09-27
	Status:		release	
	Version:	4.0.3
	Build:		20151223
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100927:	Starting point
	20101218:	Finished initial.
	20101227:	Updated contructor, Activated delete process
	20130513:	Added _IsDefault as test for DEFAULT_SUBITEM
	20140112:	Refactored constructor
	20151223:	Updated export to xlsx
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {

	using System;
	using System.Xml;

	/// <summary>
    /// Spaces Broker
    /// </summary>
	public class CmsXBrokerSpaces : CmsXBrokerBase {
		#region Invisible properties
        #endregion

        #region Constant name strings
		private const string logid = "CmsXBrokerSpaces.";
		private const string PREFIX_SEARCH = "SPACES_SEARCH";
		private const string PREFIX_SEARCHITEM = "TS_Spaces_";
		private const string PROFILE_SPACE_ID = "spaceID";
		private const string PROFILE_FILTER_SPACES_ID = "TS_Filter_SpacesID";
		private const string PROFILE_QUERY_SPACES_ID = "TS_Query_SpacesID";

		private const string ADMINX_USER = "administrator";
		#endregion

        #region Constant error strings
		private const string error_list = "Error listing the spaces: ";
		private const string error_get = "Error getting the space: ";
		private const string error_select = "Error selecting a space: ";
		private const string error_submit = "Error updating a space: ";
		private const string error_delete = "Error deleting a space: ";
		private const string error_space_blog = "Error getting / saving space blog: ";
		#endregion

        #region Visible properties
        #endregion

        #region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerSpaces(CmsXProfileX thispage, string blogxID) : base(thispage, blogxID, typeof(CmsXBrokerSpaces), logid) {
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
				case "get_topics":
					gettopics();
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
				case "getblog":
					getblog();
					break;
				case "saveblog":
					saveblog();
					break;
				case "gettopicblog":
					gettopicblog();
					break;
				case "savetopicblog":
					savetopicblog();
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
				UserProfile.Clear(PROFILE_FILTER_SPACES_ID);
			}
			if (clearSearch) {
				xLogger.Debug("list_filter::clearSearch:");
				UserProfile.Clear(PREFIX_SEARCH);
				UserProfile.ClearAll(PREFIX_SEARCHITEM);
			}
			string SpacesID = _GetQueryID(PROFILE_QUERY_SPACES_ID);
			xLogger.Debug("list_filter::seriesID:", SpacesID);
		}

		/// <summary>List spaces</summary>
		private void list_admin() {
			list_admin(false);
		}

		/// <summary>List spaces</summary>
		private void list_admin(bool clearProfile) {
			Logger.Info(String.Concat(logid, "list_admin:"));

			//_Spaces.ListSpaces();
			UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Spaces.ListXmlRoot, true));

			Logger.Debug(String.Concat(logid, "list_admin:ok"));
		}

		/// <summary>List spaces</summary>
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
					UserProfile.Clear(PROFILE_FILTER_SPACES_ID);
					UserProfile.Clear(PROFILE_QUERY_SPACES_ID);
					
					// Set search items...
					SearchItem spaceid = new SearchItem(PREFIX_SEARCHITEM, "ID", UserProfile, xLogger);
					// Do search...
					//_SpacesWS.SearchSpaces(false,
					//        spaceid.Type, spaceid.Val,
					//        setting);
					if (is_csv) {
						//CsvUtil.GetColumns(_SpacesWS.ListItems);
					} else {
						//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_SpacesWS.ListXmlRoot, true));
					}
				} else {
					xLogger.Debug("list::search:", setting.Column, ":", setting.Page.ToString(), ":", setting.Rows.ToString());
					if (is_csv) {
						setting.MaxRows = true;
					}
					string filterType = "";
					// Do list...
					_Spaces.List(filterType, setting);
					
					if (is_csv) {
						CmsExport.ExportType = ExportType.XLSX;
						CmsExport.GetColumns(_Spaces.ListItems);
					} else {
						UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Spaces.ListXmlRoot, true));
						xLogger.Debug("list:ok");
					}
				}
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("list::error_list:", e.Message);
				throw (new x_exception("error_list", String.Concat(error_list, e.Message)));
			}
		}
		
		/// <summary>Get a space</summary> 
		private void get() {
			try {
				string spaceID = _GetQueryID(PROFILE_SPACE_ID);
				xLogger.Info("get::spaceID:", spaceID);

				_Spaces.GetSpace(spaceID);
				UserProfile.Add(PROFILE_SPACE_ID, spaceID);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Spaces.ItemXmlRootNode, true));
				xLogger.Debug("get:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_get", String.Concat(error_get, e.Message)));
			}
		}

		/// <summary>Get a space's topics</summary> 
		private void gettopics() {
			try {
				string spaceID = _GetQueryID(PROFILE_SPACE_ID);
				xLogger.Info("gettopics::spaceID:", spaceID);

				_Spaces.GetSpaceTopics(spaceID);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Spaces.ItemXmlRootNode, true));
				xLogger.Debug("gettopics:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_get", String.Concat(error_get, e.Message)));
			}
		}

		/// <summary>Select a space</summary>
		private void select() {
			try {
				xLogger.Info("select:");

				xLogger.Debug("select:ok");
			} catch (x_exception e) {
				throw (new x_exception("error_select", String.Concat(error_select, e.Code, " - ", e.Message)));
			} catch (Exception e) {
				throw (new x_exception("error_select", String.Concat(error_select, e.Message)));
			}
		}

		/// <summary>List spaces</summary>
		private void submit() {
			submit(false);
		}

		/// <summary>Add/Edit a space</summary> 
		private void submit(bool is_new) {
			try {
				Logger.Debug(String.Concat(logid, "submit:is_new:", is_new.ToString()));

				string spaceid = UserProfile.Value("id");
				xLogger.Debug("submit:spaceid:", spaceid);

				//int space_id = Int32.Parse(spaceid);
				string name = UserProfile.Value("name");
				string label = UserProfile.Value("label");
				string descr = UserProfile.Value("description");
				string type = UserProfile.Value("type");
				string active = UserProfile.Value("active");
				string topics = UserProfile.Value("topics");

				string blogtitle = UserProfile.Value("blogtitle");
				string blogauthor = UserProfile.Value("blogauthor", _User.Fullname);
				string blogdate = UserProfile.Value("blogdate");
				string bloghtml = UserProfile.Value("bloghtml");

				if (is_new) {
					_Spaces.AddSpace(name, label, descr, type, active, blogauthor, blogtitle, bloghtml);
				} else {
					_Spaces.UpdateSpace(name, label, descr, type, active, blogauthor, blogtitle, bloghtml);
				}

				handleTopics("0_record_", name, is_new);

				Logger.Debug(String.Concat(logid, "submit:ok"));
			} catch (x_exception e) {
				Logger.Debug(String.Concat(logid, "submit:xerror:", e.Code, "::", e.Message));
				throw e;
			} catch (Exception e) {
				Logger.Debug(String.Concat(logid, "submit:error:", e.Message));
				throw (new x_exception("error_submit", String.Concat(error_submit, e.Message)));
			} finally {
				UserProfile.Add("space_actions", "");
			}
		}

		/// <summary>Add, update, remove credits</summary>
		private void handleTopics(string profilePrefix, string space, bool is_new) {
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string topicsCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);

				if (!String.IsNullOrEmpty(topicsCSV)) {
					string[] topics = topicsCSV.Split(new char[] { '|' });
					string topic_id = topics[0];
					xLogger.Debug("handleSubItems::", prof, ":", topicsCSV, "::topic_id:", topic_id);
					// eg: handleSubItems::0_record_1:testsite|default|testsite|default|default|TestSiteX Default Topic|1|DEFAULT|Edit content||	- no change
					// eg: handleSubItems::0_record_0:[...]|[...]|Enter name...|Enter label...| |Enter title...| |false|							- new
					// eg: handleSubItems::0_record_5:testsite|test|testsite|test|test|TestSiteX Test Topic||TEST|Edit content|true|				- delete

					if (topic_id == "") {
						break;
					}

					if (_IsDefault(topic_id)) {
						string name = topics[2];
						string labl = topics[3];
						string titl = topics[5];
						xLogger.Debug("handleSubItems::add::name:", name);
						_Spaces.AddSpaceTopic(space, name, labl, _User.Fullname, titl);
					} else {
						string name = topics[1];
						string labl = topics[5];
						string titl = topics[7];
						string remv = topics[9];
						xLogger.Debug("handleSubItems::update::space:", space, "::name:", name, "::labl:", labl, "::titl:", titl, "::remv:", remv);
						if (remv == "true") {
							if (!(_IsDefault(topic_id) || is_new)) {
								_Spaces.DeleteSpaceTopic(space, name);
							}
						} else if (name == "default") {	// ie dont touch the default topic
							// ignore this
						} else {
							xLogger.Debug("handleSubItems::update::name:", name);
							_Spaces.UpdateSpaceTopic(space, name, labl, titl);
						}
					}
				}
			}
		}

		/// <summary>Delete a space</summary> 
		protected override void delete() {
			delete_space();
		}

		/// <summary>Delete a space</summary> 
		private void delete_space() {
			try {
				string spaceid = _GetQueryID(PROFILE_SPACE_ID);
				xLogger.Info("delete::spaceid:", spaceid);

				_Spaces.DeleteSpace(spaceid);

				xLogger.Debug("delete:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_delete", String.Concat(error_delete, e.Message)));
			}
		}
		/// <summary>Get a space's blogs</summary> 
		private void getblog() {
			try {
				string spaceID = _GetQueryID(PROFILE_SPACE_ID);
				xLogger.Info("get::spaceID:", spaceID);

				_Spaces.GetSpace(spaceID);
				UserProfile.Add(PROFILE_SPACE_ID, spaceID);
				UserProfile.Add("blogSpace", spaceID);
				
				xLogger.Info("get", "::Item:", _Spaces.Item.OuterXml);
				XmlElement blog = _Spaces.Item.SelectSingleNode("blog") as XmlElement;
				if (blog != null) {
					string html = blog.InnerText;
					html = UIPage.Server.HtmlDecode(html);
					UserProfile.Add("blogHtml", html);
				} else {
					UserProfile.Clear("blogHtml");
				}

				xLogger.Debug("getblog:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_space_blog", String.Concat(error_space_blog, e.Message)));
			}
		}

		/// <summary>Save a space's blog content</summary> 
		private void saveblog() {
			try {
				xLogger.Info("saveblog:");
				string space = UserProfile.Value("blogSpace");
				string title = UserProfile.Value("blogTitle");
				string html	= UserProfile.Value("blogHtml");

				html = processTinymceHtml(html);
				xLogger.Debug("saveblog: Bloghtml::", html);
				_Spaces.UpdateSpaceBlog(space, _User.Fullname, title, html);
				xLogger.Debug("saveblog::finished:ok");

				xLogger.Debug("saveblog:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_space_blog", String.Concat(error_space_blog, e.Message)));
			}
		}

		/// <summary>Get a space's blogs</summary> 
		private void gettopicblog() {
			try {
				string topicID = _GetQueryID(PROFILE_SPACE_ID);
				xLogger.Info("gettopicblog::topicID:", topicID);
				string [] split = topicID.Split(new Char[] { '|' });
				UserProfile.Add("blogSpace", split[0]);
				UserProfile.Add("blogTopic", split[1]);

				_Spaces.GetSpaceTopic(split[0], split[1]);

				xLogger.Info("gettopicblog", "::ItemXmlRoot:", _Spaces.ItemXmlRoot.OuterXml);
				XmlElement blog = _Spaces.ItemXmlRoot.SelectSingleNode("//topic/blog") as XmlElement;
				if (blog != null) {
					string title = blog.SelectSingleNode("title").InnerText;
					string html = blog.SelectSingleNode("bloghtml").InnerText;
					html = UIPage.Server.HtmlDecode(html);

					UserProfile.Add("blogTitle", title);
					UserProfile.Add("blogHtml", html);
				} else {
					UserProfile.Clear("blogTitle");
					UserProfile.Clear("blogHtml");
				}

				xLogger.Debug("getblog::ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_space_blog", String.Concat(error_space_blog, e.Message)));
			}
		}

		/// <summary>Save a space's blog content</summary> 
		private void savetopicblog() {
			try {
				//xLogger.Info("saveblog:");
				string space = UserProfile.Value("blogSpace");
				string topic = UserProfile.Value("blogTopic");
				string title = UserProfile.Value("blogTitle");
				string html	= UserProfile.Value("blogHtml");

				html = processTinymceHtml(html);
				//xLogger.Debug("savetopicblog: Bloghtml::", html);
				_Spaces.UpdateSpaceTopicBlog(space, topic, _User.Fullname, title, html);

				xLogger.Debug("savetopicblog::ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_space_blog", String.Concat(error_space_blog, e.Message)));
			}
		}

		#endregion

		#region Private utility methods
		#endregion
	}
}