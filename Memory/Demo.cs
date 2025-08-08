using System;
using System.Collections.Generic;
using System.Threading;

namespace Memory;

static class Demo
{

    static void Main()
    {
        string name = "Test.exe";
        IEnumerator<IntorProcc> iEIP = ProcessSearch.SearchMachineName(name);
        while (!iEIP.MoveNext())
        {
            Thread.Sleep(15);
            iEIP = ProcessSearch.SearchMachineName(name);
        }

        using (ProcessMemory m = ProcessMemory.Create(ProcessMemory.Type.Basic, iEIP.Current))
        {
            //Получение финального адреса от базового адреса
            IntPtr address = m.FinalAddressBass([0X10, 0xf2]);
            //Получение значение по адресу
            int a = m.ReadInt32(address);
            //Запись значение
            m.WriteInt32(address, a);

            //Создание маску
            IMask[] mask = IMask.ParseSignature("00 ?? FF FF ?? ?? 48");
            //Сигнатурный поиск
            address = m.SearchAddress(mask);

            IMask[][] masks = { 
                IMask.ParseSignature("00 ?? FF FF ?? ?? 48"), 
                IMask.ParseSignature("00 ?? FF FF ?? ?? 48") 
            };
            IntPtr[] adds = m.SearchAddress(masks);

            Position pos = m.ReadStruct<Position>(address);

            m.WriteStruc(address, pos);
            //С коррекцией батов если структура неровная
            m.WriteStructСorrector(address, pos);

            m.ReadToWrite<Position>(adds[0], adds[2]);

        }

    }

    public struct Position
    {

        public float x;
        public float y;

    }

}
