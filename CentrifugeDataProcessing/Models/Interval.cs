using System;
using System.Collections.Generic;

namespace CentrifugeDataProcessing.Models
{
    class Interval
    {
        public DateTime Begin;
        public DateTime End;
        public int AvgCSS; // ЧСС
        public int AvgCD; // Частота дыхания
        public float AvgADS; // ушное давление но это не точно
        public float AvgADD; //
        public float AvgADU;
        public List<DataPacket> Data = new List<DataPacket>();

    }
}
