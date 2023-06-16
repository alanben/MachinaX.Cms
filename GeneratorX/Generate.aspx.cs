using System.Configuration;
using System.Web.Services;
using System.Xml;
using System;

using XXBoom.MachinaX;
using XXBoom.MachinaX.BlogX;
using WebSupergoo.ABCpdf8;

/*	-----------------------------------------------------------------------	
	Copyright:	clickclickBOOM cc
	Author:		Alan Benington
	Started:	2013-10-30
	Status:		release	
	Version:	4.0.2
	Build:		20131030
	License:	GNU General Public License
	-----------------------------------------------------------------------	*/

/*	-----------------------------------------------------------------------	
	Development Notes:
	==================
	20131030:	Started from LoeriesX.Generator
	-----------------------------------------------------------------------	*/

namespace XXBoom.MachinaX.GeneratorX {
	public class GenerateX : pageX {




		#region Constructors/Destructors
		/// <over_Loads>Constructor</over_Loads>
		/// <summary>Default constructor</summary>
		public GenerateX() : this ("GenerateX") {
		}
		/// <summary>Constructor supplyiong class name identifier</summary>
		/// <param name="lbl">Name to be used in error and debugging messages</param>
		public GenerateX(string lbl) : base(lbl) {
		}
		#endregion



	}
}
