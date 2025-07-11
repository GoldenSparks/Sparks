﻿// Part of fCraft | Copyright 2009-2015 Matvei Stefarov <me@matvei.org> | BSD-3 | See LICENSE.txt
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoldenSparks.Gui.Popups
{
    public partial class TokenSelector : Form
    {
        public string Token;

        public TokenSelector(string title)
        {
            InitializeComponent();
            Text = title;
            SuspendLayout();

            foreach (ChatToken token in ChatTokens.Standard)
            {
                MakeButton(token);
            }
            foreach (ChatToken token in ChatTokens.Custom)
            {
                MakeButton(token);
            }

            UpdateBaseLayout();
            ResumeLayout(false);
        }

        public void TokenSelector_Load(object sender, EventArgs e)
        {
            GuiUtils.SetIcon(this);
        }


        public const int btnWidth = 110, btnHeight = 40, btnsPerCol = 9;
        public int index = 0;
        public void MakeButton(ChatToken token)
        {
            int row = index / btnsPerCol, col = index % btnsPerCol;
            index++;

            Button btn = new Button
            {
                Location = new Point(9 + row * btnWidth, 7 + col * btnHeight),
                Size = new Size(btnWidth, btnHeight),
                Name = "b" + index,
                TabIndex = index
            };
            TokenToolTip.SetToolTip(btn, token.Description);

            btn.Text = token.Trigger;
            btn.Click += delegate 
            { 
                Token = token.Trigger; 
                DialogResult = DialogResult.OK; 
                Close(); 
            };
            btn.Margin = new Padding(0);
            btn.UseMnemonic = false;
            btn.UseVisualStyleBackColor = false;
            btn.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Controls.Add(btn);
        }


        public void UpdateBaseLayout()
        {
            int rows = index / btnsPerCol;
            if ((index % btnsPerCol) != 0) rows++; // round up

            int x = 0;
            // Centre if even count, align under row if odd count
            if ((rows & 1) == 0)
            {
                x = rows * btnWidth / 2 - (100 / 2);
            }
            else
            {
                x = (rows / 2 * btnWidth) + (btnWidth - 100) / 2;
            }

            BtnCancel.Location = new Point(8 + x, 12 + btnHeight * btnsPerCol);
            ClientSize = new Size(18 + btnWidth * rows, 47 + btnHeight * btnsPerCol);
        }
    }
}