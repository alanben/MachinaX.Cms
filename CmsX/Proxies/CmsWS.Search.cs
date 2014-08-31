using System;

using log4net;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	2.5.0
	Build:		20100701
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100701:	Refactored from LoeriesAdmin
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>This is a small utility class for the Search Parameters
	/// <para>Additional information about the class</para>
	/// </summary>
	public class SearchItemDrop : SearchItem {
		#region Invisible properties
		#endregion

		#region Constants
		private const string TEST_NONE = "0";
		private const int DEFAULT_NONE = 0;
		#endregion

		#region Constructors/Destructors
		/// <summary>Constructor</summary>
		/// <param name="RootName">The root (the part that changes) of the user profile name</param>
		/// <param name="UserProfile">The user profile</param>
		public SearchItemDrop(string RootName, x_userprofile UserProfile) : base(RootName, UserProfile) {
			testNone();
		}
		/// <summary>Constructor</summary>
		/// <param name="RootName">The root (the part that changes) of the user profile name</param>
		/// <param name="UserProfile">The user profile</param>
		/// <param name="Logger">A reference to a Logger</param>
		public SearchItemDrop(string RootName, x_userprofile UserProfile, ILog Logger) : base(RootName, UserProfile) {
			testNone();
			Logger.Debug(string.Concat(":", RootName, ":", Type.ToString(), ":", Val));
		}
		/// <summary>Constructor</summary>
		/// <param name="SearchPrefix">The search prefix to be appended to the root</param>
		/// <param name="RootName">The root (the part that changes) of the user profile name</param>
		/// <param name="UserProfile">The user profile</param>
		public SearchItemDrop(string SearchPrefix, string RootName, x_userprofile UserProfile) : base(SearchPrefix, RootName, UserProfile) {
			testNone();
		}
		/// <summary>Constructor</summary>
		/// <param name="SearchPrefix">The search prefix to be appended to the root</param>
		/// <param name="RootName">The root (the part that changes) of the user profile name</param>
		/// <param name="UserProfile">The user profile</param>
		/// <param name="Logger">A reference to a Logger</param>
		public SearchItemDrop(string SearchPrefix, string RootName, x_userprofile UserProfile, ILog Logger) : base(SearchPrefix, RootName, UserProfile) {
			testNone();
			Logger.Debug(string.Concat(":", RootName, ":", Type.ToString(), ":", Val));
		}
		/// <summary>Constructor</summary>
		/// <param name="SearchPrefix">The search prefix to be appended to the root</param>
		/// <param name="RootName">The root (the part that changes) of the user profile name</param>
		/// <param name="UserProfile">The user profile</param>
		/// <param name="Logger">A reference to a Logger</param>
		public SearchItemDrop(string SearchPrefix, string RootName, x_userprofile UserProfile, x_logger Logger) : base(SearchPrefix, RootName, UserProfile) {
			testNone();
			Logger.Debug(":", RootName, ":", Type.ToString(), ":", Val);
		}
		
		private void testNone() {
			if (Val == TEST_NONE)
				Type = DEFAULT_NONE;
		}
		#endregion
	}
	
	/// <summary>This is a small utility class for the Search Parameters
	/// <para>Additional information about the class</para>
	/// </summary>
	public class SearchItem {
		#region Invisible properties
		#endregion

		#region Constants
		private const string SUFFIX_SEARCH = "_SEARCH";
		private const string SUFFIX_TYPE = "_Type";
		private const string DEFAULT_TYPE = "0";
		private const int DEFAULT_TYPEI = 0;
		private const int MAX_TYPE_CHECK = 100;	// Check up to this type for empty value
		#endregion

		#region Visible properties
		/// <summary>The value to be searched</summary>
		private string val;
		public string Val {
			get { return val; }
			set { val = value; }
		}
		/// <summary>The type of seach</summary>
		private string type;
		public int Type {
			get {
				int typ = 0;
				try { typ = Convert.ToInt32(type); } catch { }
				return typ;
			}
			set { type = value.ToString(); }
		}
		#endregion

		#region Constructors/Destructors
		/// <summary>Constructor</summary>
		/// <param name="RootName">The root (the part that changes) of the user profile name</param>
		/// <param name="UserProfile">The user profile</param>
		public SearchItem(string RootName, x_userprofile UserProfile) {
			initialise(RootName, UserProfile);
		}
		/// <summary>Constructor</summary>
		/// <param name="RootName">The root (the part that changes) of the user profile name</param>
		/// <param name="UserProfile">The user profile</param>
		/// <param name="Logger">A reference to a Logger</param>
		public SearchItem(string RootName, x_userprofile UserProfile, ILog Logger) {
			initialise(RootName, UserProfile);
			Logger.Debug(string.Concat(":", RootName, ":", Type.ToString(), ":", Val));
		}
		/// <summary>Constructor</summary>
		/// <param name="SearchPrefix">The search prefix to be appended to the root</param>
		/// <param name="RootName">The root (the part that changes) of the user profile name</param>
		/// <param name="UserProfile">The user profile</param>
		/// <param name="Logger">A reference to a Logger</param>
		public SearchItem(string SearchPrefix, string RootName, x_userprofile UserProfile) {
			string rootName = String.Concat(SearchPrefix, RootName, SUFFIX_SEARCH);
			initialise(rootName, UserProfile);
		}
		/// <summary>Constructor</summary>
		/// <param name="SearchPrefix">The search prefix to be appended to the root</param>
		/// <param name="RootName">The root (the part that changes) of the user profile name</param>
		/// <param name="UserProfile">The user profile</param>
		/// <param name="Logger">A reference to a Logger</param>
		public SearchItem(string SearchPrefix, string RootName, x_userprofile UserProfile, ILog Logger) {
			string rootName = String.Concat(SearchPrefix, RootName, SUFFIX_SEARCH);
			initialise(rootName, UserProfile);
			Logger.Debug(string.Concat(":", rootName, ":", Type.ToString(), ":", Val));
		}
		/// <summary>Constructor</summary>
		/// <param name="SearchPrefix">The search prefix to be appended to the root</param>
		/// <param name="RootName">The root (the part that changes) of the user profile name</param>
		/// <param name="UserProfile">The user profile</param>
		/// <param name="Logger">A reference to a Logger</param>
		public SearchItem(string SearchPrefix, string RootName, x_userprofile UserProfile, x_logger Logger) {
			string rootName = String.Concat(SearchPrefix, RootName, SUFFIX_SEARCH);
			initialise(rootName, UserProfile);
			Logger.Debug(":", rootName, ":", Type.ToString(), ":", Val);
		}
		#endregion

		#region Public methods
		#endregion

		#region Protected methods
		#endregion

		#region Private methods
		/// <summary>Initialise the class properties</summary>
		private void initialise(string rootName, x_userprofile userProfile) {
			string typeName = String.Concat(rootName, SUFFIX_TYPE);
			val = userProfile.Value(rootName, "");
			string typ = userProfile.Value(typeName, DEFAULT_TYPE);
			type = validType(val, typ);
			userProfile.Add(typeName, type);
		}
		/// <summary></summary>
		private string validType(string thisvalue, string thistype) {
			int typ = DEFAULT_TYPEI;
			try {
				typ = Convert.ToInt32(thistype);
				if (typ <= MAX_TYPE_CHECK && String.IsNullOrEmpty(thisvalue))
					typ = DEFAULT_TYPEI;
			} catch {
			}
			return typ.ToString();
		}
		#endregion
	}

	/// <summary>This is a utility class for the Search Parameters</summary>
	public class CmsSearch: SearchSettings {

		/// <summary>Determines how the list data is output</summary>
		private ListOutputStyle listStyle { get; set; }
		private datatable.Input input { get; set; }

		public override bool Descending {
			get { return (listStyle == ListOutputStyle.Datatable) ? input.sSortDir == "desc" : base.Descending; }	// Assume single column sort
			set { base.Descending = value; }
		}

		public override string Column {
			get { return (listStyle == ListOutputStyle.Datatable) ? input.sSortCol : base.Column; }	// Assume single column sort
			set { base.Column = value; }
		}

		/// <summary>Constructors</summary>
		public CmsSearch(CmsXProfileX thispage, x_logger xlogger, bool isMaxRows) : base(thispage.UserProfile, xlogger, isMaxRows) {
			listStyle = thispage.ListStyle;
			if (listStyle == ListOutputStyle.Datatable) {
				input = new datatable.Input(thispage.UserProfile);
			}
		}
	}

	/// <summary>This is a utility class for the Search Parameters</summary>
	public class SearchSettings {
		private const string SORT_DESC = "DESC";
		private const string SORT_ASC = "ASC";
		private const string PROFILE_DIRECTION = "dir";
		private const string PROFILE_SORTCOL = "sort";
		private const string PROFILE_LIMIT = "limit";
		private const string PROFILE_PAGE = "start";
		private const string DEFAULT_SORTCOL = "";
		private const string DEFAULT_LIMIT = "1";
		private const string DEFAULT_PAGE = "0";
		private const int MAX_ROWS = 60000;

		private string def_desc = SORT_DESC;
		/// <summary>
		/// 
		/// </summary>
		public virtual bool Descending {
			get { return (def_sort) ? !sort_desc : sort_desc; }
			set { sort_desc = value; }
		}
		private bool sort_number = false;
		/// <summary>
		/// Flag to indicate number sort (default is false)
		/// </summary>
		public bool Number {
			get { return sort_number; }
			set { sort_number = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		public string Direction {
			get { return (sort_desc) ? SORT_DESC : SORT_ASC; }
		}
		/// <summary>
		/// 
		/// </summary>
		private bool sort_desc;
		public bool SortDesc {
			get { return sort_desc; }
			set { sort_desc = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		private bool def_sort;
		public bool SortDef {
			get { return def_sort; }
			set { def_sort = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		private string sort_col;
		public virtual string Column {
			get { return sort_col; }
			set { sort_col = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		private int limit;
		public int Rows {
			get { return limit; }
			set { limit = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		private int page;
		public int Page {
			get { return page; }
			set { page = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		public bool MaxRows {
			set { limit = (value) ? MAX_ROWS : limit; }
		}

		/// <summary>Constructor</summary>
		public SearchSettings(x_userprofile UserProfile) {
			initialise(UserProfile, null as ILog);
		}

		public SearchSettings(x_userprofile UserProfile, ILog Logger) {
			initialise(UserProfile, Logger);
			Logger.Debug(String.Concat(":SortDesc:", sort_desc.ToString(), ":SortCol:", sort_col, ":Page:", page));
		}
		/// <summary>Constructor</summary>
		public SearchSettings(x_userprofile UserProfile, ILog Logger, bool IsMaxRows) {
			initialise(UserProfile, Logger);
			MaxRows = IsMaxRows;
			Logger.Debug(String.Concat(":SortDesc:", sort_desc.ToString(), ":SortCol:", sort_col, ":Page:", page));
		}
		/// <summary>Constructor</summary>
		public SearchSettings(x_userprofile UserProfile, x_logger Logger, bool IsMaxRows) {
			initialise(UserProfile, Logger);
			MaxRows = IsMaxRows;
			Logger.Debug(String.Concat(":SortDesc:", sort_desc.ToString(), ":SortCol:", sort_col, ":Page:", page));
		}
		/// <summary>Constructor</summary>
		public SearchSettings(x_userprofile UserProfile, ILog Logger, bool IsMaxRows, string DefaultSortCol) {
			initialise(UserProfile, DefaultSortCol, Logger);
			MaxRows = IsMaxRows;
			Logger.Debug(String.Concat(":SortDesc:", sort_desc.ToString(), ":SortCol:", sort_col, ":Page:", page));
		}
		/// <summary>Constructor</summary>
		public SearchSettings(x_userprofile UserProfile, ILog Logger, string DefaultSortCol) {
			initialise(UserProfile, DefaultSortCol, Logger);
		}
		/// <summary>Constructor</summary>
		public SearchSettings(x_userprofile UserProfile, ILog Logger, string DefaultSortCol, bool IsDescending) {
			def_desc = (IsDescending) ? SORT_DESC : SORT_ASC;
			initialise(UserProfile, DefaultSortCol, Logger);
			//sort_desc = IsDescending;
		}
		/// <summary>Constructor</summary>
		public SearchSettings(x_userprofile UserProfile, x_logger Logger, string DefaultSortCol, bool IsDescending) {
			def_desc = (IsDescending) ? SORT_DESC : SORT_ASC;
			initialise(UserProfile, DefaultSortCol, Logger);
			//sort_desc = IsDescending;
		}
		/// <summary>Constructor</summary>
		public SearchSettings(x_userprofile UserProfile, ILog Logger, int DefaultPage, string DefaultSortCol, bool IsDescending) {
			initialise(UserProfile, DefaultPage, DefaultSortCol, IsDescending);
		}
		/// <summary>Constructor</summary>
		public SearchSettings(x_userprofile UserProfile, ILog Logger, int DefaultPage, string DefaultSortCol, bool IsDescending, bool IsMaxRows) {
			initialise(UserProfile, DefaultPage, DefaultSortCol, IsDescending);
			MaxRows = IsMaxRows;
			Logger.Debug(String.Concat(":SortDesc:", sort_desc.ToString(), ":SortCol:", sort_col, ":Page:", page));
		}

		private void initialise(x_userprofile userProfile, ILog logger) {
			initialise(userProfile, DEFAULT_SORTCOL, logger);
		}
		private void initialise(x_userprofile userProfile, x_logger logger) {
			initialise(userProfile, DEFAULT_SORTCOL, logger);
		}
		private void initialise(x_userprofile userProfile, string defaultSortCol, ILog logger) {
			if (logger != null) { 
				logger.Debug(String.Concat(":initialise:defaultSortCol:", defaultSortCol));
			}
			sort_desc = (userProfile.Value(PROFILE_DIRECTION, SORT_ASC) == def_desc) ? true : false;
			sort_col = userProfile.Value(PROFILE_SORTCOL, "");
			def_sort = (sort_col == "") ? false : true;
			sort_col = userProfile.Value(PROFILE_SORTCOL, defaultSortCol);
			limit = Int32.Parse(userProfile.Value(PROFILE_LIMIT, DEFAULT_LIMIT));
			limit = (limit == 0) ? Int32.Parse(DEFAULT_LIMIT) : limit;
			if (logger != null)
				logger.Debug(String.Concat(":initialise:limit:", limit.ToString()));
			page = (Int32.Parse(userProfile.Value(PROFILE_PAGE, DEFAULT_PAGE)) / limit) + 1;
			if (logger != null) { 
				logger.Debug(String.Concat(":initialise:page:", page.ToString()));
			}
		}
		private void initialise(x_userprofile userProfile, string defaultSortCol, x_logger logger) {
			if (logger != null) { 
				logger.Debug(String.Concat(":initialise:defaultSortCol:", defaultSortCol));
			}
			sort_desc = (userProfile.Value(PROFILE_DIRECTION, SORT_ASC) == def_desc) ? true : false;
			sort_col = userProfile.Value(PROFILE_SORTCOL, "");
			def_sort = (sort_col == "") ? false : true;
			sort_col = userProfile.Value(PROFILE_SORTCOL, defaultSortCol);
			limit = Int32.Parse(userProfile.Value(PROFILE_LIMIT, DEFAULT_LIMIT));
			limit = (limit == 0) ? Int32.Parse(DEFAULT_LIMIT) : limit;
			if (logger != null)
				logger.Debug(String.Concat(":initialise:limit:", limit.ToString()));
			page = (Int32.Parse(userProfile.Value(PROFILE_PAGE, DEFAULT_PAGE)) / limit) + 1;
			if (logger != null) { 
				logger.Debug(String.Concat(":initialise:page:", page.ToString()));
			}
		}
		private void initialise(x_userprofile userProfile, int defaultPage, string defaultSortCol, bool isDescending) {
			sort_desc = isDescending;
			sort_col = defaultSortCol;
			def_sort = false;
			limit = Int32.Parse(DEFAULT_LIMIT);
			page = defaultPage;
		}

	
	}
}
