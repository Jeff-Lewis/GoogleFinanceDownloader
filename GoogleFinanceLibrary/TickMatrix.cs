using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {
	public class TickMatrix {
		// Private members
		private Dictionary<string, TickList> symbolDictionary = new Dictionary<string, TickList>();
		private Dictionary<DateTime, TickList> dateDictionary = new Dictionary<DateTime, TickList>();

		// Public methods
		/*
		public void VerifyDates() {
			// Ensure all tick lists have the same amount of dates
			
			List<string> problemSymbols = GetKeysWithMissingValues<string>(symbolDictionary);
			RemoveKeys<string, DateTime>(problemSymbols, symbolDictionary, dateDictionary, t => t.SymbolWithExchange);
			
			// Or that all dates have the same amount of ticks
			List<DateTime> problemDates = GetKeysWithMissingValues<DateTime>(dateDictionary);			
			RemoveKeys<DateTime, string>(problemDates, dateDictionary, symbolDictionary, t => t.Date); 
			List<DateTime> newProblemDates = GetKeysWithMissingValues<DateTime>(dateDictionary);
		}
		*/
		public void Add(string symbolWithExchange, DateTime date, Tick tick) {
			// Add to each dictionary
			if (!symbolDictionary.ContainsKey(symbolWithExchange))
				symbolDictionary.Add(symbolWithExchange, new TickList());
			symbolDictionary[symbolWithExchange].Add(tick);

			if (!dateDictionary.ContainsKey(date))
				dateDictionary.Add(date, new TickList());
			dateDictionary[date].Add(tick);
		}
		public void Set(string symbolWithExchange, TickList tickList) {
			if (symbolDictionary.ContainsKey(symbolWithExchange))
				throw new Exception("Already contains ticks for symbol: " + symbolWithExchange);
						
			foreach (Tick t in tickList)
				Add(symbolWithExchange, t.Date, t);
		}
		/*public Tick Get(string symbol, DateTime date) {
			TickList symbolMatches = symbolDictionary[symbol];
			TickList dateMatches = dateDictionary[date];

			return symbolMatches.Intersect(dateMatches).First();
		}*/
		/*public TickList GetMultiple(string symbol, DateTime startDate, int days) {			
			return GetMultiple(symbol).GetSubsetByDate(startDate, days);			
		}*/
		public TickList GetMultiple(string symbol) {
			return symbolDictionary[symbol];
		}
		public TickList GetMultiple(DateTime date) {
			return dateDictionary[date];
		}
		public List<DateTime> GetAllDates() {
			return dateDictionary.Keys.ToList();
		}		
		/*public double GetAverageChangeOverDates(string symbol, DateTime startDate, int days) {
			try {
				TickList relevantTicks = GetMultiple(symbol, startDate, days);
				double startValue = relevantTicks.First().LastTick.ClosePrice;
				double endValue = relevantTicks.Last().ClosePrice;
				double changePercent = (endValue - startValue) / startValue * 100;
				return Math.Round(changePercent / days, 2);				
			} catch (ArgumentException) {
				// I really need a way to log this
				return 0;
			}
		}*/
		public IEnumerable<string> GetAllSymbols() {
			return symbolDictionary.Keys;
		}
		public List<CorrelationResult> FindCorrelations(int[] futureDayArray, int topBottomCount, double changePercentThreshold, int interestingPercentCutoff)
		{
			List<CorrelationResult> linearResultList = new List<CorrelationResult>();				
			string[] symbols = GetAllSymbols().ToArray();

			// Compare each symbol to each other symbol
			for (int i = 0; i < symbols.Length; i++) {
				TickList predictorAxis = GetMultiple(symbols[i]);
				predictorAxis.Sort();				

				for (int j = 0; j < symbols.Length; j++) {
					TickList predicteeAxis = GetMultiple(symbols[j]);
					predicteeAxis.Sort();

					// Try each future day					
					foreach (int futureDay in futureDayArray) {
						CorrelationResult cr = GetCorrelation(predictorAxis, predicteeAxis, futureDay, changePercentThreshold);

						// Put the result into a linear list for sorting
						if ((cr.PositiveSignAgreementPercent >= interestingPercentCutoff) || (cr.NegativeSignAgreementPercent >= interestingPercentCutoff))
							linearResultList.Add(cr);
					}
				}
			}

			// Only care about most strongly correlated, in either direction			
			List<CorrelationResult> interestingResults = linearResultList.OrderByDescending(cr => cr.PositiveSignAgreementPercent).Take(topBottomCount).ToList();
			interestingResults.AddRange(linearResultList.OrderByDescending(cr => cr.NegativeSignAgreementPercent).Take(topBottomCount).ToList());

			return interestingResults;
		}
		
		// Private methods		
		private static CorrelationResult GetCorrelation(TickList predictorAxis, TickList predicteeAxis, int futureDays, double thresholdPercent) {
			int positiveAgreementCount = 0;
			int negativeAgreementCount = 0;

			// Go through each date in the predictor
			for (int i = 0; i < predictorAxis.Count; i++) {				
				// Find its date 'future days' ahead
				int futureDateIndex = i + futureDays;
				if (futureDateIndex >= predictorAxis.Count)
					continue;
	
				Tick predictorTick = predictorAxis[i];
				DateTime futureDate = predictorAxis[futureDateIndex].Date;

				// Get the corresponding date for the predictee
				Tick predicteeTick = predicteeAxis.GetTickByDate(futureDate);
				if (predicteeTick == null)
					continue;
		
				// Check for agreement
				double predictorChange = predictorTick.GetChangePercent(true);
				double predicteeChange = predictorTick.GetChangePercent(false);

				FinanceMath.SignAgreement doesAgree = FinanceMath.GetSignAgreement(predictorChange, predicteeChange, thresholdPercent);

				if (doesAgree == FinanceMath.SignAgreement.Positive)
					positiveAgreementCount++;
				else if (doesAgree == FinanceMath.SignAgreement.Negative)
					negativeAgreementCount++;
			}

			double count = Math.Max(predictorAxis.Count, predicteeAxis.Count);
			CorrelationResult result = new CorrelationResult() {
				 FutureDays = futureDays,
				 PredictorSymbol = predictorAxis.First().SymbolWithExchange, 
				 PredictorTicks = predictorAxis.GetData(t => t.ClosePrice),
				 PredicteeSymbol = predicteeAxis.First().SymbolWithExchange, 
				 PredicteeTicks = predicteeAxis.GetData(t => t.ClosePrice),
				 PositiveSignAgreementPercent = ((double)positiveAgreementCount) / count * 100, 
				 NegativeSignAgreementPercent = ((double)negativeAgreementCount) / count * 100				 
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
		/*private void RemoveDates(List<DateTime> dates) {
			foreach (DateTime date in dates) {
				dateDictionary.Remove(date);

				foreach (TickList tl in symbolDictionary.Values) {
					// Set the new last time
					foreach (Tick t in tl)
						if (t.Date == date) 
							t.LastTick = t.LastTick.LastTick;						
										
					tl.RemoveAll(t => t.Date == date);
				}
			}
		}
		private static void RemoveKeys<TPrimary, TSecondary>(List<TPrimary> keys, Dictionary<TPrimary, TickList> primaryDictionary, Dictionary<TSecondary, TickList> secondaryDictionary,
			Func<Tick, TPrimary> selector) {
			foreach (TPrimary key in keys) {
				primaryDictionary.Remove(key);

				foreach (TickList tl in secondaryDictionary.Values) {
					// Set the new last time
					foreach (Tick t in tl)
						if (selector(t).Equals(key))
							if (t.LastTick != null)
								t.LastTick = t.LastTick.LastTick;

					tl.RemoveAll(t => selector(t).Equals(key));
				}
			}
		}
		*/
	}
}
