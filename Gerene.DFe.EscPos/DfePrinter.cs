using System;
using System.Globalization;

using Vip.Printer;
using Vip.Printer.Enums;

namespace Gerene.DFe.EscPos
{
    public enum TipoPapel
    {
        Tp58mm = 0,
        Tp80mm = 1
    }
    
    public abstract class DfePrinter : IDisposable
    {
        protected enum TpAlinhamento
        {
            Left,
            Center,
            Right
        }

        public string NomeImpressora { get; set; }
        public PrinterType TipoImpressora { get; set; }
        public bool CortarPapel { get; set; }
        public bool ProdutoDuasLinhas { get; set; }
        public bool UsarBarrasComoCodigo { get; set; }
        public TipoPapel TipoPapel { get; set; }
        public bool DocumentoCancelado { get; set; }
        public string NomeDaVia { get; set; }
        public byte[] Logotipo { get; set; }
        public CultureInfo Cultura { get; set; }

        /// <summary>
        /// Informar texto completo, ex: "Desenvolvido por: ABC Sistemas"
        /// </summary>
        public string Desenvolvedor { get; set; }

        /// <summary>
        /// Alguns softwares mandam a estrutura de olho no imposto nas observações, neste caso,
        /// essa opção permite ocultar a informação "automatizada" proposta pela biblioteca
        /// </summary>
        public bool ImprimirDeOlhoNoImposto { get; set; }

        protected int ColunasNormal => TipoPapel == TipoPapel.Tp80mm ? _Printer.ColsNomal : 34;
        protected int ColunasCondensadas => TipoPapel == TipoPapel.Tp80mm ? _Printer.ColsCondensed : 46;
        protected int ColunasExtendida => TipoPapel == TipoPapel.Tp80mm ? _Printer.ColsExpanded : 17;
        protected Printer _Printer { get; set; }

        public DfePrinter()
        {
            TipoImpressora = PrinterType.Epson;
            CortarPapel = true;
            ProdutoDuasLinhas = false;
            UsarBarrasComoCodigo = false;
            TipoPapel = TipoPapel.Tp80mm;
            DocumentoCancelado = false;
            NomeDaVia = "Via do Consumidor";
            Cultura = new CultureInfo("pt-Br");
            ImprimirDeOlhoNoImposto = true;
        }

        public virtual void Dispose()
        {
            if (_Printer != null)
            {
                _Printer.Clear();
                _Printer = null;
            }
        }

        public virtual void Imprimir(string xmlcontent)
        {
            if (TipoPapel == TipoPapel.Tp58mm && !ProdutoDuasLinhas)
                throw new ArgumentException("Não é possível usar produto em lina única para 58mm");

            _Printer = new Printer(NomeImpressora, TipoImpressora);
            _Printer.AlignLeft();
        }

    }
}
