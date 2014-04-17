using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GoogleFinanceDownloader;
using Microsoft.VisualBasic.FileIO;

namespace GoogleFinanceLibrary {
	public class TickRetriever {
		// Constants
		private static readonly string fileSuffix = "companylist.csv";

		// Private members
		private static Dictionary<string, TickList> tickCache = new Dictionary<string, TickList>();
				
		// Public methods		
		public static TickList GetData(string symbol, string exchange, string dataFolder, CorrelationConfig config) {
			// If we already have the data, dont get it again					
			string file = (exchange + symbol + config.StartDate.ToShortDateString() + config.EndDate.ToShortDateString()).Replace('/', '_');
			string fileName = Path.Combine(dataFolder, file);
			string resultValue;

			if (File.Exists(fileName))
				resultValue = ReadFromFile(fileName);
			else {
				// Set up the url request
				DownloadURIBuilder uriBuilder = new DownloadURIBuilder(exchange, symbol);
				string url = uriBuilder.getGetPricesUrlForRecentData(config.StartDate, config.EndDate);

				// Get the data
				string downloadedData;
				using (WebClient wClient = new WebClient()) {
					downloadedData = wClient.DownloadString(url);
				}
								
				using (MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(downloadedData))) {
					DataProcessor dp = new DataProcessor();
					string errorMessage;
					resultValue = dp.processStreamMadeOfOneDayLinesToExtractHistoricalData(ms, out errorMessage);

					if (!String.IsNullOrEmpty(errorMessage))
						throw new Exception(errorMessage);
				}

				WriteToFile(resultValue, fileName);
			}

			TickList result = ParseStringData(exchange, symbol, resultValue, config);								

			return result;
		}
		public static TickMatrix GetData(string dataFolder, CorrelationConfig config) {
			TickMatrix result = new TickMatrix();
			List<string> symbolMasterList = new List<string>();
			foreach (string exchange in config.Exchanges) {
				// Get the symbols
				List<string> symbols = ReadSymbolsFromFile(exchange + fileSuffix, dataFolder);

				int failedSymbolCount = 0;
				foreach (string symbol in symbols) {
					try {
						TickList ticks = GetData(symbol, exchange, dataFolder, config);						
						result.Set(exchange + ":" + symbol, ticks);
					} catch (Exception ex) {
						// Maybe theres no data for this symbol.  Screw it.
						failedSymbolCount++;
					}
				}

				// TODO: Remove this
				object o = failedSymbolCount;
			}			

			return result;
		}
		
		// Private methods
		private static TickList ParseStringData(string exchange, string symbol, string dataString, CorrelationConfig config) {
			// Split into lines
			string[] lines = dataString.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			// Process		
			var query = from line in lines.Skip(1)
						let data = line.Split(',')
						select Tick.FromStringArray(exchange, symbol, data);

			return TickList.FromIEnumerable(query, config);
		}
		private static void WriteToFile(string contents, string fileName) {			
			using (TextWriter tw = new StreamWriter(File.OpenWrite(fileName))) 
				tw.Write(contents);			
		}
		private static string ReadFromFile(string fileName) {
			using (TextReader tr = new StreamReader(File.OpenRead(fileName))) 
				return tr.ReadToEnd();
		}
		private static List<string> ReadSymbolsFromFile(string file, string dataFolder) {
			string fileName = Path.Combine(dataFolder, file);

			List<string> symbols = new List<string>();
			using (TextFieldParser csvParser = new TextFieldParser(fileName) {
				TextFieldType = FieldType.Delimited,
				Delimiters = new string[] { "," },
				HasFieldsEnclosedInQuotes = true,
				CommentTokens = new string[] { "\"Symbol\"", "Symbol" }
			}) {				
				
				while (!csvParser.EndOfData) {
					string[] fields = csvParser.ReadFields();
					symbols.Add(fields[0]);
				}
			}

			return symbols;
		}
	}
}
