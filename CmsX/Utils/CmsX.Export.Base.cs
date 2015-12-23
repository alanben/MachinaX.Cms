/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2015-12-23
	Status:		release	
	Version:	4.0.3
	Build:		20151223
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20151223:	Refactored from CmsXCSV
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {

	/// <summary>
    /// Base class for utility conversion classes
    /// </summary>
    public class CmsXExportBase {

		protected x_logger xLogger;

		/// <summary>The UIPage of the processing broker</summary>
		private displayX uipage;
		protected displayX _UIPage {
			get { return uipage; }
			set { uipage = value; }
		}

        #region Constructors/Destructors
        /// <summary>Default constructor</summary>
        public CmsXExportBase() {
        }
        /// <summary>Constructor for derived classes</summary>
		public CmsXExportBase(displayX thispage, x_logger xlogger) {
			_UIPage = thispage;
			xLogger = xlogger;
        }
        #endregion
    }
}
