using System;
using System.Collections.Generic;
using System.Linq;


namespace CentrifugeDataProcessing.Models
{ class Periods
    {

        public Interval Rise = new Interval();
        public Interval Platform = new Interval();
        public Interval Descent = new Interval();
        public Interval FirstMinute = new Interval();
        public Interval LastMinute = new Interval();
        public bool endread = true;

        public void CalcAvg()
        {
            GetAverage(Rise);
            GetAverage(Platform);
            GetAverage(Descent);
            GetAverage(FirstMinute);
            GetAverage(LastMinute);

            GetMedian(Rise);
            GetMedian(Platform);
            GetMedian(Descent);
            GetMedian(FirstMinute);
            GetMedian(LastMinute);

            Rise.Data = null;
            Platform.Data = null;
            Descent.Data = null;
            FirstMinute.Data = null;
            LastMinute.Data = null;
        }                                                          

        public void GetMedian(Interval interval)
        {
            List<double> lstCss = new List<double>();
            List<double> lstCd = new List<double>();
            List<double> lstAds = new List<double>();
            List<double> lstAdu = new List<double>();
            List<double> lstAdd = new List<double>();

            foreach (var item in interval.Data)
            {
                lstCss.Add(item.CSS);
                lstCd.Add(item.CD);
                if (item.ADS > 0)
                    lstAds.Add(item.ADS);
                if (item.ADU > 0)
                    lstAdu.Add(item.ADU);
                if (item.ADD > 0)
                    lstAdd.Add(item.ADD);
            }
            interval.MedianCss = (int)lstCss.Median();
            interval.MedianCd = (int)lstCd.Median();
            interval.MedianAds = (int)lstAds.Median();
            interval.MedianAdu = (int)lstAdu.Median();
            interval.MedianAdd = (int)lstAdd.Median();

            lstCss = null;
            lstCd = null;
            lstAds = null;
            lstAdu = null;
            lstAdd = null;
        }


        public void GetAverage(Interval interval)
        {
            List<double> lstCss = new List<double>();
            List<double> lstCd = new List<double>();
            List<double> lstAds = new List<double>();
            List<double> lstAdu = new List<double>();
            List<double> lstAdd = new List<double>();

            foreach (var item in interval.Data)
            {
                lstCss.Add(item.CSS);
                lstCd.Add(item.CD);
                if (item.ADS > 0)
                    lstAds.Add(item.ADS);
                if (item.ADU > 0)
                    lstAdu.Add(item.ADU);
                if (item.ADD > 0)
                    lstAdd.Add(item.ADD);
            }

            interval.AvgCss = (int)lstCss.Average();
            interval.AvgCd = (int)lstCd.Average();
            interval.AvgAds = (int)lstAds.Average();
            interval.AvgAdu = (int)lstAdu.Average();
            interval.AvgAdd = (int)lstAdd.Average();

            lstCss = null;
            lstCd = null;
            lstAds = null;
            lstAdu = null;
            lstAdd = null;
        }

     



    }

    public static class EnumerableExtensions
    {
        public static double Median(this IEnumerable<double> sourceNumbers)
        {
        
            if (sourceNumbers == null || sourceNumbers.Count() == 0)
                return 0;


            double[] sortedPNumbers = sourceNumbers.ToArray();
            Array.Sort(sortedPNumbers);

            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = (size % 2 != 0) ? (double)sortedPNumbers[mid] : ((double)sortedPNumbers[mid] + (double)sortedPNumbers[mid - 1]) / 2;
            return median;
        }

        public static double Average(this IEnumerable<double> sourceNumbers)
        {
            var enumerable = sourceNumbers as double[] ?? sourceNumbers.ToArray();
            if (!enumerable.Any())
                return 0;
            int count = enumerable.Count();
            double sum = enumerable.Sum();
            
            return sum / count;
        }
    }
}
