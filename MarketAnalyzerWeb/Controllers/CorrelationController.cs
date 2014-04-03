using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using GoogleFinanceLibrary;

namespace MarketAnalyzerWeb.Controllers
{
    public class CorrelationController : ApiController
    {
		IEnumerable<CorrelationResult> resultCache;

		[HttpGet]
		[Route("api/correlations")]
		public IEnumerable<CorrelationResult> GetAllCorrelations() {
			if (resultCache == null) {
				string dataFolder = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data");
				resultCache = CorrelationFinder.Find(dataFolder);
			}
			return resultCache;
		}
		
		[HttpGet]
		[Route("api/correlation/{predictorSymbol}/{predicteeSymbol}/{futureDays}")]
		public CorrelationData GetRawData(string predictorSymbol, string predicteeSymbol, int futureDays) {			
			TickMatrix tm = CorrelationFinder.GetRawData();
			CorrelationData result = new CorrelationData();

			TickList predictorList = tm.GetMultiple(predictorSymbol.Replace('_', ':'));
			result.PredictorClosePrices = predictorList.GetDataExcludingEndDays(futureDays, t => t.ClosePrice);

			TickList predicteeList = tm.GetMultiple(predicteeSymbol.Replace('_', ':'));
			result.PredicteeClosePrices = predicteeList.GetDataExcludingStartDays(futureDays, t => t.ClosePrice);

			return result;
		}		
    }
}
