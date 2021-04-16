using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CentrifugeDataProcessing.Models
{
	public class ProcessingHelper
	{
		private static byte _sizeOfPacket = 144;
		private static byte _offsetToAds = 84;
		private static byte _offsetToAdd = 92;
		private static byte _offsetToTime = 112;
		private static byte _offsetToPg = 137;
		private bool _firstminute;
		private int _typeMode = 3;
		private float _aDd = 0;
		private float _aDs = 0;
		public List<Ss> fm;
		private DateTime _start = default;
		private string _name;

		public void FindingInterval(byte[] bytes, string name)
		{
			_name = name;
			int pos = GetPos(bytes);
			int len = bytes.Length;
			fm = new List<Ss>();
			bool isFind = false;
			for (int ix = pos; ix < len; ix += _sizeOfPacket)
			{
				int posToAds = ix + _offsetToAds;
				int posToAdd = ix + _offsetToAdd;

				int posToTime = ix + _offsetToTime;
				int posToPg = ix + _offsetToPg;

				float aDs = BitConverter.ToSingle(bytes, posToAds);
				float aDd = BitConverter.ToSingle(bytes, posToAdd);

				if (aDs > 0)
					_aDs = aDs;
				if (aDd > 0)
					_aDd = aDd;


				double time = BitConverter.ToDouble(bytes, posToTime);
				byte pG = bytes[posToPg];
				isFind = IsFind(pG, isFind, time);
				if (aDd > 0 && pG != 19 && pG != 20 && pG != 21)
				{
					string content = "Первая минута";
					string _typeModeStr =  "Fm" + _typeMode;
					if (pG == 23 && !_firstminute)
					{
						content = "Первая минута";
						_typeModeStr = "Fm" + _typeMode;
						_firstminute = !_firstminute;

					}else if (pG == 23 && _firstminute)
					{
						content = "Последняя минута";
						_typeModeStr = "Lm" + _typeMode;
						_firstminute = !_firstminute;
						if (_typeMode == 3)
							_typeMode = 5;
						else if (_typeMode == 5)
							_typeMode = 6;
					}

					if (pG == 17)
					{
						content = "Фон";
						_typeModeStr = "Bg";

					}
					if (pG == 16)
					{
						var item = fm[fm.Count - 2];

						item.Interval.Add = _aDd;
						item.Interval.Ads = _aDs;

						_aDd = 0;
						_aDs = 0;
						continue;
					}

					var packet = GetDataPacket(bytes, ix);
					fm.Add(new Ss()
					{
						Start = DateTime.FromOADate(time).AddSeconds(-30),
						Stop = DateTime.FromOADate(time).AddSeconds(30),
						Interval = new Interval() { ModeName = content, Add = packet.ADD, Ads = packet.ADS, TypeMode = _typeModeStr, Name = name }
						
					});
					_aDd = 0;
					_aDs = 0;

				}

			}

			Console.Write(fm.Count + " ");

		}

		private DataPacket GetDataPacket(byte[] bytes, int ix)
		{
			byte[] data = new byte[_sizeOfPacket];
			DataPacket packet = new DataPacket();
			Array.Copy(bytes, ix, data, 0, 144);
			ByteToObject(data, packet);
			return packet;
		}

		private bool IsFind(byte pG, bool isFind, double time)
		{
			switch (pG)
			{
				case 19:
					if (!isFind)
					{
						_start = DateTime.FromOADate(time);
						isFind = true;
					}

					break;
				case 20:
					if (isFind)
						isFind = AddInterval(isFind, time, "Подъем", "Up"+ _typeMode );
					break;

				case 21:
					if (!isFind)
					
						isFind = AddInterval(isFind, time, "Площадка","");
						

					break;

				case 16:
					if (isFind)
						isFind = AddInterval(isFind, time, "Спуск", "Down" + _typeMode);
					break;
			}

			return isFind;
		}

		private bool AddInterval(bool isFind, double time, string content, string typeMode)
		{
			if (_aDd !=0 && content== "Спуск")
			{
				var item = fm[fm.Count - 1];

				item.Interval.Add = _aDd;
				item.Interval.Ads = _aDs;

				_aDd = 0;
				_aDs = 0;
			}
			
			fm.Add(new Ss()
				{Start = _start, Stop = DateTime.FromOADate(time),  Interval = new Interval(){ModeName = content, Add = _aDd, Ads = _aDs, TypeMode = typeMode, Name = _name } });
			_start = DateTime.FromOADate(time);
			_aDd = 0;
			_aDs = 0;
			return !isFind;

		}

		public void AddPacketToIntervals(byte[] bytes)
		{
			int pos = GetPos(bytes);
			int len = bytes.Length;
			for (int ix = pos; ix < len; ix += 576)
			{
				byte[] data = new byte[144];
				int posToTime = ix + _offsetToTime;
				var time = DateTime.FromOADate(BitConverter.ToDouble(bytes, posToTime));

				foreach (var item in fm)
				{
					if (time.IsWithin(item.Start, item.Stop))
					{
						DataPacket packet = GetDataPacket(bytes, ix);
						item.Interval.Data.Add(packet);
					}
				}
			}
		}

        public int Calculate()
        {
	        int count = 0;
	        foreach (var item in fm)
	        {
		        item.Interval.Calc();
		        if (item.Interval.ModeName == "Площадка")
		        {
			        count++;
			    
		        }
	        }


	        return count;
        }

		public void ByteToObject<T>(byte[] receiveBytes, T obj)
		{
			int len = Marshal.SizeOf(obj);
			IntPtr i = Marshal.AllocHGlobal(len);
			Marshal.Copy(receiveBytes, 0, i, len);
			Marshal.PtrToStructure(i, obj);
			Marshal.FreeHGlobal(i);
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
	}

	public class Ss
	{
		public DateTime Start;
		public DateTime Stop;
		public Interval Interval;
	}
}