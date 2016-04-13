/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.3
	Build:		20151223
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100722:	Refactored from legacy
	20100823:	Cleanup of legacy
	20111213:	Refactored EconoVaultCSV into this.
	20151223:	Renamed CmsXExport and Refactored for Excel exports
	-----------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {

	using System;
	using System.Xml;

	/// <summary>Enumeration for style used for list output in proxies</summary>
	public enum ExportType {
		  CSV = 1		// Standard .csv export
		, XLSX = 2		// Excel .xlsx export
		, Unknown = 3	// Unknown format
	}

	/// <summary>
	/// CmsXExport contains the code for export to csv or excel and output
	/// </summary>
	public class CmsXExport {

		private string id;
		private const string DEFAULT_FILTER_PREFIX = "csv";
		private const ExportType DEFAULT_EXPORTTYPE = ExportType.XLSX;
		private const string logid = "CmsXExport.";

		protected x_logger xLogger;
		/// <summary>The UIPage of the processing broker</summary>
		private displayX uipage;
		public displayX UIPage {
			get { return uipage; }
			set { uipage = value; }
		}

		/// <summary>
		/// The CSV filter node in the User profile
		/// </summary>
		public XmlElement UserFilters {
			get { return UIPage.UserProfile.Node(String.Format("{0}filters", id)); }
		}
		public XmlElement UserFilter {
			get { return UserFilters.SelectSingleNode(String.Format("{0}filter[@name='{1}']", id, PageFilterName)) as XmlElement; }
		}
		public string UserFilePath {
			get { return UIPage.Server.MapPath(String.Format("Defaults/{0}.xml", UIPage.UserProfile.Value("UserID", "default"))); }
		}
		public string PageFilterName {
			get { return (PageFilter == null) ? "" : PageFilter.GetAttribute("name"); }
		}
		public XmlElement PageFilter {
			get { return UIPage.Content.SelectSingleNode(String.Format("//{0}filters/{0}filter", id)) as XmlElement; }
		}
		public XmlElement PageFilters {
			get { return UIPage.Content.SelectSingleNode(String.Format("//{0}filters", id)) as XmlElement; }
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

		private ExportType exportType = ExportType.Unknown;
		public ExportType ExportType {
			get { return exportType; }
			set { exportType = value; }
		}

		#region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXExport(displayX thispage) {
			inititalise(thispage, DEFAULT_FILTER_PREFIX, logid, typeof(CmsXExport), DEFAULT_EXPORTTYPE);
		}
		public CmsXExport(displayX thispage, string filterPrefix, string logId, Type logtype) {
			inititalise(thispage, filterPrefix, logId, logtype, DEFAULT_EXPORTTYPE);
		}

		private void inititalise(displayX thispage, string filterPrefix, string logId, Type logtype, ExportType exportType) {
			xLogger = new x_logger(logtype, logId, false, true);
			id = filterPrefix;
			UIPage = thispage;
			ExportType = exportType;
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
			string param = UIPage.QueryParam("itemselector", "-");
			XmlElement content = UIPage.Content.SelectSingleNode("//content") as XmlElement;
			XmlDocument csvParamDoc = getParamXml(param, content);

			if (UserFilters != null) {
				XmlElement userfilter = UserFilter;
				if (userfilter == null) {
					userfilter = UIPage.UserProfile.AddNode(UserFilters, FilterName, "");
				}
				if (userfilter != null) {
					XmlNode subobjs = userfilter.SelectSingleNode("subobjs");
					if (subobjs != null)
						userfilter.RemoveChild(subobjs);
					userfilter.AppendChild(UIPage.UserProfile.ProfileXml.ImportNode(csvParamDoc.SelectSingleNode("//subobjs"), true));
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
				UIPage.UserProfile.AddNode(filterxml.SelectSingleNode(SelectFilters) as XmlElement);
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
			string csvFileName = UIPage.Parameters.LinkName;
			xLogger.Debug("GetColumns::WantQuery:", WantQuery.ToString(), "::csvFileName:", csvFileName);
			if (WantQuery) {
				string param = UIPage.QueryParam("itemselector", "-");
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
			string csvFileName = UIPage.Parameters.LinkName;
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
			csvDoc.Load(UIPage.Server.MapPath(xmlFileName));
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
			switch (ExportType) {
				case ExportType.CSV:
					CmsXExportCSV exportCSV = new CmsXExportCSV(UIPage, xLogger);
					exportCSV.XmlToCSV(Data, Columns, FileName);
					break;
				case ExportType.XLSX:
					CmsXExportXLSX exportXLSX = new CmsXExportXLSX(UIPage, xLogger);
					exportXLSX.XmlToExcel(Data, Columns, FileName);
					break;
				case ExportType.Unknown:
				default:
					throw new Exception("Export type unknown or not defined");
			}			
		}
		/// <summary>
		/// To be overridden in inheriting class
		/// </summary>
		/// <param name="Data"></param>
		/// <param name="FileName"></param>
		protected virtual void _XmlToOutput(XmlElement Data, string FileName) {
			switch (ExportType) {
				case ExportType.CSV:
					CmsXExportCSV exportCSV = new CmsXExportCSV(UIPage, xLogger);
					exportCSV.XmlToCSV(Data, FileName);
					break;
				case ExportType.XLSX:
					throw new Exception("Column definition required for Excel exports");
				case ExportType.Unknown:
				default:
					throw new Exception("Export type unknown or not defined");
			}
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
			XmlElement pageElem = UIPage.Parameters.LinkPage;
			string xmlFileName = String.Format(fileTempl, pageElem.GetAttribute("sect"), pageElem.GetAttribute("sub"), pageElem.GetAttribute("proc"), pageElem.GetAttribute("file"));
			XmlDocument csvDoc = new XmlDocument();
			csvDoc.Load(UIPage.Server.MapPath(xmlFileName));
			return csvDoc;
		}

		#endregion
	}
}
