﻿/*
   Copyright 2015-2024 MCGalaxy

   Dual-licensed under the Educational Community License, Version 2.0 and
   the GNU General Public License, Version 3 (the "Licenses"); you may
   not use this file except in compliance with the Licenses. You may
   obtain a copy of the Licenses at

   https://opensource.org/license/ecl-2-0/
   https://www.gnu.org/licenses/gpl-3.0.html

   Unless required by applicable law or agreed to in writing,
   software distributed under the Licenses are distributed on an "AS IS"
   BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
   or implied. See the Licenses for the specific language governing
   permissions and limitations under the Licenses.
*/
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoldenSparks.Gui.Popups
{
    public partial class UpdateAvailable : Form
    {
        public static volatile bool Active;

        public UpdateAvailable()
        {
            Active = true;
            InitializeComponent();
            Text = "Update " + Colors.Strip(Server.SoftwareName) + "?";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                e.Graphics.DrawIcon(SystemIcons.Question, 25, 26);
            }
            catch
            {
                // not a critical error
            }
            base.OnPaint(e);
        }

        public void UpdateCheck_Closed(object sender, EventArgs e)
        {
            Active = false;
        }

        public void UpdateCheck_Load(object sender, EventArgs e)
        {
            GuiUtils.SetIcon(this);
        }

        public void BtnNo_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void BtnUpdate_Click(object sender, EventArgs e)
        {
            Updater.PerformUpdate();
            Close();
        }
    }
}