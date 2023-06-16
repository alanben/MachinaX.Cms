using System;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2008-06-14
	Status:		release	
	Version:	2.5.0
	Build:		20100925
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100925:	Started from EconoVault
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
	/// <summary>
	/// This class is a functional wrapper of the access web service
	/// </summary>
	public class Access : CmsWSBase {
		#region Private properties
		#endregion

		#region Constant name strings
		private const string logid = "Access.";
		private const string CONFIG_ACCESS_ID = "access_service";
		private const string CONFIG_USER_ID = "user_service";
		private const string ROOT_NAME = "Access";
		#endregion

		#region Visible properties
		private AccessWS.AccessServiceX accessService;
		public AccessWS.AccessServiceX AccessService {
			get { return accessService; }
		}
		private UserWS.UserServiceX userService;
		public UserWS.UserServiceX UserService {
			get { return userService; }
		}
		#endregion

		#region Constant error strings
		private const string ERROR_WS_RESULT = "Web service error : ";
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public Access(displayX thispage, string award_id) : base(thispage, ROOT_NAME, award_id, typeof(Access), logid) {
			initialize();
		}
		/// <summary>Default constructor</summary>
		public Access(displayX thispage) : base(thispage, ROOT_NAME, typeof(Access), logid) {
			initialize();
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize() {
			xLogger.Debug("initialize");
			xLogger.Debug("initialize", "::id:", CONFIG_ACCESS_ID, "::Url:", _GetUrl(CONFIG_ACCESS_ID));
			accessService = new AccessWS.AccessServiceX();
			accessService.Url = _GetUrl(CONFIG_ACCESS_ID);
			xLogger.Debug("initialize", "::accessService:", accessService.Url);

			userService = new UserWS.UserServiceX();
			userService.Url = _GetUrl(CONFIG_USER_ID);
			xLogger.Debug("initialize", "::userService:", userService.Url);
		}

		#endregion

		#region Public User Methods
		public void GetUser(int UserID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(userService.GetUser(UserID))
					, true)
				);
		}
		public void DeleteAdminUser(int UserID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.RemoveAdminUser(UserID))
					, true)
				);
		}
		/// <summary>List of setup items</summary>
		public void ListAdminUsersDrop(SearchSettings Settings) {
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							accessService.ListAdminUsersAll(false, 0, 0, Settings.Column, Settings.Descending)
						   )
					, true)
				);
		}
		public void ListAdminUsers(SearchSettings Settings) {
			Logger.Debug(String.Concat("ListUsers:settings:", Settings.Page.ToString(), ":", Settings.Column, ":", Settings.Descending.ToString(), ":", Settings.Rows.ToString()));
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							accessService.ListAdminUsers(false, 0, 0, Settings.Page, Settings.Column, Settings.Descending, Settings.Rows)
						   )
					, true)
				);
		}
		public void GetAdminUser(int UserID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.GetAdminUser(UserID))
					, true)
				);
		}

		public void UpdateAdminUser(int UserID, string Username, string Password, string Firstname, string Surname, string Email, string Telephone, string Cellphone, int StatusID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.UpdateAdminUser(UserID, Username, Password, Firstname, Surname, Email, Telephone, Cellphone, StatusID))
					, true)
				);
		}

		public void AddAdminUser(string Username, string Password, string Firstname, string Surname, string Email, string Telephone, string Cellphone) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.AddAdminUser(Username, Password, Firstname, Surname, Email, Telephone, Cellphone))
					, true)
				);
		}
		#endregion

		#region Public Profile Methods
		public void GetGroup(int GroupID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.GetGroup(GroupID))
					, true)
				);
		}
		public void DeleteGroup(int GroupID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.RemoveGroup(GroupID))
					, true)
				);
		}
		public void AddGroup(string Name) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.AddGroup(Name))
					, true)
				);
		}

		public void UpdateGroup(int ProfileID, string Name) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.UpdateGroup(ProfileID, Name))
					, true)
				);
		}
		/// <summary>List of setup items</summary>
		public void ListGroups(string CategoryCollectionID, SearchSettings Settings, bool IsDropDown) {
			Logger.Debug(String.Concat("_ListProfiles:settings:", Settings.Page.ToString(), ":", Settings.Column, ":", Settings.Descending.ToString(), ":", Settings.Rows.ToString()));
			XmlNode result;
			if (IsDropDown)
				result = accessService.ListProfilesDrop(false, AwardID, Int32.Parse(CategoryCollectionID), Settings.Page, Settings.Column, Settings.Descending, Settings.Rows);
			else
				result = accessService.ListGroups(false, Settings.Page, Settings.Column, Settings.Descending, Settings.Rows);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(ListXml.ImportNode(_CheckWSResult(result), true));
		}
		// List, add, edit delete Profile's users
		public void ListProfileUsers(int ProfileID, int UserID, SearchSettings Settings) {
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							accessService.ListProfileUsers(ProfileID, UserID, Settings.Column, Settings.Descending)
						   )
					, true)
				);
		}
		public void AddGroupUser(int GroupID, int UserID) {
			Logger.Debug(String.Concat("_AddGroupUser:GroupID:", GroupID.ToString(), ":UserID:", UserID.ToString()));
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.AddGroupUser(GroupID, UserID))
					, true)
				);
		}
		public void AddGroupCategory(int GroupID, int CategoryID, int RightID) {
			Logger.Debug(String.Concat("_AddGroupCategory:GroupID:", GroupID.ToString(), ":CategoryID:", CategoryID.ToString(), ":RightID", RightID.ToString()));
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.AddGroupCategory(GroupID, CategoryID, RightID))
					, true)
				);
		}

		public void UpdateProfileUser(int ProfileID, int UserID) {
			Logger.Debug(String.Concat("_UpdateProfileUser:ProfileID:", ProfileID.ToString(), ":UserID:", UserID.ToString()));
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.UpdateProfileUser(ProfileID, UserID))
					, true)
				);
		}
		public void DeleteProfileUser(int ProfileID, int UserID) {
			Logger.Debug(String.Concat("_DeleteProfileUser:ProfileID:", ProfileID.ToString(), ":UserID:", UserID.ToString()));
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.RemoveProfileUser(ProfileID, UserID))
					, true)
				);
		}
		public void DeleteGroupUsers(int GroupID) {
			Logger.Debug(String.Concat("_DeleteGroupUsers:GroupID:", GroupID.ToString()));
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.RemoveGroupUsers(GroupID))
					, true)
				);
		}
		public void DeleteGroupCategories(int GroupID) {
			Logger.Debug(String.Concat("_DeleteGroupCategories:GroupID:", GroupID.ToString()));
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.RemoveGroupCategories(GroupID))
					, true)
				);
		}
		/// <summary>List of shows</summary>
		public void ListCollections(SearchSettings Settings, bool IsDropDown) {
			Logger.Debug(String.Concat("_ListCollections:settings:", Settings.Page.ToString(), ":", Settings.Column, ":", Settings.Descending.ToString(), ":", Settings.Rows.ToString()));
			XmlNode result;
			if (IsDropDown)
				result = accessService.ListCollectionsDrop(false, Settings.Page, Settings.Column, Settings.Descending, Settings.Rows);
			else
				result = accessService.ListCollections(false, Settings.Page, Settings.Column, Settings.Descending, Settings.Rows);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(ListXml.ImportNode(_CheckWSResult(result), true));
		}
		/// <summary>Get a show</summary>
		public void GetCollection(int CollectionID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.GetCollection(CollectionID))
					, true)
				);
		}
		public void DeleteCollectionUsers(int CollectionID) {
			Logger.Debug(String.Concat("_DeleteCollectionUsers:GroupID:", CollectionID.ToString()));
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.RemoveCollectionUsers(CollectionID))
					, true)
				);
		}
		public void AddCollectionUser(int CollectionID, int UserID) {
			Logger.Debug(String.Concat("_AddCollectionUser:CollectionID:", CollectionID.ToString(), ":UserID:", UserID.ToString()));
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.AddCollectionUser(CollectionID, UserID))
					, true)
				);
		}
		public void GetGroupRights(int GroupID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.GetRights(GroupID))
					, true)
				);
		}
		public void GetUserRights(int UserID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(accessService.GetUserRights(UserID))
					, true)
				);
		}
		#endregion

	}
}

