using ACBr.Net.Core.Extensions;
using ACBr.Net.Sat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Vip.Printer;
using Vip.Printer.Enums;

namespace Gerene.DFe.EscPos
{
    public class SatPrinter : IDfePrinter
    {
        public SatPrinter()
        {
            _ACBrSat = new ACBrSat();
            _CFe = new CFe();
        }



        #region IDfe
        public string NomeImpressora { get; set; }
        public PrinterType TipoImpressora { get; set; }
        public bool CortarPapel { get; set; }
        private CultureInfo _Cultura => new CultureInfo("pt-Br");

        private Printer _Printer { get; set; }
        private ACBrSat _ACBrSat { get; set; }
        private CFe _CFe { get; set; }

        public void Imprimir(string xmlcontent)
        {
            _CFe = CFe.Load(new MemoryStream(Encoding.UTF8.GetBytes(xmlcontent)), Encoding.UTF8);

            _Printer = new Printer(NomeImpressora, TipoImpressora);

            #region Cabeçalho

            #region Dados do Emitente
            _Printer.AlignCenter();
            _Printer.BoldMode(PrinterModeState.On);
            _Printer.Append(_CFe.InfCFe.Emit.XFant.RemoveAccent());

            _Printer.AlignLeft();
            _Printer.BoldMode(PrinterModeState.Off);
            _Printer.Append(_CFe.InfCFe.Emit.XNome.RemoveAccent());

            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.Append(GereneHelpers.TextoEsquerda_Direita($"Cnpj: {_CFe.InfCFe.Emit.CNPJ.FormatoCpfCnpj()}", $"I.E.: {_CFe.InfCFe.Emit.IE}", _Printer.ColsCondensed));

            _Printer.AppendWithoutLf("End.: ");
            _Printer.Append($"{_CFe.InfCFe.Emit.EnderEmit.XLgr.RemoveAccent()},{_CFe.InfCFe.Emit.EnderEmit.Nro.RemoveAccent()} {_CFe.InfCFe.Emit.EnderEmit.XCpl.RemoveAccent()}");

            _Printer.AppendWithoutLf("Bairro: ");
            _Printer.Append($"{_CFe.InfCFe.Emit.EnderEmit.XBairro.RemoveAccent()} - {_CFe.InfCFe.Emit.EnderEmit.XMun.RemoveAccent()} - {_CFe.InfCFe.Emit.EnderEmit.CEP.FormatoCep()}");

            _Printer.CondensedMode(PrinterModeState.Off);
            _Printer.Separator();
            #endregion

            #region Número do extrato
            _Printer.AlignCenter();
            _Printer.BoldMode(PrinterModeState.On);
            _Printer.Append($"Extrato No. {_CFe.InfCFe.Ide.NCFe:D6}");
            _Printer.Append($"CUPOM FISCAL ELETRONICO SAT");
            #endregion

            #region Homologação
            if (_CFe.InfCFe.Ide.TpAmb == ACBr.Net.DFe.Core.Common.DFeTipoAmbiente.Homologacao)
            {
                _Printer.Separator();
                _Printer.AlignCenter();
                _Printer.BoldMode(PrinterModeState.On);
                _Printer.Append("AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                _Printer.BoldMode(PrinterModeState.Off);
                _Printer.Separator();
            }
            #endregion

            #region Consumidor
            _Printer.AlignLeft();
            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.AppendWithoutLf("CPF/CNPJ do Consumidor: ");
            _Printer.Append(_CFe.InfCFe.Dest?.CPF.IsNotNull() == true ? _CFe.InfCFe.Dest.CPF.FormataCPF() :
                            _CFe.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFe.InfCFe.Dest.CNPJ.FormataCNPJ() :
                            "000.000.000-00");
            _Printer.AppendWithoutLf("Razao Social/Nome: ");
            _Printer.Append(_CFe.InfCFe.Dest?.Nome ?? "CONSUMIDOR");
            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion

            _Printer.Separator();
            #endregion

            #region Detalhes
            //To do
            #endregion

            #region Pagamentos
            //To do
            #endregion

            #region Dados da entrega            
            if (_CFe.InfCFe.Entrega != null && !_CFe.InfCFe.Entrega.XLgr.IsNull())
            {
                _Printer.AlignCenter();
                _Printer.BoldMode(PrinterModeState.On);
                _Printer.Append("DADOS PARA ENTREGA");

                _Printer.AlignLeft();
                _Printer.BoldMode(PrinterModeState.Off);
                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.AppendWithoutLf("End.: ");
                _Printer.Append($"{_CFe.InfCFe.Entrega.XLgr.RemoveAccent()}, {_CFe.InfCFe.Entrega.Nro.RemoveAccent()} {_CFe.InfCFe.Entrega.XCpl.RemoveAccent()}");
                _Printer.AppendWithoutLf("Bairro: ");
                _Printer.Append($"{_CFe.InfCFe.Entrega.XBairro.RemoveAccent()} - {_CFe.InfCFe.Entrega.XMun.RemoveAccent()}/{_CFe.InfCFe.Entrega.UF}");
                _Printer.CondensedMode(PrinterModeState.Off);

                _Printer.Separator();
            }
            #endregion

            #region Observações do Fisco         
            if (_CFe.InfCFe.InfAdic.ObsFisco.Any())
            {
                _Printer.AlignLeft();
                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.BoldMode("Observacoes do Fisco");

                foreach (var fisco in _CFe.InfCFe.InfAdic.ObsFisco)
                {
                    string texto = $"{fisco.XCampo} - {fisco.XTexto}";

                    foreach (var txt in texto.Split(40))
                        _Printer.Append(txt.RemoveAccent());
                }

                _Printer.NewLine();

                _Printer.CondensedMode(PrinterModeState.Off);

            }
            #endregion

            #region Observações do Contribuinte          
            _Printer.AlignLeft();
            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.BoldMode("Observacoes do Contribuinte");

            if (!_CFe.InfCFe.InfAdic.InfCpl.IsNull())
                foreach (var txt in _CFe.InfCFe.InfAdic.InfCpl.Split(40))
                    _Printer.Append(txt.RemoveAccent());

            _Printer.NewLine();

            _Printer.CondensedMode(PrinterModeState.Off);

            #endregion

            #region Tributos
            _Printer.AlignLeft();
            _Printer.BoldMode(PrinterModeState.Off);
            _Printer.CondensedMode(PrinterModeState.On);

            _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Valor aproximado dos Tributos deste Cupom", _CFe.InfCFe.Total.VCFeLei12741.ToString("C2", _Cultura), _Printer.ColsCondensed));
            _Printer.Append("(Conforme Lei Fed. 12.741/2012)");

            _Printer.NewLine();

            _Printer.CondensedMode(PrinterModeState.Off);

            #endregion

            #region Rodape

            #region Número do extrato
            _Printer.AlignCenter();
            _Printer.Append($"SAT No. {_CFe.InfCFe.Ide.NSerieSAT:D9}");
            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.Append($"Data e Hora {_CFe.InfCFe.Ide.DEmi:dd/MM/yyyy} {_CFe.InfCFe.Ide.HEmi:HH:mm:ss}");
            _Printer.CondensedMode(PrinterModeState.Off);
            _Printer.NewLine();
            #endregion

            #region Chave de Acesso
            _Printer.AlignLeft();
            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.Append(Regex.Replace(_CFe.InfCFe.Id.OnlyNumber(), ".{4}", "$0 "));

            _Printer.NewLine();

            _Printer.AlignCenter();
            _Printer.Code128(_CFe.InfCFe.Id.OnlyNumber().Substring(0,22));
            _Printer.Code128(_CFe.InfCFe.Id.OnlyNumber().Substring(22));

            _Printer.NewLine();

            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion

            #region QrCode
            _Printer.AlignCenter();
            string _qrCode = $"{_CFe.InfCFe.Id.OnlyNumber()}|" +
                             $"{_CFe.InfCFe.Ide.DhEmissao:yyyyMMddHHmmss}|" +
                             $"{_CFe.InfCFe.Total.VCFe:0.00}|" +
                             $"{(_CFe.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFe.InfCFe.Dest.CNPJ : _CFe.InfCFe.Dest.CPF)}|" +
                             $"{_CFe.InfCFe.Ide.AssinaturaQrcode}";

            _Printer.QrCode(_qrCode, QrCodeSize.Size1);
            #endregion

            _Printer.AlignCenter();
            _Printer.CondensedMode(PrinterModeState.On);

            _Printer.Append("Consulte o QR Code pelo aplicativo \"De olho na nota\"");
            _Printer.Append("disponível na AppStore (Apple) e PlayStore (Android)");

            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion

            if (CortarPapel)
                _Printer.PartialPaperCut();

            _Printer.PrintDocument();
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (_ACBrSat != null)
            {
                _ACBrSat.Dispose();
                _ACBrSat = null;
            }

            if (_CFe != null)
            {
                _CFe = null;
            }

            if (_Printer != null)
            {
                _Printer.Clear();
                _Printer = null;
            }
        }
        #endregion
    }
}
