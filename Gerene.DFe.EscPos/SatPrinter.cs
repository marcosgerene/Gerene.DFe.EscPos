using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ACBr.Net.Core.Extensions;
using ACBr.Net.Sat;

using Vip.Printer;
using Vip.Printer.Enums;

namespace Gerene.DFe.EscPos
{
    public class SatPrinter : DfePrinter
    {
        #region Construtor
        public SatPrinter()
        {
            _ACBrSat = new ACBrSat();
            _CFe = new CFe();
            _CFeCanc = new CFeCanc();
        }
        #endregion

        #region Atributos       
        private ACBrSat _ACBrSat { get; set; }
        private CFe _CFe { get; set; }
        private CFeCanc _CFeCanc { get; set; }
        #endregion

        #region IDisposable
        public override void Dispose()
        {
            base.Dispose();

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
        }
        #endregion

        public override void Imprimir(string xmlcontent)
        {
            base.Imprimir(xmlcontent);

            _CFe = CFe.Load(new MemoryStream(Encoding.UTF8.GetBytes(xmlcontent)), Encoding.UTF8);

            #region Cabeçalho

            #region Logotipo
            if (Logotipo != null)
            {
                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignCenter();

                _Printer.Image(Logotipo);
            }
            #endregion

            #region Dados do Emitente
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.BoldMode(PrinterModeState.On);
            if (_CFe.InfCFe.Emit.XFant.IsNotNull())
                _Printer.WriteLine(_CFe.InfCFe.Emit.XFant.LimitarString(ColunasNormal).RemoveAccent());
            else
                _Printer.WriteLine(_CFe.InfCFe.Emit.XNome.LimitarString(ColunasNormal).RemoveAccent());

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignLeft();

            _Printer.BoldMode(PrinterModeState.Off);
            _Printer.WriteLine(_CFe.InfCFe.Emit.XNome.LimitarString(ColunasNormal).RemoveAccent());

            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita($"Cnpj: {_CFe.InfCFe.Emit.CNPJ.FormatoCpfCnpj()}", $"I.E.: {_CFe.InfCFe.Emit.IE}", ColunasCondensadas));

            _Printer.Write("End.: ");
            _Printer.WriteLine($"{_CFe.InfCFe.Emit.EnderEmit.XLgr.RemoveAccent()},{_CFe.InfCFe.Emit.EnderEmit.Nro.RemoveAccent()} {_CFe.InfCFe.Emit.EnderEmit.XCpl.RemoveAccent()}");

            _Printer.Write("Bairro: ");
            _Printer.WriteLine($"{_CFe.InfCFe.Emit.EnderEmit.XBairro.RemoveAccent()} - {_CFe.InfCFe.Emit.EnderEmit.XMun.RemoveAccent()} - {_CFe.InfCFe.Emit.EnderEmit.CEP.FormatoCep()}");

            _Printer.CondensedMode(PrinterModeState.Off);
            _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));
            #endregion

            #region Número do extrato
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.BoldMode(PrinterModeState.On);
            _Printer.WriteLine($"Extrato No. {_CFe.InfCFe.Ide.NCFe:D6}");
            _Printer.WriteLine($"CUPOM FISCAL ELETRONICO SAT");
            #endregion

            #region Homologação
            if (_CFe.InfCFe.Ide.TpAmb == ACBr.Net.DFe.Core.Common.DFeTipoAmbiente.Homologacao)
            {
                _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));

                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignCenter();

                _Printer.BoldMode(PrinterModeState.On);

                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.WriteLine("AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                else
                {
                    _Printer.WriteLine("AMBIENTE DE HOMOLOGACAO");
                    _Printer.WriteLine("SEM VALOR FISCAL");
                }

                _Printer.BoldMode(PrinterModeState.Off);
                _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));
            }
            #endregion

            #region Documento Cancelado
            if (DocumentoCancelado)
            {
                _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));

                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignCenter();

                _Printer.BoldMode(PrinterModeState.On);
                _Printer.WriteLine("*** DOCUMENTO CANCELADO ***");
                _Printer.BoldMode(PrinterModeState.Off);
                _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));
            }
            #endregion

            #region Consumidor
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignLeft();

            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.Write("CPF/CNPJ do Consumidor: ");
            _Printer.WriteLine(_CFe.InfCFe.Dest?.CPF.IsNotNull() == true ? _CFe.InfCFe.Dest.CPF.FormatoCpfCnpj() :
                            _CFe.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFe.InfCFe.Dest.CNPJ.FormatoCpfCnpj() :
                            "000.000.000-00");
            _Printer.Write("Razao Social/Nome: ");
            _Printer.WriteLine((_CFe.InfCFe.Dest?.Nome ?? "CONSUMIDOR").LimitarString(ColunasCondensadas));
            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion

            _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));
            #endregion

            #region Detalhes
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.BoldMode("#|COD|DESC|QTD|UN|VL UN|DESC|VL ITEM");
            else
                _Printer.BoldMode("COD|DESC|QTD|UN|VL UN|DESC|VL ITEM");

            _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));

            #region Produtos
            if (TipoPapel == TipoPapel.Tp80mm)
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

                _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita(textoE, textoR, ColunasCondensadas));

                if (ProdutoDuasLinhas)
                    _Printer.WriteLine(det.Prod.XProd.LimitarString(ColunasCondensadas));

                if (det.Prod.VOutro > 0)
                    _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Acrescimos:", det.Prod.VDesc.ToString("C2", Cultura), ColunasCondensadas));

                if (det.Prod.VDesc > 0)
                    _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Descontos:", det.Prod.VDesc.ToString("C2", Cultura), ColunasCondensadas));
            }
            _Printer.CondensedMode(PrinterModeState.Off);
            _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));

            #region Totais
            _Printer.BoldMode(PrinterModeState.On);
            _Printer.CondensedMode(PrinterModeState.On);

            _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Qtde. total de itens:", _CFe.InfCFe.Det.Count.ToString("N0", Cultura), ColunasCondensadas));

            _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Subtotal:", _CFe.InfCFe.Total.ICMSTot.VProd.ToString("C2", Cultura), ColunasCondensadas));

            if (_CFe.InfCFe.Total.ICMSTot.VOutro > 0)
                _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Acrescimos:", _CFe.InfCFe.Total.ICMSTot.VOutro.ToString("C2", Cultura), ColunasCondensadas));

            if (_CFe.InfCFe.Total.ICMSTot.VDesc > 0)
                _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Descontos:", _CFe.InfCFe.Total.ICMSTot.VDesc.ToString("C2", Cultura), ColunasCondensadas));

            _Printer.CondensedMode(PrinterModeState.Off);


            _Printer.BoldMode(GereneHelpers.TextoEsquerda_Direita("Valor TOTAL:", _CFe.InfCFe.Total.VCFe.ToString("C2", Cultura), ColunasNormal));

            _Printer.BoldMode(PrinterModeState.Off);
            #endregion
            #endregion

            _Printer.NewLine();
            #endregion

            #region Pagamentos
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignLeft();

            _Printer.CondensedMode(PrinterModeState.On);

            foreach (var _pagto in _CFe.InfCFe.Pagto.Pagamentos)
                _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita(_pagto.CMp.GetDescription().RemoveAccent(), _pagto.VMp.ToString("C2", Cultura), ColunasCondensadas));

            _Printer.CondensedMode(PrinterModeState.Off);

            _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Troco:", _CFe.InfCFe.Pagto.VTroco.ToString("C2", Cultura), ColunasNormal));
            _Printer.NewLine();
            #endregion

            #region Rodape
            #region Dados da entrega            
            if (_CFe.InfCFe.Entrega != null && !_CFe.InfCFe.Entrega.XLgr.IsNull())
            {
                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignCenter();

                _Printer.BoldMode(PrinterModeState.On);
                _Printer.WriteLine("DADOS PARA ENTREGA");

                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignLeft();

                _Printer.BoldMode(PrinterModeState.Off);
                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.Write("End.: ");
                _Printer.WriteLine($"{_CFe.InfCFe.Entrega.XLgr.RemoveAccent()}, {_CFe.InfCFe.Entrega.Nro.RemoveAccent()} {_CFe.InfCFe.Entrega.XCpl.RemoveAccent()}");
                _Printer.Write("Bairro: ");
                _Printer.WriteLine($"{_CFe.InfCFe.Entrega.XBairro.RemoveAccent()} - {_CFe.InfCFe.Entrega.XMun.RemoveAccent()}/{_CFe.InfCFe.Entrega.UF}");
                _Printer.CondensedMode(PrinterModeState.Off);

                _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));
            }
            #endregion

            #region Observações do Fisco         
            if (_CFe.InfCFe.InfAdic.ObsFisco.Any())
            {
                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignLeft();

                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.BoldMode("Observacoes do Fisco");

                foreach (var fisco in _CFe.InfCFe.InfAdic.ObsFisco)
                {
                    string texto = $"{fisco.XCampo} - {fisco.XTexto}";

                    foreach (var txt in texto.WrapText(ColunasCondensadas))
                        _Printer.WriteLine(txt.RemoveAccent());
                }

                _Printer.NewLine();

                _Printer.CondensedMode(PrinterModeState.Off);

            }
            #endregion

            #region Observações do Contribuinte          
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignLeft();

            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.BoldMode("Observacoes do Contribuinte");

            if (!_CFe.InfCFe.InfAdic.InfCpl.IsNull())
                foreach (var txt in _CFe.InfCFe.InfAdic.InfCpl.WrapText(ColunasCondensadas))
                    _Printer.WriteLine(txt.RemoveAccent());

            _Printer.NewLine();

            _Printer.CondensedMode(PrinterModeState.Off);

            #endregion

            #region Tributos
            if (ImprimirDeOlhoNoImposto)
            {
                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignLeft();

                _Printer.BoldMode(PrinterModeState.Off);
                _Printer.CondensedMode(PrinterModeState.On);

                _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Valor aproximado dos Tributos deste Cupom", _CFe.InfCFe.Total.VCFeLei12741.ToString("C2", Cultura), ColunasCondensadas));
                _Printer.WriteLine("(Conforme Lei Fed. 12.741/2012)");
                _Printer.CondensedMode(PrinterModeState.Off);

                _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));
            }
            #endregion

            #region Número do extrato
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.WriteLine($"SAT No. {_CFe.InfCFe.Ide.NSerieSAT:D9}");
            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.WriteLine($"Data e Hora {_CFe.InfCFe.Ide.DEmi:dd/MM/yyyy} {_CFe.InfCFe.Ide.HEmi:HH:mm:ss}");
            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion

            #region Chave de Acesso
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.CondensedMode(PrinterModeState.On);
            string chave = Regex.Replace(_CFe.InfCFe.Id.OnlyNumber(), ".{4}", "$0 ");

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.BoldMode(chave);
            else
            {
                _Printer.BoldMode(chave.Substring(0, 24).Trim());
                _Printer.BoldMode(chave.Substring(24).Trim());
            }

            _Printer.Code128(_CFe.InfCFe.Id.OnlyNumber().Substring(0, 22));
            _Printer.Code128(_CFe.InfCFe.Id.OnlyNumber().Substring(22));

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.NewLine();

            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion

            #region QrCode
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            string _qrCode = $"{_CFe.InfCFe.Id.OnlyNumber()}|" +
                             $"{_CFe.InfCFe.Ide.DhEmissao:yyyyMMddHHmmss}|" +
                             $"{_CFe.InfCFe.Total.VCFe:0.00}|" +
                             $"{(_CFe.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFe.InfCFe.Dest.CNPJ : _CFe.InfCFe.Dest.CPF)}|" +
                             $"{_CFe.InfCFe.Ide.AssinaturaQrcode}";

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.QrCode(_qrCode, QrCodeSize.Size1);
            else
                _Printer.QrCode(_qrCode, QrCodeSize.Size0);

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.NewLine();
            #endregion

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.CondensedMode(PrinterModeState.On);

            if (TipoPapel == TipoPapel.Tp80mm)
            {
                _Printer.WriteLine("Consulte o QR Code pelo aplicativo \"De olho na nota\"");
                _Printer.WriteLine("disponível na AppStore (Apple) e PlayStore (Android)");
            }
            else
            {
                _Printer.WriteLine("Consulte o QR Code pelo aplicativo");
                _Printer.WriteLine("\"De olho na nota\" disponível na");
                _Printer.WriteLine("AppStore (Apple) e PlayStore (Android)");
            }

            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion

            if (CortarPapel)
                _Printer.PartialPaperCut();

            _Printer.PrintDocument();
        }

        public void ImprimirCancelamento(string xmlContent)
        {
            base.Imprimir(xmlContent);

            _CFeCanc = CFeCanc.Load(new MemoryStream(Encoding.UTF8.GetBytes(xmlContent)), Encoding.UTF8);

            #region Logotipo
            if (Logotipo != null)
            {
                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignCenter();

                _Printer.Image(Logotipo);
            }
            #endregion

            #region Dados do Emitente
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.BoldMode(PrinterModeState.On);
            _Printer.WriteLine((_CFeCanc.InfCFe.Emit.XFant.IsNotNull() ? _CFeCanc.InfCFe.Emit.XFant : _CFeCanc.InfCFe.Emit.XNome).LimitarString(ColunasNormal).RemoveAccent());

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignLeft();

            _Printer.BoldMode(PrinterModeState.Off);
            _Printer.WriteLine(_CFeCanc.InfCFe.Emit.XNome.LimitarString(ColunasNormal).RemoveAccent());

            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita($"Cnpj: {_CFeCanc.InfCFe.Emit.CNPJ.FormatoCpfCnpj()}", $"I.E.: {_CFeCanc.InfCFe.Emit.IE}", ColunasCondensadas));

            _Printer.Write("End.: ");
            _Printer.WriteLine($"{_CFeCanc.InfCFe.Emit.EnderEmit.XLgr.RemoveAccent()},{_CFeCanc.InfCFe.Emit.EnderEmit.Nro.RemoveAccent()} {_CFeCanc.InfCFe.Emit.EnderEmit.XCpl.RemoveAccent()}");

            _Printer.Write("Bairro: ");
            _Printer.WriteLine($"{_CFeCanc.InfCFe.Emit.EnderEmit.XBairro.RemoveAccent()} - {_CFeCanc.InfCFe.Emit.EnderEmit.XMun.RemoveAccent()} - {_CFeCanc.InfCFe.Emit.EnderEmit.CEP.FormatoCep()}");

            _Printer.CondensedMode(PrinterModeState.Off);
            _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));
            #endregion

            #region Número do extrato
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.BoldMode(PrinterModeState.On);
            _Printer.WriteLine($"Extrato No. {_CFeCanc.InfCFe.Ide.NCFe:D6}");
            _Printer.WriteLine($"CUPOM FISCAL ELETRONICO SAT");
            _Printer.WriteLine($"CANCELAMENTO");
            _Printer.BoldMode(PrinterModeState.Off);
            #endregion

            #region Homologação
            /*
             * if (_CFeCanc.InfCFe.Ide.TpAmb == ACBr.Net.DFe.Core.Common.DFeTipoAmbiente.Homologacao)
            {
                _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));
                
                if (TipoPapel == TipoPapel.Tp80mm)                
                    _Printer.AlignCenter();
                
                _Printer.BoldMode(PrinterModeState.On);
                _Printer.WriteLine("AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                _Printer.BoldMode(PrinterModeState.Off);
                _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));
            }
            */
            #endregion

            #region Dados do cupom cancelado
            _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignLeft();

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.BoldMode("DADOS DO CUPOM FISCAL ELETRONICO CANCELADO");
            else
            {
                _Printer.BoldMode("DADOS DO CUPOM FISCAL");
                _Printer.BoldMode("ELETRONICO CANCELADO");
            }

            _Printer.CondensedMode(PrinterModeState.On);

            _Printer.Write("CPF/CNPJ do Consumidor: ");
            _Printer.WriteLine(_CFeCanc.InfCFe.Dest?.CPF.IsNotNull() == true ? _CFeCanc.InfCFe.Dest.CPF.FormatoCpfCnpj() :
                            _CFeCanc.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFeCanc.InfCFe.Dest.CNPJ.FormatoCpfCnpj() :
                            "000.000.000-00");
            _Printer.BoldMode(GereneHelpers.TextoEsquerda_Direita("Valor total:", _CFeCanc.InfCFe.Total.VCFe.ToString("C2", Cultura), ColunasCondensadas));

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.NewLine();

            _Printer.CondensedMode(PrinterModeState.Off);

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.WriteLine($"SAT No. {_CFeCanc.InfCFe.Ide.NSerieSAT:D9}");
            _Printer.WriteLine($"Data e Hora {_CFeCanc.InfCFe.Ide.DEmi:dd/MM/yyyy} {_CFeCanc.InfCFe.Ide.HEmi:HH:mm:ss}");

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignLeft();

            _Printer.CondensedMode(PrinterModeState.On);
            #region Chave de Acesso
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.NewLine();

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            string chave = Regex.Replace(_CFeCanc.InfCFe.ChCanc.OnlyNumber(), ".{4}", "$0 ");

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.BoldMode(chave);
            else
            {
                _Printer.BoldMode(chave.Substring(0, 24).Trim());
                _Printer.BoldMode(chave.Substring(24).Trim());
            }

            _Printer.Code128(_CFeCanc.InfCFe.ChCanc.OnlyNumber().Substring(0, 24));
            _Printer.Code128(_CFeCanc.InfCFe.ChCanc.OnlyNumber().Substring(24));

            _Printer.NewLine();
            #endregion

            _Printer.CondensedMode(PrinterModeState.Off);

            #region QrCode
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            string _qrCode = $"{_CFeCanc.InfCFe.ChCanc.OnlyNumber()}|" +
                             $"{_CFeCanc.InfCFe.Ide.DhEmissao:yyyyMMddHHmmss}|" +
                             $"{_CFeCanc.InfCFe.Ide.DhEmissao:0.00}|" +
                             $"{(_CFeCanc.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFeCanc.InfCFe.Dest.CNPJ : _CFeCanc.InfCFe.Dest.CPF)}|" +
                             $"{_CFeCanc.InfCFe.Ide.AssinaturaQrcode}";

            _Printer.QrCode(_qrCode, TipoPapel == TipoPapel.Tp80mm ? QrCodeSize.Size1 : QrCodeSize.Size0);

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.NewLine();
            #endregion

            #endregion

            #region Dados do cupom cancelado
            _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignLeft();

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.BoldMode("DADOS DO CUPOM FISCAL ELETRONICO DE CANCELAMENTO");
            else
            {
                _Printer.BoldMode("DADOS DO CUPOM FISCAL");
                _Printer.BoldMode("ELETRONICO DE CANCELAMENTO");
            }

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.WriteLine($"SAT No. {_CFeCanc.InfCFe.Ide.NSerieSAT:D9}");
            _Printer.WriteLine($"Data e Hora {_CFeCanc.InfCFe.Ide.DEmi:dd/MM/yyyy} {_CFeCanc.InfCFe.Ide.HEmi:HH:mm:ss}");

            #region Chave de Acesso            
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.CondensedMode(PrinterModeState.On);

            string chave2 = Regex.Replace(_CFeCanc.InfCFe.Id.OnlyNumber(), ".{4}", "$0 ");

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.BoldMode(chave2);
            else
            {
                _Printer.BoldMode(chave2.Substring(0,24).Trim());
                _Printer.BoldMode(chave2.Substring(24).Trim());
            }

            _Printer.Code128(_CFeCanc.InfCFe.Id.OnlyNumber().Substring(0, 24));
            _Printer.Code128(_CFeCanc.InfCFe.Id.OnlyNumber().Substring(24));

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.NewLine();
            #endregion

            _Printer.CondensedMode(PrinterModeState.Off);

            #region QrCode
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            string _qrCodeCancel = $"{_CFeCanc.InfCFe.Id.OnlyNumber()}|" +
                             $"{_CFeCanc.InfCFe.Ide.DhEmissao:yyyyMMddHHmmss}|" +
                             $"{_CFeCanc.InfCFe.Total.VCFe:0.00}|" +
                             $"{(_CFeCanc.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFeCanc.InfCFe.Dest.CNPJ : _CFeCanc.InfCFe.Dest.CPF)}|" +
                             $"{_CFeCanc.InfCFe.Ide.AssinaturaQrcode}";

            _Printer.QrCode(_qrCodeCancel, TipoPapel == TipoPapel.Tp80mm ? QrCodeSize.Size1 : QrCodeSize.Size0);

            _Printer.NewLine();
            #endregion

            #endregion

            _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));

            #region Desenvolvedor
            if (Desenvolvedor.IsNotNull())
            {
                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignRight();

                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.CondensedMode(Desenvolvedor);
                _Printer.CondensedMode(PrinterModeState.Off);
            }
            #endregion

            if (CortarPapel)
                _Printer.PartialPaperCut();

            _Printer.PrintDocument();
        }

    }
}
