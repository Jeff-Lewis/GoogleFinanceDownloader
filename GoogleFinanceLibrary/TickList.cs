using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {	
	public class TickList : SortedList<DateTime, Tick> {
		// Constants
		//private const double MinimumPercentInterestingTicks = 0.50;

		// Public properties
		public double InterestingTickPercent { get; private set; }

		// Factory method
		public static TickList FromIEnumerable(IEnumerable<Tick> tickEnumerable, DateTime startDate){
			TickList result = new TickList();

			// Add
			int totalTickCount = 0, interestingTickCount = 0;
			foreach (Tick t in tickEnumerable) {
				if (t.Date >= startDate) {
					totalTickCount++;
					result.Add(t.Date, t);

					if (t.IsChangeInteresting(true))
						interestingTickCount++;					
				}
			}

			result.InterestingTickPercent = ((double)interestingTickCount) / ((double)totalTickCount) * 100;
			
			// Set last ticks
			for (int i = 1; i < result.Count; i++) 
				result.Values[i].LastTick = result.Values[i - 1];
			
			return result;
		}
		// Public methods
		public double[] GetData(Func<Tick, double> selector) {
			return GetDataExcludingEndDays(0, selector);
		}
		public double[] GetDataExcludingEndDays(int days, Func<Tick, double> selector) {
			return this.Values.Take(this.Count - days).Select(selector).ToArray();
		}
		public double[] GetDataExcludingStartDays(int days, Func<Tick, double> selector) {
			return this.Values.Skip(days).Select(selector).ToArray();
		}
		public Tick GetTickByDate(DateTime date) {
			Tick result;
			this.TryGetValue(date, out result);

			return result;
		}		
	}
}
