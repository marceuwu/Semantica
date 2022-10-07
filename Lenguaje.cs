//DIAZ GUERRERO MARCELA
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

/*
    Requerimiento 1.- Actualizar el dominante para variables en la expresion (pq solo lo hicimos con numeros) 
                    ej: float x; char y; y=x -> no podemos asiganar 4 bytes a 1 byte
    Requerimiento 2.- Actualizar el dominante para el casteo (si el casteo me dice char, el dominante es char)(casteo en postfijo, al final se castea)        
                    y el valor de la subexpresion   char x; float area; x=(char)area; ->permitir
    Requerimiento 3.- Funcion de conversion, programar un método de conversion de un valor a un tipo de dato
                    ej. priavte float Convertir(float valor, string TipoDato) regresa el valor al cual se debe cambiar
                    Deberan usar el residuo de la division %255, %65535

                    char ->255
                    float->256

                    debemos hacer una converion 
                    y asignar el valor nuevo a la variable
    Requerimiento 4.- Evaluar nuevamente la condición de If - else(se debe comportar de manera contraria), While, Do While, For corespecto al parametro 
                que recibe evaluacion=ejecuta -> verificar que la condición sea correcta para entrar al ciclo
    Requerimiento 5.- Levantar una excepción en el scanf cuando la captura no sea un número
    Requerimiento 6.- Ejecutar el for 
*/
namespace Semantica
{
    
    public class Lenguaje : Sintaxis
    {
        List<Variable> listaVariables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        public Lenguaje()
        {

        }
        public Lenguaje(string nombre) : base(nombre)
        {

        }

        private float ConvertirDato(float valor, string sTipoDato)
        {
 
            switch(sTipoDato)
            {
                case  "char":
                    valor = valor%255;
                    break;
                case  "int":
                    valor = valor%65535;
                    break;
            }
            return valor;
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
        
        //Main      -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(true); 
            
        }

        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
            
            match("}"); 
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "case" && getContenido() !=  "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion);
            }
            else if(getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if(getContenido() == "for")
            {
                For(evaluacion);
            }
            else if(getContenido() == "switch")
            {
                Switch(evaluacion);
            }
            else
            {
                Asignacion(evaluacion);
            }
        }
        private Variable.TipoDato EvaluaNuemro(float resultado)
        {
            
            if(resultado % 1 !=0){
                return Variable.TipoDato.Float;
            }
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
        private void Asignacion(bool evaluacion)
        {
            if(ExisteVariable(getContenido()) == false)
            {
                throw new Error("Error de sintaxis, la variable no ha sido declarada <" + getContenido() + "> en linea: " + linea, log);
            }

            log.WriteLine();
            string nombre = getContenido();
     
            match(Tipos.Identificador);
            log.Write(nombre + " = ");
            match(Tipos.Asignacion);

            dominante = Variable.TipoDato.Char;
            Expresion();
            match(";");

            float resultado = stack.Pop();
            
            log.WriteLine(" = " + resultado);
            log.WriteLine();

            Console.WriteLine(dominante);
            Console.WriteLine(EvaluaNuemro(resultado));

            if(dominante < EvaluaNuemro(resultado))
            {
                dominante = EvaluaNuemro(resultado);
                
            }

            if( dominante <= getTipoVariable(nombre))
            {
                if(evaluacion)
                {
                    ModificaVariable(nombre, resultado);
                }
            }
            else
            {
                throw new Error("Error de semantica, NO podemos asiganar un <" + dominante + "> a un "+getTipoVariable(nombre)+ " en linea: " + linea, log);
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            //Requermiento 4
            //encontrar una relacion entre evaluación y condicion para ejjecutar el bloque de instrucciones
            bool validarWhile = Condicion();
            match(")");
            if (getContenido() == "{") 
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
            }
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
            } 
            match("while");
            match("(");
            //Requerimmiento 4
            bool validarDo = Condicion();
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            match("for");
            match("(");
            Asignacion(evaluacion);
            //Requerimmiento 4 verificar que la condición se a verdadera
            //Requerimmiento 6:  a) guardar la direccion la posición del archivo de texto
            
            bool validarFor = Condicion();
            // b) Metemos un ciclo (while)pero despues de validar el for
            //while()
            //{

            
                match(";");
                Incremento(evaluacion);
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion);  
                }
                else
                {
                    Instruccion(evaluacion);
                }
            //c) Regresar a la posición de lectura del archivo
            //d) Sacar otro token
            //}
        }

        //Incremento -> Identificador ++ | --
        private void Incremento(bool evaluacion)
        {
            string variable = getContenido(); //salvamos el token
            if (ExisteVariable(variable) == false)
            {
                throw new Error("Error de sintaxis, la variable no ha sido declarada <" + getContenido() + "> en linea: " + linea, log);
            }

            match(Tipos.Identificador);
            if(getContenido() == "++")
            {
                match("++");
                if(evaluacion)
                {
                //sacar el valor de la variable, incrementarlo en 1 y luego meterlo en la variable
                ModificaVariable(variable,getValorVariable(variable) + 1);
                }
            }
            else
            {
                match("--");
                if(evaluacion)
                {
                ModificaVariable(variable,getValorVariable(variable) - 1);
                }
            }
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion)
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            match(")");
            match("{");
            ListaDeCasos(evaluacion);
            if(getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion);  
                }
                else
                {
                    Instruccion(evaluacion);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion)
        {
            match("case");
            Expresion();
            stack.Pop();
            match(":");
            ListaInstruccionesCase(evaluacion);
            if(getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if(getContenido() == "case")
            {
                ListaDeCasos(evaluacion);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion()
        {
            Expresion();
            
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();

            float e2 = stack.Pop();
            float e1 = stack.Pop();

            switch (operador)
            {
                case "==":
                    return  e1 == e2;
                case ">":
                    return  e1 > e2;
                case "<":
                    return  e1 < e2; 
                case ">=":
                    return  e1 >= e2;
                case "<=":
                    return  e1 <= e2;
                default:
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            match("if");
            match("(");
            //Requerimiento 4
            bool validarIf = Condicion();
            match(")");

            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf);  
            }
            else
            {
                Instruccion(validarIf);
            }
            if (getContenido() == "else")
            {
                match("else");
                //Requerimiento 4 
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarIf);
                }
                else
                {
                    Instruccion(validarIf);
                }
            }
        }

        //Printf -> printf(cadena|expresion);
        private void Printf(bool evaluacion)
        {
            match("printf");
            match("(");
            if(getClasificacion() == Tipos.Cadena)
            {
                if(evaluacion)
                {
                    Console.Write(EliminaComillas(getContenido()));
                }
                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                float resulatado = stack.Pop();
                if(evaluacion)
                {
                    Console.Write(resulatado);//imprimimos en la consola
                }
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena,& identificador);
        private void Scanf(bool evaluacion)    
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");

            if (ExisteVariable(getContenido()) == false)
            {
                throw new Error("Error de sintaxis, la variable no ha sido declarada <" + getContenido() + "> en linea: " + linea, log);
            }

            if(evaluacion)
            {
                string sValor =""+ Console.ReadLine(); //Capturamos en pantalla
                //Requerimiento 5.- el redLine debe capturar un nuemero, pero si manda una cadena debemos levantar una excepción
                //si el sValor es un numero, modifica el valor, sino no y levantamos una excepción de que no es un número :)
                ModificaVariable(getContenido(),float.Parse(sValor));
            }

            match(Tipos.Identificador);
            match(")");
            match(";");
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
                
                if(dominante < EvaluaNuemro(float.Parse(getContenido())))
                {
                    dominante = EvaluaNuemro(float.Parse(getContenido()));
                }

                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
                
            }
            else if (getClasificacion() == Tipos.Identificador)
            { 
                if (ExisteVariable(getContenido()) == false)
                {
                    throw new Error("Error de sintaxis, la variable no ha sido declarada <" + getContenido() + "> en linea: " + linea, log);
                }
                if(dominante < getTipoVariable(getContenido()))
                {
                    dominante = getTipoVariable(getContenido());
                }
                
                match(Tipos.Identificador);
                log.Write(getContenido() + " " );
                //Requerimiento 1. condicion parecida a la de arriba 
                stack.Push(getValorVariable(getContenido()));

               
            }
            else
            {
                bool huboCasteo = false;
                string sTipoDato = "";

                Variable.TipoDato casteo = Variable.TipoDato.Char;
               
                match("(");
                if(getClasificacion() == Tipos.TipoDato)
                {
                    huboCasteo = true;
                    sTipoDato = getContenido();

                    switch(sTipoDato)
                    {
                        case  "char":
                            casteo = Variable.TipoDato.Char;
                            break;
                        case  "int":
                            casteo = Variable.TipoDato.Int;
                            break;
                        case  "float":
                            casteo = Variable.TipoDato.Float;
                            break;
                    }

                    match(Tipos.TipoDato);
                    match(")");
                }
                Expresion();
               
                if(huboCasteo)
                {
                    //Requerimiento 2.- Actualizar el dominante en base a la variable.TipoDato
                    dominante = casteo;
                    float valorCastear = stack.Pop();
                    stack.Push(ConvertirDato(valorCastear,sTipoDato));
                    
                    //sacar un elemento del satck
                    //convierto ese valor al equivalente en casteo 
                    //ej. si el casteo es char y el pop regresa un 256 
                    //el valor equivalente al casteo es un 0
                    //uasar metodo Requerimiento 3
                }
            }
        }
    }
}