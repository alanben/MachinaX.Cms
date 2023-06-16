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
	/// This class is a functional wrapper of the judging web service
	/// </summary>
	public class LinkLog : CmsWSBase {
		#region Private properties
		#endregion

		#region Constant name strings
		private const string logid = "LinkLog.";
		private const string CONFIG_USER_ID = "linklog_service";
		private const string ROOT_NAME = "LinkLog";
		#endregion

		#region Visible properties
		private LinkLogWS.LinkLogServiceX linklogService;
		public LinkLogWS.LinkLogServiceX LinkLogService {
			get { return linklogService; }
		}
		#endregion

		#region Constant error strings
		private const string ERROR_WS_RESULT = "Web service error : ";
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public LinkLog(displayX thispage, string award_id) : base(thispage, ROOT_NAME, award_id, typeof(Access), logid) {
			initialize();
		}
		/// <summary>Default constructor</summary>
		public LinkLog(displayX thispage) : base(thispage, ROOT_NAME, typeof(Access), logid) {
			initialize();
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize() {
			xLogger.Debug("initialize::");
			linklogService = new LinkLogWS.LinkLogServiceX();
			linklogService.Url = _GetUrl(CONFIG_USER_ID);
			xLogger.Debug("initialize:linklogService:", linklogService.Url);
		}

		#endregion

		#region Public Methods
		public void GetLinkLog(int LinkLogID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(linklogService.GetLinkLog(LinkLogID))
					, true)
				);
		}

		public void ListLinkLogsAll(int PersonID, SearchSettings Settings) {
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							linklogService.ListLinkLogsAll(false, PersonID, Settings.Column, Settings.Descending)
						   )
					, true)
				);
		}

		public void ListLinkLogs(int PersonID, SearchSettings Settings) {
			xLogger.Debug("ListLinkLogs::sort:", Settings.Column, "::desc:", Settings.Descending.ToString());
		    ListXmlRoot.RemoveAll();
		    ListXmlRoot.AppendChild(
		            ListXml.ImportNode(
		                _CheckWSResult(
							linklogService.ListLinkLogs(false, PersonID, Settings.Page, Settings.Column, Settings.Descending, Settings.Rows)
		                   )
		            , true)
		        );
		}

		public void SearchLinkLogs(bool IsCSV, int PersonIDSrchType, string PersonID, int NameSrchType, string Name, int LinkSrchType, string Link,
								int DateStartSrchType, string DateStart, int DateEndSrchType, string DateEnd,
								SearchSettings search) {
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							linklogService.SearchLinkLogs(IsCSV, PersonIDSrchType, PersonID, NameSrchType, Name, LinkSrchType, Link,
								DateStartSrchType, DateStart, DateEndSrchType, DateEnd,
								(IsCSV) ? 0 : search.Page, search.Column, search.Descending, search.Rows)
							)
						, true)
				);
		}

		public void DeleteLinkLog(int LinkLogID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(linklogService.RemoveLinkLog(LinkLogID))
					, true)
				);
		}

		public void AddLinkLog(string Link, string Token, int PersonID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(linklogService.AddLinkLog(Link, Token, PersonID))
					, true)
				);
		}

		public void UpdateLinkLog(int LinkLogID, string Link, string Token, int PersonID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(linklogService.UpdateLinkLog(LinkLogID, Link, Token, PersonID))
					, true)
				);
		}

		#endregion
	}
}

