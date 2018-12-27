using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Lab15
{
    class Program
    {
        private static object locker = new object();
        static int q = 0;

        public static void getProcessesInfo()//получение информации о запущеных процессах
        {
            Console.WriteLine("Процессы:");
            foreach (Process process in Process.GetProcesses())//получение списка всех запущеных процессов
            {
                Console.WriteLine("Имя: " + process.ProcessName + "\tId:" + process.Id + "\tВыделенный объем памяти:" + process.VirtualMemorySize64);//вывод информации о процессе
            }
            Console.WriteLine();
        }

        public static void getDomainInfo()// вывод информации о домене приложения
        {
            Console.WriteLine("Домен:");
            AppDomain domain = AppDomain.CurrentDomain;//получение домена приложения
            Console.WriteLine("Имя:"+domain.FriendlyName+"\tКонфигурация:"+domain.SetupInformation);
            Console.WriteLine("Сборки домена:");
            Assembly[] assemblies = domain.GetAssemblies();//получение спсика загруженных библиотек в домен
            foreach (Assembly asm in assemblies)//вывод имен библиотек
            {
                Console.WriteLine(asm.GetName().Name);
            }
            Console.WriteLine();
            AppDomain domain2 = AppDomain.CreateDomain("domain2");//создание домена
            domain2.Load(new AssemblyName("mscorlib"));//загрузка библиотеки в домен
            AppDomain.Unload(domain2);//выгрузка домена
        }

        public static void OneToN(object x)
        {
            StreamWriter sw = new StreamWriter("thread.txt");
            int n = (int)x;
            Console.WriteLine();
            for(int i = 1; i < n+1; i++)
            {
                sw.WriteLine(i);
                Console.WriteLine(i);
                Thread.Sleep(300);
            }
            sw.Close();
            Console.WriteLine();
        }

        public static void Even(object x)
        {
            lock (locker)//вход в критическую секцию
            {
                StreamWriter sw = new StreamWriter("OddAndEven.txt",true);
                int n = (int)x;
                int i1 = 0;
                while (i1 <= n)
                { 
                    sw.WriteLine(i1);
                    Console.WriteLine("Четное: " + i1);
                    i1 += 2;
                    Thread.Sleep(500);//пауза потока на пол секунды
                }

                sw.Close();
            }
        }

        public static void Odd(object x)
        {
            lock (locker)//вход в критическую секцию
            {
                StreamWriter sw = new StreamWriter("OddAndEven.txt",true);
                int n = (int)x;
                int i2 = 1;
                while (i2 <= n)
                {
                    sw.WriteLine(i2);
                    Console.WriteLine("Нечетное: " + i2);
                    i2 += 2;
                    Thread.Sleep(1000);//пауза потока на секунду
                }

                sw.Close();
            }
        }

        
        public static void Function(object obj)
        { 
            Console.WriteLine("Прошло времени: "+q+" секунд");
            q++;
        }

        static void Main(string[] args)
        {
            getProcessesInfo();
            getDomainInfo();
            //инициализация делегатов для потоков(можно на прямую передать сразу метод: Thread threadOnetoN = new Thread(OneToN);)
            ParameterizedThreadStart theDelegate0 = OneToN;
            ParameterizedThreadStart theDelegate = Even;
            ParameterizedThreadStart theDelegate2 = Odd;
            Thread evenThread1 = new Thread(theDelegate);
            Thread oddThread1 = new Thread(theDelegate2);
            Thread threadOnetoN = new Thread(theDelegate0);
            threadOnetoN.Priority = ThreadPriority.Lowest; // установка приоритета
            threadOnetoN.Name = "OneToN"; // установка имени потока
            Console.WriteLine("Введите число:");
            int n = int.Parse(Console.ReadLine());
            Console.WriteLine("Имя потока: " + threadOnetoN.Name);
            Console.WriteLine("Статус потока: " + threadOnetoN.ThreadState);
            Console.WriteLine("Приоритет потока: " + threadOnetoN.Priority);
            threadOnetoN.Start(n);// запуск потока с предачей параметра в метод потока
            Console.ReadKey();
            evenThread1.Start(n);
            oddThread1.Start(n);
            Console.ReadKey();
            Console.WriteLine();
            TimerCallback tm = Function; //инициализация функции таймера(можно на прямую передать сразу метод: TTimer timer = new Timer(Function, null,0, 1000);)
            Timer timer = new Timer(tm, null, 0, 1000);// создание таймера, так как задержка старта выставлена в 0 таймер запускается сразу
            Console.ReadLine();
            Console.ReadKey();

        }
    }
}