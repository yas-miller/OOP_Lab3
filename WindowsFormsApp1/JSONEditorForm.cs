using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JSON;

namespace WindowsFormsApp1
{
	public partial class JSONEditorForm : Form
	{
		static public JSONEditorForm JSONForm;
		public JSONEditorForm()
		{
			InitializeComponent();
		}
		private TreeNode newNode(string name, object a)
		{
			TreeNode t = new TreeNode(name);

			if (a == null)
			{
				// JSON 'null' value
				t.Tag = new jsonValue();
				return t;
			}

			switch (a.GetType().Name)
			{
				case "jsonBoolean":
				case "jsonString":
				case "jsonReal":
				case "jsonNumber":
				case "jsonInteger":
					t.Text += " : " + (string)((jsonValue)a);
					t.Tag = (jsonValue)a;
					break;

				case "ArrayList":
					int i = 0;
					t.Tag = "[";
					foreach (object o in (System.Collections.ArrayList)a)
						t.Nodes.Add(newNode(null, o));
					break;

				default:
					t.Tag = "{";
					foreach (KeyValuePair<string, object> o in (Dictionary<string, object>)a)
						t.Nodes.Add(newNode(o.Key, o.Value));
					break;
			}

			return t;
		}

		StringBuilder sb;
		int indentCount = 0;
		string indentString = "   ";

		private void endLine(string s)
		{
			sb.Append(s);
			sb.Append(Environment.NewLine);
		}

		private void newLine(string s)
		{
			for (int i = 0; i < indentCount; i++)
				sb.Append(indentString);
			endLine(s);
		}

		private void indent(string s)
		{
			indentCount++;
			for (int i = 0; i < indentCount; i++)
				sb.Append(indentString);
			endLine(s);
		}

		private void dedent(string s)
		{
			for (int i = 0; i < indentCount; i++)
				sb.Append(indentString);
			indentCount--;
			endLine(s);
		}

		private void scanTree(TreeNode t)
		{
			foreach (TreeNode o in t.Nodes)
			{
				if (o.Nodes.Count == 0)
				{
					// leaf node
					for (int i = 0; i < indentCount; i++)
						sb.Append(indentString);
					string[] s = o.Text.Split(':');
					if ((s[0] = s[0].Trim()) != "")
						// not an array, so emit the name
						sb.Append("\"" + s[0] + "\" : ");
					// emit the value string
					sb.Append(((jsonValue)o.Tag).Emit());
					endLine(",");
				}
				else
				{
					newLine("\"" + o.Text + "\" : ");
					indent((string)o.Tag);
					scanTree(o);
					dedent(((string)o.Tag)[0] == '[' ? "]," : "},");
				}
			}
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (tabControl.SelectedIndex)
			{
				case 0:
					treeView.Nodes.Clear();
					try
					{
						treeView.Nodes.Add(treeView.TopNode = newNode(textView.Text[0].ToString(), JSON.JSON.Parse(textView.Text)));
					}
					catch
					{
					
					}
					treeView.Show();
					break;

				case 1:
					if (treeView.Nodes.Count != 0)
					{
						// scan the tree to build a JSON text string
						sb = new StringBuilder();
						newLine(treeView.TopNode.Text);
						scanTree(treeView.TopNode);
						newLine((treeView.TopNode.Text)[0] == '[' ? "]" : "}");
						textView.Text = sb.ToString();
					}
					break;
			}
		}

		private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeView tree = (TreeView)sender;
			keyTextBox.Text = tree.SelectedNode.Text.Split(new char[] { ' ', ':' })[0];
			if (tree.SelectedNode.Text.Contains(" : ") == false)
			{
				// we done annotate nodes
				valueTextBox.Text = "";
				return;
			}

			// at a leaf node, tag should always be a JSON 'basic' type: string, number, boolean, null and stored as a jsonValue
			Type t = tree.SelectedNode.Tag.GetType();
			valueTextBox.Text = (string)((jsonValue)tree.SelectedNode.Tag);
		}

		private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				treeView.SelectedNode = treeView.GetNodeAt(e.X, e.Y);
				int x = e.X + this.Left + treeView.Left;
				int y = e.Y + this.Top + treeView.Top;
				contextMenuStrip.Show(x, y);
			}
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeView tree = treeView;

			tree.Nodes.Remove(tree.SelectedNode);

			keyTextBox.Text =
			valueTextBox.Text = "";
		}

		private void keyTextBox_TextChanged(object sender, EventArgs e)
		{
			TextBox b = (TextBox)sender;
			if (treeView.SelectedNode.Text.Contains(" : ") == false)
			{
				// we don't annotate nodes
				treeView.SelectedNode.Text = b.Text;
				valueTextBox.Text = "";
				return;
			}

			// leaf node
			treeView.SelectedNode.Text = b.Text + " : " +
			(valueTextBox.Text = (string)((jsonValue)treeView.SelectedNode.Tag));
		}

		private void valueTextBox_TextChanged(object sender, EventArgs e)
		{
			if (treeView.SelectedNode.Text.Contains(" : ") == false)
			{
				// we don't annotate nodes
				valueTextBox.Text = "";
				return;
			}

			// leaf node
			TreeNode t = treeView.SelectedNode;
			t.Text = keyTextBox.Text + " : " + valueTextBox.Text;
			switch (t.Tag.GetType().Name)
			{
				case "jsonBoolean":
					t.Tag = (jsonBoolean)valueTextBox.Text;
					break;
				case "jsonString":
					t.Tag = (jsonString)valueTextBox.Text;
					break;
				case "jsonReal":
					t.Tag = (jsonReal)valueTextBox.Text;
					break;
				case "jsonNumber":
					t.Tag = (jsonNumber)valueTextBox.Text;
					break;
				case "jsonInteger":
					t.Tag = (jsonInteger)valueTextBox.Text;
					break;
			}
		}
		private void button1_Click(object sender, EventArgs e)
		{
			if (treeView.Nodes.Count != 0)
			{
				// scan the tree to build a JSON text string
				sb = new StringBuilder();
				newLine(treeView.TopNode.Text);
				scanTree(treeView.TopNode);
				newLine((treeView.TopNode.Text)[0] == '[' ? "]" : "}");
				textView.Text = sb.ToString();
			}

			Program.form.textBox1.Text = textView.Text;
			Close();
		}
		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void textView_TextChanged(object sender, EventArgs e)
		{
			treeView.Nodes.Clear();
			try
			{
				treeView.Nodes.Add(treeView.TopNode = newNode(textView.Text[0].ToString(), JSON.JSON.Parse(textView.Text)));
			}
			catch
			{
			
			}
			treeView.Show();
		}
	}
}
