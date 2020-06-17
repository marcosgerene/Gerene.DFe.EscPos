using ACBr.Net.Core.Extensions;
using ACBr.Net.Sat;
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
        public bool ProdutoDuasLinhas { get; set; }
        public bool UsarBarrasComoCodigo { get; set; }
        public bool DocumentoCancelado { get; set; }       
        public byte[] Logotipo { get; set; }
        public CultureInfo Cultura { get; set; } = new CultureInfo("pt-Br");

        private Printer _Printer { get; set; }
        private ACBrSat _ACBrSat { get; set; }
        private CFe _CFe { get; set; }
        private CFeCanc _CFeCanc { get; set; }

        public void Imprimir(string xmlcontent)
        {
            _CFe = CFe.Load(new MemoryStream(Encoding.UTF8.GetBytes(xmlcontent)), Encoding.UTF8);

            _Printer = new Printer(NomeImpressora, TipoImpressora);

            #region Cabeçalho

            #region Logotipo
            if (Logotipo != null)
            {
                //Impressão do logotipo ainda não implementada
            }
            #endregion

            #region Dados do Emitente
            _Printer.AlignCenter();
            _Printer.BoldMode(PrinterModeState.On);
            _Printer.Append(_CFe.InfCFe.Emit.XFant.LimitarString(_Printer.ColsNomal).RemoveAccent());

            _Printer.AlignLeft();
            _Printer.BoldMode(PrinterModeState.Off);
            _Printer.Append(_CFe.InfCFe.Emit.XNome.LimitarString(_Printer.ColsNomal).RemoveAccent());

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

            #region Documento Cancelado
            if (DocumentoCancelado)
            {
                _Printer.Separator();
                _Printer.AlignCenter();
                _Printer.BoldMode(PrinterModeState.On);
                _Printer.Append("*** DOCUMENTO CANCELADO ***");
                _Printer.BoldMode(PrinterModeState.Off);
                _Printer.Separator();
            }
            #endregion

            #region Consumidor
            _Printer.AlignLeft();
            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.AppendWithoutLf("CPF/CNPJ do Consumidor: ");
            _Printer.Append(_CFe.InfCFe.Dest?.CPF.IsNotNull() == true ? _CFe.InfCFe.Dest.CPF.FormatoCpfCnpj() :
                            _CFe.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFe.InfCFe.Dest.CNPJ.FormatoCpfCnpj() :
                            "000.000.000-00");
            _Printer.AppendWithoutLf("Razao Social/Nome: ");
            _Printer.Append((_CFe.InfCFe.Dest?.Nome ?? "CONSUMIDOR").LimitarString(_Printer.ColsCondensed));
            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion

            _Printer.Separator();
            #endregion

            #region Detalhes
            _Printer.AlignCenter();
            _Printer.BoldMode("#|COD|DESC|QTD|UN|VL UN|DESC|VL ITEM");
            _Printer.Separator();

            #region Produtos
            _Printer.AlignLeft();
            _Printer.CondensedMode(PrinterModeState.On);
            foreach (var det in _CFe.InfCFe.Det)
            {
                string textoE = string.Empty;

                string codProd = det.Prod.CProd;
                if (UsarBarrasComoCodigo)
                    codProd = $"{(UsarBarrasComoCodigo && det.Prod.CEAN.IsNotNull() ? det.Prod.CEAN : det.Prod.CProd).PadRight(13)}";

                if (ProdutoDuasLinhas)
                    textoE = $"{ det.NItem:D3} | {codProd}";
                else
                    textoE = $"{ det.NItem:D3} | {codProd} {det.Prod.XProd}";

                string textoR = $"{det.Prod.QCom:N3} {det.Prod.UCom} x {det.Prod.VUnCom:N2} = {det.Prod.VItem:N2}";

                _Printer.Append(GereneHelpers.TextoEsquerda_Direita(textoE, textoR, _Printer.ColsCondensed));

                if (ProdutoDuasLinhas)
                    _Printer.Append(det.Prod.XProd.LimitarString(_Printer.ColsCondensed));

                if (det.Prod.VOutro > 0)
                    _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Acrescimos:", det.Prod.VDesc.ToString("C2", Cultura), _Printer.ColsCondensed));

                if (det.Prod.VDesc > 0)
                    _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Descontos:", det.Prod.VDesc.ToString("C2", Cultura), _Printer.ColsCondensed));
            }
            _Printer.CondensedMode(PrinterModeState.Off);
            _Printer.Separator();

            #region Totais
            _Printer.BoldMode(PrinterModeState.On);
            _Printer.CondensedMode(PrinterModeState.On);

            _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Qtde. total de itens:", _CFe.InfCFe.Det.Count.ToString("N0", Cultura), _Printer.ColsCondensed));

            _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Subtotal:", _CFe.InfCFe.Total.ICMSTot.VProd.ToString("C2", Cultura), _Printer.ColsCondensed));

            if (_CFe.InfCFe.Total.ICMSTot.VOutro > 0)
                _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Acrescimos:", _CFe.InfCFe.Total.ICMSTot.VOutro.ToString("C2", Cultura), _Printer.ColsCondensed));

            if (_CFe.InfCFe.Total.ICMSTot.VDesc > 0)
                _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Descontos:", _CFe.InfCFe.Total.ICMSTot.VDesc.ToString("C2", Cultura), _Printer.ColsCondensed));

            _Printer.CondensedMode(PrinterModeState.Off);


            _Printer.BoldMode(GereneHelpers.TextoEsquerda_Direita("Valor TOTAL:", _CFe.InfCFe.Total.VCFe.ToString("C2", Cultura), _Printer.ColsNomal));

            _Printer.BoldMode(PrinterModeState.Off);
            #endregion
            #endregion

            _Printer.NewLine();
            #endregion

            #region Pagamentos
            _Printer.AlignLeft();
            _Printer.CondensedMode(PrinterModeState.On);

            foreach (var _pagto in _CFe.InfCFe.Pagto.Pagamentos)
                _Printer.Append(GereneHelpers.TextoEsquerda_Direita(_pagto.CMp.GetDescription().RemoveAccent(), _pagto.VMp.ToString("C2", Cultura), _Printer.ColsCondensed));

            _Printer.CondensedMode(PrinterModeState.Off);

            _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Troco:", _CFe.InfCFe.Pagto.VTroco.ToString("C2", Cultura), _Printer.ColsNomal));
            _Printer.NewLine();
            #endregion

            #region Rodape
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

            _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Valor aproximado dos Tributos deste Cupom", _CFe.InfCFe.Total.VCFeLei12741.ToString("C2", Cultura), _Printer.ColsCondensed));
            _Printer.Append("(Conforme Lei Fed. 12.741/2012)");
            _Printer.CondensedMode(PrinterModeState.Off);

            _Printer.Separator();
            #endregion

            #region Número do extrato
            _Printer.AlignCenter();
            _Printer.Append($"SAT No. {_CFe.InfCFe.Ide.NSerieSAT:D9}");
            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.Append($"Data e Hora {_CFe.InfCFe.Ide.DEmi:dd/MM/yyyy} {_CFe.InfCFe.Ide.HEmi:HH:mm:ss}");
            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion

            #region Chave de Acesso
            _Printer.AlignCenter();
            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.BoldMode(Regex.Replace(_CFe.InfCFe.Id.OnlyNumber(), ".{4}", "$0 "));

            _Printer.Code128(_CFe.InfCFe.Id.OnlyNumber().Substring(0, 22));
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

            _Printer.NewLine();
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

        public void ImprimirCancelamento(string xmlContent)
        {
            _CFeCanc = CFeCanc.Load(new MemoryStream(Encoding.UTF8.GetBytes(xmlContent)), Encoding.UTF8);

            _Printer = new Printer(NomeImpressora, TipoImpressora);

            #region Logotipo
            if (Logotipo != null)
            {
                //Impressão do logotipo ainda não implementada
            }
            #endregion

            #region Dados do Emitente
            _Printer.AlignCenter();
            _Printer.BoldMode(PrinterModeState.On);
            _Printer.Append((_CFeCanc.InfCFe.Emit.XFant.IsNotNull() ? _CFeCanc.InfCFe.Emit.XFant : _CFeCanc.InfCFe.Emit.XNome).LimitarString(_Printer.ColsNomal).RemoveAccent());

            _Printer.AlignLeft();
            _Printer.BoldMode(PrinterModeState.Off);
            _Printer.Append(_CFeCanc.InfCFe.Emit.XNome.LimitarString(_Printer.ColsNomal).RemoveAccent());

            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.Append(GereneHelpers.TextoEsquerda_Direita($"Cnpj: {_CFeCanc.InfCFe.Emit.CNPJ.FormatoCpfCnpj()}", $"I.E.: {_CFeCanc.InfCFe.Emit.IE}", _Printer.ColsCondensed));

            _Printer.AppendWithoutLf("End.: ");
            _Printer.Append($"{_CFeCanc.InfCFe.Emit.EnderEmit.XLgr.RemoveAccent()},{_CFeCanc.InfCFe.Emit.EnderEmit.Nro.RemoveAccent()} {_CFeCanc.InfCFe.Emit.EnderEmit.XCpl.RemoveAccent()}");

            _Printer.AppendWithoutLf("Bairro: ");
            _Printer.Append($"{_CFeCanc.InfCFe.Emit.EnderEmit.XBairro.RemoveAccent()} - {_CFeCanc.InfCFe.Emit.EnderEmit.XMun.RemoveAccent()} - {_CFeCanc.InfCFe.Emit.EnderEmit.CEP.FormatoCep()}");

            _Printer.CondensedMode(PrinterModeState.Off);
            _Printer.Separator();
            #endregion

            #region Número do extrato
            _Printer.AlignCenter();
            _Printer.BoldMode(PrinterModeState.On);
            _Printer.Append($"Extrato No. {_CFeCanc.InfCFe.Ide.NCFe:D6}");
            _Printer.Append($"CUPOM FISCAL ELETRONICO SAT");
            _Printer.Append($"CANCELAMENTO");
            _Printer.BoldMode(PrinterModeState.Off);
            #endregion

            #region Homologação
            /*
             * if (_CFeCanc.InfCFe.Ide.TpAmb == ACBr.Net.DFe.Core.Common.DFeTipoAmbiente.Homologacao)
            {
                _Printer.Separator();
                _Printer.AlignCenter();
                _Printer.BoldMode(PrinterModeState.On);
                _Printer.Append("AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                _Printer.BoldMode(PrinterModeState.Off);
                _Printer.Separator();
            }
            */
            #endregion

            #region Dados do cupom cancelado
            _Printer.Separator();

            _Printer.AlignLeft();
            _Printer.BoldMode("DADOS DO CUPOM FISCAL ELETRONICO CANCELADO");

            _Printer.CondensedMode(PrinterModeState.On);

            _Printer.AppendWithoutLf("CPF/CNPJ do Consumidor: ");
            _Printer.Append(_CFeCanc.InfCFe.Dest?.CPF.IsNotNull() == true ? _CFeCanc.InfCFe.Dest.CPF.FormatoCpfCnpj() :
                            _CFeCanc.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFeCanc.InfCFe.Dest.CNPJ.FormatoCpfCnpj() :
                            "000.000.000-00");
            _Printer.BoldMode(GereneHelpers.TextoEsquerda_Direita("Valor total:", _CFeCanc.InfCFe.Total.VCFe.ToString("C2", Cultura), _Printer.ColsCondensed));

            _Printer.NewLine();

            _Printer.CondensedMode(PrinterModeState.Off);
            
            _Printer.AlignCenter();
            _Printer.Append($"SAT No. {_CFeCanc.InfCFe.Ide.NSerieSAT:D9}");
            _Printer.Append($"Data e Hora {_CFeCanc.InfCFe.Ide.DEmi:dd/MM/yyyy} {_CFeCanc.InfCFe.Ide.HEmi:HH:mm:ss}");
            _Printer.AlignLeft();

            _Printer.CondensedMode(PrinterModeState.On);
            #region Chave de Acesso
            _Printer.NewLine();
            _Printer.AlignCenter();
            _Printer.BoldMode(Regex.Replace(_CFeCanc.InfCFe.ChCanc.OnlyNumber(), ".{4}", "$0 "));

            _Printer.Code128(_CFeCanc.InfCFe.ChCanc.OnlyNumber().Substring(0, 22));
            _Printer.Code128(_CFeCanc.InfCFe.ChCanc.OnlyNumber().Substring(22));

            _Printer.NewLine();
            #endregion

            _Printer.CondensedMode(PrinterModeState.Off);

            #region QrCode
            _Printer.AlignCenter();
            string _qrCode = $"{_CFeCanc.InfCFe.ChCanc.OnlyNumber()}|" +
                             $"{_CFeCanc.InfCFe.Ide.DhEmissao:yyyyMMddHHmmss}|" +
                             $"{_CFeCanc.InfCFe.Ide.DhEmissao:0.00}|" +
                             $"{(_CFeCanc.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFeCanc.InfCFe.Dest.CNPJ : _CFeCanc.InfCFe.Dest.CPF)}|" +
                             $"{_CFeCanc.InfCFe.Ide.AssinaturaQrcode}";

            _Printer.QrCode(_qrCode, QrCodeSize.Size1);

            _Printer.NewLine();
            #endregion

            #endregion

            #region Dados do cupom cancelado
            _Printer.Separator();

            _Printer.AlignLeft();
            _Printer.BoldMode("DADOS DO CUPOM FISCAL ELETRONICO DE CANCELAMENTO");

            _Printer.AlignCenter();
            _Printer.Append($"SAT No. {_CFeCanc.InfCFe.Ide.NSerieSAT:D9}");
            _Printer.Append($"Data e Hora {_CFeCanc.InfCFe.Ide.DEmi:dd/MM/yyyy} {_CFeCanc.InfCFe.Ide.HEmi:HH:mm:ss}");

            #region Chave de Acesso            
            _Printer.AlignCenter();
            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.BoldMode(Regex.Replace(_CFeCanc.InfCFe.Id.OnlyNumber(), ".{4}", "$0 "));

            _Printer.Code128(_CFeCanc.InfCFe.Id.OnlyNumber().Substring(0, 22));
            _Printer.Code128(_CFeCanc.InfCFe.Id.OnlyNumber().Substring(22));

            _Printer.NewLine();
            #endregion

            _Printer.CondensedMode(PrinterModeState.Off);

            #region QrCode
            _Printer.AlignCenter();
            string _qrCodeCancel = $"{_CFeCanc.InfCFe.Id.OnlyNumber()}|" +
                             $"{_CFeCanc.InfCFe.Ide.DhEmissao:yyyyMMddHHmmss}|" +
                             $"{_CFeCanc.InfCFe.Total.VCFe:0.00}|" +
                             $"{(_CFeCanc.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFeCanc.InfCFe.Dest.CNPJ : _CFeCanc.InfCFe.Dest.CPF)}|" +
                             $"{_CFeCanc.InfCFe.Ide.AssinaturaQrcode}";

            _Printer.QrCode(_qrCode, QrCodeSize.Size1);

            _Printer.NewLine();
            #endregion

            #endregion

            _Printer.Separator();
            #endregion

            if (CortarPapel)
                _Printer.PartialPaperCut();

            _Printer.PrintDocument();
        }

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

            if (_CFeCanc != null)
            {
                _CFeCanc = null;
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
