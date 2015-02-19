namespace FrbaHotel.Login
{
    partial class IntermediaCliente
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
            this.cboFunCli = new System.Windows.Forms.ComboBox();
            this.Continuar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cboFunCli
            // 
            this.cboFunCli.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboFunCli.FormattingEnabled = true;
            this.cboFunCli.Items.AddRange(new object[] {
            "Generar o Modificar Reserva",
            "Cancelar Reserva"});
            this.cboFunCli.Location = new System.Drawing.Point(57, 139);
            this.cboFunCli.Name = "cboFunCli";
            this.cboFunCli.Size = new System.Drawing.Size(290, 28);
            this.cboFunCli.TabIndex = 0;
            this.cboFunCli.Text = "Elegir...";
            this.cboFunCli.SelectedIndexChanged += new System.EventHandler(this.cboFunCli_SelectedIndexChanged);
            // 
            // Continuar
            // 
            this.Continuar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Continuar.Location = new System.Drawing.Point(150, 186);
            this.Continuar.Name = "Continuar";
            this.Continuar.Size = new System.Drawing.Size(121, 40);
            this.Continuar.TabIndex = 1;
            this.Continuar.Text = "Continuar";
            this.Continuar.UseVisualStyleBackColor = true;
            this.Continuar.Click += new System.EventHandler(this.Continuar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label1.Location = new System.Drawing.Point(75, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(272, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "Seleccione una funcionalidad";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label2.Location = new System.Drawing.Point(93, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(224, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "para ingresar al sistema";
            // 
            // IntermediaCliente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 268);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Continuar);
            this.Controls.Add(this.cboFunCli);
            this.Name = "IntermediaCliente";
            this.Text = "IntermediaCliente";
            this.Load += new System.EventHandler(this.IntermediaCliente_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IntermediaCliente_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboFunCli;
        private System.Windows.Forms.Button Continuar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;

    }
}