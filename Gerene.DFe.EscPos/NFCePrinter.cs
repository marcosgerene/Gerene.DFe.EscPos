using DFe.Classes.Flags;
using DFe.Utils;
using NFe.Utils.NFe;
using Shared.DFe.Utils;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Vip.Printer;
using Vip.Printer.Enums;

using NotaFiscal = NFe.Classes.nfeProc;

namespace Gerene.DFe.EscPos
{
    public sealed class NFCePrinter : IDfePrinter
    {
        public NFCePrinter()
        {
            _NFCe = new NotaFiscal();
            NomeDaVia = "Via do Consumidor";            
        }

        #region IDfe
        public string NomeImpressora { get; set; }
        public PrinterType TipoImpressora { get; set; }
        public bool CortarPapel { get; set; }
        public bool ProdutoDuasLinhas { get; set; }
        public bool UsarBarrasComoCodigo { get; set; }
        public bool DocumentoCancelado { get; set; }
        public string NomeDaVia { get; set; }
        public byte[] Logotipo { get; set; }
        public CultureInfo Cultura { get; set; } = new CultureInfo("pt-Br");

        private Printer _Printer { get; set; }
        private NotaFiscal _NFCe { get; set; }

        public void Imprimir(string xmlcontent)
        {
            bool isOffline = false;

            //Fora do estado de SP pode haver a impressão de NFCe offline, ou seja, sem a tag NFeProc
            if (xmlcontent.ToLower().Contains("<nfeproc"))
                _NFCe = new NotaFiscal().CarregarDeXmlString(xmlcontent);
            else
            {
                _NFCe = new NotaFiscal()
                {
                    NFe = new NFe.Classes.NFe().CarregarDeXmlString(xmlcontent),
                    protNFe = new NFe.Classes.Protocolo.protNFe()
                };

                _NFCe.versao = _NFCe.NFe.infNFe.versao;

                isOffline = true;
            }

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
            _Printer.Append(_NFCe.NFe.infNFe.emit.xFant.LimitarString(_Printer.ColsNomal).RemoverAcentos());

            _Printer.AlignLeft();
            _Printer.BoldMode(PrinterModeState.Off);
            _Printer.Append(_NFCe.NFe.infNFe.emit.xNome.LimitarString(_Printer.ColsNomal).RemoverAcentos());

            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.Append(GereneHelpers.TextoEsquerda_Direita($"Cnpj: {_NFCe.NFe.infNFe.emit.CNPJ.FormatoCpfCnpj()}", $"I.E.: {_NFCe.NFe.infNFe.emit.IE}", _Printer.ColsCondensed));

            _Printer.AppendWithoutLf("End.: ");
            _Printer.Append($"{_NFCe.NFe.infNFe.emit.enderEmit.xLgr.RemoverAcentos()},{_NFCe.NFe.infNFe.emit.enderEmit.nro.RemoverAcentos()} {_NFCe.NFe.infNFe.emit.enderEmit.xCpl.RemoverAcentos()}");

            _Printer.AppendWithoutLf("Bairro: ");
            _Printer.Append($"{_NFCe.NFe.infNFe.emit.enderEmit.xBairro.RemoverAcentos()} - {_NFCe.NFe.infNFe.emit.enderEmit.xMun.RemoverAcentos()} - {_NFCe.NFe.infNFe.emit.enderEmit.CEP.FormatoCep()}");

            _Printer.CondensedMode(PrinterModeState.Off);
            _Printer.Separator();
            #endregion

            #region DANFE e Via
            _Printer.AlignCenter();
            _Printer.BoldMode("DANFE NFC-e");
            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.BoldMode("Documento Auxiliar da Nota Fiscal de Consumidor Eletronica");
            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion

            #region Impressão Offline
            if (isOffline)
            {
                _Printer.Separator();
                _Printer.AlignCenter();
                _Printer.BoldMode("*** DOCUMENTO EMITIDO OFFLINE ***");
            }
            #endregion

            #region Homologação
            if (_NFCe.NFe.infNFe.ide.tpAmb == TipoAmbiente.Homologacao)
            {
                _Printer.Separator();
                _Printer.AlignCenter();
                _Printer.BoldMode(PrinterModeState.On);
                _Printer.Append("AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                _Printer.BoldMode(PrinterModeState.Off);
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
            }
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
            foreach (var det in _NFCe.NFe.infNFe.det)
            {
                string codProd = det.prod.cProd;
                if (UsarBarrasComoCodigo)
                    codProd = $"{(UsarBarrasComoCodigo && det.prod.cEAN.IsNotNull() ? det.prod.cEAN : det.prod.cProd).PadRight(13)}";

                string textoE;
                if (ProdutoDuasLinhas)
                    textoE = $"{ det.nItem:D3} | {codProd}";
                else
                    textoE = $"{ det.nItem:D3} | {codProd} {det.prod.xProd}";

                string textoR = $"{det.prod.qCom:N3} {det.prod.uCom} x {det.prod.vUnCom:N2} = {det.prod.vProd:N2}";

                _Printer.Append(GereneHelpers.TextoEsquerda_Direita(textoE, textoR, _Printer.ColsCondensed));

                if (ProdutoDuasLinhas)
                    _Printer.Append(det.prod.xProd.LimitarString(_Printer.ColsCondensed));

                if (det.prod.vOutro.HasValue && det.prod.vOutro.Value > 0)
                    _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Acrescimos:", det.prod.vOutro.Value.ToString("C2", Cultura), _Printer.ColsCondensed));

                if (det.prod.vDesc.HasValue && det.prod.vDesc.Value > 0)
                    _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Descontos:", det.prod.vDesc.Value.ToString("C2", Cultura), _Printer.ColsCondensed));
            }
            _Printer.CondensedMode(PrinterModeState.Off);
            _Printer.Separator();

            #region Totais
            _Printer.BoldMode(PrinterModeState.On);
            _Printer.CondensedMode(PrinterModeState.On);

            _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Qtde. total de itens:", _NFCe.NFe.infNFe.det.Count.ToString("N0", Cultura), _Printer.ColsCondensed));

            _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Subtotal:", _NFCe.NFe.infNFe.total.ICMSTot.vProd.ToString("C2", Cultura), _Printer.ColsCondensed));

            if (_NFCe.NFe.infNFe.total.ICMSTot.vOutro > 0)
                _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Acrescimos:", _NFCe.NFe.infNFe.total.ICMSTot.vOutro.ToString("C2", Cultura), _Printer.ColsCondensed));

            if (_NFCe.NFe.infNFe.total.ICMSTot.vDesc > 0)
                _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Descontos:", _NFCe.NFe.infNFe.total.ICMSTot.vDesc.ToString("C2", Cultura), _Printer.ColsCondensed));

            if (_NFCe.NFe.infNFe.total.ICMSTot.vFrete > 0)
                _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Frete:", _NFCe.NFe.infNFe.total.ICMSTot.vFrete.ToString("C2", Cultura), _Printer.ColsCondensed));

            if (_NFCe.NFe.infNFe.total.ICMSTot.vSeg > 0)
                _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Seguro:", _NFCe.NFe.infNFe.total.ICMSTot.vSeg.ToString("C2", Cultura), _Printer.ColsCondensed));

            if (_NFCe.NFe.infNFe.total.ICMSTot.vDesc > 0)
                _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Descontos:", _NFCe.NFe.infNFe.total.ICMSTot.vDesc.ToString("C2", Cultura), _Printer.ColsCondensed));


            _Printer.CondensedMode(PrinterModeState.Off);

            _Printer.BoldMode(GereneHelpers.TextoEsquerda_Direita("Valor TOTAL:", _NFCe.NFe.infNFe.total.ICMSTot.vNF.ToString("C2", Cultura), _Printer.ColsNomal));

            _Printer.BoldMode(PrinterModeState.Off);
            #endregion
            #endregion

            _Printer.NewLine();
            #endregion

            #region Pagamentos
            _Printer.AlignLeft();

            bool imprimiutroco = false;
            foreach (var _pagto in _NFCe.NFe.infNFe.pag)
            {
                _Printer.CondensedMode(PrinterModeState.On);

                foreach (var _detpagto in _pagto.detPag)
                    _Printer.Append(GereneHelpers.TextoEsquerda_Direita(_detpagto.tPag.Descricao().RemoverAcentos(), _detpagto.vPag.ToString("C2", Cultura), _Printer.ColsCondensed));

                _Printer.CondensedMode(PrinterModeState.Off);

                if (_pagto.vTroco.HasValue && _pagto.vTroco.Value > 0)
                {
                    imprimiutroco = true;
                    _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Troco:", _pagto.vTroco.Value.ToString("C2", Cultura), _Printer.ColsNomal));
                }
            }

            if (!imprimiutroco)
                _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Troco:", 0.ToString("C2", Cultura), _Printer.ColsNomal));

            _Printer.NewLine();
            #endregion

            #region Rodape
            #region Dados da entrega            
            if (_NFCe.NFe.infNFe.entrega != null && !_NFCe.NFe.infNFe.entrega.xLgr.IsNull())
            {
                _Printer.AlignCenter();
                _Printer.BoldMode(PrinterModeState.On);
                _Printer.Append("DADOS PARA ENTREGA");

                _Printer.AlignLeft();
                _Printer.BoldMode(PrinterModeState.Off);
                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.AppendWithoutLf("End.: ");
                _Printer.Append($"{_NFCe.NFe.infNFe.entrega.xLgr.RemoverAcentos()}, {_NFCe.NFe.infNFe.entrega.nro.RemoverAcentos()} {_NFCe.NFe.infNFe.entrega.xCpl.RemoverAcentos()}");
                _Printer.AppendWithoutLf("Bairro: ");
                _Printer.Append($"{_NFCe.NFe.infNFe.entrega.xBairro.RemoverAcentos()} - {_NFCe.NFe.infNFe.entrega.xMun.RemoverAcentos()}/{_NFCe.NFe.infNFe.entrega.UF}");
                _Printer.CondensedMode(PrinterModeState.Off);

                _Printer.Separator();
            }
            #endregion

            #region Observações do Fisco         
            if (_NFCe.NFe.infNFe.infAdic.obsFisco.Any() || _NFCe.NFe.infNFe.infAdic.infAdFisco.IsNotNull())
            {
                _Printer.AlignLeft();
                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.BoldMode("Observacoes do Fisco");

                foreach (var fisco in _NFCe.NFe.infNFe.infAdic.obsFisco)
                {
                    string texto = $"{fisco.xCampo} - {fisco.xTexto}";

                    foreach (var txt in texto.Split(40))
                        _Printer.Append(txt.RemoverAcentos());
                }

                if (_NFCe.NFe.infNFe.infAdic.infAdFisco.IsNotNull())
                    _Printer.Append(_NFCe.NFe.infNFe.infAdic.infAdFisco.RemoverAcentos());

                _Printer.NewLine();

                _Printer.CondensedMode(PrinterModeState.Off);

            }
            #endregion

            #region Observações do Contribuinte          
            _Printer.AlignLeft();
            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.BoldMode("Observacoes do Contribuinte");

            if (_NFCe.NFe.infNFe.infAdic.infCpl.IsNotNull())
                foreach (var txt in _NFCe.NFe.infNFe.infAdic.infCpl.Split(40))
                    _Printer.Append(txt.RemoverAcentos());

            _Printer.NewLine();

            _Printer.CondensedMode(PrinterModeState.Off);

            #endregion

            #region Tributos
            _Printer.AlignLeft();
            _Printer.BoldMode(PrinterModeState.Off);
            _Printer.CondensedMode(PrinterModeState.On);

            _Printer.Append(GereneHelpers.TextoEsquerda_Direita("Valor aproximado dos Tributos deste Cupom", _NFCe.NFe.infNFe.total.ICMSTot.vTotTrib.ToString("C2", Cultura), _Printer.ColsCondensed));
            _Printer.Append("(Conforme Lei Fed. 12.741/2012)");

            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion

            #region Consumidor
            _Printer.Separator();
            if (_NFCe.NFe.infNFe.dest != null)
            {
                _Printer.AlignLeft();
                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.AppendWithoutLf("CPF/CNPJ do Consumidor: ");
                _Printer.Append(_NFCe.NFe.infNFe.dest?.CPF.IsNotNull() == true ? _NFCe.NFe.infNFe.dest.CPF.FormatoCpfCnpj() :
                                _NFCe.NFe.infNFe.dest?.CNPJ.IsNotNull() == true ? _NFCe.NFe.infNFe.dest.CNPJ.FormatoCpfCnpj() :
                                "000.000.000-00");
                _Printer.AppendWithoutLf("Razao Social/Nome: ");
                _Printer.Append((_NFCe.NFe.infNFe.dest?.xNome ?? "CONSUMIDOR").LimitarString(_Printer.ColsCondensed));
                _Printer.CondensedMode(PrinterModeState.Off);
            }
            else
            {
                _Printer.AlignLeft();
                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.BoldMode("CONSUMIDOR NAO IDENTIFICADO");
                _Printer.CondensedMode(PrinterModeState.Off);

            }
            _Printer.Separator();
            #endregion

            #region Número e série do documento
            _Printer.AlignCenter();
            _Printer.BoldMode(PrinterModeState.On);
            _Printer.Append($"No.: {_NFCe.NFe.infNFe.ide.nNF:D9} Serie: {_NFCe.NFe.infNFe.ide.serie:D3}");
            _Printer.Append($"Emissão: {_NFCe.NFe.infNFe.ide.dhEmi:dd/MM/yyyy HH:mm:ss}");
            _Printer.BoldMode(PrinterModeState.Off);
            _Printer.Append(NomeDaVia);

            _Printer.Separator();
            #endregion

            #region Chave de Acesso
            _Printer.CondensedMode(PrinterModeState.On);

            _Printer.AlignCenter();

            _Printer.Append("Consulte pela chave de acesso em:");
            _Printer.Append(_NFCe.NFe.infNFeSupl.urlChave);

            _Printer.Append("Chave de Acesso");
            _Printer.BoldMode(Regex.Replace(_NFCe.NFe.infNFe.Id.OnlyNumber(), ".{4}", "$0 "));

            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion   

            #region QrCode
            if (_NFCe.NFe.infNFeSupl != null && _NFCe.NFe.infNFeSupl.qrCode.IsNotNull())
            {
                _Printer.AlignCenter();
                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.BoldMode("Consulta via leitor de QR Code");
                _Printer.CondensedMode(PrinterModeState.Off);

                _Printer.AlignCenter();
                _Printer.QrCode(_NFCe.NFe.infNFeSupl.qrCode, QrCodeSize.Size1);

                _Printer.NewLine();
            }
            #endregion

            _Printer.AlignCenter();
            _Printer.CondensedMode(PrinterModeState.On);

            _Printer.Append("Protocolo de autorizacao");
            _Printer.Append($"{_NFCe.protNFe.infProt.nProt} {_NFCe.protNFe.infProt.dhRecbto:@dd/MM/yyyy HH:mm:ss}");

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
            if (_NFCe != null)
            {
                _NFCe = null;
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
