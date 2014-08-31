using System;
using System.Xml;

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
	20100701:	Refactored from LoeriesAdmin
	20140112:	Refactored to derive from CmsXBroker
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>
	/// Base class for brokers that use templates and data in Application / Session memory.
	/// <para></para>
	/// </summary>
	public class CmsXBrokerTemplate : CmsXBroker {
		#region Invisible properties
		private const string error_templateElement			= "Template Error: ";
		private const string error_templateElement_null		= "No template";
		private const string error_templateElement_read		= "Error reading template";
		private const string error_templateElement_loadxml	= "Error parsing template";
		#endregion

		#region Visible properties
		private string template;
		/// <summary>The page's template</summary>
		/// <value>A template string</value>
		public string Template {
			get { return template; }
			set { template = value; }
		}
		private XmlElement templateElement;
		/// <summary>The page's templateElement</summary>
		/// <value>A document containing the templateElement of the page</value>
		public XmlElement TemplateElement {
			get { return templateElement; }
			set { templateElement = value; }
		}
		private XmlDocument templateDocument;
		/// <summary>The page's templateDocument</summary>
		/// <value>the template XmlDocument</value>
		public XmlDocument TemplateDocument {
			get { return templateDocument; }
			set { templateDocument = value; }
		}
		#endregion

		#region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerTemplate(CmsXProfileX thispage) : base(thispage) {
			initialise();
		}
		/// <summary>Constructor description</summary>
		/// <param name="thistemplate">The page's template</param>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerTemplate(CmsXProfileX thispage, string thistemplate) : base(thispage) {
			template = thistemplate;
			initialise();
		}
		#endregion

		#region Public methods
		/// <overloads>Gets the page from the template specified</overloads>
		/// <summary>The template is supplied</summary>
		/// <param name="thistemplate">The page's template</param>
		public void Get(string thistemplate) {
			template = thistemplate;
			Get();
		}
		/// <summary>The template is assumed to have been set</summary>
		/// <param name="pageel">The content element on the page</param>
		public void Get(XmlElement pageel) {
			getTemplate();
			if (pageel != null) {
				pageel.AppendChild(pageel.OwnerDocument.ImportNode(templateElement, true));
			}
		}
		/// <summary>The template is assumed to have been set</summary>
		public void Get() {
			getTemplate();
		}

		/// <overloads>Gets the document from the template specified</overloads>
		/// <summary>The template is supplied</summary>
		/// <param name="thistemplate">The page's template document</param>
		public void GetDocument(string thistemplate) {
			template = thistemplate;
			GetDocument();
		}
		/// <summary>The template is assumed to have been set</summary>
		public void GetDocument() {
			templateDocument = (XmlDocument)UIPage.Application[template];
		}

		/// <summary>Gets and element in teh template document</summary>
		/// <remarks>The template is assumed to have been set</remarks>
		public XmlElement GetElement(string xpath) {
			return templateDocument.SelectSingleNode(xpath) as XmlElement;
		}
		#endregion
		
		#region Protected methods
		#endregion
				
		#region Private methods
		/// <summary>
		/// Description of this method
		/// </summary>
		private void initialise() {
		}
		/// <summary>
		/// Description of this method
		/// </summary>
		private void getTemplate() {
			templateElement = (XmlElement) UIPage.Application[template];
			if (templateElement == null)
				throw new x_exception("error_templateElement_null", String.Concat(error_templateElement, error_templateElement_null));
				
		}
		#endregion
	}
}
