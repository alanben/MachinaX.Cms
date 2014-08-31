using System;
using System.Web;
using System.Xml;

using clickclickboom.machinaX.blogX;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2010-07-06
	Status:		release	
	Version:	4.0.2
	Build:		20140112
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20100706:	Started.
	20111213:	Added SERVICEX
	20140102:	Added LINK_DESTINATION
	20140112:	Added ListOutputStyle
	---------------------------------------------------------------------------	*/

namespace clickclickboom.machinaX.blogX.cmsX {

	/// <summary>Enumeration for style used for list output in proxies</summary>
	public enum ListOutputStyle {
		XmlTransform = 1	// Xml format, output via Xsl
		, XmlDirect = 2		// Xml format, output directly
		, Datatable = 3		// JSON format, suitable for Datatables.net
	}
    /// <summary>
    /// This is a static utility class to encapsulte common profile information (properties and constants).
    /// Typically used by any broker or display class
    /// </summary>
    public static class Cms {

		//General constants
		public const string APPLICATION_ROOT = "AwardX_";
		public const string CONFIG_ROOT = "CmsX";
		public const string CONFIG_ELEM = "CmsX";
		public const string SELECT_RESULT = "Result";
		public const string SELECT_RESULT_CODE = "Result/Result_Code";
		public const string SELECT_RESULT_DESC = "Result/Description";
		public const string SELECT_ITEMS = "//items";
		public const string SELECT_ITEMS_ROWS = "//items/@rows";
		public const string SELECT_ITEMSITEM = "//item";
		public const string SELECT_ITEMSITEM_ID = "//items/item[@id='{0}']";
		public const string SELECT_ITEMSITEM_NOTID = "//items/item[not(@id='{0}')]";
		public const string SELECT_ITEMSITEM_ROW = "//items/item[@id='{0}']/@row";

		public const string SERVICEX = "ServiceX";
		public const string SELECT_SERVICEX_ITEM = "ServiceX/items/item";
		public const string SELECT_ALL_ITEMS = "//items";
		public const string SELECT_ALL_ITEM = "//item";
		
		public const string ITEMS = "items";
		public const string ITEM = "item";
		public const string ITEM_LAST = "item[last()]";
		public const string SELECT_FIELD_NAME = "//fields/fld[@name='{0}']";

		public const string DEFAULT_COOKIE = "True";
		public const string DEFAULT_CHECK = "True";
		public const string DEFAULT_SKIN = "white";
		public const string DEFAULT_WIZARD = "yes";
		public const string DEFAULT_LISTOUTPUTSTYLE = "XmlTransform";

		public const string BLOGX_LISTOUTPUTSTYLE = "BlogX[@id='{0}']/ListOutputStyle";

		// Public Cookie constants
		public const string COOKIES = "CmsX";
		public const string COOKIE_SKIN = "Skin";
		public const string COOKIE_TOKEN = "TokenX";
		public const string COOKIE_WIZARD = "Wizard";
		public const string COOKIE_CHECK = "CheckCookieToken";
		public const int COOKIE_EXPIRY_YEARS = 5;

		// Public Profile name constants
		public const string PROFILE_SITES = "BlogXSites";
		public const string PROFILE_SITE_ID = "BlogXID";
		public const string PROFILE_SITE_SPACE = "BlogXSpace";
		public const string PROFILE_SITES_ALL = "SitesFullList";
		public const string PROFILE_TOKEN = "PXToken";
		public const string PROFILE_TOPICS_BLOGSPACE = "blogSpace";	// space for topics drop-down / selection

		public const string LINK_DESTINATION = "DestinationLink";
		
		// Profile patterns
		public const string PATTERN_PUBLIC = "public";
		public const string PATTERN_CUSTOMER = "customer";
		public const string PATTERN_ADMINUSER = "adminuser";
	}
}
