;Archivo: prueba.cpp
;Fecha: 07/11/2022 10:00:02 a. m.
#make_COM#
include 'emu8086.inc'
ORG 100h
;Variables
	a DW ? 
	altura DW ? 
MOV AX,3
PUSH AX
POP AX
MOV a, AX
InicioWhile1:
MOV AX,2
PUSH AX
POP AX
POP BX
CMP AX,BX
JE FinWhile1
PRINTN "*"
MOV AX,2
PUSH AX
POP AX
MOV a, AX
JMP InicioWhile1
FinWhile1:
RET
DEFINE_SCAN_NUM
