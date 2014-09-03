using System;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2014-01-12
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20140112:	Refactored from CmsXBrokerBase
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>
	/// Base class for CmsXBrokers.
	/// <para></para>
	/// </summary>
	public class CmsXBroker : x_broker {
		
		private const string CONFIG_PASSPORT = "PassportX";
		private const string CONFIG_PASSPORT_CUSTOMERID = "customer";
		private const string CONFIG_PASSPORT_ADMINID = "admin";
		private const string CONFIG_PASSPORT_PASSPORTID = "passport";
		
		protected CmsXUser _User;
		protected string _BlogxID;

		/// <summary>Reference to the CmsXProfileX object that invoked the broker</summary>
		public CmsXProfileX ProfilePage { get; set; }
		public x_config Config { get; set; }

		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBroker(CmsXProfileX thispage) : base(thispage) {
			ProfilePage = thispage;
			Config = thispage.Config;
			_User = thispage.WebsiteUser;
		}

		private CmsXCSV csvutil;
		public CmsXCSV CsvUtil {
			get {
				if (csvutil == null) {
					csvutil = new CmsXCSV(ProfilePage);
				}
				return csvutil;
			}
		}

		private Displayx displayX;
		public Displayx DisplayX {
			get {
				if (displayX == null) {
					displayX = String.IsNullOrEmpty(_BlogxID) ? new Displayx(ProfilePage) : new Displayx(ProfilePage, _BlogxID);
				}
				return displayX;
			}
		}

		private AdminX adminX;
		protected AdminX _AdminX {
			get {
				if (adminX == null) {
					adminX = new AdminX(ProfilePage);
				}
				return adminX;
			}
		}

		private PassportX passportX;
		protected PassportX _PassportX {
			get {
				if (passportX == null) {
					passportX = new PassportX(ProfilePage);
				}
				return passportX;
			}
		}

		private Users users;
		protected Users _Users {
			get {
				if (users == null) {
					users = new Users(ProfilePage);
				}
				return users;
			}
		}

		private Access access;
		protected Access _Access {
			get {
				if (access == null) {
					access = new Access(ProfilePage);
				}
				return access;
			}
		}

		private LinkLog linklog;
		protected LinkLog _LinkLog {
			get {
				if (linklog == null) {
					linklog = new LinkLog(ProfilePage);
				}
				return linklog;
			}
		}

		private Links links;
		protected Links _Links {
			get {
				if (links == null) {
					links = new Links(ProfilePage, _BlogxID);
				}
				return links;
			}
		}

		private Menus menus;
		protected Menus _Menus {
			get {
				if (menus == null) {
					menus = new Menus(ProfilePage, _BlogxID);
				}
				return menus;
			}
		}

		private Spaces spaces;
		protected Spaces _Spaces {
			get {
				if (spaces == null) {
					spaces = new Spaces(ProfilePage, _BlogxID);
				}
				return spaces;
			}
		}

		private Topics topics;
		protected Topics _Topics {
			get {
				if (topics == null) {
					topics = new Topics(ProfilePage, _BlogxID);
				}
				return topics;
			}
		}

		private Blogs blogs;
		protected Blogs _Blogs {
			get {
				if (blogs == null) {
					blogs = new Blogs(ProfilePage, _BlogxID);
				}
				return blogs;
			}
		}

		private Media media;
		protected Media _Media {
			get {
				if (media == null) {
					media = new Media(ProfilePage, _BlogxID);
				}
				return media;
			}
		}

		private Files files;
		protected Files _Files {
			get {
				if (files == null) {
					files = new Files(ProfilePage, _BlogxID);
				}
				return files;
			}
		}

		private PassportCustomerWS.PassportXCustomerServices customerWS;
		protected PassportCustomerWS.PassportXCustomerServices _CustomerWS {
			get {
				if (customerWS == null) {
					customerWS = new PassportCustomerWS.PassportXCustomerServices();
					customerWS.Url = getPassportUrl(CONFIG_PASSPORT_CUSTOMERID);
				}
				return customerWS;
			}
		}

		private PassportAdminWS.PassportXAdminServices adminWS;
		protected PassportAdminWS.PassportXAdminServices _AdminWS {
			get {
				if (adminWS == null) {
					adminWS = new PassportAdminWS.PassportXAdminServices();
					adminWS.Url = getPassportUrl(CONFIG_PASSPORT_ADMINID);
				}
				return adminWS;
			}
		}

		private PassportWS.PassportXPassportServices passportWS;
		protected PassportWS.PassportXPassportServices _PassportWS {
			get {
				if (passportWS == null) {
					passportWS = new PassportWS.PassportXPassportServices();
					passportWS.Url = getPassportUrl(CONFIG_PASSPORT_PASSPORTID);
				}
				return passportWS;
			}
		}

		/// <summary>Gets the url of a webservice by config element</summary>
		private string getPassportUrl(string configID) {
			return UIPage.Config.Value(String.Concat(CONFIG_PASSPORT, "/Url[@id='", configID, "']"));
		}

	}
}
