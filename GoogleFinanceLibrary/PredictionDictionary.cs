using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {
	public class PredictionDictionary : Dictionary<DateTime, Prediction>{
		public double GetDirectionAgreementPercent() {
			//double ratio = this.Values.Where(p => p.IsDirectionAccurate).Count() / this.Values.Count;
			double accurateCount = this.Values.Where(p => p.IsDirectionAccurate).Count();
			double totalCount = this.Values.Count;
			double ratio = accurateCount / totalCount;
				
			return Math.Round(ratio, 2);
		}
		public override string ToString() {			
			StringBuilder sb = new StringBuilder();

			// Header
			Prediction firstPrediction = this.Values.First();
			sb.AppendFormat("{0,-20}{1,-20}{2,-20}{3,-20}{4,-20}", "Date", "Index", "Average", "Ratio", "SignAgree");
			foreach (string ticker in firstPrediction.ChangePerPredictorTicker.Keys)
				sb.AppendFormat("{0,-20}", ticker);
			sb.AppendLine();

			// Body
			foreach (KeyValuePair<DateTime, Prediction> keyValue in this) {
				sb.AppendFormat("{0,-20}{1,-20}{2,-20}{3,-20}{4,-20}", keyValue.Key.ToShortDateString(), keyValue.Value.ActualTick.GetChangePercent(true), keyValue.Value.PredictorAverageChange, 
					keyValue.Value.ActualChangeDividedByPredictorAverage, keyValue.Value.IsDirectionAccurate);
				foreach (KeyValuePair<string, double> changePerTickerKeyValue in keyValue.Value.ChangePerPredictorTicker) {
					sb.AppendFormat("{0,-20}", changePerTickerKeyValue.Value);
				}
				sb.AppendLine();
			}

			sb.AppendLine("DirectionAgreementPercent: " + GetDirectionAgreementPercent());

			return sb.ToString();		
		}
	}
}
