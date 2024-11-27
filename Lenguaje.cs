using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sintaxix_1;

/*
    REQUERIMIENTOS
    1. Concatenaciones ----> COMPLETADO
    2. Inicializar una variable desde la declaración ----> COMPLETADO
    3. Evaluar las expresiones matemáticas ----> COMPLETADO
    4. Levantar si en el Console.ReadLine() no ingresan números 
    5. Modificar la variable con el resto de operadores (Incremento de factor y termino) 
    6. Hacer que funcione el else
*/

namespace Sintaxis_1
{
    public class Lenguaje : Sintaxis
    {
        Stack<float> s;
        List<Variable> t;
        List<Variable> l;
        public Lenguaje(string name) : base(name)
        {
            s = new Stack<float>();  // Inicialización del Stack
            t = new List<Variable>(); // Inicialización de la lista t
            l = new List<Variable>(); // Inicialización de la lista l
        }

        private void displayStack()
        {
            Console.WriteLine("Contenido del stack: ");
            foreach (float elemento in s)
            {
                Console.WriteLine(elemento);
            }
        }
        private void displayLista()
        {
            log.WriteLine("Lista de variables: ");
            foreach (Variable elemento in l)
            {
                log.WriteLine($"{elemento.GetNombre()} {elemento.GetTipoDato()} {elemento.GetValor()}");
            }
        }


        // ? Cerradura epsilon
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (getContenido() == "using")
            {
                Librerias();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            Main();
            displayLista(); // Agregada la llamada a displayLista() al final del método
        }



        // Console -> Console.(WriteLine|Write) (cadena | identificador concatenaciones?);
        private void console()
        {
            match("Console");
            match(".");

            if (getContenido() == "WriteLine")
            {
                match("WriteLine");
            }
            else if (getContenido() == "Write")
            {
                match("Write");
            }
            else
            {
                throw new Exception("Error de sintaxis: Se esperaba Write o WriteLine.");
            }

            match("(");

            // Procesar concatenaciones o una simple cadena/identificador
            string resultado = "";

            if (getClasificacion() == Tipos.Cadena)
            {
                resultado += getContenido().Trim('\"');
                match(Tipos.Cadena);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                // Buscar la variable en la lista y obtener su valor
                Variable? v = l.Find(variable => variable.GetNombre() == getContenido());
                if (v == null)
                {
                    throw new Exception($"Error: La variable {getContenido()} no está definida.");
                }
                resultado += v.GetValor();
                match(Tipos.Identificador);
            }


            // Procesar posibles concatenaciones
            while (getContenido() == "+")
            {
                match("+");

                if (getClasificacion() == Tipos.Cadena)
                {
                    resultado += getContenido().Trim('\"'); // Concatenar cadena sin comillas
                    match(Tipos.Cadena);
                }
                else if (getClasificacion() == Tipos.Identificador)
                {
                    // Simular obtener el valor del identificador
                    resultado += $"[Valor de {getContenido()}]";
                    match(Tipos.Identificador);
                }
                else
                {
                    throw new Exception("Error de sintaxis: Se esperaba una cadena o identificador después de '+'.");
                }
            }

            match(")");
            match(";");

            // Imprimir el resultado final
            Console.WriteLine(resultado);
        }




        //Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");

            if (getContenido() == "using")
            {
                Librerias();
            }
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;

            switch (getContenido())
            {
                case "int": tipo = Variable.TipoDato.Int; break;
                case "float": tipo = Variable.TipoDato.Float; break;
            }

            match(Tipos.TipoDato);
            ListaIdentificadores(tipo);
            match(";");
        }


        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);

            if (getContenido() == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }

        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            if (l.Find(Variable => Variable.GetNombre() == getContenido()) != null)
            {
                throw new Exception($"Error de sintaxis: La variable {getContenido()} ya existe. Línea: {linea}");
            }

            string nombre = getContenido();
            match(Tipos.Identificador);

            float valor = 0;

            if (getContenido() == "=")
            {
                match("=");

                if (getContenido() == "Console")
                {
                    match("Console");
                    match(".");
                    match("ReadLine");
                    match("(");
                    string input = Console.ReadLine();
                    if (!float.TryParse(input, out valor))
                    {
                        throw new Exception($"Error: '{input}' no es un número válido.");
                    }
                    match(")");
                }
                else
                {
                    Expresion();
                    valor = s.Pop();
                }
            }

            l.Add(new Variable(t, nombre, valor));

            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }


        // BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool execute)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(execute);
            }
            else
            {
                match("}");
            }
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool execute)
        {
            Instruccion(execute);
            if (getContenido() != "}")
            {
                ListaInstrucciones(execute);
            }
            else
            {
                match("}");
            }
        }

        // Instruccion -> console | If | While | do | For | Variables | Asignación
        private void Instruccion(bool execute)
        {
            if (getContenido() == "Console")
            {
                console();
            }
            else if (getContenido() == "if")
            {
                If(execute);
            }
            else if (getContenido() == "while")
            {
                While();
            }
            else if (getContenido() == "do")
            {
                Do();
            }
            else if (getContenido() == "for")
            {
                For();
            }
            else if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
                //match(";");
            }
        }
        // Asignacion -> Identificador = Expresion | ID++ | ID-- 
        // | ID IncrementoTermino Expresion | ID IncrementoFactor Expresion
        // | ID = Console.Read() | ID = Console.ReadLine()
        private void Asignacion()
        {
            // Buscar la variable en la lista
            Variable? v = l.Find(Variable => Variable.GetNombre() == getContenido());
            if (v == null)
            {
                throw new Error("La variable " + getContenido() + " NO está definida en la línea: " + linea);
            }

            match(Tipos.Identificador);

            if (getContenido() == "=")
            {
                match("=");

                // Manejar entrada desde Console.Read o Console.ReadLine
                if (getContenido() == "Console")
                {
                    match("Console");
                    match(".");
                    if (getContenido() == "Read")
                    {
                        match("Read");
                        match("(");
                        int input = Console.Read();
                        v.SetValor(input);
                        match(")");
                    }
                    else if (getContenido() == "ReadLine")
                    {
                        match("ReadLine");
                        match("(");
                        string input = Console.ReadLine();
                        if (!float.TryParse(input, out float valor))
                        {
                            throw new Exception($"Error: '{input}' no es un número válido.");
                        }
                        v.SetValor(valor);
                        match(")");
                    }
                }
                else
                {
                    // Evaluar una expresión
                    Expresion();
                    v.SetValor(s.Pop());
                }

                match(";");
            }
            else if (getContenido() == "++" || getContenido() == "--")
            {
                // Incremento o decremento
                if (getContenido() == "++")
                {
                    v.SetValor(v.GetValor() + 1);
                }
                else if (getContenido() == "--")
                {
                    v.SetValor(v.GetValor() - 1);
                }
                match(Tipos.IncrementoTermino);
                match(";");
            }
            else if (getContenido() == "+=" || getContenido() == "-=")
            {
                // Asignaciones incrementales
                string operador = getContenido();
                match(Tipos.IncrementoTermino);
                Expresion();
                float valor = s.Pop();

                if (operador == "+=")
                {
                    v.SetValor(v.GetValor() + valor);
                }
                else if (operador == "-=")
                {
                    v.SetValor(v.GetValor() - valor);
                }
                match(";");
            }
            else if (getContenido() == "*=" || getContenido() == "/=")
            {
                // Asignaciones de factor
                string operador = getContenido();
                match(Tipos.IncrementoFactor);
                Expresion();
                float valor = s.Pop();

                if (operador == "*=")
                {
                    v.SetValor(v.GetValor() * valor);
                }
                else if (operador == "/=")
                {
                    if (valor == 0)
                    {
                        throw new Exception("Error: División entre cero.");
                    }
                    v.SetValor(v.GetValor() / valor);
                }
                match(";");
            }
            else
            {
                throw new Exception("Error de sintaxis: Se esperaba un '=' u operador válido después del identificador.");
            }
        }


        // If -> if (Condicion) bloqueInstrucciones | instruccion
        // (else bloqueInstrucciones | instruccion)?
        private void If(bool execute2)
        {
            match("if");
            match("(");
            bool execute = Condicion();
            Console.WriteLine(execute);
            match(")");

            if (getContenido() == "{")
            {
                BloqueInstrucciones(execute);
            }
            else
            {
                Instruccion(execute);
            }

            if (getContenido() == "else")
            {
                match("else");

                if (getContenido() == "{")
                {
                    BloqueInstrucciones(false);
                }
                else
                {
                    Instruccion(false);
                }
            }
        }
        // Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            Expresion();
            float valor1 = s.Pop();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float valor2 = s.Pop();

            switch (operador)
            {
                case ">": return valor1 > valor2;
                case ">=": return valor1 >= valor2;
                case "<": return valor1 < valor2;
                case "<=": return valor1 <= valor2;
                case "==": return valor1 == valor2;
                default: return valor1 != valor2;
            }
        }
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");

            if (getContenido() == "{")
            {
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
            }
        }
        // Do -> do 
        // bloqueInstrucciones | intruccion 
        // while(Condicion);
        private void Do()
        {
            match("do");

            if (getContenido() == "{")
            {
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
            }

            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        // For -> for(Asignacion; Condicion; Asignación) 
        // BloqueInstrucciones | Intruccion
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            //match(";");
            Condicion();
            match(";");
            Asignacion();
            match(")");

            if (getContenido() == "{")
            {
                BloqueInstrucciones(true);
            }
            else
            {
                Instruccion(true);
            }
        }
        //Concatenaciones -> + cadena
        private void Concatenaciones()
        {
            match("+");

            if (getClasificacion() == Tipos.Cadena)
            {
                match(Tipos.Cadena);
            }
            else
            {
                match(Tipos.Identificador);
            }

            if (getContenido() == "+")
            {
                Concatenaciones();
            }
        }
        // Main -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones(true);
        }
        // Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        // MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();

                //Console.Write(operador + " ");

                float n1 = s.Pop();
                float n2 = s.Pop();
                switch (operador)
                {
                    case "+": s.Push(n2 + n1); break;
                    case "-": s.Push(n2 - n1); break;
                }
            }
        }

        // Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        // PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();

                //Console.Write(operador + " ");

                float n1 = s.Pop();
                float n2 = s.Pop();
                switch (operador)
                {
                    case "*": s.Push(n2 * n1); break;
                    case "/": s.Push(n2 / n1); break;
                    case "%": s.Push(n2 % n1); break;
                }
            }
        }

        // Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                s.Push(float.Parse(getContenido()));
                //Console.Write(getContent() + " ");
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.GetNombre() == getContenido());

                if (v == null)
                {
                    throw new Error("Sintaxis: la variable " + getContenido() + " no está definida " + linea);
                }

                s.Push(v.GetValor());
                //Console.Write(getContent() + " ");
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