using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sintaxis_1
{
    public class Program : Token
    {
        static void Main()
        {
            try
            {
                using Lenguaje l = new("prueba.cpp");
                //token.GetAllTokens();
                l.Programa();
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}