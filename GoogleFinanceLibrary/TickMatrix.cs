using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {
	public class TickMatrix {
		// Private members
		private Dictionary<string, TickList> symbolDictionary = new Dictionary<string, TickList>();		
		
		// Public methods				
		public void Set(string symbolWithExchange, TickList tickList) {
			if (symbolDictionary.ContainsKey(symbolWithExchange))
				throw new Exception("Already contains ticks for symbol: " + symbolWithExchange);

			symbolDictionary.Add(symbolWithExchange, tickList);
		}		
		public TickList GetMultiple(string symbol) {
			return symbolDictionary[symbol];
		}		
		public IEnumerable<string> GetAllSymbols() {
			return symbolDictionary.Keys;
		}
		public List<CorrelationResult> FindCorrelations(CorrelationConfig config)
		{
			List<CorrelationResult> linearResultList = new List<CorrelationResult>();				
			string[] symbols = GetAllSymbols().ToArray();

			// Compare each symbol to each other symbol
			for (int i = 0; i < symbols.Length; i++) {
				TickList predictorAxis = GetMultiple(symbols[i]);

				if ((predictorAxis.PredictorInterestingTickPercent <= config.InterestingTickPercent) || (predictorAxis.Count < 1) || (predictorAxis.AveragePrice < config.MinimumStockPriceDollars))
					continue;

				for (int j = 0; j < symbols.Length; j++) {
					TickList predicteeAxis = GetMultiple(symbols[j]);

					if ((predicteeAxis.PredicteeInterestingTickPercent <= config.InterestingTickPercent) || (predicteeAxis.Count < 1) || (predicteeAxis.AveragePrice < config.MinimumStockPriceDollars))
						continue;

					// Try each future day					
					foreach (int futureDay in config.FutureDays) {
						// For the same day, dont check against yourself
						if ((futureDay == 0) && (symbols[i] == symbols[j]))
							continue;

						CorrelationResult cr = GetCorrelation(predictorAxis, predicteeAxis, futureDay, config);

						if (cr == null)
							continue;

						// Put the result into a linear list for sorting
						if ((cr.PositiveSignAgreementPercent >= config.InterestingSignAgreementPercentCutoff) || (cr.NegativeSignAgreementPercent >= config.InterestingSignAgreementPercentCutoff))
							linearResultList.Add(cr);
					}
				}
			}

			// Only care about most strongly correlated, in either direction			
			List<CorrelationResult> interestingResults = linearResultList.OrderByDescending(cr => cr.PositiveSignAgreementPercent).Take(config.TopBottomCount).ToList();
			interestingResults.AddRange(linearResultList.OrderByDescending(cr => cr.NegativeSignAgreementPercent).Take(config.TopBottomCount).ToList());

			return interestingResults;
		}
		
		// Private methods			
		private static CorrelationResult GetCorrelation(TickList predictorAxis, TickList predicteeAxis, int futureDay, CorrelationConfig config) {
			int positiveAgreementCount = 0;
			int negativeAgreementCount = 0;			

			// Keep track of compared changes
			Dictionary<DateTime, Tuple<double, double>> comparedPercentChanges = new Dictionary<DateTime, Tuple<double, double>>();

			// Go through each date in the predictor
			for (int i = 0; i < predictorAxis.Count; i++) {				
				// Find its date 'future days' ahead
				int futureDateIndex = i + futureDay;
				if (futureDateIndex >= predictorAxis.Count)
					continue;
	
				Tick predictorTick = predictorAxis.Values[i];
				DateTime futureDate = predictorAxis.Values[futureDateIndex].Date;

				// Get the corresponding date for the predictee
				Tick predicteeTick = predicteeAxis.GetTickByDate(futureDate);
				if (predicteeTick == null)
					continue;
		
				// Check for agreement
				double predictorChange = predictorTick.GetChangePercent(true);
				double predicteeChange = predicteeTick.GetChangePercent(false);

				FinanceMath.SignAgreement doesAgree = FinanceMath.GetSignAgreement(predictorChange, predicteeChange, config.PredictorChangePercentThreshold, config.PredicteeChangePercentThreshold);
				comparedPercentChanges.Add(predicteeTick.Date, new Tuple<double, double>(predictorChange, predicteeChange));
								
				if (doesAgree == FinanceMath.SignAgreement.Positive)
					positiveAgreementCount++;
				else if (doesAgree == FinanceMath.SignAgreement.Negative)
					negativeAgreementCount++;
			}

			// Only create a real result if there are a comparable amount of points in predictor and predictee
			if (comparedPercentChanges.Count <= config.MinimumTicksComparedCount)
				return null;

			CorrelationResult result = new CorrelationResult() {
				 FutureDays = futureDay,
				 PredictorSymbol = predictorAxis.Values.First().SymbolWithExchange, 
				 PredictorTicks = predictorAxis.GetData(t => t.ClosePrice),
				 PredicteeSymbol = predicteeAxis.Values.First().SymbolWithExchange, 
				 PredicteeTicks = predicteeAxis.GetData(t => t.ClosePrice),
				 PositiveSignAgreementPercent = Math.Round(((double)positiveAgreementCount) / ((double)comparedPercentChanges.Count) * 100, 2),
				 NegativeSignAgreementPercent = Math.Round(((double)negativeAgreementCount) / ((double)comparedPercentChanges.Count) * 100, 2),
			 	 CheckedTickCount = comparedPercentChanges.Count,
				 ComparedTickPercentChanges = comparedPercentChanges
			};

			return result;
		}
		private static List<T> GetKeysWithMissingValues<T>(Dictionary<T, TickList> dictionary) {
			// Find how many ticks each date should have
			int maxTicks = 0;
			foreach (TickList tl in dictionary.Values)
				if (tl.Count > maxTicks)
					maxTicks = tl.Count;

			// See who is lacking
			List<T> problems = new List<T>();
			foreach (KeyValuePair<T, TickList> keyValue in dictionary)
				if (keyValue.Value.Count < maxTicks)
					problems.Add(keyValue.Key);

			return problems;
		}	
	}
}
