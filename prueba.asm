;Archivo: prueba.cpp
;Fecha: 04/11/2022 09:55:10 a. m.
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
MOV AX,4
PUSH AX
POP AX
MOV altura, AX
MOV AX, 4
MOV BX, 3
SUB AX,BX
MOV altura, AX
RET
DEFINE_SCAN_NUM
