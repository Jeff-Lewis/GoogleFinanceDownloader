using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoogleFinanceLibrary {
	public class CorrelationData {
		//public int[] Time { get; set; }
		public double[] PredictorClosePrices { get; set; }
		public double[] PredicteeClosePrices { get; set; }
	}
}