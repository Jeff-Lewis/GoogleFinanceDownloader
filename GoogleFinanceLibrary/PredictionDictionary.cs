using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {
	public class PredictionDictionary : Dictionary<DateTime, Prediction >{
		// Properties
		public int WindowsDays { get; set; }
		public int FutureDays { get; set; }		

		public double GetDirectionAgreementPercent() {			
			double accurateCount = this.Values.Where(p => p.IsDirectionAccurate).Count();
			double totalCount = this.Values.Count;
			double ratio = accurateCount / totalCount;
				
			return Math.Round(ratio, 2);
		}
		public override string ToString() {			
			StringBuilder sb = new StringBuilder();

			// Header
			Prediction firstPrediction = this.Values.First();
			sb.AppendLine("Window Days: " + WindowsDays);
			sb.AppendLine("Future Days: " + FutureDays);
			sb.AppendFormat("{0,-20}{1,-20}{2,-20}{3,-20}{4,-20}", "Date", "Index", "Average", "Ratio", "SignAgree");
			foreach (string ticker in firstPrediction.ChangePerPredictorSymbol.Keys)
				sb.AppendFormat("{0,-20}", ticker);
			sb.AppendLine();

			// Body
			foreach (KeyValuePair<DateTime, Prediction> keyValue in this) {
				sb.AppendFormat("{0,-20}{1,-20}{2,-20}{3,-20}{4,-20}", 
					keyValue.Key.ToShortDateString(), 
					keyValue.Value.ActualTick.GetChangePercent(true), 					
					keyValue.Value.PredictorAverageChange, 
					keyValue.Value.ActualChangeDividedByPredictorAverage, 
					keyValue.Value.IsDirectionAccurate);
				foreach (KeyValuePair<string, double> changePerTickerKeyValue in keyValue.Value.ChangePerPredictorSymbol) {
					sb.AppendFormat("{0,-20}", changePerTickerKeyValue.Value);
				}
				sb.AppendFormat("({0})", keyValue.Value.PredictorStartDate.ToShortDateString());
				sb.AppendLine();
			}

			sb.AppendLine("DirectionAgreementPercent: " + GetDirectionAgreementPercent());

			return sb.ToString();		
		}
	}
}
