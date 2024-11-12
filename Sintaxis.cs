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
            nextToken();
        }

        public Sintaxis(string name) : base(name)
        {
            nextToken();
        }
        public void match(string contenido)
        {
            if (contenido == getContenido())
            {
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un " + contenido);
            }
        }

        public void match(Tipos clasification)
        {
            if (clasification == getClasificacion())
            {
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un " + clasification);
            }
        }
    }
}