
using System;
using log4net;

using XXBoom.MachinaX.Domain.Interface;

namespace XXBoom.MachinaX.BlogX.CmsX {

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

}
