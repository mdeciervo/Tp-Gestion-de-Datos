namespace FrbaHotel.ABM_de_Usuario
{
    partial class HomeUsuario
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
            this.ALTA = new System.Windows.Forms.Button();
            this.LISTADO = new System.Windows.Forms.Button();
            this.MODIFICACION = new System.Windows.Forms.Button();
            this.BAJA = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ALTA
            // 
            this.ALTA.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ALTA.Location = new System.Drawing.Point(107, 40);
            this.ALTA.Name = "ALTA";
            this.ALTA.Size = new System.Drawing.Size(200, 39);
            this.ALTA.TabIndex = 0;
            this.ALTA.Text = "ALTA";
            this.ALTA.UseVisualStyleBackColor = true;
            this.ALTA.Click += new System.EventHandler(this.ALTA_Click);
            // 
            // LISTADO
            // 
            this.LISTADO.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold);
            this.LISTADO.Location = new System.Drawing.Point(107, 103);
            this.LISTADO.Name = "LISTADO";
            this.LISTADO.Size = new System.Drawing.Size(200, 39);
            this.LISTADO.TabIndex = 1;
            this.LISTADO.Text = "LISTADO";
            this.LISTADO.UseVisualStyleBackColor = true;
            this.LISTADO.Click += new System.EventHandler(this.LISTADO_Click);
            // 
            // MODIFICACION
            // 
            this.MODIFICACION.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold);
            this.MODIFICACION.Location = new System.Drawing.Point(87, 160);
            this.MODIFICACION.Name = "MODIFICACION";
            this.MODIFICACION.Size = new System.Drawing.Size(244, 39);
            this.MODIFICACION.TabIndex = 2;
            this.MODIFICACION.Text = "MODIFICACION";
            this.MODIFICACION.UseVisualStyleBackColor = true;
            this.MODIFICACION.Click += new System.EventHandler(this.MODIFICACION_Click);
            // 
            // BAJA
            // 
            this.BAJA.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold);
            this.BAJA.Location = new System.Drawing.Point(107, 220);
            this.BAJA.Name = "BAJA";
            this.BAJA.Size = new System.Drawing.Size(200, 39);
            this.BAJA.TabIndex = 3;
            this.BAJA.Text = "BAJA";
            this.BAJA.UseVisualStyleBackColor = true;
            this.BAJA.Click += new System.EventHandler(this.BAJA_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold);
            this.button3.Location = new System.Drawing.Point(87, 284);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(244, 39);
            this.button3.TabIndex = 4;
            this.button3.Text = "VOLVER ATRAS";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // HomeUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 375);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.BAJA);
            this.Controls.Add(this.MODIFICACION);
            this.Controls.Add(this.LISTADO);
            this.Controls.Add(this.ALTA);
            this.Name = "HomeUsuario";
            this.Text = "ABM de Usuario";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HomeUsuario_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ALTA;
        private System.Windows.Forms.Button LISTADO;
        private System.Windows.Forms.Button MODIFICACION;
        private System.Windows.Forms.Button BAJA;
        private System.Windows.Forms.Button button3;
    }
}