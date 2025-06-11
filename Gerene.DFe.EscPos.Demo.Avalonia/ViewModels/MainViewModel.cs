using CommunityToolkit.Mvvm.Input;
using OpenAC.Net.EscPos.Commom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Media.Imaging;

using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;
using SixLabors.ImageSharp;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;

namespace Gerene.DFe.EscPos.Demo.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public enum ModeloDFe
    {
        NFCe
    }

    public MainViewModel()
    {
        CarregarEnums();
        ListarImpressoras();

        CortarAoFinal = true;
        ProdutoEmDuasLinhas = true;
        RemoverAcertos = true;
        QrCodeLateral = true;
        Desenvolvedor = "Desenvolvidor por XXXX";
    }

    [ObservableProperty]
    private string caminhoXml;

    public bool CortarAoFinal { get; set; }
    public bool ProdutoEmDuasLinhas { get; set; }
    public bool BarrasComoCodigo { get; set; }
    public bool DocumentoCancelado { get; set; }
    public bool RemoverAcertos { get; set; }
    public bool QrCodeLateral { get; set; }
    public bool QrCodeComoImagem { get; set; }
    public string Desenvolvedor { get; set; }

    public ModeloDFe Modelo { get; set; }
    public ProtocoloEscPos Protocolo { get; set; }
    public TipoPapel TipoPapel { get; set; }

    public IEnumerable<ModeloDFe> ModelosDfe { get; set; }
    public IEnumerable<ProtocoloEscPos> Protocolos { get; set; }
    public IEnumerable<TipoPapel> TiposPapel { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LogotipoBitmap))]
    private byte[]? logotipoBytes;

    //https://docs.avaloniaui.net/docs/guides/data-binding/how-to-bind-image-files

    public Bitmap? LogotipoBitmap
    {
        get
        {
            if (logotipoBytes is null)
                return null;

            using (MemoryStream stream = new(logotipoBytes))
                return new Bitmap(stream);
        }
    }

    public IEnumerable<string> Impressoras { get; set; }
    public string Impressora { get; set; }

    private void CarregarEnums()
    {
        ModelosDfe = (IEnumerable<ModeloDFe>)Enum.GetValues(typeof(ModeloDFe));
        Protocolos = (IEnumerable<ProtocoloEscPos>)Enum.GetValues(typeof(ProtocoloEscPos));
        TiposPapel = (IEnumerable<TipoPapel>)Enum.GetValues(typeof(TipoPapel));

        Modelo = ModeloDFe.NFCe;
        Protocolo = ProtocoloEscPos.EscPos;
        TipoPapel = TipoPapel.Tp80mm;
    }

    private void ListarImpressoras()
    {
        List<string> impressoras = new();

        if (OperatingSystem.IsWindows())
        {
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
                impressoras.Add(PrinterSettings.InstalledPrinters[i]);
        }

        else if (OperatingSystem.IsLinux())
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "lpstat",
                        Arguments = "-a",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    if (line.StartsWith("printer"))
                        impressoras.Add(line.Split(' ')[1]);
                }
            }

            catch
            {
                /* sudo apt-get install cups-client  */
            }
        }

        Impressoras = impressoras;
        Impressora = impressoras.FirstOrDefault() ?? string.Empty;
    }

    //https://docs.avaloniaui.net/docs/basics/user-interface/file-dialogs
    private async void CarregarLogotipo()
    {
        TopLevel topLevel;

        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            topLevel = desktop.MainWindow ?? throw new InvalidOperationException();
        else
            throw new ArgumentException("lifetime inválido");

        FilePickerOpenOptions pickerOptions = new()
        {
            Title = "Escolher logotipo",
            //https://docs.avaloniaui.net/docs/concepts/services/storage-provider/file-picker-options
            FileTypeFilter = new[] { FilePickerFileTypes.ImageAll },
            AllowMultiple = false
        };


        var files = await topLevel.StorageProvider.OpenFilePickerAsync(pickerOptions);

        if (files.Count == 1)
        {
            await using var stream = await files.Single().OpenReadAsync();
            using MemoryStream memoryStream = new();
            stream.CopyTo(memoryStream);
            LogotipoBytes = memoryStream.ToArray();
        }
    }

    //https://docs.avaloniaui.net/docs/basics/user-interface/file-dialogs
    private async void CarregarXml()
    {
        TopLevel topLevel;

        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            topLevel = desktop.MainWindow ?? throw new InvalidOperationException();
        else
            throw new ArgumentException("lifetime inválido");

        FilePickerOpenOptions pickerOptions = new()
        {
            Title = "Escolher arquivo XML",
            FileTypeFilter = new[] {
                new FilePickerFileType("Arquivo XML")
                {
                    Patterns = new string[1] { "*.xml" },
                    AppleUniformTypeIdentifiers = new string[1] { "public.xml" },
                    MimeTypes = new string[1] { "text/plain" }
                }
            },
            AllowMultiple = false
        };

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(pickerOptions);

        if (files.Count == 1)
            CaminhoXml = files.Single().TryGetLocalPath();
    }

    private void Imprimir()
    {
        DfePrinter _printer = null;
        try
        {
            if (!File.Exists(caminhoXml))
                throw new ArgumentException("Arquivo xml não encontrado");

            if (Impressora.IsNull())
                throw new ArgumentException("Nenhuma impressora preenchida");

            string xml = File.ReadAllText(caminhoXml, Encoding.UTF8);

            switch (Modelo)
            {
                case ModeloDFe.NFCe:
                    _printer = new NFCePrinter();
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (logotipoBytes != null)
                _printer.Logotipo = logotipoBytes;

            _printer.Protocolo = Protocolo;
            _printer.Impressora = Impressora;
            _printer.CortarPapel = CortarAoFinal;
            _printer.ProdutoDuasLinhas = ProdutoEmDuasLinhas;
            _printer.UsarBarrasComoCodigo = BarrasComoCodigo;
            _printer.DocumentoCancelado = DocumentoCancelado;
            _printer.TipoPapel = TipoPapel;
            _printer.RemoverAcentos = RemoverAcertos;
            _printer.QrCodeLateral = QrCodeLateral;
            _printer.Desenvolvedor = Desenvolvedor;

            if (QrCodeComoImagem)
            {
                string qrcode = _printer.QRCodeTexto(xml);

                SixLabors.ImageSharp.Image qrCodeImage;
                using (var qrGenerator = new QRCoder.QRCodeGenerator())
                using (var qrCodeData = qrGenerator.CreateQrCode(qrcode, QRCoder.QRCodeGenerator.ECCLevel.H))
                using (var qrCode = new QRCoder.QRCode(qrCodeData))
                    qrCodeImage = qrCode.GetGraphic(3);

                using MemoryStream memoryStream = new();
                qrCodeImage.SaveAsPng(memoryStream);
                _printer.QrCodeImagem = new System.Drawing.Bitmap(memoryStream);
            }

            _printer.Imprimir(xml);
        }
        catch (Exception ex)
        {
            //https://github.com/AvaloniaCommunity/MessageBox.Avalonia

            var box = MessageBoxManager.GetMessageBoxStandard("Falha ao imprimir", ex.Message, ButtonEnum.Ok);
            box.ShowAsync();
        }
        finally
        {
            if (_printer != null)
                _printer.Dispose();
        }
    }

    #region Commands
    public ICommand ListarImpressorasCommand => new RelayCommand(ListarImpressoras);

    public ICommand CarregarLogotipoCommand => new RelayCommand(CarregarLogotipo);

    public ICommand LimparLogotipoCommand => new RelayCommand(() => LogotipoBytes = null);

    public ICommand CarregarXmlCommand => new RelayCommand(CarregarXml);

    public ICommand ImprimirCommand => new RelayCommand(Imprimir);
    #endregion
}
