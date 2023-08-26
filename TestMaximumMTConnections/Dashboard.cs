using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace TestMaximumMTConnections
{
    [SuppressMessage("ReSharper", "LocalizableElement")]
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (buttonStart.Text == "Start")
            {
                progressBar.Value = 0;
                buttonStart.Text = "Stop";
                flowLayoutPanelProgress.Controls.Clear();
                for (int i = 0; i < Configs.NumTerms; i++)
                {
                    var tb = new TextBox
                    {
                        Size = new Size(20, 20),
                        Enabled = false,
                        TabIndex = 0,
                        BackColor = Color.DarkGray
                    };
                    flowLayoutPanelProgress.Controls.Add(tb);
                }
                MTRunner.Instance.Start(() => {
                    buttonStart.Text = "Start";
                });
            }
            else
            {
                buttonStart.Text = "Start";
                MTRunner.Instance.Stop();
            }
        }
    }
}
