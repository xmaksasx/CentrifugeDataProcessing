using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        List<MyTable> result = new List<MyTable>();
        List<foo> fooresult = new List<foo>();

        public ResultPage(IList ilst)
        {
            _ilst = ilst.Cast<FileCentrifugeInfo>().ToList();
            _niggers = new List<Nigger>();
            InitializeComponent();
            ProgressLoad.Maximum = ilst.Count;
            //  for (int i = 0; i < _ilst.Count; i++)
             //     Simulation(_ilst[i]);
            //  Thread simThread = new Thread(new ThreadStart(Simulation));
            // simThread.IsBackground = true;
            // simThread.Start();




                Parallel.ForEach(_ilst, new ParallelOptions() { MaxDegreeOfParallelism = 2 }, d => { Simulation(d); });



            //var myEntities = _ilst;
            //var maxThreads = 300;


            //var semaphoreSlim = new SemaphoreSlim(maxThreads);
            //var tasks = new List<Task>(myEntities.Count);
            //foreach (var entity in myEntities)
            //{
            //    tasks.Add(Task.Run(() =>
            //    {
            //        semaphoreSlim.Wait();
            //        try
            //        {
            //            Simulation(entity);
            //        }
            //        finally
            //        {
            //            semaphoreSlim.Release();
            //        }
            //    }));
            //}

            //Task.WaitAll(tasks.ToArray());

        }

        class MyTable
        {
            public MyTable(string ModeName, string Css, string Cd)
            {
                this.ModeName = ModeName;
                this.Css = Css;
                this.Cd = Cd;

            }

            public string ModeName { get; set; }
            public string Css { get; set; }
            public string Cd { get; set; }
        }

        class foo
        {
            public string NamePilot { get; set; }
            public List<MyTable> G3 { get; set; } = new List<MyTable>();
            public List<MyTable> G5 { get; set; } = new List<MyTable>();
            public List<MyTable> G6 { get; set; } = new List<MyTable>();
        }



        private async void Simulation(FileCentrifugeInfo file)
        {
            //async
                 await Task.Run(() =>
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
                    double max = 0;
                    double val = 0;
                    Dispatcher.Invoke(() => val = ProgressLoad.Value++);
                    Dispatcher.Invoke(() => max = ProgressLoad.Maximum);
                    nigger.Period3.CalcAvg();
                    nigger.Period5.CalcAvg();
                    nigger.Period6.CalcAvg();

                    List<MyTable> result3 = new List<MyTable>();
                    List<MyTable> result5 = new List<MyTable>();
                    List<MyTable> result6 = new List<MyTable>();



                    // result6.Add(new MyTable(file.Name, "ЧСС", "Дыхание"));
                    result3.Add(new MyTable("Набор", nigger.Period3.Rise.AvgCSS.ToString(),
                        nigger.Period3.Rise.AvgCD.ToString()));
                    result3.Add(new MyTable("Площадка", nigger.Period3.Platform.AvgCSS.ToString(),
                        nigger.Period3.Platform.AvgCD.ToString()));
                    result3.Add(new MyTable("Спуск", nigger.Period3.Descent.AvgCSS.ToString(),
                        nigger.Period3.Descent.AvgCD.ToString()));
                    result3.Add(new MyTable("Первая минута", nigger.Period3.FirstMinute.AvgCSS.ToString(),
                        nigger.Period3.FirstMinute.AvgCD.ToString()));
                    result3.Add(new MyTable("Последняя минута", nigger.Period3.LastMinute.AvgCSS.ToString(),
                        nigger.Period3.LastMinute.AvgCD.ToString()));
                    result3.Add(new MyTable(" ", " ", " "));

                    result5.Add(new MyTable("Набор", nigger.Period5.Rise.AvgCSS.ToString(),
                        nigger.Period5.Rise.AvgCD.ToString()));
                    result5.Add(new MyTable("Площадка", nigger.Period5.Platform.AvgCSS.ToString(),
                        nigger.Period5.Platform.AvgCD.ToString()));
                    result5.Add(new MyTable("Спуск", nigger.Period5.Descent.AvgCSS.ToString(),
                        nigger.Period5.Descent.AvgCD.ToString()));
                    result5.Add(new MyTable("Первая минута", nigger.Period5.FirstMinute.AvgCSS.ToString(),
                        nigger.Period5.FirstMinute.AvgCD.ToString()));
                    result5.Add(new MyTable("Последняя минута", nigger.Period5.LastMinute.AvgCSS.ToString(),
                        nigger.Period5.LastMinute.AvgCD.ToString()));
                    result5.Add(new MyTable(" ", " ", " "));

                    result6.Add(new MyTable("Набор", nigger.Period6.Rise.AvgCSS.ToString(),
                        nigger.Period6.Rise.AvgCD.ToString()));
                    result6.Add(new MyTable("Площадка", nigger.Period6.Platform.AvgCSS.ToString(),
                        nigger.Period6.Platform.AvgCD.ToString()));
                    result6.Add(new MyTable("Спуск", nigger.Period6.Descent.AvgCSS.ToString(),
                        nigger.Period6.Descent.AvgCD.ToString()));
                    result6.Add(new MyTable("Первая минута", nigger.Period6.FirstMinute.AvgCSS.ToString(),
                        nigger.Period6.FirstMinute.AvgCD.ToString()));
                    result6.Add(new MyTable("Последняя минута", nigger.Period6.LastMinute.AvgCSS.ToString(),
                        nigger.Period6.LastMinute.AvgCD.ToString()));
                    result6.Add(new MyTable(" ", " ", " "));

                    fooresult.Add(new foo() {NamePilot = file.Family, G3 = result3, G5 = result5, G6 = result6});


                    if (val == max)
                    {
                        Dispatcher.Invoke(() => ProgressGrid.Visibility = Visibility.Hidden);
                        Dispatcher.Invoke(() => GridProducts.ItemsSource = fooresult);
                        Dispatcher.Invoke(() => GridProducts.Items.Refresh());
                    }


                }



            });
            GC.Collect();
        }

        private static int GetPos(byte[] bytes)
        {
            var pos = 1422;
            var offset = BitConverter.ToInt32(bytes, 1422);
            if (offset > 0)
                pos = pos + 4 + offset;
            else pos = pos + 4;
            return pos;
        }

        private static void FindIntervals3(Periods period, DataPacket packet)
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

        private static void FindIntervals5(Periods currReriod, Periods prevReriod, DataPacket packet)
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

        private static void FindIntervals6(Periods currReriod, Periods prevReriod, DataPacket packet)
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

        private static void AddPacketToInterval(Periods period, DataPacket packet, DateTime dateTime)
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

        public static void ByteToObject<T>(byte[] receiveBytes, T obj)
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
    }
}
