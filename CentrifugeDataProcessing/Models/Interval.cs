using System;
using System.Collections.Generic;
using System.Linq;

namespace CentrifugeDataProcessing.Models
{
	public class Interval
	{
		public DateTime Begin;
		public DateTime End;
		public List<DataPacket> Data = new List<DataPacket>();
		public string Name { get; set; }
		public string ModeName { get; set; }

		public string TypeMode;
		public int MedianCss { get; set; } // ЧСС
		public int MedianCd { get; set; } // Частота дыхания
		public int AvgCss { get; set; } // ЧСС
		public int AvgCd { get; set; } // Частота дыхания
		public float AvgTenz { get; set; }
		public float AvgTenzL { get; set; }
		public float AvgTenzR { get; set; }
		public float Add { get; set; }
		public float Ads { get; set; }

		public string Result="";

		public void Calc()
		{
			if (!Data.Any())
				return;

			int aCss = 0;
			double g = 0;
			int aCd = 0;
			float aTenz = 0;
			float aTenzL = 0;
			float aTenzR = 0;
			int count = Data.Count;
			List<int> mCss = new List<int>();
			List<int> mCd = new List<int>();


			foreach (var item in Data)
			{
				g += item.G;
				aCss += item.CSS;
				aCd += item.CD;
				aTenzL += item.TENZ1;
				aTenzR += item.TENZ2;
				aTenz += (item.TENZ1 + item.TENZ2) / 2;
				mCss.Add(item.CSS);
				mCd.Add(item.CD);
			}

			MedianCss = Median(mCss, count);
			MedianCd = Median(mCd, count);

			AvgCss = aCss / count;
			AvgCd = aCd / count;
			AvgTenz = aTenz / count;
			AvgTenzL = aTenzL / count;
			AvgTenzR = aTenzR / count;

			g = g / count;
			if (ModeName == "Площадка")
			{
				TypeMode = "Platform" + Math.Round(g);
			
			}
			
		}

		public int Median(List<int> source, int count)
		{
			source.Sort();
			int mid = count / 2;
			return (count % 2 != 0) ? source[mid] : (source[mid] + source[mid - 1]) / 2;
		}

	}
}