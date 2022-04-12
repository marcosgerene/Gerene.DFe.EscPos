using OpenAC.Net.Devices;
using OpenAC.Net.EscPos;
using OpenAC.Net.EscPos.Commom;
using System;
using System.Globalization;
using System.Text;
using OpenAlinhamento = OpenAC.Net.EscPos.Commom.CmdAlinhamento;
using OpenTamanhoFonte = OpenAC.Net.EscPos.Commom.CmdTamanhoFonte;

namespace Gerene.DFe.EscPos
{
    public enum TipoPapel
    {
        Tp58mm = 0,
        Tp80mm = 1
    }

    public enum TipoConexao
    {
        Serial,
        TCP,
        RAW
    }

    public abstract class DfePrinter : IDisposable
    {
        protected enum TpAlinhamento
        {
            Left,
            Center,
            Right
        }

        /// <summary>
        /// Tipo de conexão da impressora: Serial, TCP ou RAW (padrão)
        /// </summary>
        public TipoConexao TipoConexao { get; set; }

        /// <summary>
        /// Esta configuração permite um funcionamento mais simples da ferramenta, dispensando configurar cada tipo de conexão quando busca-se manter a configuração padrão
        ///     - Para conexão SERIAL informe a porta, ex: "COM1"
        ///     - Para conexão TCP informe o IP, ex: "192.168.1.110"
        ///     - Para RAW informe o nome da impressora instalada, ex: "ESPON TM-20"
        /// </summary>
        public string Impressora { get; set; }

        /// <summary>
        /// Protocolo de comunicação com a impressora
        /// </summary>
        public ProtocoloEscPos Protocolo { get; set; }

        /// <summary>
        /// Este atributo é utilizado somente quando o TipoConexao for "RAW"
        /// </summary>
        public RawConfig ConfiguracaoRAW { get; set; }

        /// <summary>
        /// Este atributo é utilizado somente quando o TipoConexao for "TCP"
        /// </summary>
        public TCPConfig ConfiguracaoTCP { get; set; }

        /// <summary>
        /// Este atributo é utilizado somente quando o TipoConexao for "Serial"
        /// </summary>
        public SerialConfig ConfiguracaoSerial { get; set; }

        public bool CortarPapel { get; set; }
        public bool ProdutoDuasLinhas { get; set; }
        public bool UsarBarrasComoCodigo { get; set; }
        public TipoPapel TipoPapel { get; set; }
        public bool DocumentoCancelado { get; set; }
        public string NomeDaVia { get; set; }
        public byte[] Logotipo { get; set; }
        public CultureInfo Cultura { get; set; }
        public int CasasDecimaisQuantidade { get; set; }

        /// <summary>
        /// Informar texto completo, ex: "Desenvolvido por: ABC Sistemas"
        /// </summary>
        public string Desenvolvedor { get; set; }

        /// <summary>
        /// Alguns softwares mandam a estrutura de olho no imposto nas observações, neste caso,
        /// essa opção permite ocultar a informação "automatizada" proposta pela biblioteca
        /// </summary>
        public bool ImprimirDeOlhoNoImposto { get; set; }
        
        public bool RemoverAcentos 
        {
            get => GereneHelpers._RemoverAcento;
            set => GereneHelpers._RemoverAcento = value;
        }
        private int _ColunasNormalDefault
        {
            get
            {
                switch (Protocolo)
                {
                    case ProtocoloEscPos.EscBema: return 50;
                    default: return 48; //Default com base em EscPos
                }
            }
        }
        private int _ColunasCondensadoDefault
        {
            get
            {
                switch (Protocolo)
                {
                    case ProtocoloEscPos.EscBema: return 67;
                    case ProtocoloEscPos.EscDaruma: return 57;
                    default: return 64; //Default com base em EscPos
                }
            }
        }
        private int _ColunasExtendidoDefault
        {
            get
            {
                switch (Protocolo)
                {
                    case ProtocoloEscPos.EscBema: return 25;
                    case ProtocoloEscPos.EscDaruma: return 25;
                    default: return 24; //Default com base em EscPos
                }
            }
        }

        protected int ColunasNormal => TipoPapel == TipoPapel.Tp80mm ? _ColunasNormalDefault : 34;
        protected int ColunasCondensado => TipoPapel == TipoPapel.Tp80mm ? _ColunasCondensadoDefault : 46;
        protected int ColunasExtendido => TipoPapel == TipoPapel.Tp80mm ? _ColunasExtendidoDefault : 17;

        protected EscPosPrinter _Printer { get; set; }

        public DfePrinter()
        {
            TipoConexao = TipoConexao.RAW;
            Protocolo = ProtocoloEscPos.EscPos;
            Impressora = string.Empty;

            CortarPapel = true;
            ProdutoDuasLinhas = false;
            UsarBarrasComoCodigo = false;
            TipoPapel = TipoPapel.Tp80mm;
            DocumentoCancelado = false;
            NomeDaVia = "Via do Consumidor";
            Cultura = new CultureInfo("pt-Br");
            ImprimirDeOlhoNoImposto = true;
            CasasDecimaisQuantidade = 3;

            RemoverAcentos = true;

            ConfiguracaoRAW = new RawConfig();
            ConfiguracaoSerial = new SerialConfig();
            ConfiguracaoTCP = new TCPConfig();
        }

        public virtual void Dispose()
        {
            if (_Printer != null)
            {
                if (_Printer.Conectado)
                {
                    _Printer.Clear();
                    _Printer.Desconectar();
                }

                if (!_Printer.IsDisposed)
                    _Printer.Dispose();

                _Printer = null;
            }
        }

        public virtual void Imprimir(string xmlcontent)
        {           
            if (TipoPapel == TipoPapel.Tp58mm && !ProdutoDuasLinhas)
                throw new ArgumentException("Não é possível usar produto em lina única para 58mm");

            switch (TipoConexao)
            {
                case TipoConexao.RAW:
                    if (Impressora.IsNotNull())
                        ConfiguracaoRAW.Impressora = Impressora;

                    _Printer = new EscPosPrinter<RawConfig>(ConfiguracaoRAW);
                    break;

                case TipoConexao.TCP:
                    if (Impressora.IsNotNull())
                        ConfiguracaoTCP.IP = Impressora;

                    _Printer = new EscPosPrinter<TCPConfig>(ConfiguracaoTCP);
                    break;

                case TipoConexao.Serial:
                    if (Impressora.IsNotNull())
                        ConfiguracaoSerial.Porta = Impressora;

                    _Printer = new EscPosPrinter<SerialConfig>(ConfiguracaoSerial);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _Printer.Protocolo = Protocolo;
            _Printer.Conectar();
        }

        protected void EnviarDados()
        {
            _Printer.Imprimir();

            _Printer.Desconectar();
        }

        protected OpenAlinhamento CentralizadoSeTp80mm => TipoPapel == TipoPapel.Tp80mm ? OpenAlinhamento.Centro : OpenAlinhamento.Esquerda;
        protected void ImprimirSeparador(char _char = '-') => _Printer.ImprimirTexto(string.Empty.PadLeft(ColunasCondensado, _char), OpenTamanhoFonte.Condensada);
    }
}
