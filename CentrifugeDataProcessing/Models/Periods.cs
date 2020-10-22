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
            Rise.AvgCSS = (int)GetAvgCss(Rise.Data);
            Rise.AvgCD = (int)GetAvgCD(Rise.Data);

            Platform.AvgCSS = (int)GetAvgCss(Platform.Data);
            Platform.AvgCD = (int)GetAvgCD(Platform.Data);

            Descent.AvgCSS = (int)GetAvgCss(Descent.Data);
            Descent.AvgCD = (int)GetAvgCD(Descent.Data);

            FirstMinute.AvgCSS = (int)GetAvgCss(FirstMinute.Data);
            FirstMinute.AvgCD = (int)GetAvgCD(FirstMinute.Data);

            LastMinute.AvgCSS = (int)GetAvgCss(LastMinute.Data);
            LastMinute.AvgCD = (int)GetAvgCD(LastMinute.Data);

            Rise.Data = null;
            Platform.Data = null;
            Descent.Data = null;
            FirstMinute.Data = null;
            LastMinute.Data = null;
        }                                                          

        public double GetAvgCss(List<DataPacket> Data)
        {
            List<double> lst = new List<double>();
            foreach (var item in Data)
                lst.Add(item.CSS);
            return lst.Median();
        }

        public double GetAvgCD(List<DataPacket> Data)
        {
            List<double> lst = new List<double>();
            foreach (var item in Data)
                lst.Add(item.CD);
            return lst.Median();
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

    }
}
