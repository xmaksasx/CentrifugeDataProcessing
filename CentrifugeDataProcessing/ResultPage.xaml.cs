using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CentrifugeDataProcessing.Models;
using Microsoft.Win32;
using ServiceStack.Text;


namespace CentrifugeDataProcessing
{
    /// <summary>
    /// Interaction logic for ResultPage.xaml
    /// </summary>

    public partial class ResultPage : Window
    {
        private readonly List<FileCentrifugeInfo> _ilst;
        private List<User> _users;
        private User _user;

        private ExportWindow exportWindow;
		ExportConfig exportConfig = new ExportConfig();
        public ResultPage(IList ilst)
        {
            _ilst = ilst.Cast<FileCentrifugeInfo>().ToList();
            _users = new List<User>();
            InitializeComponent();
            ProgressLoad.Maximum = ilst.Count-1;
            ProgressDEvent += ProgressDe;
           
            Task.Run(() => Parallel.ForEach(_ilst, new ParallelOptions() { MaxDegreeOfParallelism = 1 }, d => { Simulation(d); }));
            exportWindow = new ExportWindow(exportConfig);
		}

        private void ProgressDe(int idx)
        {

            double max = 0;
            double val = 0;

            Dispatcher.Invoke(() => val = ProgressLoad.Value++);
            Dispatcher.Invoke(() => max = ProgressLoad.Maximum);
            Dispatcher.Invoke(() => MaximumFile.Text = ProgressLoad.Maximum.ToString());
            Dispatcher.Invoke(() => CurrentFile.Text = ProgressLoad.Value.ToString());
            if (val == max)
            {
	            var end = DateTime.Now;
                Dispatcher.Invoke(() => ProgressGrid.Visibility = Visibility.Hidden);
                Dispatcher.Invoke(() => GridProducts.ItemsSource = _users);
              
            }
        }

        private void Simulation(FileCentrifugeInfo file)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            if (File.Exists(file.Path))
            {
	            var begin = DateTime.Now;
	            var name = file.Family + " " +
	                       file.Name + " " +
	                       file.Lastname;
                _user = new User(File.ReadAllBytes(file.Path)) { Name = name, Path = file.Path};
                _user.Prepare();
                _users.Add(_user);
                var end = DateTime.Now;
	            Console.WriteLine((end - begin).Seconds+":"+(end - begin).Milliseconds + " - "+ file.Family);
	            OnProgressDEvent(0);
            }
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
	        Close();
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

        private static StreamWriter SaveDialog()
        {
	        StreamWriter sw;
	        CsvConfig.ItemSeperatorString = ";";
	        CsvConfig<Interval>.OmitHeaders = false;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
	        saveFileDialog.Filter = "Csv files (*.csv)|*.csv";
	        saveFileDialog.FilterIndex = 1;
	        saveFileDialog.RestoreDirectory = true;
	        if (saveFileDialog.ShowDialog() == true)
		        return sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8);
	        return null;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
	        IList rows = GridProducts.SelectedItems;
            User model = (sender as Button).DataContext as User;
	        var vd = new ViewerData(model.Path);
	        vd.ShowDialog();
        }
 
        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
	        exportWindow.ShowDialog();
	        StreamWriter sw = SaveDialog();
	        if (sw == null)
		        return;
	        var countG = exportConfig.CountG;

			CsvConfig<Interval>.OmitHeaders = false;
			foreach (var foor in _users)
	        {
		        switch (countG)
		        {
			        case "CountAll":
				        if (foor.Count > 0)
					        sw.Write(exportConfig.Export(foor));
				        break;
			        case "Count2":
				        if (foor.Count == 2)
					        sw.Write(exportConfig.Export(foor));
				        break;

			        case "Count3":
				        if (foor.Count == 3)
					        sw.Write(exportConfig.Export(foor));
				        break;
		        }
	        }
	        sw.Close();
        }

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			exportWindow.Close();
		}
	}
    
	public static class Extension
    {
	    public static bool IsWithin<T>(this T value, T minimum, T maximum) where T : IComparable<T>
	    {
		    if (value.CompareTo(minimum) < 0)
			    return false;
		    if (value.CompareTo(maximum) > 0)
			    return false;
		    return true;
	    }
    }
}
