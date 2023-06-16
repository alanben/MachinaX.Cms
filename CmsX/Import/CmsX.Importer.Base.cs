using System;
using System.IO;
using System.Globalization;
using System.Web;
using System.Xml;

using LumenWorks.Framework.IO.Csv;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2011-10-12
	Status:		release	
	Version:	2.6.0
	Build:		20111025
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20111116:	Started from FoundationX.Admin
	---------------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.BlogX.CmsX {
	/// <summary>
	/// This is the parent class of AdminReader and AdminImporter
    /// </summary>
    public class Importer {
        #region Invisible properties
		#endregion

        #region Private constants
		private const string logid = "Importer.";
		private const string XML_SERVICEX = "<ServiceX><Result><Result_Code>0</Result_Code><Description>OK</Description></Result><items/></ServiceX>";
		private const string error_reading = "Error processing the uploaded data: ";
		private const string error_listing = "Error listing the processed data: ";
		private const string error_importing = "Error importing the processed data: ";
		#endregion

        #region Public constants
		protected x_logger xLogger;
		public const string PROFILE_UPLOAD_TYPE = "UploadFileType";
		public const string PROFILE_UPLOAD_FILE = "UploadFileName";
		public const string csvDelimiter = ",;";
		public const string STATUS_OK = "ok";
		public const string STATUS_NOTOK = "notok";
		public const string STATUS_BAD = "bad";
		public const string STATUS_READOK = "read ok";
		public const string STATUS_PENDING = "pending";
		public const string STATUS_BAD_VALID = "bad.validation";
		public const string STATUS_BAD_LINE = "bad.lineid";
		public const string STATUS_BAD_TYPE = "bad.type";
		public const string STATUS_BAD_EXPECT = "bad.expect";
		public const string STATUS_BAD_DEFAULT = "bad.default";
		public const string STATUS_BAD_COUNT = "bad.count";
		public const string STATUS_BAD_EXCEPT = "bad.exception";
		public const string PAIR_NAME = "name";
		public const string PAIR_ID = "id";
		public const string PAIR_VALUE = "value";
		public const string ERROR = "error";
		public const string ERRORCODE = "errorcode";
		public const string ERRORDESC = "errordesc";
		public const string STATUS = "status";
		public const string ROW = "row";
		public const string VALUE = "value";
		public const string DEFAULTS = "defaults";
		public const string DEFAULT = "default";
		public const string SUBITEMS = "subitems";
		public const string SUBITEM	 = "subitem";
			
		private const string GENDERS		= "male|m|female|f";
		private const string GENDER_MALE	= "Male";
		private const string GENDER_FEMALE	= "Female";

		protected const string OPTIONS_YESNO		= "Yes|No";
		protected const string OPTIONS_INOUT		= "In|Out";
		protected const string OPTIONS_SALARY		= "APS|CTC";
		protected const string OPTIONS_INVESTMENT	= "AVC|Endowment|Annuity|Unit Trust|Other";
		protected const string OPTIONS_FREQUENCY	= "Monthly|Yearly|Once-off";
		protected const string RANGE_FUNDTYPEID	= "1|2|1";
		protected const string RANGE_FUNDRISKID	= "1|4|1";
		protected const string RANGE_RETIRE		= "50|70|1";
		protected const string RANGE_CONTRIB	= "0|0.2|0.001";
		protected const string RANGE_PERCENT	= "0|1|0.001";

		// Required...?
		protected const string PERIOD = "period";
		protected const string VECTOR = "vector";
		protected const string VECTORS = "vectors";
		protected const string COUNT = "count";
		protected const string EXPECT = "expect";
		protected const string INDEX = "index";
		#endregion

        #region Visible properties
		private x_userprofile profile;
		/// <summary>
		/// The UserProfile object
		/// </summary>
		protected x_userprofile Profile {
			get { return profile; }
			set { profile = value; }
		}

		private Char[] optionSplit;
		/// <summary>
		/// The Character array for splitting options string
		/// </summary>
		public Char[] OptionSplit {
			get { return optionSplit; }
			set { optionSplit = value; }
		}

		/// <summary>
		/// The text input file containing the raw data
		/// </summary>
		public string InputFile {
			get { return Profile.Value(PROFILE_UPLOAD_FILE); }
		}
		/// <summary>
		/// The type (ie format) of the raw data
		/// </summary>
		public string InputType {
			get { return Profile.Value(PROFILE_UPLOAD_TYPE); }
		}
		/// <summary>
		/// The xml document filename containing the processed (uploaded) data
		/// </summary>
		public string UploadDocName {
			get { return String.Concat(InputFile, ".xml"); }
		}
		/// <summary>
		/// The xml document filename containing the imported data
		/// </summary>
		public string ImportDocName {
			get { return String.Concat(InputFile, ".import.xml"); }
		}
		/// <summary>
		/// The xml document containing the processed (uploaded) data
		/// </summary>
		public XmlDocument UploadDoc {
			get { 
				XmlDocument listDoc = new XmlDocument();
				listDoc.Load(UploadDocName);
				return listDoc;
			}
		}
		/// <summary>
		/// The xml document containing the imported data
		/// </summary>
		public XmlDocument ImportDoc {
			get {
				XmlDocument listDoc = new XmlDocument();
				listDoc.Load(ImportDocName);
				return listDoc;
			}
		}

		private CsvReader reader;
		/// <summary>
		/// The CSV Reader
		/// </summary>
		protected CsvReader _Reader {
			get {	
				if (reader == null) {
					reader = new CsvReader(new StreamReader(InputFile), true);
				}
				return reader;
			}
			set { reader = value; }
		}

		private XmlElement uploadItem;
		/// <summary>The current uploaded item element</summary>
		public XmlElement UploadItem {
			get { return uploadItem; }
			set { uploadItem = value; }
		}
		/// <summary>The current uploaded item's value</summary>
		public string Upload {
			get { return UploadItem.InnerText; }
			set { UploadItem.InnerText = value; }
		}
		/// <summary>The current uploaded item's row</summary>
		public string UploadRow {
			get { return UploadItem.GetAttribute(ROW); }
			set { UploadItem.SetAttribute(ROW, value); }
		}
		/// <summary>The current uploaded item's status</summary>
		public string UploadStatus {
			get { return UploadItem.GetAttribute(STATUS); }
			set { UploadItem.SetAttribute(STATUS, value); }
		}

		private XmlElement currentItem;
		/// <summary>The current item element</summary>
		public XmlElement CurrentItem {
			get { return currentItem; }
			set { currentItem = value; }
		}
		/// <summary>The current item's value</summary>
		public string Current {
			get { return CurrentItem.InnerText; }
			set { CurrentItem.InnerText = value; }
		}
		/// <summary>The current item's row</summary>
		public string CurrentRow {
			get { return CurrentItem.GetAttribute(ROW); }
			set { CurrentItem.SetAttribute(ROW, value); }
		}
		/// <summary>The current item's status</summary>
		public string CurrentStatus {
			get { return CurrentItem.GetAttribute(STATUS); }
			set { CurrentItem.SetAttribute(STATUS, value); }
		}
		#endregion

        #region Constructors/Destructors
        /// <summary>Default constructor</summary>
        public Importer() {
			optionSplit = new Char[] { '|' };
        }
        /// <summary>Constructor for derived classes</summary>
        public Importer(x_userprofile uprofile, string logid){
			optionSplit = new Char[] { '|' };
			xLogger = new x_logger(typeof(Importer), logid, false, true);
			profile = uprofile;
        }
        /// <summary>Common constructor</summary>
		//public Importer(displayX thispage) {
		//    xLogger = new x_logger(typeof(Importer), logid, false, true);
		//    profile = thispage.UserProfile;
		//}
        #endregion

		#region Public methods
		#endregion

		#region Protected Validation methods
		/// <summary>Validates column contains string (not null)</summary>
		protected string _ValidateExists(string input, int col) {
			string val = removeQuotes(input);
			validateExists(val, col);
			return val;
		}
		
		/// <summary>Validates column contains number (not null)</summary>
		protected string _ValidateNumber(string input, int col) {
			return _ValidateNumber(input, col, false);
		}
		/// <summary>Validates column contains number within range (not null)</summary>
		protected string _ValidateNumber(string input, int col, string range) {
			return _ValidateRange(input, col, range, false, ValidationType.Decimal);
		}
		/// <summary>Validates column contains number</summary>
		protected string _ValidateNumber(string input, int col, bool allowNull) {
			string val = removeQuotes(input);
			if (!allowNull) {
				validateExists(val, col);
			}
			if (!String.IsNullOrEmpty(val)) {
				val = validateNumber(val, col).ToString();
			}
			return val;
		}
		
		/// <summary>Validates column contains number (not null, zero ok)</summary>
		protected string _ValidateCurrency(string input, int col) {
			return _ValidateCurrency(input, col, false, true);
		}
		/// <summary>Validates column contains number within range (not null)</summary>
		protected string _ValidateCurrency(string input, int col, string range) {
			return _ValidateRange(input, col, range, false, ValidationType.Decimal);
		}
		/// <summary>Validates column contains number</summary>
		protected string _ValidateCurrency(string input, int col, bool allowNull, bool allowZero) {
			string val = removeQuotes(input);
			if (!allowNull) {
				validateExists(val, col);
			}
			CultureInfo enZA = CultureInfo.CreateSpecificCulture("en-ZA");
			if (!String.IsNullOrEmpty(val)) {
				val = validateCurrency(val, col, allowZero).ToString("C", enZA);
			}
			return val;
		}
		
		/// <summary>Validates column contains percent (not null)</summary>
		protected string _ValidatePercent(string input, int col) {
			return _ValidatePercent(input, col, false);
		}
		/// <summary>Validates column contains percent within range (not null)</summary>
		protected string _ValidatePercent(string input, int col, string range) {
			return _ValidateRange(input, col, range, false, ValidationType.Percent);
		}
		/// <summary>Validates column contains percent</summary>
		protected string _ValidatePercent(string input, int col, bool allowNull) {
			string val = removeQuotes(input);
			if (!allowNull) {
				validateExists(val, col);
			}
			if (!String.IsNullOrEmpty(val)) {
				val = validatePercent(val, col).ToString("#0.0%");
			}
			return val;
		}
		/// <summary>Validates column contains date (or is null)</summary>
		protected string _ValidateDate(string input, int col) {
			return _ValidateDate(input, col, true);
		}
		/// <summary>Validates column contains date</summary>
		protected string _ValidateDate(string input, int col, bool allowNull) {
			string val = removeQuotes(input);
			if (!allowNull) {
				validateExists(val, col);
			}
			if (!String.IsNullOrEmpty(val)) {
				val = validateDate(val, col);
			}
			return val;
		}
		/// <summary>Validates column contains date (or is null)</summary>
		protected string _ValidateGender(string input, int col) {
			return _ValidateGender(input, col, true);
		}
		/// <summary>Validates column contains date</summary>
		protected string _ValidateGender(string input, int col, bool allowNull) {
			string val = removeQuotes(input);
			if (!allowNull) {
				validateExists(val, col);
			}
			if (!String.IsNullOrEmpty(val)) {
				val = validateGender(val, col);
			}
			return val;
		}
		/// <summary>Validates column is an option (or is null)</summary>
		protected string _ValidateOptions(string input, int col, string options) {
			return _ValidateOptions(input, col, options, true);
		}
		/// <summary>Validates column is an option</summary>
		protected string _ValidateOptions(string input, int col, string options, bool allowNull) {
			string val = removeQuotes(input);
			if (!allowNull) {
				validateExists(val, col);
			}
			if (!String.IsNullOrEmpty(val)) {
				val = validateOptions(val, options, col);
			}
			return val;
		}
		/// <summary>Validates column is an option (or is null)</summary>
		protected string _ValidateYesNo(string input, int col) {
			return _ValidateYesNo(input, col, true);
		}
		/// <summary>Validates column is an option</summary>
		protected string _ValidateYesNo(string input, int col, bool allowNull) {
			string val = removeQuotes(input);
			if (!allowNull) {
				validateExists(val, col);
			}
			if (!String.IsNullOrEmpty(val)) {
				val = validateOptions(val, OPTIONS_YESNO, col);
			}
			return val;
		}
		/// <summary>Validates column is an option (or is null)</summary>
		protected string _ValidateRange(string input, int col, string range) {
			return _ValidateRange(input, col, range, true, ValidationType.Integer);
		}
		/// <summary>Validates column within range</summary>
		protected string _ValidateRange(string input, int col, string range, bool allowNull, ValidationType type) {
			string val = removeQuotes(input);
			if (!allowNull) {
				validateExists(val, col);
			}
			if (!String.IsNullOrEmpty(val)) {
				switch (type) {
					case ValidationType.Integer:
						int ivl = validateInteger(val, col);
						val = validateRange(ivl, range, col);
						break;
					case ValidationType.Decimal:
						decimal dvl = validateNumber(val, col);
						val = validateRange(dvl, range, col);
						break;
					case ValidationType.Percent:
						decimal prc = validatePercent(val, col);
						val = validateRange(prc, range, col);
						break;
				}
			}
			return val;
		}
		
		/// <summary>Validates column is an option</summary>
		protected int _GetOptionIndex(string input, string options) {
			string val = removeQuotes(input);
			return getOptionIndex(val, options);
		}

		protected decimal _GetCurrency(string val) {
			return validateCurrency(val);
		}

		protected decimal _GetPercent(string val) {
			return validatePercent(val, -1);
		}

		protected DateTime _GetDate(string val) {
			return validateDate(val);
		}

		protected bool _GetYesNo(string val) {
			return (validateOptions(val, OPTIONS_YESNO, -1) == "Yes");
		}

		protected bool _GetInOut(string val) {
			return (validateOptions(val, OPTIONS_INOUT, -1) == "In");
		}
		#endregion

		#region Private Validation methods
		private void validateExists(string val, int col) {
			if (String.IsNullOrEmpty(val)) {
				throw new ValidationException("error_null", String.Format("{0} empty value", getColumn(col)));
			}
		}
		private decimal validateNumber(string val, int col) {
			return validateNumber(val, col, true);
		}
		private decimal validateNumber(string val, int col, bool allowZero) {
			decimal num = 0;
			if (!decimal.TryParse(val, out num)) {
				throw new ValidationException("error_number", String.Format("{0} not valid number", getColumn(col)));
			}
			if (!allowZero && num == 0) {
				throw new ValidationException("error_number", String.Format("{0} zero number", getColumn(col)));
			}
			return num;
		}
		private decimal validateCurrency(string val) {
			decimal num = 0;
			CultureInfo enZA = CultureInfo.CreateSpecificCulture("en-ZA");
			if (!decimal.TryParse(val, NumberStyles.Currency, enZA, out num)) {
				throw new ValidationException("error_number", String.Format("{0} not valid number", getColumn(-1)));
			}
			return num;
		}
		private decimal validateCurrency(string val, int col, bool allowZero) {
			return validateNumber(val.ToUpper().Replace("R", "").Replace(",", "").Replace(" ", ""), col, allowZero);
		}
		private int validateInteger(string val, int col) {
			int num = 0;
			if (!int.TryParse(val, out num)) {
				throw new ValidationException("error_integer", String.Format("{0} not valid integer", getColumn(col)));
			}
			return num;
		}
		private decimal validatePercent(string val, int col) {
			decimal num = 0;
			bool isliteral = (val.IndexOf('%') > 0);
			val = val.Replace("%", "").Replace(" ", "");
			if (!decimal.TryParse(val, out num)) {
				throw new ValidationException("error_percent", String.Format("{0} not valid percent ('{1}')", getColumn(col), val));
			}
			return isliteral? num / 100 : num;
		}
		private DateTime validateDate(string val) {
			CultureInfo enZA = CultureInfo.CreateSpecificCulture("en-ZA");
			DateTime dte = DateTime.MinValue;
			if (!DateTime.TryParse(val, enZA, DateTimeStyles.AssumeLocal, out dte)) {
				throw new ValidationException("error_date", String.Format("{0} not valid date, should be '{1}'", getColumn(-1), enZA.DateTimeFormat.ShortDatePattern));
			}
			return dte;
		}
		private string validateDate(string val, int col) {
			CultureInfo enZA = CultureInfo.CreateSpecificCulture("en-ZA");
			DateTime dte = DateTime.MinValue;
			if (!DateTime.TryParse(val, enZA, DateTimeStyles.AssumeLocal, out dte)) {
				throw new ValidationException("error_date", String.Format("{0} not valid date, should be '{1}'", getColumn(col), enZA.DateTimeFormat.ShortDatePattern));
			}
			return dte.ToString(enZA);
		}
		private string validateGender(string val, int col) {
			string gen = val.ToLower();
			bool isMale = true;
			int index = -1;
			string[] genders = GENDERS.Split(optionSplit);
			for (int i = 0; i <= genders.Length; i++) {
				if (gen == genders[i]) {
					index = i;
					isMale = (i < 2);
					break;
				}
			}
			if (index < 0) {
				throw new ValidationException("error_gender", String.Format("{0} not valid gender ({1})", getColumn(col), val));
			}
			return isMale? GENDER_MALE : GENDER_FEMALE;
		}
		private string validateOptions(string val, string opts, int col) {
			string opt = val.ToLower();
			xLogger.Debug("validateOptions", "::opt:", opt, "::opts:", opts);
			int index = -1;
			string[] options = opts.Split(optionSplit);
			for (int i = 0; i < options.Length; i++) {
				if (opt == options[i].ToLower()) {
					index = i;
					break;
				}
			}
			xLogger.Debug("validateOptions", "::index:", index.ToString());
			if (index < 0) {
				throw new ValidationException("error_option", String.Format("{0} not valid option ({1})", getColumn(col), val));
			}
			return options[index];
		}
		private int getOptionIndex(string val, string opts) {
			string opt = val.ToLower();
			int index = -1;
			string[] options = opts.ToLower().Split(optionSplit);
			for (int i = 0; i <= options.Length; i++) {
				if (opt == options[i].ToLower()) {
					index = i;
					break;
				}
			}
			return index+1;	// Not zero based, ie zero = no option
		}
		private string validateRange(int val, string rnge, int col) {
			string[] range = rnge.Split(optionSplit);
			int min = int.Parse(range[0]);
			int max = int.Parse(range[1]);

			if (val < min || val > max) {
				throw new ValidationException("error_range", String.Format("{0} not within range ({1})", getColumn(col), val.ToString()));
			}
			return val.ToString();
		}
		private string validateRange(decimal val, string rnge, int col) {
			string[] range = rnge.Split(optionSplit);
			decimal min = decimal.Parse(range[0]);
			decimal max = decimal.Parse(range[1]);

			if (val < min || val > max) {
				throw new ValidationException("error_range", String.Format("{0} not within range ({1})", getColumn(col), val.ToString()));
			}
			return val.ToString();
		}

		private string getColumn(int col) {
			string ret;
			if (col < 0) {
				ret = "Value:";
			} else if (reader == null || !reader.HasHeaders) {
				ret = String.Format("Column {0}:", col.ToString());
			} else {
				string[] headers = reader.GetFieldHeaders();
				ret = String.Format("Field '{0}':", headers[col - 1]);
			}
			return ret;
		}
		#endregion

		#region Protected methods
		protected string _RemoveQuotes(string input) {
			return removeQuotes(input);
		}
		#endregion

        #region Private methods
		private string removeQuotes(string input) {
			string output = input;
			if (output.IndexOf('"') == 0 && output.LastIndexOf('"') == output.Length - 1) {
				output = output.Substring(1, output.Length - 2);
			}
			return output.Trim();
		}
		#endregion
    }
	/// <summary>
	/// 
	/// </summary>
	public enum ValidationType {
		  Integer = 1
		, Decimal = 2
		, Percent = 3
	}

	/// <summary>
	/// This is the parent class of AdminReader and AdminImporter
    /// </summary>
    public class ValidationException : Exception {

		#region Visible properties
		private string code;
		/// <summary>Error identifier</summary>
		public string Code {
			get { return code; }
			set { code = value; }
		}
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public ValidationException(string ErrorCode, string ErrorMessage)
			: base(ErrorMessage) {
		}
		#endregion
	}

	/// <summary>
	/// This is the parent class of AdminReader and AdminImporter
	/// </summary>
	public class HeaderException : ReaderException {
		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public HeaderException(string ErrorCode, string ErrorMessage)
			: base(ErrorCode, ErrorMessage) {
		}
		#endregion
	}

	/// <summary>
	/// This is the parent class of AdminReader and AdminImporter
	/// </summary>
	public class ReaderException : Exception {

		#region Visible properties
		private string code;
		/// <summary>Error identifier</summary>
		public string Code {
			get { return code; }
			set { code = value; }
		}
		#endregion

		#region Constructors/Destructors
		/// <summary>Default constructor</summary>
		public ReaderException(string ErrorCode, string ErrorMessage)
			: base(ErrorMessage) {
		}
		#endregion
	}
}
