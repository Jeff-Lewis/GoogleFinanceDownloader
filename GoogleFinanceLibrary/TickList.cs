using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {
	public class TickList : List<Tick> {
		// Factory method
		public static TickList FromIEnumerable(IEnumerable<Tick> ienumerable){
			TickList result = new TickList();
			result.AddRange(ienumerable);

			// Set the last ticks
			for (int i = 1; i < result.Count; i++)
				result[i].LastTick = result[i - 1];

			// Ensure its in order by date (it should be, but dont make assumptions)
			result.Sort();

			return result;
		}

		// Public methods
		public TickList GetSubsetByDate(DateTime startDate, int days) {
			int index = GetFutureIndex(startDate, days);
			return TickList.FromIEnumerable(this.GetRange(index, days));
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
		}

		// Private methods
		private int GetFutureIndex(DateTime startDate, int days) {
			// Move to the starting point
			int startIndex = this.FindIndex(t => t.Date == startDate);
			return startIndex + days;
		}
	}
}
