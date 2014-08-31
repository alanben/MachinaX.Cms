using System;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

//using clickclickboom.machinaX.blogX;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2011-10-12
	Status:		release	
	Version:	2.6.0
	Build:		20111012
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20111012:	Started
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>
    /// This is a utility class to encapsulte functionality related to the importation of series and data.
    /// Typically this class is created within brokers and the display classes
    /// </summary>
	public class YyyyImporter : Importer {
        #region Invisible properties
		private Yyyys _Yyyys;
		#endregion

        #region Private constants
		private const string logid = "YyyyImporter.";
		#endregion

        #region Public constants
        #endregion

        #region Visible properties
        #endregion

        #region Constructors/Destructors
        /// <summary>Default constructor</summary>
        public YyyyImporter() {
        }
        /// <summary>Constructor for derived classes</summary>
        public YyyyImporter(x_userprofile uprofile, Yyyys yyyyws) : base(uprofile, logid) {
			_Yyyys = yyyyws;
        }
        #endregion

        #region Public methods
		/// <summary>
		/// Read bulk XmlDocument and import into database via web service layer
		/// </summary>
		public void Import(XmlDocument outputdoc) {
			importSeries(outputdoc, UploadDoc);
		}

        #endregion

        #region Protected methods
        #endregion

        #region Private methods
		private void importSeries(XmlDocument outdoc, XmlDocument indoc) {
			XmlElement item = null;
			XmlElement items = outdoc.SelectSingleNode(Cms.SELECT_ALL_ITEMS) as XmlElement;
			int counter = 0;
			foreach (XmlNode seriesxnode in indoc.SelectNodes(Cms.SELECT_SERVICEX_ITEM)) {
				XmlElement seriesEl = seriesxnode as XmlElement;
				string name = seriesEl.GetAttribute("name");
				string status = seriesEl.GetAttribute("status");
				xLogger.Debug("importSeries::name:", name, "status:", status);
				if (status == "ok") {	// dont import if not 'ok'
					// get parameters for web service and call
					string desc = seriesEl.GetAttribute("desc");
					int source_id = Int32.Parse(seriesEl.GetAttribute("source_id"));
					int type_id = Int32.Parse(seriesEl.GetAttribute("type_id"));
					//string startdate = EconoVault.GetStartDate(seriesEl.GetAttribute("periodstart"), type_id);
					//xLogger.Debug("importSeries::startdate:", startdate);

					item = items.AppendChild(outdoc.CreateElement("item")) as XmlElement;
					item.SetAttribute("row", counter.ToString());
					item.SetAttribute("name", name);
					item.SetAttribute("desc", desc);
					item.SetAttribute("source_id", source_id.ToString());
					item.SetAttribute("type_id", type_id.ToString());
					try {
						//econo.AddSeries(name, desc, (int)SeriesArchive.current, source_id, (int)SeriesStatus.active, type_id);
						//int series_id = Int32.Parse(econo.ItemID);
						//item.SetAttribute("id", series_id.ToString());

						counter++;
						//int number = importData(outdoc, indoc, seriesEl, series_id, startdate, type_id);
						//item.SetAttribute("count", number.ToString());

						// Set output item nod
						item.SetAttribute("source", seriesEl.GetAttribute("source"));
						item.SetAttribute("type", seriesEl.GetAttribute("type"));
						item.SetAttribute("expect", seriesEl.GetAttribute("count"));
						item.SetAttribute("periodstart", seriesEl.GetAttribute("periodstart"));
						item.SetAttribute("periodend", seriesEl.GetAttribute("periodend"));
						item.SetAttribute("status", "ok");
					} catch (Exception e) {
						item.InnerText = String.Concat("Error:", e.Message, "::", e.StackTrace);
						xLogger.Debug("importSeries::Error:", item.InnerText);
						item.SetAttribute("status", "notok");
					}
				}
			}
		}
		private int importData(XmlDocument outdoc, XmlDocument indoc, XmlElement series, int seriesid, string start, int typeid) {
			//xLogger.Debug("importData::series:", series.OuterXml);
			xLogger.Debug("importData::seriesid:", seriesid.ToString());
			
			XmlElement vectors = series.SelectSingleNode("vectors") as XmlElement;
			int counted = Int32.Parse(series.GetAttribute("count"));
			xLogger.Debug("importData::counted:", counted.ToString());
			
			string values = "";
			int counter = 0;
			foreach (XmlNode vectorxnode in vectors.SelectNodes("vector")) {
				counter++;
				//xLogger.Debug("importData::counter:", counter.ToString()); 
				XmlElement vector = vectorxnode as XmlElement;
				values = String.Concat(values, vector.GetAttribute("value"), (counter == counted)? "" : ",");
			}
			xLogger.Debug("importData::values:", values);
			//_Yyyys.AddVectors(seriesid, start, values, typeid);
			return counter;
		}

        #endregion
    }
}
