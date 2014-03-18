using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GoogleFinanceDownloader;

namespace GoogleFinanceLibrary
{
    public class LeaderPrediction
    {
		private class Prediction{
			public Dictionary<string, float> ChangePerTicker { get; set; }
			//public List<Tick> PredictorTicks { get; set;}
			public Tick ActualTick { get; set; }
			//public bool IsDirectionAccurate { get; set; }
			//public float MagnitudeRatio { get; set; }
			
		}

		// Public methods
		public static string Predict(string[] leaderTickerArray, string indexTicker, string exchange, DateTime startDate, DateTime endDate, int leaderWindowDays, int futureDays) {
			// Get leader ticker data
			Dictionary<string, List<Tick>> leaderTicks = TickRetriever.GetData(leaderTickerArray, exchange, startDate, endDate);

			// Get index data
			List<Tick> indexTicks = TickRetriever.GetData(indexTicker, exchange, startDate, endDate);

			Dictionary<DateTime, Prediction> predictionDictionary = GetPredictions(leaderTicks, indexTicks, leaderWindowDays, futureDays);			

			return GetStringOutput(
		}

		// Private methods
		private static string GetStringOutput(Dictionary<DateTime, Prediction> predictionDictionary) {
			StringBuilder sb = new StringBuilder();

			// Header
			Prediction firstPrediction = predictionDictionary.Values.First();
			sb.Append("Date");
			foreach (string ticker in firstPrediction.ChangePerTicker.Keys)
				sb.Append("\t").Append(ticker);
			sb.AppendLine();

			// Body
			foreach (KeyValuePair<DateTime, Prediction> keyValue in predictionDictionary) {
				sb.Append(keyValue.Key);
				foreach (KeyValuePair<string, float> changePerTickerKeyValue in keyValue.Value.ChangePerTicker) {
					sb.Append("\t").Append(changePerTickerKeyValue.Key + "(" + changePerTickerKeyValue.Value + ")");
				}
				sb.AppendLine();
			}

			return sb.ToString();
		}
		private static Dictionary<DateTime, Prediction> GetPredictions(Dictionary<string, List<Tick>> leaderTickDictionary, List<Tick> indexTicks, int leaderWindowDays, int futureDays){
			// Check slope of lead data against future slope of index data
			Dictionary<DateTime, Prediction> predictionDictionary = new Dictionary<DateTime, Prediction>();

			// Assume all dates are the same, so take the first from the leader ticks
			List<Tick> firstLeaderTickList = leaderTickDictionary.Values.First();
			for (int i = 0; i < (firstLeaderTickList.Count - futureDays); i++) {				

				// Find change per ticker					
				Prediction p = new Prediction();
				p.ChangePerTicker = new Dictionary<string, float>();
				foreach (KeyValuePair<string, List<Tick>> keyValue in leaderTickDictionary) {					
					// Go ahead how many days we need to
					float cumulativeChange = 0;
					for (int j = 0; j < leaderWindowDays; j++) 
						cumulativeChange += keyValue.Value[j].GetChangePercent(true);
					
					p.ChangePerTicker.Add(keyValue.Key, cumulativeChange);
				}				

				// Add this prediction		
				DateTime predictionDate = firstLeaderTickList[i + futureDays].Date;
				p.ActualTick = indexTicks[i + futureDays];
				predictionDictionary.Add(predictionDate, p);
			}

			return predictionDictionary;
		}		
    }
}
