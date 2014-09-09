using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {	
	public class Tick : IComparable<Tick> {
		// Public properties
		public string SymbolWithExchange { get; private set; }
		public DateTime Date {get; private set;}
		public double OpenPrice {get; private set;}
		public double HighPrice { get; private set; }
		public double LowPrice { get; private set; }
		public double ClosePrice { get; private set; }
		public double Volume { get; private set; }
		public Tick LastTick { get; set; }
		
		// Private members
		private double? ChangePercentSinceLastClose = null;
		private double? ChangePercentSinceOpen = null;
		
		// Factory method
		public static Tick FromStringArray(string exchange, string symbol, string[] data) {
			return new Tick {
				SymbolWithExchange = exchange + ":" + symbol,
				Date = DateTime.Parse(data[0]),
				OpenPrice = float.Parse(data[1]),
				HighPrice = float.Parse(data[2]),
				LowPrice = float.Parse(data[3]),
				ClosePrice = float.Parse(data[4]),
				Volume = float.Parse(data[5])
			};
		}
		
		// Public methods
		public double GetChangePercent(bool sinceLastClose) {
			if (sinceLastClose && (LastTick != null)) {
				if (ChangePercentSinceLastClose == null)
					ChangePercentSinceLastClose = (ClosePrice - LastTick.ClosePrice) / LastTick.ClosePrice * 100;
				
				if (double.IsInfinity(ChangePercentSinceLastClose.Value))
					ChangePercentSinceLastClose = 0;				

				return ChangePercentSinceLastClose.Value;
			} else {
				if (ChangePercentSinceOpen == null)
					ChangePercentSinceOpen = (ClosePrice - OpenPrice) / OpenPrice * 100;
				
				if (double.IsInfinity(ChangePercentSinceOpen.Value))
					ChangePercentSinceOpen = 0;
				
				return ChangePercentSinceOpen.Value;
			}
		}
		public bool IsChangeInteresting(bool sinceLastClose, double minimumInterestingChangePercent) {
			double changePercent = GetChangePercent(sinceLastClose);
			if (Math.Abs(changePercent) >= minimumInterestingChangePercent)
				return true;
			else
				return false;
		}

		#region IComparable<Tick> Members

		public int CompareTo(Tick other) {
			return this.Date.CompareTo(other.Date);
		}

		#endregion
	}
}
