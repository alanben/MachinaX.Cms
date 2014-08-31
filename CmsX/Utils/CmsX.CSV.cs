using System;
using System.Text;
using System.Web;
using System.Xml;

using log4net;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	2.6.0
	Build:		20111213
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100722:	Refactored from legacy
	20100823:	Cleanup of legacy
	20111213:	Refactored EconoVaultCSV into this.
	-----------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>
	/// The EconoVaultCSV contains the code for CSV conversion and output
	/// </summary>
    public class CmsXCSV {
		#region Invisible properties
		private string id = "csv";
		#endregion

		#region Constant name strings
		private const string logid = "CmsXCSV.";
		protected const string TOKEN_COMMA = ",";
		#endregion

		#region Constant error strings
		private const string error_no_error			= "Not an error";
		private const string error_csv			= "CmsXCSV error::";
		private const string error_csv_login	= " Error logging in to account: ";
        private const string error_csv_email    = " Error updating email notification: ";
		private const string error_csv_nodata	= " no CSV data supplied: ";
		#endregion

		#region Constant name strings
		#endregion

		#region Visible properties
		protected x_logger xLogger;
		private displayX uipage;
		/// <summary>The UIPage of the processing broker</summary>
		public displayX UIPage {
			get { return uipage; }
			set { uipage = value; }
		}
		/// <summary>
		/// The CSV filter node in the User profile
		/// </summary>
		public XmlElement UserFilters {
			get { return uipage.UserProfile.Node(String.Format("{0}filters", id)); }
		}
		public XmlElement UserFilter {
			get { return UserFilters.SelectSingleNode(String.Format("{0}filter[@name='{1}']", id, PageFilterName)) as XmlElement; }
		}
		public string UserFilePath {
			get { return uipage.Server.MapPath(String.Format("Defaults/{0}.xml", uipage.UserProfile.Value("UserID", "default"))); }
		}
		public string PageFilterName {
			get { return (PageFilter == null) ? "" : PageFilter.GetAttribute("name"); }
		}
		public XmlElement PageFilter {
			get { return uipage.Content.SelectSingleNode(String.Format("//{0}filters/{0}filter", id)) as XmlElement; }
		}
		public XmlElement PageFilters {
			get { return uipage.Content.SelectSingleNode(String.Format("//{0}filters", id)) as XmlElement; }
		}

		protected string FilterName {
			get { return String.Format("{0}filter", id); }
		}
		protected string FilterXml {
			get { return String.Format("<{0}filters/>", id); }
		}
		protected string SelectFilters {
			get { return String.Format("/{0}filters", id); }
		}
		#endregion

		#region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXCSV(displayX thispage) {
			xLogger = new x_logger(typeof(CmsXCSV), logid, false, true);
			uipage = thispage;
			log4net.Config.XmlConfigurator.Configure();
		}
		public CmsXCSV(displayX thispage, string prefix, string logId, Type logtype) {
			xLogger = new x_logger(logtype, logId, false, true);
			id = prefix;
			uipage = thispage;
			log4net.Config.XmlConfigurator.Configure();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Saves the CSV columns 
		/// </summary>
		public void SaveFilters(bool saveUser) {
			xLogger.Debug("SaveFilters::saveUser:", saveUser.ToString());
			SaveFilters();
			if (saveUser) {
				xLogger.Debug("SaveFilters::UserFilePath:", UserFilePath);
				// Now save the filters node to a file to be read (at login?)
				XmlDocument filterxml = new XmlDocument();
				filterxml.LoadXml(UserFilters.OuterXml);
				filterxml.Save(UserFilePath);
				xLogger.Debug("SaveFilters::saved:");
			}
		}
		/// <summary>
		/// Saves the CSV columns 
		/// </summary>
		public void SaveFilters() {
			string param = uipage.QueryParam("itemselector", "-");
			XmlElement content = uipage.Content.SelectSingleNode("//content") as XmlElement;
			XmlDocument csvParamDoc = getParamXml(param, content);

			if (UserFilters != null) {
				XmlElement userfilter = UserFilter;
				if (userfilter == null) {
					userfilter = uipage.UserProfile.AddNode(UserFilters, FilterName, "");
				}
				if (userfilter != null) {
					XmlNode subobjs = userfilter.SelectSingleNode("subobjs");
					if (subobjs != null)
						userfilter.RemoveChild(subobjs);
					userfilter.AppendChild(uipage.UserProfile.ProfileXml.ImportNode(csvParamDoc.SelectSingleNode("//subobjs"), true));
					userfilter.SetAttribute("name", PageFilterName);
				}
			}
			xLogger.Debug("SaveFilters::ok:");
		}
		/// <summary>
		/// Loads the CSV Filters into the user profile
		/// </summary>
		public void LoadFilters() {
			xLogger.Debug("LoadFilters::UserFilePath:", UserFilePath);
			if (UserFilters != null) {
				XmlDocument filterxml = new XmlDocument();
				try {
					filterxml.Load(UserFilePath);
				} catch (System.IO.FileNotFoundException e) {
					xLogger.Debug("SetFilters::FileNotFoundException:", e.Message);
					filterxml.LoadXml(FilterXml);
				}
				uipage.UserProfile.AddNode(filterxml.SelectSingleNode(SelectFilters) as XmlElement);
			}
		}
		/// <summary>
		/// Sets the CSV Filter defaults on the page from the user profile
		/// </summary>
		public void SetFilters() {
			xLogger.Debug("SetFilters:");
			XmlElement userfilter = UserFilter;

			if (userfilter != null) {
				// first remove defaults from page filters
				foreach (XmlNode filter in PageFilter.SelectNodes("subobjs/subwf_obj")) {
					((XmlElement)filter).RemoveAttribute("default");
				}

				// Move matching page filters to the bottom in the order of the user filters and set default 
				XmlNode pagefilterobjs = PageFilter.SelectSingleNode("subobjs");
				foreach (XmlNode filter in userfilter.SelectNodes("subobjs/subwf_obj")) {
					string val = filter.SelectSingleNode("value").InnerText;
					XmlNode userfilterobj = PageFilter.SelectSingleNode(String.Concat("subobjs/subwf_obj[value/text() = '", val, "']"));
					if (userfilterobj != null) {
						XmlElement filterel = pagefilterobjs.AppendChild(userfilterobj) as XmlElement;
						filterel.SetAttribute("default", "yes");
					}
				}
			}
		}

		/// <summary>
		/// Gets the CSV by column
		/// </summary>
		/// <param name="Data"></param>
		public void GetColumns(XmlElement Data) {
			GetColumns(Data, true);
		}
		/// <summary>
		/// Gets the CSV by column
		/// </summary>
		/// <param name="Data"></param>
		public void GetColumns(XmlElement Data, bool WantQuery) {
			string csvFileName = uipage.Parameters.LinkName;
			xLogger.Debug("GetColumns::WantQuery:", WantQuery.ToString(), "::csvFileName:", csvFileName);
			if (WantQuery) {
				string param = uipage.QueryParam("itemselector", "-");
				xLogger.Debug("GetColumns::param:", param);
				GetColumns(Data, csvFileName, param);
			} else {
				GetColumns(Data, csvFileName);
			}
		}
		/// <summary>
		/// Gets the CSV (all columns)
		/// </summary>
		/// <param name="Data">The data to convert to CSV</param>
		/// <param name="xmlFileName">The page xml filename</param>
		public void GetColumns(XmlElement Data, string csvFileName) {
		}
		/// <summary>
		/// Gets the CSV by column
		/// </summary>
		/// <param name="Data">The data to convert to CSV</param>
		/// <param name="xmlFileName">The page xml filename</param>
		public void GetColumns(XmlElement Data, XmlElement columnSubobjs) {
			string csvFileName = uipage.Parameters.LinkName;
			GetColumns(Data, columnSubobjs, csvFileName);
		}
		/// <summary>
		/// Gets the CSV by column
		/// </summary>
		/// <param name="Data">The data to convert to CSV</param>
		/// <param name="xmlFileName">The page xml filename</param>
		public void GetColumns(XmlElement Data, XmlElement columnSubobjs, string csvFileName) {
			xLogger.Debug("GetColumns::csvFileName:", csvFileName);
			xLogger.Debug("GetColumns::Data:", Data.OuterXml);
			xLogger.Debug("GetColumns::columnSubobjs:", columnSubobjs.OuterXml);
			XmlNodeList entriesColumns = columnSubobjs.SelectNodes("subwf_obj");
			_XmlToOutput(Data, entriesColumns, csvFileName);
		}
		public void Output(XmlElement Data, string csvFileName) {
			xLogger.Debug("Output::csvFileName:", csvFileName);
			xLogger.Debug("Output::Data:", Data.OuterXml);
			_XmlToOutput(Data, csvFileName);
		}
		/// <summary>
		/// Gets the CSV by column
		/// </summary>
		/// <param name="Data">The data to convert to CSV</param>
		/// <param name="xmlFileName">The page xml filename</param>
		/// <param name="csvFileName">The name of the CSV file</param>
		/// <param name="param">A list of selected field parameters</param>
		public void GetColumns(XmlElement Data, string xmlFileName, string csvFileName, string param) {
			XmlDocument csvDoc = new XmlDocument();
			csvDoc.Load(uipage.Server.MapPath(xmlFileName));
			//xLogger.Debug(csvDoc.OuterXml));

			XmlDocument csvParamDoc = getParamXml(param, csvDoc);
			//xLogger.Debug(csvParamDoc.OuterXml));

			XmlNodeList entriesColumns = csvParamDoc.SelectNodes("//subwf_obj");
			_XmlToOutput(Data, entriesColumns, csvFileName);
		}
		/// <summary>
		/// Gets the CSV by column
		/// </summary>
		/// <param name="Data">The data to convert to CSV</param>
		/// <param name="csvFileName">The name of the CSV file</param>
		/// <param name="param">A list of selected field parameters</param>
		public void GetColumns(XmlElement Data, string csvFileName, string param) {
			XmlDocument csvDoc = getPageXml();
			//xLogger.Debug("GetColumns::csvDoc:", csvDoc.OuterXml));

			XmlDocument csvParamDoc = getParamXml(param, csvDoc);
			//xLogger.Debug("GetColumns::csvParamDoc:", csvParamDoc.OuterXml));

			XmlNodeList entriesColumns = csvParamDoc.SelectNodes("//subwf_obj");
			_XmlToOutput(Data, entriesColumns, csvFileName);
		}
		#endregion
		
		#region Protected methods
		/// <summary>
		/// Converts xml data to output format. Overridden by inheriting class to implement another output format
		/// </summary>
		/// <param name="Data">The xml data node (typically the 'items' node)</param>
		/// <param name="Columns">A list of output column elemements</param>
		/// <param name="FileName">The name of the output file</param>
		protected virtual void _XmlToOutput(XmlElement Data, XmlNodeList Columns, string FileName) {
			xmlToCSV(Data, Columns, FileName);
		}
		/// <summary>
		/// To be overridden in inheriting class
		/// </summary>
		/// <param name="Data"></param>
		/// <param name="FileName"></param>
		protected virtual void _XmlToOutput(XmlElement Data, string FileName) {
			xmlToCSV(Data, FileName);
		}
		#endregion

		#region Private methods
		/// <summary>
		///	Gets the user selected subset of the xml fields defined in the page xml csvfilters
		/// </summary>
		/// <param name="param">A list of selected field parameters</param>
		/// <param name="pagexml">The page xml document</param>
		/// <returns>An xml document containg the nodes selected by the user from the page xml field list</returns>
		private XmlDocument getParamXml(string param, XmlDocument pagexml) {
			return getParamXml(param, pagexml.DocumentElement);
		}
		private XmlDocument getParamXml(string param, XmlElement pagexml) {
			//xLogger.Debug("getParamXml::pagexml:", (pagexml == null) ? "null" : pagexml.OuterXml));
			string[] parameters = param.Split(new Char[] { ',' });
			XmlDocument csvFilteredDoc = new XmlDocument();
			csvFilteredDoc.LoadXml("<subobjs/>");

			XmlNode paramsNode = csvFilteredDoc.SelectSingleNode("subobjs");
			//xLogger.Debug("getParamXml::parameters:", parameters.Length.ToString()));
			for (int x = 0; x < parameters.Length; x++) {
				//xLogger.Debug("getParamXml::x:", x.ToString(), "parameters[x]:", parameters[x]));
				XmlNode paramNode = pagexml.SelectSingleNode(String.Concat("//csvfilters/csvfilter/subobjs/subwf_obj[value = '", parameters[x], "']"));
				XmlElement filterEl = paramsNode.AppendChild(paramsNode.OwnerDocument.ImportNode(paramNode, true)) as XmlElement;
				filterEl.RemoveAttribute("default");
			}
			return csvFilteredDoc;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private XmlDocument getPageXml() {
			string fileTempl = "pages/{0}/{1}/{2}/{3}.xml";
			XmlElement pageElem = uipage.Parameters.LinkPage;
			string xmlFileName = String.Format(fileTempl, pageElem.GetAttribute("sect"), pageElem.GetAttribute("sub"), pageElem.GetAttribute("proc"), pageElem.GetAttribute("file"));
			XmlDocument csvDoc = new XmlDocument();
			csvDoc.Load(uipage.Server.MapPath(xmlFileName));
			return csvDoc;
		}


		/// <summary>Converts the dataXml into CSVs </summary>
		/// <remarks>Note that this was taken from AwardsX.xmlToCsv, but differs in that the first row is assumed not to have data but used to get column names only</remarks>
		/// <returns>Returns a string containing the Xml nodes in CSV format</returns>
		private void xmlToCSV_Deprecated(XmlElement Data, XmlNodeList columns, string csvFileName) {
			xLogger.Debug("xmlToCSV"); 
			string xpath_nodes = ".//child::*";
			// Check for data and throw exception so message can be served in popup window
			if (Data.SelectSingleNode(xpath_nodes) == null)
				throw new x_exception("error_csv_nodata", String.Concat(error_csv, error_csv_nodata, csvFileName));

			uipage.Response.Clear();
			uipage.Response.ClearHeaders();
			uipage.Response.AddHeader("Content-Disposition", String.Concat("filename=", csvFileName, ".csv"));
			uipage.Response.ContentType = "text/csv";
			uipage.Response.ContentEncoding = Encoding.GetEncoding(1252);
			int x = 0;
			foreach (XmlNode column in columns) {
				string name = column.SelectSingleNode("prompt").InnerText;
				uipage.Response.Write((x == 0) ? "" : TOKEN_COMMA);
				uipage.Response.Write(name);
				x++;
			}
			uipage.Response.Write("\n");
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
						value = (attNode != null)? attNode.InnerText.Replace(",", ";").Replace("\r\n", " ").Replace('\n', ' ') : "undefined";
						uipage.Response.Write((j == 0) ? "" : TOKEN_COMMA);
						uipage.Response.Write(value);
						j++;
					} catch (Exception e) {
						xLogger.Debug("xmlToCsv::Error:", e.Message, "::column:", column.OuterXml, "::itemNode:", itemNode.OuterXml);
					}
				}
				uipage.Response.Write("\n");
			}
			xLogger.Debug("xmlToCsv::finished:ok");
			uipage.Response.Flush();
			uipage.Response.End();
		}


		/// <summary>Converts the dataXml into CSVs </summary>
		/// <remarks>Note that this was taken from AwardsX.xmlToCsv, but differs in that the first row is assumed not to have data but used to get column names only</remarks>
		/// <returns>Returns a string containing the Xml nodes in CSV format</returns>
		private void xmlToCSV(XmlElement Data, XmlNodeList columns, string csvFileName) {
			string xpath_nodes = ".//child::*";
			// Check for data and throw exception so message can be served in popup window
			if (Data.SelectSingleNode(xpath_nodes) == null)
				throw new x_exception("error_csv_nodata", String.Concat(error_csv, error_csv_nodata, csvFileName));

			string file = (csvFileName == null) ? Guid.NewGuid().ToString() : String.Concat(csvFileName, "-", DateTime.Now.ToShortDateString());
			uipage.Response.Clear();
			uipage.Response.ClearHeaders();
			uipage.Response.AddHeader("Content-Disposition", String.Concat("filename=", file, ".csv"));
			uipage.Response.ContentType = "text/csv";
			uipage.Response.ContentEncoding = Encoding.GetEncoding(1252);
			int x = 0;
			foreach (XmlNode column in columns) {
				string name = column.SelectSingleNode("prompt").InnerText;
				uipage.Response.Write((x == 0) ? "" : TOKEN_COMMA);
				uipage.Response.Write(name);
				//Logger.Debug(String.Concat(logid, " COLUMNS::", column.OuterXml)); 
				x++;
			}
			uipage.Response.Write("\n");
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
						uipage.Response.Write((j == 0) ? "" : TOKEN_COMMA);
						switch (valueformat) {
							case "date_ddmmyyyy":
								DateTime dte;
								if (DateTime.TryParse(value, out dte)) {
									uipage.Response.Write(dte.ToString("dd/MM/yyyy"));
								}
								break;
							case "text":
								uipage.Response.Write(String.Concat("'", value));
								break;
							case "quote":
								uipage.Response.Write(String.Concat("\"", value, "\""));
								break;
							case "force":	// putting an '=' before the double quotes forces the data to be text. (Excel only?)
								uipage.Response.Write(String.Concat("=\"", value, "\""));
								break;
							case "quoted":
							default:
								uipage.Response.Write(value);
								break;
						}
						j++;
					} catch (Exception e) {
						xLogger.Debug("xmlToCsv::Error:", e.Message, "::column:", column.OuterXml, "::itemNode:", itemNode.OuterXml);
					}
				}
				uipage.Response.Write("\n");
			}
			xLogger.Debug("xmlToCsv::Data:", Data.OuterXml);
			xLogger.Debug("xmlToCsv::finished:ok");
			uipage.Response.Flush();
			uipage.Response.End();
		}

		/// <summary>Converts the dataXml into CSVs </summary>
		/// <remarks>Note that this was taken from xmlToCSV, and columns removed</remarks>
		/// <returns>Returns a string containing the Xml nodes in CSV format</returns>
		private void xmlToCSV(XmlElement Data, string csvFileName) {
			xLogger.Debug("xmlToCSV");
			string xpath_nodes = "child::*";

			// Check for data and throw exception so message can be served in popup window
			if (Data.SelectSingleNode(xpath_nodes) == null)
				throw new x_exception("error_csv_nodata", String.Concat(error_csv, error_csv_nodata, csvFileName));

			uipage.Response.Clear();
			uipage.Response.ClearHeaders();
			uipage.Response.AddHeader("Content-Disposition", String.Concat("filename=", csvFileName, ".csv"));
			uipage.Response.ContentType = "text/csv";
			uipage.Response.ContentEncoding = Encoding.GetEncoding(1252);
			
			// Use node names as headings
			XmlNode first = Data.SelectSingleNode(xpath_nodes);
			int k = 0;
			foreach (XmlAttribute attr in first.Attributes) {
				try {
					uipage.Response.Write((k == 0) ? "" : TOKEN_COMMA);
					uipage.Response.Write(attr.Name);
					k++;
				} catch (Exception e) {
					xLogger.Debug("xmlToCsv::Error:", e.Message, "::attr:", attr.OuterXml);
				}
			}
			uipage.Response.Write("\n");

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
						uipage.Response.Write((j == 0) ? "" : TOKEN_COMMA);
						uipage.Response.Write(value);
						j++;
					} catch (Exception e) {
						xLogger.Debug("xmlToCsv::Error:", e.Message, "::itemNode:", itemNode.OuterXml);
					}
				}
				uipage.Response.Write("\n");
			}
			xLogger.Debug("xmlToCsv::finished:ok");
			uipage.Response.Flush();
			uipage.Response.End();
		}
		#endregion

        #region Private utility methods
		#endregion
	}
}
