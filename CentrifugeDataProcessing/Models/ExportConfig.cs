
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CentrifugeDataProcessing.Annotations;
using Microsoft.Research.DynamicDataDisplay;
using ServiceStack.Text;

namespace CentrifugeDataProcessing.Models
{
	public class ExportConfig:INotifyPropertyChanged
	{
		public string CountG { get; set; }
		
		private bool bg;
		public bool Bg {
			get { return bg; }
			set
			{
				bg = value;
				OnPropertyChanged("Bg");
			}
		}

		public ExportConfig()
		{
			
		}

		public MyClass G3 { get; set; } = new MyClass();
		public MyClass G5 { get; set; } = new MyClass();
		public MyClass G6 { get; set; } = new MyClass();

		public string Export(User foor)
		{
			string str =  GetHead(foor);

			foreach (var item in foor.fm)
			{
				var tm = item.Interval.TypeMode;

				if (Bg && tm == "Bg")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G3.Up && tm == "Up3")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G3.Platform && tm == "Platform3")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G3.Down && tm == "Down3")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G3.Fm && tm == "Fm3")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G3.Lm && tm == "Lm3")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G5.Up && tm == "Up5")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G5.Platform && tm == "Platform5")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G5.Down && tm == "Down5")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G5.Fm && tm == "Fm5")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G5.Lm && tm == "Lm5")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G6.Up && tm == "Up6")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G6.Platform && tm == "Platform6")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G6.Down && tm == "Down6")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G6.Fm && tm == "Fm6")
					str += CsvSerializer.SerializeToString(item.Interval);

				if (G6.Lm && tm == "Lm6")
					str += CsvSerializer.SerializeToString(item.Interval);
			}

			return str;



		}

		private string GetHead(User foor)
		{
			if (CsvConfig<Interval>.OmitHeaders == true)
				return "";
			CsvConfig<Interval>.CustomHeaders = new
			{
				Name = "ФИО",
				ModeName = "Режим",
				MedianCss = "ЧСС (Медиана)",
				MedianCd = "ЧД (Медиана)",
				AvgCss = "ЧСС (Среднее)",
				AvgCd = "ЧД (Среднее)",
				AvgTenz = "Педали (Среднее)",
				AvgTenzL = "ПП (Среднее)",
				AvgTenzR = "ПЛ (Среднее)",
				Add = "АДД",
				Ads = "АДС",
			};

			string[] split = new[] { "\r\n" };
			string[] str = new string[1];
			CsvConfig<Interval>.OmitHeaders = false;
			var strS = CsvSerializer.SerializeToString(foor.fm[0].Interval);
			str = strS.Split(split, StringSplitOptions.RemoveEmptyEntries);
			
			CsvConfig<Interval>.OmitHeaders = true;
			return  str[0]+ "\r\n";
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	public class MyClass : INotifyPropertyChanged
	{
		private bool up;
		public bool Up
		{
			get { return up;}
			set
			{
				up = value;
				OnPropertyChanged("Up");
			}
		}

		private bool platform;
		public bool Platform
		{
			get { return platform; }
			set
			{
				platform = value;
				OnPropertyChanged("Platform");
			}
		}

		private bool down;
		public bool Down
		{
			get { return down; }
			set
			{
				down = value;
				OnPropertyChanged("Down");
			}
		}

		private bool fm;
		public bool Fm
		{
			get { return fm;}
			set
			{
				fm = value;
				OnPropertyChanged("Fm");
			}
		}

		private bool lm;
		public bool Lm
		{
			get { return lm; }
			set
			{
				lm = value;
				OnPropertyChanged("Lm");
			}
		}

		private bool selectAll;
		public bool SelectAll
		{
			
			set
			{
				Up = value;
				Platform = value;
				Down = value;
				Fm = value;
				Lm = value;
			
				OnPropertyChanged();
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
