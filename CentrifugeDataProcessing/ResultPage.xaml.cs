using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using CentrifugeDataProcessing.Annotations;
using CentrifugeDataProcessing.Models;


namespace CentrifugeDataProcessing
{
    /// <summary>
    /// Interaction logic for ResultPage.xaml
    /// </summary>
    public partial class ResultPage : Window
    {
        private readonly List<FileCentrifugeInfo> _ilst;
        private List<Nigger> _niggers;
        List<string> result = new List<string>();
        List<foo> fooresult = new List<foo>();


        public ResultPage(IList ilst)
        {
            _ilst = ilst.Cast<FileCentrifugeInfo>().ToList();
            _niggers = new List<Nigger>();
            InitializeComponent();
            ProgressLoad.Maximum = ilst.Count-1;
            ProgressDEvent += ProgressDE;
            Task.Run(() => Parallel.ForEach(_ilst, new ParallelOptions() { MaxDegreeOfParallelism = 20 }, d => { Simulation(d); }));
         

        }

        private void ProgressDE(int idx)
        {

            double max = 0;
            double val = 0;

            Dispatcher.Invoke(() => val = ProgressLoad.Value++);
            Dispatcher.Invoke(() => max = ProgressLoad.Maximum);
            Dispatcher.Invoke(() => MaximumFile.Text = ProgressLoad.Maximum.ToString());
            Dispatcher.Invoke(() => CurrentFile.Text = ProgressLoad.Value.ToString());

            if (val == max)
            {
                Dispatcher.Invoke(() => ProgressGrid.Visibility = Visibility.Hidden);
                Dispatcher.Invoke(() => GridProducts.ItemsSource = fooresult);
            }
        }


        class foo 
        {
            public string NamePilot { get; set; }
            public string PathFile { get; set; }
            public List<Interval> G3 { get; set; } = new List<Interval>();
            public List<Interval> G5 { get; set; } = new List<Interval>();
            public List<Interval> G6 { get; set; } = new List<Interval>();
        }

        private void Simulation(FileCentrifugeInfo file)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            if (File.Exists(file.Path))
            {
                var nigger = new Nigger() {Name = file.Name};
                _niggers.Add(nigger);

                DateTime dateTime = DateTime.Now;

                var bytes = File.ReadAllBytes(file.Path);
                var len = bytes.Length;
                var pos = GetPos(bytes);
                for (long ix = pos; ix < len; ix += 1440)
                {
                    byte[] data = new byte[144];
                    DataPacket packet = new DataPacket();
                    Array.Copy(bytes, ix, data, 0, 144);

                    ByteToObject(data, packet);
                    dateTime = DateTime.FromOADate(packet.Time);
                    if (packet.G > 2.8 && packet.G < 3.1)
                        FindIntervals3(nigger.Period3, packet);
                    if (packet.G > 4.8 && packet.G < 5.1)
                        FindIntervals5(nigger.Period5, nigger.Period3, packet);
                    if (packet.G > 5.8 && packet.G < 6.1)
                        FindIntervals6(nigger.Period6, nigger.Period5, packet);
                }


                for (long ix = pos; ix < len; ix += 288)
                {
                    byte[] data = new byte[144];
                    DataPacket packet = new DataPacket();
                    Array.Copy(bytes, ix, data, 0, 144);
                    ByteToObject(data, packet);
                    dateTime = DateTime.FromOADate(packet.Time);
                    AddPacketToInterval(nigger.Period3, packet, dateTime);
                    AddPacketToInterval(nigger.Period5, packet, dateTime);
                    AddPacketToInterval(nigger.Period6, packet, dateTime);
                }

                bytes = null;

                nigger.Period3.CalcAvg();
                nigger.Period5.CalcAvg();
                nigger.Period6.CalcAvg();

                List<Interval> result3 = new List<Interval>();
                List<Interval> result5 = new List<Interval>();
                List<Interval> result6 = new List<Interval>();

                FillCollection(result3, nigger.Period3);
                FillCollection(result5, nigger.Period5);
                FillCollection(result6, nigger.Period6);

                fooresult.Add(new foo()
                {
                    NamePilot = file.Family + " " + file.Name + " " + file.Lastname,
                    PathFile = file.Path,
                    G3 = result3,
                    G5 = result5,
                    G6 = result6
                });

                OnProgressDEvent(0);
            }
        }

        private void FillCollection(List<Interval> result, Periods period)
        {
            FillItemOfCollection(result, period.Rise,"Набор");
            FillItemOfCollection(result, period.Platform, "Площадка");
            FillItemOfCollection(result, period.Descent, "Спуск");
            FillItemOfCollection(result, period.FirstMinute, "Первая минута");
            FillItemOfCollection(result, period.LastMinute, "Последняя минута");
        }

        private void FillItemOfCollection(List<Interval> result, Interval interval, string modeName)
        {

            result.Add(new Interval()
            {
                ModeName = modeName,
                AvgCss = interval.AvgCss,
                AvgCd = interval.AvgCd,
                AvgAds = interval.AvgAds,
                AvgAdd = interval.AvgAdd,
                AvgAdu = interval.AvgAdu,
                MedianCss = interval.MedianCss,
                MedianCd = interval.MedianCd,
                MedianAds = interval.MedianAds,
                MedianAdd = interval.MedianAdd,
                MedianAdu = interval.MedianAdu
            });
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

        private void FindIntervals3(Periods period, DataPacket packet)
        {
            var dateTime = DateTime.FromOADate(packet.Time);

            if (period.endread)
            {
                period.Rise.Begin = dateTime.AddSeconds(-7);
                period.Rise.End = dateTime;

                period.Platform.Begin = dateTime;
                period.Platform.End = dateTime.AddSeconds(30);

                period.Descent.Begin = dateTime.AddSeconds(30);
                period.Descent.End = dateTime.AddSeconds(37);

                period.FirstMinute.Begin = dateTime.AddSeconds(37);
                period.FirstMinute.End = dateTime.AddSeconds(97);

                period.endread = false;
            }
        }

        private void FindIntervals5(Periods currReriod, Periods prevReriod, DataPacket packet)
        {
            var dateTime = DateTime.FromOADate(packet.Time);

            if (currReriod.endread)
            {
                currReriod.Rise.Begin = dateTime.AddSeconds(-7);
                currReriod.Rise.End = dateTime;

                currReriod.Platform.Begin = dateTime;
                currReriod.Platform.End = dateTime.AddSeconds(30);

                currReriod.Descent.Begin = dateTime.AddSeconds(30);
                currReriod.Descent.End = dateTime.AddSeconds(37);

                currReriod.FirstMinute.Begin = dateTime.AddSeconds(37);
                currReriod.FirstMinute.End = dateTime.AddSeconds(97);

                prevReriod.LastMinute.Begin = dateTime.AddSeconds(-97);
                prevReriod.LastMinute.End = dateTime.AddSeconds(-37);

                currReriod.endread = false;
            }
        }

        private void FindIntervals6(Periods currReriod, Periods prevReriod, DataPacket packet)
        {
            var dateTime = DateTime.FromOADate(packet.Time);

            if (currReriod.endread)
            {
                currReriod.Rise.Begin = dateTime.AddSeconds(-7);
                currReriod.Rise.End = dateTime;

                currReriod.Platform.Begin = dateTime;
                currReriod.Platform.End = dateTime.AddSeconds(30);

                currReriod.Descent.Begin = dateTime.AddSeconds(30);
                currReriod.Descent.End = dateTime.AddSeconds(37);

                currReriod.FirstMinute.Begin = dateTime.AddSeconds(37);
                currReriod.FirstMinute.End = dateTime.AddSeconds(97);

                currReriod.LastMinute.Begin = dateTime.AddSeconds(127);
                currReriod.LastMinute.End = dateTime.AddSeconds(187);

                prevReriod.LastMinute.Begin = dateTime.AddSeconds(-97);
                prevReriod.LastMinute.End = dateTime.AddSeconds(-37);

                currReriod.endread = false;
            }
        }

        private void AddPacketToInterval(Periods period, DataPacket packet, DateTime dateTime)
        {
            if ((dateTime >= period.Rise.Begin) && (dateTime <= period.Rise.End))
            {
                period.Rise.Data.Add(packet);
            }

            if ((dateTime >= period.Platform.Begin) && (dateTime <= period.Platform.End))
            {
                period.Platform.Data.Add(packet);
            }

            if ((dateTime >= period.Descent.Begin) && (dateTime <= period.Descent.End))
            {
                period.Descent.Data.Add(packet);
            }

            if ((dateTime >= period.FirstMinute.Begin) && (dateTime <= period.FirstMinute.End))
            {
                period.FirstMinute.Data.Add(packet);
            }

            if ((dateTime >= period.LastMinute.Begin) && (dateTime <= period.LastMinute.End))
            {
                period.LastMinute.Data.Add(packet);
            }
        }

        public void ByteToObject<T>(byte[] receiveBytes, T obj)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            Marshal.Copy(receiveBytes, 0, i, len);
            Marshal.PtrToStructure(i, obj);
            Marshal.FreeHGlobal(i);
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnScrollUp()
        {
            var scrollViewer = GetScrollViewer(GridProducts) as ScrollViewer;
            ScrollViewer1?.ScrollToVerticalOffset(ScrollViewer1.VerticalOffset - 30);
        }

        private void OnScrollDown()
        {
            var scrollViewer = GetScrollViewer(GridProducts) as ScrollViewer;
            ScrollViewer1?.ScrollToVerticalOffset(ScrollViewer1.VerticalOffset + 30);
        }

        public static DependencyObject GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            {
                return o;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                    continue;
                return result;
            }

            return null;
        }

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                OnScrollDown();
            }
            else
            {
                OnScrollUp();
            }
        }

        public delegate void ProgressD(int idx);
        public static event ProgressD ProgressDEvent;

        public static void OnProgressDEvent(int idx)
        {
            ProgressDEvent?.Invoke(idx);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ViewNigger_OnClick(object sender, RoutedEventArgs e)
        {
            var path = ((Button) sender).Tag;
            

        }

        private void SaveNigger_OnClick(object sender, RoutedEventArgs e)
        {
            var path = ((Button)sender).Tag;
            var name = ((Button)sender).Uid;

            foreach (var foor in fooresult)
            {
                 
                if (name== foor.NamePilot)
                {
                    var tfoor = 0;
                }
            }

            
        }
    }
}
