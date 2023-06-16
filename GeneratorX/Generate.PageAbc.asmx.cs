using System;
/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2013-10-30
	Status:		release	
	Version:	4.0.3
	Build:		20151014
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20131030:	Started from LoeriesX.Generator
	20151014:	Refactor to contain ABCpdf specific code, ie
				- Web service methods
				- _GeneratePdf overrides (specific to ABCpdf)
	-----------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.GeneratorX {

	using XXBoom.MachinaX;
	using System.IO;
	using System.Web.Services;
	using System.Xml;
	using WebSupergoo.ABCpdf8;
	
	/// <summary>
	/// Credit Generator web service
	/// </summary>
	[WebService(Name = "GeneratePageAbc",
				Namespace = "http://www.clickclickBOOM.com/MachinaX/GenerateX",
				Description = "GeneratePageAbc Web Service")]
	public class GeneratePageAbc : GenerateBase {

		private const string CONFIG_ID = "ABCpdf";

		private Doc pdf;
		/// <summary>Generator _Pdf document object</summary>
		/// <value>ABCpdf object</value>
		protected Doc _Pdf {
			get {
				if (pdf == null) {
					//try {
					pdf = new Doc();
					pdf.SetInfo(0, "License", "bc8b5c07da69df2b0a9039b1aae0a68bef283c3c54b61a799a7bea3cbc84232cfe8f03eb3130842a");
					//} catch (Exception e) {
					//	xLogger.Debug("initialise", "Error", e.Message);
					//}
				}
				return pdf;
			}
		}

		/// <summary>Constructor</summary>
		public GeneratePageAbc() : base("GeneratePage", CONFIG_ID) {
		}

		/// <summary>
		/// Test
		/// </summary>
		[WebMethod] 
		public XmlDocument License() {
			try {
				AddOk();
				AddNode("license", _Pdf.License);
			} 
			catch(x_exception e)		{_AddError(e);}
			catch(System.Exception e) 	{_AddError(e);}
			return(Result);
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
			} catch (x_exception e) {
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
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}


		/// <summary>generate::generates a _Pdf frrom a url</summary>
		protected override void _GeneratePdf(string[] urls, string filepath) {
			xLogger.Debug("_GeneratePdf", "filepath", filepath);

			_Pdf.Rect.Inset(0, 0);
			foreach (string url in urls)
				_GeneratePdf(url);
			_Pdf.Save(filepath);
			_Pdf.Clear();
		}

		/// <summary>generate::generates a _Pdf frrom a url</summary>
		protected override void _GeneratePdf(string url, string filepath, XmlNode options) {
			xLogger.Debug("_GeneratePdf", "url", url, "filepath", filepath);

			_Pdf.Rect.Inset(0, 0);
			_GeneratePdf(url);
			_Pdf.Save(filepath);
			_Pdf.Clear();
		}

		/// <summary>generate::generates a _Pdf frrom a url</summary>
		protected override void _GeneratePdf(string url, Stream stream) {
			xLogger.Debug("_GeneratePdf", "url", url);

			_Pdf.Rect.Inset(0, 0);
			_GeneratePdf(url);
			_Pdf.Save(stream);
			_Pdf.Clear();
		}

		/// <summary>generate::generates a _Pdf frrom a url</summary>
		protected override void _GeneratePdf(string url) {
			xLogger.Debug("_GeneratePdf", "url", url);

			if (!String.IsNullOrEmpty(PageSize)) {
				xLogger.Debug("Generate", "PageSize", PageSize);
				//_Pdf.SetInfo(_Pdf.Page, "/MediaBox:Rect", PageSize);
				_Pdf.HtmlOptions.FontEmbed = true;
				_Pdf.HtmlOptions.FontSubstitute = false;
				_Pdf.HtmlOptions.FontProtection = false;
				_Pdf.HtmlOptions.Engine = EngineType.Gecko;
				_Pdf.MediaBox.String = PageSize;
				_Pdf.Rect.String = _Pdf.MediaBox.String;

				_Pdf.Font = _Pdf.AddFont(Font);
				_Pdf.FontSize = Int32.Parse(FontSize);
			}
			_Pdf.Page = _Pdf.AddPage();
			int thisid = _Pdf.AddImageUrl(url);
			while (true) {
				_Pdf.FrameRect(); // add a black border
				if (!_Pdf.Chainable(thisid))
					break;
				_Pdf.Page = _Pdf.AddPage();
				thisid = _Pdf.AddImageToChain(thisid);
			}
			for (int i = 1; i <= _Pdf.PageCount; i++) {
				_Pdf.PageNumber = i;
				_Pdf.Flatten();
			}
		}

		/// <summary>DEPREACTED: Generate from html file</summary>
		public string Generate() {
			xLogger.Debug("Generate");

			string outputFile = String.Concat(PdfDir, "\\", _GetToken(), ".pdf");
			xLogger.Debug("Generate", "outputFile", outputFile);

			_Pdf.Font = _Pdf.AddFont("Helvetica");
			_Pdf.FontSize = 36;
			_Pdf.Page = _Pdf.AddPage();
			_Pdf.Pos.X = 40;
			_Pdf.Pos.Y = _Pdf.MediaBox.Top - 40;
			_Pdf.AddText("Multimedia features");
			_Pdf.FontSize = 24;

			/*	as per example, but has problems...
			_Pdf.Pos.String = "40 690";
			_Pdf.AddText("Flash movie:");
			Annotation movie1 = new Annotation(_Pdf, "80 420 520 650", Server.MapPath("Untitled-1.swf"));
			
			//WMV is courtesy of NASA - http://www.nasa.gov/wmv/30873main_cardiovascular_300.wmv
			_Pdf.Pos.String = "40 400";
			_Pdf.AddText("Video File:");
			Annotation movie2 = new Annotation(_Pdf, "80 40 520 360", Server.MapPath("video.wmv"));
			*/

			_Pdf.Save(outputFile);
			_Pdf.Clear();
			return outputFile;
		}



	}
}