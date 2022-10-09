//DIAZ GUERRERO MARCELA
#include <iostream>
#include <stdio.h>
#include <conio.h>
float area, radio, pi, resultado;
int a, d, altura;
float x;
char y;int i ;int j;
// Este programa calcula el volumen de un cilindro.
void main(){
    //Requerimiento 5.- Levanta una excepcion en el scanf si la captura no es un numero
    printf("Introduce la altura de la piramide: ");
    scanf("altura", &altura);
    for(j = 0; j < altura-1; j++){
        if(j!=2){
            printf("*");
        }
        else{
            printf("-");//Requerimiento 4.- evalua nuevamente los else
        }
    }
}