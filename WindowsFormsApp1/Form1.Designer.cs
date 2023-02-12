
namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.pacManComponent3 = new PacMan.PacManComponent();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.ForeColor = System.Drawing.SystemColors.Window;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 25;
            this.listBox1.Location = new System.Drawing.Point(433, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(396, 429);
            this.listBox1.TabIndex = 1;
            // 
            // pacManComponent3
            // 
            this.pacManComponent3.BackColor = System.Drawing.Color.Black;
            this.pacManComponent3.Location = new System.Drawing.Point(12, 12);
            this.pacManComponent3.MapPath = "";
            this.pacManComponent3.Name = "pacManComponent3";
            this.pacManComponent3.Size = new System.Drawing.Size(392, 434);
            this.pacManComponent3.TabIndex = 0;
            this.pacManComponent3.Text = "pacManComponent3";
            this.pacManComponent3.OnMatchLoose += new PacMan.PacManComponent.MethodContainer(this.pacManComponent3_OnMatchLoose);
            this.pacManComponent3.OnMatchWin += new PacMan.PacManComponent.MethodContainer(this.pacManComponent3_OnMatchWin);
            this.pacManComponent3.Click += new System.EventHandler(this.pacManComponent3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(1294, 688);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.pacManComponent3);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private PacMan.PacManComponent pacManComponent3;
        private System.Windows.Forms.ListBox listBox1;
    }
}

