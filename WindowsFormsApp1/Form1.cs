using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;


namespace WindowsFormsApp1
{  
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.ViewTypes();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            JSONEditorForm.JSONForm = new JSONEditorForm();
            JSONEditorForm.JSONForm.Show();
            JSONEditorForm.JSONForm.textView.Text = Program.FromTextBox();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            if (listBox2.SelectedItem != null)
            {
                using (Deserializer deserializer = new Deserializer())
                {
                    deserializer.Type = Data.GetObject(listBox2.SelectedIndex).GetType();
                    deserializer.ConvertToObject();
                    Data.SetObject(deserializer.Object, listBox2.SelectedIndex);
                }
            }
            else toolStripStatusLabel1.Text = "Не выбран объект из списка!";
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Program.ViewTypes();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Lines = File.ReadAllLines(openFileDialog1.FileName);
                Program.filepath = openFileDialog1.FileName;
                Program.ShowPath();
                toolStripStatusLabel1.Text = "Файл успешно сохранен";
            }
            if (toolStripStatusLabel1.Text == "") toolStripStatusLabel1.Text = "Не удалось открыть файл";
        }
        private void button4_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            if (Program.filepath == null)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllLines(saveFileDialog1.FileName, textBox1.Lines);
                    Program.filepath = saveFileDialog1.FileName;
                    Program.ShowPath();
                    toolStripStatusLabel1.Text = "Файл успешно сохранен";

                }
            }
            else
            {
                File.WriteAllLines(Program.filepath, textBox1.Lines);
                Program.ShowPath();
                toolStripStatusLabel1.Text = "Файл успешно сохранен";

            }
            if (toolStripStatusLabel1.Text == "") toolStripStatusLabel1.Text = "Не удалось сохранить файл";

        }

        private void button5_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(saveFileDialog1.FileName, textBox1.Lines);
                Program.filepath = saveFileDialog1.FileName;
                Program.ShowPath();
                toolStripStatusLabel1.Text = "Файл успешно сохранен";
            }
            if (toolStripStatusLabel1.Text == "") toolStripStatusLabel1.Text = "Не удалось сохранить файл";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            if (listBox1.SelectedItem != null)
            {
                var type = Program.GetTypeFromString(listBox1.SelectedItem.ToString());
                Data.Add(Activator.CreateInstance(type));
            }
            if (toolStripStatusLabel1.Text == "") toolStripStatusLabel1.Text = "Не удалось создать объект";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            if (listBox2.SelectedItem != null)
            {
                Data.Delete(listBox2.SelectedIndex);
                textBox1.Clear();
                textBox3.Clear();
            }
            if (toolStripStatusLabel1.Text == "") toolStripStatusLabel1.Text = "Не удалось удалить объект";
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            if (listBox2.SelectedItem != null)
            {
                using (Serializer serializer = new Serializer())
                {
                    serializer.Object = Data.GetObject(listBox2.SelectedIndex);
                    serializer.Type = serializer.Object.GetType();
                    //Program.GetTypeFromString(listBox2.SelectedItem.ToString());
                    serializer.ConvertToJson();
                }
            }
            else toolStripStatusLabel1.Text = "Не выбран объект из списка!";
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.form.Close();
        }
    }
}
