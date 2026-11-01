using System;
using System.Collections.Generic;

namespace Unidad2.Ejercicios
{
    /// <summary>Resultado de una resolución de las Torres de Hanoi.</summary>
    public sealed class ResultadoHanoi
    {
        /// <summary>Número de movimientos de disco realizados.</summary>
        public long Movimientos { get; set; }

        /// <summary>Número total de llamadas (marcos) creados.</summary>
        public long Llamadas { get; set; }

        /// <summary>Profundidad máxima alcanzada por la pila de llamadas (o de la pila explícita).</summary>
        public int ProfundidadMaxima { get; set; }
    }

    /// <summary>
    /// Torres de Hanoi con tres pilas: A (origen), B (auxiliar) y C (destino).
    /// Reglas: mover un disco a la vez y nunca colocar un disco sobre otro menor.
    /// Solución recursiva: mover n−1 discos a la auxiliar, mover el disco n
    /// al destino y mover los n−1 discos desde la auxiliar al destino.
    /// </summary>
    public static class TorresHanoi
    {
        /// <summary>Delegado que recibe cada movimiento: disco, origen y destino.</summary>
        public delegate void RegistroMovimiento(int disco, char origen, char destino);

        /// <summary>Versión recursiva. Resuelve n discos de A a C usando B como auxiliar.</summary>
        public static ResultadoHanoi Recursivo(int n, RegistroMovimiento? registrar = null)
        {
            Validar(n);
            var resultado = new ResultadoHanoi();
            MoverRecursivo(n, 'A', 'B', 'C', 0, resultado, registrar);
            return resultado;
        }

        /// <summary>
        /// Versión iterativa. Simula la pila de llamadas con una pila explícita
        /// (Stack&lt;Marco&gt;). Cada marco tiene un estado que indica qué paso ejecutar.
        /// </summary>
        public static ResultadoHanoi Iterativo(int n, RegistroMovimiento? registrar = null)
        {
            Validar(n);
            var resultado = new ResultadoHanoi();
            var pila = new Stack<Marco>();

            Empujar(pila, n, 'A', 'B', 'C', 0, resultado);

            while (pila.Count > 0)
            {
                Marco m = pila.Peek();

                if (m.Estado == 0)
                {
                    // Caso base: sin discos, el marco termina.
                    if (m.Disco == 0)
                    {
                        pila.Pop();
                        continue;
                    }
                    // Paso 1: resolver n−1 discos de origen a auxiliar.
                    m.Estado = 1;
                    Empujar(pila, m.Disco - 1, m.Origen, m.Destino, m.Auxiliar, m.Profundidad + 1, resultado);
                }
                else if (m.Estado == 1)
                {
                    // Paso 2: mover el disco más grande del marco.
                    resultado.Movimientos++;
                    registrar?.Invoke(m.Disco, m.Origen, m.Destino);
                    // Paso 3: resolver n−1 discos de auxiliar a destino.
                    m.Estado = 2;
                    Empujar(pila, m.Disco - 1, m.Auxiliar, m.Origen, m.Destino, m.Profundidad + 1, resultado);
                }
                else
                {
                    // Marco completado: se retira de la pila (equivale al retorno).
                    pila.Pop();
                }
            }
            return resultado;
        }

        private static void MoverRecursivo(int n, char origen, char auxiliar, char destino,
                                           int profundidad, ResultadoHanoi r, RegistroMovimiento? registrar)
        {
            r.Llamadas++;
            if (profundidad > r.ProfundidadMaxima)
            {
                r.ProfundidadMaxima = profundidad;
            }

            // Caso base: sin discos que mover.
            if (n == 0)
            {
                return;
            }

            MoverRecursivo(n - 1, origen, destino, auxiliar, profundidad + 1, r, registrar);
            r.Movimientos++;
            registrar?.Invoke(n, origen, destino);
            MoverRecursivo(n - 1, auxiliar, origen, destino, profundidad + 1, r, registrar);
        }

        private static void Empujar(Stack<Marco> pila, int disco, char origen, char auxiliar,
                                    char destino, int profundidad, ResultadoHanoi r)
        {
            r.Llamadas++;
            if (profundidad > r.ProfundidadMaxima)
            {
                r.ProfundidadMaxima = profundidad;
            }
            pila.Push(new Marco
            {
                Disco = disco,
                Origen = origen,
                Auxiliar = auxiliar,
                Destino = destino,
                Profundidad = profundidad,
                Estado = 0
            });
        }

        private static void Validar(int n)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n), "El número de discos no puede ser negativo.");
            }
        }

        /// <summary>Marco de activación explícito usado por la versión iterativa.</summary>
        private sealed class Marco
        {
            public int Disco;
            public char Origen;
            public char Auxiliar;
            public char Destino;
            public int Profundidad;
            public int Estado;
        }
    }
}
