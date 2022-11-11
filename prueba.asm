;Archivo: prueba.cpp
;Fecha: 10/11/2022 09:53:50 p. m.
#make_COM#
include 'emu8086.inc'
ORG 100h
;Variables
	area DW ? 
	radio DW ? 
	pi DW ? 
	resultado DW ? 
	a DW ? 
	d DW ? 
	altura DW ? 
	cinco DW ? 
	x DW ? 
	y DW ? 
	i DW ? 
	j DW ? 
	k DW ? 
PRINT "Introduce la altura de la piramide: "
CALL SCAN_NUM
MOV altura,CX
MOV AX,altura
PUSH AX
MOV AX,2
PUSH AX
POP BX
POP AX
CMP AX,BX
JLE if1
MOV AX,altura
PUSH AX
POP AX
MOV i, AX
inicioFor0:
MOV AX,i
PUSH AX
MOV AX,0
PUSH AX
POP BX
POP AX
CMP AX,BX
JLE finFor0
MOV AX,1
PUSH AX
POP AX
MOV AX, i
MOV BX, 1
SUB AX,BX
PUSH AX
MOV AX,0
PUSH AX
POP AX
MOV j, AX
InicioWhile1:
MOV AX,j
PUSH AX
MOV AX,altura
PUSH AX
MOV AX,i
PUSH AX
POP BX
POP AX
SUB AX,BX
PUSH AX
POP BX
POP AX
CMP AX,BX
JGE FinWhile1
MOV AX,j
PUSH AX
MOV AX,2
PUSH AX
POP BX
POP AX
DIV BX
PUSH DX
MOV AX,0
PUSH AX
POP BX
POP AX
CMP AX,BX
JNE if2
PRINT "*"
JMP else2
if2:
PRINT "-"
else2:
MOV AX,1
PUSH AX
POP AX
MOV AX, j
MOV BX, 1
ADD AX, BX
PUSH AX
POP AX
MOV j, AX
JMP InicioWhile1
FinWhile1:
PRINTN
PRINT ""
POP AX
MOV i, AX
JMP inicioFor0
finFor0:
MOV AX,0
PUSH AX
POP AX
MOV k, AX
InicioDoWhile1:
PRINT "-"
MOV AX,2
PUSH AX
POP AX
MOV AX, k
MOV BX, 2
ADD AX, BX
PUSH AX
POP AX
MOV k, AX
MOV AX,k
PUSH AX
MOV AX,altura
PUSH AX
MOV AX,2
PUSH AX
POP BX
POP AX
MUL BX
PUSH AX
POP BX
POP AX
CMP AX,BX
JGE FinDoWhile1
JMP InicioDoWhile1
FinDoWhile1:
PRINTN
PRINT ""
JMP else1
if1:
PRINTN
PRINT "Error: la altura debe de ser mayor que 2"
PRINTN
PRINT ""
else1:
MOV AX,1
PUSH AX
MOV AX,1
PUSH AX
POP BX
POP AX
CMP AX,BX
JE if3
PRINT "Esto no se debe imprimir"
MOV AX,2
PUSH AX
MOV AX,2
PUSH AX
POP BX
POP AX
CMP AX,BX
JNE if4
PRINT "Esto tampoco"
JMP else4
if4:
else4:
JMP else3
if3:
else3:
MOV AX,260
PUSH AX
POP AX
MOV a, AX
PRINT "Valor de variable int a antes del casteo: "
MOV AX,a
PUSH AX
POP AX
CALL PRINT_NUM
MOV AX,a
PUSH AX
POP AX
MOV AX,4
PUSH AX
POP AX
MOV y, AX
PRINTN
PRINT "Valor de variable char y despues del casteo de a: "
MOV AX,y
PUSH AX
POP AX
CALL PRINT_NUM
PRINTN
PRINT "A continuacion se intenta asignar un int a un char sin usar casteo: "
PRINTN
PRINT ""
RET
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
END
