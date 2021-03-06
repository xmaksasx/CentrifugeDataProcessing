﻿using System;
using System.Runtime.InteropServices;

namespace CentrifugeDataProcessing.Models
{


    //  1.Фамилия(строка с завершающим нулем, общая длина 250 байт)
    //  2.Имя(строка с завершающим нулем, общая длина 250 байт)
    //  3.Отчество(строка с завершающим нулем, общая длина 250 байт)
    //  4.Категория(строка с завершающим нулем, общая длина 250 байт)
    //  5.Дата рождения(double, 8 байт в формате TDateTime)
    //  6.Номер(строка с завершающим нулем, общая длина 250 байт)
    //  7.Тип центрифуги(Цф 6 или 18, сейчас не помню) Длина 4 байта(integer)
    //  8.Далее идет блок результатов проверки остроты зрения:
    //  Массив для проверки остроты зрения, который хранит все значения за тренировку
    //  Первое число: 0 - пусто; 1,2,3,4 - номер нажатой кнопки до этого; 1X - верно,2X - не верно,3X - нет ответа
    //  Второе число: смещение от начал файла, где было измерение остроты (0 - пусто)
    //  unsigned int OstrZren[20][2];
    //  Общая длина 160 байт.
    //  9.Длина текста с информацией(integer 4 байта)
    //  10.Текст с информацией(строка без завершающего нуля. Общая длина из п.9)

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public class DataPacket
    {
        float EKG1; // ЭКГ 1
        float EKG2; // ЭКГ 2
        float EKG3; // ЭКГ 3
        float EKG4; // ЭКГ 4
        float EKG5; // ЭКГ 5
        float EKGR; // ЭКГ R
        public float PAD; //может быть давление странные значения
        public float PADU; //может быть давление странные значения
        public float FPGP; // ФПГ пальца
        public float FPGU; // ФПГ уха
        public float TO; // Тахоосцилляция
        float EMG1; // ЭМГ 1
        float EMG2; // ЭМГ 2
        public float TENZ1; //педаль
        public float TENZ2; //педаль
        public float PG; // Пневмограмма
        public float G; // Перегрузка
        float TR;
        int TP;
        public int CSS; // ЧСС
        public int CD; // Частота дыхания
        public float ADS; // артериальное давление систолическим (АДс)
        int ADSIzmPos;
        public float ADD; // артериальное давление диастолическим (АДд)
        int ADDIzmPos;
        public float ADU; //давление ушное 
        int ADUIzmPos;
        byte Tangenta; // Тангента, нажата/отпущена
        public double Time; // Дата/время текущего пакета

       public ushort EKG1Tech; // 00, Режим(2), 0, ЭКГ-R(1), Ку (2), 00, ФНЧ(2), 00, ФВЧ(2)
       public ushort EKG2Tech; // 00, Режим(2), 0, ЭКГ-R(1), Ку (2), 00, ФНЧ(2), 00, ФВЧ(2)
       public ushort EKG3Tech; // 00, Режим(2), 0, ЭКГ-R(1), Ку (2), 00, ФНЧ(2), 00, ФВЧ(2)
       public ushort EKG4Tech; // 00, Режим(2), 0, ЭКГ-R(1), Ку (2), 00, ФНЧ(2), 00, ФВЧ(2)
       public ushort EKG5Tech; // 00, Режим(2), 0, ЭКГ-R(1), Ку (2), 00, ФНЧ(2), 00, ФВЧ(2)
       public byte FPGPTech; // Режим(2), Ку (2), ФНЧ(2), ФВЧ(2)
       public byte FPGUTech; // Режим(2), Ку (2), ФНЧ(2), ФВЧ(2)
       public byte TOTech; // Режим(2), Ку (2), ФНЧ(2), ФВЧ(2)
       public byte PMTech; // Режим(2), 0, Клапан (1), 0, P max(3)
       public byte PUTech; // Режим(2), 0, Клапан (1), 0, P max(3)
       public byte EMG1Tech; // 00, Режим(2), 00, Ку (2)
       public byte EMG2Tech; // 00, Режим(2), 00, Ку (2)
       public byte PGTech; // 00, Режим(2), Технические метки проводимыхэтапов тренировки(4)
       public byte TENZ1Tech; // 00, Режим(2), 0000
       public byte TENZ2Tech; // 00, Режим(2), 00, ЦФ(1), БОРТ(1)
       public byte ZRNTech; // Острота(3), Метка(1), (1)Ответная реакция, Поле(3)
      
    };

 
}
