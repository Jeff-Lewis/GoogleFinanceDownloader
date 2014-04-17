using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {
	public class CorrelationConfig {
		// Singleton instance
		private static CorrelationConfig instance;

		// Accessor property
		public static CorrelationConfig Instance {
			get {
				if (instance == null)
					instance = new CorrelationConfig();

				return instance;
			}
		}

		// Properties
		public DateTime EndDate { get; private set; }
		public DateTime StartDate { get; private set; }
		public string[] Exchanges { get; private set; }
		public int[] FutureDays { get; private set; }
		public double PredictorChangePercentThreshold { get; private set; }
		public double PredicteeChangePercentThreshold { get; private set; }
		public int TopBottomCount { get; private set; }
		public int InterestingSignAgreementPercentCutoff { get; private set; }
		public int InterestingTickPercent { get; private set; }
		public int MinimumTicksComparedCount { get; private set; }

		// Constructor
		private CorrelationConfig() {
			EndDate = new DateTime(2014, 3, 25);
			StartDate = EndDate.AddMonths(-3);
			Exchanges = new string[] { /*"NYSE", "NASDAQ", "CURRENCY",*/ "" };
			FutureDays = Enumerable.Range(1, 3).ToArray();
			PredictorChangePercentThreshold = 0.2;
			PredicteeChangePercentThreshold = 0.8;
			//PredictorChangePercentThreshold = 0.3;
			//PredicteeChangePercentThreshold = 0.3;
			TopBottomCount = 50;
			InterestingSignAgreementPercentCutoff = 60;
			InterestingTickPercent = 50;
			MinimumTicksComparedCount = 50;		// 3 months data is about 60 ticks
		}
	}
}
