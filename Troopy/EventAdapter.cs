using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Troopy
{
	/// <summary>
	/// V1.01
	/// Added etMouseEnter
	/// </summary>
	public class EventAdapter
	{		
		public const string etClick = "etClick";
		public const string etKeyDown = "keyDown";
		public const string etMouseClick = "mouseClick";
		public const string etMouseMove = "mouseMove";
		public const string etMouseEnter = "mouseEnter";
		public const string etMouseLeave = "mouseLeave";
		
		public const string etPaint = "paint";
		
		public const string etCellClick = "cellClick";
		public const string etSorted = "sorted";
		
		private List<Control> controls = new List<Control>(){};
		public List<Control> Controls {
			get{
				return this.controls;
			}
		}
		
		private List<string> eventTypes = new List<string>(){};
		public List<string> EventTypes {
			get {
				return this.eventTypes;
			}
		}
		
		private List<Delegate> functions = new List<Delegate>(){};
		public List<Delegate> Functions {
			get {
				return this.functions;
			}
		}
		
		public EventAdapter()
		{
		}
		
		public void addEvent(Control ctrl, string eventType, Delegate func)
		{
			this.controls.Add(ctrl);
			this.functions.Add(func);
			this.eventTypes.Add(eventType);
					
			switch(eventType) {
				
				case etKeyDown:
					ctrl.KeyDown += (KeyEventHandler)func;
					break;

				case etClick:
					ctrl.Click += (EventHandler)func;
					break;
					
				case etMouseClick:
					ctrl.MouseClick += (MouseEventHandler)func;
					break;
				
				case etCellClick:
					DataGridView tc = (DataGridView)ctrl;
					tc.CellClick += (DataGridViewCellEventHandler)func;
					break;

				case etMouseEnter:
					ctrl.MouseEnter += (EventHandler)func;
					break;					
					
				case etMouseLeave:
					ctrl.MouseLeave += (EventHandler)func;
					break;
					
				case etMouseMove:
					ctrl.MouseMove += (MouseEventHandler)func;
					break;
					
				case etPaint:
					ctrl.Paint += (PaintEventHandler)func;
					break;
					
				case etSorted:
					DataGridView tempDgv = (DataGridView)ctrl;
					tempDgv.Sorted += (EventHandler)func;
					break;					
			}
		}
		
		public void clearEvents()
		{
			if(this.controls.Count > 0) {
				for(int i=0; i<this.controls.Count; i++) {
					Control ctrl = this.Controls[i];
					string eventType = this.EventTypes[i];
					Delegate func = this.functions[i];
					
					switch(eventType) {
						
						case etKeyDown:
							ctrl.KeyDown -= (KeyEventHandler)func;
							break;

						case etClick:
							ctrl.Click -= (EventHandler)func;
							break;
							
						case etMouseClick:
							ctrl.MouseClick -= (MouseEventHandler)func;
							break;

						case etCellClick:
							DataGridView tc = (DataGridView)ctrl;
							tc.CellClick -= (DataGridViewCellEventHandler)func;
							break;

						case etMouseEnter:
							ctrl.MouseEnter -= (EventHandler)func;
							break;							
							
						case etMouseLeave:
							ctrl.MouseLeave -= (EventHandler)func;
							break;
							
						case etMouseMove:
							ctrl.MouseMove -= (MouseEventHandler)func;
							break;
							
						case etPaint:
							ctrl.Paint -= (PaintEventHandler)func;
							break;		

						case etSorted:
							DataGridView tempDgv = (DataGridView)ctrl;
							tempDgv.Sorted -= (EventHandler)func;
							break;							
					}
				}
			}
		}
	}
}
