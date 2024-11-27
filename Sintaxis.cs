using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sintaxis_1
{
    public class Sintaxis : Lexico
    {
        public Sintaxis() : base()
        {
            if (!finArchivo()) // Verifica que el archivo esté listo
            {
                nextToken();
            }
            else
            {
                throw new Exception("El archivo de entrada está vacío o no se pudo inicializar.");
            }
        }

        public Sintaxis(string name) : base(name)
        {
            if (!finArchivo())
            {
                nextToken();
            }
            else
            {
                throw new Exception("El archivo de entrada está vacío o no se pudo inicializar.");
            }
        }

        public void match(string contenido)
        {
            if (contenido == getContenido())
            {
                nextToken();
            }
            else
            {
                throw new Error($"Sintaxis: se esperaba '{contenido}', pero se encontró '{getContenido()}' en la línea {linea - 1}");
            }
        }

        public void match(Tipos clasificacion)
        {
            if (clasificacion == getClasificacion())
            {
                nextToken();
            }
            else
            {
                throw new Error($"Sintaxis: se esperaba '{clasificacion}', pero se encontró '{getClasificacion()}' en la línea {linea - 1}");
            }
        }

    }
}