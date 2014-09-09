using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {	
	public class TickList : SortedList<DateTime, Tick> {
		// Public properties
		public double PredictorInterestingTickPercent { get; private set; }
		public double PredicteeInterestingTickPercent { get; private set; }
		public double AveragePrice { get; private set; }

		// Factory method
		public static TickList FromIEnumerable(IEnumerable<Tick> tickEnumerable, CorrelationConfig config){
			TickList result = new TickList();

			// Add
			int totalTickCount = 0, predictorInterestingTickCount = 0, predicteeInterestingTickCount = 0;
			foreach (Tick t in tickEnumerable) {
				if ((t.Date >= config.StartDate) && (t.Date <= config.EndDate)) {
					totalTickCount++;
					result.Add(t.Date, t);

					if (t.IsChangeInteresting(true, config.PredictorChangePercentThreshold))
						predictorInterestingTickCount++;

					if (t.IsChangeInteresting(true, config.PredicteeChangePercentThreshold))
						predicteeInterestingTickCount++;
				}
			}

			result.PredictorInterestingTickPercent = ((double)predictorInterestingTickCount) / ((double)totalTickCount) * 100;
			result.PredicteeInterestingTickPercent = ((double)predicteeInterestingTickCount) / ((double)totalTickCount) * 100;

			// Set Average price
			result.AveragePrice = result.Average(t => t.Value.ClosePrice);
			
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
