/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2011-05-04
	Status:		release	
	Version:	4.0.3
	Build:		20151223
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20110504:	Started
	20110519:	Updated submit(true)
	20151223:	Updated export to xlsx
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {

	using System;
	using System.Xml;

	/// <summary>
    /// News Broker
    /// </summary>
	public class CmsXBrokerNews : CmsXBrokerBlogs {
		#region Invisible properties
        #endregion

        #region Constant name strings
		private const string logid = "CmsXBrokerNews.";
		private const string PREFIX_SEARCH = "NEWS_SEARCH";
		private const string PREFIX_SEARCHITEM = "TS_News_";
		private const string PROFILE_NEWS_ID = "newsID";
		private const string PROFILE_FILTER_NEWS_ID = "TS_Filter_NewsID";
		private const string PROFILE_QUERY_NEWS_ID = "TS_Query_NewsID";
		#endregion

        #region Constant error strings
		private const string error_news_list = "Error listing the news: ";
		private const string error_news_get = "Error getting the news: ";
		private const string error_news_select = "Error selecting a news blog: ";
		private const string error_news_submit = "Error updating a news blog: ";
		private const string error_news_delete = "Error deleting a news blog: ";
		private const string error_news_save = "Error saving the news blog: ";
		//private const string error_news_profile = "Error setting profile for an news: ";
		#endregion

        #region Visible properties
        #endregion

        #region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerNews(CmsX thispage, string blogxID) : base(thispage, blogxID, typeof(CmsXBrokerBlogs), logid) {
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
				case "getblog":
					getblog();
					break;
				case "saveblog":
					saveblog();
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
				UserProfile.Clear(PROFILE_FILTER_NEWS_ID);
			}
			if (clearSearch) {
				xLogger.Debug("list_filter::clearSearch:");
				UserProfile.Clear(PREFIX_SEARCH);
				UserProfile.ClearAll(PREFIX_SEARCHITEM);
			}
			string NewsID = _GetQueryID(PROFILE_QUERY_NEWS_ID);
			xLogger.Debug("list_filter::seriesID:", NewsID);
		}

		/// <summary>List news</summary>
		private void list_admin() {
			list_admin(false);
		}

		/// <summary>List news</summary>
		private void list_admin(bool clearProfile) {
			xLogger.Info("list_admin:");

			xLogger.Debug("list_admin:ok");
		}

		/// <summary>List news</summary>
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
					UserProfile.Clear(PROFILE_FILTER_NEWS_ID);
					UserProfile.Clear(PROFILE_QUERY_NEWS_ID);
					
					// Set search items...
					SearchItem newsid = new SearchItem(PREFIX_SEARCHITEM, "ID", UserProfile, xLogger);
					// Do search...
					//_NewsWS.SearchNews(setting,
					//	,newsid.Type	,newsid.Val
					//);
					if (is_csv) {
						//CsvUtil.GetColumns(_NewsWS.ListItems);
					} else {
						//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_NewsWS.ListXmlRoot, true));
					}
				} else {
					xLogger.Debug("list::search:", setting.Column, ":", setting.Page.ToString(), ":", setting.Rows.ToString());
					if (is_csv) {
						setting.MaxRows = true;
					}
					
					string space = UserProfile.Value(Cms.PROFILE_SITE_SPACE);
					xLogger.Debug("list", "::space:", space);
					// Do list...
					_Blogs.List(space, "default", setting);

					// Output list to csv or content page (for rendering to grid)
					if (is_csv) {
						CmsExport.ExportType = ExportType.XLSX;
						CmsExport.GetColumns(_Blogs.ListItems);
					} else {
						UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Blogs.ListXmlRoot, true));
						xLogger.Debug("list:ok");
					}
				}
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("list::error_news_list:", e.Message);
				throw (new x_exception("error_news_list", String.Concat(error_news_list, e.Message)));
			}
		}
		
		/// <summary>Get a news blog</summary> 
		private void get() {
			try {
				_GetBlogQuery();
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Blogs.ItemXmlRootNode, true));
				xLogger.Debug("get:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_news_get", String.Concat(error_news_get, e.Message)));
			}
		}

		/// <summary>Select a news blog</summary>
		private void select() {
			try {
				xLogger.Info("select:");

				xLogger.Debug("select:ok");
			} catch (x_exception e) {
				throw (new x_exception("error_news_select", String.Concat(error_news_select, e.Code, " - ", e.Message)));
			} catch (Exception e) {
				throw (new x_exception("error_news_select", String.Concat(error_news_select, e.Message)));
			}
		}

		/// <summary>Add/Edit a news blog</summary> 
		private void submit(bool is_new) {
			try {
				xLogger.Debug("submit:is_new:", is_new.ToString());

				string space = "";
				string topic = "default";
				string blogi = "";
				string title = UserProfile.Value("title");
				string blogt = UserProfile.Value("blogtext");
				xLogger.Debug("submit", "::blogt:", blogt);

				if (is_new) {
					space = UserProfile.Value(Cms.PROFILE_SITE_SPACE);
					xLogger.Debug("submit", "::space:", space);
				} else {
					string blogid = UserProfile.Value("id", "0");
					xLogger.Debug("submit", "::blogid:", blogid);
					string [] split = _SplitBlogQuery(blogid);
					space = split[0];
					topic = split[1];
					blogi = split[2];
				}
				string blogh = UserProfile.Value("bloghtml");
				string blogd = UserProfile.Value("blogdate");
				xLogger.Debug("submit", "::blogd:", blogd);

				string [] date = splitDate(blogd);

				if (is_new) {
					_Blogs.AddNewsBlog(space, topic, _User.Fullname, date[0], date[1], date[2], title, "", "");
				} else {
					//_Blogs.UpdateNewsBlog(space, topic, blogi, date[0], date[1], date[2], title, blogt, blogh);
					_Blogs.UpdateNews(space, topic, blogi, date[0], date[1], date[2], title);
				}

				//handleComments("0_record_", space, topic, blogi, is_new);
				 _HandleHistory("0_record_", space, topic, blogi, is_new);

				xLogger.Debug("submit:ok");
			} catch (x_exception e) {
				xLogger.Debug("submit:xerror:", e.Code, "::", e.Message);
				throw e;
			} catch (Exception e) {
				xLogger.Debug("submit:error:", e.Message);
				throw (new x_exception("error_news_submit", String.Concat(error_news_submit, e.Message)));
			} finally {
				UserProfile.Add("news_actions", "");
			}
		}

		/// <summary>List news</summary>
		private void submit() {
			submit(false);
		}

		/// <summary>Delete a news blog</summary> 
		protected override void delete() {
			delete_news();
		}

		/// <summary>Delete a news blog</summary> 
		private void delete_news() {
			try {
				xLogger.Info("delete_news:");

				string blogID = _GetQueryID(PROFILE_BLOG_ID);
				string [] split = _SplitBlogQuery(blogID);
				string blogSpace = split[0];
				string blogTopic = split[1];
				string blogBlog = split[2];
				xLogger.Debug("delete_news", "::blogSpace:", blogSpace, "::blogTopic:", blogTopic, "::blogBlog:", blogBlog);

				_Blogs.DeleteBlog(blogSpace, blogTopic, blogBlog);

				xLogger.Debug("delete_news:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_news_delete", String.Concat(error_news_delete, e.Message)));
			}
		}

		/// <summary>Get a news blog</summary> 
		private void getblog() {
			try {
				xLogger.Info("getblog:");

				string blogID = _GetQueryID(PROFILE_BLOG_ID);
				string [] split = _SplitBlogQuery(blogID);
				string blogSpace = split[0];
				string blogTopic = split[1];
				string blogBlog = split[2];
				xLogger.Debug("getblog", "::blogSpace:", blogSpace, "::blogTopic:", blogTopic, "::blogBlog:", blogBlog);
				UserProfile.Add("blogSpace", blogSpace);
				UserProfile.Add("blogTopic", blogTopic);
				UserProfile.Add("blogBlog", blogBlog);



				_Blogs.GetBlog(blogSpace, blogTopic, blogBlog);
				xLogger.Info("get", "::Item:", _Blogs.Item.OuterXml);

				XmlElement blog = _Blogs.Item.SelectSingleNode("blog") as XmlElement;
				XmlElement text = _Blogs.Item.SelectSingleNode("text") as XmlElement;
				if (blog != null && text != null) {
					string [] date = splitDate(_Blogs.Item.GetAttribute("datecreate"));
					UserProfile.Add("blogYear", date[0]);
					UserProfile.Add("blogMonth", date[1]);
					UserProfile.Add("blogDay", date[2]);
					
					string title = _Blogs.Item.GetAttribute("title");
					string html = blog.InnerText;
					html = UIPage.Server.HtmlDecode(html);

					UserProfile.Add("blogTitle", title);
					UserProfile.Add("blogText", text.InnerText);
					UserProfile.Add("blogHtml", html);
				} else {
					UserProfile.Clear("blogTitle");
					UserProfile.Clear("blogText");
					UserProfile.Clear("blogHtml");
				}
				xLogger.Debug("getblog:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_news_get", String.Concat(error_news_get, e.Message)));
			}
		}

		/// <summary>Get a news blog</summary> 
		private void saveblog() {
			try {
				xLogger.Info("saveblog:");

				string space = UserProfile.Value("blogSpace");
				string topic = UserProfile.Value("blogTopic");
				string blogid = UserProfile.Value("blogBlog");
				string dateyr = UserProfile.Value("blogYear");
				string datemn = UserProfile.Value("blogMonth");
				string datedy = UserProfile.Value("blogDay");
				string title = UserProfile.Value("blogTitle");
				string text	= UserProfile.Value("blogText");
				string html	= UserProfile.Value("blogHtml");

				html = processTinymceHtml(html);
				xLogger.Debug("saveblog: Bloghtml::", html);
				_Blogs.UpdateNewsBlog(space, topic, blogid, dateyr, datemn, datedy, title, text, html);
				xLogger.Debug("saveblog::finished:ok");

				xLogger.Debug("saveblog:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_news_save", String.Concat(error_news_save, e.Message)));
			}
		}
		#endregion

		#region Private utility methods
		private string[] splitDate(string date) {
			string [] datetime = date.Split(new Char[] { 'T' });
			return datetime[0].Split(new Char[] { '-' });
		}

		private string[] splitTime(string date) {
			string [] datetime = date.Split(new Char[] { 'T' });
			return datetime[1].Split(new Char[] { ':' });
		}
		#endregion
	}
}