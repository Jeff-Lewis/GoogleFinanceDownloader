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
		private static readonly string[] exchanges = new string[] { "NYSE", "NASDAQ", "CURRENCY" };
		private static readonly int[] futureDays = Enumerable.Range(1, 3).ToArray();
		private static readonly double changePercentThreshold = 0.3;
		private static readonly int topBottomCount = 50;
		private static readonly int interestingPercentCutoff = 60;

		private static TickMatrix tickData = null;
		private static List<CorrelationResult> interestingResults = null;
		
		// Public methods
		public static List<CorrelationResult> Find(string dataFolder) {
			if (interestingResults == null) {
				List<CorrelationResult> linearResultList = new List<CorrelationResult>();
				tickData = TickRetriever.GetData(exchanges, startDate, endDate, dataFolder);
				string[] symbols = tickData.GetAllSymbols().ToArray();

				// Compare each symbol to each other symbol
				for (int i = 0; i < symbols.Length; i++) {
					TickList predictorAxis = tickData.GetMultiple(symbols[i]);

					for (int j = 0; j < symbols.Length; j++) {
						TickList predicteeAxis = tickData.GetMultiple(symbols[j]);

						// Try each future day					
						foreach (int futureDay in futureDays) {
							double[] predictorTicks = predictorAxis.GetDataExcludingEndDays(futureDay, t => t.GetChangePercent(true));
							double[] predicteeTicks = predicteeAxis.GetDataExcludingStartDays(futureDay, t => t.GetChangePercent(false));
							//double correlation = Correlation.Pearson(predictorTicks, predicteeTicks);
							//double correlation = FinanceMath.GetSignAgreementPercent(predictorTicks, predicteeTicks);
							double positiveCorrelationPercent, negativeCorrelationPercent;
							FinanceMath.GetInterestingSignAgreementPercent(predictorTicks, predicteeTicks, changePercentThreshold, out positiveCorrelationPercent, out negativeCorrelationPercent);

							CorrelationResult cr = new CorrelationResult() {
								//Coefficient = correlation,
								PositiveSignAgreementPercent = Math.Round(positiveCorrelationPercent, 2),
								NegativeSignAgreementPercent = Math.Round(negativeCorrelationPercent, 2),
								PredictorSymbol = symbols[i],
								PredictorTicks = predictorTicks,
								PredicteeSymbol = symbols[j],
								PredicteeTicks = predicteeTicks,
								FutureDays = futureDay
							};


							// Put the result into a linear list for sorting
							if ((cr.PositiveSignAgreementPercent >= interestingPercentCutoff) || (cr.NegativeSignAgreementPercent >= interestingPercentCutoff))
								linearResultList.Add(cr);
						}
					}
				}

				// Only care about most strongly correlated, in either direction
				interestingResults = linearResultList.OrderByDescending(cr => cr.PositiveSignAgreementPercent).Take(topBottomCount).ToList();
				interestingResults.AddRange(linearResultList.OrderByDescending(cr => cr.NegativeSignAgreementPercent).Take(topBottomCount).ToList());
			}

			return interestingResults;
		}
		public static TickMatrix GetRawData() {
			return tickData;
		}
		/*
		public static CorrelationData GetCorrelationData(string predictorSymbol, string predicteeSymbol, int futureDays) {
			if (interestingResults == null)
				throw new Exception("Calculate results first");

			CorrelationResult cr = interestingResults.Where(c => (c.PredictorSymbol == predictorSymbol) && (c.PredicteeSymbol == predicteeSymbol) && (c.FutureDays == futureDays)).Single();

			CorrelationData cd = new CorrelationData() { PredictorChange = cr.PredictorTicks, PredicteeChange = cr.PredicteeTicks };

			return cd;
		}*/

		/*public static string StringifyLinearList(List<CorrelationResult> resultList) {
			StringBuilder sb = new StringBuilder();
			foreach (CorrelationResult cr in resultList)
				sb.AppendLine(cr.ToString());

			return sb.ToString();
		}	*/	
	}
}
