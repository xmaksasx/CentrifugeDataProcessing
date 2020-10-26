using System;
using System.Collections.Generic;

namespace CentrifugeDataProcessing.Models
{
    class Interval
    {
        public DateTime Begin;
        public DateTime End;
        public List<DataPacket> Data = new List<DataPacket>();

        public string ModeName { get; set; }
        public int MedianCss { get; set; } // ЧСС
        public int MedianCd { get; set; } // Частота дыхания
        public float MedianAds { get; set; } // ушное давление но это не точно
        public float MedianAdd { get; set; } // Давление верхнее
        public float MedianAdu { get; set; } // давление нижнее
        public int AvgCss { get; set; } // ЧСС
        public int AvgCd { get; set; } // Частота дыхания
        public float AvgAds { get; set; } // ушное давление но это не точно
        public float AvgAdd { get; set; } // Давление верхнее
        public float AvgAdu { get; set; } // давление нижнее
    }
}
