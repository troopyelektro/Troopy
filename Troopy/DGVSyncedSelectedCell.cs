using System;
using System.Data;
using System.Windows.Forms;

namespace Troopy
{
	public class DGVSyncedSelectedCell
	{
		private DGVSyncedGrid g;
		private DataGridViewCell c;
		
		private int rowIndex;
		public int RowIndex{
			get{
				return this.rowIndex;
			}
		}
		
		private int columnIndex;
		public int ColumnIndex{
			get{
				return this.columnIndex;
			}
		}
		
		public DGVSyncedSelectedCell(DGVSyncedGrid g, DataGridViewCell c)
		{	
			this.g = g;
			this.c = c;
			 
			// Column Index
			this.columnIndex = c.ColumnIndex;
			
			// Row index
			this.rowIndex = this.g.rowSource(c.RowIndex);
		}
	}
}
