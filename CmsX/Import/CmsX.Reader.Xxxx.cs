using System;
using System.IO;
using System.Web;
using System.Xml;

using log4net;
//using clickclickboom.machinaX.blogX;
//using clickclickboom.machinaX.blogX.cmsX;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2011-10-12
	Status:		release	
	Version:	2.6.0
	Build:		20111012
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20111012:	Started
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
	/// <summary>
    /// This is a utility class to encapsulte functionality related to reading import file contents.
    /// Typically this class is created within brokers and the display classes
    /// </summary>
    public class YyyyReader : Importer {
        #region Invisible properties
		private Yyyys _Yyyys;
		#endregion

        #region Private constants
		private const string logid = "YyyyReader.";
		#endregion

        #region Public constants
        #endregion

        #region Visible properties
		private char[] csvDelimiters;
		/// <summary>
		/// 
		/// </summary>
		public char[] CsvDelimiters {
			get { return csvDelimiters; }
			set { csvDelimiters = value; }
		}

		#endregion

        #region Constructors/Destructors
        /// <summary>Default constructor</summary>
        public YyyyReader() {
			initialise();
		}
        /// <summary>Constructor for derived classes</summary>
        public YyyyReader(x_userprofile uprofile, Yyyys yyyyws) : base(uprofile, logid) {
			_Yyyys = yyyyws;
			initialise();
        }
        #endregion

        #region Public methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="OutputDoc"></param>
		/// <param name="IsBulkFile"></param>
		public void Read(XmlDocument OutputDoc) {
			_Read(OutputDoc);
		}
		#endregion

        #region Protected methods
		/// <summary>
		/// Read bulk file to XmlDocument for listing
		/// </summary>
		public void _Read(XmlDocument outputdoc) {
			switch (InputType) {
				case "csv":
					readCSV(outputdoc, InputFile, false);
					break;
				case "csvdata":
					readCSVDefaults(outputdoc, InputFile, false);
					break;
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Read CSV file format to XmlDocument for listing
		/// </summary>
		private void readCSV(XmlDocument outputdoc, string inputfile, Boolean wantdata) {
			if (wantdata) {
			} else {
				readCSV(outputdoc, inputfile);
			}
		}
		/// <summary>
		/// Read CSV+Data file format to XmlDocument for listing
		/// </summary>
		private void readCSVDefaults(XmlDocument outputdoc, string inputfile, Boolean wantdata) {
			if (wantdata) {
			} else {
				getDefaults();
				readCSVDefaults(outputdoc, inputfile);
			}
		}
		#endregion

		#region Private utility methods
		private void initialise() {
			CsvDelimiters = csvDelimiter.ToCharArray();
		}
		#endregion

		#region Private CSV methods
		/// <summary>Load CSV (yyyy basics only)</summary>
		/// <remarks>Columns:"SeriesID","Name","Description","PeriodStart","PeriodEnd","Quantity","SourceID","TypeID"</remarks>
		private void readCSV(XmlDocument outdoc, string filepath) {
			xLogger.Debug("readCSV::filepath:", filepath);
			XmlElement items = outdoc.SelectSingleNode("//items") as XmlElement;
			int counter = 0;
			try {
				using (StreamReader rdr = new StreamReader(filepath)) {
					String line;
					while ((line = rdr.ReadLine()) != null) {
						xLogger.Debug("readCSV::line:", line);
						// assume first line contains column names
						if (counter > 0) {
							string[] toks = line.Split(CsvDelimiters);
							xLogger.Debug("readCSV::toks:", toks.Length.ToString());
							if (toks.Length == 3) {
								XmlElement item = items.AppendChild(outdoc.CreateElement(Cms.ITEM)) as XmlElement;
								csvItem(item, counter.ToString(), toks[0], toks[1], toks[2]);
							}
						}
						counter++;
					}
					items.SetAttribute("rows", (counter - 1).ToString());
				}
			} catch (Exception e) {
				xLogger.Debug("readCSV::error:", e.Message);
			}
			xLogger.Debug("readCSV:ok");
		}

		/// <summary>Load CSV (basics + defaults)</summary>
		private void readCSVDefaults(XmlDocument outdoc, string filepath) {
			xLogger.Debug("readCSVDefaults::filepath:", filepath);
			XmlElement items = outdoc.SelectSingleNode("//items") as XmlElement;
			int counter = 0;
			try {
				using (StreamReader rdr = new StreamReader(filepath)) {
					String line;
					while ((line = rdr.ReadLine()) != null) {
						xLogger.Debug("readCSVDefaults::line:", line);
						// assume first line contains column names
						if (counter > 0) {
							string[] toks = line.Split(CsvDelimiters);
							xLogger.Debug("readCSVDefaults::toks:", toks.Length.ToString());
							if (toks.Length >= 3) {
								int defcount = 0;
								XmlElement item = items.AppendChild(outdoc.CreateElement(Cms.ITEM)) as XmlElement;
								csvItem(item, counter.ToString(), toks[0], toks[1], toks[2]);
								if (toks.Length > 3) {
									XmlElement deflts = item.AppendChild(outdoc.CreateElement(DEFAULTS)) as XmlElement;
									for (int i = 3, j = 4; j < toks.Length; i=i+2, j=j+2) {
										XmlElement deflt = deflts.AppendChild(outdoc.CreateElement(DEFAULT)) as XmlElement;
										csvItem(deflt, defcount.ToString(), toks[i], toks[j]);
										checkDefault(item, deflt);
										defcount++;
									}
									deflts.SetAttribute(COUNT, defcount.ToString());
								}
							}
						}
						counter++;
					}
					items.SetAttribute("rows", (counter - 1).ToString());
				}
			} catch (Exception e) {
				xLogger.Debug("readCSV::error:", e.Message);
			}
			xLogger.Debug("readCSV:ok");
		}
		
		/// <summary>Load CSV (basics + defaults)</summary>
		private void getDefaults() {
			SearchSettings settings = new SearchSettings(Profile, xLogger, true);
			//_Yyyys.ListDefaultTypes(settings);
		}

		/// <summary>Load CSV (basics + defaults)</summary>
		private void checkDefault(XmlElement item, XmlElement deflt) {
			XmlElement deftype = _Yyyys.ListItems.SelectSingleNode(String.Format("item[@id='{0}']", deflt.GetAttribute("id"))) as XmlElement;
			if (deftype != null) {
				deflt.SetAttribute("desc", deftype.GetAttribute("desc"));
				deflt.SetAttribute("unit", deftype.GetAttribute("unit"));
			} else {
				deflt.SetAttribute("desc", "??");
				deflt.SetAttribute("unit", "-");
				deflt.SetAttribute(STATUS, "notok");
				deflt.SetAttribute(ERROR, "no such setting");
				item.SetAttribute(STATUS, STATUS_BAD_DEFAULT);
				item.SetAttribute(ERROR, String.Format("Unknown setting for default {0}", deflt.GetAttribute(ROW)));
			}
		}

		#endregion

		#region Private xml utility methods
		/// <summary>Update a summary item</summary>
		/// <param name="item"></param>
		/// <param name=row></param>
		/// <param name="name"></param>
		/// <param name="desc"></param>
		/// <param name="email"></param>
		private void csvItem(XmlElement item, string row, string name, string desc, string email) {
			item.SetAttribute(ROW, row);
			item.SetAttribute("name", _RemoveQuotes(name));
			item.SetAttribute("desc", _RemoveQuotes(desc));
			item.SetAttribute("email", _RemoveQuotes(email));
			item.SetAttribute(STATUS, STATUS_OK);
			item.SetAttribute(ERROR, "");
		}
		/// <summary>Update a summary item</summary>
		/// <param name="item"></param>
		/// <param name=row></param>
		/// <param name="id"></param>
		/// <param name="value"></param>
		private void csvItem(XmlElement item, string row, string id, string value) {
			item.SetAttribute(ROW, row);
			item.SetAttribute(PAIR_ID, _RemoveQuotes(id));
			item.SetAttribute(PAIR_VALUE, _RemoveQuotes(value));
			item.SetAttribute(STATUS, STATUS_OK);
			item.SetAttribute(ERROR, "");
		}
		#endregion
    }
}
