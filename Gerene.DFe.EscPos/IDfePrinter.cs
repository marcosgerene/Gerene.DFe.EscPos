using System;
using Vip.Printer.Enums;

namespace Gerene.DFe.EscPos
{
    public interface IDfePrinter : IDisposable
    {
        string NomeImpressora { get; set; }
        PrinterType TipoImpressora { get; set; }
        bool CortarPapel { get; set; }

        void Imprimir(string xmlcontent);
    }
}
