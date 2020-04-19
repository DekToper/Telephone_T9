using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp26
{

    public partial class Form1 : Form
    {
        public ButtonMode buttonMode = ButtonMode.Numbers;

        StringHandler stringHandler;

        Thread thread;

        string text = "";

        StreamReader r = new StreamReader("../../words.txt",encoding:Encoding.Default);

        List<string> list = new List<string>();

        public Form1()
        {
            InitializeComponent();

            stringHandler = new StringHandler();
         
        }

        private void button_Click(object sender, EventArgs e)
        {     
            char data = stringHandler.OutputData(Convert.ToInt32((sender as Button).Tag),buttonMode);
            textBox.Text = stringHandler.Place(data, textBox.Text);
        }


        public void EditTextBox(int i, string U, string E,int number)
        {
            
        }

        private void clear_Button_Click(object sender, EventArgs e)
        {
            textBox.Text = stringHandler.Clear(textBox.Text);
            stringHandler.AbortThread();

        }

        private void switchMode_Click(object sender, EventArgs e)
        {
            if(buttonMode == ButtonMode.Numbers)
            {
                buttonMode = ButtonMode.Ukraine;
            }
            else if(buttonMode == ButtonMode.Ukraine)
            {
                buttonMode = ButtonMode.English;
            }
            else
            {
                buttonMode = ButtonMode.Numbers;
            }

            stringHandler.AbortThread();
            (sender as Button).Text = buttonMode.ToString();
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            string newText = stringHandler.Replace(textBox.Text, '|', -1);
            textBox.Text = newText;
            textBox.Update();
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {
            string newText = stringHandler.Replace(textBox.Text, '|', 0);
            textBox.Text = newText;
            textBox.Update();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            label2.Text = stringHandler.buffer;
        }

        public List<string> GetStrings(StreamReader r)
        {
            List<string> list = new List<string>();
            while(true)
            {
                
                string line = r.ReadLine();
                if(line == null)
                {
                    break;
                }
                list.Add(line);
                
            }
            return list;
        }

        public void T9()
        {
            list = GetStrings(r);
            r.Dispose();
            text = "";
            bool b = false;
            while(true)
            {
                foreach(string item in list)
                {
                    if (stringHandler.buffer != "")
                    {
                        for (int i = 0; i < stringHandler.buffer.Length; i++)
                        {
                            try
                            {
                                if (stringHandler.buffer.ToLower()[i] != item.ToLower()[i])
                                {
                                    b = false;
                                    break;
                                }
                                else
                                {
                                    b = true;
                                }
                            }
                            catch
                            {
                                b = false;
                                break;
                            }
                        }
                        if (stringHandler.buffer.ToLower() != item.ToLower() && b == true)
                        {                            
                            text = item;
                            break;
                        }
                    }
                    else 
                    {
                        text = "";
                    }
                        
                }
                textBox1.Invoke(new Action(() => { textBox1.Text = text; }));
                Thread.Sleep(500);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            thread = new Thread(T9);
            thread.Start();
        }

        private void Form_Closed(object sender, FormClosedEventArgs e)
        {
            thread.Abort();
        }

        private void Enter_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < text.Length;i++)
            {
                try
                {
                    if (stringHandler.buffer.ToLower()[i] != text.ToLower()[i])
                    {
                        textBox.Text = stringHandler.Place(text[i], textBox.Text);
                    }
                }
                catch
                {
                    textBox.Text = stringHandler.Place(text[i], textBox.Text);
                }
            }
            label2.Text = text.ToUpper();
            stringHandler.buffer = text.ToUpper();
            text = "";
            textBox1.Text = "";

        }

        private void add_Button_Click(object sender, EventArgs e)
        {
            StreamWriter writer = new StreamWriter("../../words.txt",true, encoding: Encoding.Default);
            writer.WriteLine(stringHandler.buffer.ToLower());
            writer.Dispose();
            StreamReader r = new StreamReader("../../words.txt", encoding: Encoding.Default);
            list = GetStrings(r);
            r.Dispose();
        }
    }
}
