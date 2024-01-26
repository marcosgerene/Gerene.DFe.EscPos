namespace Gerene.DFe.EscPos.Demo
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
            label1 = new System.Windows.Forms.Label();
            BtnCarregar = new System.Windows.Forms.Button();
            TextArquivoXml = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            BtnListarImpressoras = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            ComboModeloDFe = new System.Windows.Forms.ComboBox();
            ComboProtocolo = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            BtnImprimir = new System.Windows.Forms.Button();
            ComboImpressoras = new System.Windows.Forms.ComboBox();
            ChbCortar = new System.Windows.Forms.CheckBox();
            ChbDuasLinhas = new System.Windows.Forms.CheckBox();
            ChbBarrasCodigo = new System.Windows.Forms.CheckBox();
            ChbDocumentoCancelado = new System.Windows.Forms.CheckBox();
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            label5 = new System.Windows.Forms.Label();
            ComboTipoPapel = new System.Windows.Forms.ComboBox();
            label6 = new System.Windows.Forms.Label();
            ChbRemoverAcentos = new System.Windows.Forms.CheckBox();
            ChbQRCodeLateral = new System.Windows.Forms.CheckBox();
            label7 = new System.Windows.Forms.Label();
            TextDesenvolvedor = new System.Windows.Forms.TextBox();
            ChbQrCodeImagem = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(10, 9);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(78, 16);
            label1.TabIndex = 14;
            label1.Text = "Arquivo Xml";
            // 
            // BtnCarregar
            // 
            BtnCarregar.FlatAppearance.BorderSize = 0;
            BtnCarregar.Location = new System.Drawing.Point(430, 34);
            BtnCarregar.Margin = new System.Windows.Forms.Padding(4);
            BtnCarregar.Name = "BtnCarregar";
            BtnCarregar.Size = new System.Drawing.Size(43, 28);
            BtnCarregar.TabIndex = 13;
            BtnCarregar.Text = "...";
            BtnCarregar.UseVisualStyleBackColor = true;
            BtnCarregar.Click += BtnCarregar_Click;
            // 
            // TextArquivoXml
            // 
            TextArquivoXml.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TextArquivoXml.Location = new System.Drawing.Point(14, 34);
            TextArquivoXml.Margin = new System.Windows.Forms.Padding(4);
            TextArquivoXml.Name = "TextArquivoXml";
            TextArquivoXml.Size = new System.Drawing.Size(409, 22);
            TextArquivoXml.TabIndex = 12;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(10, 70);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(75, 16);
            label2.TabIndex = 16;
            label2.Text = "Impressora";
            // 
            // BtnListarImpressoras
            // 
            BtnListarImpressoras.FlatAppearance.BorderSize = 0;
            BtnListarImpressoras.Location = new System.Drawing.Point(430, 94);
            BtnListarImpressoras.Margin = new System.Windows.Forms.Padding(4);
            BtnListarImpressoras.Name = "BtnListarImpressoras";
            BtnListarImpressoras.Size = new System.Drawing.Size(43, 32);
            BtnListarImpressoras.TabIndex = 17;
            BtnListarImpressoras.Text = "...";
            BtnListarImpressoras.UseVisualStyleBackColor = true;
            BtnListarImpressoras.Click += BtnListarImpressoras_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(10, 135);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(82, 16);
            label3.TabIndex = 18;
            label3.Text = "Modelo DFe";
            // 
            // ComboModeloDFe
            // 
            ComboModeloDFe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ComboModeloDFe.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ComboModeloDFe.FormattingEnabled = true;
            ComboModeloDFe.Location = new System.Drawing.Point(14, 159);
            ComboModeloDFe.Margin = new System.Windows.Forms.Padding(4);
            ComboModeloDFe.Name = "ComboModeloDFe";
            ComboModeloDFe.Size = new System.Drawing.Size(152, 23);
            ComboModeloDFe.TabIndex = 19;
            // 
            // ComboProtocolo
            // 
            ComboProtocolo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ComboProtocolo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ComboProtocolo.FormattingEnabled = true;
            ComboProtocolo.Location = new System.Drawing.Point(177, 159);
            ComboProtocolo.Margin = new System.Windows.Forms.Padding(4);
            ComboProtocolo.Name = "ComboProtocolo";
            ComboProtocolo.Size = new System.Drawing.Size(143, 23);
            ComboProtocolo.TabIndex = 21;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label4.Location = new System.Drawing.Point(174, 135);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(65, 16);
            label4.TabIndex = 20;
            label4.Text = "Protocolo";
            // 
            // BtnImprimir
            // 
            BtnImprimir.FlatAppearance.BorderSize = 0;
            BtnImprimir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            BtnImprimir.Location = new System.Drawing.Point(10, 406);
            BtnImprimir.Margin = new System.Windows.Forms.Padding(4);
            BtnImprimir.Name = "BtnImprimir";
            BtnImprimir.Size = new System.Drawing.Size(137, 51);
            BtnImprimir.TabIndex = 22;
            BtnImprimir.Text = "Imprimir";
            BtnImprimir.UseVisualStyleBackColor = true;
            BtnImprimir.Click += BtnImprimir_Click;
            // 
            // ComboImpressoras
            // 
            ComboImpressoras.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ComboImpressoras.FormattingEnabled = true;
            ComboImpressoras.Location = new System.Drawing.Point(14, 97);
            ComboImpressoras.Margin = new System.Windows.Forms.Padding(4);
            ComboImpressoras.Name = "ComboImpressoras";
            ComboImpressoras.Size = new System.Drawing.Size(409, 24);
            ComboImpressoras.TabIndex = 23;
            // 
            // ChbCortar
            // 
            ChbCortar.AutoSize = true;
            ChbCortar.Checked = true;
            ChbCortar.CheckState = System.Windows.Forms.CheckState.Checked;
            ChbCortar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ChbCortar.Location = new System.Drawing.Point(14, 193);
            ChbCortar.Margin = new System.Windows.Forms.Padding(4);
            ChbCortar.Name = "ChbCortar";
            ChbCortar.Size = new System.Drawing.Size(123, 20);
            ChbCortar.TabIndex = 24;
            ChbCortar.Text = "Cortar ao final";
            ChbCortar.UseVisualStyleBackColor = true;
            // 
            // ChbDuasLinhas
            // 
            ChbDuasLinhas.AutoSize = true;
            ChbDuasLinhas.Checked = true;
            ChbDuasLinhas.CheckState = System.Windows.Forms.CheckState.Checked;
            ChbDuasLinhas.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ChbDuasLinhas.Location = new System.Drawing.Point(14, 215);
            ChbDuasLinhas.Margin = new System.Windows.Forms.Padding(4);
            ChbDuasLinhas.Name = "ChbDuasLinhas";
            ChbDuasLinhas.Size = new System.Drawing.Size(188, 20);
            ChbDuasLinhas.TabIndex = 25;
            ChbDuasLinhas.Text = "Produto em duas linhas";
            ChbDuasLinhas.UseVisualStyleBackColor = true;
            // 
            // ChbBarrasCodigo
            // 
            ChbBarrasCodigo.AutoSize = true;
            ChbBarrasCodigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ChbBarrasCodigo.Location = new System.Drawing.Point(14, 237);
            ChbBarrasCodigo.Margin = new System.Windows.Forms.Padding(4);
            ChbBarrasCodigo.Name = "ChbBarrasCodigo";
            ChbBarrasCodigo.Size = new System.Drawing.Size(166, 20);
            ChbBarrasCodigo.TabIndex = 26;
            ChbBarrasCodigo.Text = "Barras como código";
            ChbBarrasCodigo.UseVisualStyleBackColor = true;
            // 
            // ChbDocumentoCancelado
            // 
            ChbDocumentoCancelado.AutoSize = true;
            ChbDocumentoCancelado.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ChbDocumentoCancelado.Location = new System.Drawing.Point(14, 257);
            ChbDocumentoCancelado.Margin = new System.Windows.Forms.Padding(4);
            ChbDocumentoCancelado.Name = "ChbDocumentoCancelado";
            ChbDocumentoCancelado.Size = new System.Drawing.Size(181, 20);
            ChbDocumentoCancelado.TabIndex = 27;
            ChbDocumentoCancelado.Text = "Documento cancelado";
            ChbDocumentoCancelado.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.FlatAppearance.BorderSize = 0;
            button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            button1.Location = new System.Drawing.Point(228, 430);
            button1.Margin = new System.Windows.Forms.Padding(4);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(112, 22);
            button1.TabIndex = 22;
            button1.Text = "Carregar";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.FlatAppearance.BorderSize = 0;
            button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            button2.Location = new System.Drawing.Point(347, 430);
            button2.Margin = new System.Windows.Forms.Padding(4);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(112, 22);
            button2.TabIndex = 22;
            button2.Text = "Limpar";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pictureBox1.Location = new System.Drawing.Point(219, 212);
            pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(253, 211);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 28;
            pictureBox1.TabStop = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label5.Location = new System.Drawing.Point(219, 193);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(60, 16);
            label5.TabIndex = 18;
            label5.Text = "Logotipo";
            // 
            // ComboTipoPapel
            // 
            ComboTipoPapel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ComboTipoPapel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ComboTipoPapel.FormattingEnabled = true;
            ComboTipoPapel.Location = new System.Drawing.Point(330, 159);
            ComboTipoPapel.Margin = new System.Windows.Forms.Padding(4);
            ComboTipoPapel.Name = "ComboTipoPapel";
            ComboTipoPapel.Size = new System.Drawing.Size(143, 23);
            ComboTipoPapel.TabIndex = 21;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label6.Location = new System.Drawing.Point(327, 135);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(93, 16);
            label6.TabIndex = 20;
            label6.Text = "Tipo de Papel";
            // 
            // ChbRemoverAcentos
            // 
            ChbRemoverAcentos.AutoSize = true;
            ChbRemoverAcentos.Checked = true;
            ChbRemoverAcentos.CheckState = System.Windows.Forms.CheckState.Checked;
            ChbRemoverAcentos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ChbRemoverAcentos.Location = new System.Drawing.Point(14, 279);
            ChbRemoverAcentos.Margin = new System.Windows.Forms.Padding(4);
            ChbRemoverAcentos.Name = "ChbRemoverAcentos";
            ChbRemoverAcentos.Size = new System.Drawing.Size(149, 20);
            ChbRemoverAcentos.TabIndex = 29;
            ChbRemoverAcentos.Text = "Remover Acentos";
            ChbRemoverAcentos.UseVisualStyleBackColor = true;
            // 
            // ChbQRCodeLateral
            // 
            ChbQRCodeLateral.AutoSize = true;
            ChbQRCodeLateral.Checked = true;
            ChbQRCodeLateral.CheckState = System.Windows.Forms.CheckState.Checked;
            ChbQRCodeLateral.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ChbQRCodeLateral.Location = new System.Drawing.Point(14, 303);
            ChbQRCodeLateral.Margin = new System.Windows.Forms.Padding(4);
            ChbQRCodeLateral.Name = "ChbQRCodeLateral";
            ChbQRCodeLateral.Size = new System.Drawing.Size(141, 20);
            ChbQRCodeLateral.TabIndex = 30;
            ChbQRCodeLateral.Text = "QR Code Lateral";
            ChbQRCodeLateral.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label7.Location = new System.Drawing.Point(14, 351);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(100, 16);
            label7.TabIndex = 31;
            label7.Text = "Desenvolvedor";
            // 
            // TextDesenvolvedor
            // 
            TextDesenvolvedor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TextDesenvolvedor.Location = new System.Drawing.Point(14, 371);
            TextDesenvolvedor.Margin = new System.Windows.Forms.Padding(4);
            TextDesenvolvedor.Name = "TextDesenvolvedor";
            TextDesenvolvedor.Size = new System.Drawing.Size(195, 22);
            TextDesenvolvedor.TabIndex = 32;
            TextDesenvolvedor.Text = "Desenvolvido por XXX";
            // 
            // ChbQrCodeImagem
            // 
            ChbQrCodeImagem.AutoSize = true;
            ChbQrCodeImagem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ChbQrCodeImagem.Location = new System.Drawing.Point(14, 326);
            ChbQrCodeImagem.Margin = new System.Windows.Forms.Padding(4);
            ChbQrCodeImagem.Name = "ChbQrCodeImagem";
            ChbQrCodeImagem.Size = new System.Drawing.Size(190, 20);
            ChbQrCodeImagem.TabIndex = 33;
            ChbQrCodeImagem.Text = "QR Code como imagem";
            ChbQrCodeImagem.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(484, 466);
            Controls.Add(ChbQrCodeImagem);
            Controls.Add(TextDesenvolvedor);
            Controls.Add(label7);
            Controls.Add(ChbQRCodeLateral);
            Controls.Add(ChbRemoverAcentos);
            Controls.Add(label6);
            Controls.Add(ComboTipoPapel);
            Controls.Add(label5);
            Controls.Add(pictureBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(ChbDocumentoCancelado);
            Controls.Add(ChbBarrasCodigo);
            Controls.Add(ChbDuasLinhas);
            Controls.Add(ChbCortar);
            Controls.Add(ComboImpressoras);
            Controls.Add(BtnImprimir);
            Controls.Add(ComboProtocolo);
            Controls.Add(label4);
            Controls.Add(ComboModeloDFe);
            Controls.Add(label3);
            Controls.Add(BtnListarImpressoras);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(BtnCarregar);
            Controls.Add(TextArquivoXml);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnCarregar;
        private System.Windows.Forms.TextBox TextArquivoXml;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtnListarImpressoras;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ComboModeloDFe;
        private System.Windows.Forms.ComboBox ComboProtocolo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button BtnImprimir;
        private System.Windows.Forms.ComboBox ComboImpressoras;
        private System.Windows.Forms.CheckBox ChbCortar;
        private System.Windows.Forms.CheckBox ChbDuasLinhas;
        private System.Windows.Forms.CheckBox ChbBarrasCodigo;
        private System.Windows.Forms.CheckBox ChbDocumentoCancelado;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox ComboTipoPapel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox ChbRemoverAcentos;
        private System.Windows.Forms.CheckBox ChbQRCodeLateral;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TextDesenvolvedor;
        private System.Windows.Forms.CheckBox ChbQrCodeImagem;
    }
}

