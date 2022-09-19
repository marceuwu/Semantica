using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Semantica
{
    //Clase que almacena solo una variable, necesitamos una estructura de datos para alamacenar más varibales
    public class Variable
    {
        public enum TipoDato
        {
            Char, Int, Float
        };
        string nombre;
        float valor;
        TipoDato tipo;

        public Variable(string nombre, TipoDato tipo)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            valor = 0;

        }

        public void setValor(float valor)
        {
            this.valor = valor;
        }

        public float getValor()
        {
            return this.valor;
        }

        public string getNombre()
        {
            return this.nombre;
        }

        public TipoDato getTipo()
        {
            return this.tipo;
        }

    }
}