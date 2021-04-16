using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CentrifugeDataProcessing.Models;


namespace CentrifugeDataProcessing
{
	/// <summary>
	/// Interaction logic for ExportWindow.xaml
	/// </summary>
	public partial class ExportWindow : Window
	{
		private ExportConfig _exportConfig;
		public ExportWindow(ExportConfig exportConfig)
		{
			_exportConfig = exportConfig;
			InitializeComponent();
	    	DataContext = _exportConfig;

		}
		private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{

			this.Visibility = Visibility.Hidden; 
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			this.Visibility = Visibility.Hidden;
		}

		private void AllCount_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var t = (ComboBox)sender;
			var t1 = (ComboBoxItem)t.SelectedItem;
			_exportConfig.CountG = t1.Tag.ToString();
		}

		private void AllType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var t = (ComboBox) sender;
			var t1 = (ComboBoxItem) t.SelectedItem;
			//_exportConfig.TypeG = t1.Tag.ToString();
		}

		
		

		private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
		{
			_exportConfig.Bg = true;
			_exportConfig.G3.SelectAll = true;
			_exportConfig.G5.SelectAll = true;
			_exportConfig.G6.SelectAll = true;
		}

		private void all_Unchecked(object sender, RoutedEventArgs e)
		{
			_exportConfig.Bg = false;
			_exportConfig.G3.SelectAll = false;
			_exportConfig.G5.SelectAll = false;
			_exportConfig.G6.SelectAll = false;
		}
	}
}
