/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington	
	Started:	2015-11-24
	Status:		release	
	Version:	4.0.3
	Build:		20180817
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20151124:	Starting point RecaptchaX
	20180817:	Changed default keys to non-valid key for error checking of config
	-----------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.recaptchaX {

	using clickclickboom.machinaX;
	using Newtonsoft.Json;
	using System;
	using System.Net;
	using System.Xml;

	/// <summary>This class encapsulates the recaptcha into a simple utility object</summary>
	public class Recaptcha2X {

		private const string DEFAULT_PRIVATE_KEY	= "Private_Key_not_configured";	//"6Ld_lgoAAAAAAEUFoNlhny5LI8CfoqPp-tsIGQtG";
		private const string DEFAULT_PUBLIC_KEY		= "Public_Key_not_configured";	//"6Ld_lgoAAAAAABrml78WDjqgtVxHBlbJIcmlyujR";

		private const string error_recaptcha	= "ReCaptcha error: general error -";
		private const string error_incorrect	= "ReCaptcha error: incorrect words";
		private const string error_unavailable	= "ReCaptcha error: api unavailable";

		private x_config config;
		/// <summary>Configuration object</summary>
		public x_config Config {
			get { return config; }
		}

		/// <summary>The logger object</summary>
		public x_logger xLogger { get; set; }

		/// <summary>Private ReCaptcha key as defined in config</summary>
		public string PrivateKey { get; set; }

		/// <summary>Public ReCaptcha key as defined in config</summary>
		public string PublicKey { get; set; }

		/// <summary>Default constructor</summary>
		public Recaptcha2X() {
			initialise();
		}

		private void initialise() {
			config = new x_config();
			xLogger = new x_logger(typeof(Recaptcha2X), "Recaptcha2X", false, ":");

			PrivateKey = config.Value("ReCaptchaX/Key[@id='private']", DEFAULT_PRIVATE_KEY);
			PublicKey = config.Value("ReCaptchaX/Key[@id='public']", DEFAULT_PUBLIC_KEY);
			//xLogger.Debug("initialise:", "::PublicKey:", PublicKey);    //, "::PrivateKey:", PrivateKey
		}

		/// <summary>Recaptcha check</summary>
		public void Check(string EncodedResponse) {
			xLogger.Info("Check:");

			try {
				string googleReply = "", googleURI = "https://www.google.com/recaptcha/api/siteverify";
				string parameters = string.Format("secret={0}&response={1}", PrivateKey, EncodedResponse);
				using (WebClient client = new WebClient()) {
					client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
					googleReply = client.UploadString(googleURI, parameters);
				}
				//string googleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", PrivateKey, EncodedResponse));
				xLogger.Info("Check:", "::googleReply:", googleReply);

				XmlDocument result = JsonConvert.DeserializeXmlNode(googleReply, "Result");
				xLogger.Debug("Check:", "::result:", result.OuterXml);

				if (result.SelectSingleNode("//success").InnerText != "true") {
					// Actually test error codes... but for now...
					xLogger.Debug("Check:", "::PublicKey:", PublicKey);
					//xLogger.Debug("Check:", "::PrivateKey:", PrivateKey);

					XmlNode errorcodes = result.SelectSingleNode("//error-codes");
					if (errorcodes != null) {
						string code = errorcodes.InnerText;
						if (code == "missing-input-secret" || code == "invalid-input-secret")  {
							throw (new x_exception("error_unavailable", error_unavailable));
						} else if (code == "missing-input-response" || code == "invalid-input-response") {
							throw (new x_exception("error_incorrect", error_incorrect));
						} else {
							throw (new Exception(code));
						}
					} else {
						throw (new x_exception("error_unavailable", error_unavailable));
					}
				}

			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				xLogger.Debug("Check:", "::error:", e.Message);
				throw (new x_exception("error_unavailable", error_unavailable));
			}
		}

		/// <summary>Deprecated: Recaptcha check version1</summary>
		public void Check(string challenge, string response, string userhost) {
			try {
				RecaptchaValidator validator = new RecaptchaValidator();

				validator.PrivateKey = PrivateKey;
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

	}
}
