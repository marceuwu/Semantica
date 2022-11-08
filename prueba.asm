;Archivo: prueba.cpp
;Fecha: 07/11/2022 09:34:20 p. m.
#make_COM#
include 'emu8086.inc'
ORG 100h
;Variables
	a DW ? 
	altura DW ? 
	j DW ? 
	b DW ? 
MOV AX,19
PUSH AX
POP AX
MOV a,19
MOV a, AX
MOV AX,5
PUSH AX
POP AX
MOV altura,5
MOV altura, AX
MOV AX,altura
PUSH AX
POP AX
MOV AX, 19
MOV CX, 5
DIV CX
MOV a, DX
MOV a,4
MOV AX,a
PUSH AX
POP AX
RET
DEFINE_SCAN_NUM
