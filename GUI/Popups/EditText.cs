/*
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
using System.Windows.Forms;
using GoldenSparks.Util;

namespace GoldenSparks.Gui.Popups
{
    public partial class EditText : Form
    {
        public TextFile curFile;

        public EditText()
        {
            InitializeComponent();
            foreach (var kvp in TextFile.Files)
            {
                CmbList.Items.Add(kvp.Key);
            }
            CmbList.Text = "Select file..";
        }

        public void EditText_Load(object sender, EventArgs e)
        {
            GuiUtils.SetIcon(this);
        }

        public void cmbList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbList.SelectedIndex == -1) return;
            TrySaveChanges();

            string selectedName = CmbList.SelectedItem.ToString();
            curFile = TextFile.Files[selectedName];

            try
            {
                curFile.EnsureExists();
                TxtEdit.Lines = curFile.GetText();
                Text = "Editing " + curFile.Filename;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Popup.Error("Failed to read text from " + curFile.Filename);

                curFile = null;
                CmbList.Text = "";
                Text = "Editing (none)";
            }
        }

        public void SaveChanges(string[] lines)
        {
            curFile.SetText(lines);
            Popup.Message("Saved " + curFile.Filename);
        }

        public void TrySaveChanges()
        {
            if (curFile == null) return;
            string[] lines = TxtEdit.Lines;
            if (!HasChanged(lines)) return;

            if (Popup.YesNo("Save changes to " + curFile.Filename + "?", "Save changes"))
            {
                SaveChanges(lines);
            }
        }

        public bool HasChanged(string[] lines)
        {
            string[] curLines = curFile.GetText();
            if (lines.Length != curLines.Length) return true;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != curLines[i]) return true;
            }
            return false;
        }


        public void btnColor_Click(object sender, EventArgs e)
        {
            using (ColorSelector sel = new ColorSelector("Insert color", '\0'))
            {
                DialogResult result = sel.ShowDialog();
                if (result == DialogResult.Cancel) return;
                InsertText("&" + sel.ColorCode);
            }
        }

        public void btnToken_Click(object sender, EventArgs e)
        {
            using (TokenSelector sel = new TokenSelector("Insert token"))
            {
                DialogResult result = sel.ShowDialog();
                if (result == DialogResult.Cancel || sel.Token == null) return;
                InsertText(sel.Token);
            }
        }

        public void InsertText(string text)
        {
            int selStart = TxtEdit.SelectionStart, selLength = TxtEdit.SelectionLength;
            TxtEdit.Paste(text);
            // re highlight now replaced text
            if (selLength > 0) TxtEdit.Select(selStart, text.Length);
            TxtEdit.Focus();
        }

        public void EditTxt_Unload(object sender, EventArgs e)
        {
            TrySaveChanges();
        }

        public void btnSave_Click(object sender, EventArgs e)
        {
            if (curFile == null) return;
            SaveChanges(TxtEdit.Lines);
        }
    }
}