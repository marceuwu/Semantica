;Archivo: prueba.cpp
;Fecha: 08/11/2022 12:46:59 a. m.
#make_COM#
include 'emu8086.inc'
ORG 100h
;Variables
	a DW ? 
	altura DW ? 
	j DW ? 
	b DW ? 
MOV AX,0
PUSH AX
POP AX
MOV a,0
MOV a, AX
InicioWhile1:
MOV AX,a
PUSH AX
MOV AX,3
PUSH AX
POP BX
POP AX
CMP AX,BX
JGE FinWhile1
PRINT "*"
INC a
JMP InicioWhile1
FinWhile1:
RET
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
END
