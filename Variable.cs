using System;

namespace Sintaxix_1
{
    public class Variable
    {
        public enum TipoDato
        {
            Char, Int, Float
        }

        private TipoDato tipo; // Campo privado para almacenar el tipo
        private string nombre; // Campo privado para almacenar el nombre
        private float valor;   // Campo privado para almacenar el valor

        // Constructor para inicializar la variable
        public Variable(TipoDato tipo, string nombre, float valor = 0)
        {
            this.tipo = tipo;
            this.nombre = nombre;
            this.valor = valor;
        }

        // Propiedad pública para acceder al tipo
        public TipoDato GetTipoDato()
        {
            return tipo;
        }

        // Propiedad pública para acceder y modificar el valor
        public float GetValor()
        {
            return valor;
        }

        public void SetValor(float valor)
        {
            this.valor = valor;
        }

        // Propiedad pública para acceder al nombre
        public string GetNombre()
        {
            return nombre;
        }
    }
}
