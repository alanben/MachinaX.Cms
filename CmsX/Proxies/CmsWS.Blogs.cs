using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-11-15
	Status:		release	
	Version:	4.0.2
	Build:		20140120
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20101115:	Started.
	20140120:	Changed constructor to take CmsXProfileX (was displayX)
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
	/// <summary>This class is a functional wrapper of the DisplayXBlogs web service</summary>
	public class Blogs : CmsWSSite {

		#region Constant name strings
		private const string logid = "Blogs.";
		private const string CONFIG_USER_ID = "blogs_service";
		private const string ROOT_NAME = "Blogs";
		#endregion

		#region Visible properties
		private ContentXBlogsWS.ContentXBlogs blogsService;
		public ContentXBlogsWS.ContentXBlogs BlogsService {
			get { return blogsService; }
		}
		public string ServiceConfigID { get; set; }
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public Blogs(CmsXProfileX thispage, string blogx_id) : base(thispage, ROOT_NAME, blogx_id, typeof(Blogs), logid) {
			ServiceConfigID = CONFIG_USER_ID;
			initialize();
		}
		public Blogs(CmsXProfileX thispage, string blogx_id, string service_configID) : base(thispage, ROOT_NAME, blogx_id, typeof(Blogs), logid) {
			ServiceConfigID = service_configID;
			initialize();
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize() {
			xLogger.Debug("initialize::");
			blogsService = new ContentXBlogsWS.ContentXBlogs();
			blogsService.Url = _GetUrl(ServiceConfigID);
			xLogger.Debug("initialize:blogsService:", blogsService.Url);
		}

		#endregion

		#region Public List Methods
		/// <summary>
		/// Get a list of all blogs
		/// </summary>
		public void List(string Space, string Topic, SearchSettings Settings) {
			xLogger.Debug("List", "::BlogxID:", BlogxID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							blogsService.List(Space, Topic, Settings.Column, !Settings.Descending)
						   )
					, true)
				);
		}
		#endregion

		#region Public Blog Methods
		public void GetBlog(string Space, string Topic, string BlogID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.Get(Space, Topic, BlogID))
					, true)
				);
		}

		public void AddBlog(string Space, string Topic, string Author, string Title, string Html) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.Add(Space, Topic, Author, Title, Html))
					, true)
				);
		}

		public void UpdateBlog(string Space, string Topic, string BlogID, string Title, string Html) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.Edit(Space, Topic, BlogID, Title, Html))
					, true)
				);
		}

		public void UpdateBlogTitle(string Space, string Topic, string BlogID, string Title) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.EditTitle(Space, Topic, BlogID, Title))
					, true)
				);
		}

		public void DeleteBlog(string Space, string Topic, string BlogID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.Delete(Space, Topic, BlogID))
					, true)
				);
		}
		#endregion

		#region Public News Blog Methods
		public void AddNewsBlog(string Space, string Topic, string Author, string Year, string Month, string Day, string Title, string Text, string Html) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.AddNews(Space, Topic, Author, Year, Month, Day, Title, Text, Html))
					, true)
				);
		}

		public void UpdateNewsBlog(string Space, string Topic, string BlogID, string Year, string Month, string Day, string Title, string Text, string Html) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.EditNews(Space, Topic, BlogID, Year, Month, Day, Title, Text, Html))
					, true)
				);
		}

		public void UpdateNews(string Space, string Topic, string BlogID, string Year, string Month, string Day, string Title) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.EditDateTitle(Space, Topic, BlogID, Year, Month, Day, Title))
					, true)
				);
		}
		#endregion

		#region Public Comment Methods
		public void GetBlogComments(string Space, string Topic, string BlogID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.GetComments(Space, Topic, BlogID))
					, true)
				);
		}

		public void GetBlogComment(string Space, string Topic, string BlogID, string CommentID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.GetComment(Space, Topic, BlogID, CommentID))
					, true)
				);
		}

		public void DeleteBlogComment(string Space, string Topic, string BlogID, string CommentID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.DeleteComment(Space, Topic, BlogID, CommentID))
					, true)
				);
		}

		public void AddBlogComment(string Space, string Topic, string BlogID, string Username, string Author, string Title, string Comment, bool Approve) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.AddComment(Space, Topic, BlogID, Username, Author, Title, Comment, Approve))
					, true)
				);
		}

		public void UpdateBlogComment(string Space, string Topic, string BlogID, string CommentID, string Title, string Comment, bool Approve) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.EditComment(Space, Topic, BlogID, CommentID, Title, Comment, Approve))
					, true)
				);
		}
		#endregion

		#region Public History Methods
		public void ListBlogHistory(string Space, string Topic, string BlogID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.ListHistory(Space, Topic, BlogID))
					, true)
				);
		}

		public void GetBlogHistory(string Space, string Topic, string BlogID, string HistoryID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.GetHistory(Space, Topic, BlogID, HistoryID))
					, true)
				);
		}

		public void RestoreBlogHistory(string Space, string Topic, string BlogID, string HistoryID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.RestoreHistory(Space, Topic, BlogID, HistoryID))
					, true)
				);
		}

		public void DeleteBlogHistory(string Space, string Topic, string BlogID, string HistoryID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(blogsService.DeleteHistory(Space, Topic, BlogID, HistoryID))
					, true)
				);
		}
		#endregion
	}
}

