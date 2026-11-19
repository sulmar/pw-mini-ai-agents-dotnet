from pathlib import Path
from reportlab.platypus import SimpleDocTemplate, Paragraph, Image, Table, TableStyle, Spacer
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.pagesizes import A4
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from pypdf import PdfReader

root=Path('/Users/marcinsulecki/Projects/pw-mini-ai-agents-dotnet')
slides=root/'docs/lectures/09-multi-agent/slides'
out=root/'output/pdf/multi-agent-sciaga-prowadzacego.pdf'
pdfmetrics.registerFont(TTFont('AgendaPL','/System/Library/Fonts/Supplemental/Arial.ttf'))
pdfmetrics.registerFont(TTFont('AgendaPLBold','/System/Library/Fonts/Supplemental/Arial Bold.ttf'))
title=ParagraphStyle('Title',fontName='AgendaPLBold',fontSize=19,leading=23,spaceAfter=14)
label=ParagraphStyle('Label',fontName='AgendaPL',fontSize=12,leading=17)
items=[
 ('01-multi-agent.png','1. Multi-agents'),
 ('02-od-jednego-agenta-do-zespolu.png','2. Od jednego agenta do zespołu'),
 ('03-anatomia-systemu-wieloagentowego.png','3. Anatomia systemu wieloagentowego'),
 ('04-wzorce-wspolpracy.png','4. Wzorce współpracy'),
 ('05-komunikacja-miedzy-agentami.png','5. Komunikacja między agentami'),
 ('06-shelly-lab-programista-i-tester.png','6. Shelly Lab: programista i tester'),
 ('07-wspolny-cel-rozne-role.png','7. Wspólny cel, różne role'),
]
rows=[]
for filename,text in items:
 im=Image(str(slides/filename))
 ratio=im.imageHeight/im.imageWidth
 im.drawWidth=125;im.drawHeight=125*ratio
 rows.append([im,Paragraph(text,label)])
t=Table(rows,colWidths=[143,360],hAlign='LEFT')
t.setStyle(TableStyle([('VALIGN',(0,0),(-1,-1),'MIDDLE'),('LEFTPADDING',(0,0),(-1,-1),0),('RIGHTPADDING',(0,0),(-1,-1),12),('TOPPADDING',(0,0),(-1,-1),5),('BOTTOMPADDING',(0,0),(-1,-1),5)]))
SimpleDocTemplate(str(out),pagesize=A4,leftMargin=42,rightMargin=42,topMargin=32,bottomMargin=30,title='Multi-agents - agenda wykładu').build([Paragraph('Multi-agents - agenda wykładu',title),t])
assert len(PdfReader(out).pages)==1
print(out)
