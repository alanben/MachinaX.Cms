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

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>
	/// Class to handle add, edit and delete of Yyyy SubItems.
	/// </summary>
	class AccessGroup : CmsXSubItem {
		#region Invisible properties
		#endregion

		#region Constant name strings
		#endregion

		#region Visible properties
		private int groupid;
		/// <summary></summary>
		public int GroupID {
			get { return groupid; }
			set { groupid = value; }
		}

		private Access access;
		/// <summary></summary>
		public Access _Access {
			get { return access; }
			set { access = value; }
		}
		#endregion

		#region Constructors/Destructors
		public AccessGroup(x_logger xlogger, int id, Access accessws) : base(xlogger) {
			xLogger.Debug("constructor");
			GroupID = id;
			_Access = accessws;
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
			int user_id = _GetValue("user_id", 0);
			if (user_id != 0) {
				//_Access.DeleteGroupUser(GroupID, user_id);
			}
			xLogger.Debug("_Delete:ok");
		}
		#endregion

		#region Private utility methods
		private void update() {
			int user_id = _GetValue("user_id", 0);
			string stat_id = _GetValue("stat_id");
			if (user_id != 0 && (stat_id == "true" || stat_id == "1")) {	// true when clicked, 1 when exists
				xLogger.Debug("update", "::adding...", user_id.ToString());
				_Access.AddGroupUser(GroupID, user_id);
			}

		}
		#endregion
	}
}