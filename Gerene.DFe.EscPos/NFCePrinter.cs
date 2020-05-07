using DFe.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        }

        #region IDfe
        public string NomeImpressora { get; set; }
        public PrinterType TipoImpressora { get; set; }
        public bool CortarPapel { get; set; }
        public bool ProdutoDuasLinhas { get; set; }
        public bool UsarBarrasComoCodio { get; set; }
        public byte[] Logotipo { get; set; }

        private Printer _Printer { get; set; }
        private NotaFiscal _NFCe { get; set; }

        public void Imprimir(string xmlcontent)
        {
            _NFCe = FuncoesXml.XmlStringParaClasse<NotaFiscal>(xmlcontent);

            _Printer = new Printer(NomeImpressora, TipoImpressora);

            
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
