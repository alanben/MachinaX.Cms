/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.3
	Build:		20160822
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Refactored from LoeriesAdmin
	20110905:	Replaced Debugger.Spoor with xLogger.Debug
	20130418:	Added Authenticate property to manage authentication
	20160822:	Refactor (minor)
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	
	using System;
	using System.Xml;

	/// <summary>
	/// Description of the classX class.
	/// <para>Additional information about the class</para>
	/// </summary>
	public class CmsXProfileX : profileX {

		private const string logid = "CmsXProfileX.";
		private const string CONFIG_ROOT = "WebsiteX";

		protected new x_logger xLogger;

		public CmsXCookies Cooker { get; set; }
		public CmsXUser WebsiteUser { get; set; }
		public bool Authenticate { get; set; }

		/// <summary>Determines how the list data is output</summary>
		public ListOutputStyle ListStyle { get; set; }

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public CmsXProfileX() : base(CONFIG_ROOT) {
			initialise(typeof(CmsXProfileX), logid);
		}
		/// <summary>Constructor for derived classes</summary>
		public CmsXProfileX(string label) : base(label) {
			initialise(typeof(CmsXProfileX), logid);
		}
		/// <summary>Constructor for derived classes</summary>
		public CmsXProfileX(string label, Type type, string loggerID) : base(label) {
			initialise(type, loggerID);
		}
		#endregion

        protected override void _Initialise() {
			base._Initialise();
			WebsiteUser = new CmsXUser(UserProfile);
			Cooker = new CmsXCookies(this);
		}
		protected override void _Initialise(string linkName) {
			base._Initialise(linkName);
			WebsiteUser = new CmsXUser(UserProfile);
			Cooker = new CmsXCookies(this);
		}
		protected override void _ReInitialise(string linkName) {
			base._ReInitialise(linkName);
			WebsiteUser = new CmsXUser(UserProfile);
			Cooker = new CmsXCookies(this);
		}

		/// <summary>Initialises the page and adds custom arguments to be passed to the xsl processor</summary>
		protected override void _InitialiseTemplate() {
            base._InitialiseTemplate();
			xLogger.Debug("_InitialiseTemplate", "skin", Cooker.Skin, "email", WebsiteUser.Username, "fullname", WebsiteUser.Fullname);

			TemplateArguments.AddParam("skin", "", Cooker.Skin);
			TemplateArguments.AddParam("user", "", WebsiteUser.Username);
			TemplateArguments.AddParam("person", "", WebsiteUser.Fullname);            
		}

		/// <summary>
		/// Post-processes the content for profiling (menus/links at this stage).
		/// NB this is done by calling the base method so that form fields and includes are done first.
		/// </summary>
		protected override void _PostProcessContent() {
			base._PostProcessContent();
			xLogger.Debug("PostProcessContent");
			if (UserProfile.LoginPattern != "") {
				xLogger.Debug("_PostProcessContent", "fields[pattern]");
				foreach (XmlNode fieldsNode in Content.SelectNodes("//content/page/fields[pattern]")) {
					try {
						string pattern = fieldsNode.SelectSingleNode("pattern").InnerText;
						XmlElement RightNode = UserProfile.Node("accessxml").SelectSingleNode(String.Concat("//items/item[@category='", pattern, "']")) as XmlElement;
						XmlNode SubEdit = fieldsNode.SelectSingleNode("@subedit");
						if (RightNode != null) {
							int right = Convert.ToInt32(RightNode.SelectSingleNode("@right").InnerText);
							//for admin users with full edit rights
							if (right == 3) {
								((XmlElement)fieldsNode).SetAttribute("readonly", "false");
								xLogger.Debug("fieldsNode:", fieldsNode.SelectSingleNode("@grid").OuterXml);
							//for admin users with sub edit rights
							} else if (SubEdit != null && right == 2) {
								((XmlElement)fieldsNode.SelectSingleNode("../grid/columns")).SetAttribute("delete", "no"); fieldsNode.SelectSingleNode("fld[@type='grid']/not_editable").InnerText = "Y";
								((XmlElement)fieldsNode).RemoveAttribute("ttl_new");
								((XmlElement)fieldsNode).SetAttribute("readonly", "false");
								XmlNodeList groupNodeList = fieldsNode.SelectNodes("group");
								foreach (XmlNode groupNode in groupNodeList) {
									XmlDocumentFragment frag = Content.OwnerDocument.CreateDocumentFragment();
									foreach (XmlNode fldNode in groupNode) {
										if (fldNode.Name == "fld") {
											XmlNode Edit = fldNode.SelectSingleNode("@specialedit");
											if (Edit == null) {
												XmlNode copy = fldNode.CloneNode(true);
												((XmlElement)copy).SetAttribute("type", "fld_hidden");
												frag.AppendChild(copy);
												string name = fldNode.SelectSingleNode("@name").InnerText;
												((XmlElement)fldNode).SetAttribute("name", String.Concat(name, "Copy"));
											};
											((XmlElement)fldNode).SetAttribute("disabled", "yes");
										}
									}
									groupNode.AppendChild(frag);
								}
								XmlNodeList fldsNodeList = Content.SelectNodes("//content/page/fields/fld[@type='grid']/subobjs/subwf_obj/fld");
								foreach (XmlNode fldNode in fldsNodeList) {
									((XmlElement)fldNode).SetAttribute("locked", "yes");
								}
							} else {
								((XmlElement)fieldsNode).RemoveAttribute("ttl_new"); 
								((XmlElement)fieldsNode.SelectSingleNode("../grid/columns")).SetAttribute("delete", "no"); 
								fieldsNode.SelectSingleNode("fld[@type='grid']/not_editable").InnerText = "Y";
							}
						}else{
								((XmlElement)fieldsNode).RemoveAttribute("ttl_new");
						}
					} catch (Exception e) {
						xLogger.Debug("_PostProcessContent", "error", e.Message);
					}
				}
				//for admin users with no judging/winners/finalists view rights- this makes the judging secion in the entries have hidden fields
				XmlNodeList GroupsNode = Content.SelectNodes("//content/page/fields/group[pattern]");
				xLogger.Debug("_PostProcessContent", "group[pattern]");
				foreach (XmlNode groupNode in GroupsNode) {
					foreach (XmlNode fldNode in groupNode) { 
						try {
							xLogger.Debug("flds", fldNode.OuterXml);
							string pattern = fldNode.SelectSingleNode("../pattern").InnerText;
							XmlElement RightNode = UserProfile.Node("accessxml").SelectSingleNode(String.Concat("//items/item[@category='", pattern, "']")) as XmlElement;
							if (RightNode == null && fldNode.Name.Equals("fld")) {
								((XmlElement)fldNode).SetAttribute("type", "fld_hidden");
							}
							xLogger.Debug("fldsPatternNode", fldNode.SelectSingleNode("../pattern").OuterXml);
						} catch (Exception e) {
							xLogger.Debug("_PostProcessContent", "error", e.Message);
						}
					}
				}
				XmlNodeList SearchNode = Content.SelectNodes("//content/page/grid/filters[@title='Search']/filter[pattern]");
				xLogger.Debug("_PostProcessContent", "filter[pattern]");
				foreach (XmlNode filterNode in SearchNode) {
					try {
						string pattern = filterNode.SelectSingleNode("pattern").InnerText;
						XmlElement RightNode = UserProfile.Node("accessxml").SelectSingleNode(String.Concat("//items/item[@category='", pattern, "']")) as XmlElement;
						if (RightNode == null) {
							XmlNode remove = filterNode;
							remove.ParentNode.RemoveChild(remove);
						}
					} catch (Exception e) {
						xLogger.Debug("_PostProcessContent", "error", e.Message);
					}
				}
				XmlNode ExportCSV = Content.SelectSingleNode("//content/page/para[pattern]");
				xLogger.Debug("_PostProcessContent", "para[pattern]");
				try {
					if (ExportCSV != null) {
						string pattern = ExportCSV.SelectSingleNode("pattern").InnerText;
						XmlElement RightNode = UserProfile.Node("accessxml").SelectSingleNode(String.Concat("//items/item[@category='", pattern, "']")) as XmlElement;
						if (RightNode == null) {
							XmlNode remove = ExportCSV;
							remove.ParentNode.RemoveChild(remove);
						}
					}
				} catch (Exception e) {
					xLogger.Debug("_PostProcessContent", "error", e.Message);
				}
			}
		}

		private void initialise(Type type, string loggerID) {
			xLogger = new x_logger(type, loggerID, false, true);
			Authenticate = true;
			xLogger.Debug("initialize");

			// Firts get global setting (applies to all sites - in CmsX section)
			string listOutputStyle = Config.Value(String.Concat(Cms.CONFIG_ROOT, "/ListOutputStyle"), Cms.DEFAULT_LISTOUTPUTSTYLE);
			xLogger.Debug("initialize", "::listOutputStyle:", listOutputStyle);

			// Get BlogX specific setting (applies only to this - in BlogX section)
			listOutputStyle = Config.Value(String.Format(Cms.BLOGX_LISTOUTPUTSTYLE, Label), listOutputStyle);
			xLogger.Debug("initialize", "::listOutputStyle:", listOutputStyle);

			if (listOutputStyle == ListOutputStyle.XmlDirect.ToString()) {
				ListStyle = ListOutputStyle.XmlDirect;
			} else if (listOutputStyle == ListOutputStyle.Datatable.ToString()) {
				ListStyle = ListOutputStyle.Datatable;
			} else {
				ListStyle = ListOutputStyle.XmlTransform;
			}
			xLogger.Debug("initialize", "::ListStyle:", ListStyle.ToString());
		}
	}
}
