
/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2009-05-05
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20090505	Started.
	20091009:	Minor cleanups
	20100223:	Added _GetTimeout
	20100927:	Added _CheckWSResult overrides and _CheckAdminXResult
	20140112:	Refactored
				Added _WriteServiceList, _WriteServiceItem, _WriteDatatableList
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {

	using System;
	using System.Xml;

	using log4net;

	public class CmsWSBaseNew : x_actor {

		#region Constants
		private const string logid = "CmsWSBase.";
		private const int DEFAULT_WSTIMEOUT = 100000;	// 100000 is sytem default
		
		protected const int SEARCH_PAGE_VALUE = 0;
		protected const string SELECT_RESULT = "/Result";
		protected const string SELECT_RESULT_CODE = "/Result/Result_Code";
		protected const string SELECT_RESULT_DESC = "/Result/Description";

		protected const string SELECT_PASSPORT_RESULT = "//Result";
		protected const string SELECT_PASSPORT_RESULT_CODE = "//Result/ResultCode";
		protected const string SELECT_PASSPORT_RESULT_DESC = "//Result/Description";
		#endregion

		#region Visible properties
		/// <summary>Web Configuration object</summary>
		//public clickclickboom.machinaX.blogX.XConfig Config { get; set; }
		public XLogger xLogger; // ideally move this to x_broker / x_actor
        /// NB: Not available in XLogger - artifact from x_logger
        //public ILog Logger {
        //	get { return xLogger.Logger; }
        //}

        /// <summary>Determines how the list data is output</summary>
        public ListOutputStyle ListStyle { get; set; }

		/// <summary>
		/// Required in Access.ListGroups (leave in for now)
		/// </summary>
		public int AwardID { get; set; }
		public string ConfigID { get; set; }
		public XmlDocument ListXml { get; set; }
		public XmlDocument ItemXml { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public XmlElement ItemXmlRoot {
			get { return ItemXml.DocumentElement; }
		}
		/// <summary>
		/// 
		/// </summary>
		public XmlElement ListXmlRoot {
			get { return ListXml.DocumentElement; }
		}
		/// <summary>
		/// 
		/// </summary>
		public XmlNode ItemXmlRootNode {
			get { return ItemXml.DocumentElement.SelectSingleNode("."); }
		}
		/// <summary>
		/// 
		/// </summary>
		public XmlNode ListXmlRootNode {
			get { return ListXml.DocumentElement.SelectSingleNode("."); }
		}
		private string name;
		/// <summary>
		/// 
		/// </summary>
		public string Name {
			get { return name; }
			set { name = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		public XmlElement Item {
			get { return ItemXml.DocumentElement.SelectSingleNode(Cms.SELECT_ITEMSITEM) as XmlElement; }
		}
		/// <summary>
		/// 
		/// </summary>
		public string ItemID {
			get { return Item.GetAttribute("id"); }
		}
		/// <summary>
		/// 
		/// </summary>
		public int ItemIID {
			get { return Convert.ToInt32(Item.GetAttribute("id")); }
		}
		/// <summary>
		/// 
		/// </summary>
		public XmlElement Items {
			get { return ItemXml.DocumentElement.SelectSingleNode(Cms.SELECT_ITEMS) as XmlElement; }
		}
		/// <summary>
		/// 
		/// </summary>
		public XmlNodeList ItemList {
			get { return ItemXml.DocumentElement.SelectNodes(Cms.SELECT_ITEMSITEM); }
		}
		/// <summary>
		/// 
		/// </summary>
		public XmlElement ListItem {
			get { return ListXml.DocumentElement.SelectSingleNode(Cms.SELECT_ITEMSITEM) as XmlElement; }
		}
		/// <summary>
		/// 
		/// </summary>
		public XmlElement ListItems {
			get { return ListXml.DocumentElement.SelectSingleNode(Cms.SELECT_ITEMS) as XmlElement; }
		}
		/// <summary>
		/// 
		/// </summary>
		public XmlNodeList ListItemsList {
			get { return ListXml.DocumentElement.SelectNodes(Cms.SELECT_ITEMSITEM); }
		}

		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public CmsWSBaseNew(displayX thispage, string config_id, string root_name) : base(thispage) {
			initialise(null, null);
			ConfigID = config_id;
			name = root_name;
			initialize();
		}
		/// <summary>Default constructor</summary>
		public CmsWSBaseNew(displayX thispage, string config_id, string root_name, Type type, string loggerID) : base(thispage) {
			initialise(type, loggerID);
			ConfigID = config_id;
			name = root_name;
			initialize();
		}
		/// <summary>Default constructor</summary>
		public CmsWSBaseNew(displayX thispage, string root_name) : base(thispage) {
			initialise(null, null);
			name = root_name;
			initialize();
		}
		/// <summary>Default constructor</summary>
		public CmsWSBaseNew(displayX thispage, string root_name, Type type, string loggerID) : base(thispage) {
			initialise(type, loggerID);
			name = root_name;
			initialize();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Gets the value of the named attribute for the first item node
		/// </summary>
		/// <param name="name">The name of the item attribute</param>
		/// <returns>The named attribute value</returns>
		public string GetItemValue(string name) {
			return Item.GetAttribute(name);
		}
		public void MoveItemsToRoot() {
			ItemXmlRootNode.FirstChild.AppendChild(Items);
		}
		#endregion

		#region Protected Methods
		/// <summary>check WS result</summary>
		protected XmlElement _CheckAdminXResult(XmlNode result) {
			return checkResult(false, result, true, true);
		}
		/// <summary>check WS result</summary>
		protected XmlElement _CheckWSResult(XmlNode result) {
			return checkResult(false, result, true, false);
		}
		/// <summary>check WS result</summary>
		protected XmlElement _CheckWSResult(XmlNode result, bool throwException) {
			return checkResult(false, result, throwException, false);
		}
		/// <summary>check WS result</summary>
		protected XmlElement _CheckWSResult(XmlNode result, bool throwException, bool throwCode) {
			return checkResult(false, result, throwException, throwCode);
		}
		/// <summary>check Passport WS result</summary>
		protected XmlElement _CheckPassportResult(XmlNode result, bool throwException) {
			return checkResult(true, result, throwException, false);
		}
		/// <summary>check WS result</summary>
		private XmlElement checkResult(bool isPassport, XmlNode result, bool throwException, bool throwCode) {
			xLogger.Debug("CheckResult", "::isPassport:", isPassport.ToString(), "::result:", result.OuterXml);

			XmlElement codeEl = result.SelectSingleNode(isPassport ? SELECT_PASSPORT_RESULT_CODE  : SELECT_RESULT_CODE) as XmlElement;
			string code = codeEl.InnerText;
			string desc = result.SelectSingleNode(isPassport ? SELECT_PASSPORT_RESULT_DESC : SELECT_RESULT_DESC).InnerText;
			XmlElement resultEl = result.SelectSingleNode(isPassport ? SELECT_PASSPORT_RESULT : SELECT_RESULT) as XmlElement;

			bool isOK = (code == "0");
			if (throwException && !isOK) {
				if (throwCode) {
					throw (new XException(code, desc));
				} else {
					throw (new XException("error_service", String.Concat(logid, "::code:", code, "::description:", desc)));
				}
			}
			if (throwException) {	// ie configure for extjs handler
				XmlNode ext_success = codeEl.CloneNode(true);
				ext_success.InnerText = isOK.ToString().ToLower();
				resultEl.RemoveChild(codeEl);
				resultEl.AppendChild(ext_success);
			}
			return result as XmlElement;
		}
		/// <summary>get the WS config URL</summary>
		protected string _GetUrl() {
			return Config.Value(String.Concat(Cms.CONFIG_ROOT, "/", "Url[@id='", ConfigID, "']"));
		}
		/// <summary>get the WS config URL</summary>
		protected string _GetUrl(string ConfigID) {
			return Config.Value(String.Concat(Cms.CONFIG_ROOT, "/", "Url[@id='", ConfigID, "']"));
		}
		protected string _GetUrl(string ConfigRoot, string ConfigID) {
			return Config.Value(String.Concat(ConfigRoot, "/", "Url[@id='", ConfigID, "']"));
		}
		/// <summary>Returns a int equivalent of a string - or default value if conversion fails</summary>
		protected int _GetInt(string value, int defval) {
			int val = defval;
			try {
				val = Int32.Parse(value);
			} catch {
			}
			return val;
		}
		/// <summary>get the WS config timeout</summary>
		protected int _GetTimeout(string ConfigID) {
			return Int32.Parse(Config.Value(String.Concat(Cms.CONFIG_ROOT, "/", "Url[@id='", ConfigID, "']/@timeout"), DEFAULT_WSTIMEOUT.ToString()));
		}
		#endregion

		#region Write Methods
		protected void _WriteList(XmlNode result) {
			xLogger.Debug("_WriteList", "::ListStyle:", ListStyle.ToString());
			
			if (ListStyle == ListOutputStyle.Datatable) {
				writeDatatableList(result);
			} else if (ListStyle == ListOutputStyle.XmlDirect) {
				writeDirectList(result);
			} else {
				writeServiceList(result);
			}
		}

		protected void _WriteItem(XmlNode result) {
			xLogger.Debug("_WriteItem", "::ListStyle:", ListStyle.ToString());

			if (ListStyle == ListOutputStyle.Datatable) {
				//writeDatatableItem(result);
				writeDirectItem(result);
			} else if (ListStyle == ListOutputStyle.XmlDirect) {
				writeDirectItem(result);
			} else {
				writeServiceItem(result);
			}
		}

		/// <summary>NOT COMPLETE: Check result items and write JSON to response stream (Datatables.net format)</summary>
		private void writeDatatableList(XmlNode result) {
			xLogger.Debug("writeDatatableList", "::sEcho:", UserProfile.Value("sEcho"));

			//output.CmsXDatatable output = new output.CmsXDatatable(UserProfile);

			UIPage.Response.ContentType = "application/json; charset=utf-8";
			//UIPage.Response.Write(output.Get(_CheckWSResult(result)));
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

		private void writeServiceList(XmlNode ServiceList) {
			xLogger.Debug("writeServiceList");

			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(ListXml.ImportNode(_CheckWSResult(ServiceList), true));

			xLogger.Debug("writeServiceList:ok");
		}
		private void writeServiceItem(XmlNode ServiceItem) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(ItemXml.ImportNode(_CheckWSResult(ServiceItem), true));
		}
		#endregion

		#region Private Methods
		/// <summary>Initialise the xLogger</summary>
		private void initialise(Type logType, string logID) {
			xLogger = new XLogger((logType == null) ? typeof(CmsXProfileX) : logType, (String.IsNullOrEmpty(logID)) ? logid : logID, false, true);
		}
		/// <summary>Initiate properties</summary>
		private void initialize() {
			//Config = new clickclickboom.machinaX.blogX.XConfig();
			string listOutputStyle = Config.Value(String.Concat(Cms.CONFIG_ROOT, "/ListOutputStyle"), Cms.DEFAULT_LISTOUTPUTSTYLE);
			xLogger.Debug("initialize", "::listOutputStyle:", listOutputStyle);

			if (listOutputStyle == ListOutputStyle.XmlDirect.ToString()) {
				ListStyle = ListOutputStyle.XmlDirect;
			} else if (listOutputStyle == ListOutputStyle.Datatable.ToString()) {
				ListStyle = ListOutputStyle.Datatable;
			} else { 
				ListStyle = ListOutputStyle.XmlTransform;
			}
			xLogger.Debug("initialize", "::ListStyle:", ListStyle.ToString());

			ItemXml = new XmlDocument();
			ItemXml.LoadXml(String.Format("<{0}/>", name));
			ListXml = new XmlDocument();
			ListXml.LoadXml(String.Format("<{0}List/>", name));
		}
		#endregion
	}
}

