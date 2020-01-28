using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Troopy
{
	public class DGVSyncedGrid : Grid
	{
		//
		// DataGridView Storage
		//
		protected DataGridView dgv;
		public DataGridView Dgv{
			get{
				return this.dgv;
			}
			set{
				this.ea.clearEvents();
				this.dgv = value;
			}
		}
		
		public EventAdapter ea;
		
		// DataGridView Used
		public bool IsDGVUsed{
			get{
				if(this.dgv == null) {
					return false;
				} else {
					return true;
				}
			}
			set{
				if(value==false) {
					this.dgv = null;
				}
			}
		}
		
		// DataGridView Storage
		private DataGridView dgvStyle = new DataGridView();
		public DataGridView DgvStyle {
			get{
				return this.dgvStyle;
			}
			set{
				this.dgvStyle = value;
			}
		}
		
		
		//
		// Sync parameters
		//
		private string syncTable = "";
		public string SyncTable{
			get{
				return this.syncTable;
			}
			set{
				this.syncTable = value;
				// Generate DataGridView Style
				this.dgvStyle = new DataGridView();
				foreach(DataColumn c in this.getTable(this.syncTable).Columns) {
					this.dgvStyle.Columns.Add(c.ColumnName, c.ColumnName);
				}
				
				int rCnt = this.getRowCount(this.syncTable);
				if(rCnt > 0) {
					this.dgvStyle.Rows.Add(this.getRowCount(this.syncTable));
				}
			}
		}
		
		private string syncColumn = "";
		public string SyncColumn{
			get{
				return this.syncColumn;
			}
			set{
				this.syncColumn = value;
			}
		}
		
		public bool SyncParamsReady{
			get{
				if(this.IsDGVUsed && this.syncTable.Length>0 && this.syncColumn.Length>0) {
					return true;
				} else {
					return false;
				}
			}
		}
		
		private bool synced = false;
		public bool Synced{
			get{
				return this.synced;
			}
		}
		
		private List<string> syncValuesSource = new List<string>();
		public List<string> SyncValuesSource{
			get {
				return this.syncValuesSource;
			}
		}

		private List<string> syncValuesView = new List<string>();
		public List<string> SyncValuesView{
			get {
				return this.syncValuesView;
			}
		}

		//
		// Selected cells
		//
		public List<DGVSyncedSelectedCell> SelectedCells {
			get{
				List<DGVSyncedSelectedCell> l = new List<DGVSyncedSelectedCell>();
				if(this.IsDGVUsed) {
					foreach(DataGridViewCell dgvc in this.Dgv.SelectedCells) {
						DGVSyncedSelectedCell dtc = new DGVSyncedSelectedCell(this, dgvc);
						l.Add(dtc);
					}
				}
				return l;
			}
		}
		
		//
		// Content edit
		//
		public override Grid setCell(string table, int row, int col, string value)
		{
			base.setCell(table, row, col, value);
			if(this.IsDGVUsed && table==this.syncTable && this.Synced) {
				int rDgv = this.rowView(row);
				this.Dgv[col, rDgv].Value = value;
			}
			return this;
		}
		
		public override Grid setCell(string table, int row, string col, string value)
		{
			int ci = this.getColumnIndex(table, col);
			return this.setCell(table, row, ci, value);
		}
		
		//
		// Sweep
		//
		public static void sweep(DGVSyncedGrid g)
		{
			if(g != null) {
				g.ea.clearEvents();
			}
		}
		
		
		//
		// Constructor
		//
		public DGVSyncedGrid(DataGridView dg = null) : base()
		{
			this.ea = new EventAdapter();
			this.Dgv = dg;
		}
		
		//
		// Synchronize
		//
		public bool sync()
		{
			// Reset Synced property
			this.synced = false;
			this.ea.clearEvents();
			
			// Check if parameters are ready
			if(this.SyncParamsReady) {
				
				// It must be checked if DataTable with the column exists
				if(!this.tableExists(this.SyncTable)) {
					throw new DGVSyncParamsException(
						"Synchronizace není možná, protože tabulka " +
						this.SyncTable +
						" neexistuje."
					);
				}
				
				if(!this.columnExists(this.SyncTable, this.SyncColumn)) {
					throw new DGVSyncParamsException(
						"Synchronizace není možná, protože sloupec " +
						this.SyncColumn + " v tabulce " + this.SyncTable +
						" neexistuje."
					);
				}
				
				// It must be checked that in DataTable int selected column are all values unique
				string tblUnCheck = this.SyncTable + "_tableUniqueCheck";
				int unValCnt = this.createUnique(this.SyncTable, this.SyncColumn, tblUnCheck)
					.getRowCount(tblUnCheck);
				this.deleteTable(tblUnCheck);
				if(unValCnt != this.getRowCount(this.SyncTable)) {
					throw new DGVSyncParamsException(
						"Synchronizace není možná, protože sloupec " +
						this.SyncColumn + " v tabulce " + this.SyncTable +
						" obsahuje " + unValCnt.ToString() + " unikátních hodnot, ale " +
						"má " + this.getRowCount(this.SyncTable).ToString() + " řádků. " +
						"Počet musí být stejný."
					);
				}

				// Load Sync values
				this.syncValuesSource = this.getColumnValues(this.SyncTable, this.SyncColumn);
				this.syncValuesView = this.getColumnValues(this.SyncTable, this.SyncColumn);				
				
				// Show Data Table In DataGridView
				this.dt2dgv(this.Dgv, this.syncTable);
				
				// Refresh
				this.refresh();
				
				// Create on sort event
				this.ea.addEvent(
					this.Dgv,
					EventAdapter.etSorted,
					new EventHandler(this.DgvSort)
				);
				
				// Sync Completed
				this.synced = true;	
			}

			return this.Synced;
		}
		
		//
		// On Sort
		//
		void DgvSort(object sender, EventArgs e)
		{
			this.refresh();
		}
		
		
		//
		// Refresh
		//
		public DGVSyncedGrid refresh()
		{
			this.refreshSyncKeys();
			this.refreshStyle();
			return this;
		}
		
		
		public DGVSyncedGrid refreshStyle()
		{
			if(this.IsDGVUsed) {
				
				// Refresh Style
				int rCnt = this.getRowCount(this.syncTable);
				int cCnt = this.getColumnCount(this.SyncTable);
				
				for(int r=0; r<this.getRowCount(this.syncTable); r++) {
					int rDgv = this.rowView(r);
					
					for(int c=0; c<cCnt; c++) {
						DataGridViewCellStyle cs = this.DgvStyle.Rows[r].Cells[c].Style;
						Dgv.Rows[rDgv].Cells[c].Style = null;
						Dgv.Rows[rDgv].Cells[c].Style.ApplyStyle(cs);
					}
				}
			}
			return this;
		}
		
		public DGVSyncedGrid refreshStyleCell(int r, int c)
		{
			if(this.IsDGVUsed) {
				int rDgv = this.rowView(r);
				DataGridViewCellStyle cs = this.DgvStyle.Rows[r].Cells[c].Style;
				Dgv.Rows[rDgv].Cells[c].Style = null;
				Dgv.Rows[rDgv].Cells[c].Style.ApplyStyle(cs);
			}
			return this;
		}
		
		public DGVSyncedGrid refreshSyncKeys()
		{
			if(this.IsDGVUsed) {
				// Refresh SyncValuesView
				List<string> svv = new List<string>();
				int rCnt = this.Dgv.Rows.Count;
				for(int r=0; r<rCnt; r++) {
					svv.Add(this.Dgv.Rows[r].Cells[this.SyncColumn].Value.ToString());
				}
				this.syncValuesView = svv;
			}
			return this;
		}
		
		public int rowView(int rowSource) {
			string key = this.syncValuesSource[rowSource].ToString();
			return this.syncValuesView.IndexOf(key);
		}
		
		public int rowSource(int rowView) {			
			string key = this.syncValuesView[rowView].ToString();
			return this.SyncValuesSource.IndexOf(key);
		}
	}
	
	// ==========
	// EXCEPTIONS
	// ==========
	public class DGVSyncedGridException : GridException
	{
		public DGVSyncedGridException(string message) : base(message)
		{
		}
	}
	
	public class DGVSyncParamsException : DGVSyncedGridException
	{
		public DGVSyncParamsException(string message) : base(message)
		{
		}
	}
}
