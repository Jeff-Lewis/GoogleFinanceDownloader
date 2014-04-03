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
    {/*
		// Public methods
		public static PredictionDictionary Predict(string[] leaderTickerArray, string indexTicker, string exchange, DateTime startDate, DateTime endDate, int leaderWindowDays, int futureDays) {
			// Get leader ticker data
			TickMatrix leaderTicks = TickRetriever.GetData(leaderTickerArray, exchange, startDate, endDate);

			// Get index data
			TickList indexTicks = TickRetriever.GetData(indexTicker, exchange, startDate, endDate);

			PredictionDictionary result = GetPredictions(leaderTicks, indexTicks, leaderWindowDays, futureDays);
			
			return result;
		}
		*/
		// Private methods		
		/*
		private static PredictionDictionary GetPredictions(TickMatrix leaderTicks, TickList indexTicks, int leaderWindowDays, int futureDays) {
			// Check slope of lead data against future slope of index data			
			PredictionDictionary result = new PredictionDictionary();
			result.WindowsDays = leaderWindowDays;
			result.FutureDays = futureDays;

			// Create a prediction for each day
			foreach (DateTime day in leaderTicks.GetAllDates()) {
				Prediction p = new Prediction();
				try {
					p.ActualTick = indexTicks.GetFutureTick(day, leaderWindowDays - 1 + futureDays);				
				} catch {
					// If we dont have this tick, screw it and go to the next iteration
					continue;
				}

				// Find the change per ticker over the window days
				foreach (string symbol in leaderTicks.GetAllSymbols()) {
					double change = leaderTicks.GetAverageChangeOverDates(symbol, day, leaderWindowDays);
					p.ChangePerPredictorSymbol.Add(symbol, change);
					p.PredictorStartDate = day;
				}

				// Add this prediction
				result.Add(p.ActualTick.Date, p);
			}

			return result;
		}	
		*/
    }
}
