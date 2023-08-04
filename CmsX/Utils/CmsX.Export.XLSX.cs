/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2015-12-23
	Status:		release	
	Version:	4.0.3
	Build:		20201012
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20151223:	Refactored from CmsXCSV
	20201012:	Added false parameter to generator.XmlToExcel
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {

	using XXBoom.MachinaX.GeneratorX;
	using System;
	using System.Xml;

	/// <summary>
	/// This is a utility class to encapsulte profile and other functionality related to a xxxx.
	/// Typically this class is created within brokers and the display classes
	/// </summary>
	public class CmsXExportXLSX : CmsXExportBase {

		private const string error_no_error		= "Not an error";
		private const string error_xlsx			= "CmsXExportXLSX error::";

		private const string error_xlsx_nodata	= " no data supplied: ";

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public CmsXExportXLSX()
			: base() {
		}
		/// <summary>Constructor for derived classes</summary>
		public CmsXExportXLSX(displayX thispage, x_logger xlogger)
			: base(thispage, xlogger) {
		}
		#endregion

		/// <summary>Converts the dataXml into Excel xls</summary>
		public void XmlToExcel(XmlElement Data, XmlNodeList columns, string exportFileName) {
			string xpath_nodes = ".//child::*";
			// Check for data and throw exception so message can be served in popup window
			if (Data.SelectSingleNode(xpath_nodes) == null) { 
				throw new XException("error_xlsx_nodata", String.Concat(error_xlsx, error_xlsx_nodata, exportFileName));
			}
			GenerateExcel generator = new GenerateExcel();
			generator.XmlToExcel(Data, columns, exportFileName, _UIPage.Response, false);
		}
	}
}
