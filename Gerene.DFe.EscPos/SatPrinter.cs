using ACBr.Net.Sat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        private Printer _Printer { get; set; }
        private ACBrSat _ACBrSat { get; set; }
        private CFe _CFe { get; set; }

        public void Imprimir(string xmlcontent)
        {
            _CFe = CFe.Load(new MemoryStream(Encoding.UTF8.GetBytes(xmlcontent)), Encoding.UTF8);

            _Printer = new Printer(NomeImpressora, TipoImpressora);
            _Printer.AlignCenter();




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
