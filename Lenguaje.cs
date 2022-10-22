//DIAZ GUERRERO MARCELA
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Text;
using System.Text.RegularExpressions;

/*
    Requerimiento 1.- Actulización: 
                        a) agregar el residuo de la divison en porfactor
                        b) agregar en Instrucción los incrementos de termino y los increnmentos de factor
                            a++, a--, a+=1, a-=1, a*=1, a/=1, a%=1,
                            donde el 1 puede ser una expresion, valida la expresion y hace el incremneto
                        c) programar el destructor de clase lexico para ejecutar cerrarArchivo
                            -> ejecutarse sin invocar a close.archivo()
                            -> #libreria especial con un contenedor

    Requerimiento 2.- Actulización:
                        c) marcar errores semanticos cuando los incrementos de termino o incrementos de factor superen el rango de la variable
                            y =  255; y++; //error
                        d) considerar el b) y c) para el for   
                            j=j+1, j=j-1 
                        e) Hacer funcionar el do y while
    Requerimiento 3.- Agregar:
                        a) considerar las variables y los casteos de las expresiones matematicas en ensamblador
                        b) considerar el residuo de la división en ensamblador, el residuo de la division queda en dx 

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
        ~Lenguaje()
        {
            Console.WriteLine("Destructor de Lenguaje");
            cerrar();
        }
        private float ConvertirDato(float valor, string sTipoDato)
        {
 
            switch(sTipoDato)
            {
                case  "char":
                    valor = valor%256;
                    break;
                case  "int":
                    valor = valor%65536;
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
        private void VariablesASM()
        {
            asm.WriteLine(";Variables");
            foreach (Variable v in listaVariables) // este for no necesita que pongamos que se incremente el contador
            {
                asm.WriteLine("\t"+v.getNombre()+" DW ? ");
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
                    break;
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
            asm.WriteLine("#make_COM#");
            asm.WriteLine("include 'emu8086.inc'");
            asm.WriteLine("ORG 1000h");
            Libreria();
            Variables();
            VariablesASM();
            Main();
            this.DisplayVariables();
            asm.WriteLine("RET");
            asm.WriteLine("END");
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
            if(getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {
                //Requerimiento 1.b
                //poner los match's
                //Requerimiento 2.c
            }
            else
            {
               
                Expresion();
                match(";");

                float resultado = stack.Pop();
                asm.WriteLine("POP AX");

                log.WriteLine(" = " + resultado);
                log.WriteLine();

                //Console.WriteLine(dominante);
                //Console.WriteLine(EvaluaNuemro(resultado));

                if(dominante < EvaluaNuemro(resultado))
                {
                    dominante = EvaluaNuemro(resultado);
                    
                }

                if( dominante <= getTipoVariable(nombre))
                {
                    if(evaluacion)
                    {
                        ModificaVariable(nombre, resultado);
                        //aqui se hace la modificacion de la variable en ensamblador
                        asm.WriteLine("MOV " + nombre + ", AX");
                    }
                }
                else
                {
                    throw new Error("Error de semantica, NO podemos asiganar un <" + dominante + "> a un "+getTipoVariable(nombre)+ " en linea: " + linea, log);
                }
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            //Requermiento 4
            bool validarWhile = Condicion();
            //encontrar una relacion entre evaluación y condicion para ejecutar el bloque de instrucciones
            if (!evaluacion)
            {
                validarWhile = false;
            }
            match(")");
            if (getContenido() == "{") 
            {
                BloqueInstrucciones(validarWhile);
            }
            else
            {
                Instruccion(validarWhile);
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
            if (!evaluacion)
            {
                validarDo = false;
            }
            match(")");
            match(";");
        }
        public void CambiarPosArchivo(int posArchivo)
        {
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(posArchivo,SeekOrigin.Begin);
        }

        // Incremento del for donde no se modifica el valor de la variable, solo lo retorna para que al final del for sea modificado
        private float IncrementoFor(bool evaluacion)
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
                    return getValorVariable(variable) + 1;
                }
            }
            else
            {
                match("--");
                if(evaluacion)
                {
                    return getValorVariable(variable) - 1;
                }
            }
            return 0;
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            int posArchivo; 
            bool validarFor;
            float valorInc = 0;
            string variableControl ="";
            int lineaActual;

            match("for");
            match("(");
            Asignacion(evaluacion);
                        
            //Requerimmiento 6:  a) guardar la direccion la posición del archivo de texto
            posArchivo = getContCaracter() - getContenido().Length;
            lineaActual = linea;
            do
            {
                //Requerimmiento 4 verificar que la condición se a verdadera
                validarFor = Condicion();
                if(!evaluacion)
                {
                    validarFor = false;
                }
                match(";");
                variableControl = getContenido();
                valorInc = IncrementoFor(validarFor);
                //Requerimmiento 1.d
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor);  
                }
                else
                {
                    Instruccion(validarFor);
                }
                if (validarFor)
                {
                    setContCaracter(posArchivo);
                    linea = lineaActual;
                    //c) Regresar a la posición de lectura del archivo
                    CambiarPosArchivo(posArchivo);
                    //d) Sacar otro token
                    NextToken();   
                }
                ModificaVariable(variableControl, valorInc);
            }while(validarFor);
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
            asm.WriteLine("POP AX");
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
            asm.WriteLine("POP AX");
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
            asm.WriteLine("POP AX");
            float e1 = stack.Pop();
            asm.WriteLine("POP BX");

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
            bool validarElse = false;
            if(!evaluacion)
            {
                validarIf = false;
            }
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
                if(evaluacion && !validarIf)
                {
                    validarElse= true;
                }
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarElse);
                }
                else
                {
                    Instruccion(validarElse);
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
                asm.WriteLine("POP AX");
                if(evaluacion)
                {
                    Console.Write(resulatado);
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
                if(float.TryParse(sValor,out float valor))
                {
                    ModificaVariable(getContenido(),float.Parse(sValor));
                }
                else
                {
                    throw new Error("\nError de sintaxis, el valor no es un numero <" + getContenido() + "> en linea: " + linea, log);
                }
                
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
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                switch (operador){
                    case "+":
                        stack.Push(n1+n2);
                        asm.WriteLine("ADD AX,BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "-":
                        stack.Push(n2-n1);
                        asm.WriteLine("SUB AX,BX");
                        asm.WriteLine("PUSH AX");
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
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                //Requerimiento 1.a
                switch (operador)
                {
                    case "*":
                        stack.Push(n2*n1);
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "/":
                        stack.Push(n2/n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "%":
                        stack.Push(n2 % n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH DX");
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
                asm.WriteLine("MOV AX,"+getContenido());
                asm.WriteLine("PUSH AX");
                match(Tipos.Numero);
                
            }
            else if (getClasificacion() == Tipos.Identificador)
            { 
                log.Write(getContenido() + " " );
                if (!ExisteVariable(getContenido()))
                {
                    throw new Error("Error de sintaxis, la variable no ha sido declarada <" + getContenido() + "> en linea: " + linea, log);
                }
                if(dominante < getTipoVariable(getContenido()))
                {
                    dominante = getTipoVariable(getContenido());
                }
                stack.Push(getValorVariable(getContenido()));
                match(Tipos.Identificador);
                
                //Requerimiento 1. condicion parecida a la de arriba 
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
                    match("(");
                }
                Expresion();
                match(")");
                if(huboCasteo)
                {
                    //Requerimiento 2.- Actualizar el dominante en base a la variable.TipoDato
                    dominante = casteo;
                    string nombreVar = getContenido();
                    float valor;
                    float valorCastear = stack.Pop();
                    asm.WriteLine("POP AX");
                    //sacar un elemento del satck
                    //convierto ese valor al equivalente en casteo 
                    //ej. si el casteo es char y el pop regresa un 256 
                    //el valor equivalente al casteo es un 0
                    //uasar metodo Requerimiento 3
                    valor = ConvertirDato(valorCastear,sTipoDato);
                    ModificaVariable(nombreVar,valor);
                    stack.Push(valor);
                }
            }
        }
    }
}