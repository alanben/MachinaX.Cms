/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2013-10-30
	Status:		release	
	Version:	4.0.3
	Build:		20160829
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20131030:	Started from LoeriesX.Generator
	20151014:	Refactor so that derived classes contain the PDF engine specific code, 
				ie a derived class will consist of:
				- Web service methods
				- _GeneratePdf overrides (specific to PDF engine)
	20160829:	Added Generate override to take template parameter list
	-----------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.GeneratorX {

	using XXBoom.MachinaX;
	using System;
	using System.IO;
	using System.Xml;
	using System.Xml.Xsl;

	/// <summary>
	/// Loeries Generator Base class
	/// </summary>
	public class GenerateBase : x_result {

		private const string CONFIG_ID = "default";
		private const string CONFIG_ROOT = "GeneratorX";
		
		private const string FILE_PDF_DIR = "GeneratorX/PdfDir";
		private const string FILE_TEMPLATE_DIR = "GeneratorX/Template";
		private const string FILE_HTML_DIR = "GeneratorX/HtmlDir";
		private const string FILE_EXCEL_DIR = "GeneratorX/ExcelDir";
		private const string FILE_INPUT_DIR = "GeneratorX/InputDir";

		private const string DEFAULT_PDF_DIR	= "Output/Pdf";
		private const string DEFAULT_HTML_DIR	= "Output/Html";
		private const string DEFAULT_EXCEL_DIR	= "Output/Excel";
		private const string DEFAULT_INPUT_DIR = "Uploads";

		private const string BASE_URL = "GeneratorX/BaseUrl";
		private const string PAGESIZE = "GeneratorX/PageSize";
		private const string PAGEFONT = "GeneratorX/Font";
		private const string PAGEFONTSIZE = "GeneratorX/FontSize";
		private const string WANTPDF = "GeneratorX/WantPdf";
		
		private const string SELECT_RESULT = "/Result";
		private const string SELECT_RESULT_CODE = "/Result/Result_Code";
		private const string SELECT_RESULT_DESC = "/Result/Description";

		private const string SELECT_PASSPORT_RESULT = "//Result";
		private const string SELECT_PASSPORT_RESULT_CODE = "//Result/ResultCode";
		private const string SELECT_PASSPORT_RESULT_DESC = "//Result/Description";

		public string ConfigID { get; set; }

		public XmlElement Generator {
			get { return Config.Element(String.Format("{0}/Generator[@id = '{1}']", CONFIG_ROOT, ConfigID)); }
		}

		/// <summary>Generator default Html output directory</summary>
		public string Html {
			get { return Config.Value(FILE_HTML_DIR, DEFAULT_HTML_DIR); }
			//set { input = value; }
		}
		/// <summary>Generator default Html output directory path</summary>
		public string HtmlDir {
			get { return Server.MapPath(Html); }
		}

		//private string output;
		/// <summary>Generator default Pdf output directory</summary>
		public string Pdf {
			get { return Config.Value(FILE_PDF_DIR, DEFAULT_PDF_DIR); }
			//set { output = value; }
		}
		/// <summary>Generator default Pdf output directory path</summary>
		public string PdfDir {
			get { return Server.MapPath(Pdf); }
		}

		/// <summary>Generator default Excel output directory</summary>
		public string Excel {
			get { return Config.Value(FILE_EXCEL_DIR, DEFAULT_EXCEL_DIR); }
			//set { input = value; }
		}
		/// <summary>Generator default Excel output directory path</summary>
		public string ExcelDir {
			get { return Server.MapPath(Excel); }
		}

		/// <summary>Generator default input directory</summary>
		public string Input {
			get { return Config.Value(FILE_INPUT_DIR, DEFAULT_INPUT_DIR); }
			//set { input = value; }
		}
		/// <summary>Generator default input directory path</summary>
		public string InputDir {
			get { return Server.MapPath(Input); }
		}

		/// <summary>Generator template file directory</summary>
		public string TemplateDir {
			get { return Server.MapPath(Config.Value(FILE_TEMPLATE_DIR)); }
		}

		/// <summary>Generator template file directory</summary>
		public string BaseUrl {
			get { return Config.Value(BASE_URL); }
		}

		/// <summary>Generator template file directory</summary>
		public string PageSize {
			get { return Config.Value(PAGESIZE); }
		}

		/// <summary>Generator template file directory</summary>
		public string Font {
			get { return Config.Value(PAGEFONT); }
		}

		/// <summary>Generator template file directory</summary>
		public string FontSize {
			get { return Config.Value(PAGEFONTSIZE); }
		}

		public string WantPdf {
			get { return Config.Value(WANTPDF, "no"); }
		}

		private x_config config;
		/// <summary>Generator default output file directory</summary>
		/// <value>full directory path</value>
		public x_config Config {
			get { return config; }
			set { config = value; }
		}

		private x_logger logger;
		/// <summary>Generator default output file directory</summary>
		/// <value>full directory path</value>
		public x_logger xLogger {
			get { return logger; }
			set { logger = value; }
		}

		public Guid Tok {
			get { return Guid.NewGuid(); }
		}

		public string Token {
			get { return Tok.ToString(); }
		}


		///	<overloads>Constructors</overloads>
		/// <summary>Constructor that defines the id</summary>
		public GenerateBase(string thisid, string configID) : base(thisid) {
			ConfigID = configID;
			initialise();
		}
		/// <summary>Default constructor</summary>
		public GenerateBase() : base("GenerateBase") {
			ConfigID = CONFIG_ID;
			initialise();
		}
		private void initialise() {
			//xLogger = new x_logger(typeof(GenerateBase), "gen---", true, false);
			xLogger = new x_logger(typeof(GenerateBase), "gen---", false, ":");
			Config = new x_config();
			xLogger.Debug("initialise", "HtmlDir", HtmlDir, "PdfDir", PdfDir, "ExcelDir", ExcelDir);

			ResultType = x_resultType.webservice;
		}


		/// <summary>get the WS config URL</summary>
		protected string _GetUrl(string id) {
			return Config.Value(String.Concat(CONFIG_ROOT, "/", "Url[@id='", id, "']"));
		}
		/// <summary>check WS result</summary>
		protected XmlNode _CheckWSResult(XmlNode result) {
			return checkResult(false, result, false);
		}
		/// <summary>check WS result</summary>
		private XmlNode checkResult(bool isPassport, XmlNode result, bool throwCode) {
			xLogger.Debug("CheckResult", "::isPassport:", isPassport.ToString(), "::result:", result.OuterXml);

			XmlElement codeEl = result.SelectSingleNode(isPassport ? SELECT_PASSPORT_RESULT_CODE : SELECT_RESULT_CODE) as XmlElement;
			string code = codeEl.InnerText;
			string desc = result.SelectSingleNode(isPassport ? SELECT_PASSPORT_RESULT_DESC : SELECT_RESULT_DESC).InnerText;
			XmlElement resultEl = result.SelectSingleNode(isPassport ? SELECT_PASSPORT_RESULT : SELECT_RESULT) as XmlElement;

			bool isOK = (code == "0");
			if (!isOK) {
				if (throwCode) {
					throw (new XException(code, desc));
				} else {
					throw (new XException("error_service", String.Concat("::code:", code, "::description:", desc)));
				}
			}
			return result;
		}


        /// <summary>Generate from html file</summary>
        /// <param name="urlStrCSV">The urls list comma sepparated to generate pdf page per url from.</param>
        public string Generate(string urlStrCSV) {
            return Generate(urlStrCSV, null);
        }

        /// <summary>Generate from html file</summary>
        /// <param name="location">The file name or the url.</param>
        /// <param name="isUrl">Flag to indicate file name or url.</param>
        // <param name="location">The name prefix of the html file in the html directory.</param>
		public string Generate(string urlStrCSV, string filename) {
            string[] urls = urlStrCSV.Split(new char[] { ',' });
            string fileName = String.Concat((filename==null) ? _GetToken() : filename, ".pdf");
			string outputFile = String.Concat(PdfDir, "\\", fileName);
			_GeneratePdf(urls, outputFile);
            return fileName;
        }

        /// <summary>Generate from html (url or file in Html)</summary>
        /// <param name="location">The file name or the url.</param>
        /// <param name="isUrl">Flag to indicate file name or url.</param>
        // <param name="location">The name prefix of the html file in the html directory.</param>
		public string Generate(string location, bool isUrl) {
            return Generate(location, isUrl, null);
        }

        /// <summary>Generate from html file</summary>
        /// <param name="location">The file name or the url.</param>
        /// <param name="isUrl">Flag to indicate file name or url.</param>
        // <param name="location">The name prefix of the html file in the html directory.</param>
		public string Generate(string location, bool isUrl, string filename) {
			string thisurl = (isUrl) ? location : String.Concat(HtmlDir, "\\", location, ".html");
            string fileName = String.Concat((filename == null) ? _GetToken() : filename, ".pdf");
			string outputFile = String.Concat(PdfDir, "\\", fileName);
			xLogger.Debug("Generate", "::outputFile:", outputFile);

			_GeneratePdf(thisurl, outputFile);
			return fileName;
		}


		public XmlDocument Generate(XmlElement Source, string TemplateName) {
			Guid tok = Guid.NewGuid();
			return Generate(Source, TemplateName, tok.ToString(), (WantPdf == "yes"));
		}

		public XmlDocument Generate(XmlElement Source, string TemplateName, string FileName) {
			return Generate(Source, TemplateName, FileName, (WantPdf == "yes"));
		}

		public XmlDocument Generate(XmlElement Source, string TemplateName, string FileName, XsltArgumentList args) {
			return Generate(Source, TemplateName, FileName, (WantPdf == "yes"), args, null);
		}
		public XmlDocument Generate(XmlElement Source, string FileName, XslCompiledTransform template, XsltArgumentList args) {
			return Generate(Source, FileName, (WantPdf == "yes"), template, args, null);
		}

		public XmlDocument Generate(XmlElement Source, string TemplateName, string FileName, bool WantPDF) {
			return Generate(Source, TemplateName, FileName, WantPDF, null, null);
		}

		public XmlDocument Generate(XmlElement Source, string TemplateName, string FileName, bool WantPDF, XsltArgumentList args, XmlNode OptionsXml) {
			XslCompiledTransform templ = getTemplate(TemplateName);
			return Generate(Source, FileName, WantPDF, templ, args, OptionsXml);
		}

		public XmlDocument Generate(XmlElement Source, string FileName, bool WantPDF, XslCompiledTransform template, XsltArgumentList args, XmlNode OptionsXml) {
			xLogger.Debug("Generate", "FileName", FileName, "WantPDF", WantPDF.ToString());

			string htmlfile = String.Concat(HtmlDir, "\\", FileName, ".html");
			string htmlurl = String.Concat(BaseUrl, "/", Html, "/", FileName, ".html");

			_CreatePageHTML(htmlfile, Source, template, args);

			AddOk();
			AddNode("htmlfile", htmlfile);
			AddNode("htmlurl", htmlurl);
			xLogger.Debug("Generate", "htmlfile", htmlfile, "htmlurl", htmlurl);

			if (WantPDF) {
				string pdffile = String.Concat(PdfDir, "\\", FileName, ".pdf");
				string pdfurl = String.Concat(BaseUrl, "/", Pdf, "/", FileName, ".pdf");
				xLogger.Debug("Generate", "pdffile", pdffile, "pdfurl", pdfurl);

				// create pdf...
				//_Generate(htmlurl, pdffile);
				_GeneratePdf(htmlurl, pdffile, OptionsXml);

				AddNode("pdffile", pdffile);
				AddNode("pdfurl", pdfurl);
			}
			xLogger.Debug("Result", Result);

			return (Result);
		}

		public XmlDocument Generate(XmlDocument Source, string TemplateName, string FilePath, XsltArgumentList args) {
			xLogger.Debug("generate", "FilePath", FilePath, "TemplateName", TemplateName);

			return Generate(Source, FilePath, getTemplate(TemplateName), args);
		}
		public XmlDocument Generate(XmlDocument Source, string FilePath, XslCompiledTransform template, XsltArgumentList args) {
			_CreatePageHTML(FilePath, Source, template, args);
			AddNode("filepath", FilePath);
			xLogger.Debug("generate", "filepath", FilePath);

			return (Result);
		}

		/// <summary>Generate from html (url or file in Html)</summary>
		/// <param name="location">The file name or the url.</param>
		/// <param name="isUrl">Flag to indicate file name or url.</param>
		// <param name="location">The name prefix of the html file in the html directory.</param>
		protected void _Generate(string htmlFilepath, string pdfFilepath) {
			xLogger.Debug("_Generate", "htmlFilepath", htmlFilepath);
			xLogger.Debug("_Generate", "pdfFilepath", pdfFilepath);
			_GeneratePdf(htmlFilepath, pdfFilepath);
		}


		protected void _CreatePageHTML(string filepath, XmlElement source, string templateName, XsltArgumentList args) {
			XslCompiledTransform templ = getTemplate(templateName);
			_CreatePageHTML(filepath, source, templ, args);
		}
		protected void _CreatePageHTML(string filepath, XmlDocument source, string templateName, XsltArgumentList args) {
			XslCompiledTransform templ = getTemplate(templateName);
			_CreatePageHTML(filepath, source, templ, args);
		}
		protected void _CreatePageHTML(string filepath, XmlElement source, XslCompiledTransform templ, XsltArgumentList args) {
			writePageHTML(filepath, source, templ, args);
		}
		protected void _CreatePageHTML(string filepath, XmlDocument source, XslCompiledTransform templ, XsltArgumentList args) {
			writePageHTML(filepath, source, templ, args);
		}


		/// <summary>
		/// Loads a xsl template for special purposes from the templates directory
		/// </summary>
		public XslCompiledTransform GetTemplate(string thisfile) {
			return getTemplate(thisfile);
		}
		private XslCompiledTransform getTemplate(string thisfile) {
			xLogger.Debug("getTemplate", "thisfile", thisfile);
			try {
				XslCompiledTransform templ = new XslCompiledTransform();
				string tmplpath = String.Concat(TemplateDir, "/", thisfile, ".xsl");
				xLogger.Debug("getTemplate", "tmplpath", tmplpath);

				templ.Load(tmplpath);
				return (templ);
			} catch (XsltCompileException e) {
				throw new XException("template_compile_error", String.Format("Template compile error: '{0}' at line {1} position {2}", e.Message, e.LineNumber, e.LinePosition));
			} catch (Exception e) {
				throw new XException("template_error", String.Format("Template error: '{0} [{1}]'", e.Message, e.ToString()));
			}
		}
		private void writePageHTML(string filepath, XmlElement source, XslCompiledTransform xslt, XsltArgumentList args) {
			using (StreamWriter writer = new StreamWriter(filepath)) {
				xslt.Transform(source, args, writer);
			}
		}
		private void writePageHTML(string filepath, XmlDocument source, XslCompiledTransform xslt, XsltArgumentList args) {
			using (StreamWriter writer = new StreamWriter(filepath)) {
				xslt.Transform(source, args, writer);
			}
		}


		/// <summary>getToken::generates a token</summary>
		/// <note>NB: this method is a potential inclusion in a x_blogbase class</note>
		protected string _GetToken() {
			Random rdm = new Random(unchecked((int)DateTime.Now.Ticks)); 
			Byte[] b = new Byte[16];
			rdm.NextBytes(b);
			Guid thisguid = new Guid(b) ;
            return (thisguid.ToString("D"));
        }

        /// <summary>generate::generates a _Pdf frrom a url</summary>
		/// <remarks>NB: Needs to be overridden in inheriting class</remarks>
		protected virtual void _GeneratePdf(string[] urls, string filepath) {
			xLogger.Debug("_GeneratePdf", "filepath", filepath);
		}

		/// <summary>generate::generates a _Pdf frrom a url</summary>
		/// <remarks>NB: Needs to be overridden in inheriting class</remarks>
		protected void _GeneratePdf(string url, string filepath) {
			_GeneratePdf(url, filepath, null);
		}

		/// <summary>generate::generates a _Pdf frrom a url</summary>
		/// <remarks>NB: Needs to be overridden in inheriting class</remarks>
		protected virtual void _GeneratePdf(string url, string filepath, XmlNode options) {
			xLogger.Debug("_GeneratePdf", "url", url, "filepath", filepath, "::options:", options);
        }

		/// <summary>generate::generates a _Pdf frrom a url</summary>
		/// <remarks>NB: Needs to be overridden in inheriting class</remarks>
		protected virtual void _GeneratePdf(string url, Stream stream) {
			xLogger.Debug("_GeneratePdf", "url", url);
		}

		/// <summary>generate::generates a _Pdf frrom a url</summary>
		/// <remarks>NB: Needs to be overridden in inheriting class</remarks>
		protected virtual void _GeneratePdf(string url) {
			xLogger.Debug("_GeneratePdf", "url", url);
		}
	}
}
