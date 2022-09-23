//DIAZ GUERRERO MARCELA
using System;
using System.IO;

namespace Semantica
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                /*Byte x =(Byte)256;//castear y convertir el valor de la nueva variable
                Console.WriteLine(x);
                x++;
                Console.WriteLine(x);
                ¿cómo saber que es de tipo char?
                si el valor <256 o <= 255
                */

                Lenguaje a = new Lenguaje();

                a.Programa();


                
                /*a.match("#");
                a.match("include");
                a.match("<");
                a.match(Token.Tipos.Identificador);
                a.match(".");
                a.match("h");
                a.match(">"); */
                
                //while(!a.FinArchivo())
                //{
                  //  a.NextToken();
                //}
                a.cerrar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}