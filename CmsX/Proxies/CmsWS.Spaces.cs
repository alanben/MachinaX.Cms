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
	20101218:	Started.
	20140120:	Changed constructor to take CmsXProfileX (was displayX)
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>This class is a functional wrapper of the DisplayXSpaces web service</summary>
	public class Spaces : CmsWSSite {
		#region Private properties
		#endregion

		#region Constant name strings
		private const string logid = "Spaces.";
		private const string CONFIG_USER_ID = "spaces_service";
		private const string ROOT_NAME = "Spaces";
		#endregion

		#region Visible properties
		private ContentXSpacesWS.ContentXSpaces spacesService;
		public ContentXSpacesWS.ContentXSpaces SpacesService {
			get { return spacesService; }
		}
		#endregion

		#region Constant error strings
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public Spaces(CmsXProfileX thispage, string blogx_id) : base(thispage, ROOT_NAME, blogx_id, typeof(Spaces), logid) {
			initialize();
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize() {
			xLogger.Debug("initialize::");
			spacesService = new ContentXSpacesWS.ContentXSpaces();
			spacesService.Url = _GetUrl(CONFIG_USER_ID);
			xLogger.Debug("initialize:spacesService:", spacesService.Url);
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Get a list of all spaces
		/// </summary>
		public void List(string Type, SearchSettings Settings) {
			xLogger.Debug("List", "::BlogxID:", BlogxID, "::Type:", Type, "::Column:", Settings.Column, "::Descending:", Settings.Descending.ToString());
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							spacesService.List(BlogxID, Type, Settings.Column, !Settings.Descending)
						   )
					, true)
				);
		}

		public void GetSpace(string SpaceID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(spacesService.Get(BlogxID, SpaceID))
					, true)
				);
		}

		public void AddSpace(string Name, string Label, string Desc, string Type, string Active, string Author, string Title, string Html) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(spacesService.Add(BlogxID, Name, Label, Desc, Type, Active, Author, Title, Html))
					, true)
				);
		}

		public void UpdateSpace(string Name, string Label, string Desc, string Type, string Active, string Author, string Title, string Html) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(spacesService.Edit(BlogxID, Name, Label, Desc, Type, Active, Author, Title, Html))
					, true)
				);
		}

		public void UpdateSpaceBlog(string Name, string Author, string Title, string Html) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(spacesService.EditBlog(BlogxID, Name, Author, Title, Html))
					, true)
				);
		}

		public void DeleteSpace(string Name) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(spacesService.Delete(BlogxID, Name))
					, true)
				);
		}

		#endregion

		#region Public Topic Methods
		public void ListSpacesTopics() {
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							spacesService.ListSpacesTopics(BlogxID)
						   )
					, true)
				);
		}

		public void GetSpaceTopics(string SpaceID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(spacesService.GetTopics(BlogxID, SpaceID))
					, true)
				);
		}

		public void GetSpaceTopic(string SpaceID, string Topic) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(spacesService.GetTopic(BlogxID, SpaceID, Topic))
					, true)
				);
		}

		public void DeleteSpaceTopic(string SpaceID, string Topic) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(spacesService.DeleteTopic(BlogxID, SpaceID, Topic))
					, true)
				);
		}

		public void AddSpaceTopic(string SpaceID, string Topic, string Label, string Author, string Title) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(spacesService.AddTopic(BlogxID, SpaceID, Topic, Label, Author, Title))
					, true)
				);
		}

		public void UpdateSpaceTopic(string SpaceID, string Topic, string Label, string Title) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(spacesService.EditTopic(BlogxID, SpaceID, Topic, Label, Title))
					, true)
				);
		}

		public void UpdateSpaceTopicBlog(string SpaceID, string Topic, string Author, string Title, string Html) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(spacesService.EditTopicBlog(BlogxID, SpaceID, Topic, Author, Title, Html))
					, true)
				);
		}
		#endregion
	}
}

