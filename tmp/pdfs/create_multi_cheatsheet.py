from pathlib import Path
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, PageBreak, KeepTogether
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.pagesizes import A4
from reportlab.lib import colors
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from pypdf import PdfReader

root=Path('/Users/marcinsulecki/Projects/pw-mini-ai-agents-dotnet')
out=root/'output/pdf/multi-agent-sciaga-prowadzacego.pdf'
out.parent.mkdir(parents=True,exist_ok=True)
pdfmetrics.registerFont(TTFont('PL','/System/Library/Fonts/Supplemental/Arial.ttf'))
pdfmetrics.registerFont(TTFont('PLB','/System/Library/Fonts/Supplemental/Arial Bold.ttf'))
pdfmetrics.registerFontFamily('PL',normal='PL',bold='PLB')
styles={
 'title':ParagraphStyle('title',fontName='PLB',fontSize=22,leading=26,spaceAfter=7),
 'sub':ParagraphStyle('sub',fontName='PL',fontSize=10,leading=14,spaceAfter=12),
 'h':ParagraphStyle('h',fontName='PLB',fontSize=12,leading=16,spaceBefore=12,spaceAfter=5),
 'body':ParagraphStyle('body',fontName='PL',fontSize=10,leading=14,spaceAfter=5),
 'small':ParagraphStyle('small',fontName='PL',fontSize=8,leading=11,textColor=colors.HexColor('#555555'),spaceAfter=5),
}
def p(t,s='body'):return Paragraph(t,styles[s])
story=[p('Multi-agents','title'),p('Ściąga prowadzącego | Wykład 09 | Shelly Lab','sub'),p('<b>Pytanie otwierające:</b> Kiedy jeden agent wystarcza, a kiedy warto zbudować zespół?'),p('Kolejność zgodna z plikami 01-07 w katalogu slajdów. Notatki do mówienia, nie tekst do odczytania.','small')]
slides=[
('01','Otwarcie wykładu','01-multi-agent.png',[
('Powiedz','Celem jest przejście od wymagania użytkownika do skryptu działającego na fizycznym Shelly.'),
('Zapytaj','Czy jeden agent może zebrać wymagania, napisać skrypt, przetestować go i wdrożyć? Co zyskalibyśmy przez podział pracy?'),
('Przejście','Najpierw sprawdźmy, kiedy taki podział ma sens.')]),
('02','Od jednego agenta do zespołu','02-od-jednego-agenta-do-zespolu.png',[
('Myśl','Jeden agent to dobry punkt wyjścia dla prostego zadania. Więcej kroków nie oznacza automatycznie potrzeby wielu agentów.'),
('Wskaż','Programista, tester i dokumentalista: podział odpowiedzialności, specjalizacja oraz osobne konteksty i uprawnienia.'),
('Dopowiedz','Równolegle uruchamiamy tylko niezależne zadania. Test konkretnej wersji kodu wymaga wcześniejszego dostarczenia tej wersji.')]),
('03','Anatomia systemu wieloagentowego','03-anatomia-systemu-wieloagentowego.png',[
('Myśl','Agent ma rolę, instrukcje, narzędzia i kontekst. Agenci mogą korzystać z tego samego modelu.'),
('Wskaż','Programista tworzy kod, tester korzysta z symulatora, dokumentalista opracowuje opis. Dokumentacja może być zasobem dostępnym przez MCP.'),
('Dopowiedz','Tester potrzebuje wymagań i kryteriów akceptacji, nie tylko kodu. Nie każdy agent musi otrzymać całą historię rozmowy.')]),
('04','Wzorce współpracy','04-wzorce-wspolpracy.png',[
('Sekwencja','Wynik jednego agenta jest wejściem następnego. Przykład: Autor -> Recenzent.'),
('Równolegle','Niezależne zadania są wykonywane osobno, a następnie wyniki są zbierane.'),
('Koordynator','Rozdziela zadania i odbiera wyniki specjalistów. Wybór wzorca wynika z zależności zadania.')]),
('05','Komunikacja między agentami','05-komunikacja-miedzy-agentami.png',[
('Myśl','Przekazujemy konkretny cel, dane wejściowe i oczekiwany wynik.'),
('Prześledź','Koordynator przekazuje wymagania i kryteria. Programista przekazuje kod i wymagania. Tester zwraca raport z testów.'),
('Dopowiedz','Raport powinien wskazywać scenariusz, oczekiwane i uzyskane zachowanie oraz wersję skryptu. Samo „nie działa” nie wystarcza.')]),
('06','Shelly Lab: programista i tester','06-shelly-lab-programista-i-tester.png',[
('Prześledź','Programista -> skrypt -> Tester -> testy -> Symulator. Błędy wracają w raporcie do poprawy.'),
('Podkreśl','Symulator jest narzędziem, nie kolejnym agentem. Shelly Lab udostępnia narzędzia i dokumentację przez MCP; orkiestracja organizuje pracę agentów.'),
('Wdrożenie','Po zaliczeniu testów gotowy skrypt trafia na fizyczne Shelly po potwierdzeniu użytkownika. Potwierdzenie i uprawnienia egzekwuje aplikacja.'),
('Ograniczenie','Ustal limit prób. Testy w symulatorze nie gwarantują działania na sprzęcie: po wdrożeniu sprawdź zachowanie urządzenia.')]),
('07','Wspólny cel, różne role','07-wspolny-cel-rozne-role.png',[
('Podsumuj','Użytkownik określa cel. Zespół planuje, wykonuje i sprawdza. Każdy agent odpowiada za swoją część zadania.'),
('Zapytaj','Co w naszym przykładzie uzasadnia osobnego testera? Które zadania można wykonać równolegle?'),
('Zamknij','Oceń wynik względem jednego agenta: jakość, czas, koszt i łatwość diagnozowania błędów.')]),
]
for num,title,file,notes in slides:
 if num=='05':
  story += [PageBreak(),p('Multi-agents: komunikacja i demo','title'),p('Druga część wykładu | slajdy 05-07','sub')]
 assert (root/'docs/lectures/09-multi-agent/slides'/file).is_file()
 block=[p(num+' | '+title,'h'),p(file,'small')]
 for label,text in notes:block.append(p('<b>'+label+':</b> '+text))
 story.append(KeepTogether(block))
story += [p('Demo MAF: Autor -> Recenzent','h'),p('Pokaż dwie instrukcje, zbudowanie sekwencji i odpowiedzi obu agentów. Następnie połącz role z Shelly Lab: Autor staje się Programistą, Recenzent Testerem. Ocena tekstu przez model nie zastępuje wykonania testów.'),p('Przed wykładem: sprawdź uruchomienie demo, dostęp do modelu, serwera MCP, symulatora i urządzenia. Przygotuj wymaganie oraz oczekiwane wyniki testów.','small')]
def footer(c,d):
 c.setStrokeColor(colors.HexColor('#bbbbbb'));c.line(42,35,A4[0]-42,35)
 c.setFont('PL',8);c.setFillColor(colors.HexColor('#555555'));c.drawString(42,23,'Multi-agents | notatki prowadzącego');c.drawRightString(A4[0]-42,23,str(d.page))
SimpleDocTemplate(str(out),pagesize=A4,leftMargin=42,rightMargin=42,topMargin=34,bottomMargin=45,title='Multi-agents - ściąga prowadzącego').build(story,onFirstPage=footer,onLaterPages=footer)
r=PdfReader(out)
assert len(r.pages)==2,len(r.pages)
text='\n'.join(x.extract_text() for x in r.pages)
for phrase in ['Symulator','Koordynator','potwierdzeniu','07 |']:assert phrase in text,phrase
print(out)
