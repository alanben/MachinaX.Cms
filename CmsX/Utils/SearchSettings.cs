using log4net;
using System;
using XXBoom.MachinaX.Domain.Interface;

namespace XXBoom.MachinaX.BlogX.CmsX {
	/// <summary>This is a utility class for the Search Parameters</summary>
	public class SearchSettings : ISearchSettings {
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

		/// <summary>Default Constructor</summary>
		public SearchSettings() {
			int defaultPage = 1;
			string defaultSortCol = "";
			bool isDescending = false;
			initialise(defaultPage, defaultSortCol, isDescending);
			MaxRows = true;
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
			initialise(DefaultPage, DefaultSortCol, IsDescending);
		}
		/// <summary>Constructor</summary>
		public SearchSettings(x_userprofile UserProfile, ILog Logger, int DefaultPage, string DefaultSortCol, bool IsDescending, bool IsMaxRows) {
			initialise(DefaultPage, DefaultSortCol, IsDescending);
			MaxRows = IsMaxRows;
			Logger.Debug(String.Concat(":SortDesc:", sort_desc.ToString(), ":SortCol:", sort_col, ":Page:", page));
		}
		/// <summary>Constructor</summary>
		public SearchSettings(int DefaultPage, string DefaultSortCol, bool IsDescending, bool IsMaxRows) {
			initialise(DefaultPage, DefaultSortCol, IsDescending);
			MaxRows = IsMaxRows;
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
		private void initialise(int defaultPage, string defaultSortCol, bool isDescending) {
			sort_desc = isDescending;
			sort_col = defaultSortCol;
			def_sort = false;
			limit = Int32.Parse(DEFAULT_LIMIT);
			page = defaultPage;
		}
	}
}
