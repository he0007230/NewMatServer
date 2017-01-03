namespace SerTest
{
    partial class CFPlusTestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_ser = new System.Windows.Forms.Button();
            this.label_stat = new System.Windows.Forms.Label();
            this.button_ds = new System.Windows.Forms.Button();
            this.button_myobj = new System.Windows.Forms.Button();
            this.button_inher = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_ser
            // 
            this.button_ser.Location = new System.Drawing.Point(108, 12);
            this.button_ser.Name = "button_ser";
            this.button_ser.Size = new System.Drawing.Size(114, 23);
            this.button_ser.TabIndex = 0;
            this.button_ser.Text = "Serialize";
            this.button_ser.UseVisualStyleBackColor = true;
            this.button_ser.Click += new System.EventHandler(this.button_ser_Click);
            // 
            // label_stat
            // 
            this.label_stat.AutoSize = true;
            this.label_stat.Location = new System.Drawing.Point(59, 168);
            this.label_stat.Name = "label_stat";
            this.label_stat.Size = new System.Drawing.Size(43, 13);
            this.label_stat.TabIndex = 1;
            this.label_stat.Text = "Status :";
            // 
            // button_ds
            // 
            this.button_ds.Location = new System.Drawing.Point(108, 41);
            this.button_ds.Name = "button_ds";
            this.button_ds.Size = new System.Drawing.Size(114, 23);
            this.button_ds.TabIndex = 2;
            this.button_ds.Text = "Ser DataSet";
            this.button_ds.UseVisualStyleBackColor = true;
            this.button_ds.Click += new System.EventHandler(this.button_ds_Click);
            // 
            // button_myobj
            // 
            this.button_myobj.Location = new System.Drawing.Point(108, 71);
            this.button_myobj.Name = "button_myobj";
            this.button_myobj.Size = new System.Drawing.Size(114, 23);
            this.button_myobj.TabIndex = 3;
            this.button_myobj.Text = "Ser MyObject";
            this.button_myobj.UseVisualStyleBackColor = true;
            this.button_myobj.Click += new System.EventHandler(this.button_myobj_Click);
            // 
            // button_inher
            // 
            this.button_inher.Location = new System.Drawing.Point(108, 100);
            this.button_inher.Name = "button_inher";
            this.button_inher.Size = new System.Drawing.Size(114, 25);
            this.button_inher.TabIndex = 4;
            this.button_inher.Text = "Inheritance";
            this.button_inher.UseVisualStyleBackColor = true;
            this.button_inher.Click += new System.EventHandler(this.button_inher_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.button_inher);
            this.Controls.Add(this.button_myobj);
            this.Controls.Add(this.button_ds);
            this.Controls.Add(this.label_stat);
            this.Controls.Add(this.button_ser);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_ser;
        private System.Windows.Forms.Label label_stat;
        private System.Windows.Forms.Button button_ds;
        private System.Windows.Forms.Button button_myobj;
        private System.Windows.Forms.Button button_inher;
    }
}

