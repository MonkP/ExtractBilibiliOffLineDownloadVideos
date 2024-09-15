namespace ExtractBilibiliOffLineDownloadVideos
{
	partial class Form1
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.btn_Exec = new System.Windows.Forms.Button();
            this.txt_SourceDir = new System.Windows.Forms.TextBox();
            this.btn_ChooseSourceDir = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_ChooseOutputDir = new System.Windows.Forms.Button();
            this.txt_OutputDir = new System.Windows.Forms.TextBox();
            this.txt_Output = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_ChooseFfmpeg = new System.Windows.Forms.Button();
            this.txt_ffmpegPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_Exec
            // 
            this.btn_Exec.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Exec.Location = new System.Drawing.Point(390, 131);
            this.btn_Exec.Margin = new System.Windows.Forms.Padding(2);
            this.btn_Exec.Name = "btn_Exec";
            this.btn_Exec.Size = new System.Drawing.Size(150, 30);
            this.btn_Exec.TabIndex = 0;
            this.btn_Exec.Text = "执行";
            this.btn_Exec.UseVisualStyleBackColor = true;
            this.btn_Exec.Click += new System.EventHandler(this.btn_Exec_Click);
            // 
            // txt_SourceDir
            // 
            this.txt_SourceDir.Location = new System.Drawing.Point(96, 10);
            this.txt_SourceDir.Margin = new System.Windows.Forms.Padding(2);
            this.txt_SourceDir.Name = "txt_SourceDir";
            this.txt_SourceDir.Size = new System.Drawing.Size(358, 21);
            this.txt_SourceDir.TabIndex = 1;
            // 
            // btn_ChooseSourceDir
            // 
            this.btn_ChooseSourceDir.Location = new System.Drawing.Point(467, 8);
            this.btn_ChooseSourceDir.Name = "btn_ChooseSourceDir";
            this.btn_ChooseSourceDir.Size = new System.Drawing.Size(75, 23);
            this.btn_ChooseSourceDir.TabIndex = 2;
            this.btn_ChooseSourceDir.Text = "选择";
            this.btn_ChooseSourceDir.UseVisualStyleBackColor = true;
            this.btn_ChooseSourceDir.Click += new System.EventHandler(this.btn_ChooseSourceDir_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "源目录";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "输出目录";
            // 
            // btn_ChooseOutputDir
            // 
            this.btn_ChooseOutputDir.Location = new System.Drawing.Point(467, 53);
            this.btn_ChooseOutputDir.Name = "btn_ChooseOutputDir";
            this.btn_ChooseOutputDir.Size = new System.Drawing.Size(75, 23);
            this.btn_ChooseOutputDir.TabIndex = 5;
            this.btn_ChooseOutputDir.Text = "选择";
            this.btn_ChooseOutputDir.UseVisualStyleBackColor = true;
            this.btn_ChooseOutputDir.Click += new System.EventHandler(this.btn_ChooseOutputDir_Click);
            // 
            // txt_OutputDir
            // 
            this.txt_OutputDir.Location = new System.Drawing.Point(96, 55);
            this.txt_OutputDir.Margin = new System.Windows.Forms.Padding(2);
            this.txt_OutputDir.Name = "txt_OutputDir";
            this.txt_OutputDir.Size = new System.Drawing.Size(358, 21);
            this.txt_OutputDir.TabIndex = 4;
            // 
            // txt_Output
            // 
            this.txt_Output.Location = new System.Drawing.Point(12, 176);
            this.txt_Output.Multiline = true;
            this.txt_Output.Name = "txt_Output";
            this.txt_Output.ReadOnly = true;
            this.txt_Output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Output.Size = new System.Drawing.Size(528, 193);
            this.txt_Output.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "ffmpeg路径";
            // 
            // btn_ChooseFfmpeg
            // 
            this.btn_ChooseFfmpeg.Location = new System.Drawing.Point(467, 103);
            this.btn_ChooseFfmpeg.Name = "btn_ChooseFfmpeg";
            this.btn_ChooseFfmpeg.Size = new System.Drawing.Size(75, 23);
            this.btn_ChooseFfmpeg.TabIndex = 9;
            this.btn_ChooseFfmpeg.Text = "选择";
            this.btn_ChooseFfmpeg.UseVisualStyleBackColor = true;
            this.btn_ChooseFfmpeg.Click += new System.EventHandler(this.btn_ChooseFfmpeg_Click);
            // 
            // txt_ffmpegPath
            // 
            this.txt_ffmpegPath.Location = new System.Drawing.Point(96, 105);
            this.txt_ffmpegPath.Margin = new System.Windows.Forms.Padding(2);
            this.txt_ffmpegPath.Name = "txt_ffmpegPath";
            this.txt_ffmpegPath.Size = new System.Drawing.Size(358, 21);
            this.txt_ffmpegPath.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 381);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_ChooseFfmpeg);
            this.Controls.Add(this.txt_ffmpegPath);
            this.Controls.Add(this.btn_Exec);
            this.Controls.Add(this.txt_Output);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_ChooseOutputDir);
            this.Controls.Add(this.txt_OutputDir);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_ChooseSourceDir);
            this.Controls.Add(this.txt_SourceDir);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "转换器";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btn_Exec;
		private System.Windows.Forms.TextBox txt_SourceDir;
        private System.Windows.Forms.Button btn_ChooseSourceDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_ChooseOutputDir;
        private System.Windows.Forms.TextBox txt_OutputDir;
        private System.Windows.Forms.TextBox txt_Output;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_ChooseFfmpeg;
        private System.Windows.Forms.TextBox txt_ffmpegPath;
    }
}

