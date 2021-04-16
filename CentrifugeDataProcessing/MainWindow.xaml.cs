using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CentrifugeDataProcessing
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            load();
        }

        async void load()
        {
            var b = DateTime.Now;
            await Task.Run(LoadUsers);
            var e = DateTime.Now;
            var hh = (e - b).TotalSeconds;
            LstTesters.ItemsSource = lst;
        }

        List<FileCentrifugeInfo> lst = new List<FileCentrifugeInfo>();

        private void LoadUsers()
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            var files = Directory.GetFiles("Users");
            foreach (var file in files)
                if (File.Exists(file))
                {
                    using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open)))
                    {

                        var family =
                            Encoding.Default.GetString(reader.ReadBytes(250), 0, 100)
                                .Replace("\0", ""); //  1.Фамилия(строка с завершающим нулем, общая длина 250 байт)
                        var name = Encoding.Default.GetString(reader.ReadBytes(250), 0, 100)
                            .Replace("\0", ""); //  2.Имя(строка с завершающим нулем, общая длина 250 байт)
                        var lastname =
                            Encoding.Default.GetString(reader.ReadBytes(250), 0, 100)
                                .Replace("\0", ""); //  3.Отчество(строка с завершающим нулем, общая длина 250 байт)
                        var category =
                            reader.ReadBytes(250); //  4.Категория(строка с завершающим нулем, общая длина 250 байт)
                        var birthday = reader.ReadDouble(); //  5.Дата рождения(double, 8 байт в формате TDateTime)
                        var number =
                            reader.ReadBytes(250); //  6.Номер(строка с завершающим нулем, общая длина 250 байт)
                        var type = reader
                            .ReadInt32(); //  7.Тип центрифуги(Цф 6 или 18, сейчас не помню) Длина 4 байта(integer)
                        var array = reader.ReadBytes(160); //  8.Далее идет блок результатов проверки остроты зрения:
                        var lengthStr = reader.ReadInt32();
                        var str = reader.ReadBytes(lengthStr);


                        var birthdate = DateTime.FromOADate(birthday);
                        var today = DateTime.Today;
                        var age = today.Year - birthdate.Year;
                        if (birthdate.Date > today.AddYears(-age)) age--;

                        if(age>20)
                            Console.WriteLine(age);
                        lst.Add( new FileCentrifugeInfo(){ Family=family , Name= name, Lastname= lastname, Path = file, Age = age });
                    }
                }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {

            if (LstTesters.SelectedItems.Count == 0)
                LstTesters.SelectAll();

            var tt = LstTesters.SelectedItems;
            ResultPage r = new ResultPage(LstTesters.SelectedItems);
            r.ShowDialog();
        }




        private void LstTesters_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Next.Content = LstTesters.SelectedItems.Count > 0 ? "Продолжить" : "Выбрать все";
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
           Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
    public class FileCentrifugeInfo
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public string Lastname { get; set; }
        public int Age { get; set; }
        public string Path { get; set; }

    }
}
