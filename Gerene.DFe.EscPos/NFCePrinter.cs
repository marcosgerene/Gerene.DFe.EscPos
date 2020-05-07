using NFe.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vip.Printer.Enums;
using NotaFiscal = NFe.Classes.nfeProc;

namespace Gerene.DFe.EscPos
{
    public sealed class NFCePrinter : IDfePrinter
    {
        #region IDfe
        public string NomeImpressora { get; set; }
        public PrinterType TipoImpressora { get; set; }        

        public void Imprimir(string xmlcontent)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
        }
        #endregion
    }
}
