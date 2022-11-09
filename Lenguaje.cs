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
                        c) Programar el printf y el scanf en assembler
    Requerimiento 4.- Programar 
                        a) el eslse en assembler
                        b) Programar el for en assembler
    Requerimiento 5.- Programar 
                        a) el while en assembler
                        b)do while en assembler

*/
namespace Semantica
{
    
    public class Lenguaje : Sintaxis, IDisposable
    {
        List<Variable> listaVariables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        int cIf;
        int cFor;
        int cWhile;
        int cDo;
        public Lenguaje()
        {
            cDo = cWhile = cIf = cFor = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cDo = cWhile = cIf = cFor = 0;
        }
        public void Dispose()
        {
            Console.WriteLine("Destructor de Lenguaje");
            //Libera el recurso
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
            asm.WriteLine("ORG 100h");
            Libreria();
            Variables();
            VariablesASM();
            Main();
            this.DisplayVariables();
            asm.WriteLine("RET");
            asm.WriteLine("DEFINE_SCAN_NUM");
            asm.WriteLine("DEFINE_PRINT_NUM");
            asm.WriteLine("DEFINE_PRINT_NUM_UNS");
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
        
        //Main -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(true,true); 
        }

        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion, bool ejecutaASM)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, ejecutaASM);
            }
            match("}"); 
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion, bool ejecutaASM)
        {
            Instruccion(evaluacion, ejecutaASM);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, ejecutaASM);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion, bool ejecutaASM)
        {
            Instruccion(evaluacion, ejecutaASM);
            if (getContenido() != "case" && getContenido() !=  "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion, ejecutaASM);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion, bool ejecutaASM)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion, ejecutaASM);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion, ejecutaASM);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion, ejecutaASM);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion, ejecutaASM);
            }
            else if(getContenido() == "do")
            {
                Do(evaluacion, ejecutaASM);
            }
            else if(getContenido() == "for")
            {
                For(evaluacion, ejecutaASM);
            }
            else if(getContenido() == "switch")
            {
                Switch(evaluacion, ejecutaASM);
            }
            else
            {
                Asignacion(evaluacion, ejecutaASM);
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
        private void Asignacion(bool evaluacion, bool ejecutaASM)
        {
            if(ExisteVariable(getContenido()) == false)
            {
                throw new Error("Error de sintaxis, la variable no ha sido declarada <" + getContenido() + "> en linea: " + linea, log);
            }

            log.WriteLine();
            string nombre = getContenido();
            match(Tipos.Identificador);
            log.Write(nombre + " = ");
            dominante = Variable.TipoDato.Char;

            if(getClasificacion() == Tipos.Asignacion)
            {
                bool identificador = false;
                match(Tipos.Asignacion);
                string var = getContenido();
                if(Tipos.Identificador == getClasificacion())
                {
                    identificador = true;
                }
                Expresion(ejecutaASM);
                match(";");

                float resultado = stack.Pop();
                if(ejecutaASM)
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

                        if (ejecutaASM)
                        {
                            asm.WriteLine("MOV " + nombre + ", AX");
                            /*if (identificador)
                            {
                                //asm.WriteLine("MOV AX, "+ var);
                                asm.WriteLine("MOV " + nombre + ", AX");
                            }
                            else
                            {
                                asm.WriteLine("MOV " + nombre + ", AX");
                            }*/
                        }
                        //aqui se hace la modificacion de la variable en ensamblador
                    }
                }
                else
                {
                    throw new Error("Error de semantica, NO podemos asiganar un <" + dominante + "> a un "+getTipoVariable(nombre)+ " en linea: " + linea, log);
                }
                //if(ejecutaASM)
                  //asm.WriteLine("MOV " + nombre + ", AX");
                
            }
            else if(getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {
                dominante = Variable.TipoDato.Char;
                //float resultado = Incremento(evaluacion,nombre,ejecutaASM);
                Incremento(evaluacion,nombre,ejecutaASM);
                float resultado = stack.Pop();
                if(ejecutaASM)
                    asm.WriteLine("POP AX");

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
                    if (ejecutaASM)
                    {
                         asm.WriteLine("MOV " + nombre + ", AX");
                    }
                }
                else
                {
                    throw new Error("Error de semantica, NO podemos asiganar un <" + dominante + "> a un "+getTipoVariable(nombre)+ " en linea: " + linea, log);
                }
                match(";");
                //Requerimiento 1.b
                //poner los match's
                //Requerimiento 2.c
            }
           
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion, bool ejecutaASM)
        {
            if(ejecutaASM)
                cWhile++;
            string lb_InicioWhile = "InicioWhile" + cWhile;
            string lb_FinWhile = "FinWhile" + cWhile;
            match("while");
            match("(");
            bool validarWhile;
            int posWhile = getContCaracter() - getContenido().Length;
            int lineaActual = linea;
            if(ejecutaASM)
                asm.WriteLine(lb_InicioWhile+":");
            do
            {
                validarWhile = Condicion(lb_FinWhile,ejecutaASM);
                
                if (!evaluacion)
                {
                    validarWhile = false;
                }
                match(")");
                if (getContenido() == "{") 
                {
                    BloqueInstrucciones(validarWhile,ejecutaASM);
                }
                else
                {
                    Instruccion(validarWhile,ejecutaASM);
                }
                if(validarWhile)
                {
                    setContCaracter(posWhile);
                    linea = lineaActual;
                    CambiarPosArchivo(posWhile);
                    NextToken();
                } 

                if (ejecutaASM)
                {
                    asm.WriteLine("JMP "+lb_InicioWhile);
                    asm.WriteLine(lb_FinWhile +":");
                }
                ejecutaASM = false;
            }
            while(validarWhile);
            
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion, bool ejecutaASM)
        {
            if(ejecutaASM)
                cDo++;
            string lb_inicioDo = "InicioDoWhile" + cDo;
            string lb_finDo = "FinDoWhile" + cDo;
            bool validarDo;
            int posDo = getContCaracter() - getContenido().Length;
            int lineaActual = linea;
            
            if(ejecutaASM)
                asm.WriteLine(lb_inicioDo+":");
            do
            {
                match("do");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, ejecutaASM);
                }
                else
                {
                    Instruccion(evaluacion, ejecutaASM);
                } 
                match("while");
                match("(");
                //Requerimmiento 4
                validarDo = Condicion(lb_finDo,ejecutaASM);
                if (!evaluacion)
                {
                    validarDo = evaluacion;
                }
                if(validarDo)
                {
                    setContCaracter(posDo);
                    linea = lineaActual;
                    CambiarPosArchivo(posDo);
                    NextToken();
                } 
                if (ejecutaASM)
                {
                    asm.WriteLine("JMP " + lb_inicioDo);
                    asm.WriteLine(lb_finDo + ":");
                }
                ejecutaASM = false;
            }
            while(validarDo);
            match(")");
            match(";");
        }
        public void CambiarPosArchivo(int posArchivo)
        {
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(posArchivo,SeekOrigin.Begin);
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion, bool ejecutaASM)
        {
            string etiquetaInicioFor = "inicioFor" +cFor;
            string etiquetaFinFor = "finFor" +cFor++;
            int posArchivo; 
            bool validarFor;
            float valorInc = 0;
            string variableControl ="";
            int lineaActual;
            match("for");
            match("(");
            Asignacion(evaluacion, ejecutaASM);
            posArchivo = getContCaracter() - getContenido().Length;
            lineaActual = linea;
            if(ejecutaASM)
               asm.WriteLine(etiquetaInicioFor+":");
            do
            {
                validarFor = Condicion(etiquetaFinFor,ejecutaASM);
                if(!evaluacion)
                {
                    validarFor = false;
                }
                match(";");
                variableControl = getContenido();
                if (ExisteVariable(variableControl) == false)
                {
                    throw new Error("Error de sintaxis, la variable no ha sido declarada <" + getContenido() + "> en linea: " + linea, log);
                }
                match(Tipos.Identificador);
                //valorInc = Incremento(validarFor,variableControl, ejecutaASM);
                Incremento(validarFor,variableControl, ejecutaASM);
                //Requerimmiento 1.d
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor, ejecutaASM);  
                }
                else
                {
                    Instruccion(validarFor, ejecutaASM);
                }
                if (validarFor)
                {
                    setContCaracter(posArchivo);
                    linea = lineaActual;
                    CambiarPosArchivo(posArchivo);
                    NextToken();   
                }
                valorInc = stack.Pop();
                ModificaVariable(variableControl, valorInc);
                if (ejecutaASM)
                {
                    asm.WriteLine("POP AX");
                    asm.WriteLine("MOV " + variableControl + ", AX");
                }  
                if (ejecutaASM)
                {
                    asm.WriteLine("JMP "+etiquetaInicioFor);
                    asm.WriteLine(etiquetaFinFor +":");
                }
                ejecutaASM = false;
            }while(validarFor);
            //asm.WriteLine(etiquetaFinFor + ":");
        }
        //Incremento -> Identificador ++ | --
        public void Incremento(bool evaluacion, string variable, bool ejecutaASM)
        {
            float valorExpresion;
            float valorVar;  
            switch (getContenido()){
                case "++":
                    match("++");
                    //Console.WriteLine("INC " + variable);
                    valorVar = getValorVariable(variable);
                    if (ejecutaASM)
                    {
                        asm.WriteLine("MOV AX, " + variable);
                        asm.WriteLine("ADD AX, 1");
                        asm.WriteLine("PUSH AX");
                    }
                    stack.Push(valorVar + 1);
                    break;
                case "--":
                    match("--");
                    valorVar = getValorVariable(variable);
                    if (ejecutaASM)
                    {
                        asm.WriteLine("MOV AX, " + variable);
                        asm.WriteLine("SUB AX, 1");
                        asm.WriteLine("PUSH AX");
                    }
                    stack.Push(valorVar - 1);
                    break;
                case "+=":
                    match("+=");
                    Expresion(ejecutaASM);
                    valorExpresion = stack.Pop();
                    if (ejecutaASM)
                        asm.WriteLine("POP AX");
                    valorVar = getValorVariable(variable);
                    if (ejecutaASM)
                    {
                        asm.WriteLine("MOV AX, " + variable);
                        asm.WriteLine("MOV BX, " + valorExpresion);
                        asm.WriteLine("ADD AX, BX");
                        asm.WriteLine("PUSH AX");
                    }
                    stack.Push(valorVar + valorExpresion);
                    break;
                case "-=":  
                    match("-=");
                    Expresion(ejecutaASM);
                    valorExpresion  = stack.Pop();
                    if (ejecutaASM)
                    {
                        asm.WriteLine("POP AX");  
                    }
                    valorVar = getValorVariable(variable);
                    if (ejecutaASM)
                    {
                        asm.WriteLine("MOV AX, " + variable);
                        asm.WriteLine("MOV BX, " + valorExpresion);
                        asm.WriteLine("SUB AX,BX");
                        asm.WriteLine("PUSH AX");
                    }
                    stack.Push(valorVar - valorExpresion);
                    break;
                case "*=":
                    match("*=");
                    Expresion(ejecutaASM);
                    valorExpresion  = stack.Pop();
                    if(ejecutaASM)
                        asm.WriteLine("POP AX");
                    valorVar = getValorVariable(variable);

                    if (ejecutaASM)
                    {
                        asm.WriteLine("MOV AX, " + variable);
                        asm.WriteLine("MOV BX, " + valorExpresion);
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("PUSH AX");
                    }
                    stack.Push(valorVar * valorExpresion);
                    break;
                case "/=":
                    match("/=");
                    Expresion(ejecutaASM);
                    valorExpresion  = stack.Pop();
                    if (ejecutaASM)
                        asm.WriteLine("POP AX");
                    valorVar = getValorVariable(variable);
                    if (ejecutaASM)
                    {
                        asm.WriteLine("MOV AX, " + variable);
                        asm.WriteLine("MOV BX, " + valorExpresion);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH AX");
                    }
                    stack.Push(valorVar / valorExpresion);
                    break;
                case "%=":
                    match("%=");
                    Expresion(ejecutaASM);
                    valorExpresion = stack.Pop();
                    if (ejecutaASM)
                        asm.WriteLine("POP AX");
                    valorVar = getValorVariable(variable);
                    if (ejecutaASM)
                    {
                        asm.WriteLine("MOV AX, " + variable);
                        asm.WriteLine("MOV CX, " + valorExpresion);
                        asm.WriteLine("DIV CX");
                        asm.WriteLine("PUSH DX");
                    }
                     stack.Push(valorVar % valorExpresion);
                    break;
            }
        }
        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion, bool ejecutaASM)
        {
            match("switch");
            match("(");
            Expresion(ejecutaASM);
            stack.Pop();
            if (ejecutaASM)
                asm.WriteLine("POP AX");
            match(")");
            match("{");
            ListaDeCasos(evaluacion, ejecutaASM);
            if(getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, ejecutaASM);  
                }
                else
                {
                    Instruccion(evaluacion, ejecutaASM);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion, bool ejecutaASM)
        {
            match("case");
            Expresion(ejecutaASM);
            stack.Pop();
            if (ejecutaASM)
                asm.WriteLine("POP AX");
            match(":");
            ListaInstruccionesCase(evaluacion, ejecutaASM);
            if(getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if(getContenido() == "case")
            {
                ListaDeCasos(evaluacion, ejecutaASM);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta,bool ejecutaASM)
        {
            Expresion(ejecutaASM);
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion(ejecutaASM);

            float e2 = stack.Pop();
            if (ejecutaASM)
                asm.WriteLine("POP BX");
            float e1 = stack.Pop();
            if (ejecutaASM)
            {
                asm.WriteLine("POP AX");
                asm.WriteLine("CMP AX,BX");
            }
            switch (operador)
            {
                case "==":
                    if (ejecutaASM)
                        asm.WriteLine("JNE " + etiqueta);
                    return  e1 == e2;
                case ">":
                    if (ejecutaASM)
                        asm.WriteLine("JLE " + etiqueta);
                    return  e1 > e2;
                case "<":
                    if (ejecutaASM)
                        asm.WriteLine("JGE " + etiqueta);
                    return  e1 < e2; 
                case ">=":
                    if (ejecutaASM)
                        asm.WriteLine("JL " + etiqueta);
                    return  e1 >= e2;
                case "<=":
                    if (ejecutaASM)
                        asm.WriteLine("JG " + etiqueta);
                    return  e1 <= e2;
                default:
                    if (ejecutaASM)
                        asm.WriteLine("JE " + etiqueta);
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion, bool ejecutaASM)
        {
            if(ejecutaASM)
                cIf++;
            string lb_If = "if" + cIf;
            string lb_else = "else" + cIf;
            match("if");
            match("(");
            //Requerimiento 4
            bool validarIf = Condicion(lb_If,ejecutaASM);
            bool validarElse = false;
            if(!evaluacion)
            {
                validarIf = false;
            }
            match(")");

            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf, ejecutaASM);  
            }
            else
            {
                Instruccion(validarIf, ejecutaASM);
            }
            if (ejecutaASM)
            {
                asm.WriteLine("JMP " + lb_else);
                asm.WriteLine(lb_If + ":");
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
                    BloqueInstrucciones(validarElse, ejecutaASM);
                }
                else
                {
                    Instruccion(validarElse, ejecutaASM);
                }
            }

            if(ejecutaASM)
                asm.WriteLine(lb_else +":");
        }
        public void SaltosDeLinea(string cadena)
        {
            if (cadena.Contains("\n"))
            {
                string[] subs = cadena.Split("\n");
                for(int i = 0; i < subs.Length; i++)
                {
                   // subs[i] = subs[i].Replace("\n","");
                   //Console.WriteLine(subs[i]);
                   if(cadena == "\n")
                   {
                       asm.WriteLine("PRINTN");
                   }
                   else
                   {
                       asm.WriteLine("PRINT "+ "\"" + subs[i] + "\"");
                       asm.WriteLine("PRINTN");
                   }
                   
                }
            }
            else
            {
                asm.WriteLine("PRINT "+ "\"" + cadena + "\"");
            }
        }
        //Printf -> printf(cadena|expresion);
        private void Printf(bool evaluacion, bool ejecutaASM)
        {
            string cadena;
            match("printf");
            match("(");
            if(getClasificacion() == Tipos.Cadena)
            {
                cadena = EliminaComillas(getContenido());
                if(evaluacion)
                {
                    Console.Write(cadena);
                }
                if(ejecutaASM)
                    SaltosDeLinea(cadena);
                match(Tipos.Cadena);
            }
            else
            {
                Expresion(ejecutaASM);
                float resulatado = stack.Pop();
                 
                if(evaluacion)
                {
                    Console.Write(resulatado);
                    //codigo ensamblador para imprimir una variable
                    if (ejecutaASM)
                    {
                        asm.WriteLine("POP AX");
                        asm.WriteLine("CALL PRINT_NUM");
                    }
                }
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena,& identificador);
        private void Scanf(bool evaluacion, bool ejecutaASM)    
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
                string sValor =""+ Console.ReadLine();
                if(float.TryParse(sValor,out float valor))
                {
                    ModificaVariable(getContenido(),float.Parse(sValor));
                }
                else
                {
                    throw new Error("\nError de sintaxis, el valor no es un numero <" + getContenido() + "> en linea: " + linea, log);
                }
                if (ejecutaASM)
                {
                    asm.WriteLine("CALL SCAN_NUM");
                    asm.WriteLine("MOV "+ getContenido() + ",CX");
                    //asm.WriteLine("PRINT \'\'");
                }
                //asm.WriteLine("SCAN_NUM " + getContenido());
            }
            match(Tipos.Identificador);
            match(")");
            match(";");
        }

        //Expresion -> Termino MasTermino
        private void Expresion(bool ejecutaASM)
        {
            Termino(ejecutaASM);
            MasTermino(ejecutaASM);
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(bool ejecutaASM)
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino(ejecutaASM);
                log.Write(operador+" ");
                float n1 = stack.Pop();
                if (ejecutaASM)
                    asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                if (ejecutaASM)
                    asm.WriteLine("POP AX");
                switch (operador){
                    case "+":
                        stack.Push(n1+n2);
                        if (ejecutaASM)
                        {
                            asm.WriteLine("ADD AX,BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "-":
                        stack.Push(n2-n1);
                        if (ejecutaASM)
                        {
                            asm.WriteLine("SUB AX,BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino(bool ejecutaASM)
        {
            Factor(ejecutaASM);
            PorFactor(ejecutaASM);
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor(bool ejecutaASM)
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor(ejecutaASM);
                log.Write(operador + " ");
                float n1 = stack.Pop();
                if (ejecutaASM)
                    asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                if (ejecutaASM)
                    asm.WriteLine("POP AX");
                //Requerimiento 1.a
                switch (operador)
                {
                    case "*":
                        stack.Push(n2*n1);
                        if (ejecutaASM)
                        {
                            asm.WriteLine("MUL BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "/":
                        stack.Push(n2/n1);
                        if (ejecutaASM)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "%":
                        stack.Push(n2 % n1);
                        if (ejecutaASM)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH DX");
                        }
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor(bool ejecutaASM)
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " " ); 
                if(dominante < EvaluaNuemro(float.Parse(getContenido())))
                {
                    dominante = EvaluaNuemro(float.Parse(getContenido()));
                }

                stack.Push(float.Parse(getContenido()));
                if (ejecutaASM)
                {
                    asm.WriteLine("MOV AX,"+getContenido());
                    asm.WriteLine("PUSH AX");
                }
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
                if (ejecutaASM)
                {
                    asm.WriteLine("MOV AX,"+getContenido());
                    asm.WriteLine("PUSH AX");
                }
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
                    
                }
                if(getContenido() == "(")
                {
                    match("(");
                    Expresion(ejecutaASM);
                    match(")");
                }
                else
                {
                    Expresion(ejecutaASM);
                }
                if(huboCasteo)
                {
                    dominante = casteo;
                    string nombreVar = getContenido();
                    float valor;
                    float valorCastear = stack.Pop();
                    if (ejecutaASM)
                        asm.WriteLine("POP AX");
                    valor = ConvertirDato(valorCastear,sTipoDato);
                    ModificaVariable(nombreVar,valor);
                    stack.Push(valor);
                    if (ejecutaASM)
                    {
                        asm.WriteLine("MOV AX,"+valor);
                        asm.WriteLine("PUSH AX");
                    }
                }
            }
        }
    }
}