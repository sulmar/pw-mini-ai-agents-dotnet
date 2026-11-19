from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib import colors
from reportlab.lib.enums import TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from pypdf import PdfReader

OUT='output/pdf/agenda-multi-agents-shelly-lab.pdf'
pdfmetrics.registerFont(TTFont('ArialPL','/System/Library/Fonts/Supplemental/Arial.ttf'))
pdfmetrics.registerFont(TTFont('ArialPLBold','/System/Library/Fonts/Supplemental/Arial Bold.ttf'))
pdfmetrics.registerFontFamily('ArialPL',normal='ArialPL',bold='ArialPLBold')
styles=getSampleStyleSheet()
styles.add(ParagraphStyle(name='BodyPL',fontName='ArialPL',fontSize=9,leading=12,spaceAfter=5))
styles.add(ParagraphStyle(name='TitlePL',fontName='ArialPLBold',fontSize=23,leading=27,spaceAfter=5))
styles.add(ParagraphStyle(name='SubPL',fontName='ArialPL',fontSize=11,leading=15,spaceAfter=14))
styles.add(ParagraphStyle(name='HeadingPL',fontName='ArialPLBold',fontSize=10,leading=13,spaceBefore=10,spaceAfter=5))
styles.add(ParagraphStyle(name='SmallPL',fontName='ArialPL',fontSize=8,leading=11))
def p(t,style='BodyPL'): return Paragraph(t,styles[style])
story=[p('TWORZENIE AGENTÓW AI W .NET','SmallPL'),Spacer(1,9),p('Multi-agents','TitlePL'),p('Od wymagania do skryptu na urządzeniu Shelly','SubPL')]
story += [p('<b>Pytanie przewodnie:</b> Jak podzielić pracę między agentów, żeby wygenerowany skrypt został sprawdzony przed wdrożeniem?'),p('<b>Cel:</b> Zaprojektować współpracę agentów, którzy generują skrypt Shelly, testują go w symulatorze i po potwierdzeniu użytkownika wdrażają na fizyczne urządzenie.')]
story += [p('Agenda wykładu','HeadingPL')]
items=[
('1. Shelly Lab jako środowisko pracy','Serwer MCP: narzędzia, dokumentacja, symulator i dostęp do urządzeń. Wyszukiwanie dokumentacji i dostarczanie odpowiednich fragmentów do kontekstu (RAG).'),
('2. Jeden agent jako punkt odniesienia','Cały przebieg w jednym kontekście: analiza wymagania, generowanie skryptu, testowanie i wdrażanie. Gdzie pojawiają się trudności?'),
('3. Podział odpowiedzialności','Koordynator, programista, tester i wdrożeniowiec. Osobne instrukcje, konteksty oraz zakresy uprawnień.'),
('4. Przekazywanie pracy między agentami','Jawne wyniki etapów: wymagania, kryteria akceptacji, kod, scenariusze testowe i raporty. Co powinien otrzymać kolejny agent?'),
('5. Pętla programista - tester','Uruchomienie skryptu w symulatorze, wykrycie błędu, raport, poprawka i ponowne testy. Warunki zakończenia oraz limit prób.'),
('6. Od symulatora do fizycznego Shelly','Sprawdzenie urządzenia docelowego, plan wdrożenia, potwierdzenie użytkownika i weryfikacja działania po wgraniu.'),
('7. Czy wielu agentów pomogło?','Porównanie z jednym agentem: jakość, czas, koszt i diagnozowanie błędów. Kiedy podział pracy uzasadnia dodatkową złożoność?')]
for title,desc in items: story.append(p('<b>'+title+'</b><br/>'+desc))
story += [p('Role w demonstracji','HeadingPL')]
rows=[['Agent','Odpowiedzialność'],['Koordynator','Doprecyzowuje wymagania, ustala kryteria i kieruje pracą.'],['Programista','Korzysta z dokumentacji, tworzy skrypt i wprowadza poprawki.'],['Tester','Tworzy scenariusze na podstawie wymagań i ocenia wyniki symulacji.'],['Wdrożeniowiec','Sprawdza cel, wdraża po potwierdzeniu i weryfikuje działanie.']]
t=Table([[p(c,'SmallPL') for c in r] for r in rows],colWidths=[95,412])
t.setStyle(TableStyle([('BACKGROUND',(0,0),(-1,0),colors.HexColor('#eeeeee')),('LINEBELOW',(0,0),(-1,0),.6,colors.black),('LINEBELOW',(0,1),(-1,-1),.3,colors.HexColor('#bbbbbb')),('VALIGN',(0,0),(-1,-1),'TOP'),('TOPPADDING',(0,0),(-1,-1),5),('BOTTOMPADDING',(0,0),(-1,-1),5)]))
story += [t,p('Przebieg demonstracji','HeadingPL'),p('Wymaganie → kryteria akceptacji → dokumentacja → skrypt → symulacja i testy → poprawki → potwierdzenie → urządzenie.'),p('<b>Myśl przewodnia:</b> MCP udostępnia możliwości Shelly Lab. Architektura wieloagentowa organizuje współpracę. Tester sprawdza wymagania, a aplikacja egzekwuje uprawnienia i potwierdzenie wdrożenia.','SmallPL')]
def footer(c,d):
 c.setFont('ArialPL',8);c.setFillColor(colors.HexColor('#555555'));c.drawString(44,25,'Agenda wykładu | Shelly Lab');c.drawRightString(A4[0]-44,25,str(d.page))
SimpleDocTemplate(OUT,pagesize=A4,rightMargin=44,leftMargin=44,topMargin=34,bottomMargin=38,title='Multi-agents - agenda wykładu z Shelly Lab',author='').build(story,onFirstPage=footer,onLaterPages=footer)
r=PdfReader(OUT)
assert len(r.pages)==1, len(r.pages)
text=r.pages[0].extract_text()
assert 'Koordynator' in text and 'urządzenie' in text
print(OUT, '| stron:',len(r.pages))
