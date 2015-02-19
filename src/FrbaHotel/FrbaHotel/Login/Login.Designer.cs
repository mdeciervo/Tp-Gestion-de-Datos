namespace FrbaHotel.Login
{
    partial class HomeLogin
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
            this.Cancelar = new System.Windows.Forms.Button();
            this.Aceptar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.usuario = new System.Windows.Forms.TextBox();
            this.clave = new System.Windows.Forms.TextBox();
            this.Guest = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.SuspendLayout();
            // 
            // Cancelar
            // 
            this.Cancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.Cancelar.Location = new System.Drawing.Point(355, 248);
            this.Cancelar.Name = "Cancelar";
            this.Cancelar.Size = new System.Drawing.Size(133, 34);
            this.Cancelar.TabIndex = 2;
            this.Cancelar.Text = "CANCELAR";
            this.Cancelar.UseVisualStyleBackColor = true;
            this.Cancelar.Click += new System.EventHandler(this.Cancelar_Click);
            // 
            // Aceptar
            // 
            this.Aceptar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.Aceptar.Location = new System.Drawing.Point(540, 248);
            this.Aceptar.Name = "Aceptar";
            this.Aceptar.Size = new System.Drawing.Size(133, 34);
            this.Aceptar.TabIndex = 0;
            this.Aceptar.Text = "ACEPTAR";
            this.Aceptar.UseVisualStyleBackColor = true;
            this.Aceptar.Click += new System.EventHandler(this.Aceptar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Navy;
            this.label1.Location = new System.Drawing.Point(390, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "USUARIO:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Navy;
            this.label2.Location = new System.Drawing.Point(340, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(147, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "CONTRASEÑA:";
            // 
            // usuario
            // 
            this.usuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.usuario.Location = new System.Drawing.Point(503, 65);
            this.usuario.Name = "usuario";
            this.usuario.Size = new System.Drawing.Size(154, 29);
            this.usuario.TabIndex = 5;
            // 
            // clave
            // 
            this.clave.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.clave.Location = new System.Drawing.Point(503, 128);
            this.clave.Name = "clave";
            this.clave.Size = new System.Drawing.Size(154, 29);
            this.clave.TabIndex = 6;
            this.clave.UseSystemPasswordChar = true;
            this.clave.TextChanged += new System.EventHandler(this.clave_TextChanged);
            // 
            // Guest
            // 
            this.Guest.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.Guest.Location = new System.Drawing.Point(90, 248);
            this.Guest.Name = "Guest";
            this.Guest.Size = new System.Drawing.Size(133, 34);
            this.Guest.TabIndex = 1;
            this.Guest.Text = "GUEST";
            this.Guest.UseVisualStyleBackColor = true;
            this.Guest.Click += new System.EventHandler(this.GUEST_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Lucida Console", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Navy;
            this.label4.Location = new System.Drawing.Point(40, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(249, 38);
            this.label4.TabIndex = 2;
            this.label4.Text = "Si es un Cliente,\r\nPor favor click aqui";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(334, 349);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // HomeLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 349);
            this.Controls.Add(this.Guest);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.clave);
            this.Controls.Add(this.usuario);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Aceptar);
            this.Controls.Add(this.Cancelar);
            this.Name = "HomeLogin";
            this.Text = "LOGIN";
            this.Load += new System.EventHandler(this.HomeLogin_Load);
            this.Shown += new System.EventHandler(this.HomeLogin_Shown);
            this.Activated += new System.EventHandler(this.HomeLogin_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HomeLogin_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancelar;
        private System.Windows.Forms.Button Aceptar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox usuario;
        private System.Windows.Forms.TextBox clave;
        private System.Windows.Forms.Button Guest;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Splitter splitter1;

    }
}