/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-10-20
	Status:		release	
	Version:	4.0.3
	Build:		20151223
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20101218:	Started from template
	20101227:	Updated contructor
	20130513:	Added _IsDefault as test for DEFAULT_SUBITEM
	20140112:	Refactored constructor
	20151223:	Updated Blog export to xlsx
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {

	using System;
	using System.Xml;

	/// <summary>
    /// Blogs Broker
    /// </summary>
	public class CmsXBrokerBlogs : CmsXBrokerBase {
		#region Invisible properties
		private string default_filter;
        #endregion

        #region Constant name strings
		private const string logid = "CmsXBrokerBlogs.";
		private const string PREFIX_SEARCH = "BLOG_SEARCH";
		private const string PREFIX_SEARCHITEM = "TS_Blog_";
		protected const string PROFILE_BLOG_ID = "blogID";
		protected const string PROFILE_COMMENT_ID = "commentID";
		protected const string PROFILE_HISTORY_ID = "historyID";
		protected const string PROFILE_FILTER_BLOG_ID = "TS_Filter_BlogID";
		protected const string PROFILE_QUERY_BLOG_ID = "TS_Query_BlogID";
		#endregion

        #region Constant error strings
		private const string error_blogs_list = "Error listing the blogs: ";
		private const string error_blogs_get = "Error getting the blog: ";
		private const string error_blogs_select = "Error selecting a blog: ";
		private const string error_blogs_submit = "Error updating a blog: ";
		private const string error_blogs_delete = "Error deleting a blog: ";
		private const string error_blogs_blog = "Error getting / saving space blog: ";
		#endregion

        #region Visible properties
		private string blogConfig;

		private Blogs blogs;
		protected new Blogs _Blogs {
			get {
				if (blogs == null) {
					blogs = (String.IsNullOrEmpty(blogConfig)) ? new Blogs(ProfilePage, _BlogxID) : new Blogs(ProfilePage, _BlogxID, blogConfig);
				}
				return blogs;
			}
		}
		#endregion

        #region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerBlogs(CmsXProfileX thispage, string blogxID) : base(thispage, blogxID, typeof(CmsXBrokerBlogs), logid) {
			initialise(null, UserProfile.Value(Cms.PROFILE_SITE_SPACE));
		}
		public CmsXBrokerBlogs(CmsXProfileX thispage, string blogxID, Type type, string logid) : base(thispage, blogxID, type, logid) {
			initialise(null, UserProfile.Value(Cms.PROFILE_SITE_SPACE));
		}
		/// <summary>Constructor that specifies config service</summary>
		public CmsXBrokerBlogs(CmsXProfileX thispage, string blogxID, string blog_config, string site_space) : base(thispage, blogxID, typeof(CmsXBrokerBlogs), logid) {
			initialise(blog_config, site_space);
		}
		/// <summary>Constructor that specifies config service</summary>
		public CmsXBrokerBlogs(CmsXProfileX thispage, string blogxID, Type type, string logid, string blog_config, string site_space) : base(thispage, blogxID, type, logid) {
			initialise(blog_config, site_space);
		}

		private void initialise(string blog_config, string site_space) {
			blogConfig = blog_config;
			default_filter = String.Format("{0}|0", site_space);
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
				case "filter_dd":
					spacetopics();
					break;
				case "get":
					get();
					break;
				case "get_comments":
					getcomments();
					break;
				case "get_history":
					gethistory();
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
					getblog(false);
					break;
				case "view_history":
					getblog(true);
					break;
				case "view_comment":
					getcomment();
					break;
				case "saveblog":
					saveblog();
					break;
			}
        }
        #endregion

        #region Protected methods
		protected void _GetBlogQuery() {
			getBlogQuery(false);
		}
		protected string[] _SplitBlogQuery(string BlogID) {
			return splitBlogQuery(BlogID);
		}
		protected void _HandleHistory(string profilePrefix, string space, string topic, string blogid, bool is_new) {
			handleHistory(profilePrefix, space, topic, blogid, is_new);
		}
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
				UserProfile.Clear(PROFILE_FILTER_BLOG_ID);
				UserProfile.Add(default_filter);
			}
			if (clearSearch) {
				xLogger.Debug("list_filter::clearSearch:");
				UserProfile.Clear(PREFIX_SEARCH);
				UserProfile.ClearAll(PREFIX_SEARCHITEM);
			}
			string BlogID = _GetQueryID(PROFILE_QUERY_BLOG_ID);
			xLogger.Debug("list_filter::seriesID:", BlogID);
		}

		/// <summary>List blogs</summary>
		private void list_admin() {
			list_admin(false);
		}

		/// <summary>List blogs</summary>
		private void list_admin(bool clearProfile) {
			xLogger.Info("list_admin:");

			xLogger.Debug("list_admin:ok");
		}

		/// <summary>List blogs</summary>
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
					UserProfile.Clear(PROFILE_FILTER_BLOG_ID);
					UserProfile.Clear(PROFILE_QUERY_BLOG_ID);
					
					// Set search items...
					SearchItem blogid = new SearchItem(PREFIX_SEARCHITEM, "ID", UserProfile, xLogger);
					// Do search...
					//_Blogs.SearchBlog(setting,
					//	,blogid.Type	,blogid.Val
					//);
					if (is_csv) {
						//CsvUtil.GetColumns(_Blogs.ListItems);
					} else {
						//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Blogs.ListXmlRoot, true));
					}
				} else {
					xLogger.Debug("list::search:", setting.Column, ":", setting.Page.ToString(), ":", setting.Rows.ToString());
					if (is_csv) {
						setting.MaxRows = true;
					}

					// Get filter(s) and call for list
					string filter = UserProfile.Value(PROFILE_FILTER_BLOG_ID);					// eg string filter
					if (filter == "0" || String.IsNullOrEmpty(filter)) {
						filter = default_filter;
					}
					xLogger.Debug("list", "::filter:", filter);
					string [] split = filter.Split(new Char[] { '|' });
					xLogger.Debug("list", "::space:", split[0], "::topic:", split[1]);

					_Blogs.List(split[0], split[1], setting);

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
				xLogger.Debug("list::error_blogs_list:", e.Message);
				throw (new x_exception("error_blogs_list", String.Concat(error_blogs_list, e.Message)));
			}
		}
		
		/// <summary>Get a blog</summary> 
		private void get() {
			try {
				getBlogQuery();
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Blogs.ItemXmlRootNode, true));
				xLogger.Debug("get:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_blogs_get", String.Concat(error_blogs_get, e.Message)));
			}
		}

		/// <summary>Get a blogs's comments</summary> 
		private void getcomments() {
			try {
				string blogID = _GetQueryID(PROFILE_BLOG_ID);
				xLogger.Info("getBlogQuery::blogID:", blogID);
				string [] split = blogID.Split(new Char[] { '|' });
				xLogger.Debug("getBlogQuery", "::space:", split[0], "::topic:", split[1], "::blogid:", split[2]);

				_Blogs.GetBlogComments(split[0], split[1], split[2]);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Blogs.ItemXmlRootNode, true));
				xLogger.Debug("gettopics:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_blogs_get", String.Concat(error_blogs_get, e.Message)));
			}
		}

		/// <summary>Get a blogs's history</summary> 
		private void gethistory() {
			try {
				string blogID = _GetQueryID(PROFILE_BLOG_ID);
				xLogger.Info("getBlogQuery::blogID:", blogID);
				string [] split = blogID.Split(new Char[] { '|' });
				xLogger.Debug("getBlogQuery", "::space:", split[0], "::topic:", split[1], "::blogid:", split[2]);

				_Blogs.ListBlogHistory(split[0], split[1], split[2]);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Blogs.ItemXmlRootNode, true));
				xLogger.Debug("gettopics:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_blogs_get", String.Concat(error_blogs_get, e.Message)));
			}
		}

		/// <summary>Select a blog</summary>
		private void select() {
			try {
				xLogger.Info("select:");

				xLogger.Debug("select:ok");
			} catch (x_exception e) {
				throw (new x_exception("error_blogs_select", String.Concat(error_blogs_select, e.Code, " - ", e.Message)));
			} catch (Exception e) {
				throw (new x_exception("error_blogs_select", String.Concat(error_blogs_select, e.Message)));
			}
		}

		/// <summary>List blogs</summary>
		private void submit() {
			submit(false);
		}

		/// <summary>Add/Edit a blog</summary> 
		private void submit(bool is_new) {
			try {
				xLogger.Debug("submit:is_new:", is_new.ToString());

				string space = "";
				string topic = "";
				string blogi = "";
				string title = UserProfile.Value("title");
				string blogh = UserProfile.Value("bloghtml");
				xLogger.Debug("submit:blogh:", blogh);

				if (is_new) {
					string filter = UserProfile.Value(PROFILE_FILTER_BLOG_ID);					// eg string filter
					if (filter == "0" || String.IsNullOrEmpty(filter)) {
						filter = default_filter;
					}
					xLogger.Debug("submit", "::filter:", filter);
					
					string [] split = filter.Split(new Char[] { '|' });
					space = split[0];
					topic = split[1];
					xLogger.Debug("submit", "::space:", space, "::topic:", topic, "::author:", _User.Fullname);
					
					_Blogs.AddBlog(space, topic, _User.Fullname, title, blogh);
					xLogger.Debug("submit", "::Item:", _Blogs.ItemXmlRoot.OuterXml);
					XmlElement blog = _Blogs.ItemXmlRoot.SelectSingleNode("//blog") as XmlElement;
					blogi = blog.GetAttribute("id");
				} else {
					string blogid = UserProfile.Value("id", "0");
					xLogger.Debug("submit:blogid:", blogid);
					
					string [] split = splitBlogQuery(blogid);
					space = split[0];
					topic = split[1];
					blogi = split[2];
					xLogger.Debug("submit", "::space:", space, "::topic:", topic, "::blogi:", blogi);
					
					_Blogs.UpdateBlogTitle(space, topic, blogi, title);
				}

				handleComments("0_record_", space, topic, blogi, is_new);
				handleHistory("1_record_", space, topic, blogi, is_new);

				xLogger.Debug("submit:ok");
			} catch (x_exception e) {
				xLogger.Debug("submit:xerror:", e.Code, "::", e.Message);
				throw e;
			} catch (Exception e) {
				xLogger.Debug("submit:error:", e.Message);
				throw (new x_exception("error_blogs_submit", String.Concat(error_blogs_submit, e.Message)));
			} finally {
				UserProfile.Add("blog_actions", "");
			}
		}

		/// <summary>Add, update, remove comments</summary>
		private void handleComments(string profilePrefix, string space, string topic, string blogid, bool is_new) {
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string blogCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);

				if (!String.IsNullOrEmpty(blogCSV)) {
					string[] blog = blogCSV.Split(new char[] { '|' });
					string this_id = blog[0];
					xLogger.Debug("handleComments::", prof, ":", blogCSV);
					// ie upd: handleComments::0_record_0:blogs|alanblog|3|1|blogs|alanblog|3|1|1|no|iBurst|2008-02-28T16:04:41|Alan's Letter|Test|View Comment|true|
					// ie add: handleComments::0_record_0:[...]| | |yes| | |Test|Test comment| |false|
					xLogger.Debug("handleComments", "::count:", blog.Length.ToString(), "::this_id:", this_id);

					if (this_id == "")
						break;

					if (_IsDefault(this_id)) {
						string isok = blog[3];
						string titl = blog[6];
						string text = blog[7];
						xLogger.Debug("handleComments::add", "::isok:", isok, "::titl:", titl, "::text:", text);
						_Blogs.AddBlogComment(space, topic, blogid, _User.Username, _User.Fullname, titl, text, (isok == "yes"));
					} else {
						string comm = blog[3];
						string isok = blog[9];
						string auth = blog[10];
						string titl = blog[12];
						string text = blog[13];
						string dele = blog[15];
						xLogger.Debug("handleComments", "::comm:", comm, "::isok:", isok, "::auth:", auth, "::titl:", titl, "::text:", text, "::dele:", dele);
						if (dele == "true" && !is_new) {
							xLogger.Debug("handleComments::delete:");
							_Blogs.DeleteBlogComment(space, topic, blogid, comm);
						} else {
							xLogger.Debug("handleComments::update");
							_Blogs.UpdateBlogComment(space, topic, blogid, comm, titl, text, (isok == "yes"));
						}
					}
				}
			}
		}

		/// <summary>Restore history item</summary>
		private void handleHistory(string profilePrefix, string space, string topic, string blogid, bool is_new) {
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string blogCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);

				if (!String.IsNullOrEmpty(blogCSV)) {
					string[] blog = blogCSV.Split(new char[] { '|' });
					xLogger.Debug("handleHistory::", prof, ":", blogCSV);

					string restore = blog[12];
					string remove = blog[13];
					string histi = blog[7];
					xLogger.Debug("handleHistory", "::histi:", histi, "::restore:", restore, "::remove:", remove);
					if (remove == "true") {
						xLogger.Debug("handleHistory:remove");
						_Blogs.DeleteBlogHistory(space, topic, blogid, histi);
					} else if (!String.IsNullOrEmpty(restore)) {
						if (Boolean.Parse(restore)) {
							xLogger.Debug("handleHistory:restore");
							_Blogs.RestoreBlogHistory(space, topic, blogid, histi);
							break;
						}
					}
				}
			}
		}

		/// <summary>Delete a blog</summary> 
		protected override void delete() {
			delete_blog();
		}

		/// <summary>Delete a blog</summary> 
		private void delete_blog() {
			try {
				xLogger.Info("delete_blog:");

				string blogID = _GetQueryID(PROFILE_BLOG_ID);
				string [] split = splitBlogQuery(blogID);
				string blogSpace = split[0];
				string blogTopic = split[1];
				string blogBlog = split[2];
				xLogger.Debug("delete_blog", "::blogSpace:", blogSpace, "::blogTopic:", blogTopic, "::blogBlog:", blogBlog);

				_Blogs.DeleteBlog(blogSpace, blogTopic, blogBlog);

				xLogger.Debug("delete_blog:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_blogs_delete", String.Concat(error_blogs_delete, e.Message)));
			}
		}

		/// <summary>Get a blog's blogs</summary> 
		private void getblog(bool isHistory) {
			try {
				getBlogQuery(isHistory);
				xLogger.Info("get", "::Item:", _Blogs.Item.OuterXml);
				
				XmlElement blog = _Blogs.Item.SelectSingleNode("blog") as XmlElement;
				if (blog != null) {
					//string title = blog.GetAttribute("title");
					string title = _Blogs.Item.GetAttribute("title");
					string html = blog.InnerText;
					html = UIPage.Server.HtmlDecode(html);

					UserProfile.Add("blogTitle", title);
					UserProfile.Add("blogHtml", html);
				} else {
					UserProfile.Clear("blogTitle");
					UserProfile.Clear("blogHtml");
				}

				xLogger.Debug("getblog:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_blogs_blog", String.Concat(error_blogs_blog, e.Message)));
			}
		}

		/// <summary>Save a blog's blog content</summary> 
		private void saveblog() {
			try {
				xLogger.Info("saveblog:");
				string space = UserProfile.Value("blogSpace");
				string topic = UserProfile.Value("blogTopic");
				string blogid = UserProfile.Value("blogBlog");
				string title = UserProfile.Value("blogTitle");
				string html	= UserProfile.Value("blogHtml");

				html = processTinymceHtml(html);
				xLogger.Debug("saveblog: Bloghtml::", html);
				_Blogs.UpdateBlog(space, topic, blogid, title, html);
				xLogger.Debug("saveblog::finished:ok");

				xLogger.Debug("saveblog:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_blogs_blog", String.Concat(error_blogs_blog, e.Message)));
			}
		}

		/// <summary>Get a blog comment</summary> 
		private void getcomment() {
			try {
				string blogID = _GetQueryID(PROFILE_COMMENT_ID);
				xLogger.Info("getcomment::blogID:", blogID);
				string [] split = blogID.Split(new Char[] { '|' });
				xLogger.Debug("getcomment", "::space:", split[0], "::topic:", split[1], "::blogid:", split[2], "::commentid:", split[3]);

				_Blogs.GetBlogComment(split[0], split[1], split[2], split[3]);
				xLogger.Info("getcomment", "::RootNode:", _Blogs.ItemXmlRootNode.OuterXml);

				XmlElement comment = _Blogs.ItemXmlRoot.SelectSingleNode("//comment") as XmlElement;
				if (comment != null) {
					string text = comment.SelectSingleNode("text").InnerText;
					UIPage.Content.SelectSingleNode("//page/comment").InnerXml = comment.SelectSingleNode("text").InnerXml;
				}

				xLogger.Debug("getcomment:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_blogs_blog", String.Concat(error_blogs_blog, e.Message)));
			}
		}
		#endregion

		#region Private link dd methods
		/// <summary>Get list of spacetopics</summary>
		private void spacetopics() {
			try {
				xLogger.Info("spacetopics:");

				_Spaces.ListSpacesTopics();

				xLogger.Debug("spacetopics:", ":_Spaces.ListXmlRoot:", _Spaces.ListXmlRoot.OuterXml);
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Spaces.ListXmlRoot, true));
				xLogger.Debug("spacetopics:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("spacetopics::error_blogs_list:", e.Message);
				throw (new x_exception("error_blogs_list", String.Concat(error_blogs_list, e.Message)));
			}
		}
		#endregion

		#region Private utility methods
		private void getBlogQuery() {
			getBlogQuery(false);
		}
		private string[] splitBlogQuery(string blogID) {
			xLogger.Info("getBlogQuery::blogID:", blogID);
			string [] split = blogID.Split(new Char[] { '|' });
			xLogger.Debug("getBlogQuery", "::space:", split[0], "::topic:", split[1], "::blogid:", split[2]);
			return split;
		}
		private void getBlogQuery(bool isHistory) {
			string profileID = (isHistory) ? PROFILE_HISTORY_ID : PROFILE_BLOG_ID;
			string blogID = _GetQueryID(profileID);
			string [] split = splitBlogQuery(blogID);
			UserProfile.Add("blogSpace", split[0]);
			UserProfile.Add("blogTopic", split[1]);
			UserProfile.Add("blogBlog", split[2]);

			if (isHistory) {
				UserProfile.Add("blogHistory", split[3]);
				xLogger.Debug("getBlogQuery", "::histid:", split[3]);
				_Blogs.GetBlogHistory(split[0], split[1], split[2], split[3]);
			} else {
				_Blogs.GetBlog(split[0], split[1], split[2]);
			}
			UserProfile.Add(profileID, blogID);
		}
		
		#endregion
	}
}