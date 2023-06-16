using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web.Services;
using System.Xml;
using System.Xml.Xsl;

using XXBoom.MachinaX;
using Ionic.Zip;
using WebSupergoo.ABCpdf8;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2013-10-30
	Status:		release	
	Version:	4.0.2
	Build:		20131030
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20131030:	Started from LoeriesX.Generator
	-----------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.GeneratorX {
	/// <summary>
	/// Xxxx Generator web service
	/// </summary>
	[WebService(Name = "GenerateXxxx",
				Namespace = "http://www.clickclickBOOM.com/MachinaX/GenerateX",
				Description = "GenerateXxxx Web Service")]
	public class GenerateXxxx : GenerateBase {

		private const string CONFIG_ID = "Xxxx";

		//private XxxxWS.Xxxx service;
		//public XxxxWS.Xxxx Service {
		//	get {
		//		if (service == null) {
		//			service = new XxxxWS.Xxxx();
		//			service.Url = _GetUrl("XxxxWS");
		//		}
		//		return service;
		//	}
		//}

		/// <summary>Constructor</summary>
		public GenerateXxxx() : base("GenerateXxxx", CONFIG_ID) {
		}

		/// <summary>
		/// Generate a zip archive containing the xxxxs pdfs
		/// </summary>
		/// <param name="url">A url accessible to the Generator web service</param>
		[WebMethod(Description = "Generate a zip archive containing the xxxxs pdfs")]
		public XmlDocument GenerateXxxxZipWS(int TypeID, string Sorting, bool Descending, string Prefix, string Template) {
			xLogger.Debug("GenerateXxxxZipWS", "Template", Template);
			try {
				AddOk();

				string zipname = String.Concat(Prefix, "_", DateTime.Now.ToString("yyyyMMddTHH-mm-ss"));
				//generateXxxxZip(zipname, _CheckWSResult(Service.ListXxxxs(TypeID, Sorting, Descending)), Template);

				xLogger.Debug("GenerateXxxxZipWS:ok");
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}
        
		/// <summary>
		/// Generate a zip archive containing the xxxxs pdfs
        /// </summary>
        /// <param name="url">A url accessible to the Generator web service</param>
        [WebMethod(Description="Generate a zip archive containing the xxxxs pdfs")]
		public XmlDocument GenerateXxxxZip(string XxxxXml, string Template) {
			xLogger.Debug("GenerateXxxxZip", "XxxxXml", XxxxXml);
			try {
				XmlDocument xxxxdoc = new XmlDocument();
				xxxxdoc.LoadXml(XxxxXml);
				AddOk();

				Guid tok = Guid.NewGuid();
				generateXxxxZip(tok.ToString(), xxxxdoc, Template);

				xLogger.Debug("GenerateXxxxZip:ok");
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
            return (Result);
        }

		private void generateXxxxZip(string zipname, XmlNode xxxxxml, string template) {
			string htmldir = createXxxxHTML(xxxxxml, template);
			xLogger.Debug("GenerateXxxxZip", "htmldir", htmldir);

			string pdfsdir = createXxxxPDFs(zipname, xxxxxml, htmldir);
			xLogger.Debug("GenerateXxxxZip", "pdfsdir", pdfsdir);

			string zipfile = createXxxxZip(pdfsdir);
			xLogger.Debug("GenerateXxxxZip", "zipfile", zipfile);

			string htmdir = String.Concat(HtmlDir, "\\", htmldir);
			string zipdir = String.Concat(PdfDir, "\\", pdfsdir);
			//Directory.Delete(htmdir, true);
			Directory.Delete(zipdir, true);

			AddNode("filepath", zipfile);
		}

		private string createXxxxHTML(XmlNode xxxxdoc, string template) {
			Guid tok = Guid.NewGuid();
			string subdir = tok.ToString();
			string htmlout = String.Concat(HtmlDir, "\\", subdir);
			Directory.CreateDirectory(htmlout);

			XslCompiledTransform templ = getTemplate(template);

			foreach (XmlNode entry in xxxxdoc.SelectNodes("/items/item")) {
				XmlElement entryel = entry as XmlElement;


				writeXxxxHTML(htmlout, entryel, templ, null);
				//Thread.Sleep(500);
			}

			return subdir;
		}

		private string createXxxxPDFs(string subdir, XmlNode xxxxdoc, string htmldir) {
			string pdfsout = String.Concat(PdfDir, "\\", subdir);
			Directory.CreateDirectory(pdfsout);

			foreach (XmlNode entryitem in xxxxdoc.SelectNodes("/items/item")) {
				XmlElement entryel = entryitem as XmlElement;
				string entry = entryel.GetAttribute("display");
				string prize = entryel.SelectSingleNode("prize").InnerText.ToUpper();

				string pdf = String.Concat(pdfsout, "\\", prize, "_", entry, ".pdf");
				string url = String.Concat(BaseUrl, "/", Html, "/", htmldir, "/", prize, "_", entry, ".html");
				// create pdf...
				_Generate(url, pdf);
			}

			return subdir;
		}

		// Create a zip archive from the folder containg the pdfs and then delete the folder
		private string createXxxxZip(string pdfsout) {
			string zipdir = String.Concat(PdfDir, "\\", pdfsout);
			string zipfile = String.Concat(pdfsout, ".zip");
			string zippath = String.Concat(PdfDir, "\\", zipfile);

			using (ZipFile zip = new ZipFile()) {
				//zip.UseUnicode = true;  // utf-8
				zip.AddDirectory(zipdir);
				zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
				zip.Save(zippath);
			}

			return zippath;
		}


		/// <summary>
		/// Loads a xsl template for special purposes from the templates directory
		/// </summary>
		protected XslCompiledTransform getTemplate(string thisfile) {
			XslCompiledTransform templ = new XslCompiledTransform();
			string tmplpath = String.Concat(TemplateDir, "/", thisfile, ".xsl");
			templ.Load(tmplpath);
			return (templ);
		}

		private void writeXxxxHTML(string htmldir, XmlElement entryEl, XslCompiledTransform xslt, XsltArgumentList args) {

			string entry = entryEl.GetAttribute("display");
			string prize = entryEl.SelectSingleNode("prize").InnerText.ToUpper();

			string filepath = String.Concat(htmldir, "/", prize, "_", entry, ".html");
			using (StreamWriter writer = new StreamWriter(filepath)) {
				xslt.Transform(entryEl, args, writer);
			}
		}

	}
}
