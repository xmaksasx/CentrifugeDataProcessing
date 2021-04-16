using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CentrifugeDataProcessing.Models;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace CentrifugeDataProcessing
{

    public partial class ViewerData : Window
    {

        public ViewerData(string path)
        {
            InitializeComponent();
            Simulation(path);

            Plotter.Children.Remove(Plotter.MainHorizontalAxis);

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private HorizontalTimeSpanAxis timeSpanAxis = new HorizontalTimeSpanAxis();
        private ObservableDataSource<TimeParam> InitSource()
        {
            var source = new ObservableDataSource<TimeParam>();
            source.SetXMapping(x => timeSpanAxis.ConvertToDouble(x.Time));
            source.SetYMapping(y => y.y);
            return source;
        }

        public static void ByteToObject<T>(byte[] receiveBytes, T obj)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            Marshal.Copy(receiveBytes, 0, i, len);
            Marshal.PtrToStructure(i, obj);
            Marshal.FreeHGlobal(i);
        }

        class TimeParam
        {
            public TimeParam(double y1, TimeSpan Time1)
            {
                y = y1;
                Time = Time1;

            }

            public double y;
            public TimeSpan Time;
        }

        List<TimeParam> GSource = new List<TimeParam>();
        List<TimeParam> CssSource = new List<TimeParam>();
        List<TimeParam> CdSource = new List<TimeParam>();
        List<TimeParam> AddSource = new List<TimeParam>();
        List<TimeParam> AdsSource = new List<TimeParam>();
        List<TimeParam> source5 = new List<TimeParam>();
        List<TimeParam> source6 = new List<TimeParam>();

        private void Simulation(string path)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            if (File.Exists(path))
            {
                DateTime dateTime = DateTime.Now;
                var bytes = File.ReadAllBytes(path);
                var len = bytes.Length;
                var pos = GetPos(bytes);
                for (long ix = pos; ix < len; ix += 144)
                {
                    byte[] data = new byte[144];
                    DataPacket packet = new DataPacket();
                    Array.Copy(bytes, ix, data, 0, 144);

                    ByteToObject(data, packet);
                    dateTime = DateTime.FromOADate(packet.Time);
                    if (dateTime.Hour != 0)
                    {
                        var y = new TimeSpan(0, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
                        GSource.Add(new TimeParam(packet.G, y));
                        CssSource.Add(new TimeParam(packet.CSS, y));
                        CdSource.Add(new TimeParam(packet.CD, y));
                        AddSource.Add(new TimeParam(packet.ADD, y));
                        AdsSource.Add(new TimeParam(packet.ADS, y));
                    }
                }

                bytes = null;
            }

        }

        private int GetPos(byte[] bytes)
        {
            var pos = 1422;
            var offset = BitConverter.ToInt32(bytes, 1422);
            if (offset > 0)
                pos = pos + 4 + offset;
            else pos = pos + 4;
            return pos;
        }

        private void AddGraph_OnClick(object sender, RoutedEventArgs e)
        {
            var uid = ((Button) sender).Uid;
            var ods = InitSource();
            Plotter.AddLineGraph(ods, 1, uid);
            switch (uid)
            {
                case "GSource":
                    ods.AppendMany(GSource);
                    break;

                case "CssSource":
                    ods.AppendMany(CssSource);
                    break;
                case "CdSource":
                    ods.AppendMany(CdSource);
                    break;
                case "AddSource":
                    ods.AppendMany(AddSource);
                    break;
                case "AdsSource":
                    ods.AppendMany(AdsSource);
                    break;
            }
        }

        private void RemoveGraph_OnClick(object sender, RoutedEventArgs e)
        {
            var uid = ((Button)sender).Uid;
            LineGraph lineGraph = null;
            foreach (var VARIABLE in Plotter.Children)
            {
                var line = VARIABLE as LineGraph;
                if (line != null && line.Description.Full == uid)
                    lineGraph = line;
            }
            if (lineGraph != null)
                Plotter.Children.Remove(lineGraph);

        }


        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}