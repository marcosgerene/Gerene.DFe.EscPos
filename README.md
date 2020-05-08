# Gerene.DFe.EscPos

[![Nuget count](http://img.shields.io/nuget/v/Gerene.DFe.EscPos.svg)](https://www.nuget.org/packages/Gerene.DFe.EscPos)

Impressão em ESC/POS e BemaPOS para DFes

Atualmente a biblioteca atende os documentos SAT e NFCe.

Funcionamento:
----

O projeto conta com um DEMO construído em WinForms para facilitar o entendimento.

Exemplo de uso:
```
using (var _printer = new SatPrinter()) //ou new NFCePrinter();
{
	_printer.TipoImpressora = PrinterType.Epson; //Ou bematech para bemapos
	_printer.NomeImpressora = "Nome da impressora";
	_printer.CortarPapel = true;
	_printer.ProdutoDuasLinhas = false;
	_printer.UsarBarrasComoCodigo = false;
	_printer.DocumentoCancelado = false;

	_printer.Imprimir(string_com_o_conteudo_do_xml);
}
```
O projeto é construído em .Net Standard 2.0.

O projeto conta com dois demos um em .Net Framework 4.6.2 e outro em .Net 3.1


Implementando um novo tipo documento:
----

Para ajudar e implementar um novo tipo de documento basta implementar a interface IDFe

```
public interface IDfePrinter : IDisposable
{
	string NomeImpressora { get; set; }
	PrinterType TipoImpressora { get; set; }
	bool CortarPapel { get; set; }
	bool ProdutoDuasLinhas { get; set; }
	bool UsarBarrasComoCodigo { get; set; }
	bool DocumentoCancelado {get; set; }
	byte[] Logotipo { get; set; }

	void Imprimir(string xmlcontent);
}
```

Dependências:
----

Vip.Printer - https://github.com/leandrovip/Vip.Printer

ACBr.Net.Sat - https://github.com/ACBrNet/ACBr.Net.Sat

DFeBR.Net - https://github.com/dfebr/dfebr-net

----


A licença do projeto é MIT, o seu uso é livre.
Não garantimos QUALQUER suporte.
