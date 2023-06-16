using System;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2014-01-20
	Status:		release	
	Version:	4.0.2
	Build:		20140120
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20140120:	Started
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX.datatable {
    /// <summary>
    /// This is the base class to handle data for jquery.dataTables plugin
    /// </summary>
    public class Base {

		protected int ID { get; set; }
		protected x_userprofile Profile { get; set; }
		protected x_logger xLogger { get; set; }

        /// <summary>Constructor</summary>
		public Base(x_userprofile profile) {
			initialise(profile, 0);
		}
		public Base(x_userprofile profile, int id) {
			initialise(profile, id);
		}
		private void initialise(x_userprofile profile, int id) {
			xLogger = new x_logger(typeof(Base), ".datatable", false, true);
			Profile = profile;
			ID = id;
		}

		protected string _GetParam(string name) {
			return Profile.Value((ID == 0)? name : String.Format("{0}_{1}", name, ID.ToString()));
		}
		protected Boolean _GetParam(string name, bool defval) {
			return Profile.Value((ID == 0) ? name : String.Format("{0}_{1}", name, ID.ToString()), defval);
		}
		protected int _GetParam(string name, int defval) {
			return Profile.Value((ID == 0) ? name : String.Format("{0}_{1}", name, ID.ToString()), defval);
		}

		protected void _SetParam(string name, string value) {
			Profile.Add((ID == 0) ? name : String.Format("{0}_{1}", name, ID.ToString()), value);
		}
		protected void _SetParam(string name, Boolean value) {
			Profile.Add((ID == 0) ? name : String.Format("{0}_{1}", name, ID.ToString()), value.ToString());
		}
		protected void _SetParam(string name, int value) {
			Profile.Add((ID == 0) ? name : String.Format("{0}_{1}", name, ID.ToString()), value.ToString());
		}
	}
}
