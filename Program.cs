//DIAZ GUERRERO MARCELA
using System;
using System.IO;

namespace Semantica
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (Lenguaje a = new Lenguaje())
            {
                try
                {
                    a.Programa();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            

            Console.ReadLine();
        }
    }
}