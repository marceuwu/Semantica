;Archivo: prueba.cpp
;Fecha: 08/11/2022 09:34:30 a. m.
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
MOV AX,5
PUSH AX
POP AX
MOV altura,5
MOV AX,altura
PUSH AX
POP AX
MOV AX, altura
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
SUB i, 1
MOV AX,0
PUSH AX
POP AX
MOV j,0
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
JNE if1
PRINT "*"
JMP else1
if1:
PRINT "-"
else1:
MOV AX,1
PUSH AX
POP AX
JMP InicioWhile1
FinWhile1:
PRINTN
PRINTN
JMP inicioFor0
finFor0:
RET
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
END
