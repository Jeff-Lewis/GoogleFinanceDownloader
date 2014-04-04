using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {	
	public class TickList : List<Tick> {
		// Factory method
		public static TickList FromIEnumerable(IEnumerable<Tick> ienumerable, DateTime startDate){
			TickList result = new TickList();
			result.AddRange(ienumerable.Where(t => t.Date >= startDate));

			// Set the last ticks
			for (int i = 1; i < result.Count; i++)
				result[i].LastTick = result[i - 1];

			// Ensure its in order by date (it should be, but dont make assumptions)
			result.Sort();

			return result;
		}
		// Public methods
		/*public TickList GetSubsetByDate(DateTime startDate, int days) {
			int index = GetFutureIndex(startDate, 0);
			return TickList.FromIEnumerable(this.GetRange(index, days), startDate);
		}
		public DateTime GetFutureDate(DateTime startDate, int days) {			
			return GetFutureTick(startDate, days).Date;
		}
		public Tick GetFutureTick(DateTime startDate, int days) {
			try {
				int index = GetFutureIndex(startDate, days);
				return this[index];
			} catch (Exception ex) {
				throw new Exception("No future tick exists", ex);
			}
		}*/
		public double[] GetData(Func<Tick, double> selector) {
			return GetDataExcludingEndDays(0, selector);
		}
		public double[] GetDataExcludingEndDays(int days, Func<Tick, double> selector) {
			return this.Take(this.Count - days).Select(selector).ToArray();
		}
		public double[] GetDataExcludingStartDays(int days, Func<Tick, double> selector) {
			return this.Skip(days).Select(selector).ToArray();
		}
		public Tick GetTickByDate(DateTime date) {
			return this.FirstOrDefault(t => t.Date == date);
		}

		// Private methods
		/*private int GetFutureIndex(DateTime startDate, int days) {
			// Move to the starting point
			int startIndex = this.FindIndex(t => t.Date == startDate);
			return startIndex + days;
		}*/
	}
}
