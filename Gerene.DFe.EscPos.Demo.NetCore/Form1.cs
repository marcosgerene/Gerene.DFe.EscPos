﻿using System;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Vip.Printer.Enums;

namespace Gerene.DFe.EscPos.Demo.NetCore
{
    public partial class Form1 : Form
    {
        private enum ModeloDFe
        {
            NFCe,
            SAT
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //https://stackoverflow.com/questions/39065988/utf8-is-not-a-supported-encoding-name
            EncodingProvider provider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(provider);

            //https://stackoverflow.com/questions/906899/binding-an-enum-to-a-winforms-combo-box-and-then-setting-it
            ComboModeloDFe.DataSource = Enum.GetValues(typeof(ModeloDFe));
            ComboTipoImpressora.DataSource = Enum.GetValues(typeof(PrinterType));
        }

        private void BtnListarImpressoras_Click(object sender, EventArgs e)
        {
            // https://docs.microsoft.com/pt-br/dotnet/api/system.drawing.printing.printersettings.installedprinters?view=dotnet-plat-ext-3.1

            // Add list of installed printers found to the combo box.
            // The pkInstalledPrinters string will be used to provide the display string.
            String pkInstalledPrinters;
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                pkInstalledPrinters = PrinterSettings.InstalledPrinters[i];
                ComboImpressoras.Items.Add(pkInstalledPrinters);
            }
        }

        private void BtnCarregar_Click(object sender, EventArgs e)
        {
            //https://stackoverflow.com/questions/17567533/openfiledialog-xml-filter-allowing-htm-shortcuts

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML Files (*.xml)|*.xml";
            ofd.FilterIndex = 0;
            ofd.DefaultExt = "xml";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (!String.Equals(Path.GetExtension(ofd.FileName),
                                   ".xml",
                                   StringComparison.OrdinalIgnoreCase))
                {
                    // Invalid file type selected; display an error.
                    MessageBox.Show("The type of the selected file is not supported by this application. You must select an XML file.",
                                    "Invalid File Type",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                    // Optionally, force the user to select another file.
                    // ...
                }
                else
                {
                    TextArquivoXml.Text = ofd.FileName;
                }
            }
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            IDfePrinter _printer = null;
            try
            {
                if (!File.Exists(TextArquivoXml.Text))
                    throw new ArgumentException("Arquivo xml não encontrado");

                if (ComboImpressoras.Text.IsNull())
                    throw new ArgumentException("Nenhuma impressora preenchida");

                string xml = File.ReadAllText(TextArquivoXml.Text, Encoding.UTF8);

                switch ((ModeloDFe)ComboModeloDFe.SelectedItem)
                {
                    case ModeloDFe.NFCe:
                        _printer = new NFCePrinter();
                        break;
                    case ModeloDFe.SAT:
                        _printer = new SatPrinter();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                _printer.TipoImpressora = (PrinterType)ComboTipoImpressora.SelectedItem;
                _printer.NomeImpressora = ComboImpressoras.Text;
                _printer.CortarPapel = ChbCortar.Checked;
                _printer.ProdutoDuasLinhas = ChbDuasLinhas.Checked;
                _printer.UsarBarrasComoCodigo = ChbBarrasCodigo.Checked;
                _printer.DocumentoCancelado = ChbDocumentoCancelado.Checked;

                _printer.Imprimir(xml);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (_printer != null)
                    _printer.Dispose();
            }
        }
    }
}
