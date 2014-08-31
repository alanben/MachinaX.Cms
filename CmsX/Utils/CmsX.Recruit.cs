using System;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2012-11-06
	Status:		release	
	Version:	4.0.1
	Build:		20121106
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20121106:	Started
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
    /// <summary>
    /// This is a utility class to encapsulte profile and other functionality related to a user.
    /// Typically this class is created within brokers and the display classes
    /// </summary>
    public class CmsXRecruit {

		protected x_userprofile Profile {
			get;
			set;
		}

        public const string PROPERTY_ID = "ID";
		public const string PROPERTY_USERNAME = "Username";
        public const string PROPERTY_PASSWORD = "Password";
        public const string PROPERTY_TOKEN = "RXToken";
        public const string PROPERTY_FIRSTNAME = "FirstName";
        public const string PROPERTY_SURNAME = "Surname";
        public const string PROPERTY_FULLNAME = "Fullname";
        public const string PROPERTY_EMAIL = "Email";
		public const string PROPERTY_TELNO = "Telephone";
		public const string PROPERTY_CELLNO = "Cellphone";
		public const string PROPERTY_TYPE = "Type";
		public const string PROPERTY_TYPEID = "TypeID";
		public const string PROPERTY_PERSONID = "PersonID";
        public const string PROPERTY_SIGNDATE = "SignupDate";

		public const string PROPERTY_RECAPTCHA = "RecaptchaX";
		public const string PROPERTY_RECAPTCHA_CHALLENGE = "recaptcha_challenge_field";
		public const string PROPERTY_RECAPTCHA_RESPONSE = "recaptcha_response_field";
		public const string WANT_RECAPTCHA = "yes";

		public string RecruitID {
			get { return Profile.Value(PROPERTY_ID); }
			set { Profile.Add(PROPERTY_ID, value); }
		}
		public string Username {
			get { return Profile.Value(PROPERTY_USERNAME); }
			set { Profile.Add(PROPERTY_USERNAME, value); }
        }
        public string Password {
			get { return Profile.Value(PROPERTY_PASSWORD); }
			set { Profile.Add(PROPERTY_PASSWORD, value); }
        }
        public string Firstname {
			get { return Profile.Value(PROPERTY_FIRSTNAME); }
			set { Profile.Add(PROPERTY_FIRSTNAME, value); }
        }
        public string Surname {
			get { return Profile.Value(PROPERTY_SURNAME); }
			set { Profile.Add(PROPERTY_SURNAME, value); }
        }
        public string Fullname {
			get { return Profile.Value(PROPERTY_FULLNAME); }
			set { Profile.Add(PROPERTY_FULLNAME, value); }
        }
        public string Email {
			get { return Profile.Value(PROPERTY_EMAIL); }
			set { Profile.Add(PROPERTY_EMAIL, value); }
        }
        public string Telephone {
			get { return Profile.Value(PROPERTY_TELNO); }
			set { Profile.Add(PROPERTY_TELNO, value); }
        }
		public string Cellphone {
			get { return Profile.Value(PROPERTY_CELLNO); }
			set { Profile.Add(PROPERTY_CELLNO, value); }
        }
        public string Token {
			get { return Profile.Value(PROPERTY_TOKEN); }
			set { Profile.Add(PROPERTY_TOKEN, value); }
        }
        public string PersonID {
            get { return (Profile.Value(PROPERTY_PERSONID, "0")); }
            set { Profile.Add(PROPERTY_PERSONID, value); }
        }
		public string TypeName {
			get { return Profile.Value(PROPERTY_TYPE); }
			set { Profile.Add(PROPERTY_TYPE, value); }
		}
		public string TypeID {
			get { return Profile.Value(PROPERTY_TYPEID); }
			set { Profile.Add(PROPERTY_TYPEID, value); }
        }
		public string SignupDate {
			get { return Profile.Value(PROPERTY_SIGNDATE); }
			set { Profile.Add(PROPERTY_SIGNDATE, value); }
		}


		public int ID {
			get { return Convert.ToInt32(RecruitID); }
			set { Profile.Add(PROPERTY_ID, value.ToString()); }
		}
		public int PersonIID {
			get { return Convert.ToInt32(PersonID); }
			set { Profile.Add(PROPERTY_PERSONID, value.ToString()); }
		}
		public int TypeIID {
			get { return Convert.ToInt32(TypeID); }
			set { Profile.Add(PROPERTY_TYPEID, value.ToString()); }
		}

		// Properties for ReCaptcha
		public string Recaptcha {
			get { return Profile.Value(PROPERTY_RECAPTCHA); }
			set { Profile.Add(PROPERTY_RECAPTCHA, value); }
		}
		public bool WantRecaptcha {
			//get { return (Recaptcha == WANT_RECAPTCHA); }
			get { return !String.IsNullOrEmpty(Recaptcha); }
		}
		public string RecaptchaChallenge {
			get { return Profile.Value(PROPERTY_RECAPTCHA_CHALLENGE); }
			set { Profile.Add(PROPERTY_RECAPTCHA_CHALLENGE, value); }
		}
		public string RecaptchaResponse {
			get { return Profile.Value(PROPERTY_RECAPTCHA_RESPONSE); }
			set { Profile.Add(PROPERTY_RECAPTCHA_RESPONSE, value); }
		}

		/// <summary>Constructor - to be used only for serialisation</summary>
		public CmsXRecruit() {
        }
        /// <summary>Constructor</summary>
        public CmsXRecruit(x_userprofile profile){
            Profile = profile;
        }

		/// <summary>
		/// Populates profile from result of recruit add/edit
		/// </summary>
		/// <param name="user"></param>
		public void Set(XmlElement user) {
			RecruitID	= user.GetAttribute("ID");
			Token		= user.GetAttribute("Token");
			Username	= user.GetAttribute("UserName");
			Firstname	= user.GetAttribute("FirstName");
			Surname		= user.GetAttribute("Surname");
			Email		= user.GetAttribute("EMail");
			Telephone	= user.GetAttribute("TelNo");
			Cellphone	= user.GetAttribute("CellPhone");
			SignupDate	= user.GetAttribute("SignupDate");
			TypeName	= user.GetAttribute("Type");
			TypeID		= user.GetAttribute("TypeID");
			PersonID	= user.GetAttribute("UserID");
			Fullname	= String.Concat(Firstname, (Surname != "") ? " " : "", Surname);
		}

		/// <summary>
		/// Populates profile from result of facebook login
		/// </summary>
		/// <param name="user"></param>
		public void Set(string first_name, string middle_name, string last_name, string name, string username, string id, string email) {
			Username	= username;
			Firstname	= first_name;
			Surname		= last_name;
			Email		= email;
			Fullname	= name;
		}

        /// <summary>Clear essential profile values (ie not all eg retain 'user.Username' in profile if exists</summary>
        public void Reset() {
            Profile.Clear(PROPERTY_TOKEN);
            Profile.Clear(PROPERTY_PASSWORD);
			Profile.Logout();
        }
    }
}
