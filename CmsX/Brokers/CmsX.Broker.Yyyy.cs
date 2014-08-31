using System;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2011-10-03
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20111003:	Started
	20111012:	Added upload / import processes
	20111027:	Added csv support
	20130513:	Added _IsDefault as test for DEFAULT_SUBITEM
	20140112:	Refactored constructor
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
    /// <summary>
    /// Description of the class.
    /// <para>Additional information about the class</para>
    /// </summary>
	class BrokerYyyy : BrokerIO {

        #region Constant name strings
		private const string logid = "BrokerYyyy.";
		private const string PREFIX_SEARCH = "YYYY_SEARCH";
		private const string PREFIX_SEARCHITEM = "TS_Yyyy_";
		private const string PROFILE_YYYY_ID = "yyyyID";
		private const string PROFILE_FILTER_YYYY_ID = "TS_Filter_YyyyID";
		private const string PROFILE_QUERY_YYYY_ID = "TS_Query_YyyyID";
		#endregion

        #region Constant error strings
		private const string error_yyyy_list = "Error listing the yyyys: ";
		private const string error_yyyy_get = "Error getting the yyyy: ";
		private const string error_yyyy_select = "Error selecting a yyyy: ";
		private const string error_yyyy_submit = "Error updating a yyyy: ";
		private const string error_yyyy_delete = "Error deleting a yyyy: ";
		//private const string error_yyyy_profile = "Error setting profile for an yyyy: ";
		private const string error_yyyy_upload = "Error uploading the yyyys file: ";
		private const string error_yyyy_upload_list = "Error listing the yyyys file: ";
		private const string error_yyyy_upload_get = "Error getting the yyyy: ";
		private const string error_yyyy_upload_submit = "Error updating a yyyy: ";
		private const string error_yyyy_upload_delete = "Error deleting the yyyy: ";
		//
		private const string error_yyyy_import = "Error importing the yyyys: ";
		private const string error_yyyy_import_list = "Error listing the import file: ";
		#endregion

        #region Visible properties
        #endregion

        #region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public BrokerYyyy(CmsXProfileX thispage)
			: base(thispage, typeof(BrokerYyyy), logid) {
		}
		public BrokerYyyy(CmsXGridX thispage)
			: base(thispage, typeof(BrokerYyyy), logid) {
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
					list(false);
					break;
				case "list_csv":
					list(true);
					break;
				case "list_dd":
					list_init(true);
					break;
				case "list_init":
					list_init(false);
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
				// alternative way to handle deletes and duplications...
				//case "edit"				: _GridEdit(UIPage.QueryParam("field", "none")); break;

				// upload / import processes
				case "upload":
					upload();
					break;
				case "upload_list":
					upload_list();
					break;
				case "upload_defaults":
					defaults();
					break;
				case "upload_get":
					upload_get();
					break;
				case "upload_add":
					upload_submit(true);
					break;
				case "upload_edit":
					upload_submit(false);
					break;
				case "upload_delete":
					upload_delete();
					break;
				case "import":
					import();
					break;
				case "import_list":
					import_list();
					break;
			}
        }
        #endregion

        #region Protected methods
		/// <summary>Import the uploaded file</summary>
		/// <remarks>This implements a basic importer. It is typically overriden in the derived broker class</remarks>
		protected override void _Import(XmlDocument ListDoc) {
			xLogger.Debug("_Import:");
			xLogger.Debug("_Import", "::ListDoc:", ListDoc.OuterXml);
			YyyyImporter importer = new YyyyImporter(UserProfile, _Yyyys);
			importer.Import(ListDoc);
		}
		/// <summary>Read the uploaded file</summary>
		/// <remarks>This implements a basic reader. It is typically overriden in the derived broker class</remarks>
		protected override void _Read(XmlDocument ListDoc) {
			xLogger.Debug("_Import:");
			xLogger.Debug("_Import", "::ListDoc:", ListDoc.OuterXml);
			YyyyReader reader = new YyyyReader(UserProfile, _Yyyys);
			reader.Read(ListDoc);
		}
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
				UserProfile.Clear(PROFILE_FILTER_YYYY_ID);
			}
			if (clearSearch) {
				xLogger.Debug("list_filter::clearSearch:");
				UserProfile.Clear(PREFIX_SEARCH);
				UserProfile.ClearAll(PREFIX_SEARCHITEM);
			}
			string YyyyID = _GetQueryID(PROFILE_QUERY_YYYY_ID);
			xLogger.Debug("list_filter::seriesID:", YyyyID);
		}

		/// <summary>Initialise Yyyys</summary>
		private void list_init(bool clearProfile) {
			xLogger.Info("list_init:");

			xLogger.Debug("list_init:ok");
		}

		/// <summary>Get list of Yyyys</summary>
		private void list(bool is_csv) {
			xLogger.Info("list", "::is_csv:", is_csv.ToString());
			try {
				SearchSettings setting = new SearchSettings(UserProfile, xLogger, is_csv);

				UserProfile.Add(PREFIX_SEARCH, (_IsSearchSet(PREFIX_SEARCHITEM, true)) ? "no" : "yes");
				xLogger.Debug("list::search:", UserProfile.Value(PREFIX_SEARCH));
				
				if (UserProfile.Value(PREFIX_SEARCH, "no") == "yes") {
					UserProfile.Clear(PROFILE_FILTER_YYYY_ID);
					UserProfile.Clear(PROFILE_QUERY_YYYY_ID);
					
					// Set search items...
					SearchItem yyyyid = new SearchItem(PREFIX_SEARCHITEM, "ID", UserProfile, xLogger);
					// Do search...
					//_YyyyWS.SearchYyyy(setting,
					//	,yyyyid.Type	,yyyyid.Val
					//);
					if (is_csv) {
						//CsvUtil.GetColumns(_YyyyWS.ListItems);
					} else {
						//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_YyyyWS.ListXmlRoot, true));
					}
				} else {
					xLogger.Debug("list::search:", setting.Column, ":", setting.Page.ToString(), ":", setting.Rows.ToString());
					if (is_csv) {
						setting.MaxRows = true;
					}

					// Get filter(s) and call for list
					string filter = UserProfile.Value(PROFILE_FILTER_YYYY_ID, "");					// eg string filter
					int filterType = Int32.Parse(UserProfile.Value(PROFILE_FILTER_YYYY_ID, "0"));	// eg int filter
					//_YyyyWS.ListYyyy(filterType, setting);

					// Output list to csv or content page (for rendering to grid)
					if (is_csv) {
						//CsvUtil.GetColumns(_YyyyWS.ListItems);
					} else {
						//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_YyyyWS.ListXmlRoot, true));
						xLogger.Debug("list:ok");
					}
				}
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("list::error_yyyy_list:", e.Message);
				throw (new x_exception("error_yyyy_list", String.Concat(error_yyyy_list, e.Message)));
			}
		}
		
		/// <summary>Get a yyyy</summary> 
		private void get() {
			try {
				string yyyyID = _GetQueryID(PROFILE_YYYY_ID);
				xLogger.Info("get::yyyyID:", yyyyID);

				//_YyyyWS.GetYyyy(yyyyID);
				UserProfile.Add(PROFILE_YYYY_ID, yyyyID);

				//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_YyyyWS.ItemXmlRootNode, true));
				xLogger.Debug("get:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_yyyy_get", String.Concat(error_yyyy_get, e.Message)));
			}
		}

		/// <summary>Select a yyyy</summary>
		private void select() {
			try {
				xLogger.Info("select:");

				xLogger.Debug("select:ok");
			} catch (x_exception e) {
				throw (new x_exception("error_yyyy_select", String.Concat(error_yyyy_select, e.Code, " - ", e.Message)));
			} catch (Exception e) {
				throw (new x_exception("error_yyyy_select", String.Concat(error_yyyy_select, e.Message)));
			}
		}

		/// <summary>Add/Edit a yyyy</summary> 
		private void submit(bool is_new) {
			try {
				xLogger.Debug("submit:is_new:", is_new.ToString());

				string yyyyid = UserProfile.Value("id", "0");
				yyyyid = (String.IsNullOrEmpty(yyyyid)) ? "0" : yyyyid;
				xLogger.Debug("submit:yyyyid:", yyyyid);

				int yyyy_id = Int32.Parse(yyyyid);
				if (is_new) {
					xLogger.Debug("submit:yyyy_id:", yyyy_id.ToString());
					//_YyyyWS.AddYyyy(name, desc, user_id);
					//yyyy_id = Int32.Parse(_YyyyWS.ItemID);
				} else {
					//_YyyyWS.UpdateYyyy(extract_id, name, desc, user_id);
				}

				handleSubItems("0_record_", yyyy_id.ToString(), is_new);

				xLogger.Debug("submit:ok");
			} catch (x_exception e) {
				xLogger.Debug("submit:xerror:", e.Code, "::", e.Message);
				throw e;
			} catch (Exception e) {
				xLogger.Debug("submit:error:", e.Message);
				throw (new x_exception("error_yyyy_submit", String.Concat(error_yyyy_submit, e.Message)));
			} finally {
				UserProfile.Add("yyyy_actions", "");
			}
		}

		/// <summary>List yyyys</summary>
		private void submit() {
			submit(false);
		}

		/// <summary>Add, update, remove subitems</summary>
		private void handleSubItems(string profilePrefix, string yyyy_id) {
			handleSubItems(profilePrefix, yyyy_id, false);
		}

		/// <summary>Add, update, remove subitems</summary>
		private void handleSubItems(string profilePrefix, string yyyy_id, bool is_new) {
			for (int i = 0; i < 1000; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string yyyyCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);

				if (!String.IsNullOrEmpty(yyyyCSV)) {
					string[] yyyy = yyyyCSV.Split(new char[] { '|' });
					xLogger.Debug("handleSubItems:", prof, ":", yyyyCSV, "; yyyy_id:", yyyy_id);
					// ie handleSubItems:0_record_0:default|default|1260|default|false|; yyyy_id:0

					string thisyyyy_id = yyyy[1];
					if (thisyyyy_id == "")
						break;
					else {
						string this_id = yyyy[2];
						string this_desc = yyyy[3];
						string this_remove = yyyy[4];
						if (this_remove == "true") {
							if (!(_IsDefault(thisyyyy_id) || is_new)) {
								//_YyyyWS.DeleteSubYyyy(Int32.Parse(thisyyyy_id));
							}
						} else if (_IsDefault(this_id)) {
							// ignore this
						} else {
							if (_IsDefault(thisyyyy_id) || is_new) {
								xLogger.Debug("handleSubItems::add::yyyy_id:", yyyy_id, "::this_id:", this_id);
								//_YyyyWS.AddSubYyyy(Int32.Parse(yyyy_id), Int32.Parse(this_id));
							} else {
								xLogger.Debug("handleSubItems::update::thisyyyy_id:", thisyyyy_id, "::yyyy_id:", yyyy_id, "::this_id:", this_id);
								//_YyyyWS.UpdateSubYyyy(Int32.Parse(thisyyyy_id), Int32.Parse(yyyy_id), Int32.Parse(this_id));
							}
						}
					}
				}
			}
		}

		/// <summary>Delete a yyyy</summary> 
		protected override void delete() {
			delete_yyyy();
		}

		/// <summary>Delete a yyyy</summary> 
		private void delete_yyyy() {
			try {
				xLogger.Info("delete_yyyy:");

				xLogger.Debug("delete_yyyy:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_yyyy_delete", String.Concat(error_yyyy_delete, e.Message)));
			}
		}
		#endregion

		#region Private upload / import methods
		/// <summary>Upload file and save</summary>
		private void upload() {
			xLogger.Debug("upload:");
			try {
				_Upload();
				UserProfile.Add(PROFILE_PROCESSED_FLAG, "no");
				xLogger.Debug("upload:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("upload:error::", e.Message, ":trace:", e.StackTrace);
				throw (new x_exception("error_yyyy_upload", String.Concat(error_yyyy_upload, e.Message)));
			}
		}

		/// <summary>List round</summary>
		private void upload_list() {
			xLogger.Debug("upload_list:");
			try {
				XmlElement list;
				if (UserProfile.Value(PROFILE_PROCESSED_FLAG) != "yes") {
					list = _Read();
					UserProfile.Add(PROFILE_PROCESSED_FLAG, "yes");
				} else {
					list = UploadDoc.DocumentElement;
				}
				//XmlElement yyyys = list.SelectSingleNode("//item/yyyys") as XmlElement;
				xLogger.Debug("upload_list", "::yyyys:", list.OuterXml);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(list, true));
				xLogger.Debug("upload_list", "::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_yyyy_upload_list", String.Concat(error_yyyy_upload_list, e.Message)));
			}
		}

		/// <summary>Get list of defaults</summary>
		private void defaults() {
			try {
				xLogger.Info("defaults:");

				SearchSettings setting = new SearchSettings(UserProfile, xLogger, true);
				//_Yyyys.ListDefaultTypesDrop(setting);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Yyyys.ListXmlRoot, true));
				xLogger.Debug("defaults:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("defaults", "::error_yyyy_upload_list:", e.Message);
				throw (new x_exception("error_yyyy_upload_list", String.Concat(error_yyyy_upload_list, e.Message)));
			}
		}

		/// <summary>Get an upload</summary> 
		private void upload_get() {
			try {
				string rowID = _GetQueryID("row");
				xLogger.Info("upload_get", "::rowID:", rowID);

				XmlDocument listdoc = UploadDoc;
				XmlElement yyyy = listdoc.SelectSingleNode(String.Format("//item[@row={0}]", rowID)) as XmlElement;
				xLogger.Info("upload_get", "::yyyy:", yyyy.OuterXml);

				/* Default checking done in Reader - this to be done when adding / editing.
				XmlElement defaults = yyyy.SelectSingleNode(Importer.DEFAULTS) as XmlElement;
				if (defaults != null) {
					SearchSettings settings = new SearchSettings(UserProfile, xLogger, true);
					_Yyyys.ListDefaultTypes(settings);

					foreach (XmlNode defnode in defaults.SelectNodes(Importer.DEFAULT)) {
						XmlElement defelem = defnode as XmlElement;
						XmlElement deftype = _Yyyys.ListItems.SelectSingleNode(String.Format("item[@id='{0}']", defelem.GetAttribute("id"))) as XmlElement;
						if (deftype != null) {
							defelem.SetAttribute("desc", deftype.GetAttribute("desc"));
							defelem.SetAttribute("unit", deftype.GetAttribute("unit"));
						} else {
							defelem.SetAttribute("status", "notok");
							defelem.SetAttribute("error", "no such setting");
						}
					}
				}
				*/

				XmlNode items = UIPage.Content.AppendChild(UIPage.Document.CreateElement("items"));
				items.AppendChild(UIPage.Document.ImportNode(yyyy, true));
				xLogger.Debug("upload_get:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_import_get", String.Concat(error_yyyy_upload_get, e.Message)));
			}
		}

		/// <summary>Add/Edit an upload</summary> 
		private void upload_submit(bool is_new) {
			xLogger.Debug("upload_submit");
			try {
				string yyyyid = UserProfile.Value("row", "0");
				string yyyyname = UserProfile.Value("name");
				string yyyydesc = UserProfile.Value("desc");
				string yyyyemail = UserProfile.Value("email");
				xLogger.Debug("upload_submit", "::yyyyid:", yyyyid);

				XmlDocument listdoc = UploadDoc;
				XmlElement yyyy;

				xLogger.Debug("upload_submit", "::is_new:", is_new.ToString());
				if (is_new) {
					XmlElement yyyys = listdoc.SelectSingleNode("//items") as XmlElement;
					yyyy = yyyys.AppendChild(listdoc.CreateElement(Cms.ITEM)) as XmlElement;
					yyyy.AppendChild(listdoc.CreateElement(Importer.DEFAULTS));
					yyyyid = yyyys.SelectNodes(Cms.ITEM).Count.ToString();
				} else {
					yyyy = listdoc.SelectSingleNode(String.Format("//item[@row='{0}']", yyyyid)) as XmlElement;
				}
				xLogger.Debug("upload_submit", "::yyyy:", (yyyy == null) ? "null" : yyyy.OuterXml);

				yyyy.SetAttribute("row", yyyyid);
				yyyy.SetAttribute("name", yyyyname);
				yyyy.SetAttribute("desc", yyyydesc);
				yyyy.SetAttribute("email", yyyyemail);
				yyyy.SetAttribute("status", "ok");
				yyyy.SetAttribute("error", "");
				listdoc.Save(UploadDocName);

				upload_defaults("0_record_", is_new, yyyy, listdoc);
				listdoc.Save(UploadDocName);

				XmlNode items = UIPage.Content.AppendChild(UIPage.Document.CreateElement("items"));
				items.AppendChild(UIPage.Document.ImportNode(yyyy, true));

				xLogger.Debug("upload_submit:ok");
			} catch (x_exception e) {
				xLogger.Debug("upload_submit", "::xerror:", e.Code, "::", e.Message);
				throw e;
			} catch (Exception e) {
				xLogger.Debug("upload_submit", "::error:", e.Message);
				throw (new x_exception("error_import_upload_submit", String.Concat(error_yyyy_upload_submit, e.Message)));
			} finally {
				UserProfile.Add("yyyys_actions", "");
			}
		}

		/// <summary>Add, update, remove defaults</summary>
		private void upload_defaults(string profilePrefix, bool is_new, XmlElement yyyy, XmlDocument uploadDoc) {
			xLogger.Debug("upload_defaults", "::is_new:", is_new.ToString(), "::yyyy:", yyyy.OuterXml);

			SearchSettings setting = new SearchSettings(UserProfile, xLogger, true);
			//_Yyyys.ListDefaultTypes(setting);

			YyyyDefault defobj = new YyyyDefault(xLogger, uploadDoc, _Yyyys.ListItems, yyyy);

			for (int i = 0; i < 1000; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string defaultRecord = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);

				if (!String.IsNullOrEmpty(defaultRecord)) {
					xLogger.Debug("upload_defaults", "::profile:", prof, "  ::defaultRecord:", defaultRecord);
					/*
					upload_defaults::profile:0_record_0  ::defaultRecord:sys~[...]|row~[...]|id~6|value~5|unit~[...]|status~[...]|error~[...]|remove~false|:; yyyyid::3
					upload_defaults::profile:0_record_1  ::defaultRecord:sys~1|row~0|id~1|value~Premium|unit~String|status~ok|error~|remove~|:; yyyyid::3
					upload_defaults::profile:0_record_2  ::defaultRecord:sys~2|row~1|id~2|value~24|unit~Months|status~ok|error~|remove~|:; yyyyid::3
					upload_defaults::profile:0_record_3  ::defaultRecord:sys~3|row~2|id~3|value~299|unit~Amount|status~ok|error~|remove~|:; yyyyid::3
					upload_defaults::profile:0_record_4  ::defaultRecord:sys~4|row~3|id~4|value~Yes|unit~Yes|No|status~ok|error~|remove~|:; yyyyid::3
					upload_defaults::profile:0_record_5  ::defaultRecord:sys~5|row~4|id~5|value~Debit|unit~None|Credit|Debit|Transfer|status~ok|error~|remove~|:; yyyyid::3
					upload_defaults::profile:0_record_6  ::defaultRecord:sys~10|row~5|id~10|value~5|unit~-|status~notok|error~no such setting|remove~true|:; yyyyid::3
					*/
					defobj.Process(is_new, defaultRecord);
				}
			}
			xLogger.Debug("upload_defaults", "::uploadDoc:", uploadDoc.OuterXml);
		}

		/// <summary>Delete a series</summary> 
		private void upload_delete() {
			try {
				string rowID = _GetQueryID("row");
				xLogger.Info("upload_delete", "::rowID:", rowID);

				XmlDocument listdoc = UploadDoc;
				XmlNode yyyys = listdoc.SelectSingleNode(Cms.SELECT_ALL_ITEMS);
				XmlElement yyyy = yyyys.SelectSingleNode(String.Format("item[@row={0}]", rowID)) as XmlElement;

				yyyys.RemoveChild(yyyy);
				listdoc.Save(UploadDocName);

				Logger.Debug("upload_delete:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_yyyy_upload_delete", String.Concat(error_yyyy_upload_delete, e.Message)));
			}
		}

		/// <summary>Upload file and save</summary>
		private void import() {
			xLogger.Debug("import:");
			try {
				XmlElement list = _Import();
				xLogger.Debug("import:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("import:error::", e.Message, ":trace:", e.StackTrace);
				throw (new x_exception("error_yyyy_import", String.Concat(error_yyyy_import, e.Message)));
			}
		}

		/// <summary>List round</summary>
		private void import_list() {
			xLogger.Debug("import_list:");
			try {
				xLogger.Debug("import_list", "::ImportDocName:", ImportDocName);

				XmlElement list = _ImportList();
				xLogger.Debug("import_list", "::yyyys:", list.OuterXml);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(list, true));
				Logger.Debug(String.Concat(logid, "import_list::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_yyyy_import_list", String.Concat(error_yyyy_import_list, e.Message)));
			}
		}

		#endregion

		#region Private utility methods
		#endregion
	}
}