;Archivo: prueba.cpp
;Fecha: 08/11/2022 09:37:57 p. m.
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
PRINT "Valor de variable int a antes del casteo: "
MOV AX,a
PUSH AX
POP AX
CALL PRINT_NUM
MOV AX,a
PUSH AX
POP AX
MOV AX,0
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
