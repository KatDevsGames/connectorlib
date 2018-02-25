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
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectorLib;
using JetBrains.Annotations;

namespace ConnectorLibDemo
{
    internal partial class SNESDiagnostic : Form
    {
        [NotNull]
        private readonly ISNESConnector _connector;

        private SNESDiagnostic()
        {
            InitializeComponent();
        }

        public SNESDiagnostic([NotNull] ISNESConnector connector) : this() => _connector = connector;

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private void buttonMemoryRead_Click([CanBeNull] object sender, [CanBeNull] EventArgs e)
        {
            //buttonMemoryRead.Enabled = false;
            Task.Run(() =>
            {
                uint address = 0;
                numericAddress.Invoke(new Action(() => { address = (uint)numericAddress.Value; }));
                byte result = _connector.ReadByte(address) ?? 0;
                numericValue.Invoke(new Action(() => { numericValue.Value = result; }));
                buttonMemoryRead.Invoke(new Action(() => { buttonMemoryRead.Enabled = true; }));
            });
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private void buttonMemoryWrite_Click([CanBeNull] object sender, [CanBeNull] EventArgs e)
        {
            _connector.WriteByte((uint)numericAddress.Value, (byte) numericValue.Value);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private void buttonMessageSend_Click([CanBeNull] object sender, [CanBeNull] EventArgs e)
        {
            _connector.SendMessage(textMessage.Text);
        }
    }
}
