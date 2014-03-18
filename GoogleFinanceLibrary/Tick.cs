using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {
	public class Tick {
		// Public properties
		public DateTime Date {get; private set;}
		public float OpenPrice {get; private set;}
		public float HighPrice {get; private set;}
		public float LowPrice {get; private set;}
		public float ClosePrice {get; private set;}
		public float Volume {get; private set;}
		public Tick LastTick { get; set; }
		/*
		public float ChangeSinceOpen {
			get {
				return ClosePrice - OpenPrice;
			}
		}
		public float ChangeSinceOpenPercent {
			get {
				return ChangeSinceOpen / OpenPrice * 100;
			}
		}
		public float ChangeSinceLastClose {
			get {
				return ClosePrice - LastTick.ClosePrice;
			}
		}
		public float ChangeSinceLastClosePercent {
			get {
				return ChangeSinceLastClose / LastTick.ClosePrice * 100;
			}
		}*/

		// Factory method
		public static Tick FromStringArray(string[] data) {
			return new Tick {
				Date = DateTime.Parse(data[0]),
				OpenPrice = float.Parse(data[1]),
				HighPrice = float.Parse(data[2]),
				LowPrice = float.Parse(data[3]),
				ClosePrice = float.Parse(data[4]),
				Volume = float.Parse(data[5])
			};
		}
		
		// Public methods
		public float GetChangePercent(bool sinceLastClose) {
			float lastPrice = (sinceLastClose && LastTick != null) ? LastTick.ClosePrice : OpenPrice;

			return (ClosePrice - lastPrice) / lastPrice * 100;
		}
	}
}
