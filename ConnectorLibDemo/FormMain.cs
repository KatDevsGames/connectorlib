/*
 * Copyright 2018 Equilateral IT
 *
 * ConnectorLib is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * ConnectorLib is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with ConnectorLib.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Windows.Forms;
using ConnectorLib;
using JetBrains.Annotations;

namespace ConnectorLibDemo
{
    internal partial class FormMain : Form
    {
        private ISNESConnector _connector = null;

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public FormMain()
        {
            InitializeComponent();
            comboConnector.SelectedIndex = 0;
            comboLuaConnectorSocketType.SelectedIndex = 0;
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private void ProcessMessage([NotNull] string message)
        {
            string result = DateTime.UtcNow.ToString("[HH:mm:ss] ") + message;
            listStatus.Invoke(new Action(() => listStatus.Items.Insert(0, result)));
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private void buttonConnectorStartStop_Click([CanBeNull] object sender, [CanBeNull] EventArgs e)
        {
            if (buttonConnectorStartStop.Text?.EqualsInvariantIgnoreCase("Start") ?? false)
            {
                if (comboConnector.SelectedIndex == 1)
                {
                    buttonConnectorStartStop.Text = "Stop";
                    _connector = new sd2snesConnector(ProcessMessage);
                    buttonConnectorTest.Enabled = true;
                }
                else
                {
                    buttonConnectorStartStop.Text = "Stop";
                    comboLuaConnectorSocketType.Enabled = false;
                    IPAddress endpoint = (comboLuaConnectorSocketType.SelectedIndex == 0) ? IPAddress.Loopback : IPAddress.Any;
                    _connector = new LuaConnector(ProcessMessage, endpoint);
                    buttonConnectorTest.Enabled = true;
                }
            }
            else
            {
                buttonConnectorTest.Enabled = false;
                buttonConnectorStartStop.Text = "Start";
                comboLuaConnectorSocketType.Enabled = true;
                try { _connector?.Dispose(); }
                catch(Exception ex) { MessageBox.Show(this, ex.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error); }
                _connector = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

        }

        private void comboConnector_SelectedIndexChanged([CanBeNull] object sender, [CanBeNull] EventArgs e)
        {
            if (comboConnector?.SelectedItem == null) { return; }
            Settings.ConnectorInterface = (Settings.Connector.Interface)comboConnector.SelectedIndex;
        }

        private void comboLuaConnectorSocketType_SelectedIndexChanged([CanBeNull] object sender, [CanBeNull] EventArgs e)
        {
            if (comboLuaConnectorSocketType?.SelectedItem == null) { return; }
            Settings.ConnectorLuaSocketType = (Settings.Connector.LuaSocketType)comboLuaConnectorSocketType.SelectedIndex;
        }

        private void combosd2snesCOMPort_SelectedIndexChanged([CanBeNull] object sender, [CanBeNull] EventArgs e)
        {

        }

        private void buttonConnectorTest_Click([CanBeNull] object sender, [CanBeNull] EventArgs e)
        {
            if (_connector == null)
            {
                MessageBox.Show(this, "Game interface is not started.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SNESDiagnostic sd = new SNESDiagnostic(_connector);
            sd.Show();
        }
    }
}
