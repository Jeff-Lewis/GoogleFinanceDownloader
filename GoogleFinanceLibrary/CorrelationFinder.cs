using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet.Numerics.Statistics;

namespace GoogleFinanceLibrary {
	public class CorrelationFinder {
		public class CorrelationResult {
			public double PositiveSignAgreementPercent { get; set; }
			public double NegativeSignAgreementPercent { get; set; }
			public string PredictorSymbol { get; set; }
			public string PredicteeSymbol { get; set; }
			public int FutureDays { get; set; }
			public override string ToString() {
				return string.Format("{0} -> {1}, {2} days: Positive%: {3}, Negative%: {4}", PredictorSymbol, PredicteeSymbol, FutureDays,
					Math.Round(PositiveSignAgreementPercent), Math.Round(NegativeSignAgreementPercent));
			}
		}
					

		// Private members
		private static readonly DateTime endDate = new DateTime(2014, 3, 25);
		private static readonly DateTime startDate = endDate.AddMonths(-3);
		private static readonly string[] exchanges = new string[] { "NYSE" };
		private static readonly int[] futureDays = Enumerable.Range(1, 3).ToArray();
		private static readonly double changePercentThreshold = 0.3;
		
		// Public methods
		public static string Find() {
			List<CorrelationResult> linearResultList = new List<CorrelationResult>();
			TickMatrix tickData = GetDataByExchanges();
			string[] symbols = tickData.GetAllSymbols().ToArray();

			// Compare each symbol to each other symbol
			for (int i = 0; i < symbols.Length; i++) {
				TickList predictorAxis = tickData.GetMultiple(symbols[i]);

				for (int j = 0; j < symbols.Length; j++) {
					TickList predicteeAxis = tickData.GetMultiple(symbols[j]);
					//result[i, j] = new CorrelationResultList();

					// Try each future day					
					foreach (int futureDay in futureDays) {
						double[] predictorTicks = predictorAxis.GetChangeExcludingEndDays(futureDay);
						double[] predicteeTicks = predicteeAxis.GetChangeExcludingStartDays(futureDay);
						//double correlation = Correlation.Pearson(predictorTicks, predicteeTicks);
						//double correlation = FinanceMath.GetSignAgreementPercent(predictorTicks, predicteeTicks);
						double positiveCorrelationPercent, negativeCorrelationPercent;
						FinanceMath.GetInterestingSignAgreementPercent(predictorTicks, predicteeTicks, changePercentThreshold, out positiveCorrelationPercent, out negativeCorrelationPercent);

						CorrelationResult cr = new CorrelationResult() {
							//Coefficient = correlation,
							PositiveSignAgreementPercent = positiveCorrelationPercent,
							NegativeSignAgreementPercent = negativeCorrelationPercent,
							PredictorSymbol = symbols[i],
							PredicteeSymbol = symbols[j],
							FutureDays = futureDay
						};
		

						// Put the result into a linear list for sorting
						if ((cr.PositiveSignAgreementPercent >= 50) || (cr.NegativeSignAgreementPercent >= 50))
							linearResultList.Add(cr);
					}
				}
			}					

			//return StringifyMatrix(result);
			return StringifyLinearList(linearResultList, 50);
		}

		// Private methods
		private static TickMatrix GetDataByExchanges() {
			return TickRetriever.GetData(exchanges, startDate, endDate);
		}
		
		private static string StringifyLinearList(List<CorrelationResult> resultList, int topBottomCount) {
			// Only care about most strongly correlated, in either direction
			List<CorrelationResult> interestingResults = resultList.OrderByDescending(cr => cr.PositiveSignAgreementPercent).Take(topBottomCount).ToList();
			interestingResults.AddRange(resultList.OrderByDescending(cr => cr.NegativeSignAgreementPercent).Take(topBottomCount).ToList());

			StringBuilder sb = new StringBuilder();
			foreach (CorrelationResult cr in interestingResults) 
				sb.AppendLine(cr.ToString());							

			return sb.ToString();
		}
	}
}
