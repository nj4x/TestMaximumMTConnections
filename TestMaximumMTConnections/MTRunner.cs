using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using nj4x;
using nj4x.Metatrader;
using Color = System.Drawing.Color;

namespace TestMaximumMTConnections
{
    [SuppressMessage("ReSharper", "LocalizableElement")]
    public class MTRunner
    {
        public static MTRunner Instance = new MTRunner();
        private bool _isRunning;
        // ReSharper disable once NotAccessedField.Local
        private Task _task;
        private readonly List<Strategy> _terminals = new List<Strategy>();

        private MTRunner()
        {
        }

        public void Stop()
        {
            DisconnectAll();
            //
            Configs.Gui.flowLayoutPanelProgress.Controls.Clear();
            Configs.Gui.textBoxProgress.Text = "Connection process has been terminated...";
            Configs.Gui.progressBar.Value = 0;
            //
            _isRunning = false;
        }

        private void DisconnectAll()
        {
// close terms left from prev run
            foreach (var t in _terminals)
            {
                t.Disconnect(true);
            }
            //
            _terminals.Clear();
        }

        public void Start(Action onFinished)
        {
            if (_terminals.Count > 0) DisconnectAll();
            //
            _isRunning = true;
            Configs.Gui.textBoxProgress.Text = "Starting...";
            _task = Task.Run(() =>
            {
                var numTerms = Configs.NumTerms;
                var progressBar = Configs.Gui.progressBar;
                var fPanel = Configs.Gui.flowLayoutPanelProgress;
                var txtProgress = Configs.Gui.textBoxProgress;
                progressBar.Invoke((MethodInvoker)delegate
                {
                    progressBar.Minimum = 0;
                    progressBar.Maximum = numTerms;
                    progressBar.Step = 1;
                });
                List<string> symbols = null;
                for (int i = 0; i < numTerms && _isRunning; i++)
                {
                    var connControl = fPanel.Controls[i];
                    var s = new Strategy() {IsReconnect = false};
                    _terminals.Add(s);
                    for (int retry = 0; retry < 3 && _isRunning; retry++)
                    {
                        try
                        {
                            txtProgress.Invoke((MethodInvoker)delegate
                            {
                                connControl.BackColor = Color.Yellow;
                                txtProgress.Text = $"Connecting terminal #{i + 1} (retry {retry + 1})";
                            });
                            //
                            s.Connect(Configs.TsHost, Configs.TsPort,
                                new Broker(Configs.IsMT5 ? $"5*{Configs.Broker}" : Configs.Broker),
                                $"{Configs.Account}@Term No {(i + 1):D3}",
                                // "<accNo>@<dirSuffix>" format instructs TS to create separate directory for each terminal
                                Configs.Password);
                            connControl.Invoke((MethodInvoker)delegate
                            {
                                connControl.BackColor = Color.Blue;
                            });
                            symbols = symbols ?? s.Symbols;
                            foreach (var symbol in symbols)
                            {
                                s.SymbolSelect(symbol, false); // hide from market watch to minimize traffic/CPU usage
                            }
                            //
                            txtProgress.Invoke((MethodInvoker)delegate
                            {
                                connControl.BackColor = Color.Green;
                                txtProgress.Text = $"Terminal #{i + 1} has been connected.";
                            });
                            break;
                        }
                        catch (Exception)
                        {
                            connControl.Invoke((MethodInvoker)delegate
                            {
                                connControl.BackColor = Color.Red;
                            });
                        }
                    }
                    //
                    progressBar.Invoke((MethodInvoker)delegate
                    {
                        progressBar.PerformStep();
                    });
                }
                //
                if (_isRunning)
                {
                    txtProgress.Invoke((MethodInvoker)delegate
                    {
                        txtProgress.Text = "All done!";
                    });
                    _isRunning = false;
                    onFinished?.Invoke();
                }
            });
        }
    }
}