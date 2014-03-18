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

			return result;
		}
	}
}
