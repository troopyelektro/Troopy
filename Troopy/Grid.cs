/*
 * Created by SharpDevelop.
 * User: Troopy
 * Date: 22.5.2019
 * Time: 21:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Troopy
{
    /* Makes magic with data in datagrid */

    
    // constructor, transfers DataGridView
    public class Grid : AppBase
    {	
    	public const string tableDefault = "_Default";
        public const string tableAddAuto = "_Auto";
        public const string tableAddAutoPrefix = "Table";
        public const int tableOrdinalStart = 0;
        public const int tableOrdinalEnd = 9999;
        
        public const int sortRising = 0;
        public const int sortFalling = 1;

        public DataSet ds;
        private int iTables = 0;
    
        // Parameters for lookup
        public struct LookupParameters {
            public const int srcFilterSelectFirst = 0;
            public const int srcFilterSelectLast = 1;
            public const int srcFilterSelectNone = 2;
            public bool skipNotFound;
            public string notFoundValue;
            public string srcFilterSort;
            public int srcFilterSelect;
            public bool[] trgFilterOverwriteOn;
            public string[] trgFilterOverwriteValue;
        };
 
        // When lookup has to decide between more results
        public struct MultiMatchResult {
            public string table;
            public string[] cols;
            public string[] vals;
            public int count;
        };
        
        public List<MultiMatchResult> lookupMultiMatch = new List<MultiMatchResult>();
 
        // Constructor
        public Grid(DataGridView dg = null)
        {
            this.ds = new DataSet();
            if(dg!=null) {
                this.addTable(dg, tableDefault);
            }
            
        }

        /* @param DataGridView dgv ... Source of table
         * @param string tableName ... Name for new table (default auto)
         * @return Grid this       ... Provides a fluent interface */
        public Grid addTable(DataGridView dg, string tableName = tableAddAuto)
        {
            // Evaluate name for new table
            switch(tableName) {
                case tableAddAuto:
                    tableName = tableAddAutoPrefix + iTables.ToString();
                    break;
            }
            
            if (ds.Tables.Contains(tableName)) {
                ds.Tables.Remove(tableName);
            }            
            
            iTables++;
            this.ds.Tables.Add(dgv2dt(dg, tableName)); 
            return this;
        }
        
        /* @param DataTable t      ... Source of table
         * @param string tableName ... Name for new table (default auto)
         * @return Grid this       ... Provides a fluent interface */
        public Grid addTable(DataTable t, string tableName = tableAddAuto)
        {
            // Evaluate name for new table
            switch(tableName) {
                case tableAddAuto:
                    tableName = tableAddAutoPrefix + iTables.ToString();
                    break;
            }
            
            if (ds.Tables.Contains(tableName)) {
                ds.Tables.Remove(tableName);
            }
                    
            iTables++;
            var newTable = t.Copy();
            newTable.TableName = tableName;
            this.ds.Tables.Add(newTable);
            return this;
        }        

        /*  Convers content of DataGridView into DataTable
         *  @param DataGridView dgv ... Selected DataGridView
         *  @return DataTable */
        public static DataTable dgv2dt(DataGridView dg, string tableName)
        {
            DataTable table = new DataTable(tableName);
                        
            // Fill columns
            foreach (DataGridViewColumn col in dg.Columns) {
                table.Columns.Add(col.Name);
            }
            
            // Fill data
            foreach (DataGridViewRow row in dg.Rows)
            {                
                table.Rows.Add();
                foreach (DataGridViewCell cell in row.Cells)
                {                                                            
                    table.Rows[row.Index].SetField(cell.ColumnIndex, dg[cell.ColumnIndex,row.Index].Value);
                }                
            }  
            
            return table;
        }

        /*  Convets Grid into DataSet
         *  @param void
         *  @return DataSet */
        public DataSet getDataSet()
        {                       
            return ds;
        } 
        

        /*  Converts DataTable into DataGridView
         
         *  @param string tableName ... Name of table in dataset
         *  @param DataGridView dgv ... Tartget data grid */
        public void dt2dgv(DataGridView dg, string tableName = tableDefault)
        {
            dg.Columns.Clear();
            dg.DataSource = null; // When not called, second call fail (DGV is then empty) but not every table			
			dg.DataSource = this.getTable(tableName);
        }
        
        /* Converts column name to column index
         * @param string name   ... Name of column
         * @return int index    ... Index of column */
        public int getColumnIndex(string tableName, string columnName)
        {
            return this.ds.Tables[tableName].Columns.IndexOf(columnName);
        }
        
        /* Converts column name to column index
         * @param string tableName ... Name of table
         * @param int columnIndex  ... Index of column
         * @return string          ... Name of table */
        public string getColumnName(string tableName, int columnIndex)
        {
            return this.getTable(tableName).Columns[columnIndex].ColumnName;
        }     

        public List<string> getColumnValues(string tableName, string columnName)
        {
        	return this.getColumnValues(tableName, this.getColumnIndex(tableName, columnName));
        }         
        
        public List<string> getColumnValues(string tableName, int columnIndex)
        {
        	List<string> l = new List<string>();
        	foreach(DataRow r in this.getTable(tableName).Rows) {
        		l.Add(r.ItemArray[columnIndex].ToString());
        	}
        	return l;
        }

        public bool tableExists(string tableName){
        	if(this.ds.Tables.Contains(tableName)) {
        		return true;
        	} else {
        		return false;
        	}
        }
        
        public bool columnExists(string tableName, string columnName)
        {
        	if(this.getTable(tableName).Columns.Contains(columnName)) {
        		return true;
        	} else {
        		return false;
        	}
        }

       
        
        /* Adds new column into table (Ordinal by column name)
         * @param string tableName      ... Name of table
         * @return int                  ... Number of added row */
        public int addRow(string tableName)
        {
        	this.getTable(tableName).Rows.Add();
        	return this.getRowCount(tableName)-1;
        } 
        
        /* Adds new column into table (Ordinal by column name)
         * @param string tableName      ... Name of table
         * @param string columnName     ... Name for new column
         * @param string location       ... Where to add column
         * @return Grid this            ... Provides fluent interface */
        public Grid addColumn(string tableName,string columnName, string location)
        {
            this.addColumn(tableName, columnName, this.getColumnIndex(tableName, location));
            return this;
        }       
        
        
        /* Adds new column into table (ordinal by integer)
         * @param string tableName      ... Name of table
         * @param string columnName     ... Name for new column
         * @param int location          ... Where to add column
         * @return Grid this            ... Provides fluent interface */
        public Grid addColumn(string tableName,string columnName, int location=tableOrdinalEnd)
        {
            if(location > this.ds.Tables[tableName].Columns.Count) {
                location = this.ds.Tables[tableName].Columns.Count;
            }
            this.ds.Tables[tableName].Columns.Add(columnName).SetOrdinal(location);
            return this;
        }
 
         /* Delete column from table by Name
          * @param string tableName      ... Name of table
          * @param string columnName     ... Name of column to delete
          * @return Grid this            ... Provides fluent interface */
        public Grid deleteColumn(string tableName,string columnName)
        {
            int ci = this.getColumnIndex(tableName, columnName);
            if(ci >= 0) {
                this.ds.Tables[tableName].Columns.Remove(columnName);
            }
            return this;
        }

        /* Delete column from table by Index
         * @param string tableName      ... Name of table
         * @param int ci                ... Index of column to delete
         * @return Grid this            ... Provides fluent interface */
        public Grid deleteColumn(string tableName, int ci=tableOrdinalEnd)
        {
            if(ci >= this.ds.Tables[tableName].Columns.Count) {
                ci = this.ds.Tables[tableName].Columns.Count-1;
            }
            string cn = this.ds.Tables[tableName].Columns[ci].ColumnName;
            return this.deleteColumn(tableName, cn);
        }
        
        public Grid deleteTable(string tableName)
        {
        	try {
        		this.ds.Tables.Remove(tableName);
        	} catch(Exception ex) {
        		throw new GridException("Nelze smazat tabulku " + tableName);
        	}
        	return this;
        }

        /* Make lookup, filter and data is single column
         *   src      trg
         *  X   Y    X    Y
         * x1  y1   x1   y1
         * x2  y2   x3   y3
         * x3  y3   x4   y4
         * x4  y4   x3   y3
         * @param string src         ... Name of table with source values 
         * @param string trg         ... Name of target table where it has to be written
         * @param string srcData     ... srcData from src table is written into
         * @param string trgData     ... trgData in trgTable
         * @param string srcFilter   ... Column in src table defines searched value for src data
         * @param string trgFilter   ... Column in trg table, searched value
         * @param LookupParameters p ... Setup the lookup
         * @return Grid              ... Provides a fluent interface */
        public Grid lookup(string src,
                           string trg,
                           string srcData,
                           string trgData,
                           string srcFilter,
                           string trgFilter,
                           LookupParameters p)
        {
            return this.lookup(
                src,
                trg,
                new string[]{srcData},
                new string[]{trgData},
                new string[]{srcFilter},
                new string[]{trgFilter},
                p
            );
        }

        /* Make lookup, filter and data are multiple columns
         * @param string src         ... Name of table with source values 
         * @param string trg         ... Name of target table where it has to be written
         * @param string[] srcData     ... srcData from src table is written into
         * @param string[] trgData     ... trgData in trgTable
         * @param string[] srcFilter   ... Column in src table defines searched value for src data
         * @param string[] trgFilter   ... Column in trg table, searched value
         * @param LookupParameters p ... Setup the lookup
         * @return Grid              ... Provides a fluent interface */
        public Grid lookup(string src,
                           string trg,
                           string[] srcData,
                           string[] trgData,
                           string[] srcFilter,
                           string[] trgFilter,
                           LookupParameters p)
        {
            string nfv = p.notFoundValue;
            
            // create arrays of column indexes
            // example list: List<string> tempVehiclesIds = new List<string>();
            var liSrcData = new List<int>();
            var liTrgData = new List<int>();
            var liSrcFilter = new List<int>();
            var liTrgFilter = new List<int>();
            
            // create column index list for srcData
            foreach(string c in srcData) {
                liSrcData.Add(getColumnIndex(src, c));
            }
            
             // create column index list for trgData
            foreach(string c in trgData) {
                liTrgData.Add(getColumnIndex(trg, c));
            }           
            
            // create column index list for srcFilter
            foreach(string c in srcFilter) {
                liSrcFilter.Add(getColumnIndex(src, c));
            }             
            
            // create column index list for srcFilter
            foreach(string c in trgFilter) {
                liTrgFilter.Add(getColumnIndex(trg, c));
            }   
            
            // short access for source and target tables
            DataTable ts = this.ds.Tables[src];
            DataTable tt = this.ds.Tables[trg];
            
            // get row count of target table
            int trgTableCount = this.ds.Tables[trg].Rows.Count;
            
            for(int r=0; r <trgTableCount; r++) {               
                
                // scan filters from target table
                var trgFilterValue = new List<string>();
                                
                for(int i=0; i<liTrgFilter.Count; i++) {
                    if((p.trgFilterOverwriteOn.Length > i) && p.trgFilterOverwriteOn[i]) {
                        // Value is overwritten
                        trgFilterValue.Add(p.trgFilterOverwriteValue[i]);
                    } else {
                        // Standard
                        trgFilterValue.Add(tt.Rows[r].ItemArray[liTrgFilter[i]].ToString());
                    }
                }
                  
                // create filter for source table
                string srcFilterExpression = "";
                for(int i=0; i<trgFilterValue.Count; i++) {
                    srcFilterExpression += "[" + srcFilter[i] + "]" + 
                                           " = " +
                                           "'" + trgFilterValue[i] + "'";
                    if((i+1) < trgFilterValue.Count) {
                        srcFilterExpression += " AND ";
                    }
                }
                
                // Create filtered view
                DataView dv = new DataView(ts,
                                           srcFilterExpression,
                                           p.srcFilterSort,
                                           DataViewRowState.CurrentRows);
                
                // Check count of filtered rows
                int srcFilterCount = dv.Count;
                
                // Create new table from filtered view
                DataTable dt = dv.ToTable();
       
                // Fillup all target data
                for(int i=0; i<liTrgData.Count; i++) {
                    
                	bool fill = false;
                    int rowSrcData = 0;
                	
                    if(srcFilterCount == 1) {
						// 1 result
                    	rowSrcData = 0;
						fill = true;
                	} else if(srcFilterCount > 1) {
                        // 2+ results
                    	switch(p.srcFilterSelect) {
                            case LookupParameters.srcFilterSelectFirst:
                                rowSrcData = 0;
                                fill = true;
                                break;
                                
                            case LookupParameters.srcFilterSelectLast:
                                rowSrcData = srcFilterCount-1;
                                fill = true;
                                break;
                                
                           	case LookupParameters.srcFilterSelectNone:
                                rowSrcData = 0;
                                fill = false;
                                break;
                        }
                    } else {
                    	// 0 results
                    	if(!p.skipNotFound) {
                			tt.Rows[r].SetField(liTrgData[i], p.notFoundValue);
                		}
                    }
                    
                    // set value in target table
                    if(fill) {
                    	tt.Rows[r].SetField(
                    		liTrgData[i],
                    		dt.Rows[rowSrcData].ItemArray[liSrcData[i]].ToString()
                    	);
                    }
                    
                    // Make flag if more results found
                    if(srcFilterCount > 1) {
                    	var lmm = new MultiMatchResult();
                    	lmm.table = src;
                    	lmm.cols = srcFilter;
                    	lmm.vals = trgFilterValue.ToArray();
                    	lmm.count = srcFilterCount;
                    	this.lookupMultiMatch.Add(lmm);
                    }
                        
                }
            }
            return this;
        }
        
        /* Returns List of filter values which caused multiple row select
         * @return List<MultiMatchResult> */
        public List<MultiMatchResult> getMultiMatchResults()
        {
        	List<MultiMatchResult> res = new List<MultiMatchResult>();
        	foreach(MultiMatchResult mmr in lookupMultiMatch) {
        		res.Add(mmr);
        	}
        	return res;
        }
        
        /* Clears List of filter values which caused multiple row select
         * @return Grid ... Provides a fluent interface */
        public Grid clearMultiMatchResults()
        {
            lookupMultiMatch.Clear();
            return this;
        }
        
        
        
 
        /* Creates table with unique values
         * @param string src ... Source table name
         * @param string col ... Column for unique values
         * @param string trg ... target table name
         * @return Grid      ... provides a fluent interface*/
        public Grid createUnique(string src, string col, string trg)
        {   
            if(this.getColumnIndex(src, col) >= 0) {
                DataTable st = new DataView(ds.Tables[src],
                                            "",
                                            col,
                                            DataViewRowState.CurrentRows).
                                            ToTable();
                DataTable tt = new DataTable(trg);
                tt.Columns.Add(col);
                string rVal = "";
                string rValPrev = "";
                int srcCol = getColumnIndex(src,col);
                
                for(int r=0; r < st.Rows.Count; r++) {
                    rVal = st.Rows[r].ItemArray[srcCol].ToString();
                    if(rVal != rValPrev) {
                        tt.Rows.Add(new Object[]{rVal});
                    }
                    rValPrev = rVal;
                }
                this.ds.Tables.Add(tt);
            }
            return this;
        }
        
        /* Create filtered table
         * @param string src         ... Source table name
         * @param string trg         ... Target table name
         * @param string[] filterCol ... Columns for filtering 
         * @param string[] filterVal ... Values for filtering
         * @return Grid              ... Provides a fluent interface */
        public Grid createFiltered(
            string src,
            string trg,
            string[] filterCol,
            string[] filterVal
        ) {            
            string f = "";
            for(int i=0; i<filterCol.Length; i++) {
                f += "[" + filterCol[i] + "] = '" + filterVal[i] + "'";
                if((i+1) < filterCol.Length) {
                    f += " AND ";
                }
            }
            var dv = new DataView( this.getTable(src), f, "" , DataViewRowState.CurrentRows);
            DataTable ft;
            ft = dv.ToTable();
            this.addTable(ft, trg);
            return this;
        }
        
        
        /* Counts rows in table
         * @param string tableName ... Number of table
         * @return int             ... Number of rows */
        public int getRowCount(string tableName)
        {
        	return this.getTable(tableName).Rows.Count;
        }
        
        public int getColumnCount(string tableName)
        {
        	return this.getTable(tableName).Columns.Count;
        }
        
        /* Gives selected DataTable from DataSet
         * @param string tableName ... Selected table
         * @return DataTable */
        public DataTable getTable(string tableName)
        {
        	if(!this.tableExists(tableName)){
        		
        	}
        	return this.ds.Tables[tableName];
        } 

        /// <summary>
        /// Finds index of Column 
        /// </summary>
        /// <param name="table">Table, where column is searched</param>
        /// <param name="column">Column Name</param>
        /// <returns>Column number, or -1 if column not found</returns>
        public int columnIndex(string table, string column)
        {
        	return this.getTable(table).Columns.IndexOf(column);
        }
        
        /* Gets cell's value as string
         * @param string table ... Source table select
         * @param int row      ... Row index
         * @param int col      ... Column index 
         * @return string */
        public string getCell(string table, int row, int col)
        {
        	return this.getTable(table).Rows[row].ItemArray[col].ToString();
        }
        
        /* Gets cell's value as string
         * @param string table ... Source table select
         * @param int row      ... Row index
         * @param string col   ... Column name 
         * @return string */
        public string getCell(string table, int row, string col)
        {
        	int ci = this.getColumnIndex(table, col);
        	return this.getCell(table, row, ci);
        }
        
        
        /* Gets Color stored in cell in format "AAA,RRR,GGG,BBB"
         * @param string table ... Source table select
         * @param int row      ... Row index
         * @param intColor */
        public Color getColor(string table, int row, int col)
        {        	
    		int a = 255;
    		int r = 255;
    		int g = 255;
    		int b = 255;
        	string strColor = this.getTable(table).Rows[row].ItemArray[col].ToString();
        	string[] arrColor = strColor.Split(',');
        	if(arrColor.Length == 4) {
        		a = new Conversion(arrColor[0]).getInt();
        		r = new Conversion(arrColor[1]).getInt();
        		g = new Conversion(arrColor[2]).getInt();
        		b = new Conversion(arrColor[3]).getInt();
        	}
        	return Color.FromArgb(a, r, g, b);
        }
        
        /* Gets Color stored in cell in format "AAA,RRR,GGG,BBB"
         * @param string table ... Source table select
         * @param int row      ... Row index
         * @param string col   ... Column name 
         * @return string */
        public Color getColor(string table, int row, string col)
        {
        	int ci = this.getColumnIndex(table, col);
        	return this.getColor(table, row, ci);
        } 
		
        
        // Get Unsigned Integer from Cell
        public uint getUint(string table, int row, int col)
        {
        	return new Conversion(getCell(table, row, col)).getUint();
        }
        

        public uint getUint(string table, int row, string col)
        {
        	return new Conversion(getCell(table, row, col)).getUint();
        }


        
        /* Sets cell's value from string
         * @param string table ... Source table select
         * @param int row      ... Row index
         * @param int col      ... Column index 
         * @return Grid        ... Provides a fluent interface */
        public virtual Grid setCell(string table, int row, int col, string value)
        {
        	this.getTable(table).Rows[row].SetField(col, value);
        	return this;
        }
        
        /* Sets cell's value from string
         * @param string table ... Source table select
         * @param int row      ... Row index
         * @param string col   ... Column name *
		 * @return Grid        ... Provides a fluent interface */
        public virtual Grid setCell(string table, int row, string col, string value)
        {
        	int ci = this.getColumnIndex(table, col);
        	return this.setCell(table, row, ci, value);
        }
        
        
        //
        // Logical Or with cell value (uint stored as string in DataTable)
        //
        public Grid or(string table, int row, int col, uint value)
        {
        	string vs = this.getCell(table, row, col);
        	Conversion c = new Conversion(vs);
        	uint ui = (uint)c.getInt();
        	ui |= value;        	
        	return this.setCell(table, row, col, ui.ToString());
        }

        public Grid or(string table, int row, string col, uint value)
        {
        	int ci = this.getColumnIndex(table, col);
        	return this.or(table, row, ci, value);
        }        
        
        public Grid join(string[] tables, string target, bool deleteSources=false) {
        	
        	// get source table count
        	int sCnt = tables.Length;
        	
        	if(sCnt == 0) {
        		throw new GridException("Nebyly zvoleny žádné tabulky ke spojení");
        	}
        	
        	// Check if column count matches
        	int cCnt = this.getTable(tables[0]).Columns.Count;
        	foreach(string table in tables) {
        		if(this.getTable(table).Columns.Count != cCnt) {
        			throw new GridException("Tabulka " + table + " nemá stejný počet sloupců jako tabulka " + tables[0].ToString());
        		}
        	}
        	
        	// Add first table
        	this.addTable(this.getTable(tables[0].ToString()), target);
        	
        	// Add Next tables
        	for(int t=1; t<sCnt; t++) {
        		foreach(DataRow dr in this.getTable(tables[t]).Rows){
        			this.getTable(target).Rows.Add(dr.ItemArray);
        		}
        	}
        	
        	// delete sources
        	if(deleteSources) {
        		foreach(string table in tables) {
        			this.deleteTable(table);
        		}
        	}
        	
        	return this;
        }
        
        
        public Grid sort(string tableName, string[] columns, int[] way)
        {
        	int cntCols = columns.Length;
        	int cntWay = way.Length;
        	string s = "";
        	for(int i=0; i<cntCols; i++){
        		if(i != 0) {
        			s += " ";
        		}
        		s += "[" + columns[i] + "]";
        		if((i < cntWay) && way[i]==Grid.sortFalling) {
        			s += " desc";
        		}
        		if((i+1) != cntCols) {
        			s += ",";
        		}
        	}
        	DataView dv = this.getTable(tableName).DefaultView;
        	dv.Sort = s;
        	DataTable sortedDT = dv.ToTable();
        	sortedDT.TableName = tableName;
        	this.addTable(sortedDT, tableName);
        	return this;
        }
        
        
        // Copy data from source table to target table
        // values must be by key identified
        // cnStart and cnEnd limit columns for operation
        // Column cnNumber must containt unique row number
        public Grid transfer(string source, string target, int ciRowNumber, int ciStart = -1, int ciEnd = -1) {
        	
        	// Check if source and target table has same column set
        	if(this.columnSetCompare(source, target)) {
        		
				// Check if cnRowNumber is in range of source table
				int colCount = this.getColumnCount(source);
				
				// set start if not defined yet
				if(ciStart == -1) {
					ciStart = 0;
				}
				
				// set end if not defined yet
				if(ciEnd == -1) {
					ciEnd = colCount-1;
				}				
				
				// Check column numbers
				for(int i=0; i<3; i++) {
					
					int cn = 0;
					string textColumn = ""; 
					
					switch(cn) {
						case 0:
							cn = ciRowNumber;
							textColumn = "číslem řádku";
							break;
						case 1:
							cn = ciStart;
							textColumn = "počátkem dat";
							break;
						case 2:
							cn = ciEnd;
							textColumn = "koncem dat";
							break;
					}
					
					if(cn < 0 || cn >= colCount) {
						throw new ColumnNumberOufOfRangeException(
							"Nelze provést transfer, " +
							"sloupec s " + textColumn + " je mimo rozsah sloupců zdrojové tabulky. " +
							"Tabulka " + source + " má " + colCount.ToString() + " sloupců. " +
							"Zvolen byl sloupec číslo " + cn.ToString() + "."
						);
					}				
				}
				
				// Fix order Start/End
				if(ciStart > ciEnd) {
					int tempCnStart = ciStart;
					ciStart = ciEnd;
					ciEnd = tempCnStart;
				}
				
				// check source row count
				int srcRowCount = this.getRowCount(source);
				
				// if some rows exists, execute
				if(srcRowCount > 0) {
					for(int r=0; r<srcRowCount; r++) {
						
						string str = this.getCell(source, r, ciRowNumber);
						int tr = new Conversion(str).getInt();
						
						for(int c = ciStart; c <= ciEnd; c++) {
							
							string value = this.getCell(source, r, c);
							this.setCell(target, tr, c, value);
						}
					}
				}
        	
        	} else {
        		throw new ColumnSetMismatchException(
        				"Nelze provést přenos, tabulky " + source + " a "
        				+ target + " nemají stejné sloupce."
        			);
        	}
        
        	return this;
        }
        
        
        
        // Compare column name set of two tables
        // returns: true  if all columns are same
        //          false column count or any name mismatch
        public bool columnSetCompare(string source, string target) {
        
        	bool result = false;
        	
        	int srcColCount = this.getColumnCount(source);
        	int trgColCount = this.getColumnCount(target);
        	if(srcColCount == trgColCount) {
        		for(int i=0; i<srcColCount; i++) {
        			string srcColName = this.getColumnName(source, i);
        			string trgColName = this.getColumnName(target, i);
        			if(srcColName == trgColName) {
        				if((i+1) == srcColCount) {
        					result = true;
        				}
        			} else {
        				i = srcColCount;
        			}
        		}
        	}
        
        	return result;
        }
    }
    
	// ==========
	// EXCEPTIONS
	// ==========
	public class GridException : TroopyException
	{
		public GridException(string message) : base(message)
   		{
   		}
   	}

	public class TableNotExistsException : GridException
	{
		public TableNotExistsException(string message) : base(message)
   		{
   		}
   	}

	public class ColumnNotExistsException : GridException
	{
		public ColumnNotExistsException(string message) : base(message)
   		{
   		}
   	}
	

	public class ColumnSetMismatchException : GridException
	{
		public ColumnSetMismatchException(string message) : base(message)
   		{
   		}
   	}	

	public class ColumnNumberOufOfRangeException : GridException
	{
		public ColumnNumberOufOfRangeException(string message) : base(message)
   		{
   		}
   	}
	
}
