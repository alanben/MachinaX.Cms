using System;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-10-20
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20101020:	Started.
	20140112:	Refactored
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>This class is a functional wrapper of the DisplayXLinks web service</summary>
	public class CmsWSSite : CmsWSBase {

		/// <summary>The identifier of the website in the config</summary>
		public string BlogxID { get; set; }

		/// <summary>Determines how the list data is output</summary>
		public ListOutputStyle ListStyle { get; set; }

		/// <summary>Default constructor</summary>
		public CmsWSSite(CmsXProfileX thispage, string root, string blogx_id, Type type, string logger_id) : base(thispage, root, type, logger_id) {
			BlogxID = blogx_id;
			initialize(thispage);
		}

		private void initialize(CmsXProfileX thispage) {
			/*
			xLogger.Debug("initialize", "::Page.Label:", UIPage.Label);

			string listOutputStyle = Config.Value(String.Format(Cms.BLOGX_LISTOUTPUTSTYLE, UIPage.Label), ListStyle.ToString());
			xLogger.Debug("initialize", "::listOutputStyle:", listOutputStyle);

			if (listOutputStyle == ListOutputStyle.XmlDirect.ToString()) {
				ListStyle = ListOutputStyle.XmlDirect;
			} else if (listOutputStyle == ListOutputStyle.Datatable.ToString()) {
				ListStyle = ListOutputStyle.Datatable;
			} else {
				ListStyle = ListOutputStyle.XmlTransform;
			}
			*/
			ListStyle = thispage.ListStyle;
			xLogger.Debug("initialize", "::ListStyle:", ListStyle.ToString());
		}

		#region Write Methods
		//protected void _WriteList(XmlNode result) {
		//	xLogger.Debug("_WriteList", "::ListStyle:", ListStyle.ToString());

		//	if (ListStyle == ListOutputStyle.Datatable) {
		//		writeDatatableList(result);
		//	} else if (ListStyle == ListOutputStyle.XmlDirect) {
		//		writeDirectList(result);
		//	} else {
		//		writeServiceList(result);
		//	}
		//}

		//protected void _WriteItem(XmlNode result) {
		//	xLogger.Debug("_WriteItem", "::ListStyle:", ListStyle.ToString());

		//	if (ListStyle == ListOutputStyle.Datatable) {
		//		//writeDatatableItem(result);
		//		writeDirectItem(result);
		//	} else if (ListStyle == ListOutputStyle.XmlDirect) {
		//		writeDirectItem(result);
		//	} else {
		//		writeServiceItem(result);
		//	}
		//}

		/// <summary>Check result items and write JSON to response stream (Datatables.net format)</summary>
		private void writeDatatableList(XmlNode result) {
			xLogger.Debug("writeDatatableList", "::sEcho:", UserProfile.Value("sEcho"));
			xLogger.Debug("writeDatatableList", "::result:", result.OuterXml);

			datatable.Output output = new datatable.Output(UserProfile);

			UIPage.Response.ContentType = "application/json; charset=utf-8";
			UIPage.Response.Write(output.Get(_CheckWSResult(result)));
			UIPage.Response.Flush();
			UIPage.Response.End();

			xLogger.Debug("writeDatatableList:ok");
		}

		/// <summary>Check result items and write xml to response stream</summary>
		private void writeDirectList(XmlNode result) {
			xLogger.Debug("writeDirectList");

			_CheckWSResult(result);
			UIPage.Response.ContentType = "text/xml";
			UIPage.Response.Write(Items.OuterXml);

			xLogger.Debug("writeDirectList:ok");
		}

		/// <summary>Check result and write xml to response stream</summary>
		private void writeDirectItem(XmlNode result) {
			_CheckWSResult(result);
			UIPage.Response.ContentType = "text/xml";
			UIPage.Response.Write(result.OuterXml);
		}
		#endregion
	}
}

