/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2015-12-23
	Status:		release	
	Version:	4.0.3
	Build:		20151223
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20151223:	Refactored from CmsXCSV
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {

	using System;
	using System.Text;
	using System.Web;
	using System.Xml;

    /// <summary>
    /// This is a utility class to encapsulte profile and other functionality related to a xxxx.
    /// Typically this class is created within brokers and the display classes
    /// </summary>
	public class CmsXExportCSV : CmsXExportBase {

		private const string error_no_error = "Not an error";
		private const string error_csv = "CmsXExport error::";
		private const string error_csv_nodata = " no CSV data supplied: ";
		private const string TOKEN_COMMA = ",";

        #region Constructors/Destructors
        /// <summary>Default constructor</summary>
        public CmsXExportCSV() : base() {
        }
        /// <summary>Constructor for derived classes</summary>
		public CmsXExportCSV(displayX thispage, x_logger xlogger) : base(thispage, xlogger) {
        }
        #endregion

		/// <summary>Converts the dataXml into CSVs </summary>
		/// <remarks>Note that this was taken from AwardsX.xmlToCsv, but differs in that the first row is assumed not to have data but used to get column names only</remarks>
		/// <returns>Returns a string containing the Xml nodes in CSV format</returns>
		public void XmlToCSV_Deprecated(XmlElement Data, XmlNodeList columns, string csvFileName) {
			xLogger.Debug("xmlToCSV");
			string xpath_nodes = ".//child::*";
			// Check for data and throw exception so message can be served in popup window
			if (Data.SelectSingleNode(xpath_nodes) == null)
				throw new x_exception("error_csv_nodata", String.Concat(error_csv, error_csv_nodata, csvFileName));

			_UIPage.Response.Clear();
			_UIPage.Response.ClearHeaders();
			_UIPage.Response.AddHeader("Content-Disposition", String.Concat("filename=", csvFileName, ".csv"));
			_UIPage.Response.ContentType = "text/csv";
			_UIPage.Response.ContentEncoding = Encoding.GetEncoding(1252);
			int x = 0;
			foreach (XmlNode column in columns) {
				string name = column.SelectSingleNode("prompt").InnerText;
				_UIPage.Response.Write((x == 0) ? "" : TOKEN_COMMA);
				_UIPage.Response.Write(name);
				x++;
			}
			_UIPage.Response.Write("\n");
			// Get data rows
			XmlNodeList DataList = Data.SelectNodes("child::*");
			xLogger.Debug("xmlToCsv::DataList:", DataList.Count.ToString());
			for (int i = 0; i < DataList.Count; i++) {
				XmlNode itemNode = DataList.Item(i);
				int j = 0;
				foreach (XmlNode column in columns) {
					try {
						string value;
						string row = column.SelectSingleNode("value").InnerText;
						XmlNode attNode = itemNode.SelectSingleNode(String.Concat("@", row));
						// replace above with this in case the node required is not found should be, but...
						// rather let error reflect in the data than cause exception or hide issue
						if (attNode == null) {
							attNode = itemNode.SelectSingleNode(row);
						}
						value = (attNode != null) ? attNode.InnerText.Replace(",", ";").Replace("\r\n", " ").Replace('\n', ' ') : "undefined";
						_UIPage.Response.Write((j == 0) ? "" : TOKEN_COMMA);
						_UIPage.Response.Write(value);
						j++;
					} catch (Exception e) {
						xLogger.Debug("xmlToCsv::Error:", e.Message, "::column:", column.OuterXml, "::itemNode:", itemNode.OuterXml);
					}
				}
				_UIPage.Response.Write("\n");
			}
			xLogger.Debug("xmlToCsv::finished:ok");
			_UIPage.Response.Flush();
			_UIPage.Response.End();
		}


		/// <summary>Converts the dataXml into CSVs </summary>
		/// <remarks>Note that this was taken from AwardsX.xmlToCsv, but differs in that the first row is assumed not to have data but used to get column names only</remarks>
		/// <returns>Returns a string containing the Xml nodes in CSV format</returns>
		public void XmlToCSV(XmlElement Data, XmlNodeList columns, string csvFileName) {
			string xpath_nodes = ".//child::*";
			// Check for data and throw exception so message can be served in popup window
			if (Data.SelectSingleNode(xpath_nodes) == null)
				throw new x_exception("error_csv_nodata", String.Concat(error_csv, error_csv_nodata, csvFileName));

			string file = (csvFileName == null) ? Guid.NewGuid().ToString() : String.Concat(csvFileName, "-", DateTime.Now.ToShortDateString());
			_UIPage.Response.Clear();
			_UIPage.Response.ClearHeaders();
			_UIPage.Response.AddHeader("Content-Disposition", String.Concat("filename=", file, ".csv"));
			_UIPage.Response.ContentType = "text/csv";
			_UIPage.Response.ContentEncoding = Encoding.GetEncoding(1252);
			int x = 0;
			foreach (XmlNode column in columns) {
				string name = column.SelectSingleNode("prompt").InnerText;
				_UIPage.Response.Write((x == 0) ? "" : TOKEN_COMMA);
				_UIPage.Response.Write(name);
				//Logger.Debug(String.Concat(logid, " COLUMNS::", column.OuterXml)); 
				x++;
			}
			_UIPage.Response.Write("\n");
			// Get data rows
			XmlNodeList DataList = Data.SelectNodes("child::*");
			xLogger.Debug("xmlToCsv::DataList:", DataList.Count.ToString());
			//for (int i = 0; i < DataList.Count; i++) {
			//    XmlNode itemNode = DataList.Item(i);
			foreach (XmlNode itemNode in DataList) {
				int j = 0;
				foreach (XmlNode column in columns) {
					try {
						XmlElement valel = column.SelectSingleNode("value") as XmlElement;
						string value;
						string row = valel.InnerText;
						//string format = valel.GetAttribute("format");
						string valueformat = valel.GetAttribute("format");
						XmlNode attNode = itemNode.SelectSingleNode(String.Concat("@", row));
						// replace above with this in case the node required is not found should be, but...
						// rather let error reflect in the data than cause exception or hide issue
						if (attNode == null) {
							attNode = itemNode.SelectSingleNode(row);
						}
						if (attNode != null) {
							if (valueformat == "quoted" || valueformat == "quote") {
								if (valel.GetAttribute("linebreaks") == "no") {
									value = attNode.InnerText.Replace("\r\n", " ").Replace('\n', ' ');
								} else {
									value = attNode.InnerText;
								}
							} else if (valueformat == "xhtml") {
								string xhtml = "";
								try {
									xLogger.Debug("xmlToCsv::attNode:", attNode.InnerText);
									xhtml = attNode.InnerText.Replace("&amp;amp;", "&amp;").Replace("&lt;", " ").Replace("&gt;", " ");
									xLogger.Debug("xmlToCsv::xhtml:", xhtml);
									xhtml = HttpUtility.HtmlDecode(xhtml);
									xhtml = xhtml.Replace("&", "&amp;");

									attNode.InnerXml = xhtml;
									attNode.InnerXml = attNode.InnerXml.Replace("&nbsp;", "&#160;");
									((XmlElement)attNode).SetAttribute("decoded", "yes");
									//value = attNode.InnerText.Replace(",", ";").Replace("\r\n", " ").Replace('\n', ' ');
									value = String.Concat("\"", attNode.InnerText.Replace('\"', '\''), "\"");
								} catch (XmlException) {
									xLogger.Debug("xmlToCsv::badxhtml:", xhtml);
									value = "error: badly formed html for description";
								}
							} else {
								value = attNode.InnerText.Replace(",", ";").Replace("\r\n", " ").Replace('\n', ' ');
							}
						} else {
							value = "undefined";
						}
						_UIPage.Response.Write((j == 0) ? "" : TOKEN_COMMA);
						switch (valueformat) {
							case "date_ddmmyyyy":
								DateTime dte;
								if (DateTime.TryParse(value, out dte)) {
									_UIPage.Response.Write(dte.ToString("dd/MM/yyyy"));
								}
								break;
							case "text":
								_UIPage.Response.Write(String.Concat("'", value));
								break;
							case "quote":
								_UIPage.Response.Write(String.Concat("\"", value, "\""));
								break;
							case "force":	// putting an '=' before the double quotes forces the data to be text. (Excel only?)
								_UIPage.Response.Write(String.Concat("=\"", value, "\""));
								break;
							case "quoted":
							default:
								_UIPage.Response.Write(value);
								break;
						}
						j++;
					} catch (Exception e) {
						xLogger.Debug("xmlToCsv::Error:", e.Message, "::column:", column.OuterXml, "::itemNode:", itemNode.OuterXml);
					}
				}
				_UIPage.Response.Write("\n");
			}
			xLogger.Debug("xmlToCsv::Data:", Data.OuterXml);
			xLogger.Debug("xmlToCsv::finished:ok");
			_UIPage.Response.Flush();
			_UIPage.Response.End();
		}

		/// <summary>Converts the dataXml into CSVs </summary>
		/// <remarks>Note that this was taken from xmlToCSV, and columns removed</remarks>
		/// <returns>Returns a string containing the Xml nodes in CSV format</returns>
		public void XmlToCSV(XmlElement Data, string csvFileName) {
			xLogger.Debug("xmlToCSV");
			string xpath_nodes = "child::*";

			// Check for data and throw exception so message can be served in popup window
			if (Data.SelectSingleNode(xpath_nodes) == null)
				throw new x_exception("error_csv_nodata", String.Concat(error_csv, error_csv_nodata, csvFileName));

			_UIPage.Response.Clear();
			_UIPage.Response.ClearHeaders();
			_UIPage.Response.AddHeader("Content-Disposition", String.Concat("filename=", csvFileName, ".csv"));
			_UIPage.Response.ContentType = "text/csv";
			_UIPage.Response.ContentEncoding = Encoding.GetEncoding(1252);

			// Use node names as headings
			XmlNode first = Data.SelectSingleNode(xpath_nodes);
			int k = 0;
			foreach (XmlAttribute attr in first.Attributes) {
				try {
					_UIPage.Response.Write((k == 0) ? "" : TOKEN_COMMA);
					_UIPage.Response.Write(attr.Name);
					k++;
				} catch (Exception e) {
					xLogger.Debug("xmlToCsv::Error:", e.Message, "::attr:", attr.OuterXml);
				}
			}
			_UIPage.Response.Write("\n");

			// Get data rows
			XmlNodeList DataList = Data.SelectNodes(xpath_nodes);
			xLogger.Debug("xmlToCsv::DataList:", DataList.Count.ToString());
			for (int i = 0; i < DataList.Count; i++) {
				XmlNode itemNode = DataList.Item(i);
				int j = 0;
				foreach (XmlAttribute attr in itemNode.Attributes) {
					try {
						String attval = attr.Value;
						string value = (attval != "null") ? attval.Replace(",", ";").Replace("\r\n", " ").Replace('\n', ' ') : "";
						_UIPage.Response.Write((j == 0) ? "" : TOKEN_COMMA);
						_UIPage.Response.Write(value);
						j++;
					} catch (Exception e) {
						xLogger.Debug("xmlToCsv::Error:", e.Message, "::itemNode:", itemNode.OuterXml);
					}
				}
				_UIPage.Response.Write("\n");
			}
			xLogger.Debug("xmlToCsv::finished:ok");
			_UIPage.Response.Flush();
			_UIPage.Response.End();
		}
    }
}
