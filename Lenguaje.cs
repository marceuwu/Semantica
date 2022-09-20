//Validar la congruencia 
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

//Requerimiento 1.- Eliminar las dobles comillas del printf e interpretar las secuencia de escape dentro de la cadena ej. \n salto de linea
//Requerimiento 2.- Marcar los errores sintacticos cuando la variable no exista linea 
//Requerimiento 3.- Modificar el valor de la variable en la asignación (método asignación)
//Requerimiento 4.- Obtener el valor de la variable cuando se requiera, ya se puso en factor pero falta programar el método y programar el método getValor() (para que si queremos imprimir variables se pueda(expresión))
//Requerimiento 5.- Modificar el valor de la variable en el scanf
//Cada requerimiento vale 20%
namespace Semantica
{
    
    public class Lenguaje : Sintaxis
    {
        List<Variable> listaVariables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        public Lenguaje()
        {

        }
        public Lenguaje(string nombre) : base(nombre)
        {

        }

        private string EliminaComillas(string cadena)
        {

            cadena = cadena.Substring(1);
            cadena = cadena.Substring(0, cadena.Length - 1);

            return SecuenciaEscape(cadena);
        }

        private string SecuenciaEscape(string cadena)
        {
            List<string> lSecuenciaEscape = new List<string>();

            //Guardamos en una lista las secuencias de escape
            for (int i = 0; i < cadena.Length - 1; i++)
            {
                if (cadena[i] == '\\')
                {
                    lSecuenciaEscape.Add(cadena[i].ToString() + cadena[i + 1].ToString());

                }
            }

            foreach (string s in lSecuenciaEscape)
            {
                //remplazamos en la cadena la secuencia de escape
                //aplicamos a la cadena la secuencia de escape 
                cadena = cadena.Replace(s, Regex.Unescape(s));

            }

            return cadena;

        }
        private void AddVariable(String nombre,Variable.TipoDato tipo)
        {
            listaVariables.Add(new Variable(nombre, tipo)); 
        }

        private void DisplayVariables()
        {
            log.WriteLine();
            log.WriteLine("Variables: ");
            foreach (Variable v in listaVariables) // este for no necesita que pongamos que se incremente el contador
            {
                log.WriteLine(v.getNombre()+" "+v.getTipo()+" "+v.getValor());
            }
        }

        private bool ExisteVariable(string nombre)
        {
            foreach (Variable v in listaVariables)
            {	    
                if (v.getNombre().Equals(nombre))
                {
                    return true;
                }

            }
            return false;
        }
        private void ModificaVariable(string nombre, float nuevoValor)
        {
            foreach(Variable v in listaVariables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    v.setValor(nuevoValor);
                }
            }
        }

        private float getValorVariable(string nombreVariable)
        {
            foreach (Variable v in listaVariables)
            {
                if (v.getNombre().Equals(nombreVariable))
                {
                    return v.getValor();
                }
            }
            return 0;
        }
        private Variable.TipoDato getTipoVariable(string nombreVariable)
        {
            foreach (Variable v in listaVariables)
            {
                if (v.getNombre().Equals(nombreVariable))
                {
                    return v.getTipo();
                }
            }
            return Variable.TipoDato.Char;
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            Libreria();
            Variables();
            Main();
            this.DisplayVariables();
        }

        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador); 
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                Libreria();
            }
        }

         //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char;//la tenemos que modificar dependiendo el tipo
               
                switch (getContenido())
                {
                    case "int":
                    tipo = Variable.TipoDato.Int;
                    break;
                    case "float":
                    tipo = Variable.TipoDato.Float;
                    break;
                
                }

                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }

         //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {

            //Cada variable que detectamos la metemos en la lista
            if(getClasificacion() == Tipos.Identificador)
            {
                if(!ExisteVariable(getContenido()))
                {
                   AddVariable(getContenido(),tipo);
                }
                else
                {
                    throw new Error("Error de sintaxis, variable duplicada <"+getContenido()+"> en linea: "+linea, log);
                }
            }
            
            match(Tipos.Identificador);

            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }
        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
            
            match("}"); 
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase()
        {
            Instruccion();
            if (getContenido() != "case" && getContenido() !=  "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase();
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion()
        {
            if (getContenido() == "printf")
            {
                Printf();
            }
            else if (getContenido() == "scanf")
            {
                Scanf();
            }
            else if (getContenido() == "if")
            {
                If();
            }
            else if (getContenido() == "while")
            {
                While();
            }
            else if(getContenido() == "do")
            {
                Do();
            }
            else if(getContenido() == "for")
            {
                For();
            }
            else if(getContenido() == "switch")
            {
                Switch();
            }
            else
            {
                Asignacion();
            }
        }
        private Variable.TipoDato EvaluaNuemro(float resultado)
        {
            if(resultado <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if(resultado <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            // como saber que un numero tiene parte flotante /fraccionaria 
            return Variable.TipoDato.Float;
        }
        private bool EvaluaSemantica(string variable, float resultado)
        {
            Variable.TipoDato tipoDato = getTipoVariable(variable); 
            
            return false;
        }
        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion()
        {
            //Requerimiento 2.- si no existe la variable(getContenido()) levanta la excepcion 
            if(ExisteVariable(getContenido()) == false)
            {
                throw new Error("Error de sintaxis, la variable no ha sido declarada <" + getContenido() + "> en linea: " + linea, log);
            }

            log.WriteLine();
            string nombre = getContenido();
     
            match(Tipos.Identificador);
            log.Write(nombre + " = ");
            match(Tipos.Asignacion);
            Expresion();
            match(";");

            float resultado = stack.Pop();
            
            log.WriteLine(" = " + resultado);
            log.WriteLine();
            ModificaVariable(nombre, resultado);
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{") 
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do()
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            } 
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            Condicion();
            match(";");
            Incremento();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();  
            }
            else
            {
                Instruccion();
            }
        }

        //Incremento -> Identificador ++ | --
        private void Incremento()
        {
            string variable = getContenido(); //salvamos el token
            //Requerimiento 2.- si no existe la variable(getContenido()) levanta la excepcion 
            if (ExisteVariable(variable) == false)
            {
                throw new Error("Error de sintaxis, la variable no ha sido declarada <" + getContenido() + "> en linea: " + linea, log);
            }

            match(Tipos.Identificador);
            if(getContenido() == "++")
            {
                match("++");
                //sacar el valor de la variable, incrementarlo en 1 y luego meterlo en la variable
                ModificaVariable(variable,getValorVariable(variable)+1);

            }
            else
            {
                match("--");
                ModificaVariable(variable,getValorVariable(variable)-1);
            }
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch()
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            match(")");
            match("{");
            ListaDeCasos();
            if(getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones();  
                }
                else
                {
                    Instruccion();
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos()
        {
            match("case");
            Expresion();
            stack.Pop();
            match(":");
            ListaInstruccionesCase();
            if(getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if(getContenido() == "case")
            {
                ListaDeCasos();
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private void Condicion()
        {
            Expresion();
            stack.Pop();
            match(Tipos.OperadorRelacional);
            Expresion();
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();  
            }
            else
            {
                Instruccion();
            }
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
        }

        //Printf -> printf(cadena|expresion);
        private void Printf()
        {
            match("printf");
            match("(");
            if(getClasificacion() == Tipos.Cadena)
            {
                //Requerimiento 1.- Eliminar las comillas de la cadena 
                
                Console.Write(EliminaComillas(getContenido()));
                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                Console.Write(stack.Pop());//imprimimos en la consola
            }
            
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena,& identificador);
        private void Scanf()    
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");

            //Requerimiento 2.- Si no existe la variable levanta la excepcion
            if (ExisteVariable(getContenido()) == false)
            {
                throw new Error("Error de sintaxis, la variable no ha sido declarada <" + getContenido() + "> en linea: " + linea, log);
            }

            string sValor =""+ Console.ReadLine(); //Capturamos en pantalla

            //Requerimiento 5.- Modificar el valor de la variable en el scanf
            ModificaVariable(getContenido(),float.Parse(sValor));
            
            match(Tipos.Identificador);
            match(")");
            match(";");
        }

        //Main      -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones();
            
        }

        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(operador+" ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (operador){
                    case "+":
                        stack.Push(n1+n2);
                        break;
                    case "-":
                        stack.Push(n2-n1);
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "*":
                        stack.Push(n2*n1);
                        break;
                    case "/":
                        stack.Push(n2/n1);
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " " );
                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
                
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                //Requerimiento 2.- si no existe la variable(getContenido()) levanta la excepcion 
                if (ExisteVariable(getContenido()) == false)
                {
                    throw new Error("Error de sintaxis, la variable no ha sido declarada <" + getContenido() + "> en linea: " + linea, log);
                }

                log.Write(getContenido() + " " );
                stack.Push(getValorVariable(getContenido()));

                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}