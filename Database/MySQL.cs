using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using Troopy;
using Troopy.Database;

namespace Troopy.Database
{
	public class MySQL : Database
	{
		private const string now = "NOW()";
		private const bool useProgressBar = true;
		
		private bool connected;
		public bool Connected {
			get {															
				return this.connected;
			}
		}
		
		private MySql.Data.MySqlClient.MySqlConnection conn;
		
		private string host;
		private string dbname;
		private string username;
		private string password;
		
		public void setHost(string h) {
			this.host = h;
		}

		public void setDbname(string n) {
			this.dbname = n;
		}

		public void setUsername(string u) {
			this.username = u;
		}

		public void setPassword(string p) {
			this.password = p;
		}
					
		// Connect to database
		// return true if connection ok
		// return false if connection failed
		public bool connect() {
			
			// create connection string
			string cmdConnect;
			cmdConnect = "Database=" + dbname + ";" +
				"Data Source=" + host + ";" +
				"User Id=" + username + ";" +
				"Password=" + password;			
			try {
          		conn = new MySqlConnection(cmdConnect);
				conn.StateChange += new StateChangeEventHandler(stateChanged);
          		conn.Open();
				AppBase.addOutput("Připojení k databázi OK");				
				this.connected = true;

			}
			catch (MySqlException ex)
			{
				AppBase.addOutput("Připojení k databázi selhalo:");
				this.connected = false;
				AppBase.addOutput(ex.Message);
			}
			
			return true;
		}
		
		public void disconnect()
		{
			this.conn.Close();
		}
		
		public bool isConnected()
		{
			return this.connected;
		}

		// Event handler on Database state change
		void stateChanged(object sender, StateChangeEventArgs e)
		{			
			string s = e.CurrentState.ToString();
			if(s == "Closed") {
				this.connected = false;				
			}
		}
		
		
       	/*	Insert data to table
		 *	@param string   name 	... name of table
		 *	@param string[] col	 	... names of columns
	     * 	@param string[] val	 	... values to insert
    	 * 	@return long id		 	... id of new row in table */
		public long insert(string name, string[] col, string[] val)
		{
		    string sql = "INSERT INTO " + name + " (";            
		    int i = 0;
		    
		    // Do not insert if Length of col and val is not same
		    if(col.Length != val.Length) {
		        return 0;
		    }
		    
		    // generate column list
		    while(i < col.Length) {
		        sql += col[i];
		        if(++i < col.Length) {
		            sql += ", ";
		        }
		    }		    

		    sql += ") VALUES (";

            // generate values list		    
		    i = 0;
            while(i < val.Length) {
		        sql += "'";
		        switch(val[i]) {
		            case now:
		                sql += val[i];
		                break;
		            default:
		                sql += val[i];        
		                break;
		        }
		        sql += "'";
                if(++i < val.Length) {
                    sql += ", ";
                }
            }        			
		    
			sql += ")";

            // Execute Command
            try{
            	MySqlCommand cmd = new MySqlCommand(sql, this.conn);
            	cmd.ExecuteNonQuery();
            	return cmd.LastInsertedId;
            } catch(Exception ex) {
            	throw new DatabaseException("Nebylo možné provést příkaz " + sql);
            }
		}

        /* Updates data in database
         * @param string name       ... Name of table
         * @param string[] whCol    ... Where Column Names
         * @param string[] whOp     ... Where Operatorss
         * @param string[] whVal    ... Where Values
         * @param string[] upCol    ... Update column name
         * @param string[] upVal    ... Update values
         * @return int              ... Number of affected rows */
        public long update(
            string name,
            string[] whCol,
            string[] whOp,
            string[] whVal,
            string[] upCol,
            string[] upVal
        ) {
            string sql = "UPDATE " + name + " SET ";
            for(int i=0; i<upCol.Length; i++) {
                sql += "`" + upCol[i] + "` = '" + upVal[i] + "'";
                if(i < (upCol.Length-1)) {
                    sql += ",";
                }
                sql += " ";                                
            }
            
            sql += this.createWhere(whCol, whOp, whVal);
            
            // Execute Command
            try{
            	MySqlCommand cmd = new MySqlCommand(sql, this.conn);
            	return cmd.ExecuteNonQuery();
            } catch(Exception ex) {
            	throw new DatabaseException("Nebylo možné provést příkaz " + sql);
            }
        }
         


        /*    Deletes data from table
         *    @param string   name ... name of table
         *    @param string[] col  ... names of columns
         *    @param string[] val  ... values to insert
         *    @return long        ... Number of deleted rows */
        public long delete(string name, string[] col, string[] op, string[] val)
        {
            string sql = "DELETE FROM " + name + " ";            
            int i = 0;
            
            // Do not insert if Length of col and val is not same
            if(col.Length != val.Length) {
                return 0;
            }

            sql += createWhere(col,op,val);           
            
            // Execute Command
            try{
            	MySqlCommand cmd = new MySqlCommand(sql, this.conn);
            	return cmd.ExecuteNonQuery();
            } catch(Exception ex) {
            	throw new DatabaseException("Nebylo možné provést příkaz " + sql);
            }
        }


        /*	Deletes all data from table
		 *	@param string	name	...	name of table */		 
		public void truncate(string tableName)
		{
			string sql = "TRUNCATE " + tableName;
			
			try{
				MySqlCommand cmd = new MySqlCommand(sql, this.conn);
				cmd.ExecuteNonQuery();
			} catch(Exception ex) {
				throw new DatabaseException("Nebylo možné provést příkaz " + sql);
			}
		}

        /* Returns selected entries in table
         * @param string[] col ... Columns
         * @param string[] op  ... Operators 
         * @param string[] val ... Values 
         * @return string      ... Number of entries */
        private string createWhere(string[] col, string[] op, string[] val)
        {
            string sql = "";
            
            // add filter
            if (col.Length > 0) {
                sql += " WHERE ";
                for (int i = 0; i < col.Length; i++) {
                    
                    // Column name
                    sql += col[i];
                    
                    // Space
                    sql += " ";
                    
                    // Operator
                    if (i < op.Length) {
                        sql += op[i];
                    } else {
                        sql += "=";
                    }
                
                    // Space
                    sql += " ";
                  
                    // Value
                    sql += "'";
                    if (i < val.Length) {
                        sql += val[i];
                    }                    
                    if ((i < op.Length) && (op[i] == "LIKE")) {
                        sql += "%";
                    }
                    sql += "'";
                    
                    // Continue
                    if ((i+1) < col.Length) {
                        sql += " AND ";
                    }                
                }                                                    
            }            
            return sql;
        }

        /* Returns selected entries in table
         * @param string name  ... Name of table
         * @param string[] col ... Columns
         * @param string[] op  ... Operators 
         * @param string[] val ... Values 
         * @return DataSet     ... Number of entries */     
        public DataTable select(string name, string[] selCol, string[] col, string[] op, string[] val)
        {
            // Create query
            string sql = "SELECT ";
            for(int i = 0; i<selCol.Length ; i++) {
                // add column for select
                sql += selCol[i];
                // not last
                if((i+1) < selCol.Length) {
                    sql += ", ";
                }
            }
            sql += " FROM " + name + createWhere(col,op,val);           
            
            // Read from DB
            var dataSelect = new DataSet();                        
            
            try {
                var dbAdapter = new MySqlDataAdapter(sql, conn);
                var dbCommandBuider = new MySqlCommandBuilder(dbAdapter);                            
                dbAdapter.Fill(dataSelect, name);
                return dataSelect.Tables[name];
            } catch(Exception ex) {
                throw new DatabaseException("Nebylo možné provést příkaz " + sql);
            }
            
        }

        public int max(string name, string selCol, string[] col, string[] op, string[] val)
        {
            // Create query
            string sql = "SELECT MAX(" + selCol + ") AS '" + selCol + "'";
            sql += " FROM " + name + createWhere(col, op, val);

            // Read from DB
            var dataSelect = new DataSet();

            try
            {
                var dbAdapter = new MySqlDataAdapter(sql, conn);
                var dbCommandBuider = new MySqlCommandBuilder(dbAdapter);
                dbAdapter.Fill(dataSelect, name);
                return new Conversion(dataSelect.Tables[name].Rows[0].ItemArray[0].ToString()).getInt();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Nebylo možné provést příkaz " + sql);
            }

        }

        //
        // Select without filter
        //        
        public DataTable select(string name, string[] selCol)
        {
        	return this.select(
        		name,
        		selCol,
        		new string[]{},
        		new string[]{},
        		new string[]{}
        	);
        }

		/* Returns count of entries in table
		 * @param string name  ... Name of table
		 * @param string[] col ... Columns
		 * @param string[] op  ... Operators 
		 * @param string[] val ... Values 
		 * @return int         ... Number of entries */	 
		public int count(string name, string[] col, string[] op, string[] val)
		{
		    // Begin of command
		    string sql = "SELECT COUNT(*) FROM " + name;

            // add filter
            sql += createWhere(col, op, val);
		    
		    // terminate command
		    sql += ";";
		    		    		    
		    // Execute
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                object result = cmd.ExecuteScalar();
                if (result != null) {
                    int r = Convert.ToInt32(result);
                    return r;
                } else {
                    return 0;
                }

            } catch (Exception ex) {
                throw new DatabaseException("Nebylo možné provést příkaz " + sql);
            }		    		   		    
		}	
	}
}
