using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-11-15
	Status:		release	
	Version:	2.5.0
	Build:		20101218
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20101218:	Started.
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>This class is a functional wrapper of the DisplayXTopics web service</summary>
	public class Topics : CmsWSBase {
		#region Private properties
		#endregion

		#region Constant name strings
		private const string logid = "Topics.";
		private const string CONFIG_USER_ID = "topics_service";
		private const string ROOT_NAME = "Topics";
		#endregion

		#region Visible properties
		//private ContentXTopicsWS.ContentXTopics topicsService;
		//public ContentXTopicsWS.ContentXTopics TopicsService {
		//    get { return topicsService; }
		//}
		#endregion

		#region Constant error strings
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public Topics(displayX thispage, string blogx_id) : base(thispage, ROOT_NAME, blogx_id, typeof(Topics), logid) {
			initialize();
		}
		#endregion

		#region Private Methods
		/// <summary>Initiate properties</summary>
		private void initialize() {
			xLogger.Debug("initialize::");
			//topicsService = new ContentXTopicsWS.ContentXTopics();
			//topicsService.Url = _GetUrl(CONFIG_USER_ID);
			//xLogger.Debug("initialize:topicsService:", topicsService.Url);
		}

		#endregion

		#region Public Methods
/*
		/// <summary>
		/// Get a list of all topics
		/// </summary>
		public void List(string TopicsID) {
			xLogger.Debug("List", "::TopicsID:", TopicsID);
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(
					ListXml.ImportNode(
						_CheckWSResult(
							topicsService.ListTopics(TopicsID)
						   )
					, true)
				);
		}

		public void GetTopic(string TopicsID, int TopicID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(topicsService.Get(TopicsID, TopicID))
					, true)
				);
		}

		public void AddTopic(string TopicsID, string Name, string Label, string Image, string Width, string Height) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(topicsService.Add(TopicsID, Name, Label, Image, Width, Height))
					, true)
				);
		}

		public void UpdateTopic(string TopicsID, int TopicID, string Name, string Label, string Image, string Width, string Height) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(topicsService.Edit(TopicsID, TopicID, Name, Label, Image, Width, Height))
					, true)
				);
		}

		public void DeleteTopic(string TopicsID, int TopicID) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(
				   ItemXml.ImportNode(
						_CheckWSResult(topicsService.Delete(TopicsID, TopicID))
					, true)
				);
		}
*/
		#endregion
	}
}

