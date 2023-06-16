using System;
using System.Xml;

using Newtonsoft.Json;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2014-01-20
	Status:		release	
	Version:	4.0.2
	Build:		20140120
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20140120:	Started
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX.datatable {
    /// <summary>
    /// This is a utility class that handles the output of data for jquery.dataTables plugin
    /// </summary>
    public class Output : Base {

		protected XmlElement ResultXml { get; set; }
		protected XmlElement ResultItems {
			get { return ResultXml.SelectSingleNode(Cms.SELECT_ITEMS) as XmlElement; }
		}
		protected XmlElement ResultItem {
			get { return ResultXml.SelectSingleNode(Cms.SELECT_ITEMSITEM) as XmlElement; }
		}

		private string sEcho { 
			get { return Profile.Value("sEcho", "0"); }
			set { Profile.Add("sEcho", value); } 
		}

        #region Constructors/Destructors
        /// <summary>Default constructor</summary>
		public Output(x_userprofile profile) : base(profile) {
        }
		public Output(x_userprofile profile, XmlElement resultXml) : base(profile) {
			ResultXml = resultXml;
        }
        #endregion

		public string Get(XmlElement resultXml) {
			ResultXml = resultXml;
			return Get();
		}
		public string Get() {
			xLogger.Debug("Get", "::ResultXml:", ResultXml.OuterXml);

			int iTotalRecords = 1, iTotalDisplayRecords = 1, iRows = 1, iPages = 1;
			string pages = ResultXml.GetAttribute("pages");
			string rows = ResultXml.GetAttribute("rows");
			//xLogger.Debug("writeDatatableList", "::pages:", pages, "::rows:", rows);

			if (!String.IsNullOrEmpty(rows)) {
				iRows = Int32.Parse(rows);
			}
			if (!String.IsNullOrEmpty(pages)) {
				iPages = Int32.Parse(pages);
			}
			iTotalRecords = iRows * iPages;
			iTotalDisplayRecords = iRows;

			addItemsElement("sEcho", sEcho);
			addItemsElement("iTotalRecords", iTotalRecords.ToString());
			addItemsElement("iTotalDisplayRecords", iTotalDisplayRecords.ToString());

			ResultItems.RemoveAttribute("pages");
			ResultItems.RemoveAttribute("rows");

			string json = JsonConvert.SerializeXmlNode(ResultItems, Newtonsoft.Json.Formatting.Indented, true);
			xLogger.Debug("Get", "::json:", json);

			return json;
		}
		private void addItemsElement(string name, string value) {
			ResultItems.PrependChild(ResultXml.OwnerDocument.CreateElement(name)).InnerText = value;
		}
    }
}
