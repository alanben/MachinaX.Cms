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
	20101227:	Started.
	20140120:	Changed constructor to take CmsXProfileX (was displayX)
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>This class is a functional wrapper of the DisplayXMedia web service</summary>
	public class Media : CmsWSSite {
		#region Private properties
		#endregion

		#region Constant name strings
		private const string logid = "Media.";
		private const string CONFIG_USER_ID = "media_service";
		private const string ROOT_NAME = "Media";
		#endregion

		#region Visible properties
		private ContentXMediaWS.ContentXMedia mediaService;
		public ContentXMediaWS.ContentXMedia MediaService {
			get { return mediaService; }
		}
		#endregion

		#region Constant error strings
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public Media(CmsXProfileX thispage, string blogx_id) : base(thispage, ROOT_NAME, blogx_id, typeof(Media), logid) {
			initialize();
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize() {
			xLogger.Debug("initialize::");
			mediaService = new ContentXMediaWS.ContentXMedia();
			mediaService.Url = _GetUrl(CONFIG_USER_ID);
			xLogger.Debug("initialize:mediaService:", mediaService.Url);
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Get a list of all media
		/// </summary>
		public void List(string MediaType, SearchSettings Settings) {
			xLogger.Debug("List", "::MediaType:", MediaType);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							mediaService.List(BlogxID, MediaType, Settings.Column, !Settings.Descending)
						   )
					, true)
				);
		}

		public void GetMedia(string MediaID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(mediaService.Get(BlogxID, MediaID))
					, true)
				);
		}

		public void AddMedia(string MediaID, string Description, string Type, string SubType, string Url) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(mediaService.Add(BlogxID, MediaID, Description, Type, SubType, Url))
					, true)
				);
		}

		public void AddMedia(string MediaID, string Description, string Filename, byte[] File) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(mediaService.NewFile(BlogxID, MediaID, Description, Filename, File), true, true)
					, true)
				);
		}

		public void UpdateMedia(string MediaID, string Description, string Type, string SubType, string Url) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(mediaService.Edit(BlogxID, MediaID, Description, Type, SubType, Url))
					, true)
				);
		}

		public void UpdateMedia(string MediaID, string Description, string Filename, byte[] File) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(mediaService.File(BlogxID, MediaID, Description, Filename, File))
					, true)
				);
		}

		public void DeleteMedia(string MediaID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(mediaService.Delete(BlogxID, MediaID))
					, true)
				);
		}
		#endregion
	}
}

