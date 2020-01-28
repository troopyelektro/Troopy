/*
 * Created by SharpDevelop.
 * User: Ing. Petr Rösch
 * Date: 10.07.2019
 * Time: 6:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Troopy
{
	partial class frmException
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Label lblException;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmException));
            this.btnClose = new System.Windows.Forms.Button();
            this.lblException = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(306, 74);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(146, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Zavřít";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnCloseClick);
            // 
            // lblException
            // 
            this.lblException.AutoSize = true;
            this.lblException.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblException.Location = new System.Drawing.Point(12, 9);
            this.lblException.Name = "lblException";
            this.lblException.Size = new System.Drawing.Size(440, 44);
            this.lblException.TabIndex = 1;
            this.lblException.Text = "Došlo k neočekávané chybě v programu,\r\ndoporučujeme předat vytvořený log vývojaři" +
    ".";
            // 
            // frmException
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 112);
            this.Controls.Add(this.lblException);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmException";
            this.Text = "Neočekávaná chyba";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
