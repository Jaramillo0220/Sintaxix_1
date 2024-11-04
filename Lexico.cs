
using System;
using System.Collections.Generic;
using System.IO;
using System.Data; // Para DataSet
using ExcelDataReader; // Para ExcelDataReader
using System.Linq.Expressions;
using System.Runtime.InteropServices;

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
            {0, 1, 2, 33, 1, 12, 14, 8, 9, 10, 11, 23, 16, 16, 18, 20, 21, 26, 25, 27, 29, 32, 34, 0, F, 33},
            {F, 1, 1, F,  1, F,  F,  F, F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F, F },
            {F, F, 2, 3,  5, F,  F,  F, F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F, F },
            {E, E, 4, E,  E, E,  E,  E, E, E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E, E, E },
            {F, F, 4, 5,  F, F,  F,  F, F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F, F },
            {E, E, 7, E,  E, 6,  6,  E, E, E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E, E, E },
            {E, E, 7, E,  E, E,  E,  E, E, E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E,  E, E, E },
            {F, F, 7, F,  F, F,  F,  F, F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F, F },
            {F, F, F, F,  F, F,  F,  F, F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F, F },
            {F, F, F, F,  F, F,  F,  F, F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F, F },
            {F, F, F, F,  F, F,  F,  F, F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F, F },
            {F, F, F, F,  F, F,  F,  F, F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F, F, F },
            {F, F, F, F, F, 13, F, F, F, F, F, 13, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, 13, F, F, F, F, F, F, 15, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, 17, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, 19, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, 19, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, 22, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, 24, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, 24, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, 24, F, F, F, F, F, F, 24, F, F, F, F, F, F, F},
            {27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 28, 27, 27, 27, 27, E, 27},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30},
            {E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, 31, E, E, E, E, E},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, 32, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, 17, 36, F, F, F, F, F, F, F, F, F, 35, F, F, F},
            {35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 0, 35, 35},
            {36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 37, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36},
            {36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 36, 37, 36, 36, 36, 36, 36, 36, 36, 36, 36, 0, 36, 36, 36},
            };

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
            else if (c == 'E' || c == 'e')
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
                return 2;
            }
            else if (c == '-')
            {
                return 6;
            }
            else if (c == '+')
            {
                return 5;
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

                case 12:
                case 14: setClasificacion(Tipos.OperadorTermino); break;

                case 13: setClasificacion(Tipos.IncrementoTermino); break;
                case 15: setClasificacion(Tipos.Puntero); break;

                case 34:
                case 16: setClasificacion(Tipos.OperadorFactor); break;
                case 17: setClasificacion(Tipos.IncrementoFactor); break;

                case 18:
                case 20:
                case 29:
                case 32:
                case 33: setClasificacion(Tipos.Caracter); break;

                case 19:
                case 21: setClasificacion(Tipos.OperadorLogico); break;

                case 22:
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
                estado = ObtenerEstado(estado, Columna(c));
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
                    throw new Error("léxico, se espera fin de comentario", log, linea);
                }
            }
            if (!finArchivo())
            {
                setContenido(buffer);
                log.WriteLine(getContenido() + " = " + getClasificacion());
            }
        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }

    }
}