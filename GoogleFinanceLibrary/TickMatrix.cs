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
		public void VerifyDates() {
			// Ensure all tick lists have the same amount of dates
			/*
			List<string> problemSymbols = GetKeysWithMissingValues<string>(symbolDictionary);
			RemoveKeys<string, DateTime>(problemSymbols, symbolDictionary, dateDictionary, t => t.SymbolWithExchange);
			*/
			// Or that all dates have the same amount of ticks
			List<DateTime> problemDates = GetKeysWithMissingValues<DateTime>(dateDictionary);			
			RemoveKeys<DateTime, string>(problemDates, dateDictionary, symbolDictionary, t => t.Date); 
			List<DateTime> newProblemDates = GetKeysWithMissingValues<DateTime>(dateDictionary);
		}
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
		public TickList GetMultiple(string symbol, DateTime startDate, int days) {			
			return GetMultiple(symbol).GetSubsetByDate(startDate, days);			
		}
		public TickList GetMultiple(string symbol) {
			return symbolDictionary[symbol];
		}
		public TickList GetMultiple(DateTime date) {
			return dateDictionary[date];
		}
		public List<DateTime> GetAllDates() {
			return dateDictionary.Keys.ToList();
		}		
		public double GetAverageChangeOverDates(string symbol, DateTime startDate, int days) {
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
		}
		public IEnumerable<string> GetAllSymbols() {
			return symbolDictionary.Keys;
		}
		
		// Private methods		
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
		private void RemoveDates(List<DateTime> dates) {
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
	}
}
