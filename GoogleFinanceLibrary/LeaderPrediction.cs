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
		// Public methods
		public static PredictionDictionary Predict(string[] leaderTickerArray, string indexTicker, string exchange, DateTime startDate, DateTime endDate, int leaderWindowDays, int futureDays) {
			// Get leader ticker data
			TickMatrix leaderTicks = TickRetriever.GetData(leaderTickerArray, exchange, startDate, endDate);

			// Get index data
			TickList indexTicks = TickRetriever.GetData(indexTicker, exchange, startDate, endDate);

			PredictionDictionary result = GetPredictions(leaderTicks, indexTicks, leaderWindowDays, futureDays);
			
			return result;
		}

		// Private methods
		
		private static PredictionDictionary GetPredictions(TickMatrix leaderTicks, TickList indexTicks, int leaderWindowDays, int futureDays) {
			// Check slope of lead data against future slope of index data			
			PredictionDictionary result = new PredictionDictionary();
			result.WindowsDays = leaderWindowDays;
			result.FutureDays = futureDays;

			// Create a prediction for each day
			foreach (DateTime day in leaderTicks.GetAllDates()) {
				Prediction p = new Prediction() {
					ActualTick = indexTicks.GetFutureTick(day, futureDays)
				};

				// Find the change per ticker over the window days
				foreach (string symbol in leaderTicks.GetAllSymbols()) {
					double change = leaderTicks.GetAverageChangeOverDates(symbol, day, leaderWindowDays);
					p.ChangePerPredictorSymbol.Add(symbol, change);
				}

				// Add this prediction
				result.Add(p.ActualTick.Date, p);
			}

			/*
			// Assume all dates are the same, so take the first from the leader ticks
			List<Tick> firstLeaderTickList = leaderTickDictionary.Values.First();
			for (int i = 0; i < (firstLeaderTickList.Count - Math.Max(futureDays, leaderWindowDays)); i++) {	
				// Find change per ticker					
				Prediction p = new Prediction();			
				p.ChangePerPredictorTicker = new Dictionary<string, double>();
				foreach (KeyValuePair<string, TickList> keyValue in leaderTickDictionary) {					
					// Go ahead how many days we need to
					double cumulativeChange = 0;
					for (int j = 0; j < leaderWindowDays; j++) 
						cumulativeChange += keyValue.Value[i+j].GetChangePercent(true);

					p.ChangePerPredictorTicker.Add(keyValue.Key, cumulativeChange);
				}				

				// Add this prediction		
				DateTime predictionDate = firstLeaderTickList[i + futureDays].Date;
				p.ActualTick = indexTicks.First(t => t.Date == predictionDate);
				result.Add(predictionDate, p);
			}
			*/

			return result;
		}		
    }
}
