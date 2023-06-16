
/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-01
	Status:		release	
	Version:	4.0.3
	Build:		20161005
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20111016:	Started
	20111116:	Added _GetValue override and _GetCheck
	20131028:	Added IID
	20161005:	Try make _GetValue for decimal and double more forgiving
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {

	using System;
	using System.Globalization;
	using System.Xml;
	
	/// <summary>
    /// This is a utility class to encapsulte profile and other functionality related to a xxxx.
    /// Typically this class is created within brokers and the display classes
    /// </summary>
    public class CmsXSubItem {
        #region Invisible properties
		private string blankfield;
		#endregion

        #region Private constants
		protected const char SEPARATOR = '|';
		protected const char SEPARATOR_PAIR = '~';
		protected const string FIELD_SYS = "sys";
		protected const string FIELD_ID = "id";
		protected const string FIELD_REMOVE = "remove";
		protected const string PROFILE_PREFIX = "_record_";
		protected const int MAX_RECORDS = 1000;
		#endregion

        #region Public constants
        #endregion

        #region Visible properties
		private x_logger xlogger;
		/// <summary></summary>
		public x_logger xLogger {
			get { return xlogger; }
			set { xlogger = value; }
		}
		
		private string id;
		/// <summary></summary>
		public string ID {
			get { return id; }
			set { id = value; }
		}
		public int IID { get { return Convert.ToInt32(ID); } }
		
		private string[] fields;
		/// <summary></summary>
		public string[] Fields {
			get { return fields; }
			set { fields = value; }
		}
		
		private bool isDelete;
		/// <summary></summary>
		public bool IsDelete {
			get { return isDelete; }
			set { isDelete = value; }
		}
		
		private bool isNew;
		/// <summary></summary>
		public bool IsNew {
			get { return isNew; }
			set { isNew = value; }
		}
		
		private bool isBad;
		/// <summary></summary>
		public bool IsBad {
			get { return isBad; }
			set { isBad = value; }
		}
		
		private string[] pairs;
		/// <summary></summary>
		public string[] Pairs {
			get { return pairs; }
			set { pairs = value; }
		}
		
		private XmlElement item;
		/// <summary></summary>
		public XmlElement Item {
			get { return item; }
			set { item = value; }
		}

		private int index = 0;
		/// <summary></summary>
		public int Index {
			get { return index; }
			set { index = value; }
		}
		#endregion

        #region Constructors/Destructors
        /// <summary>Default constructor</summary>
		public CmsXSubItem(x_logger Logger) {
			xLogger = Logger;
			blankfield = CmsXBrokerBase.DEFAULT_SUBITEM;
		}
		/// <summary>Common constructor</summary>
		public CmsXSubItem(x_logger Logger, string BlankField) {
			xLogger = Logger;
			blankfield = BlankField;
		}
		/// <summary>Common constructor</summary>
		public CmsXSubItem(x_logger Logger, string BlankField, string Record) {
			xLogger = Logger;
			initialise(BlankField, Record);
		}
        #endregion

        #region Public methods
		/// <summary>Initialise object with new source record string</summary>
		public virtual void Initialise(string Record) {
			initialise(Record);
		}
		/// <summary>Reset object with new source record string</summary>
		public virtual void Reset(string Record) {
			initialise(Record);
		}
		/// <summary></summary>
		public virtual void Process(bool New, int Prefix, x_userprofile UserProfile) {
			Index = Prefix;
			Process(New, Prefix.ToString(), UserProfile);
		}
		/// <summary></summary>
		public virtual void Process(bool New, string Prefix, x_userprofile UserProfile) {
			xLogger.Debug("Process", ":", Prefix, "::New:", New.ToString());

			for (int i = 0; i < MAX_RECORDS; i++) {
				string prefix = String.Concat(Prefix, PROFILE_PREFIX, i.ToString());
				string record = UserProfile.Value(prefix, "");
				UserProfile.Clear(prefix);

				if (!String.IsNullOrEmpty(record)) {
					xLogger.Debug("Process", "::prefix:", prefix, "  ::record:", record);
					initialise(record);
					Process(New);
				}
			}
			//xLogger.Debug("Process:ok");
		}
		/// <summary></summary>
		public virtual void Process(bool New) {
			//xLogger.Debug("Process");
			if (IsDelete) {
				_Delete();
			} else if (New || isNew) {
				_Add();
			} else if (!IsBad) {
				_Edit();
			}
			//xLogger.Debug("Process:ok");
		}
		#endregion

        #region Protected methods
		protected string _GetValue(string id) {
			return getValue(id);
		}
		protected int _GetValue(string id, int defval) {
			int result = 0;
			return (int.TryParse(getValue(id), out result))? result : defval;
		}
		protected decimal _GetValue(string id, decimal defval) {
			decimal result = 0M;
			bool success = Decimal.TryParse(getValue(id), out result);
			if (!success) {
				success = Decimal.TryParse(getValue(id), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
			}
			return (success) ? result : defval;
		}
		protected double _GetValue(string id, double defval) {
			double result = 0.0;
			bool success = Double.TryParse(getValue(id), out result);
			if (!success) {
				success = Double.TryParse(getValue(id), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
			}
			return (success) ? result : defval;
		}
		protected bool _GetCheck(string id) {
			string val = getValue(id);
			return (val == "true" || val == "1");
		}
		/// <summary>Add subitem - override in derived class</summary>
		protected virtual void _Add() {

		}
		/// <summary>Edit subitem - override in derived class</summary>
		protected virtual void _Edit() {

		}
		/// <summary>Delete subitem - override in derived class</summary>
		protected virtual void _Delete() {

		}


		#endregion

        #region Private methods
		private void initialise(string BlankField, string Record) {
			blankfield = BlankField;
			initialise(Record);
		}
		protected virtual void _Initialise(string Record) {
			initialise(Record);
		}
		private void initialise(string Record) {
			//xLogger.Debug("initialise");
			isNew = false;
			isDelete = false;
			isBad = true;
			ID = "";
			//xLogger.Debug("initialise", "::Record:", Record);

			Fields = Record.Split(new char[] { SEPARATOR });
			foreach (string field in Fields) {
				string[] pair = field.Split(new char[] { SEPARATOR_PAIR });
				if (pair[0] == FIELD_SYS) {
					//if (pair[1] == blankfield) {
					if (isDefault(pair[1])) {
						IsNew = true;
					}
				} else if (pair[0] == FIELD_REMOVE) {
					if (pair[1] == "true") {
						IsDelete = true;
					}
				} else if (pair[0] == FIELD_ID) {
					if (pair[1] != blankfield) {
						IsBad = false;
						ID = pair[1];
					}
				} else {
					// all ok
				}
			}
			xLogger.Debug("initialise", "::ID:", ID, "::IsDelete:", IsDelete.ToString(), "::isNew:", isNew.ToString(), "::IsBad:", IsBad.ToString());
			//xLogger.Debug("initialise:ok");
		}
		private string getValue(string id) {
			//xLogger.Debug("getValue", "::id:", id);
			string ret = "";
			foreach (string field in Fields) {
				string[] pair = field.Split(new char[] { SEPARATOR_PAIR });
				if (pair[0] == id) {
					ret = pair[1];
					break;
				}
			}
			//xLogger.Debug("getValue", "::ret:", ret);
			return ret;
		}

		// see CmsXBrokerBase._IsDefault
		protected bool isDefault(string value) {
			return (value == blankfield || value.ToLower() == "default");
		}
		#endregion
    }
}
