using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Lab_10
{
    class Avtomat 
    {
        List<string> EntryAlphabet; 
        List<string> StackAlphabet; 
        List<List<string>> Events; 
        Stack<string> stack; 
        string result = "";
        string result2 = "";
        public Avtomat()
        {
            EntryAlphabet = new List<string>(); // входной алфавит (список)
            StackAlphabet = new List<string>(); // стековый алфавит (список)
            Events = new List<List<string>>(); // управляющая таблица (список списков)
            stack = new Stack<string>(); // стек (список)
        }
        public void ReadFromFile()
        {
            StreamReader sr = new StreamReader("Avtomat.txt");
            string s = sr.ReadLine();
            for (int i = 0; i < 3; i++)
            {
                char[] a = s.ToArray();
                if (a.First() == '+')
                {
                    switch (i)
                    {
                        case 0:
                            s = sr.ReadLine();
                            while (s.ToCharArray().First() != '+')
                            {
                                EntryAlphabet.Add(s); // входной алфавит
                                s = sr.ReadLine();
                            }
                            break;
                        case 1:
                            s = sr.ReadLine();
                            while (s.ToCharArray().First() != '+')
                            {
                                StackAlphabet.Add(s); // стековый алфавит
                                s = sr.ReadLine();
                            }
                            break;
                        case 2:
                            s = sr.ReadLine();
                            while (s.ToCharArray().First() != '+')
                            {
                                string[] tmpString = s.Split(' ');
                                List<string> tmpList = new List<string>();
                                for (int j = 0; j < tmpString.Length; j++)
                                    tmpList.Add(tmpString[j]);
                                Events.Add(tmpList); // управляющая таблица
                                s = sr.ReadLine();
                                if (s == null)
                                    break;
                            }
                            break;
                    }
                }
            }
            sr.Close();
        }
        public void PrintInfo()
        {
            Console.Write("Входной алфавит: ");
            for (int i = 0; i < EntryAlphabet.Count; i++)
            {
                Console.Write(EntryAlphabet[i] + " ");
            }
            Console.WriteLine();
            Console.Write("\nСтековый алфавит: ");
            for (int i = 0; i < StackAlphabet.Count; i++)
            {
                Console.Write(StackAlphabet[i] + " ");
            }
            Console.WriteLine();
            Console.WriteLine("\nchange - заменить, shift - сдвиг, cancel - откинуть, pop - вытолкнуть, hold - держать, ok - допустить");
            Console.Write("\nУправляющая таблица: \n");
            for (int i = 0; i < Events.Count; i++)
            {
                for (int j = 0; j < Events[i].Count; j++)
                {
                    Console.Write(Events[i][j] + "  ");
                }
                Console.WriteLine();
            }
        }
        private bool CheckInput(string[] Input)
        {
            bool fl = true;
            for (int i = 0; i < Input.Length; i++)
                if (EntryAlphabet.Contains(Input[i]))
                    continue;
                else
                {
                    fl = false;
                    break;
                }
            return fl;
        }
        private bool CheckEndMarker(string str)
        {
            if (str[str.Length - 1] == EntryAlphabet[EntryAlphabet.Count - 1][0]) // если он есть
                return true;
            else // если его нет
                return false;
        }
        private int FindRowIndex(string s)
        {
            int c = -1;
            for (int i = 0; i < StackAlphabet.Count; i++)
                if (s == StackAlphabet[i])
                    c = i;
            return c;
        }
        private int FindColumnIndex(string s)
        {
            int c = -1;
            for (int i = 0; i < EntryAlphabet.Count; i++)
                if (s == EntryAlphabet[i])
                    c = i;
            return c;
        }
        private void EventStack(int row, int col)
        {
            string EventStr = Events[row][col];
            string[] StackOper = EventStr.Split('|');
            for (int i = 0; i < StackOper.Length; i++)
            {
                if (StackOper[i].Contains("change")) // заменить
                {
                    stack.Pop();
                    string[] strs = GetChange(StackOper[i]);
                    for (int j = strs.Length - 1; j >= 0; j--)
                        stack.Push(strs[j]);
                }
                if (StackOper[i].Contains("out")) // заменить
                {
                    result2 += GetOutput(StackOper[i]);
                }
                if (StackOper[i].Contains("pop")) // вытолкнуть
                {
                    stack.Pop();
                }
                if (StackOper[i] == "cancel") // откинуть
                    result = "ОТКИНУТЬ";
                if (StackOper[i] == "ok") // допустить
                    result = "ДОПУСТИТЬ";
            }
        }

        private string GetOutput(string s)
        {
            int index_start = 0, index_end = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '[')
                    index_start = i;
                if (s[i] == ']')
                    index_end = i;
            }
            return s.Substring(index_start + 1, index_end - index_start - 1) + " ";
        }
        private string[] GetChange(string s)
        {
            int index_start = 0, index_end = 0;
            string[] strs;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(')
                    index_start = i;
                if (s[i] == ')')
                    index_end = i;
            }
            string ch = s.Substring(index_start + 1, index_end - index_start - 1);
            strs = ch.Split(',');
            return strs;
        }
        private bool EventString(int row, int col)
        {
            string EventStr = Events[row][col];
            string[] StrOper = EventStr.Split('|');
            for (int i = 0; i < StrOper.Length; i++)
            {
                if (StrOper[i] == "shift") // сдвиг
                {
                    return true;
                }
                if (StrOper[i] == "hold") // держать
                {
                    return false;
                }
                if (StrOper[i] == "cancel") // откинуть
                    result = "ОТКИНУТЬ";
                if (StrOper[i] == "ok") // допустить
                    result = "ДОПУСТИТЬ";
            }
            return false;
        }
        private void PrintCurrData(int index, string[] str)
        {
            Console.Write("Текущее стековое состояние: ");
            for (int i = stack.Count - 1; i >= 0; i--)
                Console.Write(stack.ElementAt(i) + " ");
            Console.WriteLine();
            Console.Write("Текущая входная строка: ");
            for (int i = index; i < str.Length; i++)
                Console.Write(str[i] + " ");
            Console.WriteLine("\n---------------------------------------------------------");
        }
        public void RunMachine(string str)
        {
            if (!CheckEndMarker(str)) // проверка конечного маркера
            {
                Console.WriteLine("\nКонечный маркер не обнаружен! Он будет установлен!\n");
                str += " " + EntryAlphabet[EntryAlphabet.Count - 1];
            }
            string[] InputString = str.Split(' ');
            if (!CheckInput(InputString)) //не входит в исходный алфавит
            {
                Console.WriteLine("Входная строка не входит в исходный алфавит!");
                Environment.Exit(0);
            }
            stack.Push(StackAlphabet[StackAlphabet.Count - 1]);
            stack.Push("<S>");
            for (int i = 0; i < InputString.Length; i++)
            {
                PrintCurrData(i, InputString);
                int col = FindColumnIndex(InputString[i]);
                int row = FindRowIndex(stack.Peek());
                EventStack(row, col); 
                if (EventString(row, col) == false) 
                    i--;
                if (result != "") 
                    break;
            }
            Console.WriteLine("Результат:\t" + result);
            if (result != "ОТКИНУТЬ")
                Console.WriteLine("Результат транслитерации:\t" + result2);
        }
    }
}
