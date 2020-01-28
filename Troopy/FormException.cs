/*
 * Created by SharpDevelop.
 * User: Ing. Petr Rösch
 * Date: 10.07.2019
 * Time: 6:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Troopy
{
	/// <summary>
	/// Description of Troopy_FormException.
	/// </summary>
	public partial class frmException : Form
	{
		public frmException()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}
		void BtnCloseClick(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
