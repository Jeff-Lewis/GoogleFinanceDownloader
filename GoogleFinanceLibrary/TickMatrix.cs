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
		public void Add(string symbol, DateTime date, Tick tick) {
			// Add to each dictionary
			if (!symbolDictionary.ContainsKey(symbol))
				symbolDictionary.Add(symbol, new TickList());
			symbolDictionary[symbol].Add(tick);

			if (!dateDictionary.ContainsKey(date))
				dateDictionary.Add(date, new TickList());
			dateDictionary[date].Add(tick);
		}
		public void Set(string symbol, TickList tickList) {
			if (symbolDictionary.ContainsKey(symbol))
				throw new Exception("Already contains ticks for symbol: " + symbol);
						
			foreach (Tick t in tickList)
				Add(symbol, t.Date, t);
		}
		public Tick Get(string symbol, DateTime date) {
			TickList symbolMatches = symbolDictionary[symbol];
			TickList dateMatches = dateDictionary[date];

			return symbolMatches.Intersect(dateMatches).First();
		}
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
		public List<string> GetAllSymbols() {
			return symbolDictionary.Keys.ToList();
		}
		public double GetAverageChangeOverDates(string symbol, DateTime startDate, int days) {
			try {
				TickList relevantTicks = GetMultiple(symbol, startDate, days);
				return relevantTicks.Average(t => t.GetChangePercent(true));
			} catch {
				// I really need a way to log this
				return 0;
			}
		}
	}
}
