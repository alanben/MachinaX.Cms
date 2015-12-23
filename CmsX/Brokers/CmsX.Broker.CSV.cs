/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.3
	Build:		20151223
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100925:	Started from EconoVault
	20140112:	Refactored constructor
	20151223:	Refactored  to use CsvUtil
				Renamed CmsXCSV to CmsXExport 
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	
	using System;

	/// <summary>
	/// The CmsXBrokerCSV class is a broker class to handle processes related to saving and retrieving of CSV's and settings.
	/// <para>Additional information about the class</para>
	/// </summary>
	public class CmsXBrokerCSV : CmsXBrokerBase {
		#region Invisible properties
		//private CmsXExport CsvUtil;
		#endregion

		#region Constant name strings
		private const string logid = "CmsXBrokerCSV.";
		#endregion

		#region Constant error strings
		private const string error_no_error = "Not an error";
		private const string error_csv = "CmsXBrokerCSV error::";
		private const string error_csv_saveset = " Error managing example: ";
		#endregion

		#region Visible properties
		//public new static readonly ILog Logger = LogManager.GetLogger(typeof(CmsXBrokerCSV));
		#endregion

		#region Constructors/Destructors
		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerCSV(CmsX thispage) : base(thispage, typeof(CmsXBrokerCSV), logid) {
			//CsvUtil = new CmsXExport(thispage);
		}
		#endregion

		#region Public methods
		public override void Process(string type) {
			base.Process(true);	// does login checking
			xLogger.Info("_Process:", type);
			switch (type) {
				case "settings_save":	savesettings();	break;
			}
		}
		#endregion

		#region Protected methods
		#endregion

		#region Private methods
		/// <summary>Save CSV settings</summary>
		private void savesettings() {
			xLogger.Info("savesettings:");
			
			try {
				CmsExport.SaveFilters(true);
				xLogger.Debug("savesettings::finished:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_csv_saveset", String.Concat(error_csv_saveset, e.Message)));
			}
		}
		#endregion

		#region Private utility methods
		#endregion
	}
}