;Archivo: prueba.cpp
;Fecha: 07/11/2022 11:43:15 p. m.
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
MOV AX,2
PUSH AX
POP AX
POP BX
CMP AX,BX
JGE FinWhile1
PRINT "*"
INC a
MOV a,1
JMP InicioWhile1
FinWhile1:
RET
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
END
