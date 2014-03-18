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
		// Public methods		
		public static List<Tick> GetData(string ticker, string exchange, DateTime startDate, DateTime endDate) {
			// Set up the url request
			DownloadURIBuilder uriBuilder = new DownloadURIBuilder(exchange, ticker);
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

			return ParseStringData(resultValue);
		}
		public static Dictionary<string, List<Tick>> GetData(string[] tickerArray, string exchange, DateTime startDate, DateTime endDate) {
			Dictionary<string, List<Tick>> tickDictionary = new Dictionary<string, List<Tick>>();
	
			foreach (string ticker in tickerArray) {
				List<Tick> ticks = GetData(ticker, exchange, startDate, endDate);
				tickDictionary.Add(ticker, ticks);
			}

			return tickDictionary;
		}
		
		// Private methods
		private static List<Tick> ParseStringData(string dataString) {
			// Split into lines
			string[] lines = dataString.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			// Process		
			var query = from line in lines.Skip(1)
						let data = line.Split(',')
						select Tick.FromStringArray(data);
					
			return TickList.FromIEnumerable(query);
		}
	}
}
