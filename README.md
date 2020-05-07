# Gerene.DFe.EscPos
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
	_printer.UsarBarrasComoCodio = false;

	_printer.Imprimir(string_com_o_conteudo_do_xml);
}
```
O projeto é construído em .Net Standard 2.0.
O demo está em .Net Framework 4.6.2


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
	bool UsarBarrasComoCodio { get; set; }
	byte[] Logotipo { get; set; }

	void Imprimir(string xmlcontent);
}
```

Dependências:
----

Vip.Printer - https://github.com/leandrovip/Vip.Printer

ACBr.Net.Sat - https://github.com/ACBrNet/ACBr.Net.Sat

DFe.Net - https://github.com/ZeusAutomacao/DFe.NET

----


Exemplo de impressão (SAT)
---
![](https://user-images.githubusercontent.com/15945003/81335501-19b82900-907e-11ea-907a-065ba23e3897.jpeg)


A licença do projeto é MIT, o seu uso é livre.
Não garantimos QUALQUER suporte.
