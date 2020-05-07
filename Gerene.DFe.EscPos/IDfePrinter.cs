using System;
using Vip.Printer.Enums;

namespace Gerene.DFe.EscPos
{
    public interface IDfePrinter : IDisposable
    {
        string NomeImpressora { get; set; }
        PrinterType TipoImpressora { get; set; }
        bool CortarPapel { get; set; }
        bool ProdutoDuasLinhas { get; set; }
        bool UsarBarrasComoCodio { get; set; }
        byte[] Logotipo { get; set; }

        void Imprimir(string xmlcontent);
    }
}
