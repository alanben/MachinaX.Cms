using System;
using System.Xml;

//using clickclickboom.machinaX;
//using XXBoom.MachinaX.BlogX.CmsX;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2011-10-03
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

namespace XXBoom.MachinaX.BlogX.CmsX {
    /// <summary>
    /// Class to handle add, edit and delete of Client Defaults.
    /// </summary>
	class YyyyUpload : CmsXSubItem {
		#region Invisible properties
		#endregion

		#region Constant name strings
		#endregion

		#region Visible properties
		XmlDocument uploadDoc;
		/// <summary></summary>
		public XmlDocument UploadDoc {
			get { return uploadDoc; }
			set { uploadDoc = value; }
		}

		XmlElement defaults;
		/// <summary></summary>
		public XmlElement Defaults {
			get { return defaults; }
			set { defaults = value; }
		}

		XmlElement defaultTypes;
		/// <summary></summary>
		public XmlElement DefaultTypes {
			get { return defaultTypes; }
			set { defaultTypes = value; }
		}
		#endregion

		#region Constructors/Destructors
		public YyyyUpload(XLogger xlogger, XmlDocument uploaddoc, XmlElement deftypes, XmlElement yyyy) : base(xlogger) {
			xLogger.Debug("constructor");
			UploadDoc = uploaddoc;
			DefaultTypes = deftypes;

			Defaults = yyyy.SelectSingleNode(Importer.DEFAULTS) as XmlElement;
			xLogger.Debug("constructor", "::Defaults:", (Defaults == null) ? "null" : Defaults.OuterXml);

			xLogger.Debug("constructor:ok");
		}
		#endregion

		#region Public methods
		/// <summary></summary>
		public void Process(bool New, string Record) {
			xLogger.Debug("Process");
			Initialise(Record);
			if (!IsBad && !IsNew) {
				string sel = String.Format("{0}[@id = '{1}']", Importer.DEFAULT, ID);
				Item = Defaults.SelectSingleNode(sel) as XmlElement;
			}
			xLogger.Debug("Process", "::Item:", (Item == null) ? "null" : Item.OuterXml);
			base.Process(New);
		}
		#endregion

		#region Protected methods
		/// <summary>Add subitem</summary>
		protected override void _Add() {
			xLogger.Debug("_Add");
			// <default row="0" id="1" value="Free" status="ok" error="" desc="Contract type" unit="String" />
			XmlElement deflt = UploadDoc.CreateElement(Importer.DEFAULT);

			deflt.SetAttribute("row", (Defaults.SelectNodes(Importer.DEFAULT).Count + 1).ToString());
			deflt.SetAttribute("id", ID);
			update(deflt);
			Defaults.AppendChild(deflt);
			xLogger.Debug("_Add:ok");
		}
		/// <summary>Edit subitem</summary>
		protected override void _Edit() {
			xLogger.Debug("_Edit");
			update(Item);
			xLogger.Debug("_Edit:ok");
		}
		/// <summary>Delete subitem</summary>
		protected override void _Delete() {
			xLogger.Debug("_Delete");
			Defaults.RemoveChild(Item);
			xLogger.Debug("_Delete:ok");
		}
		#endregion

		#region Private utility methods
		private void update(XmlElement deflt) {
			xLogger.Debug("update", "::deflt:", (deflt == null) ? "null" : deflt.OuterXml);
			deflt.SetAttribute("value", _GetValue("value"));
			deflt.SetAttribute("status", "ok");
			deflt.SetAttribute("error", "");
			XmlElement type = DefaultTypes.SelectSingleNode(String.Format("//item[@id={0}]", ID)) as XmlElement;
			deflt.SetAttribute("desc", type.GetAttribute("desc"));
			//deflt.SetAttribute("unit", type.GetAttribute("unit"));
		}
		#endregion
	}
}