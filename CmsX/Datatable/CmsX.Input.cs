using System;
using System.Collections.Generic;
using System.Xml;

using Newtonsoft.Json;

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
    /// This is a utility class that handles the input parameters for jquery.dataTables plugin
    /// </summary>
    public class Input : Base {

		// The following information is sent to the server for each draw request. Your server-side script must use this information to obtain the data required for the draw.

		public int	iDisplayStart	{ get { return _GetParam("iDisplayStart", 0); }		set { _SetParam("iDisplayStart", value); } }	// Display start point in the current data set.
		public int	iDisplayLength	{ get { return _GetParam("iDisplayLength", 0); }	set { _SetParam("iDisplayLength", value); } }	// Number of records that the table can display in the current draw. It is expected that the number of records returned will be equal to this number, unless the server has fewer records to return.
		public int	iColumns		{ get { return _GetParam("iColumns", 0); }			set { _SetParam("iColumns", value); } }			// Number of columns being displayed (useful for getting individual column search info)
		public string	sSearch		{ get { return _GetParam("sSearch"); }				set { _SetParam("sSearch", value); } }			// Global search field
		public Boolean	bRegex		{ get { return _GetParam("bRegex", true); }			set { _SetParam("bRegex", value); } }			// True if the global filter should be treated as a regular expression for advanced filtering, false if not.
		public int	iSortingCols	{ get { return _GetParam("iSortingCols", 0); }		set { _SetParam("iSortingCols", value); } }		// Number of columns to sort on
		public string	sEcho		{ get { return _GetParam("sEcho"); }				set { _SetParam("sEcho", value); } }			// Information for DataTables to use for rendering.

		// Initially assume that the single column sort is the primary condition - then use this (rather than Sorts - for multiple column sorting)
		public int iSortCol		{ get { return _GetParam("iSortCol", 0); }	set { _SetParam("iSortCol", value); } }		// Column being sorted on (you will need to decode this number for your database)
		public string sSortDir	{ get { return _GetParam("sSortDir"); }		set { _SetParam("sSortDir", value); } }		// Direction to be sorted - "desc" or "asc".
		public string sSortCol	{ get { return _GetParam("iSortCol"); }		set { _SetParam("iSortCol", value); } }		// Column being sorted on (you will need to decode this number for your database)

		public List<Column> Columns { get; set; }
		public List<Sort> Sorts { get; set; }

        #region Constructors/Destructors
        /// <summary>Default constructor</summary>
		public Input(x_userprofile profile) : base(profile) {
			GetColumns();
			GetSorts();
        }
        #endregion

		public void GetColumns() {
			List<Column> Columns = new List<Column>();
			for (int i = 1; i <= iColumns; i++) {
				Columns.Add(new Column(Profile, i));
			}
		}

		public void GetSorts() {
			List<Sort> Sorts = new List<Sort>();
			for (int i = 1; i <= iSortingCols; i++) {
				Sorts.Add(new Sort(Profile, i));
			}
		}
	}

	public class Column : Base {
		public Boolean	bSearchable { get { return _GetParam("bSearchable", true); }	set { _SetParam("bSearchable", value); } }	// Indicator for if a column is flagged as searchable or not on the client-side
		public Boolean	bSortable	{ get { return _GetParam("bSortable", true); }		set { _SetParam("bSortable", value); } }	// Indicator for if a column is flagged as sortable or not on the client-side
		public Boolean	bRegex		{ get { return _GetParam("bRegex", true); }			set { _SetParam("bRegex", value); } }		// True if the individual column filter should be treated as a regular expression for advanced filtering, false if not
		public string	sSearch		{ get { return _GetParam("sSearch"); }				set { _SetParam("sSearch", value); } }		// Individual column filter

		public string	mDataProp	{ get { return _GetParam("mDataProp"); }			set { _SetParam("mDataProp", value); } }	// The value specified by mDataProp for each column. This can be useful for ensuring that the processing of data is independent from the order of the columns.

		public Column(x_userprofile profile, int id) 
			: base(profile) {
        }
    }

	public class Sort : Base {
		public int iSortCol		{ get { return _GetParam("iSortCol", 0); }	set { _SetParam("iSortCol", value); } }		// Column being sorted on (you will need to decode this number for your database)
		public string sSortDir	{ get { return _GetParam("sSortDir"); }		set { _SetParam("sSortDir", value); } }		// Direction to be sorted - "desc" or "asc".
		
		public Sort(x_userprofile profile, int id)
			: base(profile) {
        }
	}

}
