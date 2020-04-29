using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Reflection;
using WindowsFormsApp1.Figures;


namespace WindowsFormsApp1
{
    public class Program
    {
        static public Form1 form;
        public static Type[] typelist;
        public static string filepath;

        public static void ShowPath()
        {
            if(filepath != null)
            {
                form.label4.Visible = true;
                form.textBox4.Visible = true;
                form.textBox4.Text = filepath;
            }
        }
        public static void ViewTypes()
        {
            Program.typelist = Program.GetTypesInNamespace(Assembly.GetExecutingAssembly(), form.textBox2.Text);
            form.listBox1.Items.Clear();
            foreach (var type in Program.typelist)
            {
                form.listBox1.Items.Add(type.FullName);
            }
        }

        public static void ToTextBox(string _string)
        {
            Program.form.textBox1.Text = _string;
        }

        public static string FromTextBox()
        {
            return Program.form.textBox1.Text;
        }
        private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }

        public static Type GetTypeFromString(string _string)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetType(_string, true, false);
        }

        [STAThread]
        public static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

            form = new Form1();

            Application.Run(form);
        }
        
    }
}
