﻿/*    
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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
using GoldenSparks.UI;
namespace GoldenSparks.Gui
{
    public partial class Window : Form
    {
        public PlayerProperties PlayerProps;
        public void NoPlayerSelected()
        {
            Popup.Warning("No player selected");
        }
        public void Pl_BtnUndo_Click(object sender, EventArgs e)
        {
            if (CurPlayer == null)
            {
                NoPlayerSelected();
                return;
            }
            TimeSpan interval = Pl_numUndo.Value;
            if (interval.TotalSeconds == 0)
            {
                Players_AppendStatus("Amount of time to undo required");
                return;
            }
            // TODO ModerationActions instead of HandleCommand
            string duration = Pl_numUndo.Value.Shorten(true, false);
            UIHelpers.HandleCommand("UndoPlayer " + CurPlayer.name + " " + duration);
            Players_AppendStatus("Undid " + CurPlayer.truename + " for " + duration);
        }
        public void Pl_BtnMessage_Click(object sender, EventArgs e)
        {
            if (CurPlayer == null)
            {
                NoPlayerSelected();
                return;
            }
            string text = Pl_txtMessage.Text.Trim();
            if (text.Length == 0)
            {
                Players_AppendStatus("No message to send");
                return;
            }
            ChatModes.MessageDirect(Player.Sparks, CurPlayer, Pl_txtMessage.Text);
            Players_AppendStatus("Sent player message: " + Pl_txtMessage.Text);
            Pl_txtMessage.Text = "";
        }
        public void Pl_BtnSendCommand_Click(object sender, EventArgs e)
        {
            if (CurPlayer == null)
            {
                NoPlayerSelected();
                return;
            }
            string text = Pl_txtSendCommand.Text.Trim()
                                                .TrimStart('/');
            if (text.Length == 0)
            {
                Players_AppendStatus("No command to execute");
                return;
            }
            string[] args = text.SplitSpaces(2);
            string cmdName = args[0], cmdArgs = args.Length > 1 ? args[1] : "";
            CommandData data = default;
            data.Rank = LevelPermission.Sparkie;
            data.Context = CommandContext.SendCmd;
            CurPlayer.HandleCommand(cmdName, cmdArgs, data);
            if (args.Length > 1)
            {
                Players_AppendStatus("Made player do /" + cmdName + " " + cmdArgs);
            }
            else
            {
                Players_AppendStatus("Made player do /" + cmdName);
            }
            Pl_txtSendCommand.Text = "";
        }
        public void Pl_BtnMute_Click(object sender, EventArgs e)
        {
            if (CurPlayer != null && !CurPlayer.muted)
            {
                DoCmd("mute", "Muted @P");
            }
            else
            {
                DoCmd("unmute", "Unmuted @P");
            }
        }
        public void Pl_BtnFreeze_Click(object sender, EventArgs e)
        {
            if (CurPlayer != null && !CurPlayer.frozen)
            {
                DoCmd("freeze", "Froze @P", "10m");
            }
            else
            {
                DoCmd("freeze", "Unfroze @P");
            }
        }
        public void Pl_BtnWarn_Click(object sender, EventArgs e) 
        { 
            DoCmd("warn", "Warned @P"); 
        }
        public void Pl_BtnKick_Click(object sender, EventArgs e) 
        { 
            DoCmd("kick", "Kicked @P"); 
        }
        public void Pl_BtnBan_Click(object sender, EventArgs e) 
        { 
            DoCmd("ban", "Banned @P"); 
        }
        public void Pl_BtnIPBan_Click(object sender, EventArgs e) 
        { 
            DoCmd("banip", "IP-Banned @P"); 
        }
        public void Pl_BtnKill_Click(object sender, EventArgs e) 
        { 
            DoCmd("kill", "Killed @P"); 
        }
        public void Pl_BtnRules_Click(object sender, EventArgs e) 
        { 
            DoCmd("Rules", "Sent rules to @P"); 
        }
        public void DoCmd(string cmdName, string action, string suffix = null)
        {
            if (CurPlayer == null) 
            { 
                NoPlayerSelected(); 
                return; 
            }
            string cmd = (cmdName + " " + CurPlayer.name + " " + suffix).Trim();
            UIHelpers.HandleCommand(cmd);
            Players_AppendStatus(action.Replace("@P", CurPlayer.truename));
            Players_UpdateButtons();
        }
        public void Pl_listBox_Click(object sender, EventArgs e)
        {
            Player p = PlayerInfo.FindExact(Pl_listBox.Text);
            if (p == null || p == CurPlayer)
            {
                return;
            }
            Pl_statusBox.Text = "";
            Players_AppendStatus("==" + p.truename + "==");
            CurPlayer = p;
            Players_SetSelected(p.truename, new PlayerProperties(p));
            Players_UpdateSelected();
        }
        public void Pl_txtSendCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Pl_BtnSendCommand_Click(sender, e);
            }
        }
        public void Pl_numUndo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Pl_BtnUndo_Click(sender, e);
            }
        }
        public void Pl_txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            { 
                Pl_BtnMessage_Click(sender, e); 
            }
        }
        public void Players_AppendStatus(string text)
        {
            Pl_statusBox.AppendText(text + Environment.NewLine);
        }
        public void Players_UpdateSelected()
        {
            if (Tabs.SelectedTab != Tp_Players) 
            { 
                return; 
            }
            try 
            { 
                Pl_pgProps.Refresh(); 
            } 
            catch 
            { 
            }
        }
        public void Players_UpdateList()
        {
            Pl_listBox.Items.Clear();
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players)
            {
                Pl_listBox.Items.Add(p.truename);
            }
            if (CurPlayer == null)
            {
                return;
            }
            if (PlayerInfo.FindExact(CurPlayer.name) != null)
            {
                return;
            }
            CurPlayer = null;
            Players_SetSelected("(none selected)", null);
        }
        public void Players_SetSelected(string name, PlayerProperties props)
        {
            PlayerProps = props;
            Pl_gbProps.Text = "Properties for " + name;
            Pl_pgProps.SelectedObject = props;
            Players_UpdateButtons();
        }
        public void Players_UpdateButtons()
        {
            Player p = CurPlayer;
            Pl_btnMute.Text = p != null && p.muted ? "Unmute" : "Mute";
            Pl_btnFreeze.Text = p != null && p.frozen ? "Unfreeze" : "Freeze";
            // TODO: Automatically update when player is muted/frozen in-game
        }
    }
}