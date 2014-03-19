using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GoogleFinanceLibrary;

namespace MarketAnalyzer {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e) {
			/*string[] leaderTickers = {"TSLA", "NFLX", "FB"};
			string indexTicker = "SPY";
			string exchange = string.Empty;
			
			DateTime endDate = DateTime.Now;
			DateTime startDate = DateTime.Now.AddMonths(-1);
			int windowDays = 1;
			int futureDays = 1;

			PredictionDictionary result = LeaderPrediction.Predict(leaderTickers, indexTicker, exchange, startDate, endDate, windowDays, futureDays);
			richTextBox1.Text = result.ToString();
			*/

			string result = LeaderPredictionParameterTester.Test();
			richTextBox1.Text = result;
		}
	}
}
