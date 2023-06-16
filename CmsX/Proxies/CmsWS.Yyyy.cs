using System.Xml;

//using clickclickboom.machinaX;
//using XXBoom.MachinaX.BlogX.CmsX;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2011-10-03
	Status:		release	
	Version:	2.6.0
	Build:		20111003
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20111003:	Started
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
	/// <summary>This class is a functional wrapper of the DisplayXYyyys web service</summary>
	public class Yyyys : CmsWSBase {
		#region Private properties
		#endregion

		#region Constant name strings
		private const string logid = "Yyyys.";
		private const string CONFIG_ROOT = "ServiceX";
		private const string CONFIG_ID = "yyyys_service";
		private const string ROOT_NAME = "Yyyys";
		#endregion

		#region Visible properties
		//private YyyysWS.Yyyy service;
		//public YyyysWS.Yyyy Service {
		//    get { return service; }
		//}
		#endregion

		#region Constant error strings
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public Yyyys(CmsXProfileX thispage) : base(thispage, ROOT_NAME, typeof(Yyyys), logid) {
			initialize();
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize() {
			xLogger.Debug("initialize::");
			//service = new YyyysWS.Yyyy();
			//Service.Url = _GetUrl(CONFIG_ROOT, CONFIG_ID);
			//xLogger.Debug("initialize:service:", Service.Url);
		}

		#endregion

		#region Public Methods
/*
		/// <summary>
		/// Get a list of all yyyys
		/// </summary>
		public void List(string YyyysID) {
			xLogger.Debug("List", "::YyyysID:", YyyysID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							Service.ListYyyys(YyyysID)
						   )
					, true)
				);
		}

		public void GetYyyy(string YyyysID, int YyyyID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(Service.Get(YyyysID, YyyyID))
					, true)
				);
		}

		public void AddYyyy(string YyyysID, string Name, string Label, string Image, string Width, string Height) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(Service.Add(YyyysID, Name, Label, Image, Width, Height))
					, true)
				);
		}

		public void UpdateYyyy(string YyyysID, int YyyyID, string Name, string Label, string Image, string Width, string Height) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(Service.Edit(YyyysID, YyyyID, Name, Label, Image, Width, Height))
					, true)
				);
		}

		public void DeleteYyyy(string YyyysID, int YyyyID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(Service.Delete(YyyysID, YyyyID))
					, true)
				);
		}
*/
		#endregion
	}
}

