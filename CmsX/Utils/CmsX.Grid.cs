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
	20100701:	Refactored from LoeriesAdmin
	20151223:	Renamed CmsXCsv to CmsXExport
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {

	using System;
	using System.Xml;

	/// <summary>This is a small utility class that represents the page grid
	/// <para>Additional information about the class...</para>
	/// </summary>
	public class CmsXGrid {
		#region Constants
		private const string ATTR_PAGES = "pages";
		private const string ATTR_PAGE = "page";
		private const string ATTR_DATAID = "data_id";
		private const string ATTR_FOCUSROW = "focus_row";
		#endregion

		#region Properties
		protected displayX _UIPage;
		
		private XmlElement grid;
		public XmlElement Grid {
			get { return grid; }
		}

		private XmlElement config;
		public XmlElement Config {
			get { return config; }
		}

		private bool descending = true;
		public bool Descending {
			set { descending = value; }
		}

		public int RowsPerPage {
			get { return Int32.Parse(config.GetAttribute(ATTR_PAGES)); }
			set { config.SetAttribute(ATTR_PAGES, value.ToString()); }
		}
		public int Page {
			set { config.SetAttribute(ATTR_PAGE, value.ToString()); }
		}
		public string DataID {
			set { config.SetAttribute(ATTR_DATAID, value); }
		}
		public int FocusRow {
			set { config.SetAttribute(ATTR_FOCUSROW, value.ToString()); }
		}
		
		#endregion

		#region Constructors/Destructors
		public CmsXGrid() {
		}
		public CmsXGrid(displayX thispage) {
			_UIPage = thispage;
			initialise();
		}
		/// <summary>
		/// This must be utilised post process
		/// </summary>
		/// <param name="thispage"></param>
		/// <param name="isDescending"></param>
		public CmsXGrid(displayX thispage, bool isDescending) {
			_UIPage = thispage;
			initialise(isDescending);
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Set the Grid to display a certain element as current
		/// </summary>
		/// <param name="ID">The data identifier of the element</param>
		/// <param name="TotalRows">The total number of rows in the dataset</param>
		/// <param name="Sequence">The sequence of the data item in the dataset</param>
		public void SetFocus(string ID, int TotalRows, int Sequence) {
			DataID = ID;
			SetFocus(TotalRows, Sequence);
		}
		public void SetFocus(int TotalRows, int Sequence) {
			int row = (descending) ? TotalRows - Sequence : Sequence-1;
			Page = pageRowBelongsTo(row);
			FocusRow = row % RowsPerPage;
		}
		#endregion

		#region Private methods
		private void initialise() {
			initialise(true);
		}
		private void initialise(bool isDescending) {
			grid = _UIPage.Content.SelectSingleNode("page/grid") as XmlElement;
			config = grid.SelectSingleNode("config") as XmlElement;
			descending = isDescending;
		}
		/// <summary>response text stream from a file</summary>
		private int pageRowBelongsTo(int rowOrderNumber) {
			int page = 0;
			double ratio = Convert.ToDouble(rowOrderNumber) / Convert.ToDouble(RowsPerPage);
			int ceil = (int)Math.Ceiling(ratio);
			int floor = (int)Math.Floor(ratio);
			if (ceil == floor)
				page = floor;
			else
				page = ceil - 1;
			return page;
		}
		#endregion
	}
}