/*
 * Created by SharpDevelop.
 * User: Ing. Petr Rösch
 * Date: 03.07.2019
 * Time: 8:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Troopy.Graphs
{
	public abstract class Base
	{
		public const int padXDefault = 20;
		public const int padYDefault = 35;
		public const int stepDefault = 100;
		
		public const string modeDateTime = "modeDateTime";
		public const string modeDouble = "modeDouble";	
		public const string modeSize = "modeSize";
		public const string modeUndefined = "modeUndefined";
		
		public const string styleLine = "styleLine";
		public const string styleDot = "styleDot";	

		public static readonly string[] modeYCompatibilityFrom = {modeDouble, modeSize};
		public static readonly string[] modeYCompatibilityTo = {modeSize, modeDouble};
	}
	
	public class Graph : Base
	{		
		private EventAdapter ea;
		
		private Panel panelPlot;
		private double[] doubleValuesY;
		private string modeX;
		private string modeY;
		private DateTime[] dateTimeValuesX;
		
		private int padX = padXDefault;
		private int padY = padYDefault;
		private int step = stepDefault;
		
		private bool cursorMouseClickOn;
		private bool cursorMouseMoveOn;
		private cursor cursorMouseClick;
		private cursor cursorMouseMove;		
		
		private int width;
		private int height;
		private int xCnt;
		private Color bgCol;
		private Color gridCol;
		private Color cursorCol;
		private Color diffCursorCol;
		private bool shwUnit;
						
		protected List<Line> lines;
		
		public struct cursor{
			public int x;
			public int y;
			public DateTime xDateTime;
			public double yDouble;
			public string descX;
			public string descY;
			public string desc;
			public int xi;
			public int yi;
		}
		
		//
		// Sweep
		//
		public static void sweep(Graph graph)
		{
			if(graph != null) {
				graph.ea.clearEvents();
			}
		}
		
		/* Contructor
		 * @param DateTime[] x ... Graph X are Dates
		 * @param double[] y   ... Graph Y are Numbers
		 * @return plot        ... Provides a fluent interface */
		public Graph(Panel panel, DateTime[] x)
		{
			// Create event adapter
			this.ea = new EventAdapter();
			
			// Set panel Control;
			this.panelPlot = panel;
			this.panelPlot.Width = this.padX + x.Length*this.step;
			this.panelPlot.BackColor = this.bgCol;
			this.panelPlot.Visible = true;
			
			// Store info about panel
			this.width = this.panelPlot.Width;
			this.height = this.panelPlot.Height;
			
			// Store values about X Axis
			this.dateTimeValuesX = x;
			this.xCnt = x.Length;
			this.modeX = modeDateTime;
			this.modeY = modeUndefined;
			this.setBackgroundColor(Color.FromArgb(255,255,255,255));
			this.setGridColor(Color.FromArgb(255,150,150,150));
			
			// Create lines of list
			lines = new List<Line>();
			
			// Disable mouse clicked cursor by default
			this.cursorMouseClickOn = false;
			this.cursorMouseMoveOn = false;
			
			// Create events
			this.ea.addEvent(this.panelPlot, EventAdapter.etPaint, new PaintEventHandler(this.eventGraphsPaint));
			this.ea.addEvent(this.panelPlot, EventAdapter.etMouseMove, new MouseEventHandler(this.eventGraphsMouseMove));
			this.ea.addEvent(this.panelPlot, EventAdapter.etMouseClick, new MouseEventHandler(this.eventGraphsMouseClick));
			this.ea.addEvent(this.panelPlot, EventAdapter.etMouseLeave, new EventHandler(this.eventGraphsMouseLeave));
			
			// Unit
			this.shwUnit = true;
		}
		
		/* Adds line to graph, only one value (horizontal line)
		 * @param double value ... Value to be displayed in graph
		 * @return Line        ... Created Line */
		public Line addLine(double value)
		{
			if(modeY == modeUndefined || modeY == modeDouble || modeY == modeSize) {
				if(modeY == modeUndefined) {
					this.modeY = modeDouble;
				}
				Line l = new Line(value, this.xCnt);
				lines.Add(l);
				return l;
			} else {
				throw new GraphException("Graf má hodnoty na ose Y ve formátu double/size, není možné přidat linku s hodnotami jiného formátu");
			}
		}
		
		/* Adds line to graph, only values
		 * @param double value ... Value to be displayed in graph
		 * @return Line        ... Created Line */
		public Line addLine(double[] value)
		{
			if(modeY == modeUndefined || modeY == modeDouble || modeY == modeSize) {
				if(value.Length == this.xCnt) {
					if(modeY == modeUndefined) {
						this.modeY = modeDouble;
					}
					Line l = new Line(value);
					lines.Add(l);
					return l;
				} else {
					throw new GraphException("Není možné přidat linku do grafu, protože počet hodnot osy Y nesouhlasí s počtem hodnot na ose X");						
				}
			} else {
				throw new GraphException("Graf má hodnoty na ose Y ve formátu double, není možné přidat linku s hodnotami jiného formátu");	
			}		
		}

		/* Gives count of lines in graph
 		 * return int ... Count of lines */
 		public int getLinesCount()
 		{
 			return this.lines.Count;
 		}
 		
 		/* Gives line on selected index
 		 * @param int index
 		 * @return Line */
 		public Line getLine(int i)
 		{
 			if(i < this.getLinesCount()) {
 				return this.lines[i];
 			} else {
				throw new GraphException("Nelze získat linku na pozici " + i.ToString() +
				                         ", maximální pozice je " + (this.getLinesCount()-1).ToString());
 			}
 		}
 		
 		/* Gives minimum value of all lines
 		 * @return double */
 		public double getMin()
 		{
 			double m = 0;
 			for(int i=0; i<this.getLinesCount(); i++) {
 				
 				// Initial value
 				if(i == 0) {
 					m = this.getLine(i).getMin();
 				} else {
 					if(this.getLine(i).getMin() < m) {
 						m = this.getLine(i).getMin();
 					}
 				}
 			}
 			return m;
 		}
 		
 		/* Gives maximum value of all lines
 		 * @return double */
 		public double getMax()
 		{
 			double m = 0;
 			for(int i=0; i<this.getLinesCount(); i++) {
 				
 				// Initial value
 				if(i == 0) {
 					m = this.getLine(i).getMax();
 				} else {
 					if(this.getLine(i).getMax() > m) {
 						m = this.getLine(i).getMax();
 					}
 				}
 			}
 			return m;
 		}

		/* Sets background color of graph
		 * @param Color c ... Color
		 * @return Graph  ... Provides a fluent interface */
		public Graph setBackgroundColor(Color c)
		{
			this.bgCol = c;
			this.panelPlot.BackColor = this.bgCol;
			return this;
		}
		
		/* Gives background Color of line
		 * @return Color */
		public Color getBackgroundColor()
		{
			return this.bgCol;
		}
		
		/* Sets grid color of graph
		 * @param Color c ... Color
		 * @return Graph  ... Provides a fluent interface */
		public Graph setGridColor(Color c)
		{
			this.gridCol = c;
			return this;
		}
		
		/* Gives Grid Color
		 * @return Color */
		public Color getGridColor()
		{
			return this.gridCol;
		}		

		/* Sets cursor color of graph
		 * @param Color c ... Color
		 * @return Graph  ... Provides a fluent interface */
		public Graph setCursorColor(Color c)
		{
			this.cursorCol = c;
			return this;
		}
		
		/* Gives cursor Color
		 * @return Color */
		public Color getCursorColor()
		{
			return this.cursorCol;
		}

		/* Sets difference cursor color of graph
		 * @param Color c ... Color
		 * @return Graph  ... Provides a fluent interface */
		public Graph setDiffCursorColor(Color c)
		{
			this.diffCursorCol = c;
			return this;
		}
		
		/* Gives difference cursor Color
		 * @return Color */
		public Color getDiffCursorColor()
		{
			return this.diffCursorCol;
		}		
		
		/* Sets, if unit should be shown
		 * @param bool value ... Determines if unit is shown
		 * @return Graph     ... Provides a fluent interface */
		public Graph setShowUnit(bool value)
		{
			this.shwUnit = value;
			return this;
		}
		
		/* Gives if units are shown
		 * @return bool */
		public bool getShowUnit()
		{
			return this.shwUnit;
		}
		
		/* Overwrite graph modeY
		 * @param string m ... mode desired
		 * @return Graphs  ... provides a fluent interface */
		public Graph setModeY(string m)
		{
			// Check compatibility
			for(int i=0; i<modeYCompatibilityFrom.Length; i++) {
				if(modeYCompatibilityFrom[i] == this.modeY && modeYCompatibilityTo[i] == m) {
					this.modeY = m;
				}
			}
			return this;
		}
		
		/* Draws Graphs
		 * @return Graph ... Provides a fluent interface */
		public Graph draw()
		{		
			// Make Drawing
			Graphics gr = this.panelPlot.CreateGraphics();			
			Pen gridPen = new Pen(this.getGridColor(), 1);
			int x;
			int hy = this.panelPlot.Height;
						
			// Draw grid and axis X
			for(int i=0; i<this.xCnt; i++) {
				
				x = this.padX + this.step * i;
				
				// Draw Date grid line
				gr.DrawLine(gridPen, x, 0, x, hy);
				
				// Draw description
				switch(this.modeX) {
					case modeDateTime:
						gr.DrawString(
							this.dateTimeValuesX[i].ToString("dd.MM.yyyy"),
							new Font("Arial", 10),
							new SolidBrush(this.getGridColor()),
							x + 5,
							hy - 15
						);
						break;
				}
			}

			// Draw Data on axis y
			int y = 0;
			int prevX=0;
			int prevY=0;
			for(int l=0; l<this.getLinesCount(); l++) {
				Line ln = this.getLine(l);
				Pen linePen = new Pen(ln.getColor(), 1);
				for(int i=0; i<this.xCnt; i++) {
														
					string s = ln.getStyle();
					x = this.padX + this.step * i;
					
					switch(ln.getMode()) {
						case Line.modeDouble:
							Int32.TryParse(Math.Round((ln.getValue(i) - this.getMin()) / this.getRatioY()).ToString(), out y);
							y = hy - this.padY - y;
							break;
					}
					
					
					switch(s) {
						case styleDot:
							gr.DrawEllipse(linePen, x-2, y-2, 4, 4);
							break;
						case styleLine:
							if(i==0) {
								prevX = x;
								prevY = y;
							}
							gr.DrawLine(linePen, prevX, prevY, x, y);
							break;
					}
					
					prevX = x;
					prevY = y;
				}
			}
			
			// Draw cursor
			Pen penCursor = new Pen(this.getCursorColor(), 1);

			if(this.cursorMouseMoveOn) {
				cursor cmm = this.cursorMouseMove;
				gr.DrawLine(penCursor, cmm.x - 10, cmm.y, cmm.x - 3, cmm.y);
				gr.DrawLine(penCursor, cmm.x + 10, cmm.y, cmm.x + 3, cmm.y);
				gr.DrawLine(penCursor, cmm.x, cmm.y - 10, cmm.x, cmm.y - 3);
				gr.DrawLine(penCursor, cmm.x, cmm.y + 10, cmm.x, cmm.y + 3);				
				gr.DrawString(
					cmm.descY,
					new Font("Arial", 10),
					new SolidBrush(this.getCursorColor()),
					cmm.x,
					cmm.y+3
				);
			}			
			
			if(this.cursorMouseClickOn) {
				cursor cmc = this.cursorMouseClick;
				gr.DrawLine(penCursor, cmc.x - 10, cmc.y, cmc.x - 3, cmc.y);
				gr.DrawLine(penCursor, cmc.x + 10, cmc.y, cmc.x + 3, cmc.y);
				gr.DrawLine(penCursor, cmc.x, cmc.y - 10, cmc.x, cmc.y - 3);
				gr.DrawLine(penCursor, cmc.x, cmc.y + 10, cmc.x, cmc.y + 3);
				gr.DrawString(
					cmc.descY,
					new Font("Arial", 10),
					new SolidBrush(this.getCursorColor()),
					cmc.x,
					cmc.y+3
				);				
			}
			
			if(this.cursorMouseMoveOn && this.cursorMouseClickOn) {
				cursor cmm = this.cursorMouseMove;
				cursor cmc = this.cursorMouseClick;
				
				// Draw only if different values are cursored
				if(cmm.x != cmc.x || cmm.y != cmc.y) {

					
					int dlY;
					if(Math.Abs(cmm.y - cmc.y) > 30) {
						dlY = ((cmm.y + cmc.y) / 2) - 5;
					} else {
						if(cmm.y > cmc.y) {
							dlY = cmc.y - 13;
						} else {
							dlY = cmm.y - 13;
						}
					}
					
					int dlX;
					
					string cursorText = "";
					switch(this.modeY) {
						case modeDouble:
							cursorText = new Conversion(cmm.yDouble - cmc.yDouble).getString(2);
							break;
						case modeSize:
							cursorText = new Conversion(cmm.yDouble - cmc.yDouble).getSize(3,2);
							break;
					}	
					

					gr.DrawLine(penCursor, cmm.x, cmm.y, cmc.x, cmm.y);
					gr.DrawLine(penCursor, cmm.x, cmc.y, cmc.x, cmc.y);
					dlX = cmm.x + 20;
					gr.DrawString(
						cursorText,
						new Font("Arial", 8),
						new SolidBrush(this.getDiffCursorColor()),
						dlX,
						dlY
					);
															
				}
			}		
			
			return this;
		}
		
		/* Creates cursor description
		 * @param int x   ... Cursor X
		 * @return Graph.cursor ... Cursor of nearest value */
		public Graph.cursor getCursor(int x, int y)
		{						
			int hy = this.panelPlot.Height;
			
			// Create cursor
			Graph.cursor c = new Graph.cursor();
			
			// Axis X
			c.xi = 0;
			
			for(int i=0; i<this.xCnt ; i++) {								
				if((x>=(this.padX + i*this.step - this.step/2))&&(x<(this.padX + i*this.step + this.step/2))) {
					c.xi =i;
					c.x = this.padX + i*this.step;
				}
			}
								
			switch(this.modeX) {
				case modeDateTime:
					c.descX = dateTimeValuesX[c.xi].ToString("dd.MM.yyyy");
					break;
			}
			
			// Axis Y									
			if(modeY == modeDouble || modeY == modeSize) {
				
				
				// Read all values for one X and sort it
				List<double> valsY = new List<double>();
				for(int li = 0; li<this.getLinesCount(); li++) {
					Line line = this.getLine(li);
					valsY.Add(line.getValue(c.xi));
					valsY.Sort();
				}
				
				// Generate their Y locations
				List<int> locY = new List<int>();
				for(int li = 0; li<valsY.Count; li++) {
					int ly=0;
					Int32.TryParse(Math.Round((valsY[li] - this.getMin()) / this.getRatioY()).ToString(), out ly);
					locY.Add(hy - this.padY - ly);
				}
				
				// Decide which value is the nearest
				if(valsY.Count == 1) {
					// Only one line
					c.yi = 0;
					c.yDouble = valsY[c.yi];
					c.y = locY[c.yi];
					c.descY = c.yDouble.ToString();
				} else {
					// More lines
					for(int li = 0; li<valsY.Count; li++) {
						switch(li) {
							case 0:
								if((y >= (locY[li] + locY[li+1])/2)) {
									c.yi = li;
								}
								break;
							default:
								if(li == (valsY.Count-1)) {
									if(y < (locY[li]+locY[li-1])/2) {
										c.yi = li;
									}
								} else {
									if((y < (locY[li] + locY[li-1])/2) && y >= (locY[li] + locY[li+1])/2) {
										c.yi = li;
									}
								}
								break;
						}
					}
					c.y = locY[c.yi];
					c.yDouble = valsY[c.yi];
					Conversion cText = new Conversion(c.yDouble);
					switch(modeY) {
						case modeDouble:
							c.descY = cText.getString(2);
							break;
						case modeSize:
							c.descY = cText.getSize(3,2,true);
							break;
					}
					
					
					
				}
					
			}
			
			// Final description
			c.desc = c.descX + ", " + c.descY;
			return c;
		}
			
		/* Calculates, how much big value is represented by one pixel on graph
		 * @return double */
		public double getRatioY()
		{
			double rangeMinMax = this.height - 2*this.padY;
			return (this.getMax()-this.getMin()) / rangeMinMax;
		}
		
		
		/* Event handler for panel */
		void eventGraphsPaint(object sender, PaintEventArgs e)
		{
			this.draw();
		}
		
		/* Event handler for Mouse Move */
		void eventGraphsMouseMove(object sender, MouseEventArgs e)
		{
			cursor c = this.getCursor(e.X, e.Y);
			if(this.cursorMouseMoveOn == false || c.x != this.cursorMouseMove.x || c.y != this.cursorMouseMove.y) {
				this.cursorMouseMove = c;
				this.cursorMouseMoveOn = true;
				this.panelPlot.Refresh();
			}
		}

		/* Event handler for Mouse Click */
		void eventGraphsMouseClick(object sender, MouseEventArgs e)
		{
			cursor c = this.getCursor(e.X, e.Y);
			this.cursorMouseClickOn = !this.cursorMouseClickOn;
			if((this.cursorMouseClickOn == true) && c.x != this.cursorMouseClick.x || c.y != this.cursorMouseClick.y) {				
				this.cursorMouseClick= c;
				this.panelPlot.Refresh();
			}
		}

		/* Clear Mouse Move Cursor after mouse out */
		void eventGraphsMouseLeave(object sender, EventArgs e)
		{
			this.cursorMouseMoveOn = false;
			this.panelPlot.Refresh();
		}
		
		public void refresh()
		{
			this.panelPlot.Refresh();
		}
	}
	
	

	
	public class Line : Base
	{
		private string style;
		private string modeY;
		private double[] valueDouble;
		private Color col;
		
		/* Constructor - single value
		 * @param double y ... value
		 * @param int xCnt ... Number of values on X
		 * @return Line    ... Provides a fluent interface */
		public Line(double y, int xCnt)
		{
			this.modeY = modeDouble;
			List<double> l = new List<double>();
			for(int i=0; i<xCnt; i++) {
				l.Add(y);
			}
			this.valueDouble = l.ToArray();
			this.setColor(Color.FromArgb(255,0,0,0));
			this.setStyle(styleLine);
		}
		
		/* Constructor - set of values
		 * @param double[] y ... valuea
		 * @return Line      ... Providemodes a fluent interface */		
		public Line(double[] y)
		{
			this.modeY = modeDouble;
			this.valueDouble = y;
			this.setColor(Color.FromArgb(255,0,0,0));
		}
		
		/* Gives mode of Y (type of values)
		 * @return string ... Type of values */
		public string getMode()
		{
			return this.modeY;
		}
		
		/* Gives number of values
		 * @return int ... Number of values */
		public int getCount()
		{
			switch(this.modeY) {
				case modeDouble:
					return this.valueDouble.Length;
					break;
					
				default:
					return 0;
					break;
			}
		}
		
		/* Gives minimum value
		 * @return double ... Minimum value */
		public double getMin()
		{
			double m = 0;
			for(int i=0; i<this.getCount(); i++) {
				// initial value
				if(i == 0) {
					m = this.getValue(i);
				} else {
					if(this.getValue(i) < m) {
						m = this.getValue(i);
					}
				}	
			}
			return m;
		}
		
		/* Gives maximum value
		 * @return double ... Maximum value */
		public double getMax()
		{
			double m = 0;
			for(int i=0; i<this.getCount(); i++) {
				// initial value
				if(i == 0) {
					m = this.getValue(i);
				} else {
					if(this.getValue(i) > m) {
						m = this.getValue(i);
					}
				}	
			}
			return m;		
		}
		
		/* Gives a value on specific index
		 * @param int index ... Index of desired value */
		public double getValue(int i) {
			if(i < this.getCount()) {
				return this.valueDouble[i];
			} else {
				throw new GraphException("Nelze získat hodnotu na pozici " + i.ToString() +
				                         ", maximální pozice je " + (this.getCount()-1).ToString());
			}
		}
		
		/* Sets color of line
		 * @param Color c ... Color
		 * @return Line   ... Provides a fluent interface */
		public Line setColor(Color c)
		{
			this.col = c;
			return this;
		}
		
		/* Gives Color of line
		 * @return Color */
		public Color getColor()
		{
			return this.col;
		}
		
		/* Set style of line
		 * @param string s ... Style wanted
		 * @return Line    ... Provides a fluent interface */
		public Line setStyle(string s)
		{
			this.style = s;
			return this;
		}
		
		/* Gives style
		 * @return string ... Current used style */
		public string getStyle()
		{
			return this.style;
		}						
	}
	
	/* Graph Exception */
	public class GraphException : TroopyException
	{
		/* Constructor
		 * @param string m ... Message */
		public GraphException(string message) : base(message) {}
	}
}
