using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleFinanceLibrary {
	public static class FinanceMath {
		public enum SignAgreement { Positive, Negative, Uninteresting }

		public static SignAgreement GetSignAgreement(double a, double b, double thresholdPercent) {
			if ((Math.Abs(a) >= thresholdPercent) &&
				(Math.Abs(b) >= thresholdPercent)) {
					if (Math.Sign(a) == Math.Sign(b))
						return SignAgreement.Positive;
					else if (Math.Sign(a) == (Math.Sign(b) * -1))
						return SignAgreement.Negative;
					else
						throw new Exception("This should never happen");
			} else
				return SignAgreement.Uninteresting;
		}
		public static double GetSignAgreementPercent(double[] arrayA, double[] arrayB) {
			if (arrayA.Length != arrayB.Length)
				throw new Exception("Arrays are not the same length!");

			int signAgreementCount = 0;
			for (int i = 0; i < arrayA.Length; i++)
				if (Math.Sign(arrayA[i]) == Math.Sign(arrayB[i]))
					signAgreementCount++;

			return ((double)signAgreementCount)/((double)arrayA.Length) * 100;
		}

		public static void GetInterestingSignAgreementPercent(double[] arrayA, double[] arrayB, double thresholdPercent, out double positiveAgreementPercent, out double negativeAgreementPercent) {
			if (arrayA.Length != arrayB.Length)
				throw new Exception("Arrays are not the same length!");

			int count = arrayA.Length;
			int positiveAgreementCount = 0;
			int negativeAgreementCount = 0;

			for (int i = 0; i < count; i++) {
				// Check if there are meaningful changes
				SignAgreement doesAgree = GetSignAgreement(arrayA[i], arrayB[i], thresholdPercent);
				if (doesAgree == SignAgreement.Positive)
					positiveAgreementCount++;
				else if (doesAgree == SignAgreement.Negative)
					negativeAgreementCount++;

				/*if ((Math.Abs(arrayA[i]) >= thresholdPercent) &&
					(Math.Abs(arrayB[i]) >= thresholdPercent)) {
					if (Math.Sign(arrayA[i]) == Math.Sign(arrayB[i])) {
						positiveAgreementCount++;
					}
					else if (Math.Sign(arrayA[i]) != Math.Sign(arrayB[i])) {
						negativeAgreementCount++;
					}					
				}*/
			}

			positiveAgreementPercent = ((double)positiveAgreementCount) / ((double)count) * 100;
			negativeAgreementPercent = ((double)negativeAgreementCount) / ((double)count) * 100;			
		}
	}
}
