using System;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	2.6.0
	Build:		20111025
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100925:	Started from EconoVault
	20111025:	Renamed to Users
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
	/// <summary>
	/// This class is a functional wrapper of the judging web service
	/// </summary>
	public class Users : CmsWSBase {
		#region Private properties
		#endregion

		#region Constant name strings
		private const string logid = "User";
		private const string CONFIG_USER_ID = "user_service";
		private const string ROOT_NAME = "User";
		#endregion

		#region Visible properties
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
		public Users(displayX thispage, string award_id) : base(thispage, ROOT_NAME, award_id) {
			initialize();
		}
		/// <summary>Default constructor</summary>
		public Users(displayX thispage) : base(thispage, ROOT_NAME) {
			initialize();
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize() {
			//Logger.Debug(String.Concat("initialize::"));
			userService = new UserWS.UserServiceX();
			userService.Url = _GetUrl(CONFIG_USER_ID);
			//Logger.Debug(String.Concat("initialize:userService:", userService.Url));
		}

		#endregion

		#region Public Methods
		/// <summary>List of setup items</summary>
		public void ListUsers(int StatusID, SearchSettings Settings) {
		    ListXmlRoot.RemoveAll();
		    ListXmlRoot.AppendChild(
		            ListXml.ImportNode(
		                _CheckWSResult(
		                    userService.ListUsers(false, StatusID, Settings.Page, Settings.Column, Settings.Descending, Settings.Rows)
		                   )
		            , true)
		        );
		}		
		
		public void GetUser(int UserID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(userService.GetUser(UserID))
					, true)
				);
		}

		public void GetUser(string Username) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(userService.GetUserName(Username))
					, true)
				);
		}

		public void DeleteUser(int UserID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(userService.RemoveUser(UserID))
					, true)
				);
		}

		public void AddUser(string Username, string Password, string Firstname, string Surname, string Email, string Telephone, string Cellphone) {
			xLogger.Debug(String.Concat("AddUser::"));
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(userService.AddUser(Username, Password, Firstname, Surname, Email, Telephone, Cellphone))
					, true)
				);
		}

		public void UpdateUser(int UserID, string Username, string Password, string Firstname, string Surname, string Email, string Telephone, string Cellphone, int StatusID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(userService.UpdateUser(UserID, Username, Password, Firstname, Surname, Email, Telephone, Cellphone, StatusID))
					, true)
				);
		}

		#endregion
	}
}

