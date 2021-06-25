using System.Linq;
using System.Text.RegularExpressions;

using DFe.Classes.Flags;
using DFe.Utils;
using NFe.Utils.NFe;
using Shared.DFe.Utils;

using Vip.Printer;
using Vip.Printer.Enums;

using NotaFiscal = NFe.Classes.nfeProc;

namespace Gerene.DFe.EscPos
{
    public sealed class NFCePrinter : DfePrinter
    {
        #region Construtor
        public NFCePrinter() : base()
        {
            _NFCe = new NotaFiscal();
        }
        #endregion

        #region Atributos
        private NotaFiscal _NFCe { get; set; }
        #endregion

        #region IDisposable
        public override void Dispose()
        {
            base.Dispose();

            if (_NFCe != null)
            {
                _NFCe = null;
            }
        }
        #endregion

        public override void Imprimir(string xmlcontent)
        {
            base.Imprimir(xmlcontent);

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
            _Printer.WriteLine(_NFCe.NFe.infNFe.emit.xFant.LimitarString(ColunasNormal).RemoverAcentos());

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignLeft();

            _Printer.BoldMode(PrinterModeState.Off);
            _Printer.WriteLine(_NFCe.NFe.infNFe.emit.xNome.LimitarString(ColunasNormal).RemoverAcentos());

            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita($"Cnpj: {_NFCe.NFe.infNFe.emit.CNPJ.FormatoCpfCnpj()}", $"I.E.: {_NFCe.NFe.infNFe.emit.IE}", ColunasCondensadas));

            _Printer.Write("End.: ");
            _Printer.WriteLine($"{_NFCe.NFe.infNFe.emit.enderEmit.xLgr.RemoverAcentos()},{_NFCe.NFe.infNFe.emit.enderEmit.nro.RemoverAcentos()} {_NFCe.NFe.infNFe.emit.enderEmit.xCpl.RemoverAcentos()}");

            _Printer.Write("Bairro: ");
            _Printer.WriteLine($"{_NFCe.NFe.infNFe.emit.enderEmit.xBairro.RemoverAcentos()} - {_NFCe.NFe.infNFe.emit.enderEmit.xMun.RemoverAcentos()} - {_NFCe.NFe.infNFe.emit.enderEmit.CEP.FormatoCep()}");

            _Printer.CondensedMode(PrinterModeState.Off);

            _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));

            #endregion

            #region DANFE e Via
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.BoldMode("DANFE NFC-e");
            _Printer.CondensedMode(PrinterModeState.On);

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.BoldMode("Documento Auxiliar da Nota Fiscal de Consumidor Eletronica");
            else
            {
                _Printer.BoldMode("Documento Auxiliar da");
                _Printer.BoldMode("Nota Fiscal de Consumidor Eletronica");
            }

            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion

            #region Impressão Offline
            if (isOffline)
            {
                _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));

                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignCenter();

                _Printer.BoldMode("*** DOCUMENTO EMITIDO OFFLINE ***");
            }
            #endregion

            #region Homologação
            if (_NFCe.NFe.infNFe.ide.tpAmb == TipoAmbiente.Homologacao)
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
            }
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

                string textoR = $"{det.prod.qCom.ToString($"N{CasasDecimaisQuantidade}")} {det.prod.uCom} x {det.prod.vUnCom:N2} = {det.prod.vProd:N2}";

                _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita(textoE, textoR, ColunasCondensadas));

                if (ProdutoDuasLinhas)
                    _Printer.WriteLine(det.prod.xProd.LimitarString(ColunasCondensadas));

                if (det.prod.vOutro.HasValue && det.prod.vOutro.Value > 0)
                    _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Acrescimos:", det.prod.vOutro.Value.ToString("C2", Cultura), ColunasCondensadas));

                if (det.prod.vDesc.HasValue && det.prod.vDesc.Value > 0)
                    _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Descontos:", det.prod.vDesc.Value.ToString("C2", Cultura), ColunasCondensadas));
            }
            _Printer.CondensedMode(PrinterModeState.Off);
            _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));

            #region Totais
            _Printer.BoldMode(PrinterModeState.On);
            _Printer.CondensedMode(PrinterModeState.On);

            _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Qtde. total de itens:", _NFCe.NFe.infNFe.det.Count.ToString("N0", Cultura), ColunasCondensadas));

            _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Subtotal:", _NFCe.NFe.infNFe.total.ICMSTot.vProd.ToString("C2", Cultura), ColunasCondensadas));

            if (_NFCe.NFe.infNFe.total.ICMSTot.vOutro > 0)
                _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Acrescimos:", _NFCe.NFe.infNFe.total.ICMSTot.vOutro.ToString("C2", Cultura), ColunasCondensadas));

            if (_NFCe.NFe.infNFe.total.ICMSTot.vDesc > 0)
                _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Descontos:", _NFCe.NFe.infNFe.total.ICMSTot.vDesc.ToString("C2", Cultura), ColunasCondensadas));

            if (_NFCe.NFe.infNFe.total.ICMSTot.vFrete > 0)
                _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Frete:", _NFCe.NFe.infNFe.total.ICMSTot.vFrete.ToString("C2", Cultura), ColunasCondensadas));

            if (_NFCe.NFe.infNFe.total.ICMSTot.vSeg > 0)
                _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Seguro:", _NFCe.NFe.infNFe.total.ICMSTot.vSeg.ToString("C2", Cultura), ColunasCondensadas));

            if (_NFCe.NFe.infNFe.total.ICMSTot.vDesc > 0)
                _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Descontos:", _NFCe.NFe.infNFe.total.ICMSTot.vDesc.ToString("C2", Cultura), ColunasCondensadas));


            _Printer.CondensedMode(PrinterModeState.Off);

            _Printer.BoldMode(GereneHelpers.TextoEsquerda_Direita("Valor TOTAL:", _NFCe.NFe.infNFe.total.ICMSTot.vNF.ToString("C2", Cultura), ColunasNormal));

            _Printer.BoldMode(PrinterModeState.Off);
            #endregion
            #endregion

            _Printer.NewLine();
            #endregion

            #region Pagamentos
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignLeft();

            bool imprimiutroco = false;
            foreach (var _pagto in _NFCe.NFe.infNFe.pag)
            {
                _Printer.CondensedMode(PrinterModeState.On);

                foreach (var _detpagto in _pagto.detPag)
                    _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita(_detpagto.tPag.Descricao().RemoverAcentos(), _detpagto.vPag.ToString("C2", Cultura), ColunasCondensadas));

                _Printer.CondensedMode(PrinterModeState.Off);

                if (_pagto.vTroco.HasValue && _pagto.vTroco.Value > 0)
                {
                    imprimiutroco = true;
                    _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Troco:", _pagto.vTroco.Value.ToString("C2", Cultura), ColunasNormal));
                }
            }

            if (!imprimiutroco)
                _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Troco:", 0.ToString("C2", Cultura), ColunasNormal));

            _Printer.NewLine();
            #endregion

            #region Rodape
            #region Dados da entrega            
            if (_NFCe.NFe.infNFe.entrega != null && !_NFCe.NFe.infNFe.entrega.xLgr.IsNull())
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
                _Printer.WriteLine($"{_NFCe.NFe.infNFe.entrega.xLgr.RemoverAcentos()}, {_NFCe.NFe.infNFe.entrega.nro.RemoverAcentos()} {_NFCe.NFe.infNFe.entrega.xCpl.RemoverAcentos()}");
                _Printer.Write("Bairro: ");
                _Printer.WriteLine($"{_NFCe.NFe.infNFe.entrega.xBairro.RemoverAcentos()} - {_NFCe.NFe.infNFe.entrega.xMun.RemoverAcentos()}/{_NFCe.NFe.infNFe.entrega.UF}");
                _Printer.CondensedMode(PrinterModeState.Off);

                _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));
            }
            #endregion

            #region Observações do Fisco         
            if (_NFCe.NFe.infNFe.infAdic != null && (_NFCe.NFe.infNFe.infAdic.obsFisco.Any() || _NFCe.NFe.infNFe.infAdic.infAdFisco.IsNotNull()))
            {
                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignLeft();

                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.BoldMode("Observacoes do Fisco");

                foreach (var fisco in _NFCe.NFe.infNFe.infAdic.obsFisco)
                {
                    string texto = $"{fisco.xCampo} - {fisco.xTexto}";

                    foreach (var txt in texto.WrapText(ColunasCondensadas))
                        _Printer.WriteLine(txt.RemoverAcentos());
                }

                if (_NFCe.NFe.infNFe.infAdic.infAdFisco.IsNotNull())
                    _Printer.WriteLine(_NFCe.NFe.infNFe.infAdic.infAdFisco.RemoverAcentos());

                _Printer.NewLine();

                _Printer.CondensedMode(PrinterModeState.Off);

            }
            #endregion

            #region Observações do Contribuinte          
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignLeft();

            _Printer.CondensedMode(PrinterModeState.On);
            _Printer.BoldMode("Observacoes do Contribuinte");

            if (_NFCe.NFe.infNFe.infAdic != null && _NFCe.NFe.infNFe.infAdic.infCpl.IsNotNull())
                foreach (var txt in _NFCe.NFe.infNFe.infAdic.infCpl.WrapText(ColunasCondensadas))
                    _Printer.WriteLine(txt.RemoverAcentos());

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

                _Printer.WriteLine(GereneHelpers.TextoEsquerda_Direita("Valor aproximado dos Tributos deste Cupom", _NFCe.NFe.infNFe.total.ICMSTot.vTotTrib.ToString("C2", Cultura), ColunasCondensadas));
                _Printer.WriteLine("(Conforme Lei Fed. 12.741/2012)");

                _Printer.CondensedMode(PrinterModeState.Off);
            }
            #endregion

            #region Consumidor
            _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));
            if (_NFCe.NFe.infNFe.dest != null)
            {
                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignLeft();

                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.Write("CPF/CNPJ do Consumidor: ");
                _Printer.WriteLine(_NFCe.NFe.infNFe.dest?.CPF.IsNotNull() == true ? _NFCe.NFe.infNFe.dest.CPF.FormatoCpfCnpj() :
                                _NFCe.NFe.infNFe.dest?.CNPJ.IsNotNull() == true ? _NFCe.NFe.infNFe.dest.CNPJ.FormatoCpfCnpj() :
                                "000.000.000-00");
                _Printer.Write("Razao Social/Nome: ");


                _Printer.WriteLine((_NFCe.NFe.infNFe.dest?.xNome ?? "CONSUMIDOR").LimitarString(ColunasCondensadas));
                _Printer.CondensedMode(PrinterModeState.Off);
            }
            else
            {
                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignLeft();

                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.BoldMode("CONSUMIDOR NAO IDENTIFICADO");
                _Printer.CondensedMode(PrinterModeState.Off);

            }
            _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));
            #endregion

            #region Número e série do documento
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.BoldMode(PrinterModeState.On);
            _Printer.WriteLine($"No.: {_NFCe.NFe.infNFe.ide.nNF:D9} Serie: {_NFCe.NFe.infNFe.ide.serie:D3}");
            _Printer.WriteLine($"Emissao: {_NFCe.NFe.infNFe.ide.dhEmi:dd/MM/yyyy HH:mm:ss}");
            _Printer.BoldMode(PrinterModeState.Off);
            _Printer.WriteLine(NomeDaVia);

            _Printer.CondensedMode(string.Empty.PadLeft(ColunasCondensadas, '-'));
            #endregion

            #region Chave de Acesso
            _Printer.CondensedMode(PrinterModeState.On);

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.WriteLine("Consulte pela chave de acesso em:");
            _Printer.WriteLine(_NFCe.NFe.infNFeSupl.urlChave);

            _Printer.WriteLine("Chave de Acesso");

            string chave = Regex.Replace(_NFCe.NFe.infNFe.Id.OnlyNumber(), ".{4}", "$0 ");
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.BoldMode(chave);
            else
            {
                _Printer.BoldMode(chave.Substring(0, 24).Trim());
                _Printer.BoldMode(chave.Substring(24).Trim());
            }

            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion   

            #region QrCode
            if (_NFCe.NFe.infNFeSupl != null && _NFCe.NFe.infNFeSupl.qrCode.IsNotNull())
            {
                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignCenter();

                _Printer.CondensedMode(PrinterModeState.On);
                _Printer.BoldMode("Consulta via leitor de QR Code");
                _Printer.CondensedMode(PrinterModeState.Off);

                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignCenter();

                _Printer.QrCode(_NFCe.NFe.infNFeSupl.qrCode, QrCodeSize.Size1);

                _Printer.NewLine();
            }
            #endregion

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.AlignCenter();

            _Printer.CondensedMode(PrinterModeState.On);

            _Printer.WriteLine("Protocolo de autorizacao");
            _Printer.WriteLine($"{_NFCe.protNFe.infProt.nProt} {_NFCe.protNFe.infProt.dhRecbto:@dd/MM/yyyy HH:mm:ss}");

            _Printer.CondensedMode(PrinterModeState.Off);
            #endregion

            #region Desenvolvedor
            if (Desenvolvedor.IsNotNull())
            {
                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.AlignRight();

                _Printer.CondensedMode(Desenvolvedor);
            }
            #endregion

            if (CortarPapel)
                _Printer.PartialPaperCut();

            _Printer.PrintDocument();
        }

    }
}
