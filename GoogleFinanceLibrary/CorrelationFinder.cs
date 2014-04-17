using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;



namespace GoogleFinanceLibrary {
	public class CorrelationFinder {
		// Private members		
		private static CorrelationConfig config = CorrelationConfig.Instance;
		private static TickMatrix tickData = null;
		private static List<CorrelationResult> interestingResults = null;
		
		// Public methods
		public static List<CorrelationResult> Find(string dataFolder) {
			if (interestingResults == null) {				
				List<CorrelationResult> linearResultList = new List<CorrelationResult>();
				tickData = TickRetriever.GetData(dataFolder, config);
				interestingResults = tickData.FindCorrelations(config);						
			}

			return interestingResults;
		}
		public static TickMatrix GetRawData() {
			return tickData;
		}		
	}
}
