;Archivo: prueba.cpp
;Fecha: 08/11/2022 02:18:01 a. m.
#make_COM#
include 'emu8086.inc'
ORG 100h
;Variables
	a DW ? 
	altura DW ? 
	j DW ? 
	b DW ? 
MOV AX,2
PUSH AX
POP AX
MOV j,2
inicioFor0:
MOV AX,j
PUSH AX
MOV AX,10
PUSH AX
POP BX
POP AX
CMP AX,BX
JGE finFor0
MOV AX,2
PUSH AX
POP AX
MOV AX, j
MOV BX, 2
MUL BX
MOV j, AX
PRINT "*"
JMP inicioFor0
finFor0:
RET
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
END
