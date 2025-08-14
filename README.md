# Gerene.DFe.EscPos

[![Nuget count](http://img.shields.io/nuget/v/Gerene.DFe.EscPos.svg)](https://www.nuget.org/packages/Gerene.DFe.EscPos)

Impressão em impressora termica para DFes via EscPos e derivados (EscBema, EscDaruma, EscElgin, entre outros) nativo em .NET nos formatos 58 e 80mm.

A partir da versão 2.0.0 atende somente NFCe. Versões anteriores (1.0.X) atendem também ao SAT.

Permite comunicação via RAW (USB), TCP, Serial e FileConfig.


Funcionamento:
----

A biblioteca conta com dois demos escritos em Winforms (Windows) e Avalonia (multi-plataforma).

Exemplo de uso:
```
using (var _printer = new NFCePrinter()) //ou new SatPrinter() para SAT até a versão 1.0.27
{
	_printer.Protocolo = ProtocoloEscPos.EscPos; //Protocolo de comunicação	
	_printer.Impressora = "Nome da impressora"; //Para RAW nome da impressora. Para TCP o IP da impressora na rede. Para serial a porta.
	_printer.CortarPapel = true;
	_printer.ProdutoDuasLinhas = false;
	_printer.UsarBarrasComoCodigo = false;
	_printer.DocumentoCancelado = false; //Exibe tarja "Documento cancelado na impressão"
	_printer.Logotipo = logotipo_em_bytes; //Impressão do logotipo, não obrigatório
	_printer.TipoPapel = TipoPapel.Tp80mm; //ou TipoPapel.Tp58mm
	_printer.QrCodeLateral = true; //Padrão false. Exige Tp80mm e exige modo pagina na impressora.
	
	_printer.TipoConexao = TipoConexao.RAW; //padrão RAW, não obrigatório para conexão USB
	_printer.RemoverAcentos = true; //padrão true, não obrigatório
	
	_printer.Imprimir(string_com_o_conteudo_do_xml); //Imprime o documento fiscal
	
	// para impressão específica do XML de cancelamento de SAT CF-e use:
        //_printer.ImprimirCancelamento(string_com_o_conteudo_do_xml_de_cancelamento);
}
```

QrCode como imagem:
----

A partir da versão 1.0.21 é possível não gerar o QrCode via comando e sim imprimir como imagem. Este processo foi realizado para que impressoras sem suporte a QRCode (como a Gprinter GP-58L) fossem capaz de imprimir o SAT/NFCe.

Caso o atributo ```QrCodeImagem``` não seja informado (padrão) a aplicação continuará enviando o QrCode via comando (o que é indicado para impressoras com suporte!).

Exemplo de uso do ```QrCodeImagem``` usando o projeto QRCoder:

```
string qrcode = _printer.QRCodeTexto(xml);

SixLabors.ImageSharp.Image qrCodeImage;
using (var qrGenerator = new QRCoder.QRCodeGenerator())
using (var qrCodeData = qrGenerator.CreateQrCode(qrcode, QRCoder.QRCodeGenerator.ECCLevel.H))
using (var qrCode = new QRCoder.QRCode(qrCodeData))
    qrCodeImage = qrCode.GetGraphic(3);

using MemoryStream memoryStream = new();
qrCodeImage.SaveAsPng(memoryStream);
_printer.QrCodeImagem = new System.Drawing.Bitmap(memoryStream);
```

*Importante: **Não** usar o parâmetro QrCodeImagem em impressoras com suporte a QrCode. A impressão de imagem é mais lenta e custosa para a impressora quando comparado a QRCode nativo!* 

Dependências:
----

OpenAC.Net.EscPos (motor de impressão) - https://github.com/OpenAC-Net/OpenAC.Net.EscPos

Hercules.NET (ZeusFiscal) (desserialização do xml da NFCe) - [https://github.com/ZeusAutomacao/DFe.NET](https://github.com/Hercules-NET/ZeusFiscal)

OpenAC.Net.Sat (desserialização do xml do SAT) - https://github.com/OpenAC-Net/OpenAC.Net.Sat (para versões 1.0.X).

Change log:
----
2.0.1 - Atualiza a versão do Hercules corrigindo #17 (corresponde à versão legada 1.0.28)<br/>
2.0.0 - Remove impressão do SAT, descontinuado pelo estado de SP (as versões 1.0.X continuarão sendo suportadas pela biblioteca).<br/>

1.0.28 - Corresponde à versão 2.0.1.
1.0.27 - Se o cEAN for SEM GTIN, mesmo que esteja configurado para usar Barras como Código, utiliza o cProd<br/>
1.0.26 - Adiciona a propriedade "PaginaCodigo" permitindo alterar a pagina de codigo da impressora<br/>
1.0.25 - QRCode não estava saindo na lateral para NFCe<br/>
1.0.24 - Adiciona File (ConfiguracaoFile) às formas de comunicação<br/>
1.0.23 - Removendo o antigo DFe.NET e migrando para Hercules.NET<br/>
1.0.22 - Não imprimia NFCe se a tag infAdic estivesse nula <br/>
1.0.21 - Permite imprimir o QrCode como imagem, util para impressoras sem suporte a QrCode <br/>
1.0.20 - Opção de customizar o tamanho das colunas (número de caracteres na linha) <br/>
1.0.19 - QR Code lateral <br/>
1.0.18 - Atualizando referencias ao Zeus (remoção dos projetos shared) <br/>
1.0.17 - Altera o motor de impressão, adicionando os protcolos TCP e Serial e novos recursos como impressão de caracteres acentuados. <br/>
1.0.16 - Remove o @ que aparecia no meio do protocolo no NFCe <br/>
1.0.15 - Migrando para OpenAC.Net.Sat <br/>
1.0.14 - Opção de alterar casas decimais da quantidade <br/>
1.0.13 - Melhora na impressão da observação do contribuinte <br/>
1.0.12 - SAT quebrava se o XFant de emitente estivesse nulo <br/>
1.0.11 - Opção de ocultar tag "De olho no imposto" <br/>
1.0.10 - Não era possível imprimir NFCe sem a tag infAdic (issue #6) <br/>
1.0.9 - Impressão em 58mm <br/>
1.0.8 - Adiciona a impressão do logotipo <br/>
1.0.7 - Adiciona Qtde. total de itens" <br/>
1.0.6 - Impressão para cancelamento do SAT


Break changes:
----

* A versão 2.0.0 remove o suporte ao SAT (descontinuado pelo estado de São Paulo). Para manter-se imprimindo SAT utilize as versões 1.0.X.

* As versões 1.0.X não serão descontinuadas, mas serão consideradas como "legadas".

* A versão 1.0.23 remove a referência ao DFe.NET (antigo ZeusNfe) e troca pelo Hercules.NET. Por se tratar de um fork é necessário que seja removido a referência ao DFe.NET também na aplicação que usar o Gerene.DFe.EscPos para evitar duplicidade de namespace.

* A versão 1.0.17 trouxe um novo motor de impressão (OpenAC.Net.EscPos) que permite impressão RAW (padrão para comunicação USB identica ao comportamento anterior) e adiciona os protocolos TCP e Serial. Basta preencher o atributo "Impressora" (antes chamado de "NomeImpressora") e substituir o atributo "TipoImpressora" por "Protocolo" e o comportamento do motor anterior será mantido.

Outras configurações:
----

Os atributos "ConfiguracaoRAW", "ConfiguracaoTCP", "ConfiguracaoSerial" e "ConfiguracaoFile" permitem alterações no comportamento da impressora e na forma de comunicação, para mais informações confira o funcionamento em https://github.com/OpenAC-Net/OpenAC.Net.EscPos

Você pode encontrar exemplos de uso em Linux onde a configuração padrão (RAW) precisa ser alterada na issue #10 (https://github.com/marcosgerene/Gerene.DFe.EscPos/issues/10#issuecomment-2133509780).


Licença:
---- 

A licença do projeto é MIT, o seu uso é livre. Não garantimos QUALQUER suporte.



Agradecimentos:
----

O projeto Vip.Printer (https://github.com/leandrovip/Vip.Printer) serviu de motor de impressão entre as versões 1.0.0 e 1.0.16 de forma gratuíta e com qualidade, permitindo o funcionamento dessa biblioteca por quase dois anos com um nível muito baixo de manutenção.

Em busca de evolução a troca de motor foi necessária na versão 1.0.17. Essa mudança permitiu novos protocolos e o uma parceria ainda mais estreita com o grupo OpenAC que vem fazendo um trabalho incrível com diversos componentes para automação comercial. Conheça mais em: https://github.com/OpenAC-Net
