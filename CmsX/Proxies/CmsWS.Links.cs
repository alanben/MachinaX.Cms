using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-10-20
	Status:		release	
	Version:	4.0.2
	Build:		20140120
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20101020:	Started.
	20101101:	Modified AddLinkBlog / UpdateLinkBlog
	20101104:	Modified AddLinkPage / UpdateLinkPage
	20110429:	Added LinkLinks methods
	20140120:	Changed constructor to take CmsXProfileX (was displayX)
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>This class is a functional wrapper of the DisplayXLinks web service</summary>
	public class Links : CmsWSSite {

		private const string logid = "Links.";
		private const string CONFIG_SERVICE_ID = "links_service";
		private const string ROOT_NAME = "Links";

		private DisplayXLinksWS.DisplayXLinks linksService;
		public DisplayXLinksWS.DisplayXLinks LinksService {
			get { 
				if (linksService == null) {
					linksService = new DisplayXLinksWS.DisplayXLinks();
					linksService.Url = _GetUrl(CONFIG_SERVICE_ID);
				}
				return linksService; }
		}

		/// <summary>
		/// The 'blogspace' of an Item
		/// </summary>
		public string BlogSpace {
			get { return Item.GetAttribute("blogspace"); }
		}

		/// <summary>Default constructor</summary>
		public Links(CmsXProfileX thispage, string blogx_id) : base(thispage, ROOT_NAME, blogx_id, typeof(Links), logid) {
		}

		#region Link Methods
		/// <summary>
		/// Get a list of all links
		/// </summary>
		public void List() {
			xLogger.Debug("List", "::BlogxID:", BlogxID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							LinksService.ListDD(BlogxID)
						   )
					, true)
				);
		}

		/// <summary>
		/// Get a list of all links
		/// </summary>
		/// <param name="Settings">Page/Order settings</param>
		public void ListAll(string Group, SearchSettings Settings) {
			xLogger.Debug("ListAll", "::BlogxID:", BlogxID, "::Group:", Group, "::Column:", Settings.Column, "::Descending:", Settings.Descending.ToString(), "::Number:", Settings.Number.ToString());

			_WriteList(LinksService.ListAll(BlogxID, Group, Settings.Column, !Settings.Descending));
		}

		/// <summary>
		/// Get a list of all links
		/// </summary>
		/// <param name="Settings">Page/Order settings</param>
		public void List(string Group, SearchSettings Settings) {
			xLogger.Debug("List", "::BlogxID:", BlogxID, "::Group:", Group, "::Column:", Settings.Column, "::Descending:", Settings.Descending.ToString(), "::Number:", Settings.Number.ToString(), "::Page:", Settings.Page.ToString(), "::Rows:", Settings.Rows.ToString());
			//ListXmlRoot.RemoveAll();
			//ListXmlRoot.AppendChild(
			//		ListXml.ImportNode(
			//			_CheckWSResult(
			//				LinksService.List(BlogxID, Group, Settings.Column, !Settings.Descending, Settings.Number, Settings.Page, Settings.Rows)
			//			   )
			//		, true)
			//	);

			_WriteList(LinksService.ListAll(BlogxID, Group, Settings.Column, !Settings.Descending));
		}

		/// <summary>
		/// Get a list of all links by search
		/// </summary>
		public void Search(SearchSettings Settings, int GroupSrchType, string Group, int NameSrchType, string Name, int PromptSrchType, string Prompt) {
			xLogger.Debug("Search", "::BlogxID:", BlogxID, "::Group:", Group, "::Name:", Name, "::Prompt:", Prompt);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							LinksService.Search(BlogxID, GroupSrchType, Group, NameSrchType, Name, PromptSrchType, Prompt, Settings.Column, !Settings.Descending, Settings.Number, Settings.Page, Settings.Rows)
							)
						, true)
				);
		}

		/// <summary>
		/// Get a list of all link groups
		/// </summary>
		public void ListGroups() {
			xLogger.Debug("ListGroups", "::BlogxID:", BlogxID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							LinksService.ListGroups(BlogxID)
						   )
					, true)
				);
		}

		public void GetLink(int LinkID) {
			xLogger.Debug("GetLink", "::BlogxID:", BlogxID, "::LinkID:", LinkID.ToString());
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.Get(BlogxID, LinkID))
					, true)
				);
		}

		public void DeleteLink(int LinkID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.Delete(BlogxID, LinkID))
					, true)
				);
		}

		public void AddLink(string Name, string Link, string Prompt, string Broker, string Group, string Display, string Secure, string Type, string Tags, string Track, string Url) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.Add(BlogxID, Name, Link, Prompt, Broker, Group, Display, Secure, Type, Tags, Track, Url))
					, true)
				);
		}

		public void UpdateLink(int LinkID, string Name, string Link, string Prompt, string Broker, string Group, string Display, string Secure, string Type, string Tags, string Track, string Url) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.Edit(BlogxID, LinkID, Name, Link, Prompt, Broker, Group, Display, Secure, Type, Tags, Track, Url))
					, true)
				);
		}
		#endregion

		#region LinkPage Methods
		/// <summary>
		/// Get a list of all link pages
		/// </summary>
		public void AddLinkPage(int LinkID, string Section, string Subsect, string Process, string File) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.AddPage(BlogxID, LinkID, Section, Subsect, Process, File))
					, true)
				);
		}

		public void UpdateLinkPage(int LinkID, string Section, string Subsect, string Process, string File) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.EditPage(BlogxID, LinkID, Section, Subsect, Process, File))
					, true)
				);
		}
		#endregion

		#region LinkBlog Methods
		public void GetLinkBlogs(int LinkID) {
			xLogger.Debug("Search", "::BlogxID:", BlogxID, "::LinkID:", LinkID.ToString());
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.GetBlogs(BlogxID, LinkID))
					, true)
				);
		}

		public void DeleteLinkBlog(int LinkID, string Space, string Topic, string Blog) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.DeleteBlog(BlogxID, LinkID, Space, Topic, Blog))
					, true)
				);
		}

		public void AddLinkBlog(int LinkID, string Space, string Topic, string Blog, string Position, string TitleFlag) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.AddBlog(BlogxID, LinkID, Space, Topic, Blog, Position, TitleFlag))
					, true)
				);
		}

		public void UpdateLinkBlog(int LinkID, string Space, string Topic, string Blog, string Position, string TitleFlag) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.EditBlog(BlogxID, LinkID, Space, Topic, Blog, Position, TitleFlag))
					, true)
				);
		}
		#endregion

		#region Public LinkFlags Methods
		/// <summary>
		/// Get a list of all link flags
		/// </summary>
		public void ListFlags() {
			xLogger.Debug("ListFlags", "::BlogxID:", BlogxID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							LinksService.ListFlags(BlogxID)
						   )
					, true)
				);
		}
		/// <summary>
		/// Get the link's flags
		/// </summary>
		public void GetLinkFlags(int LinkID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.GetFlags(BlogxID, LinkID))
					, true)
				);
		}

		public void DeleteLinkFlag(int LinkID, string Flag) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.DeleteFlag(BlogxID, LinkID, Flag))
					, true)
				);
		}

		public void AddLinkFlag(int LinkID, string Flag, string Value) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.AddFlag(BlogxID, LinkID, Flag, Value))
					, true)
				);
		}

		public void UpdateLinkFlag(int LinkID, string Flag, string Value) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.EditFlag(BlogxID, LinkID, Flag, Value))
					, true)
				);
		}
		#endregion

		#region Public LinkLinks Methods
		/// <summary>
		/// Get a list of all link links
		/// </summary>
		public void ListLinks() {
			xLogger.Debug("ListLinks", "::BlogxID:", BlogxID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							LinksService.ListLinks(BlogxID)
						   )
					, true)
				);
		}
		/// <summary>
		/// Get the link's links
		/// </summary>
		public void GetLinkLinks(int LinkID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.GetLinkLinks(BlogxID, LinkID))
					, true)
				);
		}

		public void DeleteLinkLink(int LinkID, string Link) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.DeleteLinkLink(BlogxID, LinkID, Link))
					, true)
				);
		}

		public void AddLinkLink(int LinkID, string Link, string Value) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.AddLinkLink(BlogxID, LinkID, Link, Value))
					, true)
				);
		}

		public void UpdateLinkLink(int LinkID, string Link, string Value) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.EditLinkLink(BlogxID, LinkID, Link, Value))
					, true)
				);
		}
		#endregion

		#region Public LinkFlags Methods
		/// <summary>
		/// Get a list of all link files
		/// </summary>
		public void ListFiles() {
			xLogger.Debug("ListFiles", "::BlogxID:", BlogxID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							LinksService.ListFilesDD(BlogxID)
						   )
					, true)
				);
		}
		/// <summary>
		/// Get a list of all link files
		/// </summary>
		public void ListLinkFiles() {
			xLogger.Debug("ListLinkFiles", "::BlogxID:", BlogxID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							LinksService.ListFiles(BlogxID)
						   )
					, true)
				);
		}
		/// <summary>
		/// Get the link's files
		/// </summary>
		public void GetLinkFiles(int LinkID) {
			xLogger.Debug("ListFiles", "::BlogxID:", BlogxID, "::LinkID:", LinkID.ToString());
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.GetFiles(BlogxID, LinkID))
					, true)
				);
		}

		public void DeleteLinkFile(int LinkID, string FileID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.DeleteFile(BlogxID, LinkID, FileID))
					, true)
				);
		}

		public void AddLinkFile(int LinkID, string Section, string SubSection, string Process, string File, string Position) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.AddFile(BlogxID, LinkID, Section, SubSection, Process, File, Position))
					, true)
				);
		}

		public void AddLinkFile(int LinkID, string FileID, string Position) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.AddFileID(BlogxID, LinkID, FileID, Position))
					, true)
				);
		}

		public void UpdateLinkFile(int LinkID, string FileID, string Section, string SubSection, string Process, string File, string Position) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.EditFile(BlogxID, LinkID, FileID, Section, SubSection, Process, File, Position))
					, true)
				);
		}

		public void GetLinkFile(int LinkID, string FileID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.GetFile(BlogxID, LinkID, FileID))
					, true)
				);
		}

		public void GetLinkFileXml(int LinkID, string FileID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.GetFileXml(BlogxID, LinkID, FileID))
					, true)
				);
		}

		public void SaveLinkFileXml(int LinkID, string FileID, string FileXml) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(LinksService.SaveFileXml(BlogxID, LinkID, FileID, FileXml))
					, true)
				);
		}
		#endregion
	}
}

