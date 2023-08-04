using System;
using System.Web;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2011-05-04
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20110519:	Started
	20140112:	Refactored constructor
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
    /// <summary>
    /// Description of the class.
    /// <para>Additional information about the class</para>
    /// </summary>
	public class CmsXBrokerFile : CmsXBrokerBase {
		#region Invisible properties
        #endregion

        #region Constant name strings
		private const string logid = "CmsXBrokerFile.";
		private const string PREFIX_SEARCH = "FILE_SEARCH";
		private const string PREFIX_SEARCHITEM = "TS_File_";
		private const string PROFILE_FILE_ID = "fileID";
		private const string PROFILE_FILTER_EXTN_ID = "TS_Filter_FileExtnID";
		private const string PROFILE_FILTER_PATH_ID = "TS_Filter_FilePathID";
		private const string PROFILE_QUERY_FILE_ID = "TS_Query_FileID";
		private const string DEFAULT_PATH = "";
		#endregion

        #region Constant error strings
		private const string error_file_list = "Error listing the files: ";
		private const string error_file_get = "Error getting the file: ";
		private const string error_file_select = "Error selecting a file: ";
		private const string error_file_submit = "Error updating a file: ";
		private const string error_file_delete = "Error deleting a file: ";
		//private const string error_file_profile = "Error setting profile for an file: ";
		private const string error_file_upload = "Error with media upload: ";
		#endregion

        #region Visible properties
		/// <summary>
		/// 
		/// </summary>
		public string Path {
			get { return PathID.Replace('|', '/'); }
			set { PathID = value; }
		}
		public string PathID {
			get { return (UserProfile.Value(PROFILE_FILTER_PATH_ID, DEFAULT_PATH) == "0") ? DEFAULT_PATH : UserProfile.Value(PROFILE_FILTER_PATH_ID); }
			set { UserProfile.Add(PROFILE_FILTER_PATH_ID, value); }
		}
		#endregion

        #region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		//public CmsXBrokerFile(CmsX thispage) : base(thispage, typeof(CmsXBrokerFile), logid) {
		//}
		public CmsXBrokerFile(CmsX thispage, string blogxID) : base(thispage, blogxID, typeof(CmsXBrokerFile), logid) {
		}
        #endregion

        #region Public methods
        public override void Process(string type) {
			base.Process((type == "list"));
			xLogger.Info("Process:", type);
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
				case "get":
					get();
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
				// Drop-down lists:
				case "extns_dd":
					extensions();
					break;
				case "paths_dd":
					paths();
					break;
				// File upload:
				case "upload":
					upload();
					break;
				case "doupload":
					doupload();
					break;
			}
        }
        #endregion

        #region Protected methods
        #endregion

		#region Private process methods
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
				UserProfile.Clear(PROFILE_FILTER_EXTN_ID);
				UserProfile.Clear(PROFILE_FILTER_PATH_ID);
			}
			if (clearSearch) {
				xLogger.Debug("list_filter::clearSearch:");
				UserProfile.Clear(PREFIX_SEARCH);
				UserProfile.ClearAll(PREFIX_SEARCHITEM);
			}
			string FileID = _GetQueryID(PROFILE_QUERY_FILE_ID);
			xLogger.Debug("list_filter::seriesID:", FileID);
		}

		/// <summary>List files</summary>
		private void list_admin() {
			list_admin(false);
		}

		/// <summary>List files</summary>
		private void list_admin(bool clearProfile) {
			xLogger.Info("list_admin:");

			xLogger.Debug("list_admin:ok");
		}

		/// <summary>List files</summary>
		private void list() {
			xLogger.Info("list:");
			list(false);
		}

		/// <summary>Get list of series</summary>
		private void list(bool is_csv) {
			try {
				xLogger.Info("list:");
				SearchSettings setting = new SearchSettings(UserProfile, xLogger, is_csv);

				UserProfile.Add(PREFIX_SEARCH, (_IsSearchSet(PREFIX_SEARCHITEM, true)) ? "no" : "yes");
				xLogger.Debug("list::search:", UserProfile.Value(PREFIX_SEARCH));
				
				if (UserProfile.Value(PREFIX_SEARCH, "no") == "yes") {
					UserProfile.Clear(PROFILE_FILTER_EXTN_ID);
					UserProfile.Clear(PROFILE_FILTER_PATH_ID);
					UserProfile.Clear(PROFILE_QUERY_FILE_ID);
					
					// Set search items...
					SearchItem fileid = new SearchItem(PREFIX_SEARCHITEM, "ID", UserProfile, xLogger);
					// Do search...
					//_FileWS.SearchFile(setting,
					//	,fileid.Type	,fileid.Val
					//);
					if (is_csv) {
						//CsvUtil.GetColumns(_FileWS.ListItems);
					} else {
						//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_FileWS.ListXmlRoot, true));
					}
				} else {
					xLogger.Debug("list::search:", setting.Column, ":", setting.Page.ToString(), ":", setting.Rows.ToString());
					if (is_csv) {
						setting.MaxRows = true;
					}

					// Get filter(s) and call for list
					string filterExtn = UserProfile.Value(PROFILE_FILTER_EXTN_ID, "");
					filterExtn = (filterExtn == "0") ? "" : filterExtn;
					xLogger.Debug("list", "::filterExtn:", filterExtn, "::Path:", Path);
					_Files.ListFiles(Path, filterExtn, setting);

					// Output list to csv or content page (for rendering to grid)
					if (is_csv) {
						//CsvUtil.GetColumns(_FileWS.ListItems);
					} else {
						UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Files.ListXmlRoot, true));
						xLogger.Debug("list:ok");
					}
				}
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("list::error_file_list:", e.Message);
				throw (new XException("error_file_list", String.Concat(error_file_list, e.Message)));
			}
		}
		
		/// <summary>Get a file</summary> 
		private void get() {
			try {
				string fileID = _GetQueryID(PROFILE_FILE_ID);
				xLogger.Info("get::fileID:", fileID, "::Path:", Path);

				_Files.GetFile(Path, fileID);
				UserProfile.Add(PROFILE_FILE_ID, fileID);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Files.ItemXmlRootNode, true));
				xLogger.Debug("get:ok");
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				throw (new XException("error_file_get", String.Concat(error_file_get, e.Message)));
			}
		}

		/// <summary>Select a file</summary>
		private void select() {
			try {
				xLogger.Info("select:");

				xLogger.Debug("select:ok");
			} catch (XException e) {
				throw (new XException("error_file_select", String.Concat(error_file_select, e.Code, " - ", e.Message)));
			} catch (Exception e) {
				throw (new XException("error_file_select", String.Concat(error_file_select, e.Message)));
			}
		}

		/// <summary>Add/Edit a file</summary>
		/// <remarks>Files are added and edited by upload - this not required right now, maybe when want to start changing file properties eg Read-Only</remarks>
		private void submit(bool is_new) {
			try {
				xLogger.Debug("submit:is_new:", is_new.ToString());

				string fileid = UserProfile.Value("id", "0");
				fileid = (String.IsNullOrEmpty(fileid)) ? "0" : fileid;
				xLogger.Debug("submit:fileid:", fileid);

				int file_id = Int32.Parse(fileid);
				if (is_new) {
					xLogger.Debug("submit:file_id:", file_id.ToString());
					//_FileWS.AddFile(name, desc, user_id);
					//file_id = Int32.Parse(_FileWS.ItemID);
				} else {
					//_FileWS.UpdateFile(extract_id, name, desc, user_id);
				}

				xLogger.Debug("submit:ok");
			} catch (XException e) {
				xLogger.Debug("submit:xerror:", e.Code, "::", e.Message);
				throw e;
			} catch (Exception e) {
				xLogger.Debug("submit:error:", e.Message);
				throw (new XException("error_file_submit", String.Concat(error_file_submit, e.Message)));
			} finally {
				UserProfile.Add("file_actions", "");
			}
		}

		/// <summary>List files</summary>
		private void submit() {
			submit(false);
		}

		/// <summary>Delete a file</summary> 
		protected override void delete() {
			delete_file();
		}

		/// <summary>Delete a file</summary> 
		private void delete_file() {
			try {
				string fileID = _GetQueryID(PROFILE_FILE_ID);
				xLogger.Info("delete_file", "::fileID:", fileID, "::Path:", Path);

				_Files.DeleteFile(Path, fileID);

				xLogger.Debug("delete_file::ok");
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				throw (new XException("error_file_delete", String.Concat(error_file_delete, e.Message)));
			}
		}
		#endregion

		#region Private link dd methods
		/// <summary>Get list of extensions</summary>
		private void extensions() {
			try {
				xLogger.Info("extensions:", "::Path:", Path);
				_Files.ListExtensions(DEFAULT_PATH);	// Path?
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Files.ListXmlRoot, true));
				xLogger.Debug("extensions:ok");
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("extensions::error_file_list:", e.Message);
				throw (new XException("error_file_list", String.Concat(error_file_list, e.Message)));
			}
		}
		/// <summary>Get list of paths</summary>
		private void paths() {
			try {
				xLogger.Info("paths:", "::Path:", Path);
				_Files.ListPaths(DEFAULT_PATH);	// Path?
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Files.ListXmlRoot, true));
				xLogger.Debug("paths:ok");
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("paths::error_file_list:", e.Message);
				throw (new XException("error_file_list", String.Concat(error_file_list, e.Message)));
			}
		}
		#endregion

		#region Private upload methods
		/// <summary>Initialise an upload</summary> 
		private void upload() {
			try {
				xLogger.Info("upload:", "::Path:", Path);

				string fileID = UIPage.QueryParam("id");
				xLogger.Info("upload", "::fileID:", fileID);

				setDrops();

				if (String.IsNullOrEmpty(fileID)) {
					UserProfile.Clear("FileID");
					UserProfile.Clear("FileName");
					UserProfile.Clear("FileDescription");
				} else {
					_Files.GetFile(Path, fileID);
					xLogger.Info("upload", "::ItemXmlRoot:", _Files.ItemXmlRoot.OuterXml);
					XmlElement item =  _Files.ItemXmlRoot.SelectSingleNode("//item") as XmlElement;
					UserProfile.Add("FileID", item.GetAttribute("id"));
					UserProfile.Add("FileName", item.GetAttribute("name"));
					UserProfile.Add("FileExt", item.GetAttribute("ext"));
					UserProfile.Add("FilePath", String.Concat(PathID, item.GetAttribute("pathid")));
				}
				xLogger.Debug("upload:ok");
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				throw (new XException("error_file_upload", String.Concat(error_file_upload, e.Message)));
			}
		}

		/// <summary>Set File Dropdowns</summary> 
		private void setDrops() {
			setDrop(true);
			setDrop(false);
		}
		private void setDrop(bool isPaths) {
			SearchSettings setting = new SearchSettings(UserProfile, xLogger, "", false);
			setting.MaxRows = true;
			xLogger.Debug("setDrop::search:", setting.Column, ":", setting.Page.ToString(), ":", setting.Rows.ToString());

			if (isPaths) {
				_Files.ListPaths(DEFAULT_PATH, setting);
			} else {
				_Files.ListExtensions(DEFAULT_PATH, setting);
			}
			XmlElement subobjs = UserProfile.AddSub((isPaths) ? "FilePath" : "FileExt", true);
			UserProfile.AddSubobj(subobjs, "0", "Please select...");
			foreach (XmlNode item in _Files.ListXmlRoot.SelectNodes("//item")) {
				XmlElement itemel = item as XmlElement;
				UserProfile.AddSubobj(subobjs, itemel.GetAttribute("id"), itemel.GetAttribute("desc"));
			}
		}
		
		/// <summary>Do an upload</summary> 
		private void doupload() {
			try {
				xLogger.Info("doupload:", "::Path:", Path);

				HttpPostedFile thisfile = UIPage.Request.Files.Get(0);
				xLogger.Debug("doupload", "::FileName:", thisfile.FileName);
				string name = thisfile.FileName.Substring(0, thisfile.FileName.LastIndexOf('.'));
				string extn = thisfile.FileName.Substring(thisfile.FileName.LastIndexOf('.')+1);
				string filename = thisfile.FileName;

				string fileID	= UserProfile.Value("FileID");
				string fileName	= UserProfile.Value("FileName");
				string fileExtn	= UserProfile.Value("FileExt");
				string filePath	= UserProfile.Value("FilePath");
				xLogger.Debug("doupload", "::fileName:", fileName, "::fileExtn:", fileExtn, "::filePath:", filePath);

				fileName = String.IsNullOrEmpty(fileName) ? name : fileName;
				fileExtn = (fileExtn == "0") ? extn : fileExtn;
				filePath = (filePath == "0") ? "" : filePath;
				xLogger.Debug("doupload", "::fileName:", fileName, "::fileExtn:", fileExtn, "::filePath:", filePath);

				// if exists, use fileID (ie an update) or use name (ie a new upload)
				if (String.IsNullOrEmpty(fileID)) {
					_Files.AddFile(DEFAULT_PATH, fileName, fileExtn, filePath, _ReadToEnd(thisfile.InputStream));
				} else {
					Path = filePath;
					_Files.UpdateFile(Path, fileID, fileName, fileExtn, DEFAULT_PATH, _ReadToEnd(thisfile.InputStream));
				}
				xLogger.Debug("doupload:ok");
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				throw (new XException("error_file_upload", String.Concat(error_file_upload, e.Message)));
			}
		}
		#endregion

		#region Private utility methods
		#endregion
	}
}