using System;
using System.Xml;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2012-11-06
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20121106:	Started
	20140112:	Refactored constructor
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {
    /// <summary>
    /// Description of the class.
    /// <para>Additional information about the class</para>
    /// </summary>
	public class CmsXBrokerZzzz : CmsXBrokerBase {

		private const string logid = "CmsXBrokerZzzz.";

		private const string error_zzzz_get = "Error getting the zzzz: ";
		private const string error_zzzz_select = "Error selecting a zzzz: ";
		private const string error_zzzz_remove = "Error deleting a zzzz: ";

		/// <overloads>Constructor</overloads>
		/// <summary>Default constructor</summary>
		/// <param name="thispage">The web application Page object</param>
		public CmsXBrokerZzzz(CmsX thispage) : base(thispage, typeof(CmsXBrokerZzzz), logid) {
		}

		public override void Process(string type) {
			// Call base if we want teh default function of sending email
			//base.Process((type != "list"));
			xLogger.Info("Process:", type);
			switch (type) {

				// template methods
				case "get":
					get();
					break;
				case "select":
					select();
					break;
				case "remove":
					remove();
					break;
			}
        }

		public override void Process(string type, XmlElement content) {
			xLogger.Info("Process:", type);
			switch (type) {

				// template methods
				case "select":
					select(content);
					break;
			}
		}

		/// <summary>Zzzz.get</summary>
		private void get() {
			xLogger.Info("get:");
			try {

				xLogger.Debug("get:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_zzzz_get", String.Concat(error_zzzz_get, e.Message)));
			}
		}

		/// <summary>Zzzz.select</summary>
		private void select() {
			xLogger.Info("select:");
			try {

				xLogger.Debug("select:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_zzzz_select", String.Concat(error_zzzz_select, e.Message)));
			}
		}

		/// <summary>Zzzz.select</summary>
		private void select(XmlElement content) {
			xLogger.Info("select:");
			try {

				xLogger.Debug("select:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_zzzz_select", String.Concat(error_zzzz_select, e.Message)));
			}
		}

		/// <summary>Zzzz.remove</summary>
		private void remove() {
			xLogger.Info("remove:");
			try {

				xLogger.Debug("remove:ok");
			} catch (x_exception e) {
				throw e;
			} catch (Exception e) {
				throw (new x_exception("error_zzzz_remove", String.Concat(error_zzzz_remove, e.Message)));
			}
		}
	}
}