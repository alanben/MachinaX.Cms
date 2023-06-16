using System;
using System.Web;
using System.Xml;

//using clickclickboom.machinaX;
//using clickclickboom.machinaX.blogX;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2011-10-12
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20111012:	Started
	20140112:	Refactored constructor
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
	/// <summary>
	/// This is the parent class of AdminReader and AdminImporter
    /// </summary>
	public class BrokerIO : CmsXBrokerBase {

		private Yyyys yyyys;
		protected Yyyys _Yyyys {
			get {
				if (yyyys == null) {
					yyyys = new Yyyys(ProfilePage);
				}
				return yyyys;
			}
		}

        #region Private constants
		private const string logid = "BrokerIO.";
		private const string XML_SERVICEX = "<ServiceX><Result><Result_Code>0</Result_Code><Description>OK</Description></Result><items/></ServiceX>";
		private const string error_reading = "Error processing the uploaded data: ";
		private const string error_listing = "Error listing the processed data: ";
		private const string error_importing = "Error importing the processed data: ";
		#endregion

        #region Public constants
		protected const string PROFILE_UPLOAD_TYPE = "UploadFileType";
		protected const string PROFILE_UPLOAD_FILE = "UploadFileName";
		protected const string PROFILE_PROCESSED_FLAG = "UploadProcessed";
		#endregion

        #region Visible properties
		/// <summary>
		/// The text input file containing the raw data
		/// </summary>
		public string InputFile {
			get { return UserProfile.Value(PROFILE_UPLOAD_FILE); }
		}
		/// <summary>
		/// The type (ie format) of the raw data
		/// </summary>
		public string InputType {
			get { return UserProfile.Value(PROFILE_UPLOAD_TYPE); }
		}
		/// <summary>
		/// The xml document filename containing the processed (uploaded) data
		/// </summary>
		public string UploadDocName {
			get { return String.Concat(InputFile, ".xml"); }
		}
		/// <summary>
		/// The xml document filename containing the imported data
		/// </summary>
		public string ImportDocName {
			get { return String.Concat(InputFile, ".import.xml"); }
		}
		/// <summary>
		/// The xml document containing the processed (uploaded) data
		/// </summary>
		public XmlDocument UploadDoc {
			get { 
				XmlDocument listDoc = new XmlDocument();
				listDoc.Load(UploadDocName);
				return listDoc;
			}
		}
		/// <summary>
		/// The xml document containing the imported data
		/// </summary>
		public XmlDocument ImportDoc {
			get {
				XmlDocument listDoc = new XmlDocument();
				listDoc.Load(ImportDocName);
				return listDoc;
			}
		}
		#endregion

        #region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public BrokerIO(CmsXProfileX thispage, Type type, string loggerID)
			: base(thispage, type, loggerID) {
		}
		public BrokerIO(CmsXGridX thispage, Type type, string loggerID)
			: base(thispage, type, loggerID) {
		}
        #endregion

		#region Public methods
		#endregion

		#region Protected upload methods
		/// <summary>Upload and save the source file</summary>
		protected void _Upload() {
			xLogger.Debug("Upload:file count:", UIPage.Request.Files.Count.ToString());
			for (int i = 0; i < UIPage.Request.Files.Count; i++) {
				HttpPostedFile thisfile = UIPage.Request.Files.Get(i);

				xLogger.Debug("Upload:thisfile:", thisfile.FileName);
				string thisname = thisfile.FileName.Substring(thisfile.FileName.LastIndexOf("\\")+1);

				string filename = UIPage.Server.MapPath(String.Concat("uploads/", thisname));
				xLogger.Debug("Upload:filename:", filename);

				UserProfile.Add(PROFILE_UPLOAD_FILE, filename);
				thisfile.SaveAs(filename);
			}
		}

		/// <summary>Read the uploaded file</summary>
		protected XmlElement _Read() {
			xLogger.Debug("Read::type:", UserProfile.Value(PROFILE_UPLOAD_TYPE));
			XmlDocument listDoc = new XmlDocument();
			try {
				listDoc.LoadXml(XML_SERVICEX);
				_Read(listDoc);
				listDoc.Save(UploadDocName);
				xLogger.Debug("Read::ok:");
			} catch (Exception e) {
				xLogger.Debug("Read::error:", e.Message);
				throw new x_exception("error_reading", String.Concat(error_reading, e.Message));
			}
			return listDoc.DocumentElement;
		}

		/// <summary>Read the uploaded file</summary>
		/// <remarks>This implements a basic reader. It is typically overriden in the derived broker class</remarks>
		protected virtual void _Read(XmlDocument ListDoc) {
			YyyyReader reader = new YyyyReader(UserProfile, _Yyyys);
			reader.Read(ListDoc);
		}

		/// <summary></summary>
		protected XmlElement _List(bool isBulk) {
			xLogger.Debug("List::type:", UserProfile.Value(PROFILE_UPLOAD_TYPE));
			XmlDocument listDoc;
			try {
				listDoc = UploadDoc;
				xLogger.Debug("List::ok:");
			} catch (Exception e) {
				xLogger.Debug("List::error:", e.Message);
				throw new x_exception("error_listing", String.Concat(error_listing, e.Message));
			}
			return listDoc.DocumentElement;
		}

		/// <summary>Get the list of uploaded and processed items</summary>
		protected XmlElement _UploadList() {
			return UploadDoc.DocumentElement;
		}
		#endregion

		#region Protected upload methods
		/// <summary></summary>
		protected XmlElement _Import() {
			xLogger.Debug("Import::type:", UserProfile.Value(PROFILE_UPLOAD_TYPE));
			XmlDocument listDoc = new XmlDocument();
			try {
				listDoc.LoadXml(XML_SERVICEX);
				_Import(listDoc);
				listDoc.Save(ImportDocName);
				xLogger.Debug("Import::ok:");
			} catch (Exception e) {
				xLogger.Debug("Import::error:", e.Message);
				throw new x_exception("error_importing", String.Concat(error_importing, e.Message));
			}
			return listDoc.DocumentElement;
		}

		/// <summary>Import the uploaded file</summary>
		/// <remarks>This implements a basic importer. It is typically overriden in the derived broker class</remarks>
		protected virtual void _Import(XmlDocument ListDoc) {
			YyyyImporter importer = new YyyyImporter(UserProfile, _Yyyys);
			importer.Import(ListDoc);
		}

		/// <summary>Get the list of imported items</summary>
		protected XmlElement _ImportList() {
			return ImportDoc.DocumentElement;
		}
		#endregion

        #region Private methods
        #endregion
    }
}
