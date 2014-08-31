/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	2.5.0
	Build:		20100925
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Refactored from LoeriesAdmin
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
    /// <summary>
    /// This is a utility class to encapsulte profile and other functionality related to a xxxx.
    /// Typically this class is created within brokers and the display classes
    /// </summary>
    public class CmsXClass {
        #region Invisible properties
		x_userprofile profile;
		#endregion

        #region Private constants
        #endregion

        #region Public constants
        #endregion

        #region Visible properties
        #endregion

        #region Constructors/Destructors
        /// <summary>Default constructor</summary>
        public CmsXClass() {
        }
        /// <summary>Constructor for derived classes</summary>
        public CmsXClass(x_userprofile uprofile){
            profile = uprofile;
        }
        #endregion

        #region Public methods
        #endregion

        #region Protected methods
        #endregion

        #region Private methods
        #endregion
    }
}
