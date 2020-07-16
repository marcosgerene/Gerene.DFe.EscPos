namespace Gerene.DFe.EscPos.Demo
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.BtnCarregar = new System.Windows.Forms.Button();
            this.TextArquivoXml = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnListarImpressoras = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.ComboModeloDFe = new System.Windows.Forms.ComboBox();
            this.ComboTipoImpressora = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.BtnImprimir = new System.Windows.Forms.Button();
            this.ComboImpressoras = new System.Windows.Forms.ComboBox();
            this.ChbCortar = new System.Windows.Forms.CheckBox();
            this.ChbDuasLinhas = new System.Windows.Forms.CheckBox();
            this.ChbBarrasCodigo = new System.Windows.Forms.CheckBox();
            this.ChbDocumentoCancelado = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 20);
            this.label1.TabIndex = 14;
            this.label1.Text = "Arquivo Xml";
            // 
            // BtnCarregar
            // 
            this.BtnCarregar.FlatAppearance.BorderSize = 0;
            this.BtnCarregar.Location = new System.Drawing.Point(492, 36);
            this.BtnCarregar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnCarregar.Name = "BtnCarregar";
            this.BtnCarregar.Size = new System.Drawing.Size(49, 30);
            this.BtnCarregar.TabIndex = 13;
            this.BtnCarregar.Text = "...";
            this.BtnCarregar.UseVisualStyleBackColor = true;
            this.BtnCarregar.Click += new System.EventHandler(this.BtnCarregar_Click);
            // 
            // TextArquivoXml
            // 
            this.TextArquivoXml.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextArquivoXml.Location = new System.Drawing.Point(16, 37);
            this.TextArquivoXml.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextArquivoXml.Name = "TextArquivoXml";
            this.TextArquivoXml.Size = new System.Drawing.Size(467, 26);
            this.TextArquivoXml.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 75);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 20);
            this.label2.TabIndex = 16;
            this.label2.Text = "Impressora";
            // 
            // BtnListarImpressoras
            // 
            this.BtnListarImpressoras.FlatAppearance.BorderSize = 0;
            this.BtnListarImpressoras.Location = new System.Drawing.Point(492, 101);
            this.BtnListarImpressoras.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnListarImpressoras.Name = "BtnListarImpressoras";
            this.BtnListarImpressoras.Size = new System.Drawing.Size(49, 33);
            this.BtnListarImpressoras.TabIndex = 17;
            this.BtnListarImpressoras.Text = "...";
            this.BtnListarImpressoras.UseVisualStyleBackColor = true;
            this.BtnListarImpressoras.Click += new System.EventHandler(this.BtnListarImpressoras_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 144);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 20);
            this.label3.TabIndex = 18;
            this.label3.Text = "Modelo DFe";
            // 
            // ComboModeloDFe
            // 
            this.ComboModeloDFe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboModeloDFe.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboModeloDFe.FormattingEnabled = true;
            this.ComboModeloDFe.Location = new System.Drawing.Point(16, 170);
            this.ComboModeloDFe.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ComboModeloDFe.Name = "ComboModeloDFe";
            this.ComboModeloDFe.Size = new System.Drawing.Size(173, 26);
            this.ComboModeloDFe.TabIndex = 19;
            // 
            // ComboTipoImpressora
            // 
            this.ComboTipoImpressora.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboTipoImpressora.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboTipoImpressora.FormattingEnabled = true;
            this.ComboTipoImpressora.Location = new System.Drawing.Point(205, 170);
            this.ComboTipoImpressora.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ComboTipoImpressora.Name = "ComboTipoImpressora";
            this.ComboTipoImpressora.Size = new System.Drawing.Size(163, 26);
            this.ComboTipoImpressora.TabIndex = 21;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(201, 144);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(153, 20);
            this.label4.TabIndex = 20;
            this.label4.Text = "Tipo de Impressora";
            // 
            // BtnImprimir
            // 
            this.BtnImprimir.FlatAppearance.BorderSize = 0;
            this.BtnImprimir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnImprimir.Location = new System.Drawing.Point(384, 144);
            this.BtnImprimir.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnImprimir.Name = "BtnImprimir";
            this.BtnImprimir.Size = new System.Drawing.Size(157, 54);
            this.BtnImprimir.TabIndex = 22;
            this.BtnImprimir.Text = "Imprimir";
            this.BtnImprimir.UseVisualStyleBackColor = true;
            this.BtnImprimir.Click += new System.EventHandler(this.BtnImprimir_Click);
            // 
            // ComboImpressoras
            // 
            this.ComboImpressoras.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboImpressoras.FormattingEnabled = true;
            this.ComboImpressoras.Location = new System.Drawing.Point(16, 103);
            this.ComboImpressoras.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ComboImpressoras.Name = "ComboImpressoras";
            this.ComboImpressoras.Size = new System.Drawing.Size(467, 28);
            this.ComboImpressoras.TabIndex = 23;
            // 
            // ChbCortar
            // 
            this.ChbCortar.AutoSize = true;
            this.ChbCortar.Checked = true;
            this.ChbCortar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChbCortar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChbCortar.Location = new System.Drawing.Point(16, 206);
            this.ChbCortar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ChbCortar.Name = "ChbCortar";
            this.ChbCortar.Size = new System.Drawing.Size(152, 24);
            this.ChbCortar.TabIndex = 24;
            this.ChbCortar.Text = "Cortar ao final";
            this.ChbCortar.UseVisualStyleBackColor = true;
            // 
            // ChbDuasLinhas
            // 
            this.ChbDuasLinhas.AutoSize = true;
            this.ChbDuasLinhas.Checked = true;
            this.ChbDuasLinhas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChbDuasLinhas.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChbDuasLinhas.Location = new System.Drawing.Point(16, 236);
            this.ChbDuasLinhas.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ChbDuasLinhas.Name = "ChbDuasLinhas";
            this.ChbDuasLinhas.Size = new System.Drawing.Size(229, 24);
            this.ChbDuasLinhas.TabIndex = 25;
            this.ChbDuasLinhas.Text = "Produto em duas linhas";
            this.ChbDuasLinhas.UseVisualStyleBackColor = true;
            // 
            // ChbBarrasCodigo
            // 
            this.ChbBarrasCodigo.AutoSize = true;
            this.ChbBarrasCodigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChbBarrasCodigo.Location = new System.Drawing.Point(16, 263);
            this.ChbBarrasCodigo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ChbBarrasCodigo.Name = "ChbBarrasCodigo";
            this.ChbBarrasCodigo.Size = new System.Drawing.Size(200, 24);
            this.ChbBarrasCodigo.TabIndex = 26;
            this.ChbBarrasCodigo.Text = "Barras como código";
            this.ChbBarrasCodigo.UseVisualStyleBackColor = true;
            // 
            // ChbDocumentoCancelado
            // 
            this.ChbDocumentoCancelado.AutoSize = true;
            this.ChbDocumentoCancelado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChbDocumentoCancelado.Location = new System.Drawing.Point(16, 290);
            this.ChbDocumentoCancelado.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ChbDocumentoCancelado.Name = "ChbDocumentoCancelado";
            this.ChbDocumentoCancelado.Size = new System.Drawing.Size(217, 24);
            this.ChbDocumentoCancelado.TabIndex = 27;
            this.ChbDocumentoCancelado.Text = "Documento cancelado";
            this.ChbDocumentoCancelado.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(276, 226);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(265, 131);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 28;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(276, 366);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 29;
            this.button1.Text = "Carregar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(381, 366);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 28);
            this.button2.TabIndex = 30;
            this.button2.Text = "Limpar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(272, 201);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 20);
            this.label5.TabIndex = 31;
            this.label5.Text = "Logotipo";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 401);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ChbDocumentoCancelado);
            this.Controls.Add(this.ChbBarrasCodigo);
            this.Controls.Add(this.ChbDuasLinhas);
            this.Controls.Add(this.ChbCortar);
            this.Controls.Add(this.ComboImpressoras);
            this.Controls.Add(this.BtnImprimir);
            this.Controls.Add(this.ComboTipoImpressora);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ComboModeloDFe);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BtnListarImpressoras);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnCarregar);
            this.Controls.Add(this.TextArquivoXml);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnCarregar;
        private System.Windows.Forms.TextBox TextArquivoXml;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtnListarImpressoras;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ComboModeloDFe;
        private System.Windows.Forms.ComboBox ComboTipoImpressora;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button BtnImprimir;
        private System.Windows.Forms.ComboBox ComboImpressoras;
        private System.Windows.Forms.CheckBox ChbCortar;
        private System.Windows.Forms.CheckBox ChbDuasLinhas;
        private System.Windows.Forms.CheckBox ChbBarrasCodigo;
        private System.Windows.Forms.CheckBox ChbDocumentoCancelado;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
    }
}

