using System;
using System.Web;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	2.6.0
	Build:		20110905
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Started
	20101214:	Added call to CmsXBrokerSlides
	20101218:	Spaces and Topics
	20110905:	Replaced Debugger.Spoor with xLogger.Debug
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
	/// <summary>
	/// Description of the classX class.
	/// <para>Additional information about the class</para>
	/// </summary>
	public class CmsXGridX : CmsX {
		#region Invisible properties
		#endregion

		#region Constants
		private const string CONFIG_ROOT = "CmsGridX";
		private const string logid = "CmsXGridX.";
		#endregion

		#region Visible properties
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public CmsXGridX() : base(CONFIG_ROOT, typeof(CmsXGridX), logid) {
		}
		/// <summary>Constructor for derived classes</summary>
        public CmsXGridX(string label) : base(label, typeof(CmsXGridX), logid) {
        }
		public CmsXGridX(string label, Type type, string loggerID) : base(label, type, loggerID) {
		}
		/// <summary>Constructor for derived classes</summary>
        public CmsXGridX(Type type, string loggerID) : base(CONFIG_ROOT, type, loggerID) {
        }
        #endregion

        #region Public methods
        #endregion

        #region Protected methods
        protected override void _Initialise() {
			base._Initialise();
			DisplayType = x_displayType.XmlTransform;
		}
		protected override void _Initialise(string linkName) {
			base._Initialise(linkName);
			DisplayType = x_displayType.XmlTransform;
		}
		/// <summary>
		/// Does the pre-processing of the page request. 
		///	This method is overriden in order to:
		///	- Add form fields to the profile
		/// </summary>
		protected override void _ProcessRequest() {
			xLogger.Debug("_ProcessRequest");

			string siteid = UserProfile.Value(Cms.PROFILE_SITE_ID);
			xLogger.Debug("_ProcessRequest::Broker:", Parameters.Broker, "::siteid:", siteid);
			switch (Parameters.Broker) {
				case "update_profile":				Broker = new CmsXBrokerBase(this);				Broker.Process("update_profile");		break;
				
				// Content - menus
				//case "list_menus":				Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("menus_list");			break;
				//case "get_menu":					Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("menu_get");				break;
				//case "add_menu":					Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("menu_add");				break;
				//case "delete_menu":				Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("menu_delete");			break;
				//case "edit_menu_menuitems": 		Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("menu_menuitems_edit");	break;
				//case "parent_list_dd":			Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("parent_list_dd");		break;
				//case "link_list_dd":				Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("link_list_dd");			break;

				case "list_menus":					Broker = new CmsXBrokerMenus(this, siteid);		Broker.Process("list");					break;
				case "list_menus_dd":				Broker = new CmsXBrokerMenus(this, siteid);		Broker.Process("list_dd");				break;
				case "get_menu":					Broker = new CmsXBrokerMenus(this, siteid);		Broker.Process("get");					break;
				case "add_menu":					Broker = new CmsXBrokerMenus(this, siteid);		Broker.Process("add");					break;
				case "delete_menu":					Broker = new CmsXBrokerMenus(this, siteid);		Broker.Process("delete");				break;
				case "edit_menu": 					Broker = new CmsXBrokerMenus(this, siteid);		Broker.Process("edit");					break;
				case "profiles_dd":					Broker = new CmsXBrokerMenus(this, siteid);		Broker.Process("profiles_dd");			break;
				case "get_menu_items":				Broker = new CmsXBrokerMenus(this, siteid);		Broker.Process("getitems");				break;
				case "list_menusitems_dd":			Broker = new CmsXBrokerMenus(this, siteid);		Broker.Process("list_items");			break;

				// Content - pages
				case "list_pages":					Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("pages_list");			break;
				case "list_pages_dd":				Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("pages_list_dd");		break;
				case "get_page":					Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("page_get");				break;
				case "edit_pages_content":			Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("pages_content_edit");	break;
				case "add_pages_content":			Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("pages_content_add");	break;
				case "delete_page":					Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("pages_delete");			break;
				case "list_thoughtspace_dd":		Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("thoughtspace_list_dd");	break;
				case "list_topic_dd":				Broker = new CmsXBrokerDisplay(this, siteid);	Broker.Process("topic_list_dd");		break;

				// Content - Links
				case "search_link":					Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("search");				break;
				case "list_filter_links":			Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("list_filter");			break;
				case "list_links":					Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("list");					break;
				case "list_links_dd":				Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("list_dd");				break;
				case "get_link":					Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("get");					break;
				case "get_link_flags":				Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("getflags");				break;
				case "get_link_blogs":				Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("getblogs");				break;
				case "get_link_links":				Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("getlinks");				break;
				case "get_link_hints":				Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("gethints");				break;
				case "get_link_pages":				Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("getpages");				break;
				case "select_link":        			Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("select");				break;
				case "add_link":           			Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("add");					break;
				case "edit_link":          			Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("edit");					break;
				case "delete_link":					Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("delete");				break;
				case "links_groups_dd":				Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("groups_dd");			break;
				case "links_flags_dd":				Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("flags_dd");				break;
				case "links_pages_dd":				Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("pages_dd");				break;

				case "get_sub_link":				Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("get_sub");				break;
				case "submit_sub_link":        		Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("submit_sub");			break;
				case "add_sub_link":           		Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("add_sub");				break;
				case "edit_sub_link":          		Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("edit_sub");				break;
				case "delete_sub_link":				Broker = new CmsXBrokerLinks(this, siteid);		Broker.Process("delete_sub");			break;

				// Access - users
				case "delete_admin_user":			Broker = new CmsXBrokerAccess(this); Broker.Process("user_admin_delete");				break;
                case "list_admin_users_dd":			Broker = new CmsXBrokerAccess(this); Broker.Process("user_admin_list_dd");				break;
				case "list_admin_users":			Broker = new CmsXBrokerAccess(this); Broker.Process("user_admin_list");					break;
                case "get_admin_user":				Broker = new CmsXBrokerAccess(this); Broker.Process("user_admin_get");					break;
				case "add_admin_user":				Broker = new CmsXBrokerAccess(this); Broker.Process("user_admin_add");					break;
				case "edit_admin_user":				Broker = new CmsXBrokerAccess(this); Broker.Process("user_admin_edit");					break; 
 
				// Access - groups
				case "list_groups":					Broker = new CmsXBrokerAccess(this); Broker.Process("groups_list");						break;
				case "get_groups":					Broker = new CmsXBrokerAccess(this); Broker.Process("groups_get");						break;
				case "add_groups":					Broker = new CmsXBrokerAccess(this); Broker.Process("groups_add");						break;
				case "edit_groups_categories":		Broker = new CmsXBrokerAccess(this); Broker.Process("groups_categories_edit");			break;
				case "delete_groups":				Broker = new CmsXBrokerAccess(this); Broker.Process("groups_delete");					break;
				case "get_rights_groups":			Broker = new CmsXBrokerAccess(this); Broker.Process("groups_rights_get");				break;
				case "edit_rights_groups":			Broker = new CmsXBrokerAccess(this); Broker.Process("groups_rights_edit");				break;

				// Access - shows
				case "list_shows":					Broker = new CmsXBrokerAccess(this); Broker.Process("shows_list");						break;
				case "get_shows":					Broker = new CmsXBrokerAccess(this); Broker.Process("shows_get");						break;
				case "edit_shows":					Broker = new CmsXBrokerAccess(this); Broker.Process("shows_edit");						break;
				//case "add_shows":					Broker = new CmsXBrokerAccess(this); Broker.Process("shows_add");						break;
				//case "delete_shows":				Broker = new CmsXBrokerAccess(this); Broker.Process("shows_delete");					break;

				// Access - linklogs
				case "search_linklogs":				Broker = new CmsXBrokerLinkLog(this);	Broker.Process("search");						break;
				case "list_linklogs":				Broker = new CmsXBrokerLinkLog(this);	Broker.Process("list");							break;
				case "list_linklogs_dd":			Broker = new CmsXBrokerLinkLog(this);	Broker.Process("list_dd");						break;
				case "get_linklogs":				Broker = new CmsXBrokerLinkLog(this);	Broker.Process("get");							break;
                case "submit_linklogs":        		Broker = new CmsXBrokerLinkLog(this);	Broker.Process("submit");						break;
                case "add_linklogs":           		Broker = new CmsXBrokerLinkLog(this);	Broker.Process("add");							break;
                case "edit_linklogs":          		Broker = new CmsXBrokerLinkLog(this);	Broker.Process("edit");							break;
				case "delete_linklogs":				Broker = new CmsXBrokerLinkLog(this);	Broker.Process("delete");						break;
				
				// Spaces
				case "search_space":				Broker = new CmsXBrokerSpaces(this, siteid);	Broker.Process("search");				break;
				case "list_filter_spaces":			Broker = new CmsXBrokerSpaces(this, siteid);	Broker.Process("list_filters");			break;
				case "list_spaces":					Broker = new CmsXBrokerSpaces(this, siteid);	Broker.Process("list");					break;
				case "list_spaces_dd":				Broker = new CmsXBrokerSpaces(this, siteid);	Broker.Process("list_dd");				break;
				case "list_spaces_admin":			Broker = new CmsXBrokerSpaces(this, siteid);	Broker.Process("list_admin");			break;
				case "get_space":					Broker = new CmsXBrokerSpaces(this, siteid);	Broker.Process("get");					break;
				case "get_space_topics":			Broker = new CmsXBrokerSpaces(this, siteid);	Broker.Process("get_topics");			break;
                case "select_space":        		Broker = new CmsXBrokerSpaces(this, siteid);	Broker.Process("select");				break;
                case "add_space":           		Broker = new CmsXBrokerSpaces(this, siteid);	Broker.Process("add");					break;
                case "edit_space":          		Broker = new CmsXBrokerSpaces(this, siteid);	Broker.Process("edit");					break;
				case "delete_space":				Broker = new CmsXBrokerSpaces(this, siteid);	Broker.Process("delete");				break;

				// Topics
				case "search_topic":				Broker = new CmsXBrokerTopics(this, siteid);	Broker.Process("search");				break;
				case "list_filter_topics":			Broker = new CmsXBrokerTopics(this, siteid);	Broker.Process("list_filter");			break;
				case "list_topics":					Broker = new CmsXBrokerTopics(this, siteid);	Broker.Process("list");					break;
				case "list_topics_dd":				Broker = new CmsXBrokerTopics(this, siteid);	Broker.Process("list_dd");				break;
				case "list_topics_admin":			Broker = new CmsXBrokerTopics(this, siteid);	Broker.Process("list_admin");			break;
				case "get_topic":					Broker = new CmsXBrokerTopics(this, siteid);	Broker.Process("get");					break;
                case "select_topic":        		Broker = new CmsXBrokerTopics(this, siteid);	Broker.Process("select");				break;
                case "add_topic":           		Broker = new CmsXBrokerTopics(this, siteid);	Broker.Process("add");					break;
                case "edit_topic":          		Broker = new CmsXBrokerTopics(this, siteid);	Broker.Process("edit");					break;
				case "delete_topic":				Broker = new CmsXBrokerTopics(this, siteid);	Broker.Process("delete");				break;

				// Blogs
				case "search_blogs":				Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("search");				break;
				case "list_filter_blogs":			Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("list_filters");			break;
				case "list_blogs":					Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("list");					break;
				case "list_blogs_dd":				Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("list_dd");				break;
				case "list_blogs_admin":			Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("list_admin");			break;
				case "get_blog":					Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("get");					break;
				case "select_blog":        			Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("select");				break;
				case "add_blog":           			Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("add");					break;
				case "edit_blog":          			Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("edit");					break;
				case "delete_blog":					Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("delete");				break;
				case "list_filter_blogs_dd":		Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("filter_dd");			break;
				case "get_blog_comments":			Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("get_comments");			break;
				case "get_blog_history":			Broker = new CmsXBrokerBlogs(this, siteid);		Broker.Process("get_history");			break;

				// media
				case "search_media":				Broker = new CmsXBrokerMedia(this, siteid);		Broker.Process("search");				break;
				case "list_filter_media":			Broker = new CmsXBrokerMedia(this, siteid);		Broker.Process("list_filter");			break;
				case "list_media":					Broker = new CmsXBrokerMedia(this, siteid);		Broker.Process("list");					break;
				case "list_media_dd":				Broker = new CmsXBrokerMedia(this, siteid);		Broker.Process("list_dd");				break;
				case "list_media_admin":			Broker = new CmsXBrokerMedia(this, siteid);		Broker.Process("list_admin");			break;
				case "get_media":					Broker = new CmsXBrokerMedia(this, siteid);		Broker.Process("get");					break;
				case "select_media":        		Broker = new CmsXBrokerMedia(this, siteid);		Broker.Process("select");				break;
				case "add_media":           		Broker = new CmsXBrokerMedia(this, siteid);		Broker.Process("add");					break;
				case "edit_media":          		Broker = new CmsXBrokerMedia(this, siteid);		Broker.Process("edit");					break;
				case "delete_media":				Broker = new CmsXBrokerMedia(this, siteid);		Broker.Process("delete");				break;

				// News
				case "search_news":					Broker = new CmsXBrokerNews(this, siteid);		Broker.Process("search");				break;
				case "list_filter_news":			Broker = new CmsXBrokerNews(this, siteid);		Broker.Process("list_filter");			break;
				case "list_news":					Broker = new CmsXBrokerNews(this, siteid);		Broker.Process("list");					break;
				case "list_news_dd":				Broker = new CmsXBrokerNews(this, siteid);		Broker.Process("list_dd");				break;
				case "list_news_admin":				Broker = new CmsXBrokerNews(this, siteid);		Broker.Process("list_admin");			break;
				case "get_news":					Broker = new CmsXBrokerNews(this, siteid);		Broker.Process("get");					break;
				case "select_news":        			Broker = new CmsXBrokerNews(this, siteid);		Broker.Process("select");				break;
				case "add_news":           			Broker = new CmsXBrokerNews(this, siteid);		Broker.Process("add");					break;
				case "edit_news":          			Broker = new CmsXBrokerNews(this, siteid);		Broker.Process("edit");					break;
				case "delete_news":					Broker = new CmsXBrokerNews(this, siteid);		Broker.Process("delete");				break;

				// Files
				case "search_file":					Broker = new CmsXBrokerFile(this, siteid);		Broker.Process("search");				break;
				case "list_filter_files":			Broker = new CmsXBrokerFile(this, siteid);		Broker.Process("list_filter");			break;
				case "list_files":					Broker = new CmsXBrokerFile(this, siteid);		Broker.Process("list");					break;
				case "list_files_dd":				Broker = new CmsXBrokerFile(this, siteid);		Broker.Process("list_dd");				break;
				case "list_files_admin":			Broker = new CmsXBrokerFile(this, siteid);		Broker.Process("list_admin");			break;
				case "get_file":					Broker = new CmsXBrokerFile(this, siteid);		Broker.Process("get");					break;
				case "select_file":        			Broker = new CmsXBrokerFile(this, siteid);		Broker.Process("select");				break;
				case "add_file":           			Broker = new CmsXBrokerFile(this, siteid);		Broker.Process("add");					break;
				case "edit_file":          			Broker = new CmsXBrokerFile(this, siteid);		Broker.Process("edit");					break;
				case "delete_file":					Broker = new CmsXBrokerFile(this, siteid);		Broker.Process("delete");				break;
				case "file_extns_dd":				Broker = new CmsXBrokerFile(this, siteid);		Broker.Process("extns_dd");				break;
				case "file_paths_dd":				Broker = new CmsXBrokerFile(this, siteid);		Broker.Process("paths_dd");				break;

				//// Xxxxxx
				//case "search_xxxxx":				Broker = new CmsXBrokerXxxx(this, siteid);		Broker.Process("search");				break;
				//case "list_filter_xxxxxs":		Broker = new CmsXBrokerXxxx(this, siteid);		Broker.Process("list_filter");			break;
				//case "list_xxxxxs":				Broker = new CmsXBrokerXxxx(this, siteid);		Broker.Process("list");					break;
				//case "list_xxxxxs_dd":			Broker = new CmsXBrokerXxxx(this, siteid);		Broker.Process("list_dd");				break;
				//case "list_xxxxxs_admin":			Broker = new CmsXBrokerXxxx(this, siteid);		Broker.Process("list_admin");			break;
				//case "get_xxxxx":					Broker = new CmsXBrokerXxxx(this, siteid);		Broker.Process("get");					break;
				//case "select_xxxxx":        		Broker = new CmsXBrokerXxxx(this, siteid);		Broker.Process("select");				break;
				//case "add_xxxxx":           		Broker = new CmsXBrokerXxxx(this, siteid);		Broker.Process("add");					break;
				//case "edit_xxxxx":          		Broker = new CmsXBrokerXxxx(this, siteid);		Broker.Process("edit");					break;
				//case "delete_xxxxx":				Broker = new CmsXBrokerXxxx(this, siteid);		Broker.Process("delete");				break;
				//
				//case "get_sub_xxxxx":				Broker = new CmsXBrokerXxxx(this);	Broker.Process("get_sub_xxxxx");				break;
				//case "submit_sub_xxxxx":        	Broker = new CmsXBrokerXxxx(this);	Broker.Process("submit_sub_xxxxx");				break;
				//case "add_sub_xxxxx":           	Broker = new CmsXBrokerXxxx(this);	Broker.Process("add_sub_xxxxx");				break;
				//case "edit_sub_xxxxx":          	Broker = new CmsXBrokerXxxx(this);	Broker.Process("edit_sub_xxxxx");				break;
				//case "delete_sub_xxxxx":			Broker = new CmsXBrokerXxxx(this);	Broker.Process("delete_sub_xxxxx");				break;
			}
		}
		/// <summary>
		/// Does the post-processing of the page content. 
		///	This method is overriden to post process the xml content through a broker to:
		///	- Save CSV settings (on lists)
		/// </summary>
		protected override void _ProcessContent() {
			xLogger.Debug("_ProcessContent");
			switch (Parameters.Broker) {
				case "csv_settings_save":			Broker = new CmsXBrokerCSV(this);	Broker.Process("settings_save");	break;
			}
		}
		/// <summary>
		/// Does the post-processing of the page content. 
		///	This method is overriden to post process the xml content through a broker to:
		///	- Save CSV settings (on lists)
		/// </summary>
		protected override void _DecodeContent() {
			foreach (XmlNode textnode in Content.SelectNodes("//blog/bloghtml")) {
				//XmlElement textel = (XmlElement)textnode;
				//textel.SetAttribute("decoded", "yes");
				//textel.InnerXml = HttpUtility.HtmlDecode(textel.InnerText);
				//textel.InnerXml = textel.InnerXml.Replace("&nbsp;", "&#160;");

				string decodedHtml = HttpUtility.HtmlDecode(textnode.InnerText);
				textnode.InnerText = decodedHtml;
			}
		}
		#endregion

		#region Private methods
		#endregion
	}
}
