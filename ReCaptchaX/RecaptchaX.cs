using System;
using System.Collections.Generic;
using System.Text;

using clickclickboom.machinaX;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington	
	Started:	2009-11-09	
	Status:		release	
	Version:	2.2.0
	Build:		20100118
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100118:	Starting point from SpoefRecaptchaX
	-----------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.recaptchaX {
	/// <summary>This class encapsulates the recaptcha into a simple utility object</summary>
	public class RecaptchaX {
		#region Invisible properties
		#endregion

		#region Constants
		private const string DEFAULT_PRIVATE_KEY = "6Ld_lgoAAAAAAEUFoNlhny5LI8CfoqPp-tsIGQtG";
		private const string DEFAULT_PUBLIC_KEY = "6Ld_lgoAAAAAABrml78WDjqgtVxHBlbJIcmlyujR";
		private const string error_recaptcha = "ReCaptcha error: general error -";
		private const string error_incorrect = "ReCaptcha error: incorrect words";
		private const string error_unavailable = "ReCaptcha error: api unavailable";
		#endregion

		#region Visible properties
		private x_config config;
		/// <summary>Configuration object</summary>
		/// <value>Configuration values acces via the object</value>
		public x_config Config {
			get { return config; }
		}

		private string privateKey;
		/// <summary>Private ReCaptcha key as defined in config</summary>
		public string PrivateKey {
			get { return privateKey; }
			set { privateKey = value; }
		}

		private string publicKey;
		/// <summary>Public ReCaptcha key as defined in config</summary>
		public string PublicKey {
			get { return publicKey; }
			set { publicKey = value; }
		}
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public RecaptchaX() {
			initialise();
		}
		#endregion

		#region Public methods
		/// <summary>Recaptcha check</summary>
		public void Check(string challenge, string response, string userhost) {
			try {
				RecaptchaValidator validator = new RecaptchaValidator();

				validator.PrivateKey = privateKey;
				validator.RemoteIP = userhost;
				validator.Response = response;
				validator.Challenge = challenge;

				RecaptchaResponse result = validator.Validate();

				if (result.ErrorCode == "incorrect-captcha-sol")
					throw (new x_exception("error_incorrect", error_incorrect));

				if (result.ErrorCode == "recaptcha-not-reachable")
					throw (new x_exception("error_unavailable", error_unavailable));

				if (!result.IsValid)
					throw (new Exception(result.ErrorCode));

			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_recaptcha", String.Concat(error_recaptcha, e.Message)));
			}
		}
		#endregion

		#region Protected methods
		#endregion

		#region Private methods
		private void initialise() {
			config = new x_config();
			privateKey = config.Value("ReCaptchaX/Key[@id='private']", DEFAULT_PRIVATE_KEY);
			publicKey = config.Value("ReCaptchaX/Key[@id='public']", DEFAULT_PUBLIC_KEY);
		}
		#endregion
	}
}
