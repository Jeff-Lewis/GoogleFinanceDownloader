using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {
	public class CorrelationResult {
		public double PositiveSignAgreementPercent { get; set; }
		public double NegativeSignAgreementPercent { get; set; }
		public string PredictorSymbol { get; set; }
		public string PredicteeSymbol { get; set; }
		public int FutureDays { get; set; }
		public double[] PredictorTicks { get; set; }
		public double[] PredicteeTicks { get; set; }
		public override string ToString() {
			return string.Format("{0} -> {1}, {2} days: Positive%: {3}, Negative%: {4}", PredictorSymbol, PredicteeSymbol, FutureDays,
				Math.Round(PositiveSignAgreementPercent), Math.Round(NegativeSignAgreementPercent));
		}

		public static CorrelationResult GetNoCorrelationResult(string predictorSymbol, string predicteeSymbol, int futureDays) {
			return new CorrelationResult() {
				FutureDays = futureDays,
				NegativeSignAgreementPercent = 0,
				PositiveSignAgreementPercent = 0,
				PredictorSymbol = predictorSymbol,
				PredictorTicks = null,
				PredicteeSymbol = predicteeSymbol,
				PredicteeTicks = null
			};
		}
	}	
}
