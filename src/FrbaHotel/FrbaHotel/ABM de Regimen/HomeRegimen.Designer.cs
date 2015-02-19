namespace FrbaHotel.Regimen
{
    partial class HomeRegimen
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
            this.buttonAlta = new System.Windows.Forms.Button();
            this.buttonModi = new System.Windows.Forms.Button();
            this.buttonBaja = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonAlta
            // 
            this.buttonAlta.Font = new System.Drawing.Font("Arial Black", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAlta.Location = new System.Drawing.Point(35, 39);
            this.buttonAlta.Name = "buttonAlta";
            this.buttonAlta.Size = new System.Drawing.Size(224, 64);
            this.buttonAlta.TabIndex = 0;
            this.buttonAlta.Text = "Alta";
            this.buttonAlta.UseVisualStyleBackColor = true;
            this.buttonAlta.Click += new System.EventHandler(this.buttonAlta_Click);
            // 
            // buttonModi
            // 
            this.buttonModi.Font = new System.Drawing.Font("Arial Black", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonModi.Location = new System.Drawing.Point(35, 227);
            this.buttonModi.Name = "buttonModi";
            this.buttonModi.Size = new System.Drawing.Size(224, 64);
            this.buttonModi.TabIndex = 1;
            this.buttonModi.Text = "Modificación";
            this.buttonModi.UseVisualStyleBackColor = true;
            this.buttonModi.Click += new System.EventHandler(this.buttonModi_Click);
            // 
            // buttonBaja
            // 
            this.buttonBaja.Font = new System.Drawing.Font("Arial Black", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBaja.Location = new System.Drawing.Point(35, 133);
            this.buttonBaja.Name = "buttonBaja";
            this.buttonBaja.Size = new System.Drawing.Size(224, 64);
            this.buttonBaja.TabIndex = 2;
            this.buttonBaja.Text = "Baja";
            this.buttonBaja.UseVisualStyleBackColor = true;
            this.buttonBaja.Click += new System.EventHandler(this.buttonBaja_Click);
            // 
            // HomeRegimen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 334);
            this.Controls.Add(this.buttonBaja);
            this.Controls.Add(this.buttonModi);
            this.Controls.Add(this.buttonAlta);
            this.Name = "HomeRegimen";
            this.Text = "Opciones";
            this.Load += new System.EventHandler(this.Home_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HomeRegimen_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAlta;
        private System.Windows.Forms.Button buttonModi;
        private System.Windows.Forms.Button buttonBaja;
    }
}