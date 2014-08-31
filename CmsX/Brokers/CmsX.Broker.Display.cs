using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.2
	Build:		20130513
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Refactored from LoeriesAdmin
	20100927:	Replaced topic_list with new approach that uses xsl to format 
				output, rather than manipulating xml nodes here
	20101020:	Moved process(Tinymce)Html to CmsXBrokerBase
	20101206:	Deprecated class.
				- Menu-related processes moved to CmsXBrokerMenus
				- Page-related processes stay here, for now
	20130513:	Added _IsDefault as test for DEFAULT_SUBITEM
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>
	/// Broker class to manage displayx for show.
	/// <para>Management of menus</para>
	/// </summary>
	public class CmsXBrokerDisplay : CmsXBrokerBase {
		#region Invisible properties
		#endregion

		#region Constant name strings
		private const string logid = "CmsXBrokerDisplay.";
		#endregion

		#region Constant error strings
		private const string error_menu_list = "Error listing the menus: ";
		private const string error_menu_get = "Error getting the menu item: ";
		private const string error_menu_add = "Error adding the menu: ";
		private const string error_menu_delete = "Error deleting the menu: ";
		private const string error_menu_menuitems_edit = "Error editting the menu and menuitems: ";
		private const string error_pages_list = "Error listing the pages: ";
		private const string error_page_get = "Error getting the page: ";
		private const string error_page_delete = "Error deleting the page: ";
		private const string error_pages_content_edit = "Error editing the page and content: ";
		private const string error_pages_content_add = "Error adding the page and content: ";
		private const string error_press_pages_list = "Error listing the press pages: ";
		private const string error_pages_press_add = "Error adding press page: ";
		private const string error_press_upload = "Error uploading press document: ";
		private const string error_press_page_delete = "Error deleting press document: ";
		private const string error_press_page_get = "Error getting press page: ";
		private const string error_parent_list = "Error listing the menu parents: ";
		private const string error_links_list = "Error listing the links below the parent menu node: ";
		private const string error_topics_list = "Error listing the topics: ";
		#endregion

		#region Visible properties
		#endregion

		#region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerDisplay(CmsX thispage) : base(thispage, typeof(CmsXBrokerDisplay), logid) {
		}
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerDisplay(CmsX thispage, string blogxID) : base(thispage, blogxID, typeof(CmsXBrokerDisplay), logid) {
		}
		#endregion

		#region Public methods
		public override void Process(string type) {
			base.Process(true);	// does login checking
			xLogger.Info("Process:", type);
			switch (type) {
				//menus
				case "menus_list":			menus_list();				break;
				case "menu_get":			menu_get();					break;
				case "menu_add":			menu_add();					break;
				case "menu_delete":			menu_delete();				break;
				case "menu_menuitems_edit":	menu_menuitem_edit();		break;
				case "parent_list_dd":		parent_list(true);			break;
				//pages  
				case "pages_list":			pages_list();				break; 
				case "pages_list_dd":		pages_list(true);			break;
				case "page_get":			page_get();					break;
				case "pages_content_edit":	pages_content_edit();		break;
				case "pages_content_add":	add_pages_content();		break;
				case "pages_delete":		pages_delete();				break;
				case "space_list_dd":		thoughtspace_list(true);	break;
				case "topic_list_dd":		topic_list(true);			break;
				//press
				case "pages_press_list":	pages_press_list();			break;
				case "pages_press_add":		pages_press_add();			break;
				case "press_stat":			press_stat();				break;
				case "press_upload":		press_upload();				break;
				case "pages_press_delete":	pages_press_delete();		break;
				case "pages_press_edit":	pages_press_edit();			break;
				case "pages_press_get":		pages_press_get();			break;
				case "press_view":			pages_press_view();			break;
			}
		}
		#endregion

		#region Protected methods
		#endregion

		#region Private Displayx methods
		
		/// <summary>List menus</summary>
		private void menus_list() {
			menus_list(false);
		}
		/// <summary>List menus</summary>
		private void menus_list(bool clearProfile) {
			try {
				xLogger.Info("menus_list:");
				SearchSettings search = new SearchSettings(UserProfile, Logger, 1, "3", false, true);
				if (clearProfile) {
					UserProfile.Add("LA_Profile_ProfileID", "0");
				}
				DisplayX.GetMenus(search, clearProfile);
				xLogger.Debug("GetMenusXml:::", DisplayX.ListXmlRoot.OuterXml);
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(DisplayX.ListXmlRoot as XmlNode, true));
				xLogger.Debug("menus_list::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_menu_list", String.Concat(error_menu_list, e.Message)));
			}
		}

		/// <summary>List menus</summary>
		private string getThoughtspace() {
			DisplayX.GetThoughtspace();
			string thoughtspace = DisplayX.ItemXmlRoot.SelectSingleNode("//thoughtspace/@value").InnerText;
			return thoughtspace;
		}

		/// <summary>List pages</summary>
		private void pages_list() {
			pages_list(false);
		}
		/// <summary>List menus</summary>
		private void pages_list(bool clearProfile) {
			try {
				string data_id = UIPage.QueryParam("id", "0");
				xLogger.Info("pages_list::data_id:", data_id);
				SearchSettings search = new SearchSettings(UserProfile, Logger, 1, "3", false, true);
				if (clearProfile) {
					UserProfile.Add("LA_Profile_ProfileID", "0");
				}
				DisplayX.GetPages(search, clearProfile);
				if (data_id != "0") {
					Logger.Debug(String.Concat("_SetPageRow:ID::", data_id));
					CmsXGrid grid = new CmsXGrid(UIPage, false);
					int rows = Int32.Parse(DisplayX.ListXmlRoot.SelectSingleNode(Cms.SELECT_ITEMS_ROWS).InnerText);
					int row = Int32.Parse(DisplayX.ListXmlRoot.SelectSingleNode(String.Format(Cms.SELECT_ITEMSITEM_ROW, data_id)).InnerText);
					Logger.Debug(String.Concat("Number of rows pre::", grid.RowsPerPage.ToString()));
					grid.RowsPerPage = rows;
					Logger.Debug(String.Concat("Number of rows post::", grid.RowsPerPage.ToString()));
					grid.SetFocus(rows, row);
				}
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(DisplayX.ListXmlRoot as XmlNode, true));
				xLogger.Debug("pages_list::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_pages_list", String.Concat(error_pages_list, e.Message)));
			}
		}

		/// <summary>List parents</summary>
		private void parent_list() {
			parent_list(false);
		}
		/// <summary>List parents</summary>
		private void parent_list(bool clearProfile) {
			try {
				xLogger.Info("parent_list:");
				SearchSettings search = new SearchSettings(UserProfile, Logger, 1, "3", false, true);
				if (clearProfile) {
					UserProfile.Add("LA_Profile_ProfileID", "0");
				}
				DisplayX.GetParents(search, clearProfile);
				Logger.Debug(String.Concat("_list Parents:Xml::", DisplayX.ItemXmlRoot.OuterXml));
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(DisplayX.ListXmlRoot as XmlNode, true));
				xLogger.Debug("parent_list::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_parent_list", String.Concat(error_parent_list, e.Message)));
			}
		}
		/// <summary>List Thoughtspaces</summary>
		private void thoughtspace_list() {
			thoughtspace_list(false);
		}
		/// <summary>List Thoughtspaces</summary>
		private void thoughtspace_list(bool clearProfile) {
			try {
				xLogger.Info("Thoughtspaces_list:");
				SearchSettings search = new SearchSettings(UserProfile, Logger, 1, "3", false, true);
				if (clearProfile) {
					UserProfile.Add("LA_Profile_ProfileID", "0");
				}
				_AdminX.GetThoughtspaces();

				XmlDocument xmlResult = new XmlDocument();
				xmlResult.LoadXml("<ItemList><Result/></ItemList>");
				XmlNode resultNode = xmlResult.SelectSingleNode("ItemList/Result");
				
				XmlDocument xmlThtSpaces = new XmlDocument();
				xmlThtSpaces.LoadXml("<items/>");
				
				string items = "0,";
				int Items = 1;
				XmlNodeList ThtSpaceNodes = _AdminX.ItemXmlRoot.SelectNodes("//Result/user/blogspaces/blogspace/@name");
				foreach (XmlNode ThtSpaceNode in ThtSpaceNodes) {
					XmlDocument item = new XmlDocument();
					item.LoadXml("<item/>");
					string name = ThtSpaceNode.InnerText;
					((XmlElement)item.SelectSingleNode("item")).SetAttribute("desc", name);
					((XmlElement)item.SelectSingleNode("item")).SetAttribute("id", name);
					XmlNode ThtNode = xmlThtSpaces.SelectSingleNode("items");
					ThtNode.AppendChild(ThtNode.OwnerDocument.ImportNode(item.SelectSingleNode("item"), true));
					items = (String.Concat(items, Items.ToString(), ","));
					Items++;
				}
				XmlElement itemsNode = xmlThtSpaces.SelectSingleNode("items") as XmlElement;
				itemsNode.SetAttribute("ids", items);
				resultNode.SelectSingleNode("//Result").AppendChild(resultNode.OwnerDocument.ImportNode(itemsNode as XmlNode, true));

				XmlNode thoughtspacelistNode = xmlResult.SelectSingleNode("ItemList");
				xLogger.Debug("Thoughtspaces_list::xml:::", thoughtspacelistNode.OuterXml);
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(thoughtspacelistNode, true));
				xLogger.Debug("Thoughtspaces_list::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_pages_list", String.Concat(error_pages_list, e.Message)));
			}
		}
		
		/// <summary>List Topics</summary>
		private void topic_list() {
			topic_list(false);
		}
		/// <summary>List Topics</summary>
		private void topic_list(bool clearProfile) {
			xLogger.Info("topics_list:");
			try {
				string thoughtspace = getThoughtspace();
				xLogger.Info("topics_list::thoughtspace:", thoughtspace);

				_AdminX.ListTopics(thoughtspace);
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_AdminX.ListXmlRoot, true));

				xLogger.Debug("topics_list::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_topics_list", String.Concat(error_topics_list, e.Message)));
			}
		}


		/*
		/// <summary>List Topics</summary>
		private void topic_list_org() {
			topic_list_org(false);
		}
		/// <summary>List Topics</summary>
		private void topic_list_org(bool clearProfile) {
			try {
				xLogger.Info("topics_list:");
				SearchSettings search = new SearchSettings(UserProfile, Logger, 1, "3", false, true);
				if (clearProfile) {
					UserProfile.Add("LA_Profile_ProfileID", "0");
				}
				string thoughtspace = getThoughtspace();
				xLogger.Info("topics_list::thoughtspace:", thoughtspace);
				_AdminX.GetTopics(thoughtspace);
				XmlDocument xmlResult = new XmlDocument();
				xmlResult.LoadXml("<ItemList><Result/></ItemList>");
				XmlNode resultNode = xmlResult.SelectSingleNode("ItemList/Result");
				XmlDocument xmlTopics = new XmlDocument();
				xmlTopics.LoadXml("<items/>");
				string items = "0,";
				int Items = 1;
				XmlNodeList TopicNodes = _AdminX.ItemXmlRoot.SelectNodes(String.Concat("//Result/user/blogspaces/blogspace[@name='", thoughtspace, "']/topics/topic/@name"));
				foreach (XmlNode TopicNode in TopicNodes) {
					XmlDocument item = new XmlDocument();
					item.LoadXml("<item/>");
					string name = TopicNode.InnerText;
					((XmlElement)item.SelectSingleNode("item")).SetAttribute("desc", name);
					((XmlElement)item.SelectSingleNode("item")).SetAttribute("id", name);
					XmlNode TopNode = xmlTopics.SelectSingleNode("items");
					TopNode.AppendChild(TopNode.OwnerDocument.ImportNode(item.SelectSingleNode("item"), true));
					items = (String.Concat(items, Items.ToString(), ","));
					Items++;
				}
				XmlElement itemsNode = xmlTopics.SelectSingleNode("items") as XmlElement;
				itemsNode.SetAttribute("ids", items);
				resultNode.SelectSingleNode("//Result").AppendChild(resultNode.OwnerDocument.ImportNode(itemsNode as XmlNode, true));
				XmlNode topiclistNode = xmlResult.SelectSingleNode("ItemList");
				xLogger.Debug("topics_list::xml:::", topiclistNode.OuterXml);
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(topiclistNode, true));
				xLogger.Debug("topics_list::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_topics_list", String.Concat(error_topics_list, e.Message)));
			}
		}
		*/

		/// <summary>List press pages</summary>
		private void pages_press_list() {
			pages_press_list(false);
		}
		/// <summary>List menus</summary>
		private void pages_press_list(bool clearProfile) {
			try {
				xLogger.Info("press_pages_list:");
				XmlNode thoughtspacelistNode = getPressPages(clearProfile);
				xLogger.Debug(" list presspages xml::", thoughtspacelistNode.OuterXml);
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(thoughtspacelistNode, true));
				xLogger.Debug("press_pages_list::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_press_pages_list", String.Concat(error_press_pages_list, e.Message)));
			}
		}
		/// <summary>List menus</summary>
		private XmlNode getPressPages(bool clearProfile) {
			SearchSettings search = new SearchSettings(UserProfile, Logger, 1, "3", false, true);
			if (clearProfile) {
				UserProfile.Add("LA_Profile_ProfileID", "0");
			}
			string thoughtspace = getThoughtspace(); 
			_AdminX.GetPressPages(search, clearProfile, thoughtspace);
			XmlNode topicsNode = _AdminX.ListXmlRoot.SelectSingleNode("//blogX/Result/topics");
			_AdminX.ListXmlRoot.SelectSingleNode("//blogX/Result").RemoveChild(topicsNode);

			XmlDocument xmlResult = new XmlDocument();
			xmlResult.LoadXml("<ItemList/>");
			XmlNode resultNode = xmlResult.SelectSingleNode("ItemList");
			resultNode.AppendChild(resultNode.OwnerDocument.ImportNode(_AdminX.ListXmlRoot.SelectSingleNode("//Result"), true));
			
			XmlDocument xmlBlogs = new XmlDocument();
			xmlBlogs.LoadXml("<items/>");
			string items = "0,";
			int Items = 1;
			XmlNodeList blogNodes = xmlResult.SelectNodes("//blog");
			foreach (XmlNode blogNode in blogNodes) {
				XmlDocument item = new XmlDocument();
				item.LoadXml("<item/>");
				string title = blogNode.SelectSingleNode("title").InnerText;
				DateTime dateObj = DateTime.Parse(blogNode.SelectSingleNode("date/@create").InnerText);
				string date = dateObj.ToString("yyyy/MM/dd HH:mm:ss tt");
				string blogtext = blogNode.SelectSingleNode("blogtext").InnerText;
				string id = blogNode.SelectSingleNode("@id").InnerText;
				((XmlElement)item.SelectSingleNode("item")).SetAttribute("id", id);
				((XmlElement)item.SelectSingleNode("item")).SetAttribute("title", title);
				((XmlElement)item.SelectSingleNode("item")).SetAttribute("date", date);
				((XmlElement)item.SelectSingleNode("item")).SetAttribute("blogtext", blogtext);
				XmlNode blogsNode = xmlBlogs.SelectSingleNode("items");
				blogsNode.AppendChild(blogsNode.OwnerDocument.ImportNode(item.SelectSingleNode("item"), true));
				items = (String.Concat(items, Items.ToString(), ","));
				Items++;
			}
			XmlElement itemsNode = xmlBlogs.SelectSingleNode("items") as XmlElement;
			itemsNode.SetAttribute("ids", items);
			resultNode.SelectSingleNode("//Result").AppendChild(resultNode.OwnerDocument.ImportNode(itemsNode as XmlNode, true));
			XmlNode oldBlogsNode = resultNode.SelectSingleNode("//blogs");
			resultNode.SelectSingleNode("//Result").RemoveChild(oldBlogsNode);
			XmlNode thoughtspacelistNode = xmlResult.SelectSingleNode("ItemList");
			return thoughtspacelistNode;
		}
		/// <summary>Get menu</summary>
		private void menu_get() {
			try {
				string menuID = _GetQueryID("menuId");
				xLogger.Debug("menu_get:menuID:", menuID);
				
				DisplayX.GetMenu(Int32.Parse(menuID));
				DisplayX.Item.SetAttribute("id", menuID);
				DisplayX.Item.SetAttribute("row", "1");
				xLogger.Info("menu_get:ItemXmlRootNode:", DisplayX.ItemXmlRootNode.OuterXml);
				
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(DisplayX.ItemXmlRootNode, true));

				xLogger.Debug("menu_get::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_menu_get", String.Concat(error_menu_get, e.Message)));
			}
		}
		/* originally (not sure why the foreach??)
		/// <summary>Get menu</summary>
		private void menu_get() {
			try {
				xLogger.Info("menu_get:");
				string menuID = _GetQueryID("menuId");
				xLogger.Info("menu_get:menuID:", menuID);
				DisplayX.GetMenu(Int32.Parse(menuID));
				DisplayX.MoveItemsToRoot();
				int rowi = 1;
				foreach (XmlNode itm in DisplayX.ItemXmlRootNode.SelectNodes("//users/item")) {
					((XmlElement)itm).SetAttribute("row", rowi.ToString());
					rowi++;
				}
				//DisplayX.Item.RemoveChild(DisplayX.Item.SelectSingleNode("users"));
				DisplayX.Item.SetAttribute("id", DisplayX.Item.GetAttribute("menuId"));
				DisplayX.Item.SetAttribute("row", "1");

				xLogger.Info("menu_get:ItemXmlRootNode:", DisplayX.ItemXmlRootNode.OuterXml);
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(DisplayX.ItemXmlRootNode, true));

				xLogger.Debug("menu_get::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_menu_get", String.Concat(error_menu_get, e.Message)));
			}
		}
		*/
		/// <summary>Get press page</summary>
		private XmlNode pages_press_get() {
			try {
				xLogger.Info("press_page_get:");
				string blogID = _GetQueryID("id");
				xLogger.Info("press_page_get:blogID:", blogID);
				XmlNode pressNode = getPressPages(false);

				XmlNodeList itemNodes = pressNode.SelectNodes(String.Format(Cms.SELECT_ITEMSITEM_NOTID, blogID));
				foreach (XmlNode itemNode in itemNodes) {
					pressNode.SelectSingleNode(Cms.SELECT_ITEMS).RemoveChild(itemNode);
				}
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(pressNode, true));

				xLogger.Debug("press_page_get::finished:ok, XML::", pressNode.OuterXml);
				return pressNode;
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_press_page_get", String.Concat(error_press_page_get, e.Message)));
			}
		}
		/// <summary>Get page</summary>
		private void page_get() {
			try {
				xLogger.Info("page_get:");
				string pageID = _GetQueryID("PageId");
				xLogger.Info("page_get:pageID:", pageID);
				DisplayX.GetPage(Int32.Parse(pageID));

				//XmlNode contentItems = DisplayX.ItemXmlRootNode.SelectSingleNode(Cms.SELECT_ITEMSITEM);

				string thoughtspace = DisplayX.Item.GetAttribute("Thoughtspace");
				string topic = DisplayX.Item.GetAttribute("Topic");
				string blog = DisplayX.Item.GetAttribute("Blog");
				xLogger.Debug("; Thoughtspace:", thoughtspace, "; Topic:", topic, "; Blog:", blog);
				
				XmlNode contentNode = _AdminX.GetBlog(thoughtspace, topic, blog);
				Logger.Debug(String.Concat(" Blog Content::", contentNode.OuterXml));
				addNode(contentNode, "//blog/title"); 
				addNode(contentNode, "//blog/date");
				string blogHtml = contentNode.SelectSingleNode("//blog/bloghtml").InnerText;
				string decodedHtml = HttpUtility.HtmlDecode(blogHtml);
				Logger.Debug(String.Concat(" The Decoded Html::", decodedHtml));
				contentNode.SelectSingleNode("//blog/bloghtml").InnerText = decodedHtml;
				addNode(contentNode, "//blog/bloghtml");
				
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(DisplayX.ItemXmlRootNode, true));

				xLogger.Debug("page_get::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_page_get", String.Concat(error_page_get, e.Message)));
			}
		}
		/// <summary>Edit page and content</summary>
		private void pages_content_edit() {
			try {
				xLogger.Info("pages_content_edit:");
				string thoughtspace = _GetQueryID("Thoughtspace");
				string topic = _GetQueryID("Topic");
				string blog = _GetQueryID("Blog");
				string title = _GetQueryID("title");
				string date = _GetQueryID("date");
				string bloghtml = _GetQueryID("bloghtml");
				bloghtml = processTinymceHtml(bloghtml);
				xLogger.Debug("pages_Content_Edit: Bloghtml::", bloghtml);
				_AdminX.EditBlogHtml(thoughtspace, topic, blog, title, bloghtml);
				xLogger.Debug("pages_content_edit::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_pages_content_edit", String.Concat(error_pages_content_edit, e.Message)));
			}
		}
		/// <summary>Edit page and content</summary>
		private void pages_press_edit() {
			try {
				xLogger.Info("pages_press_edit:");
				string thoughtspace = "loeries";
				string topic = "default";
				string blogID = _GetQueryID("id");
				string title = _GetQueryID("title");
				string dateTotal = _GetQueryID("date");
				DateTime date = DateTime.Parse(dateTotal);
				string year = date.ToString("yyyy");
				string month = date.ToString("MM");
				string day = date.ToString("dd");
				xLogger.Debug("AddPressPage::title:", title, ", Year:", year, ", Month:", month, ", Day:", day);
				_AdminX.EditBlogPress(thoughtspace, topic, blogID, title, year, month, day);
				xLogger.Debug("pages_press_edit::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_pages_content_edit", String.Concat(error_pages_content_edit, e.Message)));
			}
		}
		/// <summary>Add page and content</summary>
		private void add_pages_content() {
			try {
				xLogger.Info("pages_content_add:");
				string thoughtspace = getThoughtspace();
				string topic = _GetQueryID("Topic");
				string title = _GetQueryID("title");
				string bloghtml = _GetQueryID("bloghtml");
				string PageName = _GetQueryID("PageName");
				PageName = format(PageName);
				_AdminX.AddBlogHtml(thoughtspace, topic, title, bloghtml);
				string blog = _AdminX.ItemXmlRoot.SelectSingleNode("//blogX/Result/blog/@id").InnerText;
				xLogger.Debug("AddPage::PageName:", PageName, "; thoughtspace:", thoughtspace, "; Topic:", topic, ", Blog:", blog);
				DisplayX.AddPage(PageName, thoughtspace, topic, blog);
				xLogger.Debug("pages_content_add::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_pages_content_add", String.Concat(error_pages_content_add, e.Message)));
			}
		}
		/// <summary>Add press page</summary>
		private void pages_press_add() {
			try {
				xLogger.Info("pages_press_add:");
				string thoughtspace = getThoughtspace(); 
				string topic = "default";
				string title = _GetQueryID("title");
				string dateTotal = _GetQueryID("date");
				DateTime date = DateTime.Parse(dateTotal);
				string year = date.ToString("yyyy");
				string month = date.ToString("MM");
				string day = date.ToString("dd");
				string blogtext = "no document";
				string bloghtml = "<p>press</p>";
				title = htmlFormat(title);
				xLogger.Debug("AddPressPage::title:", title, ", Year:", year, ", Month:", month, ", Day:", day);
				_AdminX.AddBlogPress(thoughtspace, topic, year, month, day, title, blogtext, bloghtml);
				xLogger.Debug("pages_press_add:::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_pages_press_add", String.Concat(error_pages_press_add, e.Message)));
			}
		}

		/// <summary></summary>
		private void press_stat() {
			string data_id = UIPage.QueryParam("id", "0");
			string data = UIPage.QueryParam("data", "0");
			XmlNode itemNode = pages_press_get();
			string title = itemNode.SelectSingleNode("//item/@title").InnerText;
			string date = itemNode.SelectSingleNode("//item/@date").InnerText;
			xLogger.Debug("press_stat:data_title:", data_id, "data:", data);
			UserProfile.Add("PressId", data_id);
			UserProfile.Add("PressTitle", title);
			UserProfile.Add("PressDate", date);
		}

		/// <summary>Upload csv file and serve result</summary>
		private void press_upload() {
			try {
				HttpPostedFile thisfile = UIPage.Request.Files.Get(0);
				xLogger.Debug("press_upload:thisfile:", thisfile.FileName);
				string[] filenameArray = thisfile.FileName.Split(new Char[] { '.' });
				int extIndex = filenameArray.Length;
				string fileExt = filenameArray[extIndex-1];
				string blogId = UIPage.UserProfile.Value("PressId");
				XmlNode itemNode = pages_press_get();
				DateTime dateTotal = DateTime.Parse(itemNode.SelectSingleNode("//item/@date").InnerText);
				Logger.Debug(String.Concat("Date before::", itemNode.SelectSingleNode("//item/@date").InnerText));
				string date = dateTotal.ToString("yyyyMMdd");
				Logger.Debug(String.Concat("Date after::", date));
				string document = String.Concat("TheLoerieAwards_", date, "_", blogId , ".", fileExt);
				string filename = String.Concat("Press/", document); 
				filename = UIPage.Server.MapPath(filename);
				xLogger.Debug("press_upload:filename:", filename);
				thisfile.SaveAs(filename);
				string thoughtspace = getThoughtspace(); 
				_AdminX.GetPress(thoughtspace);

				xLogger.Debug("press_upload:ok; XML::", _AdminX.ItemXmlRoot.OuterXml);

				string title = _AdminX.ItemXmlRoot.SelectSingleNode(String.Concat("//blogX/Result/blogs/blog[@id='", blogId, "']/title")).InnerText;
				_AdminX.EditBlogPressContent(thoughtspace, "default", blogId, title, document, "<p>press</p>");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("press_upload:error::", e.Message, ":trace:", e.StackTrace);
				throw (new x_exception("error_press_upload", String.Concat(error_press_upload, e.Message)));
			}
		}

		/// <summary>View press document</summary>
		private void pages_press_view() {
			try {
				XmlNode itemNode = pages_press_get();
				string document = itemNode.SelectSingleNode("//item/@blogtext").InnerText;
				if (document != "no document") {
					string filename = String.Concat("Press/", document);
					xLogger.Debug(" FileName::", filename);
					UIPage.Response.Redirect(filename);
				}
				xLogger.Debug("press_view:ok");

			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("press_upload:error::", e.Message, ":trace:", e.StackTrace);
				throw (new x_exception("error_press_upload", String.Concat(error_press_upload, e.Message)));
			}
		}

		/// <summary>add node</summary>
		private void addNode(XmlNode contentNode, string xpath) {
			XmlNode thoughspaceNode = contentNode.SelectSingleNode(xpath);
			DisplayX.ItemXmlRootNode.SelectSingleNode("//item").AppendChild(DisplayX.ItemXmlRootNode.OwnerDocument.ImportNode(thoughspaceNode, true));
		}

		/// <summary>Add menu</summary>
		private void menu_add() {
			try {
				xLogger.Info("menu_add:");
				string MenuName = UserProfile.Value("MenuName");
				string PageName = UserProfile.Value("Page");
				PageName = format(PageName);
				string Pattern = UserProfile.Value("Pattern");
				string DisplayValue = UserProfile.Value("Display");
				string ConcatId = UserProfile.Value("ConcatId");
				xLogger.Debug("menu_add:MenuName:", MenuName, "; Page:", PageName, "; Pattern:", Pattern, "; Display:", DisplayValue, "; ConcatId:", ConcatId);

				//first add menu
				DisplayX.AddMenu(MenuName, PageName, DisplayValue, Pattern, ConcatId);
				//then check if the menu has a parent in order to add the menuitems below the menu (ie if the concatId is not 0)
				if (ConcatId != "0") {
					//get the menu id with a separate call
					menus_list();
					Logger.Debug(String.Concat("_TheMenuXml::::", DisplayX.ListXmlRoot.OuterXml));
					string menuID= DisplayX.ListXmlRoot.SelectSingleNode(String.Format("//items/item[@ConcatId='{0}']/@menuId", ConcatId)).InnerText;
					Logger.Debug(String.Concat(" MenuitemsAdd:MenuId::", menuID));
					//add menuitems
					menu_menuitemsall("0_record_", menuID);
				} else {//ie this is to be a parent menu
					//get the menu id with a separate call
					menus_list();
					XmlNodeList itemNodes = DisplayX.ListXmlRoot.SelectNodes(Cms.SELECT_ITEMSITEM);
					int menuid = itemNodes.Count;
					string menuID = menuid.ToString();
					Logger.Debug(String.Concat(" THE MENU ID IS::", menuID));
					menu_menuitemsall("0_record_", menuID);
				}
				//profile_users("0_record_", profileID, true);
				xLogger.Debug("menu_add::finished");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("menu_add:error:", e.Message, "::", e.StackTrace);
				throw (new x_exception("error_menu_add", String.Concat(error_menu_add, e.Message)));
			}
		}
		/// <summary>Delete menu</summary>
		private void menu_delete() {
			try {
				xLogger.Info("menu_delete:");
				string menuID = _GetQueryID("menuId");
				//_Access.DeleteGroupUsers(Convert.ToInt32(groupID));
				DisplayX.DeleteMenu(Int32.Parse(menuID));
				////UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Access.ItemXmlRootNode, true));

				xLogger.Debug("menu_delete::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_menu_delete", String.Concat(error_menu_delete, e.Message)));
			}
		}
		/// <summary>Delete page</summary>
		private void pages_delete() {
			try {
				xLogger.Info("page_delete:");
				string pageID = _GetQueryID("PageId");
				//_Access.DeleteGroupUsers(Convert.ToInt32(groupID));
				DisplayX.DeletePage(Int32.Parse(pageID));
				////UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Access.ItemXmlRootNode, true));

				xLogger.Debug("page_delete::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_page_delete", String.Concat(error_page_delete, e.Message)));
			}
		}
		/// <summary>Delete press page</summary>
		private void pages_press_delete() {
			try {
				xLogger.Info("press_page_delete:");
				string blogID = _GetQueryID("id");
				string thoughtspace = getThoughtspace(); 
				_AdminX.DeletePress(blogID, thoughtspace);
				xLogger.Debug("press_page_delete::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_press_page_delete", String.Concat(error_press_page_delete, e.Message)));
			}
		}
		/// <summary>Edit menu and menuitems</summary>
		private void menu_menuitem_edit() {
			try {
				xLogger.Info("menu_edit:");
				string menuID = _GetQueryID("menuId");
				DisplayX.GetMenus(Int32.Parse(menuID));		// Should be GetMenu ?
				string MenuName = UserProfile.Value("MenuName");
				string PageName = UserProfile.Value("Page");
				PageName = format(PageName);
				string Pattern = UserProfile.Value("Pattern");
				string DisplayValue = UserProfile.Value("Display");
				string ConcatId = UserProfile.Value("ConcatId", "-");

				xLogger.Debug("menu_edit:menuID", menuID, ";MenuName:", MenuName, ";PageName:", PageName, ";Pattern:", Pattern, ";DisplayValue:", DisplayValue, ";ConcatId:", ConcatId);

				DisplayX.EditMenu(Int32.Parse(menuID), MenuName, PageName, DisplayValue, Pattern, ConcatId);

				xLogger.Debug("editMenu:NewId xml::", DisplayX.ItemXml.OuterXml);
				string newId = DisplayX.ItemXml.SelectSingleNode("//Result/menuitem/@newId").InnerText;
				
				menu_menuitemsall("0_record_", newId);

				xLogger.Debug("menu_menuitems_edit::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_menu_menuitems_edit", String.Concat(error_menu_menuitems_edit, e.Message)));
			}
		}
		
		#endregion

		#region Private utility methods
		/// <summary>verify the Passport result</summary>
		/// <returns></returns>
		//private bool verify(XmlElement result) {
		//    xLogger.Debug("verify:result", result.OuterXml);
		//    XmlElement rescode = result.SelectSingleNode(SELECT_PASSPORT_RESULT_CODE) as XmlElement;
		//    if (rescode != null) {
		//        return (rescode.InnerText == "0");
		//    } else {
		//        return false;
		//    }
		//}

		private void menu_menuitemsall(string mItemPrefix, string menuID) {
			xLogger.Debug("menu_menuItemsAll:menuItemPrefix:", mItemPrefix, ":menuID:", menuID);
			//first delete and edit
			bool add = menu_del_edit(mItemPrefix, menuID);
			//then add and clear profile
			if (add) {
			    menu_addall(mItemPrefix, menuID);
			} else {
			    for (int i = 0; i < MAX_HANDLE; i++) {
			        string prof = String.Concat(mItemPrefix, i.ToString());
			        string mItemsCSV = UserProfile.Value(prof, "");
			        UserProfile.Clear(prof);
			        if (String.IsNullOrEmpty(mItemsCSV))
			            break;
			    }
			}
			xLogger.Debug("menu_menuItemsAll:finished");
		}
		private bool menu_del_edit(string mItemPrefix, string menuID){
			bool add = false;
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(mItemPrefix, i.ToString());
				string mItemsCSV = UserProfile.Value(prof, "");
				if (String.IsNullOrEmpty(mItemsCSV))
					break;

				string[] mItems = mItemsCSV.Split(new char[] { '|' });	// eg: 10180|zwetsh|Zwelakhe|Tshabangu|0||
				xLogger.Debug("menu_menuItemsAll:", prof, ":", mItemsCSV, "; menuID:", menuID);
				string mItemId = mItems[1];
				string page = mItems[2];
				string mItemName = mItems[4];
				string displayValue = mItems[5];
				if (_IsDefault(displayValue) || displayValue == "false" || displayValue == "0")
					displayValue = "no";
				string delete = mItems[6];
				xLogger.Debug("menu_menuItemsAll: mItemId", mItemId, "; mItemName:", mItemName, "; page:", page, "; displayValue:", displayValue, "; delete:", delete);

				if (delete == "true" || delete == "1") {	// true when clicked, 1 when exists
					xLogger.Debug("menu_menuItemsAll:deleting...", mItemName);
					DisplayX.DeleteMenuItem(Int32.Parse(menuID), Int32.Parse(mItemId));
				} else if (_IsDefault(mItemId)) {
					add = true;
				} else {
					xLogger.Debug("menu_menuItemsAll:edditing...", mItemName);
					DisplayX.EditMenuItem(Int32.Parse(menuID), Int32.Parse(mItemId), mItemName, page, displayValue);
				}
			}
			return add;
		}
		private void menu_addall(string mItemPrefix, string menuID) {
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(mItemPrefix, i.ToString());
				string mItemsCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);
				if (String.IsNullOrEmpty(mItemsCSV))
					break;
				string[] mItems = mItemsCSV.Split(new char[] { '|' });
				string mItemId = mItems[1];
				if (_IsDefault(mItemId)) {
					string mItemName = mItems[4];
					string page = mItems[2];
					page = format(page);
					string displayValue = mItems[5];
					if (_IsDefault(displayValue)) {
						displayValue = "no";
					}
					xLogger.Debug("menu_menuItemsAll:adding...", mItemName);
					DisplayX.AddMenuItem(Int32.Parse(menuID), mItemName, page, displayValue);
				}
			}
		}

		private string format(string format) {
			Regex rgx = new Regex(@"\d");
			format = Regex.Replace(format, @"[^a-zA-Z0-9]", "_");
			format = format.ToLower();
			return format;
		}

		private string htmlFormat(string title) {
			if (title.Contains("&") == true) {
				title = title.Replace("&", "&amp;");
			}
			return title;
		}
		#endregion
	}
}