using System;
using System.Web;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-10-20
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20101020:	Started
	20110522:	Move ReadToEnd to CmsXBrokerBase._ReadToEnd 
	20140112:	Refactored constructor
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
    /// <summary>
    /// Description of the class.
    /// <para>Additional information about the class</para>
    /// </summary>
	public class CmsXBrokerMedia : CmsXBrokerBase {
		#region Invisible properties
        #endregion

        #region Constant name strings
		private const string logid = "CmsXBrokerMedia.";
		private const string PREFIX_SEARCH = "MEDIA_SEARCH";
		private const string PREFIX_SEARCHITEM = "TS_Media_";
		private const string PROFILE_MEDIA_ID = "mediaID";
		private const string PROFILE_FILTER_MEDIA_ID = "TS_Filter_MediaID";
		private const string PROFILE_QUERY_MEDIA_ID = "TS_Query_MediaID";
		#endregion

        #region Constant error strings
		private const string error_media_list = "Error listing the media: ";
		private const string error_media_get = "Error getting the media: ";
		private const string error_media_select = "Error selecting a media: ";
		private const string error_media_submit = "Error updating a media: ";
		private const string error_media_delete = "Error deleting a media: ";
		private const string error_media_upload = "Error with media upload: ";
		#endregion

        #region Visible properties
        #endregion

        #region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerMedia(CmsX thispage, string blogxID) : base(thispage, blogxID, typeof(CmsXBrokerMedia), logid) {
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
				UserProfile.Clear(PROFILE_FILTER_MEDIA_ID);
			}
			if (clearSearch) {
				xLogger.Debug("list_filter::clearSearch:");
				UserProfile.Clear(PREFIX_SEARCH);
				UserProfile.ClearAll(PREFIX_SEARCHITEM);
			}
			string MediaID = _GetQueryID(PROFILE_QUERY_MEDIA_ID);
			xLogger.Debug("list_filter::seriesID:", MediaID);
		}

		/// <summary>List media</summary>
		private void list_admin() {
			list_admin(false);
		}

		/// <summary>List media</summary>
		private void list_admin(bool clearProfile) {
			xLogger.Info("list_admin:");

			xLogger.Debug("list_admin:ok");
		}

		/// <summary>List media</summary>
		private void list() {
			xLogger.Info("list:");
			try {
				xLogger.Info("list:");
				SearchSettings setting = new SearchSettings(UserProfile, xLogger, false);
				xLogger.Debug("list::search:", setting.Column, ":", setting.Page.ToString(), ":", setting.Rows.ToString());

				// Get filter(s) and call for list
				string filter = UserProfile.Value(PROFILE_FILTER_MEDIA_ID, "");
				_Media.List(filter, setting);

				// Output list to csv or content page (for rendering to grid)
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Media.ListXmlRoot, true));
				xLogger.Debug("list:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("list::error_media_list:", e.Message);
				throw (new x_exception("error_media_list", String.Concat(error_media_list, e.Message)));
			}
		}
		
		/// <summary>Get a media</summary> 
		private void get() {
			try {
				string mediaID = _GetQueryID(PROFILE_MEDIA_ID);
				xLogger.Info("get::mediaID:", mediaID);

				_Media.GetMedia(mediaID);
				UserProfile.Add(PROFILE_MEDIA_ID, mediaID);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Media.ItemXmlRootNode, true));
				xLogger.Debug("get:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_media_get", String.Concat(error_media_get, e.Message)));
			}
		}

		/// <summary>Select a media</summary>
		private void select() {
			try {
				xLogger.Info("select:");

				xLogger.Debug("select:ok");
			} catch (x_exception e) {
				throw (new x_exception("error_media_select", String.Concat(error_media_select, e.Code, " - ", e.Message)));
			} catch (Exception e) {
				throw (new x_exception("error_media_select", String.Concat(error_media_select, e.Message)));
			}
		}

		/// <summary>Add/Edit a media</summary> 
		private void submit(bool is_new) {
			try {
				xLogger.Debug("submit:is_new:", is_new.ToString());

				string mediaID = UserProfile.Value("id");
				xLogger.Debug("submit:mediaID:", mediaID);

				string name = UserProfile.Value("name");
				string desc = UserProfile.Value("description");
				string type = UserProfile.Value("type");
				string styp = UserProfile.Value("subtype");
				string url = UserProfile.Value("url");

				if (is_new) {
					_Media.AddMedia(name, desc, type, styp, url);
				} else {
					_Media.UpdateMedia(mediaID, desc, type, styp, url);
				}

				xLogger.Debug("submit:ok");
			} catch (x_exception e) {
				xLogger.Debug("submit:xerror:", e.Code, "::", e.Message);
				throw e;
			} catch (Exception e) {
				xLogger.Debug("submit:error:", e.Message);
				throw (new x_exception("error_media_submit", String.Concat(error_media_submit, e.Message)));
			} finally {
				UserProfile.Add("media_actions", "");
			}
		}

		/// <summary>List media</summary>
		private void submit() {
			submit(false);
		}

		/// <summary>Delete a media</summary> 
		protected override void delete() {
			delete_media();
		}

		/// <summary>Delete a media</summary> 
		private void delete_media() {
			try {
				xLogger.Info("delete_media:");

				string mediaID = _GetQueryID(PROFILE_MEDIA_ID);
				xLogger.Info("delete::mediaID:", mediaID);

				_Media.DeleteMedia(mediaID);

				xLogger.Debug("delete_media:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_media_delete", String.Concat(error_media_delete, e.Message)));
			}
		}
		#endregion

		#region Private upload methods
		/// <summary>Initialise an upload</summary> 
		private void upload() {
			try {
				xLogger.Info("upload:");

				string mediaID = UIPage.QueryParam("id");
				//string mediaID = _GetQueryID(PROFILE_MEDIA_ID);
				xLogger.Info("upload", "::mediaID:", mediaID);

				if (String.IsNullOrEmpty(mediaID)) {
					UserProfile.Clear("MediaID");
					UserProfile.Clear("MediaName");
					UserProfile.Clear("MediaDescription");
				} else {
					_Media.GetMedia(mediaID);
					xLogger.Info("upload", "::ItemXmlRoot:", _Media.ItemXmlRoot.OuterXml);
					XmlElement item =  _Media.ItemXmlRoot.SelectSingleNode("//item") as XmlElement;
					UserProfile.Add("MediaID", item.GetAttribute("name"));
					UserProfile.Add("MediaName", item.GetAttribute("name"));
					UserProfile.Add("MediaDescription", item.GetAttribute("description"));
				}
				xLogger.Debug("upload:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_media_upload", String.Concat(error_media_upload, e.Message)));
			}
		}

		/// <summary>Do an upload</summary> 
		private void doupload() {
			try {
				xLogger.Info("doupload:");

				HttpPostedFile thisfile = UIPage.Request.Files.Get(0);
				xLogger.Debug("doupload", "::FileName:", thisfile.FileName);
				string name = thisfile.FileName.Substring(0, thisfile.FileName.LastIndexOf('.'));
				string filename = thisfile.FileName;
				xLogger.Debug("doupload", "::name:", name, "::filename:", filename);

				string mediaDesc = UserProfile.Value("MediaDescription");
				string mediaID = UserProfile.Value("MediaID");
				string mediaName = UserProfile.Value("MediaName");

				// if exists, use mediaID (ie an update) or use name (ie a new upload)
				if (String.IsNullOrEmpty(mediaID)) {
					mediaID = String.IsNullOrEmpty(mediaName) ? name : mediaName;
					xLogger.Debug("doupload.add", "::mediaID:", mediaID);
					_Media.AddMedia(mediaID, mediaDesc, filename, _ReadToEnd(thisfile.InputStream));
				} else {
					xLogger.Debug("doupload.update", "::mediaID:", mediaID);
					_Media.UpdateMedia(mediaID, mediaDesc, filename, _ReadToEnd(thisfile.InputStream)); 
				}
				xLogger.Debug("doupload:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_media_upload", String.Concat(error_media_upload, e.Message)));
			}
		}
		#endregion

		#region Private utility methods
		#endregion
	}
}