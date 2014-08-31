using System;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-08-06
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100927:	Started from EconoVault
	20100927:	Changed "default" to DEFAULT_SUBITEM
	20111116:	Replaced group_usersall with AccessGroup.Process
				Replaced groups_rights_edit with AccessRight.Process
	20130513:	Added _IsDefault as test for DEFAULT_SUBITEM
	20140112:	Refactored constructor
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>
	/// Broker class to manage user access to sytem.
	/// <para>Management of users, profiles and rights</para>
	/// </summary>
	public class CmsXBrokerAccess : CmsXBrokerBase {
		#region Invisible properties
		#endregion

		#region Constant name strings
		private const string logid = "CmsXBrokerAccess";
		private const string PROFILE_ADMINID = "UserID";
		private const string GROUP_ID = "GroupID";
		private const string COLLECTION_ID = "CollectionID";
		#endregion

		#region Constant error strings
		private const string error_no_error = "Not an error";
		private const string error_access = "CmsXBrokerAccess error::";
		private const string error_user = "CmsXBrokerAccess user error::";
		private const string error_user_unlock = " Error unlocking user account: ";
		private const string error_user_manage = " Error managing user: ";
		private const string error_user_list = "Error getting the list of users: ";
		private const string error_user_get = "Error getting the user: ";
		private const string error_user_add = "Error adding the user: ";
		private const string error_user_admin_add = "Error adding the admin user: ";
		private const string error_user_edit = "Error editing user update: ";
		private const string error_user_admin_edit = "Error editing admin user update: ";
		private const string error_user_delete = "Error deleting user update: ";

		private const string error_profile = "CmsXBrokerAccess profile error::";
		private const string error_profile_manage = " Error managing profile: ";
		private const string error_profile_list = "Error getting the list of profiles: ";
		private const string error_profile_get = "Error getting the profile: ";
		private const string error_group_add = "Error adding the group: ";
		private const string error_group_edit = "Error editing group update: ";
		private const string error_group_delete = "Error deleting group update: ";
		private const string error_collection_edit = "Error editing collection update: ";
		private const string error_group_rights_get = "Error getting the group rights: ";
		#endregion

		#region Visible properties
		//public new static readonly ILog Logger = LogManager.GetLogger(typeof(CmsXBrokerAccess));
		#endregion

		#region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerAccess(CmsX thispage) : base(thispage, typeof(CmsXBrokerAccess), logid) {
			if (_Access == null || _Users == null) {
				throw new x_exception("error_access", String.Concat(error_access, "_Access or _Users is null"));
			}
		}
		#endregion

		#region Public methods
		public override void Process(string type) {
			base.Process(true);	// does login checking
			Logger.Info(String.Concat(logid, "_Process:", type));
			switch (type) {
				case "user_unlock":				user_unlock();			break;
				case "user_admin_list_dd":		user_list_dd();			break;
				case "user_admin_list":			user_admin_list();		break;
				case "user_admin_get":			user_admin_get();		break;
				case "user_admin_add":			user_admin_add();		break;
				case "user_admin_edit":			user_admin_edit();		break;
				case "user_admin_delete":		user_admin_delete();	break;
				//case "user_manage":			user_manage();		break;     ivan: do we need this?

				case "groups_list":				groups_list();			break;
				case "groups_list_dd":			group_list(true);		break;
				case "groups_get":				groups_get();			break;
				case "groups_rights_get":		groups_rights_get();	break;
				case "groups_add":				group_add();			break;
				case "groups_categories_edit":	groups_edit();			break;
				case "groups_delete":			groups_delete();		break;
				//case "profile_manage":		profile_manage();	break;     ivan: do we need this?

				case "collections_list":		collections_list();		break;
				case "collections_get":			collections_get();		break;
				case "collections_edit":		collections_edit();		break;
					
			}
		}
		#endregion

		#region Protected methods
		#endregion

		#region Private User methods
		/// <summary>Manage user</summary>
		private void user_unlock() {
			try {
				Logger.Info(String.Concat(logid, "_user_unlock:"));
				XmlElement result = _CustomerWS.GetUser(UserProfile.Value("AccessUsername")) as XmlElement;
				if (!VerifyPassport(result))
					throw new displayException("access_unlock_notuser");
				Logger.Info(String.Concat(logid, "_user_unlock:token:", result.GetAttribute("Token")));
				_CustomerWS.UnlockUser(result.GetAttribute("Token"));
				Logger.Debug(String.Concat(logid, "_user_unlock::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				Logger.Debug(String.Concat(logid, "_user_unlock:error:", e.Message));
				throw (new x_exception("error_user_unlock", String.Concat(error_user_unlock, e.Message)));
			}
		}
		/// <summary>Manage user</summary>
		private void user_manage() {
			try {
				Logger.Info(String.Concat(logid, "_user_manage:"));
				Logger.Debug(String.Concat(logid, "_user_manage::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_user_manage", String.Concat(error_user_manage, e.Message)));
			}
		}
		/// <summary>List admin user</summary>
		private void user_admin_list() {
			user_list(false);
		}
		/// <summary>List user</summary>
		private void user_list_dd() {
			try {
				Logger.Info(String.Concat(logid, "user_list_dd:"));
				SearchSettings search = new SearchSettings(UserProfile, Logger, 1, "4", false, true);

				_Access.ListAdminUsersDrop(search);

				Logger.Debug(String.Concat(logid, "user_list_dd::ListXmlRoot:", _Access.ListXmlRoot.OuterXml));
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Access.ListXmlRoot as XmlNode, true));

				Logger.Debug(String.Concat(logid, "user_list_dd::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_user_list", String.Concat(error_user_list, e.Message)));
			}
		}
		/// <summary>List user</summary>
		/// <param name="clearProfile">True when list is for a drop-down</param>
		private void user_list(bool clearProfile) {
			try {
				Logger.Info(String.Concat(logid, "_user_list:"));
				SearchSettings search = new SearchSettings(UserProfile, Logger, 1, "4", false, true);

				_Access.ListAdminUsers(search);
				Logger.Debug(String.Concat(logid, "_user_list::ListXmlRoot:", _Access.ListXmlRoot.OuterXml));
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Access.ListXmlRoot as XmlNode, true));

				Logger.Debug(String.Concat(logid, "_user_list::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_user_list", String.Concat(error_user_list, e.Message)));
			}
		}
		/// <summary>Get user</summary>
		private void user_admin_get() {
			try {
				Logger.Info(String.Concat(logid, "_user_get:"));

				string userID = _GetQueryID(PROFILE_ADMINID);
				_Access.GetAdminUser(Int32.Parse(userID));
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Access.ItemXmlRootNode, true));

				Logger.Debug(String.Concat(logid, "_user_get::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_user_get", String.Concat(error_user_get, e.Message)));
			}
		}
		/// <summary>Add admin user</summary>
		private void user_admin_add() {
			try {
				Logger.Info(String.Concat(logid, "_user_add:"));

				string username = UserProfile.Value("username");
				string password = UserProfile.Value("password");
				string firstname = UserProfile.Value("firstname");
				string surname = UserProfile.Value("surname");
				string email = UserProfile.Value("email");
				string cellphone = UserProfile.Value("cellphone");
				string telno = UserProfile.Value("telno");
				Logger.Debug(String.Concat(logid, "_user_add:username:", username));

				_Access.AddAdminUser(username, password, firstname, surname, email, telno, cellphone);
				//Logger.Debug(String.Concat(logid, "_user_admin_add:", _Users.ItemID));

				//_Access.AddUser(Int32.Parse(_Users.ItemID));

				//Logger.Debug(String.Concat(logid, "_user_admin_add::finished:userID:", _Users.ItemID, ":userID:", _Access.ItemID));
				Logger.Debug(String.Concat(logid, "_user_admin_add::finished:"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				Logger.Debug(String.Concat(logid, "_user_admin_add:error:", e.Message, "::", e.StackTrace));
				throw (new x_exception("error_user_admin_add", String.Concat(error_user_admin_add, e.Message)));
			}
		}
		/// <summary>Edit user</summary>
		private void user_admin_edit() {
			try {
				Logger.Info(String.Concat(logid, "_user_admin_edit:"));

				string userID = _GetQueryID(PROFILE_ADMINID);
				_Access.GetAdminUser(Int32.Parse(userID));
				//string userID = _Access.GetItemValue("person_id");
				string username = UserProfile.Value("username");
				string password = UserProfile.Value("password");
				string firstname = UserProfile.Value("firstname");
				string surname = UserProfile.Value("surname");
				string email = UserProfile.Value("email");
				string cellphone = UserProfile.Value("cellphone");
				string telno = UserProfile.Value("telno");
				string statusID = UserProfile.Value("personstatus_id");
				_Access.UpdateAdminUser(Int32.Parse(userID), username, password, firstname, surname, email, telno, cellphone, Int32.Parse(statusID));

				user_profiles("0_record_", userID);


				Logger.Debug(String.Concat(logid, "_user_admin_edit::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_user_admin_edit", String.Concat(error_user_admin_edit, e.Message)));
			}
		}
		/// <summary>Delete user</summary>
		private void user_admin_delete() {
			try {
				Logger.Info(String.Concat(logid, "_user_admin_delete:"));

				string userID = _GetQueryID(PROFILE_ADMINID);
				_Access.GetUser(Int32.Parse(userID));
				//string userID = _Access.GetItemValue("person_id");

				_Access.DeleteAdminUser(Int32.Parse(userID));
				//user_admin_list();
				//UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Access.ItemXmlRootNode, true));

				Logger.Debug(String.Concat(logid, "_user_delete::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_user_delete", String.Concat(error_user_delete, e.Message)));
			}
		}
		#endregion

		#region Private Profile methods
		/// <summary>Manage profile</summary>
		private void profile_manage() {
			try {
				Logger.Info(String.Concat(logid, "_profile_manage:"));
				Logger.Debug(String.Concat(logid, "_profile_manage::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_profile_manage", String.Concat(error_profile_manage, e.Message)));
			}
		}
		/// <summary>List groups</summary>
		private void groups_list() {
			group_list(false);
		}
		/// <summary>List profile</summary>
		/// <param name="clearProfile">True when list is for a drop-down</param>
		private void group_list(bool clearProfile) {
			try {
				Logger.Info(String.Concat(logid, "_group_list:"));
				SearchSettings search = new SearchSettings(UserProfile, Logger, 1, "3", false, true);

				if (clearProfile) {
					UserProfile.Add("LA_Profile_ProfileID", "0");
				}
				string profileID = UserProfile.Value("LA_Profile_ProfileID", "0");
				_Access.ListGroups(profileID, search, clearProfile);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Access.ListXmlRoot as XmlNode, true));

				Logger.Debug(String.Concat(logid, "_profile_list::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_profile_list", String.Concat(error_profile_list, e.Message)));
			}
		}
		/// <summary>Get group</summary>
		private void groups_get() {
			try {
				Logger.Info(String.Concat(logid, "_group_get:"));

				string groupID = _GetQueryID(GROUP_ID);
				Logger.Info(String.Concat(logid, "_group_get:profileID:", groupID));

				_Access.GetGroup(Int32.Parse(groupID));
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Access.ItemXmlRootNode, true));

				Logger.Debug(String.Concat(logid, "_group_get::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_group_get", String.Concat(error_profile_get, e.Message)));
			}
		}
		/// <summary>Add profile</summary>
		private void group_add() {
			try {
				Logger.Info(String.Concat(logid, "_group_add:"));

				string name = UserProfile.Value("name");
				Logger.Debug(String.Concat(logid, "_group_add:name:", name));

				_Access.AddGroup(name);
				//profile_users("0_record_", profileID, true);

				Logger.Debug(String.Concat(logid, "_group_add::finished:group:", _Access.ItemID));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				Logger.Debug(String.Concat(logid, "_group_add:error:", e.Message, "::", e.StackTrace));
				throw (new x_exception("error_group_add", String.Concat(error_group_add, e.Message)));
			}
		}
		/// <summary>Edit profile</summary>
		private void groups_edit() {
			Logger.Info(String.Concat(logid, "_group_edit:"));
			try {

				string groupID = _GetQueryID(GROUP_ID);
				int group_id = Int32.Parse(groupID);

				_Access.GetGroup(group_id);
				string name = UserProfile.Value("name");
				Logger.Debug(String.Concat(logid, "_group_edit", "::group_id:", group_id.ToString(), ":name:", name));

				_Access.UpdateGroup(group_id, name);

				// remove all users from the group and puts them into the default group
				_Access.DeleteGroupUsers(group_id);
				new AccessGroup(xLogger, group_id, _Access).Process(true, "0", UserProfile);

				// remove all rights from the group
				_Access.DeleteGroupCategories(group_id);
				new AccessRight(xLogger, group_id, _Access).Process(true, "1", UserProfile);

				Logger.Info(String.Concat(logid, "_group_edit:ok."));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_group_edit", String.Concat(error_group_edit, e.Message)));
			}
		}
		/// <summary>Delete profile</summary>
		private void groups_delete() {
			try {
				Logger.Info(String.Concat(logid, "_group_delete:"));

				string groupID = _GetQueryID(GROUP_ID);
				_Access.DeleteGroupUsers(Convert.ToInt32(groupID));
				_Access.DeleteGroup(Int32.Parse(groupID));
				////UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Access.ItemXmlRootNode, true));

				Logger.Debug(String.Concat(logid, "_group_delete::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_group_delete", String.Concat(error_group_delete, e.Message)));
			}
		}
		/// <summary>Get group rights</summary>
		private void groups_rights_get() {
			try {
				Logger.Info(String.Concat(logid, "_groupRights_get:"));

				string groupID = _GetQueryID(GROUP_ID);
				Logger.Info(String.Concat(logid, "_groupRights_get:profileID:", groupID));

				_Access.GetGroupRights(Int32.Parse(groupID));
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Access.ItemXmlRootNode, true));

				Logger.Debug(String.Concat(logid, "_groupRights_get::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_groupRights_get", String.Concat(error_group_rights_get, e.Message)));
			}
		}
		/// <summary>Edit group rights</summary>
		private void groups_rights_edit() {
			try {
				Logger.Info(String.Concat(logid, "_groupRights_edit:"));

				string groupID = _GetQueryID(GROUP_ID);
				string name = UserProfile.Value("name");
				Logger.Debug(String.Concat(logid, "_groupRights_edit:groupID", groupID));
				group_categoriesall("1_record_", groupID);

				Logger.Debug(String.Concat(logid, "_groupRights_edit::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_groupRights_edit", String.Concat(error_group_edit, e.Message)));
			}
		}
		/// <summary>List collections</summary>
		private void collections_list() {
			collection_list(false);
		}
		/// <summary>List profile</summary>
		/// <param name="clearProfile">True when list is for a drop-down</param>
		private void collection_list(bool clearProfile) {
			try {
				Logger.Info(String.Concat(logid, "_collection_list:"));
				SearchSettings search = new SearchSettings(UserProfile, Logger, 1, "3", false, true);

				//if (clearProfile) {
				//    UserProfile.Add("LA_Profile_ProfileID", "0");
				//}
				//string profileID = UserProfile.Value("LA_Profile_ProfileID", "0");
				_Access.ListCollections(search, clearProfile);

				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Access.ListXmlRoot as XmlNode, true));

				Logger.Debug(String.Concat(logid, "_profile_list::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_profile_list", String.Concat(error_profile_list, e.Message)));
			}
		}
		/// <summary>Get collection</summary>
		private void collections_get() {
			try {
				Logger.Info(String.Concat(logid, "_collection_get:"));

				string collectionID = _GetQueryID(COLLECTION_ID);
				Logger.Info(String.Concat(logid, "_collection_get:collectionID:", collectionID));

				_Access.GetCollection(Int32.Parse(collectionID));
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(_Access.ItemXmlRootNode, true));

				Logger.Debug(String.Concat(logid, "_collection_get::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_collection_get", String.Concat(error_profile_get, e.Message)));
			}
		}
		/// <summary>Edit Collections</summary>
		private void collections_edit() {
			try {
				Logger.Info(String.Concat(logid, "_collections_edit:"));

				string collectionID = _GetQueryID(COLLECTION_ID);
				//_Access.GetCollection(Int32.Parse(collectionID));
				string name = UserProfile.Value("name");

				Logger.Debug(String.Concat(logid, "_group_edit:groupID", collectionID, ":name:", name));

				//_Access.UpdateGroup(Int32.Parse(groupID), name);
				collection_usersall("0_record_", collectionID);

				Logger.Debug(String.Concat(logid, "_collection_edit::finished:ok"));
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_collection_edit", String.Concat(error_collection_edit, e.Message)));
			}
		}

		/// <summary>Add, update, remove users from a profile</summary>
		private void profile_users(string profilePrefix, string profileID) {
			profile_users(profilePrefix, profileID, false);
		}
		private void profile_users(string profilePrefix, string profileID, bool isNew) {
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string usersCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);
				if (String.IsNullOrEmpty(usersCSV))
					break;

				string[] users = usersCSV.Split(new char[] { '|' });
				Logger.Debug(String.Concat(logid, "_profile_users:", prof, ":", usersCSV, "; profileID:", profileID));

				string testcol = users[0];
				string user_id = users[1];
				Logger.Debug(String.Concat(logid, "_profile_users:user_id:", user_id));

				string user_remove = users[4];
				Logger.Debug(String.Concat(logid, "_profile_users:user_remove:", user_remove));
				if (user_remove == "true") {
					if (!(_IsDefault(testcol) || isNew))
						_Access.DeleteProfileUser(Int32.Parse(profileID), Int32.Parse(user_id));
				} else {
					if (_IsDefault(testcol))  //|| isNew
						_Access.AddGroupUser(Int32.Parse(profileID), Int32.Parse(user_id));
					else
						_Access.UpdateProfileUser(Int32.Parse(profileID), Int32.Parse(user_id));
				}
			}
			Logger.Debug(String.Concat(logid, "_profile_users:finished"));
		}
/*
		private void group_usersall(string groupPrefix, int groupID) {
			Logger.Debug(String.Concat(logid, "_group_usersall:groupPrefix:", groupPrefix, ":groupID:", groupID.ToString()));

			// now add users to the profile
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(groupPrefix, i.ToString());
				string usersCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);
				if (String.IsNullOrEmpty(usersCSV))
					break;

				string[] users = usersCSV.Split(new char[] { '|' });	// eg: 10180|zwetsh|Zwelakhe|Tshabangu|0||
				Logger.Debug(String.Concat(logid, "_group_usersall:", prof, ":", usersCSV, "; profileID:", groupID));

				string user_id = users[1];
				string test = users[5];
				if (test == "true" || test == "1") {	// true when clicked, 1 when exists
					Logger.Debug(String.Concat(logid, "_group_usersall:adding...", user_id));
					_Access.AddGroupUser(groupID, Int32.Parse(user_id));
				}
			}
			Logger.Debug(String.Concat(logid, "_group_usersall:finished"));
		}
*/
		private void group_categoriesall(string groupPrefix, string groupID) {
			Logger.Debug(String.Concat(logid, "_group_usersall:groupPrefix:", groupPrefix, ":groupID:", groupID));

			// first remove all rights from the group and gives them default rights
			int groupid = Int32.Parse(groupID);
			_Access.DeleteGroupCategories(groupid);

			// now add rights to the group
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(groupPrefix, i.ToString());
				string categoriesCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);
				if (String.IsNullOrEmpty(categoriesCSV))
					break;

				string[] groups = categoriesCSV.Split(new char[] { '|' });	// eg: 10180|zwetsh|Zwelakhe|Tshabangu|0||
				Logger.Debug(String.Concat(logid, "_group_usersall:", prof, ":", categoriesCSV, "; profileID:", groupID));
				string category_id = groups[2];
				string test = groups[6];
				if (test == "true" || test == "1") {	// true when clicked, 1 when exists
					Logger.Debug(String.Concat(logid, "_group_Categoriesall:adding...", category_id));
					_Access.AddGroupCategory(groupid, Int32.Parse(category_id), 3);
				} else {
					test = groups[5];
					if (test == "true" || test == "1") {	// true when clicked, 1 when exists
						Logger.Debug(String.Concat(logid, "_group_Categoriesall:adding...", category_id));
						_Access.AddGroupCategory(groupid, Int32.Parse(category_id), 2);
					} else {
						test = groups[4];
						if (test == "true" || test == "1") {	// true when clicked, 1 when exists
							Logger.Debug(String.Concat(logid, "_group_Categoriesall:adding...", category_id));
							_Access.AddGroupCategory(groupid, Int32.Parse(category_id), 1);
						}
					}
				}
			}
			Logger.Debug(String.Concat(logid, "_group_Categoriesall:finished"));
		}

		/// <summary>Add, update, remove profiles from a user</summary>
		private void user_profiles(string profilePrefix, string userID) {
			user_profiles(profilePrefix, userID, false);
		}
		private void user_profiles(string profilePrefix, string userID, bool isNew) {
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(profilePrefix, i.ToString());
				string profilesCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);
				if (String.IsNullOrEmpty(profilesCSV))
					break;

				string[] profiles = profilesCSV.Split(new char[] { '|' });
				Logger.Debug(String.Concat(logid, "_user_profiles:", prof, ":", profilesCSV, "; userID:", userID));

				string testcol = profiles[0];
				string profile_id = profiles[1];
				Logger.Debug(String.Concat(logid, "_user_profiles:profile_id:", profile_id));

				string user_remove = profiles[2];
				Logger.Debug(String.Concat(logid, "_user_profiles:user_remove:", user_remove));
				if (user_remove == "true") {
					if (!(_IsDefault(testcol) || isNew))
						_Access.DeleteProfileUser(Int32.Parse(profile_id), Int32.Parse(userID));
				} else {
					if (_IsDefault(testcol))  //|| isNew
						_Access.AddGroupUser(Int32.Parse(profile_id), Int32.Parse(userID));
					else
						_Access.UpdateProfileUser(Int32.Parse(profile_id), Int32.Parse(userID));
				}
			}
			Logger.Debug(String.Concat(logid, "_user_profiles:finished"));
		}

		private void collection_usersall(string collectionPrefix, string collectionID) {
			Logger.Debug(String.Concat(logid, "_group_usersall:groupPrefix:", collectionPrefix, ":groupID:", collectionID));

			// first remove all users from the collection 
			int collectionid = Int32.Parse(collectionID);
			_Access.DeleteCollectionUsers(collectionid);

			// now add users to the collection
			for (int i = 0; i < MAX_HANDLE; i++) {
				string prof = String.Concat(collectionPrefix, i.ToString());
				string usersCSV = UserProfile.Value(prof, "");
				UserProfile.Clear(prof);
				if (String.IsNullOrEmpty(usersCSV))
					break;

				string[] users = usersCSV.Split(new char[] { '|' });	// eg: 10180|zwetsh|Zwelakhe|Tshabangu|0||
				Logger.Debug(String.Concat(logid, "_collections_usersall:", prof, ":", usersCSV, "; collectionID:", collectionID));

				string user_id = users[1];
				string test = users[5];
				if (test == "true" || test == "1") {	// true when clicked, 1 when exists
					Logger.Debug(String.Concat(logid, "_collection_usersall:adding...", user_id));
					_Access.AddCollectionUser(collectionid, Int32.Parse(user_id));
				}
			}
			Logger.Debug(String.Concat(logid, "_collection_usersall:finished"));
		}




		#endregion

		#region Private utility methods
		#endregion
	}
}