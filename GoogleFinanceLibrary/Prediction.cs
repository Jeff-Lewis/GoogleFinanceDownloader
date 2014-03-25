using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {
	public class Prediction {
		// Properties
		public Tick ActualTick { get; set; }
		public Dictionary<string, double> ChangePerPredictorSymbol { get; set; }
		public double PredictorAverageChange {
			get {
				double sum = 0;
				foreach (double change in ChangePerPredictorSymbol.Values)
					sum += change;

				return Math.Round(sum / ChangePerPredictorSymbol.Count, 2);
			}
		}
		public DateTime PredictorStartDate { get; set; }		
		public double ActualChangeDividedByPredictorAverage {
			get {
				return Math.Round(ActualTick.GetChangePercent(true) / PredictorAverageChange, 2);
			}
		}		
		public bool IsDirectionAccurate {
			get {
				return (Math.Sign(ActualTick.GetChangePercent(true)) == Math.Sign(PredictorAverageChange));
			}
		}

		// Constructor
		public Prediction() {
			ChangePerPredictorSymbol = new Dictionary<string, double>();
		}
	}
}
