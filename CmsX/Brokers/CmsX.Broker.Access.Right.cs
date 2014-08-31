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
	class AccessRight : CmsXSubItem {
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
		public AccessRight(x_logger xlogger, int id, Access accessws) : base(xlogger) {
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
			int cat_id = _GetValue("cat_id", 0);
			xLogger.Debug("update", "ID", ID, "cat_id", cat_id.ToString());
			
			bool stat_view		= _GetCheck("stat_view");
			bool stat_edit		= _GetCheck("stat_edit");
			bool stat_subedit	= _GetCheck("stat_subedit");
			xLogger.Debug("update", "stat_view", stat_view.ToString(), "stat_edit", stat_edit.ToString(), "stat_subedit", stat_subedit.ToString());

			if (cat_id != 0) {
				if (stat_edit) {	// true when clicked, 1 when exists
					xLogger.Debug("update", "::adding edit...", cat_id.ToString());
					_Access.AddGroupCategory(GroupID, cat_id, 3);
				} else {
					if (stat_subedit) {	// true when clicked, 1 when exists
						xLogger.Debug("update", "::adding subedit...", cat_id.ToString());
						_Access.AddGroupCategory(GroupID, cat_id, 2);
					} else {
						if (stat_view) {	// true when clicked, 1 when exists
							xLogger.Debug("update", "::adding view...", cat_id.ToString());
							_Access.AddGroupCategory(GroupID, cat_id, 1);
						}
					}
				}
			}
		}
		#endregion
	}
}