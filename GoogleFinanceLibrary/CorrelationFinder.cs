using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet.Numerics.Statistics;

namespace GoogleFinanceLibrary {
	public class CorrelationFinder {
		public class CorrelationResult : IComparable<CorrelationResult> {
			public double Coefficient { get; set; }
			public string PredictorSymbol { get; set; }
			public string PredicteeSymbol { get; set; }
			public int FutureDays { get; set; }
			public override string ToString() {
				return Math.Round(this.Coefficient, 2).ToString();
			}

			#region IComparable<CorrelationResult> Members

			public int CompareTo(CorrelationResult other) {
				return this.Coefficient.CompareTo(other.Coefficient);
			}

			#endregion
		}
		public class CorrelationResultList : List<CorrelationResult> {
			// Index is the future days
			public override string ToString() {
				string result = "";
				for (int i = 0; i < this.Count; i++)
					result += i + ":" + this[i] + ", ";

				return result;
			}
		}
		
		

		// Private members
		private static readonly DateTime endDate = new DateTime(2014, 3, 24);
		private static readonly DateTime startDate = DateTime.Now.AddMonths(-3);
		/*private static readonly string[] symbols = new string[]{ 
			"TSLA", 
			"NFLX", 
			"FB",
			"RUT", 
			"IBB" };*/
		private static readonly string[] exchanges = new string[] { "AMEX" };

		private static readonly int[] futureDays = Enumerable.Range(1, 3).ToArray();
		
		// Public methods
		public static string Find() {
			// Create the matrix			
			//CorrelationResultList[,] result = new CorrelationResultList[symbols.Length, symbols.Length];
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
					foreach (int futureDay in futureDays){
						double[] predictorTicks = predictorAxis.GetChangeExcludingEndDays(futureDay);
						double[] predicteeTicks = predicteeAxis.GetChangeExcludingStartDays(futureDay);
						double correlation = Correlation.Pearson(predictorTicks, predicteeTicks);

						CorrelationResult cr = new CorrelationResult() { 
							Coefficient = correlation,
							PredictorSymbol = symbols[i],
							PredicteeSymbol = symbols[j],
							FutureDays = futureDay
						};

						// Put the result into the matrix
						//result[i, j].Insert(futureDay, cr);					
						
						// Put the result into a linear list for sorting
						linearResultList.Add(cr);
					}
				}
			}

			// Sort the linear results
			linearResultList.Sort();			

			//return StringifyMatrix(result);
			return StringifyLinearList(linearResultList);
		}

		// Private methods
		private static TickMatrix GetDataByExchanges() {
			return TickRetriever.GetData(exchanges, startDate, endDate);
		}
		/*private static TickMatrix GetData() {
			return TickRetriever.GetData(symbols, string.Empty, startDate, endDate);
		}*/
		/*private static string StringifyMatrix(CorrelationResultList[,] correlationMatrix) {
			StringBuilder sb = new StringBuilder();

			// Create header
			sb.AppendFormat("{0,-20}", "");
			foreach (string symbol in symbols)
				sb.AppendFormat("{0,-40}", symbol);
			sb.AppendLine();

			// Create body
			for (int i = 0; i < symbols.Length; i++) {
				sb.AppendFormat("{0,-20}", symbols[i]);

				for (int j = 0; j < symbols.Length; j++) {
					sb.AppendFormat("{0,-40}", correlationMatrix[i, j]);
				}

				sb.AppendLine();
			}

			return sb.ToString();
		}*/
		private static string StringifyLinearList(List<CorrelationResult> resultList) {
			StringBuilder sb = new StringBuilder();

			foreach (CorrelationResult cr in resultList) {
				sb.AppendFormat("{0} -> {1}, {2} days: {3}", cr.PredictorSymbol, cr.PredicteeSymbol, cr.FutureDays, Math.Round(cr.Coefficient));
				sb.AppendLine();
			}

			return sb.ToString();
		}
	}
}
