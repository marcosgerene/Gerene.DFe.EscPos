using System;
using System.Collections.Generic;
using System.Text;
using Vip.Printer.Enums;

namespace Gerene.DFe.EscPos
{
    public class SatPrinter : IDfePrinter
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
