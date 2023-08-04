using System;
/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2015-10-14
	Status:		release	
	Version:	4.0.3
	Build:		20160609
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20151014:	Started from GeneratePageAbc
				Contain wkhtmltopdf (http://wkhtmltopdf.org/) specific code, ie
				- Web service methods
				- _GeneratePdf overrides (specific to ABCpdf)
	20160606:	Quote input and ouput when invoking wkhtmltopdf
	20160609:	Removed Quoted parameters on wkhtmltopdf - single quotes dont work
	-----------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.GeneratorX {

	using XXBoom.MachinaX;
	using System.IO;
	using System.Web.Services;
	using System.Xml;
	using System.Xml.Xsl;
	using System.Diagnostics;
	using System.Text.RegularExpressions;
	
	/// <summary>
	/// Credit Generator web service
	/// </summary>
	[WebService(Name = "GeneratePageWKhtml",
				Namespace = "http://www.clickclickBOOM.com/MachinaX/GenerateX",
				Description = "GeneratePageWKhtml Web Service")]
	public class GeneratePageWKhtml : GenerateBase {

		private const string CONFIG_ID = "wkhtmltopdf";

		/// <summary>Constructor</summary>
		public GeneratePageWKhtml() : base("GeneratePage", CONFIG_ID) {
		}
		public GeneratePageWKhtml(string configID) : base("GeneratePage", configID) {
		}


		/// <summary>Generate html and optionally an equivalent pdf file.</summary>
		/// <param name="SourceXml">A string containing valid xml</param>
		/// <param name="TemplateName">The xslt template name</param>
		/// <param name="WantPDF">Flag to indicate PDF generation</param>
		[WebMethod(Description = "Generate html and optionally an equivalent pdf file.")]
		public XmlDocument Generate(string SourceXml, string TemplateName, bool WantPDF) {
			xLogger.Debug("Generate", "SourceXml", SourceXml);
			try {
				Guid tok = Guid.NewGuid();
				XmlDocument sourceDoc = new XmlDocument();
				sourceDoc.LoadXml(SourceXml);
				Generate(sourceDoc.DocumentElement, TemplateName, tok.ToString(), WantPDF);

				xLogger.Debug("Generate:ok");
			} catch (XException e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
            return (Result);
        }

		/// <summary>Generate a html file and optionally an equivalent pdf file.</summary>
		/// <param name="SourceXml">A string containing valid xml</param>
		/// <param name="TemplateName">The xslt template name</param>
		/// <param name="WantPDF">Flag to indicate PDF generation</param>
		[WebMethod(Description = "Generate a html file and optionally an equivalent pdf file.")]
		public XmlDocument GenerateFile(string SourceXml, string TemplateName, string FileName, bool WantPDF) {
			xLogger.Debug("GenerateFile", "SourceXml", SourceXml);
			try {
				XmlDocument sourceDoc = new XmlDocument();
				sourceDoc.LoadXml(SourceXml);
				Generate(sourceDoc.DocumentElement, TemplateName, FileName, WantPDF);

				xLogger.Debug("GenerateFile:ok");
			} catch (XException e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		/// <summary>Generate a pdf file from a url.</summary>
		/// <param name="Url">The url from which to generate a PDF</param>
		[WebMethod(Description = "Generate a pdf file from a url.")]
		public XmlDocument GeneratePDFfromUrl(string Url) {
			xLogger.Debug("GeneratePDFfromUrl", "Url", Url);

			try {
				addOkPdf(Generate(Url, true));

				xLogger.Debug("GeneratePDFfromUrl:ok");
			} catch (XException e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		private void addOkPdf(string pdffile) {
			AddOk();
			AddNode("pdffile", pdffile);
			AddNode("pdfpath", String.Concat(PdfDir, "\\", pdffile));
		}

		/// <summary>Generate a pdf file from a url.</summary>
		/// <param name="Url">The url from which to generate a PDF</param>
		[WebMethod(Description = "Generate a pdf file from a url.")]
		public XmlDocument GeneratePDFFilefromUrl(string Url, string Filename) {
			xLogger.Debug("GeneratePDFFilefromUrl", "Url", Url, "Filename", Filename);

			try {
				addOkPdf(Generate(Url, true, Filename));

				xLogger.Debug("GeneratePDFFilefromUrl:ok");
			} catch (XException e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		/// <summary>Generate a pdf file from a url using a specific configuration</summary>
		/// <param name="Url">The url from which to generate a PDF</param>
		/// <param name="OptionsID">The configuration Options[id='OptionsID']</param>
		[WebMethod(Description = "Generate a pdf file from a url.")]
		public XmlDocument GeneratePDFfromUrlOptions(string Url, string OptionsID) {
			xLogger.Debug("GeneratePDFfromUrlOptions", "::Url:", Url, "::OptionsID:", OptionsID);

			try {
				addOkPdf(generatePDFfromUrlOptions(Url, true, null, OptionsID));

				xLogger.Debug("GeneratePDFfromUrl:ok");
			} catch (XException e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		/// <summary>Generate a pdf file from a url using a specific configuration</summary>
		/// <param name="Url">The url from which to generate a PDF</param>
		/// <param name="Filename">The name of the file to be created</param>
		/// <param name="OptionsID">The configuration Options[id='OptionsID']</param>
		[WebMethod(Description = "Generate a pdf file from a url.")]
		public XmlDocument GeneratePDFFilefromUrlOptions(string Url, string Filename, string OptionsID) {
			xLogger.Debug("GeneratePDFFilefromUrlOptions", "::Url:", Url, "::Filename::", Filename, "::OptionsID:", OptionsID);

			try {
				addOkPdf(generatePDFfromUrlOptions(Url, true, Filename, OptionsID));

				xLogger.Debug("GeneratePDFFilefromUrl:ok");
			} catch (XException e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		/// <summary>Generate a pdf file from a url using Options xml</summary>
		/// <param name="Url">The url from which to generate a PDF</param>
		/// <param name="OptionsXml">An Options node</param>
		[WebMethod(Description = "Generate a pdf file from a url.")]
		public XmlDocument GeneratePDFfromUrlOptionsXml(string Url, XmlNode OptionsXml) {
			xLogger.Debug("GeneratePDFfromUrlOptionsXml", "::Url:", Url, "::OptionsXml:", OptionsXml);

			try {
				addOkPdf(generatePDFfromUrlOptions(Url, true, null, OptionsXml));

				xLogger.Debug("GeneratePDFfromUrl:ok");
			} catch (XException e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		/// <summary>Generate a pdf file from a url using Options xml</summary>
		/// <param name="Url">The url from which to generate a PDF</param>
		/// <param name="Filename">The name of the file to be created</param>
		/// <param name="OptionsXml">An Options node</param>
		[WebMethod(Description = "Generate a pdf file from a url.")]
		public XmlDocument GeneratePDFFilefromUrlOptionsXml(string Url, string Filename, XmlNode OptionsXml) {
			xLogger.Debug("GeneratePDFFilefromUrlOptionsXml", "::Url:", Url, "::Filename::", Filename, "::OptionsXml:", OptionsXml.OuterXml);

			try {
				addOkPdf(generatePDFfromUrlOptions(Url, true, Filename, OptionsXml));

				xLogger.Debug("GeneratePDFFilefromUrl:ok");
			} catch (XException e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		private string generatePDFfromUrlOptions(string location, bool isUrl, string filename, string optionsid) {
			string thisurl = (isUrl) ? location : String.Concat(HtmlDir, "\\", location, ".html");
			string fileName = String.Concat((filename == null) ? _GetToken() : filename, ".pdf").Replace(' ', '_');
			string outputFile = String.Concat(PdfDir, "\\", fileName);
			xLogger.Debug("generatePDFfromUrlOptions", "::outputFile:", outputFile);

			_GeneratePdf(thisurl, outputFile, optionsid);
			return fileName;
		}
		private string generatePDFfromUrlOptions(string location, bool isUrl, string filename, XmlNode options) {
			string thisurl = (isUrl) ? location : String.Concat(HtmlDir, "\\", location, ".html");
			string fileName = String.Concat((filename == null) ? _GetToken() : filename, ".pdf").Replace(' ', '_');
			string outputFile = String.Concat(PdfDir, "\\", fileName);
			xLogger.Debug("generatePDFfromUrlOptions", "::outputFile:", outputFile);

			_GeneratePdf(thisurl, outputFile, options);
			return fileName;
		}

		public XmlDocument GeneratePDFFilefromXmlOptionsXml(XmlElement Source, string TemplateName, string FileName, XsltArgumentList TemplateArgs, XmlNode OptionsXml) {
			xLogger.Debug("GeneratePDFFilefromXmlOptionsXml", "::TemplateName:", TemplateName, "::FileName::", FileName, "::OptionsXml:", OptionsXml);

			return Generate(Source, TemplateName, FileName, true, TemplateArgs, OptionsXml);
		}

		/// <summary>generate::generates a _Pdf frrom a url</summary>
		protected override void _GeneratePdf(string[] urls, string filepath) {
			xLogger.Debug("_GeneratePdf", "filepath", filepath);

			foreach (string url in urls) { 
				_GeneratePdf(url);
			}
		}

		/// <summary>generate::generates a _Pdf frrom a url</summary>
		//protected void _GeneratePdf(string url, string filepath) {
		//	_GeneratePdf(url, filepath, String.Empty);
		//}
		protected void _GeneratePdf(string url, string filepath, string optionsid) {
			xLogger.Debug("_GeneratePdf", ":url:", url, "::filepath:", filepath);

			wkhtmltopdf(getOptions(optionsid), url, filepath);
		}
		protected override void _GeneratePdf(string url, string filepath, XmlNode options) {
			xLogger.Debug("_GeneratePdf", ":url:", url, "::filepath:", filepath, "::options:", options);

			wkhtmltopdf(getOptions(options), url, filepath);
		}

		/// <summary>generate::generates a _Pdf frrom a url</summary>
		protected override void _GeneratePdf(string url, Stream stream) {
			xLogger.Debug("_GeneratePdf", "url", url);

			string pdffilepath = pdfpathFromurl(url);
			wkhtmltopdf(getOptions(String.Empty), url, pdffilepath);

			using (FileStream file = new FileStream(pdffilepath, FileMode.Open, FileAccess.Read)) {
				byte[] bytes = new byte[file.Length];
				file.Read(bytes, 0, (int)file.Length);
				stream.Write(bytes, 0, (int)file.Length);
			}
		}

		/// <summary>generate::generates a _Pdf frrom a url</summary>
		protected override void _GeneratePdf(string url) {
			xLogger.Debug("_GeneratePdf", "url", url);

			wkhtmltopdf(getOptions(String.Empty), url, pdfpathFromurl(url));
		}

		private string pdfpathFromurl(string url) {
			xLogger.Debug("pdfpathFromurl", "::url:", url);

			//string illegal = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
			string pdffile = String.Concat(PdfDir, "\\", url.Replace("http://", ""), ".pdf");

			string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
			pdffile = r.Replace(pdffile, "");
			xLogger.Debug("pdfpathFromurl", "::pdffile:", pdffile);

			return pdffile;
		}

		private string getOptions(string id) {
			string selstr = String.IsNullOrEmpty(id) ? "Options[not(@id)]" : String.Format("Options[@id = '{0}']", id);
			xLogger.Debug("getOptions", "::selstr:", selstr);

			return getOptions(Generator.SelectSingleNode(selstr));
		}

		private string getOptions(XmlNode options) {
			//xLogger.Debug("getOptions");

			string output = String.Empty;
			if (options != null) {
				foreach (XmlNode opt in options.SelectNodes("Option")) {
					output = String.Concat(output, opt.InnerText, " ");
				}
				xLogger.Debug("getOptions", "::output:", output);
			}
			xLogger.Debug("getOptions", "::output:", output);

			return output;
		}

		/// <summary>Execute >wkhtmltopdf</summary>
		/// <example>wkhtmltopdf --print-media-type Lease-264614-2015-10-02.html Lease-264614-2015-10-02.pdf</example>
		/// <param name="Command"></param>
		public void wkhtmltopdf(string options, string input, string outputfile) {
			ProcessStartInfo ProcessInfo;
			Process Process;

			String parameters = String.Format("{0} {1} {2}", options, input, outputfile.Replace(' ', '_'));
			xLogger.Debug("wkhtmltopdf", "::parameters:", parameters);

			ProcessInfo = new ProcessStartInfo(String.Format("{0}wkhtmltopdf.exe", Generator.GetAttribute("path")), parameters);
			ProcessInfo.CreateNoWindow = true;

			// Uncomment if dont want output written to output stream
			//ProcessInfo.UseShellExecute = true;

			// Set output of program to be written to process output stream
			ProcessInfo.UseShellExecute = false;
			ProcessInfo.RedirectStandardOutput = true;

			Process = Process.Start(ProcessInfo);

			// Get program output - see above re UseShellExecute and RedirectStandardOutput
			string strOutput = Process.StandardOutput.ReadToEnd();
			xLogger.Debug("wkhtmltopdf", "::strOutput:", strOutput);

			//Wait for process to finish
			Process.WaitForExit();
		}


	}
}