using System;
using System.Globalization;
using Vip.Printer.Enums;

namespace Gerene.DFe.EscPos
{
    public interface IDfePrinter : IDisposable
    {
        string NomeImpressora { get; set; }
        PrinterType TipoImpressora { get; set; }
        bool CortarPapel { get; set; }
        bool ProdutoDuasLinhas { get; set; }
        bool UsarBarrasComoCodigo { get; set; }
        bool DocumentoCancelado { get; set; }
        byte[] Logotipo { get; set; }
        CultureInfo Cultura { get; set; }
        /// <summary>
        /// Informar texto completo, ex: "Desenvolvido por: ABC Sistemas"
        /// </summary>
        string Desenvolvedor { get; set; }

        void Imprimir(string xmlcontent);
    }
}
