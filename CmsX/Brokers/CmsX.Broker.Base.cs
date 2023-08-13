
/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Refactored from LoeriesAdmin (but not possibly used as this looks 
 				like an awful lot that is specific)
	20100925:	Updates from EconoVault
	20101020:	Changed starting link to 'content_links'
				Moved processTinymceHtml (was processHtml) here from CmsXBrokerDisplay
	20101206:	Added _Menus and initialisation in constructor
	20101218:	Added _Spaces and _Topics
	20101218:	Added _Blogs
	20101227:	Added _Media
	20110522:	Added _Files and _ReadToEnd()
	20110902:	Updated CustomerWS (related to name in namespace update)
	20111213:	Refactored from EconoVaultXBroker into this.
				Added Constructor override that does not initialise _Admin
	20121114:	Updated _doValidation to call ValidateToken
	20130513:	Added _IsDefault as test for DEFAULT_SUBITEM
	20140112:	Refactored constructor overrides
				Moved WS Proxies into CmsXBroker
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {

	using System;
	using System.Text;
	using System.Web;
	using System.Xml;
	using System.Xml.XPath;

	using log4net;

	/// <summary>DropDownType</summary>
	public enum DropDownType {
		Drop = 1
		, Filter = 2
		, Select = 3
	}

	/// <summary>
	/// The base class for all brokers that deal with general content management processes
	public class CmsXBrokerBase : CmsXBrokerTemplate {
        #region Invisible properties
		#endregion

        #region Visible properties
		protected XLogger xLogger;
		#endregion

        #region Private Constants
		private const string logid = "CmsXBrokerBase.";
		private const string FORMAT_RESULT_XML = "<CmsX><Result><Description>OK</Description><Result_Code>true</Result_Code></Result><item att='0'/></CmsX>";

		private const string SELECT_SEARCH_TYPES = "//fld[starts-with(@name, '{0}') and substring-after(@name, '_SEARCH') = '_Type' and (number(value) > 0 and number(value) <= 100)]";
		private const string SELECT_SEARCH_VALUES = "//fld[starts-with(@name, '{0}') and substring-after(@name, '_SEARCH') = '' and (value!='0'and value!='')]";
		private const string SELECT_SEARCH_XTYPES = "//fld[starts-with(@name, '{0}') and substring-after(@name, '_SEARCH') = '_Type' and number(value) > 100]";
		private const string TOKEN_COMMA = ",";
		#endregion

		#region Constant error strings
		private const string error_validate		= "Error validating: ";
		private const string error_grid_edit	= "Error doing grid edit: ";
		#endregion

		#region Protected Constants
		protected const string SELECT_PASSPORT_RESULT_CODE = "/Result/ResultCode";
		protected const string SELECT_PASSPORT_RESULT_DESC = "/Result/Description";
		protected const string LINK_LOGOUT = "passport_logout";
		// The default value for an item field in a editable grid (ie set in the extjs script)
		protected const int MAX_HANDLE = 1000;
		#endregion

		#region Public Constants
		public const string DEFAULT_SUBITEM = "[...]";
		#endregion

		#region Visible properties
		/// <summary>
		/// Reference to the Logger object itself
		/// NB: Not available in XLogger - artifact from x_logger
		/// </summary>
		//public ILog Logger {
		//	get { return xLogger.Logger; }
		//}

		private bool wantCmsAuth = true;
		/// <summary>Flag to indicate if authentication (eg token validation) is done via CmsAuth (or PassportX)</summary>
		public bool WantCmsAuth {
			get { return wantCmsAuth; }
			set { wantCmsAuth = value; }
		}
		#endregion


		#region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerBase(CmsXProfileX thispage) : base(thispage) {
			initialise(thispage, typeof(CmsXBrokerBase), logid);
		}
		public CmsXBrokerBase(CmsXProfileX thispage, Type type, string loggerID) : base(thispage) {
			initialise(thispage, type, loggerID);
		}
		public CmsXBrokerBase(CmsXProfileX thispage, string blogxID, Type type, string loggerID) : base(thispage) {
			_BlogxID = blogxID;
			initialise(thispage, type, loggerID);
		}
		public CmsXBrokerBase(bool CmsAuth, CmsXProfileX thispage, Type type, string loggerID) : base(thispage) {
			WantCmsAuth = CmsAuth;
			initialise(thispage, type, loggerID);
		}

		/*
		/// <summary>Constructor that selectively creates web service wrappers</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerBase(CmsXProfileX thispage, Type type, string loggerID, bool wantDisplay, bool wantUsers, bool wantAccess, bool wantLinklog) : base(thispage) {
			initialise(thispage, type, loggerID);
			// Selectively create web service wrappers
			if (wantDisplay)
				DisplayX = new Displayx(thispage);
			if (wantUsers)
				_Users = new Users(thispage);
			if (wantAccess)
				_Access = new Access(thispage);
			if (wantLinklog)
				_LinkLog = new LinkLog(thispage);
		}
		/// <summary>Constructor that selectively creates web service wrappers</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerBase(bool CmsAuth, CmsXProfileX thispage, Type type, string loggerID, bool wantDisplay, bool wantUsers, bool wantAccess, bool wantLinklog) : base(thispage) {
			WantCmsAuth = CmsAuth;
			initialise(thispage, type, loggerID);
			// Selectively create web service wrappers
			if (wantDisplay)
				DisplayX = new Displayx(thispage);
			if (wantUsers)
				_Users = new Users(thispage);
			if (wantAccess)
				_Access = new Access(thispage);
			if (wantLinklog)
				_LinkLog = new LinkLog(thispage);
		}
		/// <summary>Constructor that selectively creates web service wrappers</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerBase(CmsXProfileX thispage, string blogxID, Type type, string loggerID, bool displayx, bool linksx, bool menusx, bool spacesx, bool topicsx, bool blogsx, bool mediax, bool filesx) : base(thispage) {
			initialise(thispage, type, loggerID);
			// Selectively create web service wrappers
			if (displayx)
				DisplayX = new Displayx(thispage, blogxID);
			if (linksx)
				_Links = new Links(thispage, blogxID);
			if (menusx)
				_Menus = new Menus(thispage, blogxID);
			if (spacesx)
				_Spaces = new Spaces(thispage, blogxID);
			if (topicsx)
				_Topics = new Topics(thispage, blogxID);
			if (blogsx)
				_Blogs = new Blogs(thispage, blogxID);
			if (mediax)
				_Media = new Media(thispage, blogxID);
			if (filesx)
				_Files = new Files(thispage, blogxID);
		}
		*/
		#endregion

		#region Public methods
		/// <summary>
		/// Overrides parent method to do login checking only. Child classes do actual processing.
		/// </summary>
		public override void Process(string type) {
			this.Process(true);
			switch (type) {
				case "update_profile":
					validate();
					break;
			}
		}
		/// <summary>
		/// Overrides parent method to do login checking only. Child classes do actual processing.
		/// </summary>
		public override void Process(bool CheckUser) {
			if (CheckUser) {
				CheckUserLoggedIn();
			}
		}

		/// <summary>Check that the user is logged in</summary>
		public void CheckUserLoggedIn() {
			CheckUserLoggedIn(LINK_LOGOUT);
		}
		public void CheckUserLoggedIn(string redirect) {
			// first get current login pattern node
			string login = UserProfile.GetPattern("login");
			xLogger.Debug("LOGIN PATTERN::", login);
			if (login == null) {
				xLogger.Info("CheckUserLoggedIn::login:null");
				throw (new displayException(redirect));
			}
			xLogger.Debug("CheckUserLoggedIn::token:");
			// validate the Token
			if (!_doValidation()) {
				xLogger.Info("CheckUserLoggedIn::redirect:", redirect);
				throw (new displayException(redirect));
			}
			xLogger.Debug("CheckUserLoggedIn::ok.");
		}
        #endregion

        #region Protected methods
		/// <summary>verify the Passport result</summary>
		/// <returns></returns>
		protected bool VerifyPassport(XmlElement result) {
			xLogger.Debug(logid, "verify:result", result.OuterXml);
			XmlElement rescode = result.SelectSingleNode(SELECT_PASSPORT_RESULT_CODE) as XmlElement;
			if (rescode != null) {
				return (rescode.InnerText == "0");
			} else {
				return false;
			}
		}
		/// <summary>Test to see if the search is activated by looking at search fields and their values</summary>
		/// <param name="Prefix">The search field name prefix (common to all search fields to be checked)</param>
		/// <returns>True if NOT set, False if set</returns>
		protected bool _IsSearchSet(string Prefix) {
			return _IsSearchSet(Prefix, false);
		}
		
		// AB: 20100223:	Not sure that thsi is working correctly. See Agency and Entries search. If I hit "Reset" the search tests as true
		//					- I think that it should test as false and the list should be restored
		//					- I suspect that this is because the the field count for types is old (assumes type value = 0) (which was '(none)' for an simple text search
		//					- Also value count gets a few it shouldn't
		//					Anyway, I'm leaving for now as there have not been any comments/complaints re the reset.

		/// <summary>Test to see if the search is activated by looking at search fields and their values</summary>
		/// <param name="Prefix">The search field name prefix (common to all search fields to be checked)</param>
		/// <returns>True if NOT set, False if set</returns>
		protected bool _IsSearchSet(string Prefix, bool valueOnly) {
			XmlNodeList srch_typ_flds = UserProfile.ProfileXml.SelectNodes(String.Format(SELECT_SEARCH_TYPES, Prefix));
			XmlNodeList srch_val_flds = UserProfile.ProfileXml.SelectNodes(String.Format(SELECT_SEARCH_VALUES, Prefix));
			XmlNodeList srch_xtyp_flds = UserProfile.ProfileXml.SelectNodes(String.Format(SELECT_SEARCH_XTYPES, Prefix));
			bool test = (srch_typ_flds.Count < 1 && srch_val_flds.Count < 1);
			bool testval = (srch_val_flds.Count < 1);
			bool xtest = (srch_xtyp_flds.Count > 0);
			if (xLogger.IsDebug) {
				foreach (XmlNode valfld in srch_val_flds) {
					xLogger.Debug("IsSearchSet::valfld:", valfld.OuterXml);
				}
			}
			xLogger.Debug("IsSearchSet::srch_val_flds:", srch_val_flds.Count.ToString(), ":srch_typ_flds:", srch_typ_flds.Count.ToString(), ":srch_xtyp_flds:", srch_xtyp_flds.Count.ToString());
			xLogger.Debug("IsSearchSet::test:", test.ToString(), "::testval:", testval.ToString(), "::srch_val_flds:", xtest.ToString());
			return (valueOnly)? testval : test;
		}
		
		/// <summary></summary>
        protected XmlNode _AddDefaultItem(XmlDocument xmlDoc, string parentXPath) {
			return _AddDefaultItem(xmlDoc, parentXPath, "0");
        }
        /// <summary></summary>
        protected XmlNode _AddDefaultItem(XmlDocument xmlDoc, string parentXPath, string defval) {
            XmlElement default_element_node = xmlDoc.CreateElement("item");
            default_element_node.SetAttribute("id", defval);
            default_element_node.SetAttribute("name", "default");
            default_element_node.SetAttribute("desc", "-");
            return xmlDoc.DocumentElement.SelectSingleNode(parentXPath).PrependChild(xmlDoc.ImportNode(default_element_node as XmlNode, true));
        }
        /// <summary>Upload a file replies</summary>
        protected Array FileUpload(string validFileExtensions) {
            xLogger.Debug("fileUpload:", UIPage.Request.Files.Count.ToString());
            string[] files = new string[1];
            string filename;
            string clientFileName;
            string clientFileExt;
            bool check_valid_ext = (validFileExtensions == "") ? false : true;
            try {
                HttpFileCollection fileColl = UIPage.Request.Files;
                xLogger.Debug("fileUpload::fileColl.Count:", fileColl.Count.ToString());
                for (int Loop1 = 0; Loop1 < fileColl.Count; Loop1++) {
                    if (check_valid_ext) {
                        clientFileName = fileColl.Get(Loop1).FileName;
                        clientFileExt = clientFileName.Substring(clientFileName.LastIndexOf("."));
                        //logger.Debug(String.Concat(logid, "_fileUpload::clientFileExt", clientFileExt));
                        if (validFileExtensions.IndexOf(clientFileExt) < 0)
                            throw (new XException("error_fileup", String.Concat("error_fileup: fileUpload(): Invalid file extension ")));
                    }
                    // Create a new file name.
                    filename = UIPage.Server.MapPath(String.Concat("temp/", Guid.NewGuid().ToString(), "-", DateTime.Now.Ticks.ToString(), ".txt"));
                    //xLogger.Debug("fileUpload::filename:", filename);
                    files[Loop1] = filename;
                    // Save the file.
                    fileColl.Get(Loop1).SaveAs(filename); 
                    xLogger.Debug("fileUpload::filename:", filename);
                    
                }
                xLogger.Debug("fileUpload:", files[0]);
            } catch (Exception e) {
                throw (new XException("error_fileup", String.Concat("error_fileup", e.Message, " - ", e.StackTrace)));
                //return null;
            }
            return files;
		}
		/// <summary>response text stream from a file</summary>
		protected void TextOut(string text, string filename, string extension) {
			textOut(text, filename, extension);
		}
        /// <summary>response text stream from a file</summary>
        protected void TextOut(string text, string filename) {
            textOut(text, filename);
        }
        /// <summary>response text stream from a file</summary>
        protected void TextOut(string text) {
            textOut(text);
        }

        /// <summary>Set profile sub values </summary>
        protected void SetProfSubs(string prof, string prompt, XmlNodeList items) {
            SetProfSubs(prof, prompt, items, null, null);
        }

        /// <summary>Set profile sub values </summary>
        protected void SetProfSub(string prof, string prompt, string prompt2, XmlNode item, string replace, string replace_with) {
            UserProfile.Add(prof, String.Concat(item.SelectSingleNode(prompt).InnerText, " - ", item.SelectSingleNode(prompt2).InnerText).Replace("\\", "").Replace(replace, replace_with), item.Attributes.GetNamedItem("id").InnerText);
        }
        /// <summary>Set profile sub values </summary>
        protected void SetProfSubs(string prof, string prompt, string prompt2,  XmlNodeList items, string replace, string replace_with) {
            foreach (XmlNode item in items)
                UserProfile.Add(prof, String.Concat(item.SelectSingleNode(prompt).InnerText, " - ", item.SelectSingleNode(prompt2).InnerText).Replace("\\", "").Replace(replace, replace_with), item.Attributes.GetNamedItem("id").InnerText);
        }
        /// <summary>Set profile sub values </summary>
        protected void SetProfSubs(string prof, string prompt, string prompt2, string prompt3, XmlNodeList items, string replace, string replace_with) {
            foreach (XmlNode item in items)
                UserProfile.Add(prof, String.Concat(item.SelectSingleNode(prompt).InnerText, " - ", item.SelectSingleNode(prompt2).InnerText, " (", item.SelectSingleNode(prompt3).InnerText,")").Replace("\\", "").Replace(replace, replace_with), item.Attributes.GetNamedItem("id").InnerText);
        }
        /// <summary>Set profile sub values </summary>
        protected void SetProfSubs(string prof, string prompt, XmlNodeList items, string replace, string replace_with) {
			if (replace == null) {
				foreach (XmlNode item in items) {
					UserProfile.Add(prof, item.SelectSingleNode(prompt).InnerText, item.Attributes.GetNamedItem("id").InnerText);
				}
			} else {
				foreach (XmlNode item in items) {
					UserProfile.Add(prof, item.SelectSingleNode(prompt).InnerText.Replace("\\", "").Replace(replace, replace_with), item.Attributes.GetNamedItem("id").InnerText);
				}
			}
        }

		/// <summary>
		/// Gets the value of the "id" in the query string and assigns it to a profile field
		/// </summary>
		/// <param name="ProfileID">The name of the profile field to contain the id</param>
		/// <returns>the value of the "id"</returns>
		protected string _GetQueryID(string ProfileID) {
			return _GetQuery("id", ProfileID, "0");
		}
		protected int _GetQueryIID(string ProfileID) {
			return _GetQuery("id", ProfileID, 0);
		}
		protected long _GetQueryIID(string ProfileID, long Default) {
			return _GetQuery("id", ProfileID, Default);
		}

		/// <summary>
		/// Gets the value of the query string and assigns it to a profile field
		/// </summary>
		/// <param name="QueryID">The name of the query parameter</param>
		/// <param name="ProfileID">The name of the profile field to contain the id</param>
		protected string _GetQuery(string QueryID, string ProfileID) {
			return _GetQuery(QueryID, ProfileID, "0");
		}

		/// <summary>
		/// Gets the value of the query string and assigns it to a profile field
		/// </summary>
		/// <param name="QueryID">The name of the query parameter</param>
		/// <param name="ProfileID">The name of the profile field to contain the id</param>
		/// <param name="Default">The default value of the query parameter (if '')</param>
		/// <returns>the value of the query parameter or default</returns>
		protected string _GetQuery(string QueryID, string ProfileID, string Default) {
			string queryID = UIPage.QueryParam(QueryID, Default);
			xLogger.Debug("GetQueryID", "::queryID:", queryID);

			queryID = (queryID == Default) ? UserProfile.Value(ProfileID, queryID) : queryID;
			xLogger.Debug("GetQueryID", "::queryID:", queryID);

			UserProfile.Add(ProfileID, queryID);
			return queryID;
		}

		/// <summary>
		/// Gets the value of the query string and assigns it to a profile field
		/// </summary>
		/// <param name="QueryID">The name of the query parameter</param>
		/// <param name="ProfileID">The name of the profile field to contain the id</param>
		/// <param name="Default">The default value of the query parameter (if '')</param>
		/// <returns>the value of the query parameter or default</returns>
		protected int _GetQuery(string QueryID, string ProfileID, int Default) {
			int queryID = UIPage.QueryParam(QueryID, Default);
			xLogger.Debug("GetQueryID", "::queryID:", queryID);

			queryID = (queryID == Default) ? UserProfile.Value(ProfileID, queryID) : queryID;
			xLogger.Debug("GetQueryID", "::queryID:", queryID);

			UserProfile.Add(ProfileID, queryID);
			return queryID;
		}
		protected long _GetQuery(string QueryID, string ProfileID, long Default) {
			long queryID = UIPage.QueryParam(QueryID, Default);
			xLogger.Debug("GetQueryID", "::queryID:", queryID);

			queryID = (queryID == Default) ? UserProfile.Value(ProfileID, queryID) : queryID;
			xLogger.Debug("GetQueryID", "::queryID:", queryID);

			UserProfile.Add(ProfileID, queryID);
			return queryID;
		}

		/// <summary>
		/// Re-Order an xml item list
		/// </summary>
		/// <param name="itemsXmlRoot">The items node</param>
		/// <param name="xPath">xPath statement to the item nodes</param>
		/// <param name="sortAtt">item attibute to sort on</param>
		/// <param name="isAscending">flag to indicate if sort is ascending</param>
		/// <param name="isNumber"></param>
		protected void _ReOrderXml(XmlNode itemsXmlRoot, string xPath, string sortAtt) {
			_ReOrderXml(itemsXmlRoot, xPath, sortAtt, true, false);
		}
		protected void _ReOrderXml(XmlNode itemsXmlRoot, string xPath, string sortAtt, bool isAscending) {
			_ReOrderXml(itemsXmlRoot, xPath, sortAtt, isAscending, false);
		}
		protected void _ReOrderXml(XmlNode itemsXmlRoot, string xPath, string sortAtt, bool isAscending, bool isNumber) {
			//get nodes re-ordered
			XPathNavigator navigator = itemsXmlRoot.CreateNavigator();
			XPathExpression selectItems = navigator.Compile(xPath);
			XPathExpression sortItems = navigator.Compile(sortAtt);
			selectItems.AddSort(sortItems, (isAscending) ? XmlSortOrder.Ascending : XmlSortOrder.Descending, XmlCaseOrder.None, "", (isNumber)? XmlDataType.Number: XmlDataType.Text);
			XPathNodeIterator iterator = navigator.Select(selectItems);
			//remove existing nodes from document
			itemsXmlRoot.RemoveAll();
			//re-add nodes re-ordered
			while (iterator.MoveNext()) {
				if (iterator.Current is IHasXmlNode) {
					XmlNode node = ((IHasXmlNode)iterator.Current).GetNode();
					itemsXmlRoot.AppendChild(node);
				}
			}
		}

		/// <summary>Converts the dataXml into CSVs </summary>
		/// <remarks>Note that this was taken from AwardsX.xmlToCsv, but differs in that the first row is assumed not to have data but used to get column names only</remarks>
		/// <returns>Returns an XmlDocument containing the Xml nodes in CSV format</returns>
		protected string _XmlToCSV(XmlElement Data) {
			return (_XmlToCSV(Data, false));
		}
		protected string _XmlToCSV(XmlElement Data, bool ignoreRowOne) {
			xLogger.Debug("xmlToCsv::start");
			StringBuilder csvData = new StringBuilder();
			string xpath_nodes = ".//child::*";
			// Get first row containing column names (from first element in selection)
			if (Data.SelectSingleNode(xpath_nodes) == null)
				return "";
			XmlNode firstNode = Data.SelectSingleNode(xpath_nodes);
			xLogger.Debug("xmlToCsv::", "firstNode:", firstNode.OuterXml);

			XmlAttributeCollection firstAttrs = firstNode.Attributes;
			for (int j = 0; j < firstAttrs.Count; j++) {
				XmlNode firstAttr = firstAttrs.Item(j);
				csvData.Append((j == 0) ? "" : TOKEN_COMMA);
				csvData.Append(firstAttr.Name);
			}
			XmlNodeList frstItemNodes = firstNode.SelectNodes(xpath_nodes);
			foreach (XmlNode firstChldNode in frstItemNodes) {
				csvData.Append((firstNode.FirstChild == firstChldNode && firstAttrs.Count == 0) ? "" : TOKEN_COMMA);
				csvData.Append(firstChldNode.Name);
			}
			csvData.Append("\n");
			// Now get data rows (NB first row is ignored)
			xLogger.Debug("xmlToCsv::add data");
			XmlNodeList DataList = Data.SelectNodes("child::*");
			for (int i = (ignoreRowOne)? 1 : 0; i < DataList.Count; i++) {
				XmlNode itemNode = DataList.Item(i);
				XmlAttributeCollection itemAttrs = itemNode.Attributes;
				for (int j = 0; j < itemAttrs.Count; j++) {
					XmlNode itemAttr = itemAttrs.Item(j);
					csvData.Append((j == 0) ? "" : TOKEN_COMMA);
					csvData.Append(itemAttr.Value.Replace(",", ";").Replace("\r\n", " ").Replace('\n', ' '));
				}
				XmlNodeList itemNodes = itemNode.SelectNodes(xpath_nodes);
				foreach (XmlNode itemChldNode in itemNodes) {
					csvData.Append((itemNode.FirstChild == itemChldNode && itemAttrs.Count == 0) ? "" : TOKEN_COMMA);
					csvData.Append(itemChldNode.InnerText.Replace(",", ";").Replace("\r\n", " ").Replace('\n', ' '));
				}
				csvData.Append("\n");
			}

			xLogger.Debug("xmlToCsv::finished:ok");
			return csvData.ToString();
		}

		/// <summary>Universal Handler for grid editing - typically deletes.</summary>
		/// <remarks>Note that this calls methods that need to be overridden in the implementation broker</remarks>
		protected void _GridEdit(string type) {
			try {
				switch (type) {
					case "duplicate":
						duplicate();
						break;
					case "delete":
					default: 
						delete(); 
						break;
				}
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				XmlNode entry = UIPage.Document.CreateElement(Cms.SERVICEX) as XmlNode;
				entry.InnerText = e.Message;
				UIPage.Content.AppendChild(entry);
				throw (new XException("error_grid_edit", String.Concat(error_grid_edit, e.Message)));
			}
		}
		/// <summary>NB: Override in implementaiton broker</summary>
		protected virtual void delete() {
		}
		/// <summary>NB: Override in implementaiton broker</summary>
		protected virtual void duplicate() {
		}
		/// <summary>Massages the html returned from the tiny_mce editor</summary>
		protected string processTinymceHtml(string html) {
			xLogger.Debug("processTinymceHtml: html::", html);
			
			html = html.Replace("<br>", "<br />");
			html = html.Replace("<BR>", "<BR />");
			string[] nodes = html.Split(new char[] { '<' });
			for (int x = 0; x < nodes.Length; x++) {
				if (nodes[x].Contains("img")) {
					if (nodes[x].Contains("/>") == false) {
						string newNode = nodes[x].Replace(">", "/>");
						html = html.Replace(nodes[x], newNode);
					}
				}
			}
			for (int x = 0; x < nodes.Length; x++) {
				if (nodes[x].Contains("IMG")) {
					if (nodes[x].Contains("/>") == false) {
						string newNode = nodes[x].Replace(">", "/>");
						newNode = newNode.Replace("border=0", "border=\"0\"");
						html = html.Replace(nodes[x], newNode);
					}
				}
			}
			return html;
		}
		/// <summary>
		/// Returns byte array for file
		/// </summary>
		protected byte[] _ReadToEnd(System.IO.Stream stream) {
			long originalPosition = stream.Position;
			stream.Position = 0;
			try {
				byte[] readBuffer = new byte[4096];
				int totalBytesRead = 0;
				int bytesRead;

				while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0) {
					totalBytesRead += bytesRead;

					if (totalBytesRead == readBuffer.Length) {
						int nextByte = stream.ReadByte();
						if (nextByte != -1) {
							byte[] temp = new byte[readBuffer.Length * 2];
							Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
							Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
							readBuffer = temp;
							totalBytesRead++;
						}
					}
				}

				byte[] buffer = readBuffer;
				if (readBuffer.Length != totalBytesRead) {
					buffer = new byte[totalBytesRead];
					Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
				}
				return buffer;
			} finally {
				stream.Position = originalPosition;
			}
		}

		protected bool _IsDefault(string value) {
			return (value == DEFAULT_SUBITEM || value.ToLower() == "default");
		}
		#endregion
		
        #region Private methods
		/// <summary>
		/// Initialises properties common to constructors
		/// </summary>
		private void initialise(CmsXProfileX thispage, Type type, string loggerID) {
			xLogger = new XLogger(typeof(CmsXBrokerBase), loggerID, false, false, ":");

			WantCmsAuth = (Config.Value("CmsX/Auth") == "CmsAuth");
			xLogger.Debug("initialise", "::WantCmsAuth:", WantCmsAuth.ToString(), "::_User:", _User.Username);

			// Initialise passport WS objects common to all: NB these object are now initialised on demand
			/*
			xLogger.Debug("initialise", "::_CustomerWS:", _CustomerWS.Url);
			xLogger.Debug("initialise", "::_AdminWS:", _AdminWS.Url);
			xLogger.Debug("initialise", "::_PassportWS:", _PassportWS.Url);
			*/

			xLogger.Debug("initialise:ok");
		}

		/// <summary>response text stream from a file</summary>
		private void textOut(string text) {
			textOut(text, null, null);

		}
		/// <summary>response text stream from a file</summary>
		private void textOut(string text, string filename) {
			textOut(text, filename, null);
		}
		/// <summary>response text stream from a file</summary>
		private void textOut(string text, string filename, string extension) {
			string file = (filename == null) ? Guid.NewGuid().ToString() : String.Concat(filename, "-", DateTime.Now.ToShortDateString());
			string ext = (extension == null) ? "csv" : extension;
			UIPage.Response.Clear();
			UIPage.Response.ClearHeaders();
			//UIPage.Response.ClearContent();

			UIPage.Response.AddHeader("Content-Disposition", String.Concat("filename=", file, ".", ext));
			UIPage.Response.ContentType = (ext == "csv")? "text/csv" : "text";
			UIPage.Response.Write(text);
			UIPage.Response.Flush();
			UIPage.Response.End();
		}

		/// <summary>response text stream from a file</summary>
		private int pageRowBelongsTo(int rowOrderNumber, int rowsPerPage) {
			int page = 0;
			double ratio = Convert.ToDouble(rowOrderNumber) / Convert.ToDouble(rowsPerPage);
			int ceil = (int)Math.Ceiling(ratio);
			int floor = (int)Math.Floor(ratio);
			if (ceil == floor)
				page = floor;
			else
				page = ceil - 1;
			return page;
		}

		/// <summary>
		/// Method to do a validation call against the validation web service
		/// </summary>
		/// <returns></returns>
		private bool _doValidation() {
			xLogger.Debug("_doValidation:", "::WantCmsAuth:", WantCmsAuth.ToString());
			
			string token = _User.Token;
			xLogger.Debug("_doValidation", "::token:", token, "::remember:", _User.Remember.ToString());

			try {
				if (String.IsNullOrEmpty(token)) {
					throw (new XException("error_token_null", "Token is null"));
				} else {
					if (WantCmsAuth) {
						_AdminX.Validate(token);
					} else {
						//_PassportX.Validate(token);
						_PassportX.ValidateToken(token, _User.Remember);
					}
				}
			} catch (XException e) {
				xLogger.Info("_doValidation:", Cms.PROFILE_TOKEN, ":", UserProfile.Value(Cms.PROFILE_TOKEN), ":invalid::", e.Code, "::message:", e.Message);
				return false;
			}
			return true;
		}

		/// <summary>Pass back validation Xml</summary>
		private void validate() {
			try {
				XmlDocument validation = new XmlDocument();
				validation.LoadXml(FORMAT_RESULT_XML);
				UIPage.Content.AppendChild(UIPage.Document.ImportNode(validation.DocumentElement as XmlNode, true));
				xLogger.Debug("validate:ok");
			} catch (XException e) {
				throw e;
			} catch (Exception e) {
				throw (new XException("error_validate", String.Concat(error_validate, e.Message)));
			}
		}
		#endregion
	}
}
