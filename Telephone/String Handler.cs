using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp26
{
    public enum ButtonMode
    {
        Numbers = 0,
        Ukraine = 1,
        English = 2
    }
    public class StringHandler
    {
        public string buffer = "";

        public ButtonMode buttonMode = ButtonMode.Numbers;

        public static int index = 0;

        public static bool next = false;

        Thread thread = new Thread(Timer);

        int prevTag = 0;

        public Dictionary<int, List<string>> keyDictionary = new Dictionary<int, List<string>>()
        {
            { 1,null},
            { 2,new List<string>(){"АБВГ","ABC"} },
            { 3,new List<string>(){"ДЕЄЖ","DEF"} },
            { 4,new List<string>(){"ЗИІЇ","GHI"} },
            { 5,new List<string>(){"ЙКЛМ","JKL"} },
            { 6,new List<string>(){"НОПР","MNO"} },
            { 7,new List<string>(){"СТУФ","PQRS"} },
            { 8,new List<string>(){"ХЦЧШ","TUV"} },
            { 9,new List<string>(){"ШЬЮЯ","WXYZ"} },
            { 10,new List<string>(){"*", "."} },
            { 11,new List<string>(){" "} },
            { 12,new List<string>(){"#", ","} },
        };

        internal string Place(char data,string text)
        {
            string leftBuffer = "";
            string rightBuffer = "";
            int curentPos = text.LastIndexOf('|');
            if (next == true)
            {
                string newString = "";

                try
                {
                    for (int i = 0; i < curentPos - 1; i++)
                    {
                        newString += text[i];
                    }
                    newString += data;
                    for (int i = curentPos; i < text.Length; i++)
                    {
                        newString += text[i];
                    }
                    return newString;
                }
                catch
                {
                    text += data;
                    text += '|';
                    return text;
                }
            }
            else
            {
                if (curentPos == -1)
                {
                    return (data.ToString() + '|'.ToString());
                }
                else
                {
                    string newString = "";

                    for (int i = 0; i < curentPos; i++)
                    {
                        leftBuffer += text[i];
                    }
                    for (int i = curentPos + 1; i < text.Length; i++)
                    {
                        rightBuffer += text[i];
                    }

                    newString = leftBuffer;
                    newString += data.ToString() + '|'.ToString();
                    newString += rightBuffer;

                    return newString;
                }
            }
        }

        internal void AbortThread()
        {
            if(thread.IsAlive)
            {
                thread.Abort();
            }
            index = 0;
            next = false;
        }

        internal string Clear(string text)
        {
            string _buffer = "";
            int curentPos = text.LastIndexOf('|');

            if(curentPos == text.Length-1)
            {
                string tempBuffer = "";

                for(int i = 0;i < buffer.Length-1;i++)
                {
                    tempBuffer += buffer[i];
                }
                buffer = tempBuffer;
            }

            for(int i = 0;i < curentPos-1;i++)
            {
                _buffer += text[i];
            }
            for(int i = curentPos;i < text.Length;i++)
            {
                _buffer += text[i];
            }
            return _buffer;
        }

        public bool newSentence()
        {
            int lenght = buffer.Length - 1;
            try
            {
                if (buffer[lenght-1] == ' ')
                {
                    if (buffer[lenght - 2] == '.')
                    {
                        return true;
                    }

                    buffer = buffer[buffer.Length - 1].ToString();
                }
            }
            catch
            {
                return true;
            }
            return false;
        }

        public char OutputData(int buttonNum, ButtonMode mode)
        {
            if(buttonNum != prevTag)
            {
                prevTag = buttonNum;
                AbortThread();
            }
            int i = buttonNum;

            char data = ' ';
            if (keyDictionary[i] == null)
            {
                data = Convert.ToChar(i + 48);
            }
            else
            {


                if (mode == ButtonMode.Numbers)
                {
                    if (i >= 10)
                    {
                        if (i == 10)
                        {
                            data = '*';
                        }
                        else if (i == 11)
                        {
                            data = '0';
                        }
                        else
                        {
                            data = '#';
                        }
                    }
                    else
                    data = Convert.ToChar(i + 48);
                }
                else if (mode == ButtonMode.Ukraine)
                {
                    if (i == 10)
                    {
                        data = '.';
                    }
                    else if (i == 12)
                    {
                        data = ',';
                    }
                    else
                    data = keyDictionary[i][0][index];
                }
                else
                {
                    try
                    {
                        data = keyDictionary[i][1][index];
                    }
                    catch
                    {
                        data = keyDictionary[i][0][index];
                    }
                }
            }

            if (!next)
            {
                buffer += data;
            }
            else
            {
                string tempBuffer = "";
                for (int j = 0; j < buffer.Length - 1; j++)
                {
                    tempBuffer += buffer[j];
                }
                tempBuffer += data;
                buffer = tempBuffer;
            }

            if (!next)
            {
                thread = new Thread(Timer);
                thread.Start();
            }
            else
            {
                if (thread.ThreadState == ThreadState.Running || thread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    thread.Abort();
                    thread = new Thread(Timer);
                    thread.Start();
                }
            }

            if (newSentence())
                return data;
            else
                return Convert.ToChar(data.ToString().ToLower());
        }

        private static void Timer()
        {
            Thread.Sleep(100);
            next = true;
            if (index != 3)
                index++;
            else
                index = 0;
            Thread.Sleep(500);
            next = false;
            index = 0;
            Thread.CurrentThread.Abort();
        }

        public string Replace(string text, char obj, int Side)
        {
            int curentPos = text.LastIndexOf(obj);
            string newString = text;
            string buffer = "";
            try
            {
                if (Side == -1)
                {
                    buffer += text[curentPos];
                    buffer += text[curentPos - 1];

                    newString = "";
                    for(int i = 0;i < curentPos - 1;i++)
                    {
                        newString += text[i];
                    }
                    newString += buffer;
                    for(int i = curentPos+1;i < text.Length;i++)
                    {
                        newString += text[i];
                    }
                }
                else 
                {
                    buffer += text[curentPos + 1];
                    buffer += text[curentPos];

                    newString = "";
                    for (int i = 0; i < curentPos; i++)
                    {
                        newString += text[i];
                    }
                    newString += buffer;
                    for (int i = curentPos + 2; i < text.Length; i++)
                    {
                        newString += text[i];
                    }
                }
            

            }
            catch
            {

            }
            
            return newString;
        }

    }
}
