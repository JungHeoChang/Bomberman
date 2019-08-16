namespace gameprog_v2
{
    partial class main
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(main));
            this.stage = new System.Windows.Forms.PictureBox();
            this.pvp = new System.Windows.Forms.PictureBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.time = new System.Windows.Forms.RichTextBox();
            this.player1_score = new System.Windows.Forms.RichTextBox();
            this.picture1p = new System.Windows.Forms.PictureBox();
            this.back_button = new System.Windows.Forms.Button();
            this.next_button = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.메뉴ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.게임방법ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.stage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pvp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture1p)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // stage
            // 
            this.stage.Image = ((System.Drawing.Image)(resources.GetObject("stage.Image")));
            this.stage.Location = new System.Drawing.Point(80, 116);
            this.stage.Name = "stage";
            this.stage.Size = new System.Drawing.Size(402, 104);
            this.stage.TabIndex = 0;
            this.stage.TabStop = false;
            this.stage.Click += new System.EventHandler(this.stage_Click);
            // 
            // pvp
            // 
            this.pvp.Image = ((System.Drawing.Image)(resources.GetObject("pvp.Image")));
            this.pvp.Location = new System.Drawing.Point(80, 279);
            this.pvp.Name = "pvp";
            this.pvp.Size = new System.Drawing.Size(402, 104);
            this.pvp.TabIndex = 1;
            this.pvp.TabStop = false;
            this.pvp.Click += new System.EventHandler(this.pvp_Click);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // time
            // 
            this.time.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.time.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.time.Location = new System.Drawing.Point(241, 12);
            this.time.Name = "time";
            this.time.ReadOnly = true;
            this.time.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.time.Size = new System.Drawing.Size(100, 30);
            this.time.TabIndex = 3;
            this.time.TabStop = false;
            this.time.Text = "";
            // 
            // player1_score
            // 
            this.player1_score.Enabled = false;
            this.player1_score.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.player1_score.Location = new System.Drawing.Point(80, 12);
            this.player1_score.Multiline = false;
            this.player1_score.Name = "player1_score";
            this.player1_score.ReadOnly = true;
            this.player1_score.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.player1_score.Size = new System.Drawing.Size(131, 30);
            this.player1_score.TabIndex = 4;
            this.player1_score.Text = "";
            // 
            // picture1p
            // 
            this.picture1p.BackColor = System.Drawing.Color.White;
            this.picture1p.Location = new System.Drawing.Point(14, 12);
            this.picture1p.Name = "picture1p";
            this.picture1p.Size = new System.Drawing.Size(38, 27);
            this.picture1p.TabIndex = 5;
            this.picture1p.TabStop = false;
            // 
            // back_button
            // 
            this.back_button.Location = new System.Drawing.Point(845, 279);
            this.back_button.Name = "back_button";
            this.back_button.Size = new System.Drawing.Size(87, 30);
            this.back_button.TabIndex = 6;
            this.back_button.Text = "돌아가기";
            this.back_button.UseVisualStyleBackColor = true;
            this.back_button.Click += new System.EventHandler(this.back_button_Click);
            // 
            // next_button
            // 
            this.next_button.Location = new System.Drawing.Point(691, 279);
            this.next_button.Name = "next_button";
            this.next_button.Size = new System.Drawing.Size(97, 30);
            this.next_button.TabIndex = 7;
            this.next_button.Text = "다음 스테이지";
            this.next_button.UseVisualStyleBackColor = true;
            this.next_button.Click += new System.EventHandler(this.next_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.메뉴ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1085, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 메뉴ToolStripMenuItem
            // 
            this.메뉴ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.게임방법ToolStripMenuItem});
            this.메뉴ToolStripMenuItem.Name = "메뉴ToolStripMenuItem";
            this.메뉴ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.메뉴ToolStripMenuItem.Text = "메뉴";
            // 
            // 게임방법ToolStripMenuItem
            // 
            this.게임방법ToolStripMenuItem.Name = "게임방법ToolStripMenuItem";
            this.게임방법ToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.게임방법ToolStripMenuItem.Text = "게임 방법";
            this.게임방법ToolStripMenuItem.Click += new System.EventHandler(this.게임방법ToolStripMenuItem_Click);
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1085, 481);
            this.Controls.Add(this.next_button);
            this.Controls.Add(this.back_button);
            this.Controls.Add(this.picture1p);
            this.Controls.Add(this.player1_score);
            this.Controls.Add(this.time);
            this.Controls.Add(this.pvp);
            this.Controls.Add(this.stage);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "main";
            this.Text = "봄버맨";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.main_FormClosing);
            this.Load += new System.EventHandler(this.main_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.main_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.stage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pvp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture1p)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox stage;
        private System.Windows.Forms.PictureBox pvp;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.RichTextBox time;
        private System.Windows.Forms.RichTextBox player1_score;
        private System.Windows.Forms.PictureBox picture1p;
        private System.Windows.Forms.Button back_button;
        private System.Windows.Forms.Button next_button;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 메뉴ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 게임방법ToolStripMenuItem;
    }
}

