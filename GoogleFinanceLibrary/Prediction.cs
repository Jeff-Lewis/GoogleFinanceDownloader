using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {
	public class Prediction {
		public Dictionary<string, double> ChangePerPredictorTicker { get; set; }
		public double PredictorAverageChange {
			get {
				double sum = 0;
				foreach (double change in ChangePerPredictorTicker.Values)
					sum += change;

				return Math.Round(sum / ChangePerPredictorTicker.Count, 2);
			}
		}
		public double ActualChangeDividedByPredictorAverage {
			get {
				return Math.Round(ActualTick.GetChangePercent(true) / PredictorAverageChange, 2);
			}
		}
		public Tick ActualTick { get; set; }
		public bool IsDirectionAccurate {
			get {
				return (Math.Sign(ActualTick.GetChangePercent(true)) == Math.Sign(PredictorAverageChange));
			}
		}

	}
}
