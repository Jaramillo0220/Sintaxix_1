using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Data;
using System.Text;

/*
    REQUERIMIENTOS
    1) Indicar en el error léxico o sintáctico el número de línea y caracter
    2) En el log colocar el nombre del archivo a compilar, la fecha y la hora, ejemplo:
        Programa: prueba.cpp
        Fecha: 11/11/2024
        Hora: 3:25p.m.
    3) Agregar el resto de asignaciones:
        ID = Expresion
        ID++
        ID--
        ID IncrementoTermino Expresion
        ID IncrementoFactor Expresion
        ID = Console.Read()
        ID = Console.ReadLine()
    4) Emular el Console.Write() y Console.WriteLine()
    5) Emular el Console.Read() y Console.ReadLine()
*/

namespace Sintaxis_1
{
    public class Lexico : Token, IDisposable
    {
        public StreamReader archivo;
        public StreamWriter log;
        public StreamWriter asm;
        public int linea = 1;
        const int F = -1;

        const int E = -2;

        int[,] TRAND = {
                        { 0,  1,  2,  33, 1,  12, 14, 8,  9,  10, 11, 23, 16, 16, 18, 20, 21, 26, 25, 27, 29, 32, 34, 0,  F, 33},
                        { F,  1,  1,  F,  1,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  2,  3,  5,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { E,  E,  4,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E, E},
                        { F,  F,  4,  F,  5,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { E,  E,  7,  E,  E,  6,  6,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E, E},
                        { E,  E,  7,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E, E},
                        { F,  F,  7,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  13, F,  F,  F,  F,  F,  13, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  13, F,  F,  F,  F,  13, F,  F,  F,  F,  F,  F,  15, F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  17, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  19, F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  19, F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  22, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  24, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  24, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  24, F,  F,  F,  F,  F,  F,  24, F,  F,  F,  F,  F,  F, F},
                        {27,  27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 28, 27, 27, 27, 27, E, 27},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        {30,  30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30,30},
                        { E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  31, E,  E,  E,  E, E},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  32, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F},
                        { F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  17, 36, F,  F,  F,  F,  F,  F,  F,  F,  F,  35, F,  F, F},
                        {35,  35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 0,  35,35},
                        {36,  36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 37, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, E,36},
                        {36,  36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 37, 36, 36, 36, 36, 36, 36, 36, 36, 36, 0,  36, E,36},
                };
        public Lexico()
        {
            log = new StreamWriter("prueba.log");
            asm = new StreamWriter("prueba.asm");
            log.AutoFlush = true;
            asm.AutoFlush = true;

            if (File.Exists("prueba.cpp"))
            {
                archivo = new StreamReader("prueba.cpp");
            }
            else
            {
                throw new Error("El archivo prueba.cpp no existe", log);
            }
        }
        public Lexico(string nombreArchivo)
        {
            string nombreArchivoWithoutExt = Path.GetFileNameWithoutExtension(nombreArchivo);

            if (File.Exists(nombreArchivo))
            {
                log = new StreamWriter(nombreArchivoWithoutExt + ".log");
                asm = new StreamWriter(nombreArchivoWithoutExt + ".asm");
                log.AutoFlush = true;
                asm.AutoFlush = true;
                archivo = new StreamReader(nombreArchivo);

                // Agregar encabezado al log
                log.WriteLine("Programa: " + nombreArchivo);
                log.WriteLine("Fecha: " + DateTime.Now.ToString("dd/MM/yy"));
                log.WriteLine("Hora: " + DateTime.Now.ToString("HH:mm"));
                log.WriteLine("-----------------------------------------");
            }
            else if (Path.GetExtension(nombreArchivo) != ".cpp")
            {
                throw new ArgumentException("El archivo debe ser de extensión .cpp");
            }
            else
            {
                throw new FileNotFoundException("El archivo " + nombreArchivo + " no existe");
            }
        }

        public void Dispose()
        {
            archivo.Close();
            log.Close();
            asm.Close();
        }

        private int Columna(char c)
        {

            if (c == '\n')
            {
                return 23;
            }
            else if (finArchivo())
            {
                return 24;
            }
            else if (char.IsWhiteSpace(c))
            {
                return 0;
            }
            else if (char.ToLower(c) == 'e')
            {
                return 4;
            }
            else if (char.IsLetter(c))
            {
                return 1;
            }
            else if (char.IsDigit(c))
            {
                return 2;
            }
            else if (c == '.')
            {
                return 3;
            }
            else if (c == '+')
            {
                return 5;
            }
            else if (c == '-')
            {
                return 6;
            }
            else if (c == ';')
            {
                return 7;
            }
            else if (c == '{')
            {
                return 8;
            }
            else if (c == '}')
            {
                return 9;
            }
            else if (c == '?')
            {
                return 10;
            }
            else if (c == '=')
            {
                return 11;
            }
            else if (c == '*')
            {
                return 12;
            }
            else if (c == '%')
            {
                return 13;
            }
            else if (c == '&')
            {
                return 14;
            }
            else if (c == '|')
            {
                return 15;
            }
            else if (c == '!')
            {
                return 16;
            }
            else if (c == '<')
            {
                return 17;
            }
            else if (c == '>')
            {
                return 18;
            }
            else if (c == '"')
            {
                return 19;
            }
            else if (c == '\'')
            {
                return 20;
            }
            else if (c == '#')
            {
                return 21;
            }
            else if (c == '/')
            {
                return 22;
            }

            return 25;
        }
        private void Clasifica(int estado)
        {
            switch (estado)
            {
                case 1: setClasificacion(Tipos.Identificador); break;
                case 2: setClasificacion(Tipos.Numero); break;
                case 8: setClasificacion(Tipos.FinSentencia); break;
                case 9: setClasificacion(Tipos.InicioBloque); break;
                case 10: setClasificacion(Tipos.FinBloque); break;
                case 11: setClasificacion(Tipos.OperadorTernario); break;

                case 12: // OPERADOR TERMINO
                case 14: setClasificacion(Tipos.OperadorTermino); break;

                case 13: setClasificacion(Tipos.IncrementoTermino); break;
                case 15: setClasificacion(Tipos.Puntero); break;

                case 16: // OPERADOR FACTOR
                case 34: setClasificacion(Tipos.OperadorFactor); break;

                case 17: setClasificacion(Tipos.IncrementoFactor); break;

                case 18: // CARACTER
                case 20:
                case 29:
                case 32:
                case 33: setClasificacion(Tipos.Caracter); break;

                case 19: //OPERADOR LOGICO
                case 21: setClasificacion(Tipos.OperadorLogico); break;

                case 22: // OPERADOR RELACIONAL
                case 24:
                case 25:
                case 26: setClasificacion(Tipos.OperadorRelacional); break;

                case 23: setClasificacion(Tipos.Asignacion); break;

                case 27: setClasificacion(Tipos.Cadena); break;

            }
        }
        public void nextToken()
        {
            char c;
            string buffer = "";
            int estado = 0;
            while (estado >= 0)
            {
                c = (char)archivo.Peek();
                estado = TRAND[estado, Columna(c)];
                Clasifica(estado);
                if (estado >= 0)
                {
                    archivo.Read();
                    if (c == '\n')
                    {
                        linea++;
                    }
                    if (estado > 0)
                    {
                        buffer += c;
                    }
                    else
                    {
                        buffer = "";
                    }
                }
            }
            if (estado == E)
            {
                if (getClasificacion() == Tipos.Cadena)
                {
                    throw new Error("léxico, se esperaba un cierre de cadena", log, linea);
                }
                else if (getClasificacion() == Tipos.Caracter)
                {
                    throw new Error("léxico, se esperaba un cierre de comilla simple", log, linea);
                }
                else if (getClasificacion() == Tipos.Numero)
                {
                    throw new Error("léxico, se esperaba un dígito", log, linea);
                }
                else
                {
                    throw new Error("léxico, se espera cierre de comentario", log, linea);
                }
            }
            if (!finArchivo())
            {
                setContenido(buffer);
                if (getClasificacion() == Tipos.Identificador)
                {
                    switch (getContenido())
                    {
                        case "char":
                        case "int":
                        case "float":
                            setClasificacion(Tipos.TipoDato);
                            break;
                        case "if":
                        case "else":
                        case "do":
                        case "while":
                        case "for":
                            setClasificacion(Tipos.PalabraReservada);
                            break;
                    }
                }
                log.WriteLine(getContenido() + " ------ " + getClasificacion());
            }
        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }

    }
}

/*

    EXPRESIÓN REGULAR
    Es un método formal el cual a través de una secuencia de 
    carácteres define un patrón de búsqueda.

    a) Reglas BNF
    b) Reglas BNF extendidas
    c) Operaciones aplicadas al lenguaje

        Operaciones Aplicadas al Lenguaje (OAF)

        1. Concactenación simple. (.)
        2. Concatenación exponencial. (^)
        3. Cerradura de Kleene. (*)
        4. Cerradura positiva. (+)
        5. Cerradura Epsilon. (?)
        6. Operador  (|)
        7. Parentesis, agrupación. ()

        L = {A, B, C, D, ..., Z, a, b, c, d, ... , z}
        D = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}

        1. L.D, LD
        2. L^3 = LLL, D^5 = DDDDD
        3. L* = Cero o más
        4. L+ = Una o más
        5. L? = Cero o una vez (Opcional)
        6. L | D = Letra o Digíto
        7. (LD)L? = Letra seguido de dígito y una letra opcional

        Producción Gramátical

        Clasificación del token -> Expresion regular

        Identificador -> L(L|D)*
        Número -> D+(.D+)?(E(+|-)?D+)?
        Fin de Sentencia -> ;
        Inicio de Bloque -> {
        Fin de Bloque -> }
        Operador Ternario -> ?
        Operador de Término -> +|-
        Operador de Factor -> *|/|%
        Incremento de Término -> (+|-)((+|-)|=)
        Incremento de Factor -> (*|/|%)=
        Operador Lógico -> &&||||!
        Operador Relacional -> >=?|<(>|=)?|==|!=
        Puntero -> ->
        Asignación -> =
        Cadena -> C*
        Caracter -> 'C'|#D*|Lambda

    AUTÓMATA
    Modelo matemático que representa una expresión regular a través
    de una grafo que consiste en un conjunto de estados bien definidos, 
    un estado inicial, un alfabeto de entrada y una función de transición.

*/