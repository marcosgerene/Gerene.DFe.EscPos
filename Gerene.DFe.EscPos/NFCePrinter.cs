using System.Linq;
using System.Text.RegularExpressions;

using DFe.Classes.Flags;
using DFe.Utils;
using NFe.Utils.NFe;
using Shared.DFe.Utils;

using OpenAC.Net.Core.Extensions;

using OpenEstiloFonte = OpenAC.Net.EscPos.Commom.CmdEstiloFonte;
using OpenTamanhoFonte = OpenAC.Net.EscPos.Commom.CmdTamanhoFonte;
using OpenAlinhamento = OpenAC.Net.EscPos.Commom.CmdAlinhamento;
using OpenBarcode = OpenAC.Net.EscPos.Commom.CmdBarcode;
using OpenQrCodeModSize = OpenAC.Net.EscPos.Commom.QrCodeModSize;

using NotaFiscal = NFe.Classes.nfeProc;
using System.Drawing;
using System.IO;

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
                Image imagem;
                using (var ms = new MemoryStream(Logotipo))
                    imagem = Image.FromStream(ms);

                _Printer.ImprimirImagem(imagem, CentralizadoSeTp80mm);
            }
            #endregion

            #region Dados do Emitente
            _Printer.ImprimirTexto(_NFCe.NFe.infNFe.emit.xFant.LimitarString(ColunasNormal).TratarAcento(), CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
            _Printer.ImprimirTexto(_NFCe.NFe.infNFe.emit.xNome.LimitarString(ColunasNormal).TratarAcento());
            _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita($"Cnpj: {_NFCe.NFe.infNFe.emit.CNPJ.FormatoCpfCnpj()}", $"I.E.: {_NFCe.NFe.infNFe.emit.IE}", ColunasCondensado).TratarAcento(), OpenTamanhoFonte.Condensada);
            _Printer.ImprimirTexto($"End.: {_NFCe.NFe.infNFe.emit.enderEmit.xLgr},{_NFCe.NFe.infNFe.emit.enderEmit.nro} {_NFCe.NFe.infNFe.emit.enderEmit.xCpl}".TratarAcento(), OpenTamanhoFonte.Condensada);
            _Printer.ImprimirTexto($"Bairro: {_NFCe.NFe.infNFe.emit.enderEmit.xBairro} - {_NFCe.NFe.infNFe.emit.enderEmit.xMun} - {_NFCe.NFe.infNFe.emit.enderEmit.CEP.FormatoCep()}".TratarAcento(), OpenTamanhoFonte.Condensada);

            ImprimirSeparador();
            #endregion

            #region DANFE e Via
            _Printer.ImprimirTexto("DANFE NFC-e", CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);

            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.ImprimirTexto("Documento Auxiliar da Nota Fiscal de Consumidor Eletrônica".TratarAcento(), OpenTamanhoFonte.Condensada, OpenAlinhamento.Centro, OpenEstiloFonte.Negrito);
            else
            {
                _Printer.ImprimirTexto("Documento Auxiliar da", OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);
                _Printer.ImprimirTexto("Nota Fiscal de Consumidor Eletrônica".TratarAcento(), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);
            }
            #endregion

            #region Impressão Offline
            if (isOffline)
            {
                ImprimirSeparador();

                _Printer.ImprimirTexto("*** DOCUMENTO EMITIDO OFFLINE ***", CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
            }
            #endregion

            #region Homologação
            if (_NFCe.NFe.infNFe.ide.tpAmb == TipoAmbiente.Homologacao)
            {
                ImprimirSeparador();

                if (TipoPapel == TipoPapel.Tp80mm)
                    _Printer.ImprimirTexto("AMBIENTE DE HOMOLOGAÇÃO - SEM VALOR FISCAL".TratarAcento(), OpenAlinhamento.Centro, OpenEstiloFonte.Negrito);
                else
                {
                    _Printer.ImprimirTexto("AMBIENTE DE HOMOLOGAÇÃO".TratarAcento(), OpenEstiloFonte.Negrito);
                    _Printer.ImprimirTexto("SEM VALOR FISCAL", OpenEstiloFonte.Negrito);
                }
            }
            #endregion

            #region Documento Cancelado
            if (DocumentoCancelado)
            {
                ImprimirSeparador();

                _Printer.ImprimirTexto("*** DOCUMENTO CANCELADO ***", CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
            }
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

                _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita(textoE, textoR, ColunasCondensado).TratarAcento(), OpenTamanhoFonte.Condensada);

                if (ProdutoDuasLinhas)
                    _Printer.ImprimirTexto(det.prod.xProd.LimitarString(ColunasCondensado).TratarAcento(), OpenTamanhoFonte.Condensada);

                if (det.prod.vOutro.HasValue && det.prod.vOutro.Value > 0)
                    _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("AcrÉscimos:", det.prod.vOutro.Value.ToString("C2", Cultura), ColunasCondensado).TratarAcento(), OpenTamanhoFonte.Condensada);

                if (det.prod.vDesc.HasValue && det.prod.vDesc.Value > 0)
                    _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Descontos:", det.prod.vDesc.Value.ToString("C2", Cultura), ColunasCondensado).TratarAcento(), OpenTamanhoFonte.Condensada);
            }

            ImprimirSeparador();

            #region Totais
            _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Qtde. total de itens:", _NFCe.NFe.infNFe.det.Count.ToString("N0", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);

            _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Subtotal:", _NFCe.NFe.infNFe.total.ICMSTot.vProd.ToString("C2", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);

            if (_NFCe.NFe.infNFe.total.ICMSTot.vOutro > 0)
                _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Acrescimos:", _NFCe.NFe.infNFe.total.ICMSTot.vOutro.ToString("C2", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);

            if (_NFCe.NFe.infNFe.total.ICMSTot.vDesc > 0)
                _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Descontos:", _NFCe.NFe.infNFe.total.ICMSTot.vDesc.ToString("C2", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);

            if (_NFCe.NFe.infNFe.total.ICMSTot.vFrete > 0)
                _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Frete:", _NFCe.NFe.infNFe.total.ICMSTot.vFrete.ToString("C2", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);

            if (_NFCe.NFe.infNFe.total.ICMSTot.vSeg > 0)
                _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Seguro:", _NFCe.NFe.infNFe.total.ICMSTot.vSeg.ToString("C2", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);

            if (_NFCe.NFe.infNFe.total.ICMSTot.vDesc > 0)
                _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Descontos:", _NFCe.NFe.infNFe.total.ICMSTot.vDesc.ToString("C2", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);

            _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Valor TOTAL:", _NFCe.NFe.infNFe.total.ICMSTot.vNF.ToString("C2", Cultura), ColunasNormal), OpenEstiloFonte.Negrito);
            #endregion
            #endregion

            _Printer.PularLinhas(1);
            #endregion

            #region Pagamentos
            bool imprimiutroco = false;
            foreach (var _pagto in _NFCe.NFe.infNFe.pag)
            {
                foreach (var _detpagto in _pagto.detPag)
                    _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita(_detpagto.tPag.Descricao(), _detpagto.vPag.ToString("C2", Cultura), ColunasCondensado).TratarAcento(), OpenTamanhoFonte.Condensada);

                if (_pagto.vTroco.HasValue && _pagto.vTroco.Value > 0)
                {
                    imprimiutroco = true;
                    _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Troco:", _pagto.vTroco.Value.ToString("C2", Cultura).TratarAcento(), ColunasNormal));
                }
            }

            if (!imprimiutroco)
                _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Troco:", 0.ToString("C2", Cultura).TratarAcento(), ColunasNormal));

            _Printer.PularLinhas(1);
            #endregion

            #region Rodape
            #region Dados da entrega            
            if (_NFCe.NFe.infNFe.entrega != null && !_NFCe.NFe.infNFe.entrega.xLgr.IsNull())
            {
                _Printer.ImprimirTexto("DADOS PARA ENTREGA", CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
                _Printer.ImprimirTexto($"End.: {_NFCe.NFe.infNFe.entrega.xLgr}, {_NFCe.NFe.infNFe.entrega.nro} {_NFCe.NFe.infNFe.entrega.xCpl}".TratarAcento(), OpenTamanhoFonte.Condensada);
                _Printer.ImprimirTexto($"Bairro: {_NFCe.NFe.infNFe.entrega.xBairro} - {_NFCe.NFe.infNFe.entrega.xMun}/{_NFCe.NFe.infNFe.entrega.UF}".TratarAcento(), OpenTamanhoFonte.Condensada);

                ImprimirSeparador();
            }
            #endregion

            #region Observações do Fisco         
            if (_NFCe.NFe.infNFe.infAdic != null && (_NFCe.NFe.infNFe.infAdic.obsFisco.Any() || _NFCe.NFe.infNFe.infAdic.infAdFisco.IsNotNull()))
            {
                _Printer.ImprimirTexto("Observações do Fisco".TratarAcento(), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);

                foreach (var fisco in _NFCe.NFe.infNFe.infAdic.obsFisco)
                {
                    string texto = $"{fisco.xCampo} - {fisco.xTexto}";

                    foreach (var txt in texto.WrapText(ColunasCondensado))
                        _Printer.ImprimirTexto(txt.TratarAcento(), OpenTamanhoFonte.Condensada);
                }

                if (_NFCe.NFe.infNFe.infAdic.infAdFisco.IsNotNull())
                    _Printer.ImprimirTexto(_NFCe.NFe.infNFe.infAdic.infAdFisco.TratarAcento(), OpenTamanhoFonte.Condensada);

                _Printer.PularLinhas(1);
            }
            #endregion

            #region Observações do Contribuinte          
            _Printer.ImprimirTexto("Observações do Contribuinte".TratarAcento(), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);

            if (_NFCe.NFe.infNFe.infAdic != null && _NFCe.NFe.infNFe.infAdic.infCpl.IsNotNull())
                foreach (var txt in _NFCe.NFe.infNFe.infAdic.infCpl.WrapText(ColunasCondensado))
                    _Printer.ImprimirTexto(txt.TratarAcento(), OpenTamanhoFonte.Condensada);

            _Printer.PularLinhas(1);
            #endregion

            #region Tributos
            if (ImprimirDeOlhoNoImposto)
            {
                _Printer.ImprimirTexto(GereneHelpers.TextoEsquerda_Direita("Valor aproximado dos Tributos deste Cupom".TratarAcento(), _NFCe.NFe.infNFe.total.ICMSTot.vTotTrib.ToString("C2", Cultura), ColunasCondensado), OpenTamanhoFonte.Condensada);
                _Printer.ImprimirTexto("(Conforme Lei Fed. 12.741/2012)".TratarAcento(), OpenTamanhoFonte.Condensada);
            }
            #endregion

            #region Consumidor
            ImprimirSeparador();
            if (_NFCe.NFe.infNFe.dest != null)
            {
                string cpfcnpj = _NFCe.NFe.infNFe.dest?.CPF.IsNotNull() == true ? _NFCe.NFe.infNFe.dest.CPF.FormatoCpfCnpj() :
                                _NFCe.NFe.infNFe.dest?.CNPJ.IsNotNull() == true ? _NFCe.NFe.infNFe.dest.CNPJ.FormatoCpfCnpj() :
                                "000.000.000-00";

                _Printer.ImprimirTexto($"CPF/CNPJ do Consumidor: {cpfcnpj}", OpenTamanhoFonte.Condensada);
                _Printer.ImprimirTexto($"Razão Social/Nome: {(_NFCe.NFe.infNFe.dest?.xNome ?? "CONSUMIDOR")}".LimitarString(ColunasCondensado).TratarAcento(), OpenTamanhoFonte.Condensada);
            }

            else
                _Printer.ImprimirTexto("CONSUMIDOR NÃO IDENTIFICADO".TratarAcento(), OpenTamanhoFonte.Condensada, CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);

            ImprimirSeparador();
            #endregion

            #region Número e série do documento
            _Printer.ImprimirTexto($"No.: {_NFCe.NFe.infNFe.ide.nNF:D9} Serie: {_NFCe.NFe.infNFe.ide.serie:D3}", CentralizadoSeTp80mm);
            _Printer.ImprimirTexto($"Emissão: {_NFCe.NFe.infNFe.ide.dhEmi:dd/MM/yyyy HH:mm:ss}".TratarAcento(), CentralizadoSeTp80mm);

            ImprimirSeparador();
            #endregion

            #region Chave de Acesso
            _Printer.ImprimirTexto("Consulte pela chave de acesso em:", OpenTamanhoFonte.Condensada, CentralizadoSeTp80mm);
            _Printer.ImprimirTexto(_NFCe.NFe.infNFeSupl.urlChave, OpenTamanhoFonte.Condensada, CentralizadoSeTp80mm);

            _Printer.ImprimirTexto("Chave de Acesso", OpenTamanhoFonte.Condensada, CentralizadoSeTp80mm);

            string chave = Regex.Replace(_NFCe.NFe.infNFe.Id.OnlyNumber(), ".{4}", "$0 ");
            if (TipoPapel == TipoPapel.Tp80mm)
                _Printer.ImprimirTexto(chave, OpenTamanhoFonte.Condensada, CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
            else
            {
                _Printer.ImprimirTexto(chave.Substring(0, 24).Trim(), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);
                _Printer.ImprimirTexto(chave.Substring(24).Trim(), OpenTamanhoFonte.Condensada, OpenEstiloFonte.Negrito);
            }
            #endregion   

            #region QrCode
            if (_NFCe.NFe.infNFeSupl != null && _NFCe.NFe.infNFeSupl.qrCode.IsNotNull())
            {
                _Printer.ImprimirTexto("Consulta via leitor de QR Code", OpenTamanhoFonte.Condensada, CentralizadoSeTp80mm, OpenEstiloFonte.Negrito);
                
                _Printer.ImprimirQrCode(_NFCe.NFe.infNFeSupl.qrCode, aAlinhamento: CentralizadoSeTp80mm, tamanho: TipoPapel == TipoPapel.Tp80mm ? OpenQrCodeModSize.Normal : OpenQrCodeModSize.Pequeno);

                _Printer.PularLinhas(1);
            }
            #endregion

            _Printer.ImprimirTexto("Protocolo de autorização".TratarAcento(), OpenTamanhoFonte.Condensada, CentralizadoSeTp80mm);
            _Printer.ImprimirTexto($"{_NFCe.protNFe.infProt.nProt} {_NFCe.protNFe.infProt.dhRecbto:dd/MM/yyyy HH:mm:ss}", OpenTamanhoFonte.Condensada, CentralizadoSeTp80mm);
            #endregion

            #region Desenvolvedor
            if (Desenvolvedor.IsNotNull())
            {
                _Printer.PularLinhas(1);
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
