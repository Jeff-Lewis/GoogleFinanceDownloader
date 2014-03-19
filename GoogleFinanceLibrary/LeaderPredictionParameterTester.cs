using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {
	public class LeaderPredictionParameterTester {
		// Constants
		private static readonly DateTime endDate = DateTime.Now;
		private static readonly DateTime startDate = DateTime.Now.AddMonths(-1);
		private const string indexTicker = "SPY";
		private const string exchange = "";

		// Private members
		private static string[][] leaderTickers = new string[][]{ 
			new string[] {"TSLA", "NFLX", "FB"}, 
			new string[] {"RUT"},  
			new string[] {"IBB"},  
		};
		private static int[] windowDays = {1, 2, 3, 4, 5, 6, 7, 8, 9};
		private static int[] futureDays = {1, 2, 3, 4, 5, 6, 7, 8, 9};

		// Public methods
		public static string Test() {
			// Run all combinations of tests
			List<PredictionDictionary> results = new List<PredictionDictionary>();
			foreach (string[] leaderTickerArray in leaderTickers)
				foreach (int windowDay in windowDays)
					foreach (int futureDay in futureDays) {
						PredictionDictionary pd = LeaderPrediction.Predict(leaderTickerArray, indexTicker, exchange, startDate, endDate, windowDay, futureDay);
						results.Add(pd);
					}

			// Check the results
			var sortedResults = results.OrderByDescending(r => r.GetDirectionAgreementPercent());
			StringBuilder sb = new StringBuilder();
			foreach (PredictionDictionary pd in sortedResults)
				sb.Append(pd.ToString());

			return sb.ToString();
		}
	}
}
