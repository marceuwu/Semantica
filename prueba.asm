;Archivo: prueba.cpp
;Fecha: 31/10/2022 09:55:50 a. m.
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
	l DW ? 
	k DW ? 
	x DW ? 
	y DW ? 
	i DW ? 
	j DW ? 
MOV AX,2
PUSH AX
POP AX
MOV altura, AX
inicioFor0:
MOV AX,0
PUSH AX
POP AX
MOV j, AX
POP AX
POP BX
CMP AX,BX
JGE 
MOV AX,1
PUSH AX
MOV AX,2
PUSH AX
POP AX
POP BX
CMP AX,BX
JE if1
PRINTN "*"
PRINTN "-"
if1:
POP AX
POP BX
CMP AX,BX
JGE 
MOV AX,1
PUSH AX
MOV AX,2
PUSH AX
POP AX
POP BX
CMP AX,BX
JE if3
PRINTN "*"
PRINTN "-"
if3:
POP AX
POP BX
CMP AX,BX
JGE 
MOV AX,1
PUSH AX
MOV AX,2
PUSH AX
POP AX
POP BX
CMP AX,BX
JE if5
PRINTN "*"
PRINTN "-"
if5:
finFor0:
RET
DEFINE_SCAN_NUM
