using System;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Refactored from LoeriesAdmin
	20101218:	Added default space to user profile on selection
	20111010:	Added load_sites process
	20140112:	Refactored constructor
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
    /// <summary>
    /// Description of the LoerieAdminCategories class.
    /// <para>Additional information about the class</para>
    /// </summary>
	public class CmsXBrokerSites : CmsXBrokerBase {
		#region Invisible properties
        #endregion

        #region Constant name strings
		private const string logid = "CmsXBrokerSites.";
		#endregion

        #region Constant error strings
		private const string error_sites_get = "Error getting the sites: ";
		private const string error_site_select = "Error selecting an site: ";
		//private const string error_site_profile = "Error setting profile for an site: ";
		#endregion

        #region Visible properties
        #endregion

        #region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerSites(CmsX thispage) : base(thispage, typeof(CmsXBrokerSites), logid) {
		}
        #endregion

        #region Public methods
        public override void Process(string type) {
			base.Process((type != "list_sites"));
			xLogger.Info("Process:", type);
            switch (type) {
                case "list_sites"              : list_sites();                	break;
                case "load_sites"              : load_sites(false);                break;
                case "list_sites_dd"           : list_sites_admin(true);     	break;
                case "list_sites_admin"        : list_sites_admin();         	break;
                case "get_site"                : get_site();                 	break;
                case "select_site"             : select_site();               	break;
                case "add_site"                : submit_site(true);          	break;
                case "edit_site"               : submit_site();              	break;
                case "delete_site"             : delete_site();              	break;
            }
        }
        #endregion

        #region Protected methods
        #endregion

		#region Private utility methods
		/// <summary>List sites</summary>
		private void list_sites_admin() {
			list_sites_admin(false);
		}

		/// <summary>List sites</summary>
		private void list_sites_admin(bool clearProfile) {
			Logger.Info(String.Concat(logid, "list_sites_admin:"));
			//string dir = UserProfile.Value("dir", "ASC");
			//string sort_col = UserProfile.Value("sort", "");
			//UserProfile.Add("LA_Awards_sort", sort_col);
			//UserProfile.Add("LA_Awards_dir", dir);
			//if (clearProfile) {
			//    UserProfile.Clear("LA_Awards_dir");
			//    UserProfile.Add("LA_Awards_sort", "3");
			//}
			//bool sort_desc = (UserProfile.Value("LA_Awards_dir", "ASC") == "DESC") ? true : false;
			//sort_col = UserProfile.Value("LA_Awards_sort", "3");
			//int limit = Int32.Parse(UserProfile.Value("limit", UIPage.QueryParam("limit", "1")));
			//int page = (Int32.Parse(UserProfile.Value("start", "0")) / limit) + 1;
			//_Categories.ListAwardsAdmin(page, sort_col, sort_desc, limit);

			//if (clearProfile) {
			//    XmlNode sitesXml = _Categories.ListXmlRoot as XmlNode;
			//    XmlNodeList sites = sitesXml.SelectNodes("//item");
			//    foreach (XmlNode site in sites)
			//        site.SelectSingleNode("@desc").InnerText = site.SelectSingleNode("@year").InnerText;
			//    UIPage.Content.AppendChild(UIPage.Document.ImportNode(sitesXml, true));
			//} else
			//    UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Categories.ListXmlRoot as XmlNode, true));
			Logger.Debug(String.Concat(logid, "list_sites_admin:ok"));
		}

		/// <summary>List sites</summary>
		private void list_sites() {
			xLogger.Info("list_sites:");
			load_sites(true);
			xLogger.Debug("list_sites:ok");
		}

		/// <summary>List sites</summary>
		private void load_sites(bool isStartup) {
			xLogger.Info("load_sites", "::isStartup:", isStartup.ToString());
			
			DisplayX.GetSites();
			xLogger.Debug("load_sites", "::sites:", DisplayX.ListXml.OuterXml);

			UserProfile.Clear(Cms.PROFILE_SITES);
			UserProfile.Add(Cms.PROFILE_SITES, "Please select...", "0");
			XmlNodeList sites = DisplayX.ListItemsList;
			SetProfSubs(Cms.PROFILE_SITES, "@label", sites);

			// Now do auto-select of site show
			if (sites.Count == 1) {
			    XmlElement current = sites.Item(0) as XmlElement;
				xLogger.Debug("load_sites", "::onlyone:", current.OuterXml);
				string site = current.GetAttribute("id");
				UserProfile.Add(Cms.PROFILE_SITES, site);

				get_site();
				if (isStartup) {
					throw (new displayException(Cms.LINK_STARTUP));
				}
			}
		}
		/// <summary>Get a media type</summary> 
		private void get_site() {
			try {
				Logger.Info(String.Concat(logid, "get_site:"));
				string site = UserProfile.Value(Cms.PROFILE_SITES);

				// Set profile values associated with site here...
				DisplayX.GetSite(site);
				xLogger.Debug("get_site::Item:", DisplayX.ItemXml.OuterXml);
				UserProfile.Add(Cms.PROFILE_SITE_SPACE, DisplayX.Item.GetAttribute("space"));
	
				//string site_id = UIPage.QueryParam("id", "0");
				//site_id = (site_id == "0") ? UserProfile.Value("siteid", site_id) : site_id;
				//_Categories.GetAward(site_id);
				//UserProfile.Add("siteid", site_id);
				//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Categories.ItemXmlRootNode, true));
				Logger.Debug(String.Concat(logid, "get_site:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_sites_get", String.Concat(error_sites_get, e.Message)));
			}
		}

		/// <summary>Select an site show</summary>
		private void select_site() {
			xLogger.Info("select_site:");
			try {
				string site = UserProfile.Value(Cms.PROFILE_SITES);
				xLogger.Info("select_site::site:", site);

				UserProfile.Add(Cms.PROFILE_SITE_ID, site);
				get_site();

				string linkStart = UserProfile.Value(Cms.PROFILE_LINK_START, Cms.LINK_START);
				xLogger.Debug("select_site", "::linkStart:", linkStart);

				throw (new displayException(linkStart));
			} catch (displayException e) {
				throw e;
			} catch (x_exception e) {
				throw (new x_exception("error_site_select", String.Concat(error_site_select, e.Code, " - ", e.Message)));
			} catch (Exception e) {
				throw (new x_exception("error_site_select", String.Concat(error_site_select, e.Message)));
			}
		}

		/// <summary>Get a media type</summary> 
		private void submit_site(bool is_new) {
			try {
				Logger.Debug(String.Concat(logid, "submit_site:is_new:", is_new.ToString()));
/*
				string site_id = UserProfile.Value("siteid", UIPage.QueryParam("id", "0"));
				string year = UserProfile.Value("year", "-");
				string desc = UserProfile.Value("desc", "");
				string group = UserProfile.Value("group", "").Trim().ToLower().Replace(' ', '_');
				string inv_prefix = UserProfile.Value("inv_prefix", "");
				string virt_inv_prefix = UserProfile.Value("virt_inv_prefix", "");
				string entry_prefix = UserProfile.Value("entry_prefix", "");
				string entry_seed = UserProfile.Value("entry_seed", "0");
				//
				string is_archived = UserProfile.Value("is_archived", "false").ToLower();
				is_archived = (is_archived == "") ? "false" : is_archived;
				string is_open = UserProfile.Value("is_open", "false").ToLower();
				is_open = (is_open == "") ? "false" : is_open;
				string site_actions = UserProfile.Value("site_actions", "");
				Logger.Debug(String.Concat(logid, "submit_site::site_actions:", site_actions, "; site_actions.IndexOf('archive'):", site_actions.IndexOf("archive").ToString()));
				//site_actions
				// todo ::
				if (is_new) {
					Logger.Debug(String.Concat(logid, "submit_site:group:", group));
					Logger.Debug(String.Concat(logid, "submit_site:adding..."));
					_Categories.AddAward(year, desc, group, entry_prefix, virt_inv_prefix, inv_prefix, entry_seed);
					Logger.Debug(String.Concat(logid, "submit_site:...added."));
				} else {
					if (site_actions.IndexOf("archive") >= 0) {
						_Categories.ArchiveAwards(site_id);
						is_archived = "true";
					}
					if (site_actions.IndexOf("duplicate") >= 0) {
						bool isLimited = (site_actions.IndexOf("duplicate_limited") >= 0);

						Logger.Debug(String.Concat(logid, "submit_site:isLimited:", isLimited.ToString()));
						if (isLimited)
							_Categories.DuplicateAwardsLimited(site_id);
						else
							_Categories.DuplicateAwards(site_id);
						Logger.Debug(String.Concat(logid, "submit_site:...duplicated."));
					}
					//,
					Logger.Debug(String.Concat(logid, "submit_site:before update..."));
					_Categories.UpdateAward(site_id, year, desc, group, Boolean.Parse(is_open), Boolean.Parse(is_archived), entry_prefix, virt_inv_prefix, inv_prefix, entry_seed);
					Logger.Debug(String.Concat(logid, "submit_site:before get..."));
					_Categories.GetAward(site_id);
					Logger.Debug(String.Concat(logid, "submit_site:...edited."));
				}

				// Now reset the site's application cache for the site shows
				LoerieAdminStartup startup = new LoerieAdminStartup(UIPage.Application, UIPage.Config, UIPage.SiteProfile);
				startup.ReLoadShows();

				UserProfile.Add("siteid", site_id);
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Categories.ItemXmlRootNode, true));
*/
				Logger.Debug(String.Concat(logid, "submit_site:ok"));
			} catch (x_exception e) {
				Logger.Debug(String.Concat(logid, "submit_site:xerror:", e.Code, "::", e.Message));
				throw e;
			} catch (Exception e) {
				Logger.Debug(String.Concat(logid, "submit_site:error:", e.Message));
				throw (new x_exception("error_sites_get", String.Concat(error_sites_get, e.Message)));
			} finally {
				UserProfile.Add("site_actions", "");
			}
		}

		/// <summary>List sites</summary>
		private void submit_site() {
			submit_site(false);
		}

		/// <summary>Get a media type</summary> 
		private void delete_site() {
			try {
				Logger.Info(String.Concat(logid, "delete_site:"));
				//string site_id = UIPage.QueryParam("id", "0");
				//site_id = (site_id == "0") ? UserProfile.Value("siteid", site_id) : site_id;
				//_Categories.DeleteAward(site_id);
				//UserProfile.Add("siteid", site_id);
				//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Categories.ItemXmlRootNode, true));
				Logger.Debug(String.Concat(logid, "delete_site:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_sites_get", String.Concat(error_sites_get, e.Message)));
			}
		}
		#endregion
    }
}