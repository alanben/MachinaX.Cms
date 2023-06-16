using System;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2011-10-03
	Status:		release	
	Version:	2.6.0
	Build:		20111017
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20111116:	Started from FoundationX.Admin
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
	/// <summary>
	/// Class to handle add, edit and delete of Yyyy SubItems.
	/// </summary>
	class YyyySubItem : CmsXSubItem {
		#region Invisible properties
		#endregion

		#region Constant name strings
		#endregion

		#region Visible properties
		private int yyyyid;
		/// <summary></summary>
		public int YyyyID {
			get { return yyyyid; }
			set { yyyyid = value; }
		}

		private Yyyys yyyys;
		/// <summary></summary>
		public Yyyys _Yyyys {
			get { return yyyys; }
			set { yyyys = value; }
		}
		#endregion

		#region Constructors/Destructors
		public YyyySubItem(x_logger xlogger, int id, Yyyys yyyyws) : base(xlogger) {
			xLogger.Debug("constructor");
			YyyyID = id;
			_Yyyys = yyyyws;
			xLogger.Debug("constructor:ok");
		}
		#endregion

		#region Public methods
		#endregion

		#region Protected methods
		/// <summary>Add subitem</summary>
		protected override void _Add() {
			xLogger.Debug("_Add");
			update();
			xLogger.Debug("_Add:ok");
		}
		/// <summary>Edit subitem</summary>
		protected override void _Edit() {
			xLogger.Debug("_Edit");
			update();
			xLogger.Debug("_Edit:ok");
		}
		/// <summary>Delete subitem</summary>
		protected override void _Delete() {
			xLogger.Debug("_Delete", "ID", ID);
			string typeid = _GetValue("typeid");
			//_Yyyys.DeleteYyyySubItem(YyyyID, int.Parse(typeid));
			xLogger.Debug("_Delete:ok");
		}
		#endregion

		#region Private utility methods
		private void update() {
			string typeid = _GetValue("typeid");
			string value = _GetValue("value");
			//_Yyyys.UpdateYyyySubItem(YyyyID, int.Parse(typeid), value);
		}
		#endregion
	}
}