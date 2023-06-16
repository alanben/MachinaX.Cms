
/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.3
	Build:		20161018
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Refactored from LoeriesAdmin
	20120716:	Added Telephone, changed Cellno to Cellphone
				Added Set (from Passport Login)
	20161018:	Trim Username and Password when setting
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {

	using System;
	using System.Xml;
	
	/// <summary>
    /// This is a utility class to encapsulte profile and other functionality related to a user.
    /// Typically this class is created within brokers and the display classes
    /// </summary>
    public class CmsXUser {
        #region Invisible properties
		protected x_userprofile Profile {
			get;
			set;
		}
        #endregion

        #region Public constants
        public const string REMEMBER_ON						= "on";
        public const string REMEMBER_OFF					= "off";
        public const string PROPERTY_USERNAME				= "Username";
        public const string PROPERTY_PASSWORD				= "Password";
        public const string PROPERTY_TOKEN					= "PXToken";
        public const string PROPERTY_FIRSTNAME				= "Firstname";
        public const string PROPERTY_SURNAME				= "Surname";
        public const string PROPERTY_FULLNAME				= "Fullname";
        public const string PROPERTY_EMAIL					= "Email";
        public const string PROPERTY_CELLNO					= "Cellphone";
		public const string PROPERTY_TELNO					= "Telephone";
		public const string PROPERTY_USERID					= "UserID";
		public const string PROPERTY_PERSONID				= "PersonID";
        public const string PROPERTY_REMEMBER				= "RememberMe";
        public const string PROPERTY_HINT_ID				= "HintID";
        public const string PROPERTY_HINT_ANSWER			= "HintAnswer";
        public const string PROPERTY_COOKIE					= "TokenX";
        public const string PROPERTY_FILES_ALL				= "FilesAll";
        public const string PROPERTY_FILES_VIDEO			= "FilesVideo";
        public const string PROPERTY_FILES_VOICE			= "FilesVoice";
        public const string PROPERTY_FRIENDID				= "FriendID";
        public const string PROPERTY_FILEID					= "FileID";
		public const string PROPERTY_FILETYPEID				= "FileTypeID";
		public const string PROPERTY_FILEUID				= "FileUID";
		public const string PROPERTY_FILENAME				= "FileName";
		public const string PROPERTY_FILEOWNERID			= "FileOwnerID";
		public const string PROPERTY_RECAPTCHA				= "RecaptchaX";
		public const string PROPERTY_RECAPTCHA_CHALLENGE	= "recaptcha_challenge_field";
		public const string PROPERTY_RECAPTCHA_RESPONSE		= "recaptcha_response_field";
		public const string WANT_RECAPTCHA					= "yes";
		#endregion

        #region Visible properties
		/// <remarks>
		/// Note re use of ID, UserID and PersonID
		///		PersonID:	The authentication ID of the user (ie the passport ID)
		///		UserID:		The service id of the user. The user service record will have a reference to the PersonID
		///	Note that immediately post authentication (ie CmsXBrokerPassport.login) the UserID = PersonID
		///	Typically post authentication the user service will be called to load the user, loading the UserID with the service ID
		/// </remarks>
		
		// Removed to clarify the above
		//public int ID {
		//	get { return Convert.ToInt32(UserID); }
		//	set { Profile.Add(PROPERTY_USERID, value); }
		//}

		public string UserID {
			get { return (Profile.Value(PROPERTY_USERID)); }
			set { Profile.Add(PROPERTY_USERID, value); }
		}
		public int UserIID { get { return Convert.ToInt32(UserID); } }

		public string PersonID {
			get { return (Profile.Value(PROPERTY_PERSONID, "0")); }
			set { Profile.Add(PROPERTY_PERSONID, value); }
		}
		public int PersonIID { get { return Convert.ToInt32(PersonID); } set { Profile.Add(PROPERTY_PERSONID, value.ToString()); } }

		public string Username {
            get { return Profile.Value(PROPERTY_USERNAME).Trim(); }
			set { Profile.Add(PROPERTY_USERNAME, value.Trim()); }
        }
        public string Password {
            get { return Profile.Value(PROPERTY_PASSWORD).Trim(); }
            set { Profile.Add(PROPERTY_PASSWORD, value.Trim()); }
        }
        public string Firstname {
            get { return (Profile.Get(PROPERTY_FIRSTNAME) == null) ? "" : Profile.Get(PROPERTY_FIRSTNAME).InnerText; }
            set { Profile.Add(PROPERTY_FIRSTNAME, value); }
        }
        public string Surname {
            get { return (Profile.Get(PROPERTY_SURNAME) == null) ? "" : Profile.Get(PROPERTY_SURNAME).InnerText; }
            set { Profile.Add(PROPERTY_SURNAME, value); }
        }
        public string Fullname {
            get { return (Profile.Get(PROPERTY_FULLNAME) == null) ? "" : Profile.Get(PROPERTY_FULLNAME).InnerText; }
            set { Profile.Add(PROPERTY_FULLNAME, value); }
        }
        public string Email {
            get { return (Profile.Get(PROPERTY_EMAIL) == null) ? "" : Profile.Get(PROPERTY_EMAIL).InnerText; }
            set { Profile.Add(PROPERTY_EMAIL, value); }
        }
        public string Telephone {
            get { return (Profile.Get(PROPERTY_TELNO) == null) ? "" : Profile.Get(PROPERTY_TELNO).InnerText; }
            set { Profile.Add(PROPERTY_TELNO, value); }
        }
		public string Cellphone {
            get { return (Profile.Get(PROPERTY_CELLNO) == null) ? "" : Profile.Get(PROPERTY_CELLNO).InnerText; }
            set { Profile.Add(PROPERTY_CELLNO, value); }
        }
        public string Token {
            get { return (Profile.Get(PROPERTY_TOKEN) == null) ? "" : Profile.Get(PROPERTY_TOKEN).InnerText; }
            set { Profile.Add(PROPERTY_TOKEN, value); }
        }
        public string RememberMe {
            get { return (Profile.Get(PROPERTY_REMEMBER) == null) ? "" : Profile.Get(PROPERTY_REMEMBER).InnerText; }
            set { Profile.Add(PROPERTY_REMEMBER, value); }
        }
        public bool Remember {
            get { return (RememberMe == REMEMBER_ON); }
			set { Profile.Add(PROPERTY_REMEMBER, value? REMEMBER_ON : REMEMBER_OFF); }
		}
        public string HintID {
            get { return (Profile.Get(PROPERTY_HINT_ID) == null) ? "" : Profile.Get(PROPERTY_HINT_ID).InnerText; }
            set { Profile.Add(PROPERTY_HINT_ID, value); }
        }
        public string HintAnswer {
            get { return (Profile.Get(PROPERTY_HINT_ANSWER) == null) ? "" : Profile.Get(PROPERTY_HINT_ANSWER).InnerText; }
            set { Profile.Add(PROPERTY_HINT_ANSWER, value); }
        }
        public string Cookie {
            get { return (Profile.Get(PROPERTY_COOKIE) == null) ? "" : Profile.Get(PROPERTY_COOKIE).InnerText; }
        }
        // Properties for files, video and voice mail
        public string AllFiles {
            get { return (Profile.Get(PROPERTY_FILES_ALL) == null) ? "" : Profile.Get(PROPERTY_FILES_ALL).InnerText; }
            set { Profile.Add(PROPERTY_FILES_ALL, value); }
        }
        public string VideoFiles {
            get { return (Profile.Get(PROPERTY_FILES_VIDEO) == null) ? "" : Profile.Get(PROPERTY_FILES_VIDEO).InnerText; }
            set { Profile.Add(PROPERTY_FILES_VIDEO, value); }
        }
        public string VoiceFiles {
            get { return (Profile.Get(PROPERTY_FILES_VOICE) == null) ? "" : Profile.Get(PROPERTY_FILES_VOICE).InnerText; }
            set { Profile.Add(PROPERTY_FILES_VOICE, value); }
        }
        public bool IsCustomer {
            get { return (Profile.LoginPattern == Cms.PATTERN_CUSTOMER); ; }
			set { Profile.LoginPattern = (value) ? Cms.PATTERN_CUSTOMER : Cms.PATTERN_PUBLIC; }
        }
        // Property for friends and files
        public string FriendID {
            get { return (Profile.Get(PROPERTY_FRIENDID) == null) ? "" : Profile.Get(PROPERTY_FRIENDID).InnerText; }
            set { Profile.Add(PROPERTY_FRIENDID, value); }
        }
        public string FileID {
            get { return (Profile.Get(PROPERTY_FILEID) == null) ? "" : Profile.Get(PROPERTY_FILEID).InnerText; }
            set { Profile.Add(PROPERTY_FILEID, value); }
        }
		public string FileTypeID {
		    get { return (Profile.Get(PROPERTY_FILETYPEID) == null) ? "" : Profile.Get(PROPERTY_FILETYPEID).InnerText; }
		    set { Profile.Add(PROPERTY_FILETYPEID, value); }
		}
		public string FileUID {
		    get { return (Profile.Get(PROPERTY_FILEUID) == null) ? "" : Profile.Get(PROPERTY_FILEUID).InnerText; }
		    set { Profile.Add(PROPERTY_FILEUID, value); }
		}
		public string FileName {
		    get { return (Profile.Get(PROPERTY_FILENAME) == null) ? "" : Profile.Get(PROPERTY_FILENAME).InnerText; }
		    set { Profile.Add(PROPERTY_FILENAME, value); }
		}
        public string FileOwnerID {
            get { return (Profile.Get(PROPERTY_FILEOWNERID) == null) ? "" : Profile.Get(PROPERTY_FILEOWNERID).InnerText; }
            set { Profile.Add(PROPERTY_FILEOWNERID, value); }
        }
		public int CurrentID {
			get { return Int32.Parse(Profile.Value(PROPERTY_USERID, "0")); }
			set { Profile.Add(PROPERTY_USERID, value.ToString()); }
		}
		// Properties for ReCaptcha
		public string Recaptcha {
			get { return (Profile.Get(PROPERTY_RECAPTCHA) == null) ? "" : Profile.Get(PROPERTY_RECAPTCHA).InnerText; }
			set { Profile.Add(PROPERTY_RECAPTCHA, value); }
		}
		public bool WantRecaptcha {
			//get { return (Recaptcha == WANT_RECAPTCHA); }
			get { return !String.IsNullOrEmpty(Recaptcha); }
        }
		public string RecaptchaChallenge {
			get { return (Profile.Get(PROPERTY_RECAPTCHA_CHALLENGE) == null) ? "" : Profile.Get(PROPERTY_RECAPTCHA_CHALLENGE).InnerText; }
			set { Profile.Add(PROPERTY_RECAPTCHA_CHALLENGE, value); }
		}
		public string RecaptchaResponse {
			get { return (Profile.Get(PROPERTY_RECAPTCHA_RESPONSE) == null) ? "" : Profile.Get(PROPERTY_RECAPTCHA_RESPONSE).InnerText; }
			set { Profile.Add(PROPERTY_RECAPTCHA_RESPONSE, value); }
		}
		public bool IsLoggedOn {
			get { return (!String.IsNullOrEmpty(Token)); }
		}

		// Properties for Cookies
		public string CookieSkin {
			get { return Profile.Value(Cms.COOKIE_SKIN); }
			set { Profile.Add(Cms.COOKIE_SKIN, value); }
		}
		public string CookieToken {
			get { return Profile.Value(Cms.COOKIE_TOKEN); }
			set { Profile.Add(Cms.COOKIE_TOKEN, value); }
		}
		public string CookieWizard {
			get { return Profile.Value(Cms.COOKIE_WIZARD); }
			set { Profile.Add(Cms.COOKIE_WIZARD, value); }
		}
		public string CookieCheckToken {
			get { return Profile.Value(Cms.COOKIE_CHECK, Cms.DEFAULT_CHECK); }
			set { Profile.Add(Cms.COOKIE_CHECK, value); }
		}
		public bool CheckCookieToken {
			get { return (Convert.ToBoolean(CookieCheckToken)); }
			set { Profile.Add(Cms.COOKIE_CHECK, value.ToString()); }
		}
		#endregion

		/// <summary>Constructor - to be used only for serialisation</summary>
		public CmsXUser() {
        }
        /// <summary>Constructor</summary>
        public CmsXUser(x_userprofile profile){
            Profile = profile;
        }

		/// <summary>
        /// Gets the value of a  named user profile parameter
        /// </summary>
        /// <param name="name">The name of the user profile parameter</param>
        /// <returns>The value of the user profile parameter</returns>
        public string Get(string name) {
            return (Profile.Get(name) == null) ? "" : Profile.Get(name).InnerText;
        }

		/// <summary>
		/// Populates profile from result of passport login
		/// </summary>
		/// <param name="user"></param>
		public virtual void Set(XmlElement person) {
			//UserID	= person.GetAttribute("UserID");
			PersonID	= person.GetAttribute("UserID");
			Username	= person.SelectSingleNode("UserName").InnerText;
			Firstname	= person.SelectSingleNode("FirstName").InnerText;
			Surname		= person.SelectSingleNode("Surname").InnerText;
			Email		= person.SelectSingleNode("EMail").InnerText;
			Cellphone	= person.SelectSingleNode("CellPhone").InnerText;
			Telephone	= person.SelectSingleNode("TelNo").InnerText;
			Fullname = String.Concat(Firstname, (Surname != "") ? " " : "", Surname);
		}

        public void Set(string token, string personid) {
            Token = token;
			//UserID = personid;
			PersonID = personid;
			Profile.LoginPattern = Cms.PATTERN_CUSTOMER;
        }
		public void Set(string token, string personid, string uname, string pword) {
            Token = token;
			//UserID = personid;
			PersonID = personid;
			Username = uname;
            Password = pword;
			Profile.LoginPattern = Cms.PATTERN_CUSTOMER;
        }
        public void Set(string token, string userid, string uname, string pword, string fname, string sname) {
            Set(token, userid, uname, pword);
            Firstname = fname;
            Surname = sname;
            Fullname = String.Concat(fname, (sname != "") ? " " : "", sname);
        }
        public void SetName(string fname, string sname) {
            Firstname = fname;
            Surname = sname;
            Fullname = String.Concat(fname, (sname != "") ? " " : "", sname);
        }
		
		/// <summary>Clear profile values</summary>
		public virtual void Reset(bool ClearAll) {
			if (ClearAll) {
				Profile.Clear(PROPERTY_USERNAME);
				Profile.Clear(PROPERTY_FIRSTNAME);
				Profile.Clear(PROPERTY_SURNAME);
				Profile.Clear(PROPERTY_FULLNAME);
				Profile.Clear(PROPERTY_EMAIL);
				Profile.Clear(PROPERTY_CELLNO);
				Profile.Clear(PROPERTY_TELNO);
				Profile.Clear(PROPERTY_USERID);
				Profile.Clear(PROPERTY_PERSONID);
				Profile.Clear(PROPERTY_REMEMBER);
				Profile.Clear(PROPERTY_HINT_ID);
				Profile.Clear(PROPERTY_HINT_ANSWER);
				Profile.Clear(PROPERTY_COOKIE);
				Profile.Clear(PROPERTY_FILES_ALL);
				Profile.Clear(PROPERTY_FILES_VIDEO);
				Profile.Clear(PROPERTY_FILES_VOICE);
				Profile.Clear(PROPERTY_FRIENDID);
				Profile.Clear(PROPERTY_FILEID);
				Profile.Clear(PROPERTY_FILETYPEID);
				Profile.Clear(PROPERTY_FILEUID);
				Profile.Clear(PROPERTY_FILENAME);
				Profile.Clear(PROPERTY_FILEOWNERID);
				Profile.Clear(PROPERTY_RECAPTCHA);
			}
			reset();
        }
		/// <summary>Clear essential profile values (ie not all eg retain 'user.Username' in profile if exists</summary>
		public virtual void Reset() {
			reset();
        }

		private void reset() {
            Profile.Clear(PROPERTY_TOKEN);
            Profile.Clear(PROPERTY_PASSWORD);
			Profile.Logout();
			Profile.LoginPattern = Cms.PATTERN_PUBLIC;
            RememberMe = REMEMBER_OFF;
		}
    }
}
