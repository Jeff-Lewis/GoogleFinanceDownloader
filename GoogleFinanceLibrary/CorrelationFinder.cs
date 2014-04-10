using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;



namespace GoogleFinanceLibrary {
	public class CorrelationFinder {
		// Private members
		private static readonly DateTime endDate = new DateTime(2014, 3, 25);
		private static readonly DateTime startDate = endDate.AddMonths(-3);
		private static readonly string[] exchanges = new string[] { /*"NYSE", "NASDAQ", "CURRENCY",*/ "" };
		private static readonly int[] futureDays = Enumerable.Range(1, 3).ToArray();
		private const double changePercentThreshold = 0.3;
		private const int topBottomCount = 50;
		private const int interestingPercentCutoff = 60;
		private const int interestingTickPercent = 50;
		private const int minimumTicksComparedCount = 50;		// 3 months data is about 60 ticks

		private static TickMatrix tickData = null;
		private static List<CorrelationResult> interestingResults = null;
		
		// Public methods
		public static List<CorrelationResult> Find(string dataFolder) {
			if (interestingResults == null) {				
				List<CorrelationResult> linearResultList = new List<CorrelationResult>();
				tickData = TickRetriever.GetData(exchanges, startDate, endDate, dataFolder);
				interestingResults = tickData.FindCorrelations(futureDays, topBottomCount, changePercentThreshold, interestingPercentCutoff, 
					interestingTickPercent, minimumTicksComparedCount);						
			}

			return interestingResults;
		}
		public static TickMatrix GetRawData() {
			return tickData;
		}		
	}
}
