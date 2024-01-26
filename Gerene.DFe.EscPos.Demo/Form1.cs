using OpenAC.Net.EscPos.Commom;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Gerene.DFe.EscPos.Demo
{
    public partial class Form1 : Form
    {
        private enum ModeloDFe
        {
            NFCe,
            SAT,
            CancelamentoSat
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
            ComboProtocolo.DataSource = Enum.GetValues(typeof(ProtocoloEscPos));
            ComboTipoPapel.DataSource = Enum.GetValues(typeof(TipoPapel));

            ComboTipoPapel.SelectedItem = TipoPapel.Tp80mm;
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
            DfePrinter _printer = null;
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
                    case ModeloDFe.CancelamentoSat:
                        _printer = new SatPrinter();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (pictureBox1.Image != null)
                    _printer.Logotipo = ImageToByteArray(pictureBox1.Image);

                _printer.Protocolo = (ProtocoloEscPos)ComboProtocolo.SelectedItem;
                _printer.Impressora = ComboImpressoras.Text;
                _printer.CortarPapel = ChbCortar.Checked;
                _printer.ProdutoDuasLinhas = ChbDuasLinhas.Checked;
                _printer.UsarBarrasComoCodigo = ChbBarrasCodigo.Checked;
                _printer.DocumentoCancelado = ChbDocumentoCancelado.Checked;
                _printer.TipoPapel = (TipoPapel)ComboTipoPapel.SelectedItem;
                _printer.RemoverAcentos = ChbRemoverAcentos.Checked;
                _printer.QrCodeLateral = ChbQRCodeLateral.Checked;
                _printer.Desenvolvedor = TextDesenvolvedor.Text;

                if (ChbQrCodeImagem.Checked)
                {
                    string qrcode = _printer.QRCodeTexto(xml);

                    Bitmap qrCodeImage;
                    using (var qrGenerator = new QRCoder.QRCodeGenerator())
                    using (var qrCodeData = qrGenerator.CreateQrCode(qrcode, QRCoder.QRCodeGenerator.ECCLevel.H))
                    using (var qrCode = new QRCoder.QRCode(qrCodeData))
                        qrCodeImage = qrCode.GetGraphic(3);

                    _printer.QrCodeImagem = qrCodeImage;                    
                }

                if ((ModeloDFe)ComboModeloDFe.SelectedItem == ModeloDFe.CancelamentoSat)
                {
                    if (ChbQrCodeImagem.Checked)
                    {
                        string qrcode = _printer.QRCodeTextoCanc(xml);

                        Bitmap qrCodeImage;
                        using (var qrGenerator = new QRCoder.QRCodeGenerator())
                        using (var qrCodeData = qrGenerator.CreateQrCode(qrcode, QRCoder.QRCodeGenerator.ECCLevel.H))
                        using (var qrCode = new QRCoder.QRCode(qrCodeData))
                            qrCodeImage = qrCode.GetGraphic(3);

                        _printer.QrCodeImagemCanc = qrCodeImage;
                    }


                    (_printer as SatPrinter).ImprimirCancelamento(xml);
                }
                else
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

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //https://www.c-sharpcorner.com/UploadFile/mirfan00/uploaddisplay-image-in-picture-box-using-C-Sharp/

            // open file dialog   
            OpenFileDialog open = new OpenFileDialog();
            // image filters  
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                // display image in picture box  
                pictureBox1.Image = new Bitmap(open.FileName);
            }
        }

        //https://stackoverflow.com/questions/3801275/how-to-convert-image-to-byte-array
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

    }
}
