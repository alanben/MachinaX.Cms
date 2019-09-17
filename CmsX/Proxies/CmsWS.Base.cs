/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2009-05-05
	Status:		release	
	Version:	4.0.3
	Build:		20160106
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20090505	Started.
	20091009:	Minor cleanups
	20100223:	Added _GetTimeout
	20100927:	Added _CheckWSResult overrides and _CheckAdminXResult
	20140302:	Refactored
	20160106:	Added Construtor so that class can be used outside of brokers
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {

	using log4net;
	using System;
	using System.Xml;

	public class CmsWSBase : x_actor {

		#region Constants
		private const string logid = "CmsWSBase.";
		private const int DEFAULT_WSTIMEOUT = 100000;	// 100000 is sytem default
		
		protected const int SEARCH_PAGE_VALUE = 0;
		protected const string SELECT_RESULT = "//Result";
		protected const string SELECT_RESULT_CODE = "//Result/Result_Code";
		protected const string SELECT_RESULT_DESC = "//Result/Description";

		protected const string SELECT_PASSPORT_RESULT = "//Result";
		protected const string SELECT_PASSPORT_RESULT_CODE = "//Result/ResultCode";
		protected const string SELECT_PASSPORT_RESULT_DESC = "//Result/Description";
		#endregion

		#region Visible properties
		/// <summary>Web Configuration object</summary>
		//public clickclickboom.machinaX.blogX.x_config Config { get; set; }
		public x_logger xLogger;	// ideally move this to x_broker / x_actor
		public ILog Logger {
			get { return xLogger.Logger; }
		}

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
		public long ItemIIID {
			get { return Convert.ToInt64(Item.GetAttribute("id")); }
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

		private bool isAdmin = true;
		public bool IsAdmin {
			get { return isAdmin; }
			set { isAdmin = value; }
		}
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public CmsWSBase(displayX thispage, string config_id, string root_name) : base(thispage) {
			initialise(null, null);
			ConfigID = config_id;
			name = root_name;
			initialize();
		}
		/// <summary>Default constructor</summary>
		public CmsWSBase(displayX thispage, string config_id, string root_name, Type type, string loggerID) : base(thispage) {
			initialise(type, loggerID);
			ConfigID = config_id;
			name = root_name;
			initialize();
		}
		public CmsWSBase(displayX thispage, string config_id, string root_name, Type type, string loggerID, bool wantuserprofile) : base(thispage, wantuserprofile) {
			initialise(type, loggerID);
			ConfigID = config_id;
			name = root_name;
			initialize();
		}
		/// <summary>Default constructor</summary>
		public CmsWSBase(displayX thispage, string root_name) : base(thispage) {
			initialise(null, null);
			name = root_name;
			initialize();
		}
		/// <summary>Default constructor</summary>
		public CmsWSBase(displayX thispage, string root_name, Type type, string loggerID) : base(thispage) {
			initialise(type, loggerID);
			name = root_name;
			initialize();
		}
		public CmsWSBase(displayX thispage, string root_name, Type type, string loggerID, bool wantuserprofile) : base(thispage, wantuserprofile) {
			initialise(type, loggerID);
			name = root_name;
			initialize();
		}
		/// <summary>Default constructor</summary>
		/// <remarks>NB: When deriving from this base overload the UIPage, UserProfile etc will be null</remarks>
		public CmsWSBase(string root_name, Type type, string loggerID) : base() {
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
			return _CheckResult(false, result, true, true);
		}
		/// <summary>check WS result</summary>
		protected XmlElement _CheckWSResult(XmlNode result) {
			return _CheckResult(false, result, true, false);
		}
		/// <summary>check WS result</summary>
		protected XmlElement _CheckWSResult(XmlNode result, bool throwException) {
			return _CheckResult(false, result, throwException, false);
		}
		/// <summary>check WS result</summary>
		protected XmlElement _CheckWSResult(XmlNode result, bool throwException, bool throwCode) {
			return _CheckResult(false, result, throwException, throwCode);
		}
		/// <summary>check Passport WS result</summary>
		protected XmlElement _CheckPassportResult(XmlNode result, bool throwException) {
			return _CheckResult(true, result, throwException, false);
		}
		/// <summary>check WS result</summary>
		protected virtual XmlElement _CheckResult(bool isPassport, XmlNode result, bool throwException, bool throwCode) {
			xLogger.Debug("CheckResult", "::isPassport:", isPassport, "::throwException:", throwException, "::throwCode:", throwCode);
			//xLogger.Debug("CheckResult", "::result:", (result == null) ? "null" : result.OuterXml);

			XmlElement codeEl = result.SelectSingleNode(isPassport ? SELECT_PASSPORT_RESULT_CODE  : SELECT_RESULT_CODE) as XmlElement;
			//xLogger.Debug("CheckResult", "::codeEl:", (codeEl == null) ? "null" : codeEl.OuterXml);
			
			string code = codeEl.InnerText;
			string desc = result.SelectSingleNode(isPassport ? SELECT_PASSPORT_RESULT_DESC : SELECT_RESULT_DESC).InnerText;
			XmlElement resultEl = result.SelectSingleNode(isPassport ? SELECT_PASSPORT_RESULT : SELECT_RESULT) as XmlElement;

			bool isOK = (code == "0");
			if (throwException && !isOK) {
				if (throwCode) {
					throw (new x_exception(code, desc));
				} else {
					throw (new x_exception("error_service", String.Concat(logid, "::code:", code, "::description:", desc)));
				}
			}
			//if (throwException) {	// ie configure for extjs handler
			if (IsAdmin) {
				XmlNode ext_success = codeEl.CloneNode(true);
				ext_success.InnerText = isOK.ToString().ToLower();
				resultEl.RemoveChild(codeEl);
				resultEl.AppendChild(ext_success);
			}
			xLogger.Debug("CheckResult:ok");
			//xLogger.Debug("CheckResult", "::result:", (result == null) ? "null" : result.OuterXml);
			
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
			xLogger.Debug("_GetUrl", "::Config:", (Config == null) ? "null" : Config.ID);
			xLogger.Debug("_GetUrl", "::ConfigRoot:", ConfigRoot, "::ConfigID:", ConfigID);
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
		// NB These are intermediate methods to brdge the gap for some code written based on (incomplete) methods of CmsWSBaseNew
		// Actually equivalent to GetServiceList / GetServiceItem
		protected void _WriteList(XmlDocument resultdoc) {
			_WriteList(resultdoc.DocumentElement);
		}
		protected void _WriteList(XmlNode result) {
			//xLogger.Debug("_WriteList", "::ListStyle:", ListStyle.ToString());

			//if (ListStyle == ListOutputStyle.Datatable) {
			//	writeDatatableList(result);
			//} else if (ListStyle == ListOutputStyle.XmlDirect) {
			//	writeDirectList(result);
			//} else {
				writeServiceList(result);
			//}
		}
		protected void _WriteItem(XmlDocument resultdoc) {
			_WriteItem(resultdoc.DocumentElement);
		}
		protected void _WriteItem(XmlNode result) {
			//xLogger.Debug("_WriteItem", "::ListStyle:", ListStyle.ToString());

			//if (ListStyle == ListOutputStyle.Datatable) {
			//	//writeDatatableItem(result);
			//	writeDirectItem(result);
			//} else if (ListStyle == ListOutputStyle.XmlDirect) {
			//	writeDirectItem(result);
			//} else {
				writeServiceItem(result);
			//}
		}
		protected void _WriteItem(XmlNode result, string errorLink) {
			try {
				_WriteItem(result);
			} catch (x_exception e) {
				xLogger.Debug("_WriteItem", "::e:", e.Message);

				throw new displayException(errorLink, e.Message);
			}
		}
		#endregion

		#region Service List / Item methods
		/// <summary>Appends the list of items to the ListXmlRoot</summary>
		/// <param name="ServiceList"></param>
		public void GetServiceList(XmlDocument ServiceListDoc) {
			GetServiceList(ServiceListDoc.DocumentElement);
		}
		public void GetServiceList(XmlNode ServiceList) {
			writeServiceList(ServiceList);
		}
		protected void writeServiceList(XmlNode ServiceList) {
			ListXmlRoot.RemoveAll();
			ListXmlRoot.AppendChild(ListXml.ImportNode(_CheckWSResult(ServiceList), true));
		}

		/// <summary>Appends the item to the ItemXmlRoot</summary>
		/// <param name="ServiceList"></param>
		public void GetServiceItem(XmlNode ServiceList) {
			writeServiceItem(ServiceList);
		}
		protected void writeServiceItem(XmlNode ServiceItem) {
			ItemXmlRoot.RemoveAll();
			ItemXmlRoot.AppendChild(ItemXml.ImportNode(_CheckWSResult(ServiceItem), true));
		}
		#endregion

		#region Private Methods
		/// <summary>Initialise the xLogger</summary>
		private void initialise(Type logType, string logID) {
			xLogger = new x_logger((logType == null) ? typeof(CmsXProfileX) : logType, (String.IsNullOrEmpty(logID)) ? logid : logID, false, true);
		}
		/// <summary>Initiate properties</summary>
		private void initialize() {
			//Config = new clickclickboom.machinaX.blogX.x_config();

			ItemXml = new XmlDocument();
			ItemXml.LoadXml(String.Format("<{0}/>", name));
			ListXml = new XmlDocument();
			ListXml.LoadXml(String.Format("<{0}List/>", name));
		}
		#endregion
	}
}

