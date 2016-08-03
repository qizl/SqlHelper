using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ReflectionTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private System.Collections.Generic.List<Entities.Widget> widgets = null;

        private void EnsureWidgets()
        {
            if (widgets == null)
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                int numWidgets = 1000000;
                widgets = new List<Entities.Widget>();
                for (int i = 1; i < numWidgets; i++)
                {
                    widgets.Add(new Entities.Widget(i, "Widget " + i.ToString()));
                }

                sw.Stop();
                AddDebugLine("Created List with " + numWidgets + " Widgets in " + sw.ElapsedMilliseconds + " ms.");
            }
        }

        private void btnRunTest_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            this.Cursor = Cursors.WaitCursor;

            try
            {
                EnsureWidgets();

                Int32 id;
                String name;

                AddDebugLine("");

                #region Compiled Code

                sw.Reset();
                sw.Start();

                foreach (Entities.Widget widget in widgets)
                {
                    id = widget.ID;
                    name = widget.Name;
                }

                sw.Stop();
                AddDebugLine("Loop 1 - Compiled Code (took " + sw.ElapsedMilliseconds + " ms.)");

                #endregion

                #region Dynamic Method

                sw.Reset();
                sw.Start();

                Reflection.TypeUtility<Entities.Widget>.MemberGetDelegate<Int32> GetID = Reflection.TypeUtility<Entities.Widget>.GetMemberGetDelegate<Int32>("ID");
                Reflection.TypeUtility<Entities.Widget>.MemberGetDelegate<String> GetName = Reflection.TypeUtility<Entities.Widget>.GetMemberGetDelegate<String>("Name");
                foreach (Entities.Widget widget in widgets)
                {
                    id = GetID(widget);
                    name = GetName(widget);
                }

                sw.Stop();
                AddDebugLine("Loop 2 - Dynamic Method (took " + sw.ElapsedMilliseconds + " ms.)");

                #endregion

                #region Reflection

                sw.Reset();
                sw.Start();

                PropertyInfo id_pi = typeof(Entities.Widget).GetProperty("ID");
                PropertyInfo name_pi = typeof(Entities.Widget).GetProperty("Name");
                foreach (Entities.Widget widget in widgets)
                {
                    id = (Int32)id_pi.GetValue(widget, null);
                    name = (String)name_pi.GetValue(widget, null);
                }

                sw.Stop();
                AddDebugLine("Loop 3 - Reflection (took " + sw.ElapsedMilliseconds + " ms.)");

                #endregion

                #region Sort By Name

                sw.Reset();
                sw.Start();

                widgets.Sort(new MemberComparer<Entities.Widget>("ID"));

                AddDebugLine("Sort By Name (took " + sw.ElapsedMilliseconds + " ms.)");

                #endregion
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void AddDebugLine(string text)
        { txtDebug.Text += text + Environment.NewLine; }
    }
}