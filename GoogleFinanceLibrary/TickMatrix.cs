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
			// Or that all dates have the same amount of ticks
			List<DateTime> problemDates = GetDatesWithMissingValues();
			RemoveDates(problemDates);
			List<DateTime> newProblemDates = GetDatesWithMissingValues();
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
		private List<DateTime> GetDatesWithMissingValues() {
			// Find how many ticks each date should have
			int maxTicks = 0;
			foreach (TickList tl in dateDictionary.Values)
				if (tl.Count > maxTicks)
					maxTicks = tl.Count;

			// See who is lacking
			List<DateTime> problemDates = new List<DateTime>();
			foreach (KeyValuePair<DateTime, TickList> keyValue in dateDictionary)
				if (keyValue.Value.Count < maxTicks)
					problemDates.Add(keyValue.Key);

			return problemDates;
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
	}
}
