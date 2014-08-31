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

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>This class is a functional wrapper of the DisplayXFiles web service</summary>
	public class Files : CmsWSSite {	// or CmsWSBase? ie no BlogxID
		#region Private properties
		#endregion

		#region Constant name strings
		private const string logid = "Files.";
		private const string CONFIG_USER_ID = "files_service";
		private const string ROOT_NAME = "Files";
		#endregion

		#region Visible properties
		private ContentXFilesWS.ContentXFiles filesService;
		public ContentXFilesWS.ContentXFiles FilesService {
			get { return filesService; }
		}
		#endregion

		#region Constant error strings
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public Files(CmsXProfileX thispage, string blogx_id) : base(thispage, ROOT_NAME, blogx_id, typeof(Files), logid) {
			initialize();
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize() {
			xLogger.Debug("initialize::");
			filesService = new ContentXFilesWS.ContentXFiles();
			filesService.Url = _GetUrl(CONFIG_USER_ID);
			xLogger.Debug("initialize:filesService:", filesService.Url);
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Get a list of all files
		/// </summary>
		public void ListFiles(string StartPath, string Filter, SearchSettings Settings) {
			xLogger.Debug("List", "::BlogxID:", BlogxID, "::StartPath:", StartPath, "::Filter:", Filter, "::Column:", Settings.Column, "::Descending:", Settings.Descending.ToString());
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							filesService.List(BlogxID, StartPath, Filter, Settings.Column, !Settings.Descending)
						   )
					, true)
				);
		}

		/// <summary>
		/// Get a list of all files
		/// </summary>
		public void ListFilesDrop(string StartPath, string Filter) {
			xLogger.Debug("List", "::BlogxID:", BlogxID, "::StartPath:", StartPath, "::Filter:", Filter);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							filesService.ListDrop(BlogxID, StartPath, Filter)
						   )
					, true)
				);
		}

		/// <summary>
		/// Get a list of all file paths
		/// </summary>
		public void ListPaths(string StartPath, SearchSettings Settings) {
			xLogger.Debug("List", "::BlogxID:", BlogxID, "::StartPath:", StartPath, "::Column:", Settings.Column, "::Descending:", Settings.Descending.ToString());
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							filesService.ListPaths(BlogxID, StartPath, Settings.Column, !Settings.Descending)
						   )
					, true)
				);
		}

		/// <summary>
		/// Get a list of all file paths
		/// </summary>
		public void ListPaths(string StartPath) {
			xLogger.Debug("List", "::BlogxID:", BlogxID, "::StartPath:", StartPath);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							filesService.ListPaths(BlogxID, StartPath, "desc", true)
						   )
					, true)
				);
		}

		/// <summary>
		/// Get a list of all file extensions
		/// </summary>
		public void ListExtensions(string StartPath, SearchSettings Settings) {
			xLogger.Debug("List", "::BlogxID:", BlogxID, "::StartPath:", StartPath, "::Column:", Settings.Column, "::Descending:", Settings.Descending.ToString());
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							filesService.ListExtensions(BlogxID, StartPath, Settings.Column, !Settings.Descending)
						   )
					, true)
				);
		}

		/// <summary>
		/// Get a list of all file extensions
		/// </summary>
		public void ListExtensions(string StartPath) {
			xLogger.Debug("List", "::BlogxID:", BlogxID, "::StartPath:", StartPath);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							filesService.ListExtensions(BlogxID, StartPath, "desc", true)
						   )
					, true)
				);
		}

		public void GetFile(string StartPath, string FileID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
					ItemXml.ImportNode(
						_CheckWSResult(filesService.Get(BlogxID, StartPath, FileID))
					, true)
				);
		}

		public void AddFile(string StartPath, string FileName, string FileExtn, string FilePath, byte[] FileData) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
					ItemXml.ImportNode(
						_CheckWSResult(filesService.Add(BlogxID, StartPath, FileName, FileExtn, FilePath, FileData))
					, true)
				);
		}

		public void UpdateFile(string StartPath, string FileID, string FileName, string FileExtn, string FilePath, byte[] FileData) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
					ItemXml.ImportNode(
						_CheckWSResult(filesService.Edit(BlogxID, StartPath, FileID, FileName, FileExtn, FilePath, FileData))
					, true)
				);
		}

		public void DeleteFile(string StartPath, string FileID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
					ItemXml.ImportNode(
						_CheckWSResult(filesService.Delete(BlogxID, StartPath, FileID))
					, true)
				);
		}
		#endregion
	}
}

