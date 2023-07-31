/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2015-03-16
	Status:		release	
	Version:	4.0.3
	Build:		20150316
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20150316:	Started
	-----------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.GeneratorX {

	using XXBoom.MachinaX;
	using OfficeOpenXml;
	using System;
	using System.IO;
	using System.Web;
	using System.Web.Services;
	using System.Xml;

    /// <summary>
    /// Excel De-Generator web service
    /// </summary>
    [WebService(Name = "DegenerateExcel",
				Namespace = "http://www.clickclickBOOM.com/MachinaX/GenerateX",
				Description = "DegenerateExcel Web Service")]
	public class DegenerateExcel : GenerateBase {

		private const string CONFIG_ID = "Excel";

		/// <summary>Constructor</summary>
		public DegenerateExcel() : base("DegenerateExcel", CONFIG_ID) {
		}
		public DegenerateExcel(string RootName) : base(RootName, CONFIG_ID) {
		}

		/// <summary>
		/// Degenerate the first worksheet of an Excel file into a result document
		/// </summary>
		/// <param name="FileName">The excel file (name.ext only - assumes in the InputDir)</param>
		[WebMethod(Description = "Degenerate the first worksheet of an Excel file into a result document")]
		public XmlDocument Degenerate(string FileName) {
			xLogger.Debug("Degenerate");
			try {
				AddOk();
				degenExcel(null, FileName);
				xLogger.Debug("Degenerate:ok");
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		/// <summary>
		/// Degenerate the first worksheet of an Excel file into a result document for specific columns
		/// </summary>
		/// <param name="ColumnsXml">The columns xml document eg by subobjs/subwf_obj/</param>
		/// <param name="FileName">The excel file (name.ext only - assumes in the InputDir)</param>
		[WebMethod(Description = "Degenerate the first worksheet of an Excel file into a result document for specific columns")]
		public XmlDocument DegenerateColumns(XmlDocument Columns, string FileName) {
			xLogger.Debug("DegenerateColumns");
			try {
				AddOk();
				degenerateExcel(Columns, FileName);
				xLogger.Debug("DegenerateColumns:ok");
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		/// <summary>
		/// Degenerate the first worksheet of an Excel file into a result document for specific columns
		/// </summary>
		/// <param name="ColumnsXml">The columns xml eg by subobjs/subwf_obj/</param>
		/// <param name="FileName">The excel file (name.ext only - assumes in the InputDir)</param>
		[WebMethod(Description = "Degenerate the first worksheet of an Excel file into a result document for specific columns")]
		public XmlDocument DegenerateColumnsXml(string ColumnsXml, string FileName) {
			xLogger.Debug("DegenerateColumnsXml");
			try {
				AddOk();
				XmlDocument Columns = new XmlDocument();
				Columns.LoadXml(ColumnsXml);
				degenerateExcel(Columns, FileName);
				xLogger.Debug("DegenerateColumnsXml:ok");
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}


		public XmlDocument DegenerateColumns(XmlElement Columns, string FileName) {
			xLogger.Debug("DegenerateColumns");
			try {
				AddOk();
				degenerateExcel(Columns, FileName);
				xLogger.Debug("DegenerateColumns:ok");
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		private void degenerateExcel(XmlDocument columnXml, string filename) {
			if (columnXml == null) {
				degenExcel(null, filename);
			} else {
				degenExcel(columnXml.SelectSingleNode("//subobjs") as XmlElement, filename);
			}
		}

		private void degenerateExcel(XmlElement columnXml, string filename) {
			if (columnXml == null) {
				degenExcel(null, filename);
			} else {
				degenExcel(columnXml, filename);
			}
		}
		
		private void degenExcel(XmlElement columns, string filename) {
			xLogger.Debug("degenExcel", "::filename:", filename);

			DirectoryInfo inputDir = new DirectoryInfo(InputDir);
			string filepath = String.Format("{0}\\{1}", inputDir.FullName, filename);
			xLogger.Debug("degenExcel", "::filepath:", filepath);

			FileInfo excelfile = new FileInfo(filepath);
			ChangeDataRoot("items");
			_Root.SetAttribute("filepath", excelfile.FullName);

			if (columns == null) {
				degenExcel(excelfile);
			} else {
				degenExcel(excelfile, columns, "subwf_obj");
			}
		}

		private void degenExcel(FileInfo excelfile) {
			xLogger.Debug("degenExcel", "excelfile", excelfile);

			using (ExcelPackage package = new ExcelPackage(excelfile)) {
                ExcelWorksheet worksheet = getFirstWorksheet(package);
                // Read first row - assume these are column names
                int cols, col = 1;
                string columns = String.Empty;
                bool first = true;
                while (true) {
                    try {
                        string val = worksheet.Cells[1, col].Value.ToString();
                        xLogger.Debug("degenExcel", "col", col, "val", val);

                        columns = String.Concat(columns, first ? "" : "|", val.Replace(' ', '_').ToLower());
                        if (first) {
                            first = false;
                        }
                    } catch {
                        xLogger.Debug("degenExcel::ends:", "col", col);
                        break;
                    }
                    col++;
                    if (col > worksheet.Cells.Columns) {    // safety
                        break;
                    }
                }
                cols = col;
                xLogger.Debug("degenExcel", "::cols:", cols, "::columns:", columns);
                string[] attributes = columns.Split(new char[] { '|' });

                // Now read rows based on above attributes (columns), stop on first row with blank first cell
                XmlElement item;
                int row = 2;
                while (true) {
                    try {
                        // Row with first column null signifies end of data
                        if (worksheet.Cells[row, 1].Value == null) {
                            xLogger.Debug("degenExcel::ends.null:", "row", row);
                            break;
                        }
                        // or... row with first column blank signifies end of data
                        if (String.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Value.ToString())) {
                            xLogger.Debug("degenExcel::ends.blank:", "row", row);
                            break;
                        }

                        item = AddNode(_Root, "item", "") as XmlElement;
                        item.SetAttribute("row", (row - 1).ToString());
                        item.SetAttribute("status", "notok");

                        for (col = 1; col < cols; col++) {
                            object obj = worksheet.Cells[row, col].Value;
                            string nme = attributes[(col - 1)];
                            string val = (obj == null) ? String.Empty : obj.ToString();
                            //xLogger.Debug("degenExcel", "row", row, "col", col, "nme", nme, "val", val);

                            item.SetAttribute(nme, val);
                        }
                        item.SetAttribute("status", "ok");
                    } catch {
                        xLogger.Debug("degenExcel::ends.error:", "row", row);
                        break;
                    }
                    row++;
                    if (row > worksheet.Cells.Rows) {   // safety
                        break;
                    }
                }
            }
        }

        private ExcelWorksheet getFirstWorksheet(ExcelPackage package) {
            ExcelWorksheet worksheet;
            // get the first worksheet in the workbook
            try {
                package.Compatibility.IsWorksheets1Based = true;
                worksheet = package.Workbook.Worksheets[1];
            } catch (Exception e) {
                xLogger.Info("getFirstWorksheet", "::error:", e.Message);
                throw;
            }
            if (worksheet == null) {
                xLogger.Info("getFirstWorksheet", "::error:", "No worksheet found");
                throw new Exception("No worksheet found");
            }

            return worksheet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="excelfile"></param>
        /// <param name="dataXml"></param>
        /// <param name="columns">Parent element of the list of column elements</param>
        /// <param name="columnElementName">Name of the column element in the list of columns (eg 'subwf_obj')</param>
        private void degenExcel(FileInfo excelfile, XmlElement columns, string columnElementName) {
			xLogger.Debug("degenExcel", "excelfile", excelfile);

			using (ExcelPackage package = new ExcelPackage(excelfile)) {
                ExcelWorksheet worksheet = getFirstWorksheet(package);

                // Read first row - assume these are column names and match columns
                int cols, col = 1;
				while (true) {
					try {
						string val = worksheet.Cells[1, col].Value.ToString().Trim();
						xLogger.Debug("degenExcel", "col", col, "val", val);

						XmlElement columnElement = columns.SelectSingleNode(String.Format("{0}[prompt = '{1}']", columnElementName, val)) as XmlElement;
						if (columnElement != null) {
							columnElement.SetAttribute("column", col.ToString());
						} 

					} catch {
						xLogger.Debug("degenExcel::ends:", "col", col);
						break;
					}
					col++;
					if (col > worksheet.Cells.Columns) {	// safety
						break;
					}
				}
				cols = col;

				// Now read rows based on above attributes (columns), stop on first row with blank first cell
				XmlElement item;
				int row = 2;
				while (true) {
					try {
						// Row with first column null signifies end of data
						if (worksheet.Cells[row, 1].Value == null) {
							xLogger.Debug("degenExcel::ends.null:", "row", row);
							break;
						}
						// or... row with first column blank signifies end of data
						if (String.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Value.ToString())) {
							xLogger.Debug("degenExcel::ends.blank:", "row", row);
							break;
						}

						item = AddNode(_Root, "item", "") as XmlElement;
						item.SetAttribute("row", (row - 1).ToString());
						item.SetAttribute("status", "notok");

						foreach(XmlNode columnNode in columns.SelectNodes(columnElementName)) {
							XmlElement columnElement = columnNode as XmlElement;

							if (!String.IsNullOrEmpty(columnElement.GetAttribute("column"))) {
								col = Int32.Parse(columnElement.GetAttribute("column"));
								XmlElement value = columnElement.SelectSingleNode("value") as XmlElement;
								string nme = value.InnerText;
								string typ = value.GetAttribute("type");
								string fmt = value.GetAttribute("format");
								//xLogger.Debug("degenExcel", "row", row, "col", col, "nme", nme, "typ", typ, "fmt", fmt);

								object obj = worksheet.Cells[row, col].Value;
								string val = String.Empty;
								if (obj != null) {
									xLogger.Debug("degenExcel", "type", obj.GetType().ToString());
									switch (typ) {
										case "time":
											DateTime tm = DateTime.FromOADate((Double)obj);
											val = (!String.IsNullOrEmpty(fmt))? tm.ToString(fmt) : tm.ToString("HH:mm:ss");
											break;
										case "datetime":
											DateTime dt = ((DateTime)obj);
											val = (!String.IsNullOrEmpty(fmt))? dt.ToString(fmt) : dt.ToString("s");
											break;
										default:
											val = obj.ToString();
											break;
									}

								}
								//xLogger.Debug("degenExcel", "val", val);

								item.SetAttribute(nme, val);
							}
						}
						item.SetAttribute("status", "ok");
					} catch (Exception e) {
						xLogger.Debug("degenExcel::ends.error:", "row", row, "error", e.Message);
						break;
					}
					row++;
					if (row > worksheet.Cells.Rows) {	// safety
						break;
					}
				}
			}
		}
		
	}
}
