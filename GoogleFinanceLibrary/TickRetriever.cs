using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GoogleFinanceDownloader;

namespace GoogleFinanceLibrary {
	public class TickRetriever {
		// Private members
		private static Dictionary<string, TickList> tickCache = new Dictionary<string, TickList>();

		// Public methods		
		public static TickList GetData(string symbol, string exchange, DateTime startDate, DateTime endDate) {
			// If we already have the data, dont get it again
			string cacheKey = symbol + exchange + startDate + endDate;
			TickList result;
			if (!tickCache.TryGetValue(cacheKey, out result))
			{
				// Set up the url request
				DownloadURIBuilder uriBuilder = new DownloadURIBuilder(exchange, symbol);
				string url = uriBuilder.getGetPricesUrlForRecentData(startDate, endDate);

				// Get the data
				string downloadedData;
				using (WebClient wClient = new WebClient()) {
					downloadedData = wClient.DownloadString(url);
				}

				string resultValue;
				using (MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(downloadedData))) {
					DataProcessor dp = new DataProcessor();
					string errorMessage;
					resultValue = dp.processStreamMadeOfOneDayLinesToExtractHistoricalData(ms, out errorMessage);

					if (!String.IsNullOrEmpty(errorMessage))
						throw new Exception(errorMessage);
				}

				result = ParseStringData(symbol, resultValue);

				// Put into cache
				tickCache.Add(cacheKey, result);
			}

			return result;
		}
		public static TickMatrix GetData(string[] symbolArray, string exchange, DateTime startDate, DateTime endDate) {			
			TickMatrix result = new TickMatrix();

			foreach (string symbol in symbolArray) {
				TickList ticks = GetData(symbol, exchange, startDate, endDate);
				result.Set(symbol, ticks);				
			}

			return result;
		}
		
		// Private methods
		private static TickList ParseStringData(string symbol, string dataString) {
			// Split into lines
			string[] lines = dataString.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			// Process		
			var query = from line in lines.Skip(1)
						let data = line.Split(',')
						select Tick.FromStringArray(symbol, data);
					
			return TickList.FromIEnumerable(query);
		}
	}
}
