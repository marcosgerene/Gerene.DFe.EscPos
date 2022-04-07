using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using OpenAC.Net.Core.Extensions;
using OpenAC.Net.Sat;

using OpenEstiloFonte = OpenAC.Net.EscPos.Commom.CmdEstiloFonte;
using OpenTamanhoFonte = OpenAC.Net.EscPos.Commom.CmdTamanhoFonte;
using OpenAlinhamento = OpenAC.Net.EscPos.Commom.CmdAlinhamento;
using OpenBarcode = OpenAC.Net.EscPos.Commom.CmdBarcode;
using OpenQrCodeModSize = OpenAC.Net.EscPos.Commom.QrCodeModSize;

namespace Gerene.DFe.EscPos
{
    public class SatPrinter : DfePrinter
    {
        #region Construtor
        public SatPrinter()
        {
            _OpenACSat = new OpenSat();
            _CFe = new CFe();
            _CFeCanc = new CFeCanc();
        }
        #endregion

        #region Atributos       
        private OpenSat _OpenACSat { get; set; }
        private CFe _CFe { get; set; }
        private CFeCanc _CFeCanc { get; set; }
        #endregion

        #region IDisposable
        public override void Dispose()
        {
            base.Dispose();

            if (_OpenACSat != null)
            {
                _OpenACSat.Dispose();
                _OpenACSat = null;
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
                Image imagem;
                using (var ms = new MemoryStream(Logotipo))
                    imagem = Image.FromStream(ms);

                _Printer.ImprimirImagem(imagem, CentralizadoSeTp80mm);
            }
            #endregion

            #region Dados do Emitente
            if (_CFe.InfCFe.Emit.XFant.IsNotNull())
                _Printer.ImprimirTexto(_CFe.InfCFe.Emit.XFant.LimitarString(ColunasNormal).TratarAcento(), CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
            else
                _Printer.ImprimirTexto(_CFe.InfCFe.Emit.XNome.LimitarString(ColunasNormal).TratarAcento(), CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);

            _Printer.ImprimirTexto(_CFe.InfCFe.Emit.XNome.LimitarString(ColunasNormal).TratarAcento());
            _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita($"Cnpj: {_CFe.InfCFe.Emit.CNPJ.FormatoCpfCnpj()}", $"I.E.: {_CFe.InfCFe.Emit.IE}", ColunasCondensado).TratarAcento(), OpenTamanhoFonte.Condensada);
            _Printer.ImprimirTexto($"End.: {_CFe.InfCFe.Emit.EnderEmit.XLgr},{_CFe.InfCFe.Emit.EnderEmit.Nro} {_CFe.InfCFe.Emit.EnderEmit.XCpl}".TratarAcento(), OpenTamanhoFonte.Condensada);
            _Printer.ImprimirTexto($"Bairro: {_CFe.InfCFe.Emit.EnderEmit.XBairro} - {_CFe.InfCFe.Emit.EnderEmit.XMun} - {_CFe.InfCFe.Emit.EnderEmit.CEP.FormatoCep()}".TratarAcento(), OpenTamanhoFonte.Condensada);
            ImprimirSeparador();
            #endregion

            #region Número do extrato
            _Printer.ImprimirTexto($"Extrato No. {_CFe.InfCFe.Ide.NCFe:D6}".TratarAcento(), CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
            _Printer.ImprimirTexto($"CUPOM FISCAL ELETRÔNICO SAT".TratarAcento(), CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
            #endregion

            #region Homologação
            if (_CFe.InfCFe.Ide.TpAmb == OpenAC.Net.DFe.Core.Common.DFeTipoAmbiente.Homologacao)
            {
                ImprimirSeparador();

                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.ImprimirTexto("AMBIENTE DE HOMOLOGAÇÃO - SEM VALOR FISCAL".TratarAcento(), OpenAlinhamento.Centro, OpenEstiloFonte.Negrito);
                else
                {
                    _Printer.ImprimirTexto("AMBIENTE DE HOMOLOGAÇÃO".TratarAcento(), OpenEstiloFonte.Negrito);
                    _Printer.ImprimirTexto("SEM VALOR FISCAL", OpenEstiloFonte.Negrito);
                }

                ImprimirSeparador();
            }
            #endregion

            #region Documento Cancelado
            if (DocumentoCancelado)
            {
                ImprimirSeparador();

                _Printer.ImprimirTexto("*** DOCUMENTO CANCELADO ***", CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);

                ImprimirSeparador();
            }
            #endregion

            #region Consumidor
            string cpfcnpj = _CFe.InfCFe.Dest?.CPF.IsNotNull() == true ? _CFe.InfCFe.Dest.CPF.FormatoCpfCnpj() :
                            _CFe.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFe.InfCFe.Dest.CNPJ.FormatoCpfCnpj() :
                            "000.000.000-00";

            _Printer.ImprimirTexto($"CPF/CNPJ do Consumidor: {cpfcnpj}", OpenTamanhoFonte.Condensada);
            _Printer.ImprimirTexto($"Razão Social/Nome: {(_CFe.InfCFe.Dest?.Nome ?? "CONSUMIDOR")}".TratarAcento().LimitarString(ColunasCondensado), OpenTamanhoFonte.Condensada);
            #endregion

            ImprimirSeparador();
            #endregion

            #region Detalhes
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.ImprimirTexto("#|COD|DESC|QTD|UN|VL UN|DESC|VL ITEM", OpenAlinhamento.Centro, OpenEstiloFonte.Negrito);
            else
                _Printer.ImprimirTexto("COD|DESC|QTD|UN|VL UN|DESC|VL ITEM", OpenEstiloFonte.Negrito);

            ImprimirSeparador();

            #region Produtos
            foreach (var det in _CFe.InfCFe.Det)
            {
                string textoE;

                string codProd = det.Prod.CProd;
                if (UsarBarrasComoCodigo)
                    codProd = $"{(UsarBarrasComoCodigo && det.Prod.CEAN.IsNotNull() ? det.Prod.CEAN : det.Prod.CProd).PadRight(13)}";

                if (ProdutoDuasLinhas)
                    textoE = $"{ det.NItem:D3} | {codProd}";
                else
                    textoE = $"{ det.NItem:D3} | {codProd} {det.Prod.XProd}";

                string textoR = $"{det.Prod.QCom.ToString($"N{CasasDecimaisQuantidade}")} {det.Prod.UCom} x {det.Prod.VUnCom:N2} = {det.Prod.VItem:N2}";

                _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita(textoE, textoR, ColunasCondensado), OpenTamanhoFonte.Condensada);

                if (ProdutoDuasLinhas)
                    _Printer.ImprimirTexto(det.Prod.XProd.LimitarString(ColunasCondensado), OpenTamanhoFonte.Condensada);

                if (det.Prod.VOutro > 0)
                    _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Acréscimos:", det.Prod.VDesc.ToString("C2", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);

                if (det.Prod.VDesc > 0)
                    _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Descontos:", det.Prod.VDesc.ToString("C2", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);
            }

            ImprimirSeparador();

            #region Totais
            _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Qtde. total de itens:", _CFe.InfCFe.Det.Count.ToString("N0", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);
            _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Subtotal:", _CFe.InfCFe.Total.ICMSTot.VProd.ToString("C2", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);

            if (_CFe.InfCFe.Total.ICMSTot.VOutro > 0)
                _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Acréscimos:", _CFe.InfCFe.Total.ICMSTot.VOutro.ToString("C2", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);

            if (_CFe.InfCFe.Total.ICMSTot.VDesc > 0)
                _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Descontos:", _CFe.InfCFe.Total.ICMSTot.VDesc.ToString("C2", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);

            _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Valor TOTAL:", _CFe.InfCFe.Total.VCFe.ToString("C2", Cultura), ColunasNormal), OpenEstiloFonte.Negrito);
            #endregion
            #endregion

            _Printer.PularLinhas(1);
            #endregion

            #region Pagamentos            
            foreach (var _pagto in _CFe.InfCFe.Pagto.Pagamentos)
                _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita(_pagto.CMp.GetDescription(), _pagto.VMp.ToString("C2", Cultura), ColunasCondensado).TratarAcento(), OpenTamanhoFonte.Condensada);

            _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Troco:", _CFe.InfCFe.Pagto.VTroco.ToString("C2", Cultura).TratarAcento(), ColunasNormal));
            _Printer.PularLinhas(1);
            #endregion

            #region Rodape

            #region Dados da entrega            
            if (_CFe.InfCFe.Entrega != null && !_CFe.InfCFe.Entrega.XLgr.IsNull())
            {
                _Printer.ImprimirTexto("DADOS PARA ENTREGA", CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
                _Printer.ImprimirTexto($"End.: {_CFe.InfCFe.Entrega.XLgr}, {_CFe.InfCFe.Entrega.Nro} {_CFe.InfCFe.Entrega.XCpl}".TratarAcento(), OpenTamanhoFonte.Condensada);
                _Printer.ImprimirTexto($"Bairro: {_CFe.InfCFe.Entrega.XBairro} - {_CFe.InfCFe.Entrega.XMun}/{_CFe.InfCFe.Entrega.UF}".TratarAcento(), OpenTamanhoFonte.Condensada);
                ImprimirSeparador();
            }
            #endregion

            #region Observações do Fisco         
            if (_CFe.InfCFe.InfAdic.ObsFisco.Any())
            {
                _Printer.ImprimirTexto("Observações do Fisco".TratarAcento(), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);

                foreach (var fisco in _CFe.InfCFe.InfAdic.ObsFisco)
                {
                    string texto = $"{fisco.XCampo} - {fisco.XTexto}";

                    foreach (var txt in texto.WrapText(ColunasCondensado))
                        _Printer.ImprimirTexto(txt.TratarAcento(), OpenTamanhoFonte.Condensada);
                }

                _Printer.PularLinhas(1);
            }
            #endregion

            #region Observações do Contribuinte          
            _Printer.ImprimirTexto("Observações do Contribuinte".TratarAcento(), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);

            if (!_CFe.InfCFe.InfAdic.InfCpl.IsNull())
                foreach (var txt in _CFe.InfCFe.InfAdic.InfCpl.WrapText(ColunasCondensado))
                    _Printer.ImprimirTexto(txt.TratarAcento(), OpenTamanhoFonte.Condensada);

            _Printer.PularLinhas(1);

            #endregion

            #region Tributos
            if (ImprimirDeOlhoNoImposto)
            {
                _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Valor aproximado dos Tributos deste Cupom", _CFe.InfCFe.Total.VCFeLei12741.ToString("C2", Cultura), ColunasCondensado).TratarAcento(), OpenTamanhoFonte.Condensada);
                _Printer.ImprimirTexto("(Conforme Lei Fed. 12.741/2012)".TratarAcento(), OpenTamanhoFonte.Condensada);

                ImprimirSeparador();
            }
            #endregion

            #region Número do extrato
            _Printer.ImprimirTexto($"SAT No. {_CFe.InfCFe.Ide.NSerieSAT:D9}", CentralizadoSeTp80mm);
            _Printer.ImprimirTexto($"Data e Hora {_CFe.InfCFe.Ide.DEmi:dd/MM/yyyy} {_CFe.InfCFe.Ide.HEmi:HH:mm:ss}", OpenTamanhoFonte.Condensada, CentralizadoSeTp80mm);
            #endregion

            #region Chave de Acesso
            string chave = Regex.Replace(_CFe.InfCFe.Id.OnlyNumber(), ".{4}", "$0 ");

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.ImprimirTexto(chave, OpenTamanhoFonte.Condensada, OpenAlinhamento.Centro, OpenEstiloFonte.Negrito);
            else
            {
                _Printer.ImprimirTexto(chave.Substring(0, 24).Trim(), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);
                _Printer.ImprimirTexto(chave.Substring(24).Trim(), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);
            }

            if (TipoPapel == TipoPapel.Tp80mm)
            {
                _Printer.ImprimirBarcode(_CFe.InfCFe.Id.OnlyNumber().Substring(0, 22), OpenBarcode.CODE128c, OpenAlinhamento.Centro);
                _Printer.ImprimirBarcode(_CFe.InfCFe.Id.OnlyNumber().Substring(22), OpenBarcode.CODE128c, OpenAlinhamento.Centro);
            }
            else
            {
                _Printer.ImprimirBarcode(_CFe.InfCFe.Id.OnlyNumber().Substring(0, 11), OpenBarcode.CODE128c);
                _Printer.ImprimirBarcode(_CFe.InfCFe.Id.OnlyNumber().Substring(11, 11), OpenBarcode.CODE128c);
                _Printer.ImprimirBarcode(_CFe.InfCFe.Id.OnlyNumber().Substring(22, 11), OpenBarcode.CODE128c);
                _Printer.ImprimirBarcode(_CFe.InfCFe.Id.OnlyNumber().Substring(33), OpenBarcode.CODE128c);
            }

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.PularLinhas(1);
            #endregion

            #region QrCode
            string _qrCode = $"{_CFe.InfCFe.Id.OnlyNumber()}|" +
                             $"{_CFe.InfCFe.Ide.DhEmissao:yyyyMMddHHmmss}|" +
                             $"{_CFe.InfCFe.Total.VCFe:0.00}|" +
                             $"{(_CFe.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFe.InfCFe.Dest.CNPJ : _CFe.InfCFe.Dest.CPF)}|" +
                             $"{_CFe.InfCFe.Ide.AssinaturaQrcode}";

            _Printer.ImprimirQrCode(_qrCode, aAlinhamento: CentralizadoSeTp80mm, tamanho: TipoPapel == TipoPapel.Tp80mm ? OpenQrCodeModSize.Normal : OpenQrCodeModSize.Pequeno);

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.PularLinhas(1);
            #endregion

            #region App
            if (TipoPapel == TipoPapel.Tp80mm)
            {
                _Printer.ImprimirTexto("Consulte o QR Code pelo aplicativo \"De olho na nota\"".TratarAcento(), OpenTamanhoFonte.Condensada, OpenAlinhamento.Centro);
                _Printer.ImprimirTexto("disponível na AppStore (Apple) e PlayStore (Android)".TratarAcento(), OpenTamanhoFonte.Condensada, OpenAlinhamento.Centro);
            }
            else
            {
                _Printer.ImprimirTexto("Consulte o QR Code pelo aplicativo".TratarAcento(), OpenTamanhoFonte.Condensada);
                _Printer.ImprimirTexto("\"De olho na nota\" disponível na".TratarAcento(), OpenTamanhoFonte.Condensada);
                _Printer.ImprimirTexto("AppStore (Apple) e PlayStore (Android)".TratarAcento(), OpenTamanhoFonte.Condensada);
            }
            #endregion

            #region Desenvolvedor
            if (Desenvolvedor.IsNotNull())
            {
                _Printer.PularLinhas(1);
                _Printer.ImprimirTexto(Desenvolvedor, TipoPapel == TipoPapel.Tp80mm ? OpenAlinhamento.Direita : OpenAlinhamento.Esquerda);
                _Printer.PularLinhas(1);
            }
            #endregion

            #endregion

            if (CortarPapel)
                _Printer.CortarPapel(true);

            EnviarDados();
        }

        public void ImprimirCancelamento(string xmlContent)
        {
            base.Imprimir(xmlContent);

            _CFeCanc = CFeCanc.Load(new MemoryStream(Encoding.UTF8.GetBytes(xmlContent)), Encoding.UTF8);

            #region Logotipo
            if (Logotipo != null)
            {
                Image imagem;
                using (var ms = new MemoryStream(Logotipo))
                    imagem = Image.FromStream(ms);

                _Printer.ImprimirImagem(imagem, CentralizadoSeTp80mm);
            }
            #endregion

            #region Dados do Emitente
            if (_CFeCanc.InfCFe.Emit.XFant.IsNotNull())
                _Printer.ImprimirTexto(_CFeCanc.InfCFe.Emit.XFant.LimitarString(ColunasNormal), CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
            else
                _Printer.ImprimirTexto(_CFeCanc.InfCFe.Emit.XNome.LimitarString(ColunasNormal), CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);

            _Printer.ImprimirTexto(_CFeCanc.InfCFe.Emit.XNome.LimitarString(ColunasNormal));
            _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita($"Cnpj: {_CFeCanc.InfCFe.Emit.CNPJ.FormatoCpfCnpj()}", $"I.E.: {_CFeCanc.InfCFe.Emit.IE}", ColunasCondensado), OpenTamanhoFonte.Condensada);
            _Printer.ImprimirTexto($"End.: {_CFeCanc.InfCFe.Emit.EnderEmit.XLgr},{_CFeCanc.InfCFe.Emit.EnderEmit.Nro} {_CFeCanc.InfCFe.Emit.EnderEmit.XCpl}", OpenTamanhoFonte.Condensada);
            _Printer.ImprimirTexto($"Bairro: {_CFeCanc.InfCFe.Emit.EnderEmit.XBairro} - {_CFeCanc.InfCFe.Emit.EnderEmit.XMun} - {_CFeCanc.InfCFe.Emit.EnderEmit.CEP.FormatoCep()}", OpenTamanhoFonte.Condensada);
            ImprimirSeparador();
            #endregion

            #region Número do extrato
            _Printer.ImprimirTexto($"Extrato No. {_CFeCanc.InfCFe.Ide.NCFe:D6}", CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
            _Printer.ImprimirTexto($"CUPOM FISCAL ELETRONICO SAT", CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
            _Printer.ImprimirTexto($"CANCELAMENTO", CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
            #endregion

            #region Dados do cupom cancelado
            ImprimirSeparador();

            string cpfcnpj = _CFeCanc.InfCFe.Dest?.CPF.IsNotNull() == true ? _CFeCanc.InfCFe.Dest.CPF.FormatoCpfCnpj() :
                            _CFeCanc.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFeCanc.InfCFe.Dest.CNPJ.FormatoCpfCnpj() :
                            "000.000.000-00";

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.ImprimirTexto("DADOS DO CUPOM FISCAL ELETRONICO CANCELADO", OpenAlinhamento.Centro, OpenEstiloFonte.Negrito);
            else
            {
                _Printer.ImprimirTexto("DADOS DO CUPOM FISCAL", OpenEstiloFonte.Negrito);
                _Printer.ImprimirTexto("ELETRONICO CANCELADO", OpenEstiloFonte.Negrito);
            }

            _Printer.ImprimirTexto($"CPF/CNPJ do Consumidor: {cpfcnpj}", OpenTamanhoFonte.Condensada);
            _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Valor total:", _CFeCanc.InfCFe.Total.VCFe.ToString("C2", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.PularLinhas(1);

            _Printer.ImprimirTexto($"SAT No. {_CFeCanc.InfCFe.Ide.NSerieSAT:D9}", CentralizadoSeTp80mm);
            _Printer.ImprimirTexto($"Data e Hora {_CFeCanc.InfCFe.Ide.DEmi:dd/MM/yyyy} {_CFeCanc.InfCFe.Ide.HEmi:HH:mm:ss}", CentralizadoSeTp80mm);

            #region Chave de Acesso
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.PularLinhas(1);

            string chave = Regex.Replace(_CFeCanc.InfCFe.ChCanc.OnlyNumber(), ".{4}", "$0 ");

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.ImprimirTexto(chave, OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);
            else
            {
                _Printer.ImprimirTexto(chave.Substring(0, 24).Trim(), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);
                _Printer.ImprimirTexto(chave.Substring(24).Trim(), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);
            }

            if (TipoPapel == TipoPapel.Tp80mm)
            {
                _Printer.ImprimirBarcode(_CFeCanc.InfCFe.ChCanc.OnlyNumber().Substring(0, 22), OpenBarcode.CODE128c, OpenAlinhamento.Centro);
                _Printer.ImprimirBarcode(_CFeCanc.InfCFe.ChCanc.OnlyNumber().Substring(22), OpenBarcode.CODE128c, OpenAlinhamento.Centro);
            }
            else
            {
                _Printer.ImprimirBarcode(_CFeCanc.InfCFe.ChCanc.OnlyNumber().Substring(0, 11), OpenBarcode.CODE128c);
                _Printer.ImprimirBarcode(_CFeCanc.InfCFe.ChCanc.OnlyNumber().Substring(11, 11), OpenBarcode.CODE128c);
                _Printer.ImprimirBarcode(_CFeCanc.InfCFe.ChCanc.OnlyNumber().Substring(22, 11), OpenBarcode.CODE128c);
                _Printer.ImprimirBarcode(_CFeCanc.InfCFe.ChCanc.OnlyNumber().Substring(33), OpenBarcode.CODE128c);
            }

            _Printer.PularLinhas(1);
            #endregion

            #region QrCode
            string _qrCode = $"{_CFeCanc.InfCFe.ChCanc.OnlyNumber()}|" +
                             $"{_CFeCanc.InfCFe.Ide.DhEmissao:yyyyMMddHHmmss}|" +
                             $"{_CFeCanc.InfCFe.Ide.DhEmissao:0.00}|" +
                             $"{(_CFeCanc.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFeCanc.InfCFe.Dest.CNPJ : _CFeCanc.InfCFe.Dest.CPF)}|" +
                             $"{_CFeCanc.InfCFe.Ide.AssinaturaQrcode}";

            _Printer.ImprimirQrCode(_qrCode, aAlinhamento: CentralizadoSeTp80mm, tamanho: TipoPapel == TipoPapel.Tp80mm ? OpenQrCodeModSize.Normal : OpenQrCodeModSize.Pequeno);

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.PularLinhas(1);
            #endregion

            #endregion

            #region Dados do cupom cancelado
            ImprimirSeparador();

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.ImprimirTexto("DADOS DO CUPOM FISCAL ELETRONICO DE CANCELAMENTO", OpenEstiloFonte.Negrito);
            else
            {
                _Printer.ImprimirTexto("DADOS DO CUPOM FISCAL", OpenEstiloFonte.Negrito);
                _Printer.ImprimirTexto("ELETRONICO DE CANCELAMENTO", OpenEstiloFonte.Negrito);
            }

            _Printer.ImprimirTexto($"SAT No. {_CFeCanc.InfCFe.Ide.NSerieSAT:D9}", CentralizadoSeTp80mm);
            _Printer.ImprimirTexto($"Data e Hora {_CFeCanc.InfCFe.Ide.DEmi:dd/MM/yyyy} {_CFeCanc.InfCFe.Ide.HEmi:HH:mm:ss}", CentralizadoSeTp80mm);

            #region Chave de Acesso            
            string chave2 = Regex.Replace(_CFeCanc.InfCFe.Id.OnlyNumber(), ".{4}", "$0 ");

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.ImprimirTexto(chave2, OpenTamanhoFonte.Condensada, OpenAlinhamento.Centro, OpenEstiloFonte.Negrito);
            else
            {
                _Printer.ImprimirTexto(chave2.Substring(0, 22).Trim(), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);
                _Printer.ImprimirTexto(chave2.Substring(22).Trim(), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);
            }

            if (TipoPapel == TipoPapel.Tp80mm)
            {
                _Printer.ImprimirBarcode(_CFeCanc.InfCFe.Id.OnlyNumber().Substring(0, 22), OpenBarcode.CODE128c, OpenAlinhamento.Centro);
                _Printer.ImprimirBarcode(_CFeCanc.InfCFe.Id.OnlyNumber().Substring(22), OpenBarcode.CODE128c, OpenAlinhamento.Centro);
            }
            else
            {
                _Printer.ImprimirBarcode(_CFeCanc.InfCFe.Id.OnlyNumber().Substring(0, 11), OpenBarcode.CODE128c);
                _Printer.ImprimirBarcode(_CFeCanc.InfCFe.Id.OnlyNumber().Substring(11, 11), OpenBarcode.CODE128c);
                _Printer.ImprimirBarcode(_CFeCanc.InfCFe.Id.OnlyNumber().Substring(22, 11), OpenBarcode.CODE128c);
                _Printer.ImprimirBarcode(_CFeCanc.InfCFe.Id.OnlyNumber().Substring(33), OpenBarcode.CODE128c);
            }

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.PularLinhas(1);
            #endregion

            #region QrCode
            string _qrCodeCancel = $"{_CFeCanc.InfCFe.Id.OnlyNumber()}|" +
                             $"{_CFeCanc.InfCFe.Ide.DhEmissao:yyyyMMddHHmmss}|" +
                             $"{_CFeCanc.InfCFe.Total.VCFe:0.00}|" +
                             $"{(_CFeCanc.InfCFe.Dest?.CNPJ.IsNotNull() == true ? _CFeCanc.InfCFe.Dest.CNPJ : _CFeCanc.InfCFe.Dest.CPF)}|" +
                             $"{_CFeCanc.InfCFe.Ide.AssinaturaQrcode}";

            _Printer.ImprimirQrCode(_qrCodeCancel, aAlinhamento: CentralizadoSeTp80mm, tamanho: TipoPapel == TipoPapel.Tp80mm ? OpenQrCodeModSize.Normal: OpenQrCodeModSize.Pequeno);

            _Printer.PularLinhas(1);
            #endregion

            #endregion

            ImprimirSeparador();

            #region Desenvolvedor
            if (Desenvolvedor.IsNotNull())
            {
                _Printer.ImprimirTexto(Desenvolvedor, TipoPapel == TipoPapel.Tp80mm ? OpenAlinhamento.Direita : OpenAlinhamento.Esquerda);
                _Printer.PularLinhas(1);
            }
            #endregion

            if (CortarPapel)
                _Printer.CortarPapel(true);

            EnviarDados();
        }
    }
}
