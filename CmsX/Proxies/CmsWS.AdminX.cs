using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	2.5.0
	Build:		20100927
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Refactored from LoeriesAdmin
	20100927:	Renamed to AdminX from Thoughtspacex
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>
	/// This class is a functional wrapper of the AdminX web service
	/// </summary>
	public class AdminX : CmsWSBase {
		#region Private properties
		private string token;
		private string archive = "";
		private x_config config;
		#endregion

		#region Constant name strings
		private const string logid = "AdminX.";
		private const string CONFIG_SERVICE_ID = "adminx_service";
		private const string ROOT_NAME = "AdminX";
		#endregion

		#region Visible properties
		private AdminXWS.adminX adminxWS;
		public AdminXWS.adminX AdminxWS {
			get { return adminxWS; }
		}

		protected string Token { 
			get { return String.IsNullOrEmpty(token)? login() : token;
			}
		}
		#endregion

		#region Constant error strings
		private const string ERROR_WS_RESULT = "Web service error : ";
		#endregion

		#region Constructors/Destructors
		///// <summary>Default constructor</summary>
		//public AdminX(displayX thispage, string award_id) : base(thispage, ROOT_NAME, award_id) {
		//    initialize();
		//}
		/// <summary>Default constructor</summary>
		public AdminX(displayX thispage) : base(thispage, ROOT_NAME, typeof(AdminX), logid) {
			initialize();
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize() {
			xLogger.Debug("initialize");
			config = new x_config();
			
			adminxWS = new AdminXWS.adminX();
			adminxWS.Url = _GetUrl(CONFIG_SERVICE_ID);
			xLogger.Debug("initialize", "::adminxWS:", adminxWS.Url);

			//login();
			xLogger.Debug("initialize::ok:");
		}

		#endregion

		#region Public blog Methods
		public XmlNode GetBlog(string thoughtspace, string topic, string blog) {
			XmlNode content = _CheckWSResult(adminxWS.GetBlog(Token, thoughtspace, topic, archive, blog));
			return content;
		}
		public void EditBlogHtml(string thoughtspace, string topic, string blog, string title, string bloghtml) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.EditHtmlBlog(Token, thoughtspace, topic, blog, title, bloghtml))
					, true)
				);
		}
		public void EditBlogTitle(string thoughtspace, string topic, string id, string title) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.EditBlogTitle(Token, thoughtspace, topic, id, title))
					, true)
				);
		}
		public void AddBlogHtml(string thoughtspace, string topic, string title, string bloghtml) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.AddHtmlBlog(Token, thoughtspace, topic, title, bloghtml))
					, true)
				);
		}
		public void DeleteBlog(string thoughtspace, string topic, string blogId) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.DeleteBlog(Token, thoughtspace, topic, blogId))
					, true)
				);
		}
		public void AddBlogPress(string thoughtspace, string topic, string year, string month, string day, string title, string blogtext, string bloghtml) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.AddHtmlBlogDate(Token, thoughtspace, topic, year, month, day, title, blogtext, bloghtml))
					, true)
				);
		}
		public void EditBlogPressContent(string thoughtspace, string topic, string id, string title, string blogtext, string bloghtml) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.EditHtmlBlogDate(Token, thoughtspace, topic, id, title, blogtext, bloghtml))
					, true)
				);
		}
		public void EditBlogPress(string thoughtspace, string topic, string id, string title, string year, string month, string day) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.EditBlogTitleDate(Token, thoughtspace, topic, id, title, year, month, day))
					, true)
				);
		}
		public void GetPress(string thoughtspace) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.ListBlogs(Token, thoughtspace, "default", archive))
					, true)
				);
		}
		public void DeletePress(string blogId, string thoughtspace) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.DeleteBlog(Token, thoughtspace, "default", blogId))
					, true)
				);
		}
		public void GetPressPages(SearchSettings Settings, bool IsDropDown, string thoughtspace) {
			Logger.Debug(String.Concat("_ListPressPages:settings:", Settings.Page.ToString(), ":", Settings.Column, ":", Settings.Descending.ToString(), ":", Settings.Rows.ToString()));
			XmlNode result;
			result = adminxWS.ListBlogs(Token, thoughtspace, "default", archive);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(ListXml.ImportNode(_CheckWSResult(result), true));
		}

		#endregion

		#region Public space Methods
		public void GetThoughtspaces() {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.ListSpaces(Token))
					, true)
				);
		}
		public void ListSpaces() {
			//ItemXmlRoot.RemoveAll();
			//XmlNode reslt = _CheckWSResult(adminxWS.ListSpaces(Token), true);
			//xLogger.Debug("ListSpaces::reslt:", reslt.OuterXml);
			
			//StringWriter sw = new StringWriter();
			//XslCompiledTransform xsltrans = new XslCompiledTransform();
			//xsltrans.Load(UIPage.Server.MapPath("/templates/AdminX.xsl"));
			//xsltrans.Transform(reslt, null, sw);
			//xLogger.Debug("ListSpaces::sw:", sw.ToString());

			//ListXmlRootNode.InnerXml = sw.ToString();

			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
				   ListXml.ImportNode(
						_CheckAdminXResult(adminxWS.ListSpaces(Token))
					, true)
				);
		}
		public void GetSpace(string space) {
			ItemXmlRoot.RemoveAll();
			XmlNode reslt = adminxWS.GetSpace(Token, space);
			xLogger.Debug("GetSpace::reslt:", reslt.OuterXml);
			ItemXmlRoot.AppendChild(ItemXml.ImportNode(_CheckAdminXResult(reslt), true));
		}
		public void AddSpace(string space, string type, string title, string text, string admin, string wanttopics) {
			ItemXmlRoot.RemoveAll();
			XmlNode reslt = adminxWS.AddSpace(Token, space, type, title, text, admin, wanttopics);
			xLogger.Debug("AddSpace::reslt:", reslt.OuterXml);
			ItemXmlRoot.AppendChild(ItemXml.ImportNode(_CheckAdminXResult(reslt), true));
		}
		public void AddSpaceBlog(string space, string author, string title, string image, string html) {
			ItemXmlRoot.RemoveAll();
			XmlNode reslt = adminxWS.AddSpaceBlog(Token, space, author, title, image, html);
			xLogger.Debug("AddSpaceBlog::reslt:", reslt.OuterXml);
			ItemXmlRoot.AppendChild(ItemXml.ImportNode(_CheckAdminXResult(reslt), true));
		}
		public void DeleteSpace(string space) {
			ItemXmlRoot.RemoveAll();
			XmlNode reslt = adminxWS.RemoveSpace(Token, space);
			xLogger.Debug("RemoveSpace::reslt:", reslt.OuterXml);
			ItemXmlRoot.AppendChild(ItemXml.ImportNode(_CheckAdminXResult(reslt), true));
		}
		public void EditSpace(string space, string type, string title, string text, string wanttopics) {
			ItemXmlRoot.RemoveAll();
			XmlNode reslt = adminxWS.EditSpace(Token, space, type, title, text, wanttopics);
			xLogger.Debug("RemoveSpace::reslt:", reslt.OuterXml);
			ItemXmlRoot.AppendChild(ItemXml.ImportNode(_CheckAdminXResult(reslt), true));
		}
		public void UpdateSpace(string space, string type, string title, string text, string author, string image, string blogtitle, string bloghtml) {
			ItemXmlRoot.RemoveAll();
			XmlNode reslt = adminxWS.UpdateSpace(Token, space, type, title, text, author, image, blogtitle, bloghtml);
			xLogger.Debug("UpdateSpace::reslt:", reslt.OuterXml);
			ItemXmlRoot.AppendChild(ItemXml.ImportNode(_CheckAdminXResult(reslt), true));
		}
		#endregion

		#region Public topic Methods
		public void GetTopics(string space) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.GetSpace(Token, space))
					, true)
				);
		}
		public void ListTopics(string space) {
			ItemXmlRoot.RemoveAll();
			XmlNode reslt = adminxWS.ListTopics(Token, space);
			xLogger.Debug("ListTopics::reslt:", reslt.OuterXml);
			ListXmlRoot.AppendChild(ListXml.ImportNode(_CheckAdminXResult(reslt), true));
		}
		public void DeleteTopic(string space, string topic) {
			ItemXmlRoot.RemoveAll();
			XmlNode reslt = adminxWS.DeleteTopic(Token, space, topic);
			xLogger.Debug("DeleteTopic::reslt:", reslt.OuterXml);
			ItemXmlRoot.AppendChild(ItemXml.ImportNode(_CheckAdminXResult(reslt), true));
		}
		public void EditTopic(string space, string topic, string title) {
			ItemXmlRoot.RemoveAll();
			XmlNode reslt = adminxWS.EditTopic(Token, space, topic, title, null);
			xLogger.Debug("EditTopic::reslt:", reslt.OuterXml);
			ItemXmlRoot.AppendChild(ItemXml.ImportNode(_CheckAdminXResult(reslt), true));
		}
		public void AddTopic(string space, string topic, string title) {
			ItemXmlRoot.RemoveAll();
			XmlNode reslt = adminxWS.AddTopic(Token, space, topic, title, "html");
			xLogger.Debug("EditTopic::reslt:", reslt.OuterXml);
			ItemXmlRoot.AppendChild(ItemXml.ImportNode(_CheckAdminXResult(reslt), true));
		}
		#endregion

		#region Public Authentication AdminX Methods
		public void Login(string username, string password) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.Login(username, password), false)
					, true)
				);
		}
		public void Validate(string Token) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.ValidateToken(Token), false)
					, true)
				);
		}
		public void Logout(string Token) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.Logout(Token), false)
					, true)
				);
		}
		public void Rights(string Token) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(adminxWS.GetRights(Token), false)
					, true)
				);
		}
		#endregion

		#region Private AdminX Methods
		private string login() {
			string username = config.Value(String.Concat(Cms.CONFIG_ROOT, "/Url[@id='", CONFIG_SERVICE_ID, "']/@username"));
			string password = config.Value(String.Concat(Cms.CONFIG_ROOT, "/Url[@id='", CONFIG_SERVICE_ID, "']/@password"));
			xLogger.Debug("login", "::Username:", username, "::Password:", password);
			XmlNode admin = adminxWS.Login(username, password);
			xLogger.Debug("login", "::admin:", admin.OuterXml);
			token = admin.SelectSingleNode("//Result/user/@token").InnerText;
			xLogger.Debug("login", "::token:", token);
			return token;
		}
		#endregion

	}
}

