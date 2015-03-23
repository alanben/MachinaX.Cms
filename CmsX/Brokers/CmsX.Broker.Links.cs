using System;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-10-20
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20101020:	Starting point
	20101101:	Update to handleBlogs
	20101218:	Updated contructor
	20101227:	Updated contructor
	20110429:	Added Link links
	20130418:	Added Process override to manage authentication
				(needed for Loeries as it does not use the same passport web service)
	20130513:	Added _IsDefault as test for DEFAULT_SUBITEM
	20140112:	Refactored constructor
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
    /// <summary>
    /// Description of the class.
    /// <para>Additional information about the class</para>
    /// </summary>
	public class CmsXBrokerLinks : CmsXBrokerBase {
		#region Invisible properties
        #endregion

        #region Constant name strings
		private const string logid = "CmsXBrokerLinks.";
		private const string PREFIX_SEARCH = "LINKS_SEARCH";
		private const string PREFIX_SEARCHITEM = "TS_Links_";
		private const string PROFILE_LINKS_ID = "linkID";
		private const string PROFILE_BLOG_ID = "blogID";
		private const string PROFILE_PAGE_ID = "pageID";
		private const string PROFILE_FILTER_LINKS_ID = "TS_Filter_Group";
		private const string PROFILE_QUERY_LINKS_ID = "TS_Query_LinksID";
		private const string PROFILE_LINK_FLAGS_ID = "0_record_";
		private const string PROFILE_LINK_BLOGS_ID = "1_record_";
		private const string PROFILE_LINK_PAGES_ID = "2_record_";
		private const string PROFILE_LINK_LINKS_ID = "3_record_";
		// NB these also set in page xml
		protected const string DEFAULT_POSITION = "Select position...";
		protected const string DEFAULT_TITLE = "Enter title...";
		protected const string DEFAULT_TITLEFLAG = "Select title flag...";
		protected const string DEFAULT_LINKFILE = "Select existing xml file...";
		#endregion

        #region Constant error strings
		private const string error_links_list = "Error listing the links: ";
		private const string error_link_get = "Error getting the link: ";
		private const string error_link_select = "Error selecting a link: ";
		private const string error_link_submit = "Error updating a link: ";
		private const string error_link_delete = "Error deleting a link: ";
		private const string error_link_blog = "Error getting / saving link blog: ";
		private const string error_link_page = "Error getting / saving link page: ";
		//private const string error_link_profile = "Error setting profile for an link: ";
		#endregion

        #region Visible properties
        #endregion

        #region Constructors/Destructors
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		/// <param name="blogxID">The MachinaX website identifier</param>
		public CmsXBrokerLinks(CmsX thispage, string blogxID) : base(thispage, blogxID, typeof(CmsXBrokerLinks), logid) {
			xLogger.Info("CmsXBrokerLinks:ok");
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
				case "groups_dd":
					groups();
					break;
				case "flags_dd":
					flags();
					break;
				case "pages_dd":
					pages();
					break;
				case "get":
					get();
					break;
				case "getflags":
					getflags();
					break;
				case "getblogs":
					getblogs();
					break;
				case "getlinks":
					getlinks();
					break;
				case "getpages":
					getpages();
					break;
				case "getblog":
					getblog();
					break;
				case "saveblog":
					saveblog();
					break;
				case "getpage":
					getpage();
					break;
				case "savepage":
					savepage();
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

		#region Private links methods
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
				UserProfile.Clear(PROFILE_FILTER_LINKS_ID);
			}
			if (clearSearch) {
				xLogger.Debug("list_filter::clearSearch:");
				UserProfile.Clear(PREFIX_SEARCH);
				UserProfile.ClearAll(PREFIX_SEARCHITEM);
			}
			string LinksID = _GetQueryID(PROFILE_QUERY_LINKS_ID);
			xLogger.Debug("list_filter::seriesID:", LinksID);
		}

		/// <summary>List links</summary>
		private void list_admin() {
			list_admin(false);
		}

		/// <summary>List links</summary>
		private void list_admin(bool clearProfile) {
			try {
				xLogger.Info("list_admin:");
				_Links.List();
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Links.ListXmlRoot, true));
				xLogger.Debug("list_admin:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("list_admin::error_links_list:", e.Message);
				throw (new x_exception("error_links_list", String.Concat(error_links_list, e.Message)));
			}
		}

		/// <summary>List links</summary>
		private void list() {
			xLogger.Info("list:");
			list(false);
		}

		/// <summary>Get list of series</summary>
		private void list(bool is_csv) {
			try {
				xLogger.Info("list:");
				//SearchSettings setting = new SearchSettings(UserProfile, xLogger, is_csv);
				CmsSearch setting = new CmsSearch(ProfilePage, xLogger, is_csv);

				UserProfile.Add(PREFIX_SEARCH, (_IsSearchSet(PREFIX_SEARCHITEM, true)) ? "no" : "yes");
				xLogger.Debug("list::search:", UserProfile.Value(PREFIX_SEARCH));
				
				if (UserProfile.Value(PREFIX_SEARCH, "no") == "yes") {
					UserProfile.Clear(PROFILE_FILTER_LINKS_ID);
					UserProfile.Clear(PROFILE_QUERY_LINKS_ID);
					
					// Set search items...
					SearchItem linkgroup = new SearchItem(PREFIX_SEARCHITEM, "Group", UserProfile, xLogger);
					SearchItem linkname = new SearchItem(PREFIX_SEARCHITEM, "Name", UserProfile, xLogger);
					SearchItem linkprompt = new SearchItem(PREFIX_SEARCHITEM, "Prompt", UserProfile, xLogger);
					// Do search...
					_Links.Search(setting
						,linkgroup.Type		,linkgroup.Val
						,linkname.Type		,linkname.Val
						,linkprompt.Type	,linkprompt.Val
					);
					if (is_csv) {
						CsvUtil.GetColumns(_Links.ListItems);
					} else {
						UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Links.ListXmlRoot, true));
					}
				} else {
					xLogger.Debug("list::params:", setting.Column, ":", setting.Page.ToString(), ":", setting.Rows.ToString());
					if (is_csv) {
						setting.MaxRows = true;
					}

					// Get filter(s) and call for list
					string filter = UserProfile.Value(PROFILE_FILTER_LINKS_ID);
					xLogger.Debug("list::filter:", filter);

					_Links.List(filter, setting);
					
					// Output list to csv or content page (for rendering to grid)
					if (is_csv) {
						CsvUtil.GetColumns(_Links.ListItems);
					} else {
						UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Links.ListXmlRoot, true));
						xLogger.Debug("list:ok");
					}
				}
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("list::error_links_list:", e.Message);
				throw (new x_exception("error_links_list", String.Concat(error_links_list, e.Message)));
			}
		}
		
		/// <summary>Get a link</summary> 
		private void get() {
			try {
				string linkID = _GetQueryID(PROFILE_LINKS_ID);
				xLogger.Info("get::linkID:", linkID);

				_Links.GetLink(Int32.Parse(linkID));
				UserProfile.Add(PROFILE_LINKS_ID, linkID);
				xLogger.Info("get::linkxml:", _Links.ItemXmlRootNode.OuterXml);
				xLogger.Info("get::item:", _Links.Item.OuterXml);
				string space = String.IsNullOrEmpty(_Links.BlogSpace)? UserProfile.Value(Cms.PROFILE_SITE_SPACE) : _Links.BlogSpace;
				UserProfile.Add(Cms.PROFILE_TOPICS_BLOGSPACE, space);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Links.ItemXmlRootNode, true));
				xLogger.Debug("get:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_link_get", String.Concat(error_link_get, e.Message)));
			}
		}

		/// <summary>Get a link's flags</summary> 
		private void getflags() {
			try {
				string linkID = _GetQueryID(PROFILE_LINKS_ID);
				xLogger.Debug("getflags::linkID:", linkID);
				_Links.GetLinkFlags(Int32.Parse(linkID));

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Links.ItemXmlRootNode, true));
				xLogger.Debug("getflags:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_link_get", String.Concat(error_link_get, e.Message)));
			}
		}

		/// <summary>Get a link's blogs</summary> 
		private void getblogs() {
			try {
				string linkID = _GetQueryID(PROFILE_LINKS_ID);
				xLogger.Debug("getblogs::linkID:", linkID);
				_Links.GetLinkBlogs(Int32.Parse(linkID));

				foreach (XmlNode itm in _Links.ItemXmlRootNode.SelectNodes("//blogs/item")) {
					XmlElement item = itm as XmlElement;
					string space = item.GetAttribute("blog_space");
					string topic = item.GetAttribute("blog_topic");
					string blog = item.GetAttribute("blog_id");
					if (String.IsNullOrEmpty(blog) || blog == "all" || blog == "*" || blog == "querystring") {
						item.SetAttribute("blog_title", "-");
						item.SetAttribute("blog_click", "-");
					} else {
						XmlNode blg = _AdminX.GetBlog(space, topic, blog);
						item.SetAttribute("blog_title", blg.SelectSingleNode("//blog/title").InnerText);
						item.SetAttribute("blog_click", "Edit content");
					}
				}

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Links.ItemXmlRootNode, true));
				xLogger.Debug("getblogs:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_link_get", String.Concat(error_link_get, e.Message)));
			}
		}

		/// <summary>Get a link's links</summary> 
		private void getlinks() {
			try {
				string linkID = _GetQueryID(PROFILE_LINKS_ID);
				xLogger.Debug("getlinks::linkID:", linkID);
				_Links.GetLinkLinks(Int32.Parse(linkID));

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Links.ItemXmlRootNode, true));
				xLogger.Debug("getlinks:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_link_get", String.Concat(error_link_get, e.Message)));
			}
		}

		/// <summary>Get a link's pages</summary> 
		private void getpages() {
			try {
				string linkID = _GetQueryID(PROFILE_LINKS_ID);
				xLogger.Debug("getpages::linkID:", linkID);
				_Links.GetLinkFiles(Int32.Parse(linkID));

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Links.ItemXmlRootNode, true));
				xLogger.Debug("getpages:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_link_get", String.Concat(error_link_get, e.Message)));
			}
		}

		/// <summary>Get a link's blogs</summary> 
		private void getblog() {
			try {
				string linkID = UserProfile.Value(PROFILE_LINKS_ID);
				string blogID = _GetQueryID(PROFILE_BLOG_ID);
				xLogger.Debug("getblog::blogID:", blogID, "::linkID:", linkID);
				_Links.GetLinkBlogs(Int32.Parse(linkID));

				XmlElement item = _Links.ItemXmlRootNode.SelectSingleNode(String.Format("//blogs/item[@id='{0}']", blogID)) as XmlElement;
				if (item != null) {
					string space = item.GetAttribute("blog_space");
					string topic = item.GetAttribute("blog_topic");
					string blog = item.GetAttribute("blog_id");

					XmlNode blg = _AdminX.GetBlog(space, topic, blog);
					string title = blg.SelectSingleNode("//blog/title").InnerText;
					string html = blg.SelectSingleNode("//blog/bloghtml").InnerText;
					html = UIPage.Server.HtmlDecode(html);

					UserProfile.Add("blogSpace", space);
					UserProfile.Add("blogTopic", topic);
					UserProfile.Add("blogBlog", blog);
					UserProfile.Add("blogTitle", title);
					UserProfile.Add("blogHtml", html);
				}

				xLogger.Debug("getblog:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_link_blog", String.Concat(error_link_blog, e.Message)));
			}
		}

		/// <summary>Save a link's blog content</summary> 
		private void saveblog() {
			try {
				xLogger.Info("saveblog:");
				string space	= UserProfile.Value("blogSpace");
				string topic	= UserProfile.Value("blogTopic");
				string blog		= UserProfile.Value("blogBlog");
				string title	= UserProfile.Value("blogTitle");
				string bloghtml	= UserProfile.Value("blogHtml");

				bloghtml = processTinymceHtml(bloghtml);
				xLogger.Debug("saveblog: Bloghtml::", bloghtml);
				_AdminX.EditBlogHtml(space, topic, blog, title, bloghtml);
				xLogger.Debug("saveblog::finished:ok");

				xLogger.Debug("saveblog:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_link_blog", String.Concat(error_link_blog, e.Message)));
			}
		}

		/// <summary>Get a link's page</summary> 
		private void getpage() {
			try {
				string linkID = UserProfile.Value(PROFILE_LINKS_ID);
				string pageID = _GetQueryID(PROFILE_PAGE_ID);
				xLogger.Debug("getpage::pageID:", pageID, "::linkID:", linkID);
				int linkid = Int32.Parse(linkID);
				_Links.GetLinkFiles(linkid);

				XmlElement item = _Links.ItemXmlRootNode.SelectSingleNode(String.Format("//pages/item[@id='{0}']", pageID)) as XmlElement;
				if (item != null) {
					string pgid = item.GetAttribute("page_id");
					string sect = item.GetAttribute("page_sect");
					string subs = item.GetAttribute("page_sub");
					string proc = item.GetAttribute("page_proc");
					string file = item.GetAttribute("page_file");

					_Links.GetLinkFileXml(linkid, pgid);
					XmlElement content = _Links.ItemXmlRootNode.SelectSingleNode("//content") as XmlElement;
					//string xml = UIPage.Server.HtmlEncode(content.OuterXml);
					string xml = content.OuterXml;
					xLogger.Debug("getpage", "::xml:", xml);

					UserProfile.Add("pageID",	pgid);
					//UserProfile.Add("pageSect", sect);
					//UserProfile.Add("pageSub",	subs);
					//UserProfile.Add("pageProc",	proc);
					//UserProfile.Add("pageFile",	file);
					UserProfile.Add("pageXml",	xml);
				}

				xLogger.Debug("getpage:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_link_page", String.Concat(error_link_page, e.Message)));
			}
		}

		/// <summary>Save a link's page content</summary> 
		private void savepage() {
			try {
				xLogger.Info("savepage:");
				string linkID = UserProfile.Value(PROFILE_LINKS_ID);
				int linkid = Int32.Parse(linkID);
				string pgid	= UserProfile.Value("pageID");
				xLogger.Debug("savepage", "::pgid:", pgid, "::linkID:", linkID);

				//string sect	= UserProfile.Value("pageSect");
				//string subs	= UserProfile.Value("pageSub");
				//string proc	= UserProfile.Value("pageProc");
				//string file	= UserProfile.Value("pageFile");
				string xml	= UserProfile.Value("pageXml");

				//xml = UIPage.Server.HtmlDecode(xml);
				xLogger.Debug("savepage", "::xml:", xml);

				_Links.SaveLinkFileXml(linkid, pgid, xml);
				xLogger.Debug("savepage::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_link_page", String.Concat(error_link_page, e.Message)));
			}
		}

		/// <summary>Select a link</summary>
		private void select() {
			try {
				xLogger.Info("select:");

				xLogger.Debug("select:ok");
			} catch (x_exception e) {
				throw (new x_exception("error_link_select", String.Concat(error_link_select, e.Code, " - ", e.Message)));
			} catch (Exception e) {
				throw (new x_exception("error_link_select", String.Concat(error_link_select, e.Message)));
			}
		}

		/// <summary>Add/Edit a link</summary> 
		private void submit(bool is_new) {
			xLogger.Debug("submit:is_new:", is_new.ToString());
			try {
				string linkid = UserProfile.Value("id", "0");
				linkid = (String.IsNullOrEmpty(linkid)) ? "0" : linkid;
				xLogger.Debug("submit:linkid:", linkid);

				string name = UserProfile.Value("name");
				string link = UserProfile.Value("link");
				link = (link == "0") ? "" : link;
				string prompt = UserProfile.Value("prompt");
				string group = UserProfile.Value("group");
				string broker = UserProfile.Value("broker");
				string display = UserProfile.Value("display");
				string track = UserProfile.Value("track");
				string type = UserProfile.Value("type");
				string tags = UserProfile.Value("tags");
				string secure = UserProfile.Value("secure");
				string url = UserProfile.Value("url");
				//string space = UserProfile.Value("blogspace");
				string space = UserProfile.Value(Cms.PROFILE_TOPICS_BLOGSPACE, UserProfile.Value(Cms.PROFILE_SITE_SPACE));

				int link_id = Int32.Parse(linkid);
				if (is_new) {
					xLogger.Debug("submit", "::name:", name, "::link:", link, "::prompt:", prompt, "::broker:", broker, "::group:", group);
					xLogger.Debug("submit", "::display:", display, "::secure:", secure, "::type:", type, "::tags:", tags, "::track:", track, "::url:", url);

					_Links.AddLink(name, link, prompt, broker, group, display, secure, type, tags, track, url);
					linkid = _Links.ItemXml.SelectSingleNode("//link/@id").InnerText;
					xLogger.Debug("submit:linkid:", linkid);
					link_id = Int32.Parse(linkid);
				} else {
					_Links.UpdateLink(link_id, name, link, prompt, broker, group, display, secure, type, tags, track, url);
				}
				xLogger.Debug("submit:link_id:", link_id.ToString());
				UserProfile.Add(PROFILE_LINKS_ID, link_id.ToString());

				xLogger.Debug("submit:ItemXml:", _Links.ItemXml.OuterXml);
				handleSEO(is_new, link_id);
				handlePage(is_new, link_id);
				handleBlogs(PROFILE_LINK_BLOGS_ID, link_id, space, is_new);
				handleFlags(PROFILE_LINK_FLAGS_ID, link_id, is_new);
				handlePages(PROFILE_LINK_PAGES_ID, link_id, is_new);
				handleLinks(PROFILE_LINK_LINKS_ID, link_id, is_new);

				xLogger.Debug("submit:ok");
			} catch (x_exception e) {
				xLogger.Debug("submit:xerror:", e.Code, "::", e.Message);
				throw e;
			} catch (Exception e) {
				xLogger.Debug("submit:error:", e.Message);
				throw (new x_exception("error_link_submit", String.Concat(error_link_submit, e.Message)));
			} finally {
				UserProfile.Add("link_actions", "");
			}
		}

		/// <summary>List links</summary>
		private void submit() {
			submit(false);
		}

		/// <summary>Add/Edit a link page</summary> 
		private void handlePage(bool is_new, int link_id) {
			xLogger.Debug("handlePage:is_new:", is_new.ToString());

			string sect = UserProfile.Value("sect");
			string sub = UserProfile.Value("sub");
			string proc = UserProfile.Value("proc");
			string file = UserProfile.Value("file");
			
			if (is_new) {
				_Links.AddLinkPage(link_id, sect, sub, proc, file);
			} else {
				_Links.UpdateLinkPage(link_id, sect, sub, proc, file);
			}
		}

		/// <summary>Add/Edit a link seo</summary> 
		private void handleSEO(bool is_new, int link_id) {
			xLogger.Debug("handleSEO", "::is_new:", is_new.ToString());

			string title = UserProfile.Value("title");
			string desc = UserProfile.Value("desc");
			string keywords = UserProfile.Value("keywords");

			if (is_new) {
				_Links.AddLinkSEO(link_id, title, desc, keywords);
			} else {
				_Links.UpdateLinkSEO(link_id, title, desc, keywords);
			}
		}

		/// <summary>Add, update, remove blogs</summary>
		private void handleBlogs(string profilePrefix, int link_id, string blogspace, bool is_new) {
			xLogger.Debug("handleBlogs::blogspace:", blogspace);
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string blogsCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);

				if (!String.IsNullOrEmpty(blogsCSV)) {
					string[] blogs = blogsCSV.Split(new char[] { '|' });
					xLogger.Debug("handleBlogs:", prof, ":", blogsCSV);
					// ie handleBlogs:0_record_1:2|2|main|2|The internet just got a whole lot easier with iBurst Prepaid Internet|Edit content|||
					
					string thisblogs_id = blogs[0];
					if (thisblogs_id == "")
						break;
					else {
						string this_id = blogs[1];
						string this_topic = blogs[2];
						string this_blog = blogs[3];
						string this_title = (blogs[4] == DEFAULT_TITLE) ? "" : blogs[4];
						string this_button = blogs[5];
						string this_position = (blogs[6] == DEFAULT_POSITION) ? "" : blogs[6];
						string this_titleflag = (blogs[7] == DEFAULT_TITLEFLAG) ? "" : blogs[7];
						string this_remove = blogs[8];
						xLogger.Debug("handleBlogs::thisblogs_id:", thisblogs_id, "::this_id:", this_id, "::this_remove:", this_remove, "::this_title:", this_title, "::this_button:", this_button);
						xLogger.Debug("handleBlogs::this_topic:", this_topic, "::this_blog:", this_blog, "::this_position:", this_position, "::this_titleflag:", this_titleflag);

						if (this_remove == "true") {
							if (!(_IsDefault(thisblogs_id) || is_new)) {
								xLogger.Debug("handleBlogs::delete:");
								_Links.DeleteLinkBlog(link_id, blogspace, this_topic, this_blog);
							}
						} else if (_IsDefault(this_topic)) {
							// ignore this
						} else {
							bool ismulti = (String.IsNullOrEmpty(this_blog) || this_blog == "all" || this_blog == "*" || this_blog == "querystring");
							if (_IsDefault(thisblogs_id) || is_new) {
								xLogger.Debug("handleBlogs::add.this_blog:", this_blog);
								if (!ismulti) {
									this_blog = addBlog(blogspace, this_topic, this_blog, this_title);
								}
								_Links.AddLinkBlog(link_id, blogspace, this_topic, this_blog, this_position, this_titleflag);
								//_AdminX.AddBlogHtml();
							} else {
								xLogger.Debug("handleBlogs::update:");
								_Links.UpdateLinkBlog(link_id, blogspace, this_topic, this_blog, this_position, this_titleflag);
								if (!ismulti) {
									_AdminX.EditBlogTitle(blogspace, this_topic, this_blog, this_title);
								}
							}
						}
					}
					
				}
			}
		}

		/// <summary>Add, update, remove flags</summary>
		private void handleFlags(string profilePrefix, int link_id, bool is_new) {
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string flagsCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);

				if (!String.IsNullOrEmpty(flagsCSV)) {
					string[] flags = flagsCSV.Split(new char[] { '|' });
					xLogger.Debug("handleFlags:", prof, ":", flagsCSV);
					// ie handleFlags:1_record_3:4|4|testimonial|yes||; link_id:5
					
					string thisflags_id = flags[0];
					if (thisflags_id == "")
						break;
					else {
						string this_id = flags[1];
						string this_name = flags[2];
						string this_value = flags[3];
						string this_remove = flags[4];
						xLogger.Debug("handleFlags::thisflags_id:", thisflags_id, "::this_id:", this_id, "::this_name:", this_name, "::this_value:", this_value, "::this_remove:", this_remove);
						
						if (this_remove == "true") {
							if (!(_IsDefault(thisflags_id) || is_new)) {
								xLogger.Debug("handleFlags::delete:");
								_Links.DeleteLinkFlag(link_id, this_name);
							}
						} else if (_IsDefault(this_name) || _IsDefault(this_value)) {
							// ignore this
						} else {
							if (_IsDefault(thisflags_id) || is_new) {
								xLogger.Debug("handleFlags::add:");
								_Links.AddLinkFlag(link_id, this_name, this_value);
							} else {
								xLogger.Debug("handleFlags::update:");
								_Links.UpdateLinkFlag(link_id, this_name, this_value);
							}
						}
					}
				}
			}
		}

		/// <summary>Add, update, remove links</summary>
		private void handleLinks(string profilePrefix, int link_id, bool is_new) {
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string linksCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);

				if (!String.IsNullOrEmpty(linksCSV)) {
					string[] links = linksCSV.Split(new char[] { '|' });
					xLogger.Debug("handleLinks:", prof, ":", linksCSV);
					// ie handleLinks:1_record_3:4|4|testimonial|yes||; link_id:5

					string thislinks_id = links[0];
					if (thislinks_id == "")
						break;
					else {
						string this_id = links[1];
						string this_name = links[2];
						string this_value = links[3];
						string this_remove = links[4];
						xLogger.Debug("handleLinks::thislinks_id:", thislinks_id, "::this_id:", this_id, "::this_name:", this_name, "::this_value:", this_value, "::this_remove:", this_remove);

						if (this_remove == "true") {
							if (!(_IsDefault(thislinks_id) || is_new)) {
								xLogger.Debug("handleLinks::delete:");
								_Links.DeleteLinkLink(link_id, this_name);
							}
						} else if (_IsDefault(this_name) || _IsDefault(this_value)) {
							// ignore this
						} else {
							if (_IsDefault(thislinks_id) || is_new) {
								xLogger.Debug("handleLinks::add:");
								_Links.AddLinkLink(link_id, this_name, this_value);
							} else {
								xLogger.Debug("handleLinks::update:");
								_Links.UpdateLinkLink(link_id, this_name, this_value);
							}
						}
					}
				}
			}
		}

		/// <summary>Add, update, remove link files (pages)</summary>
		private void handlePages(string profilePrefix, int link_id, bool is_new) {
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string linksCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);

				if (!String.IsNullOrEmpty(linksCSV)) {
					string[] links = linksCSV.Split(new char[] { '|' });
					xLogger.Debug("handlePages:", prof, ":", linksCSV);
					// Eg:
					// handlePages:2_record_0:[...]|[...]|Select existing xml file...|Section|SubSection|Process|Filename|false|
					// handlePages:2_record_1:[...]|[...]|///next.xml|[...]|[...]|[...]|[...]|false|
					// handlePages:2_record_2:1|1|services/data//login|services|data||login||


					string thislinks_id = links[0];
					if (thislinks_id == "")
						break;
					else {
						string this_id = links[1];
						string this_flid = links[2];
						string this_sect = links[3];
						string this_subs = links[4];
						string this_proc = links[5];
						string this_file = links[6];
						string this_edit = links[7];
						string this_posn = (links[8] == DEFAULT_POSITION) ? "" : links[8];
						string this_remove = links[9];
						xLogger.Debug("handlePages::thislinks_id:", thislinks_id, "::this_id:", thislinks_id, "::this_flid:", this_flid, "::this_remove:", this_remove);
						xLogger.Debug("handlePages::this_sect:", this_sect, "::this_subs:", this_subs, "::this_proc:", this_proc, "::this_file:", this_file, "::this_posn:", this_posn);

						if (this_remove == "true") {
							if (!(_IsDefault(thislinks_id) || is_new)) {
								xLogger.Debug("handlePages::delete:");
								_Links.DeleteLinkFile(link_id, this_flid);
							}
							//} else if (_IsDefault(this_name) || _IsDefault(this_value)) {
							// ignore this
						} else {
							if (_IsDefault(thislinks_id) || is_new) {
								xLogger.Debug("handlePages::add:");
								if (this_flid != DEFAULT_LINKFILE) {	// ie an existing selected
									_Links.AddLinkFile(link_id, this_flid, this_posn);
								} else {
									_Links.AddLinkFile(link_id, (_IsDefault(this_sect)) ? "" : this_sect, (_IsDefault(this_subs)) ? "" : this_subs, (_IsDefault(this_proc)) ? "" : this_proc, (_IsDefault(this_file)) ? "" : this_file, this_posn);
								}
							} else {
								xLogger.Debug("handlePages::update:");
								_Links.UpdateLinkFile(link_id, this_flid, this_sect, this_subs, this_proc, this_file, this_posn);
							}
						}
					}
				}
			}
		}

		/// <summary>Delete a link</summary> 
		protected override void delete() {
			delete_link();
		}

		/// <summary>Delete a link</summary> 
		private void delete_link() {
			try {
				xLogger.Info("delete_link:");

				string linkid =  UIPage.QueryParam("id", "0");
				xLogger.Debug("delete_link:linkid:", linkid);
				int link_id = Int32.Parse(linkid);
				
				_Links.DeleteLink(link_id);
				
				xLogger.Debug("delete_link:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_link_delete", String.Concat(error_link_delete, e.Message)));
			}
		}
		#endregion

		#region Private link dd methods
		/// <summary>Get list of groups</summary>
		private void groups() {
			try {
				xLogger.Info("groups:");
				_Links.ListGroups();
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Links.ListXmlRoot, true));
				xLogger.Debug("groups:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("groups::error_links_list:", e.Message);
				throw (new x_exception("error_links_list", String.Concat(error_links_list, e.Message)));
			}
		}
		/// <summary>Get list of flags</summary>
		private void flags() {
			try {
				xLogger.Info("flags:");
				_Links.ListFlags();
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Links.ListXmlRoot, true));
				xLogger.Debug("flags:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("list::error_links_list:", e.Message);
				throw (new x_exception("error_links_list", String.Concat(error_links_list, e.Message)));
			}
		}
		/// <summary>Get list of pages</summary>
		private void pages() {
			try {
				xLogger.Info("pages:");
				_Links.ListFiles();
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Links.ListXmlRoot, true));
				xLogger.Debug("pages:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("list::error_links_list:", e.Message);
				throw (new x_exception("error_links_list", String.Concat(error_links_list, e.Message)));
			}
		}
		#endregion

		#region Private utility methods
		private string addBlog(string space, string topic, string blog, string title) {
			if (String.IsNullOrEmpty(blog) || _IsDefault(blog)) {	// ie assume a new blog
				_AdminX.AddBlogHtml(space, topic, title, "");
				blog = _AdminX.ItemXmlRoot.SelectSingleNode("//blogX/Result/blog/@id").InnerText;
			}
			return blog;
		}
		#endregion
	}
}