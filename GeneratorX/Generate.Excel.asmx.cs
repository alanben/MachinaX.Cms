/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2015-03-11
	Status:		release	
	Version:	4.0.2
	Build:		20190818
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20150311:	Started
	20190818:	Make cell value '' if data not found (was "undefined")
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
	/// Excel Generator web service
	/// </summary>
	[WebService(Name = "GenerateExcel",
				Namespace = "http://www.clickclickBOOM.com/MachinaX/GenerateX",
				Description = "GenerateExcel Web Service")]
	public class GenerateExcel : GenerateBase {

		private const string CONFIG_ID = "Excel";

		/// <summary>Constructor</summary>
		public GenerateExcel() : base("GenerateExcel", CONFIG_ID) {
		}
		public GenerateExcel(string RootName) : base(RootName, CONFIG_ID) {
		}

		/// <summary>
		/// Generate a Excel file from a XmlDocument with items/item
        /// </summary>
		/// <param name="ExcelXml">A simple XmlDocument containing items/item</param>
		/// <param name="FileName">The excel file (fullpath / name)</param>
		[WebMethod(Description = "Generate a Excel file from a XmlDocument with items/item")]
		public XmlDocument Generate(XmlDocument Data, string FileName) {
			xLogger.Debug("Generate", "Data", Data);
			try {
				AddOk();
				generateExcel(Data, null, FileName, null, false);
				xLogger.Debug("Generate:ok");
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
            return (Result);
        }

		/// <summary>
		/// Generate a Excel file from a XmlDocument with items/item
		/// </summary>
		/// <param name="ExcelXml">A simple XmlDocument containing items/item</param>
		[WebMethod(Description = "Generate a Excel file from a XmlDocument with items/item")]
		public XmlDocument GenerateXml(string Data, string FileName) {
			xLogger.Debug("GenerateXml");
			try {
				XmlDocument ExcelDoc = new XmlDocument();
				ExcelDoc.LoadXml(Data);
				Generate(ExcelDoc, FileName);

				xLogger.Debug("GenerateXml:ok");
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		/// <summary>
		/// Generate a Excel file from a XmlDocument with items/item and columns by subobjs/subwf_obj
		/// </summary>
		/// <param name="ExcelXml">A simple XmlDocument containing items/item</param>
		/// <param name="ExcelXml">A simple XmlDocument containing subobjs/subwf_obj</param>
		[WebMethod(Description = "Generate a Excel file from a XmlDocument with items/item and columns by subobjs/subwf_obj")]
		public XmlDocument GenerateColumns(XmlDocument Data, XmlDocument Columns, string FileName) {
			xLogger.Debug("GenerateColumns", "Data", Data, "Columns", Columns);
			try {
				AddOk();
				generateExcel(Data, Columns, FileName, null, false);
				xLogger.Debug("GenerateColumns:ok");
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		/// <summary>
		/// Generate a Excel file from a XmlDocument with items/item and columns by subobjs/subwf_obj
		/// </summary>
		/// <param name="ExcelXml">A simple XmlDocument containing items/item</param>
		/// <param name="ExcelXml">A simple XmlDocument containing subobjs/subwf_obj</param>
		[WebMethod(Description = "Generate a Excel file from a XmlDocument with items/item and columns by subobjs/subwf_obj")]
		public XmlDocument GenerateColumnsXml(string Data, XmlDocument Columns, string FileName) {
			xLogger.Debug("GenerateColumnsXml");
			try {
				XmlDocument ExcelDoc = new XmlDocument();
				ExcelDoc.LoadXml(Data);
				GenerateColumns(ExcelDoc, Columns, FileName);

				xLogger.Debug("GenerateColumnsXml:ok");
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		/// <summary>
		/// Generate a Excel file from a XmlDocument with items/item and columns by subobjs/subwf_obj
		/// </summary>
		/// <param name="ExcelXml">A simple XmlDocument containing items/item</param>
		/// <param name="ExcelXml">A simple XmlDocument containing subobjs/subwf_obj</param>
		//[WebMethod(Description = "Generate a Excel file from a XmlDocument with items/item and columns by subobjs/subwf_obj")]
		public XmlDocument XmlToExcel(XmlElement Data, XmlNodeList Columns, string FileName, bool CreateCSV) {
			xLogger.Debug("XmlToExcel", "Data", (Data == null)? "null" : Data.OuterXml, "Columns", Columns.Count, "CreateCSV", CreateCSV);
			try {
				AddOk();
				generateExcel(Data, Columns, FileName, null, CreateCSV);
				xLogger.Debug("XmlToExcel:ok");
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		public XmlDocument XmlToExcel(XmlElement Data, XmlNodeList Columns, string FileName, HttpResponse Response, bool CreateCSV) {
			xLogger.Debug("XmlToExcel", "Data", Data, "Columns", Columns.Count, "CreateCSV", CreateCSV);
			try {
				AddOk();
				generateExcel(Data, Columns, FileName, Response, CreateCSV);
				xLogger.Debug("XmlToExcel:ok");
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		public XmlDocument XmlToExcelSections(XmlNode Data, string FileName, HttpResponse Response) {
			xLogger.Debug("XmlToExcelSections", "Data", Data);
			try {
				AddOk();
				generateExcel(Data, FileName, Response);
				xLogger.Debug("XmlToExcelSections:ok");
			} catch (x_exception e) {
				_AddError(e);
			} catch (System.Exception e) {
				_AddError(e);
			}
			return (Result);
		}

		private void generateExcel(XmlDocument excelXml, XmlDocument columnXml, string filename, HttpResponse response, bool createCSV) {
			if (columnXml == null) {
				generateExcel(excelXml, null, filename, response, createCSV);
			} else {
				generateExcel(excelXml.DocumentElement, columnXml.SelectNodes("//subobjs/subwf_obj"), filename, response, createCSV);
			}
		}
		private void generateExcel(XmlElement dataXml, XmlNodeList columns, string filename, HttpResponse response, bool createCSV) {
			xLogger.Debug("generateExcel", "::filename:", filename);
			try {
				DirectoryInfo outputDir = new DirectoryInfo(ExcelDir);
				string filepath = String.Format("{0}\\{1}.xlsx", outputDir.FullName, String.IsNullOrEmpty(filename) ? Token : filename);
				xLogger.Debug("generateExcel", "::filepath:", filepath);

				FileInfo excelfile = new FileInfo(filepath);
				if (excelfile.Exists) {
					excelfile.Delete();  // ensures we create a new workbook
					excelfile = new FileInfo(filepath);
				}
				xLogger.Debug("generateExcel", "::columns:", (columns == null) ? "null" : columns.Count.ToString());

				if (columns == null) {
					createExcel(excelfile, dataXml, response, createCSV);
				} else {
					createExcel(excelfile, dataXml, columns, response, createCSV);
				}
				AddNode("filepath", excelfile.FullName);
			} catch (Exception e) {
				xLogger.Debug("generateExcel", "::error:", e.Message, "::trace:", e.StackTrace);
				throw new x_exception("generateExcel", String.Concat("generateExcel", e.Message));
			}
			xLogger.Debug("generateExcel:ok");
		}

		private void createExcel(FileInfo excelfile, XmlElement dataXml, HttpResponse response, bool createCSV) {
			xLogger.Debug("createExcel", "excelfile", excelfile);

			using (ExcelPackage package = new ExcelPackage(excelfile)) {

				if (response != null) {
					initResponse(response, excelfile.Name);
				}

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Data");

				int row = 1, col = 1;
				XmlElement firstrow = dataXml.SelectSingleNode("//items/item") as XmlElement;
				foreach (XmlAttribute attribute in firstrow.Attributes) {
					worksheet.Cells[row, col].Value = attribute.Name;
					//xLogger.Debug("generateExcel", "attribute", attribute.Name);
					col++;
				}
				row++;

				foreach (XmlNode item in dataXml.SelectNodes("//items/item")) {
					XmlElement itemel = item as XmlElement;
					col = 1;
					foreach (XmlAttribute attribute in itemel.Attributes) {
						worksheet.Cells[row, col].Value = attribute.Value;
						col++;
					}
					row++;
				}

				worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

				if (createCSV) {
					EpplusCsvConverter.ConvertToCsv(package, excelfile);
				}

				if (response != null) {
					package.SaveAs(response.OutputStream);
					response.Flush();
					response.End();
				} else {
					package.Save();
				}
			}
		}

		private void createExcel(FileInfo excelfile, XmlElement dataXml, XmlNodeList columns, HttpResponse response, bool createCSV) {
			xLogger.Debug("createExcel", "excelfile", excelfile, "::response:", (response == null)? "null": "is response");

			using (ExcelPackage package = new ExcelPackage(excelfile)) {

				if (response != null) {
					initResponse(response, excelfile.Name);
				}

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Data");

				int row = 1, col = 1;
				foreach (XmlNode column in columns) {
					worksheet.Cells[row, col].Value = column.SelectSingleNode("prompt").InnerText;
					col++;
				}
				row++;

				foreach (XmlNode item in dataXml.SelectNodes("//items/item")) {
					XmlElement itemel = item as XmlElement;
					col = 1;
					foreach (XmlNode column in columns) {
						try {
							XmlElement valel = column.SelectSingleNode("value") as XmlElement;
							string value;
							string valuename = valel.InnerText;
							string valueformat = valel.GetAttribute("format");
							//xLogger.Debug("createExcel", "valuename", valuename, "valueformat", valueformat);

							XmlNode attNode = itemel.SelectSingleNode(String.Concat("@", valuename));
							// replace above with this in case the node required is not found should be, but...
							// rather let error reflect in the data than cause exception or hide issue
							if (attNode == null) {
								attNode = itemel.SelectSingleNode(valuename);
							}
							if (attNode != null) {
								if (valueformat == "quoted" || valueformat == "quote") {
									if (valel.GetAttribute("linebreaks") == "no") {
										value = attNode.InnerText.Replace("\r\n", " ").Replace('\n', ' ');
									} else {
										value = attNode.InnerText.Replace("\"", "");
									}
								} else if (valueformat == "xhtml") {
									string xhtml = "";
									try {
										xLogger.Debug("createExcel", "::attNode:", attNode.InnerText);

										xhtml = attNode.InnerText.Replace("&amp;amp;", "&amp;").Replace("&lt;", " ").Replace("&gt;", " ");
										xLogger.Debug("createExcel", "::xhtml:", xhtml);

										xhtml = HttpUtility.HtmlDecode(xhtml);
										xhtml = xhtml.Replace("&", "&amp;");

										attNode.InnerXml = xhtml;
										attNode.InnerXml = attNode.InnerXml.Replace("&nbsp;", "&#160;");
										((XmlElement)attNode).SetAttribute("decoded", "yes");
										//value = String.Concat("\"", attNode.InnerText.Replace('\"', '\''), "\"");
										value = attNode.InnerText;
									} catch (XmlException) {
										xLogger.Debug("createExcel", "::badxhtml:", xhtml);
										value = "error: badly formed html for description";
									}
								} else {
									//value = attNode.InnerText.Replace(",", ";").Replace("\r\n", " ").Replace('\n', ' ');
									value = attNode.InnerText;
								}
							} else {
								value = "";    // was "undefined"
							}
							// First set value
							worksheet.Cells[row, col].Value = value;
							// Next set format
							switch (valueformat) {
								case "integer":
									worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0";
									break;
								case "date":
									worksheet.Cells[row, col].Style.Numberformat.Format = "YYYY-MM-DD";
									break;
								case "datetime":
									worksheet.Cells[row, col].Style.Numberformat.Format = "YYYY-MM-DD mm:ss";
									break;
								case "decimal":
									worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0.00";
									break;
								case "currency":
									worksheet.Cells[row, col].Style.Numberformat.Format = "R #,##0.00";
									break;

								// The following were used originally for forcing the text in csv to be recognised as text (vs numbers)
								case "text":
								case "quote":
								case "force":
								case "quoted":
								default:
									break;
							}
							col++;
						} catch (Exception e) {
							xLogger.Debug("createExcel", "::Error:", e.Message, "::column:", column.OuterXml, "::itemel:", itemel.OuterXml);
						}
					}
					row++;
				}

				if (createCSV) {
					EpplusCsvConverter.ConvertToCsv(package, excelfile);
				}

				worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells
				if (response != null) {
					package.SaveAs(response.OutputStream);
					response.Flush();
					response.End();
				} else {
					package.Save();
				}
			}
			xLogger.Debug("createExcel:ok");
		}

		private void initResponse(HttpResponse response, string filename) {
			response.Clear();
			response.ClearHeaders();
			response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			response.AddHeader("content-disposition", String.Format("attachment;  filename={0}", filename));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataNode"></param>
		/// <param name="filename"></param>
		/// <param name="response"></param>
		/// <example>
		///		<section id="6">
		/// 		<items>
		/// 			<item count="Total" event_36="Saturday" event_37="Sunday">On Hold</item>
		/// 			<item id="1" count="0" event_36="0" event_37="0">VIP</item>
		/// 			<item id="3" count="0" event_36="0" event_37="0">Grand</item>
		/// 			<item id="7" count="0" event_36="0" event_37="0">Grand 2nd</item>
		/// 			<item id="15" count="0" event_36="0" event_37="0">Gallery</item>
		/// 			<item id="5" count="0" event_36="0" event_37="0">Student</item>
		/// 			<item id="8" count="0" event_36="-" event_37="-">Seminar</item>
		/// 			<item id="11" count="0" event_36="-" event_37="-">Seminar Student</item>
		/// 			<item tot="yes" count="0" event_36="0" event_37="0">Total</item>
		/// 		</items>
		/// 	</section>
		/// </example>
		private void generateExcel(XmlNode dataNode, string filename, HttpResponse response) {
			xLogger.Debug("generateExcel");

			using (ExcelPackage package = new ExcelPackage()) {
				// Only written to HttpResponse
				initResponse(response, filename);

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Data");
				xLogger.Debug("generateExcel", "::sections:", dataNode.SelectNodes("//sections").Count);

				int row = 1;
				foreach (XmlNode sect in dataNode.SelectNodes("//section")) {
					XmlElement section = sect as XmlElement;

					// First row
					int col = 1;
					XmlElement firstrow = section.SelectSingleNode("items/item") as XmlElement;
					
					// First column is item text
					worksheet.Cells[row, col].Value = firstrow.InnerText;
					col++;

					// Attributes are subsequent columns
					foreach (XmlAttribute attribute in firstrow.Attributes) {
						worksheet.Cells[row, col].Value = attribute.Value;
						//xLogger.Debug("generateExcel", "::row-col:", row, "-", col, "::attribute:", attribute.Name, ":", attribute.Value);
						col++;
					}
					row++;

					// Rows
					foreach (XmlNode item in section.SelectNodes("items/item[position() > 1]")) {
						XmlElement itemel = item as XmlElement;
						col = 1;
						
						// First column is item text
						worksheet.Cells[row, col].Value = item.InnerText;
						col++;
						foreach (XmlAttribute attribute in firstrow.Attributes) {
							//xLogger.Debug("generateExcel", "::row-col:", row, "-", col, "::data:", attribute.Name, ":", itemel.GetAttribute(attribute.Name));
							int val = 0;
							if (Int32.TryParse(itemel.GetAttribute(attribute.Name), out val)) {
								worksheet.Cells[row, col].Value = val;
							} else {
								worksheet.Cells[row, col].Value = itemel.GetAttribute(attribute.Name);
							}

							col++;
						}
						row++;
					}
					row++;
				}

				worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells
				// Only written to HttpResponse
				package.SaveAs(response.OutputStream);
				response.Flush();
				response.End();
			}

		}


/*
		/// <summary>Converts the dataXml into CSVs </summary>
		/// <remarks>Note that this was taken from AwardsX.xmlToCsv, but differs in that the first row is assumed not to have data but used to get column names only</remarks>
		/// <returns>Returns a string containing the Xml nodes in CSV format</returns>
		private void xmlToCSV(XmlElement Data, XmlNodeList columns, string csvFileName) {
			string xpath_nodes = ".//child::*";
			// Check for data and throw exception so message can be served in popup window
			if (Data.SelectSingleNode(xpath_nodes) == null)
				throw new x_exception("error_csv_nodata", String.Concat(error_csv, error_csv_nodata, csvFileName));

			string file = (csvFileName == null) ? Guid.NewGuid().ToString() : String.Concat(csvFileName, "-", DateTime.Now.ToShortDateString());
			uipage.Response.Clear();
			uipage.Response.ClearHeaders();
			uipage.Response.AddHeader("Content-Disposition", String.Concat("filename=", file, ".csv"));
			uipage.Response.ContentType = "text/csv";

			//uipage.Response.ContentEncoding = Encoding.GetEncoding(1252);
			// rather use the following (see http://forums.asp.net/t/1651825.aspx/2/10?Charset+encoding+for+Polish+French+Germany+Russia)
			uipage.Response.ContentEncoding = System.Text.Encoding.UTF8;
			byte[] BOM = { 0xEF, 0xBB, 0xBF };
			uipage.Response.BinaryWrite(BOM);

			int x = 0;
			foreach (XmlNode column in columns) {
				string name = column.SelectSingleNode("prompt").InnerText;
				uipage.Response.Write((x == 0) ? "" : TOKEN_COMMA);
				uipage.Response.Write(name);
				//Logger.Debug(String.Concat(logid, " COLUMNS::", column.OuterXml)); 
				x++;
			}
			uipage.Response.Write("\n");
			// Get data rows
			XmlNodeList DataList = Data.SelectNodes("child::*");
			Logger.Debug(String.Concat("xmlToCsv::DataList:", DataList.Count.ToString()));
			//for (int i = 0; i < DataList.Count; i++) {
			//    XmlNode itemNode = DataList.Item(i);
			foreach (XmlNode itemNode in DataList) {
				int j = 0;
				foreach (XmlNode column in columns) {
					try {
						XmlElement valel = column.SelectSingleNode("value") as XmlElement;
						string value;
						string row = valel.InnerText;
						//string format = valel.GetAttribute("format");
						string valueformat = valel.GetAttribute("format");
						XmlNode attNode = itemNode.SelectSingleNode(String.Concat("@", row));
						// replace above with this in case the node required is not found should be, but...
						// rather let error reflect in the data than cause exception or hide issue
						if (attNode == null) {
							attNode = itemNode.SelectSingleNode(row);
						}
						if (attNode != null) {
							if (valueformat == "quoted" || valueformat == "quote") {
								if (valel.GetAttribute("linebreaks") == "no") {
									value = attNode.InnerText.Replace("\r\n", " ").Replace('\n', ' ');
								} else {
									value = attNode.InnerText;
								}
							} else if (valueformat == "xhtml") {
								string xhtml = "";
								try {
									Logger.Debug(String.Concat("xmlToCsv::attNode:", attNode.InnerText));
									xhtml = attNode.InnerText.Replace("&amp;amp;", "&amp;").Replace("&lt;", " ").Replace("&gt;", " ");
									Logger.Debug(String.Concat("xmlToCsv::xhtml:", xhtml));
									xhtml = HttpUtility.HtmlDecode(xhtml);
									xhtml = xhtml.Replace("&", "&amp;");

									attNode.InnerXml = xhtml;
									attNode.InnerXml = attNode.InnerXml.Replace("&nbsp;", "&#160;");
									((XmlElement)attNode).SetAttribute("decoded", "yes");
									//value = attNode.InnerText.Replace(",", ";").Replace("\r\n", " ").Replace('\n', ' ');
									value = String.Concat("\"", attNode.InnerText.Replace('\"', '\''), "\"");
								} catch (XmlException) {
									Logger.Debug(String.Concat("xmlToCsv::badxhtml:", xhtml));
									value = "error: badly formed html for description";
								}
							} else {
								value = attNode.InnerText.Replace(",", ";").Replace("\r\n", " ").Replace('\n', ' ');
							}
						} else {
							value = "undefined";
						}
						uipage.Response.Write((j == 0) ? "" : TOKEN_COMMA);
						switch (valueformat) {
							case "text":
								uipage.Response.Write(String.Concat("'", value));
								break;
							case "quote":
								uipage.Response.Write(String.Concat("\"", value, "\""));
								break;
							case "force":	// putting an '=' before the double quotes forces the data to be text. (Excel only?)
								uipage.Response.Write(String.Concat("=\"", value, "\""));
								break;
							case "quoted":
							default:
								uipage.Response.Write(value);
								break;
						}
						j++;
					} catch (Exception e) {
						Logger.Debug(String.Concat("xmlToCsv::Error:", e.Message, "::column:", column.OuterXml, "::itemNode:", itemNode.OuterXml));
					}
				}
				uipage.Response.Write("\n");
			}
			Logger.Debug(String.Concat("xmlToCsv::Data:", Data.OuterXml));
			Logger.Debug(String.Concat("xmlToCsv::finished:ok"));
			Logger.Debug(String.Concat("xmlToCsv::flushing..."));
			uipage.Response.Flush();
			Logger.Debug(String.Concat("xmlToCsv::ending..."));
			uipage.Response.End();
		}
*/

/*
		private string RunSample1(DirectoryInfo outputDir) {
			FileInfo newFile = new FileInfo(outputDir.FullName + @"\sample1.xlsx");
			if (newFile.Exists) {
				newFile.Delete();  // ensures we create a new workbook
				newFile = new FileInfo(outputDir.FullName + @"\sample1.xlsx");
			}
			using (ExcelPackage package = new ExcelPackage(newFile)) {
				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Inventory");
				//Add the headers
				worksheet.Cells[1, 1].Value = "ID";
				worksheet.Cells[1, 2].Value = "Product";
				worksheet.Cells[1, 3].Value = "Quantity";
				worksheet.Cells[1, 4].Value = "Price";
				worksheet.Cells[1, 5].Value = "Value";

				//Add some items...
				worksheet.Cells["A2"].Value = 12001;
				worksheet.Cells["B2"].Value = "Nails";
				worksheet.Cells["C2"].Value = 37;
				worksheet.Cells["D2"].Value = 3.99;

				worksheet.Cells["A3"].Value = 12002;
				worksheet.Cells["B3"].Value = "Hammer";
				worksheet.Cells["C3"].Value = 5;
				worksheet.Cells["D3"].Value = 12.10;

				worksheet.Cells["A4"].Value = 12003;
				worksheet.Cells["B4"].Value = "Saw";
				worksheet.Cells["C4"].Value = 12;
				worksheet.Cells["D4"].Value = 15.37;

				//Add a formula for the value-column
				worksheet.Cells["E2:E4"].Formula = "C2*D2";

				//Ok now format the values;
				using (var range = worksheet.Cells[1, 1, 1, 5]) {
					range.Style.Font.Bold = true;
					range.Style.Fill.PatternType = ExcelFillStyle.Solid;
					range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
					range.Style.Font.Color.SetColor(Color.White);
				}

				worksheet.Cells["A5:E5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
				worksheet.Cells["A5:E5"].Style.Font.Bold = true;

				worksheet.Cells[5, 3, 5, 5].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2, 3, 4, 3).Address);
				worksheet.Cells["C2:C5"].Style.Numberformat.Format = "#,##0";
				worksheet.Cells["D2:E5"].Style.Numberformat.Format = "#,##0.00";

				//Create an autofilter for the range
				worksheet.Cells["A1:E4"].AutoFilter = true;

				worksheet.Cells["A2:A4"].Style.Numberformat.Format = "@";   //Format as text

				//There is actually no need to calculate, Excel will do it for you, but in some cases it might be useful. 
				//For example if you link to this workbook from another workbook or you will open the workbook in a program that hasn't a calculation engine or 
				//you want to use the result of a formula in your program.
				worksheet.Calculate();

				worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

				// lets set the header text 
				worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" Inventory";
				// add the page number to the footer plus the total number of pages
				worksheet.HeaderFooter.OddFooter.RightAlignedText =
					string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
				// add the sheet name to the footer
				worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
				// add the file path to the footer
				worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;

				worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:2"];
				worksheet.PrinterSettings.RepeatColumns = worksheet.Cells["A:G"];

				// Change the sheet view to show it in page layout mode
				worksheet.View.PageLayoutView = true;

				// set some document properties
				package.Workbook.Properties.Title = "Invertory";
				package.Workbook.Properties.Author = "Jan Källman";
				package.Workbook.Properties.Comments = "This sample demonstrates how to create an Excel 2007 workbook using EPPlus";

				// set some extended property values
				package.Workbook.Properties.Company = "AdventureWorks Inc.";

				// set some custom property values
				package.Workbook.Properties.SetCustomPropertyValue("Checked by", "Jan Källman");
				package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");
				// save our new workbook and we are done!
				package.Save();

			}

			return newFile.FullName;
		}
*/

	}
}
