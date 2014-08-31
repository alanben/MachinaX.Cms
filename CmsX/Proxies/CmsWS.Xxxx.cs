using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-11-15
	Status:		release	
	Version:	2.5.0
	Build:		20101115
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20101115:	Started.
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>This class is a functional wrapper of the DisplayXXxxxs web service</summary>
	public class Xxxxs : CmsWSBase {
		#region Private properties
		#endregion

		#region Constant name strings
		private const string logid = "Xxxxs.";
		private const string CONFIG_USER_ID = "xxxxs_service";
		private const string ROOT_NAME = "Xxxxs";
		#endregion

		#region Visible properties
		//private iBurstXxxxsWS.iBurstXxxxsX xxxxsService;
		//public iBurstXxxxsWS.iBurstXxxxsX XxxxsService {
		//    get { return xxxxsService; }
		//}
		#endregion

		#region Constant error strings
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public Xxxxs(displayX thispage) : base(thispage, ROOT_NAME, typeof(Xxxxs), logid) {
			initialize();
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize() {
			xLogger.Debug("initialize::");
			//xxxxsService = new iBurstXxxxsWS.iBurstXxxxsX();
			//xxxxsService.Url = _GetUrl(CONFIG_USER_ID);
			//xLogger.Debug("initialize:xxxxsService:", xxxxsService.Url);
		}

		#endregion

		#region Public Methods
/*
		/// <summary>
		/// Get a list of all xxxxs
		/// </summary>
		public void List(string XxxxsID) {
			xLogger.Debug("List", "::XxxxsID:", XxxxsID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							xxxxsService.ListXxxxs(XxxxsID)
						   )
					, true)
				);
		}

		public void GetXxxx(string XxxxsID, int XxxxID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(xxxxsService.Get(XxxxsID, XxxxID))
					, true)
				);
		}

		public void AddXxxx(string XxxxsID, string Name, string Label, string Image, string Width, string Height) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(xxxxsService.Add(XxxxsID, Name, Label, Image, Width, Height))
					, true)
				);
		}

		public void UpdateXxxx(string XxxxsID, int XxxxID, string Name, string Label, string Image, string Width, string Height) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(xxxxsService.Edit(XxxxsID, XxxxID, Name, Label, Image, Width, Height))
					, true)
				);
		}

		public void DeleteXxxx(string XxxxsID, int XxxxID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(xxxxsService.Delete(XxxxsID, XxxxID))
					, true)
				);
		}
*/
		#endregion
	}
}

