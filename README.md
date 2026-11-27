# Unidad 2 · Tarea 2.3 — Torres de Hanoi: recursión e iteración

Proyecto académico en C# (.NET 8, Windows Forms) que resuelve las **Torres de Hanoi** de dos formas, recursiva e iterativa con una pila explícita, y permite simular la solución paso a paso. Incluye también mediciones de tiempo, llamadas, profundidad y memoria asignada.

**Autora:** Kenia Aide Martínez Vallesteros · **Matrícula:** I25050370

---

## Tabla de contenido

- [Objetivo](#objetivo)
- [Estructura del repositorio](#estructura-del-repositorio)
- [El problema](#el-problema)
- [Solución recursiva](#solución-recursiva)
- [Solución iterativa](#solución-iterativa)
- [Funcionamiento de la aplicación](#funcionamiento-de-la-aplicación)
- [Requisitos](#requisitos)
- [Cómo ejecutar el proyecto](#cómo-ejecutar-el-proyecto)

---

## Objetivo

Comprender cómo un problema recursivo puede resolverse sin recursión, reemplazando la pila de llamadas del sistema por una pila propia, y comparar ambas versiones en tiempo, número de llamadas y memoria.

## Estructura del repositorio

```
Unidad2_T2.3/
├── Ejercicios/
│   └── TorresHanoi.cs       # Versiones recursiva e iterativa, y resultado de la resolución
├── MainForm.cs              # Simulación gráfica y medición de rendimiento
├── Program.cs               # Punto de entrada de la aplicación
├── Unidad2_T2.3.csproj      # Proyecto Windows Forms (net8.0-windows)
├── Unidad2_T2.3.sln         # Solución de Visual Studio
└── README.md
```

## El problema

Hay tres torres, A, B y C, y **n discos** de distinto tamaño apilados en A de mayor a menor. El objetivo es pasar todos los discos a C, con dos reglas:

1. Solo se puede mover un disco a la vez.
2. Nunca se puede colocar un disco sobre otro más pequeño.

El número mínimo de movimientos es **2ⁿ − 1**. Con 3 discos son 7 movimientos, y con 15 discos son 32 767.

## Solución recursiva

El método `TorresHanoi.Recursivo(n)` sigue una idea sencilla:

1. Mover n − 1 discos de **A a B**, usando C como auxiliar.
2. Mover el disco n de **A a C**.
3. Mover n − 1 discos de **B a C**, usando A como auxiliar.

El caso base es `n == 0`, que no hace nada. Cada llamada tiene un costo constante fuera de sus dos llamadas recursivas, y el número total de llamadas es 2ⁿ⁺¹ − 1.

La profundidad de la recursión es n, y la memoria de la pila de llamadas del sistema crece en proporción a ella.

## Solución iterativa

El método `TorresHanoi.Iterativo(n)` produce los mismos movimientos sin recursión. En su lugar:

- Usa un `Stack<Marco>` que hace el papel de la pila de llamadas. Cada `Marco` guarda el disco, las torres de origen, auxiliar y destino, la profundidad y un **estado** (0, 1 o 2) que indica qué paso debe ejecutarse a continuación.
- En cada iteración del ciclo, se mira el marco del tope y se ejecuta el siguiente paso según su estado:
  - **Estado 0:** si no hay discos, el marco termina; si los hay, se avanza al estado 1 y se empuja el subproblema de n − 1 discos hacia la auxiliar.
  - **Estado 1:** se registra el movimiento del disco n y se avanza al estado 2, empujando el subproblema de la auxiliar al destino.
  - **Estado 2:** el marco ya está completo y se retira de la pila, lo que equivale al retorno de la función.

Esta forma es equivalente a la recursiva y produce la misma secuencia de movimientos. Su diferencia es que la pila de llamadas se reemplaza por un objeto `Stack` en el *heap*, lo que hace visible el mecanismo que la recursión usa de forma implícita.

## Funcionamiento de la aplicación

### Simulación paso a paso

| Control | Acción |
|---|---|
| **Número de discos** (1 a 15) y **Resolver y simular** | Calcula la solución recursiva y reinicia la simulación. |
| **Barra de desplazamiento** | Avanza o retrocede por los movimientos. |
| **Panel de torres** | Dibuja las torres A, B y C con los discos en su posición correspondiente al paso actual. |
| **Lista de movimientos** | Muestra cada movimiento, por ejemplo `Disco 1: A -> C`. Si hay más de 2 000, el listado se omite y se indica el total. |

La etiqueta "Paso X de Y" indica el movimiento actual y el disco que se mueve.

### Medición de rendimiento

El botón **Medir rendimiento (N = 3, 5, 10, 15)** ejecuta ambas versiones para cuatro valores de n y muestra en una tabla:

| Columna | Qué mide |
|---|---|
| Movimientos | Número de movimientos realizados. |
| Llamadas recursivo / iterativo | Número de marcos creados en cada versión. |
| Profundidad | Profundidad máxima de la pila de llamadas o de la pila explícita. |
| Tiempo rec. / iter. (ms) | Tiempo promedio de 20 ejecuciones, tras un calentamiento previo. |
| Bytes rec. / iter. | Bytes asignados en el *heap* durante una ejecución. |

## Requisitos

- **Visual Studio** (2022 o posterior) con la carga de trabajo *Desarrollo de escritorio con .NET*.
- **.NET 8 SDK** con soporte para Windows (`net8.0-windows`).
- Sistema operativo **Windows**, requerido por Windows Forms.

## Cómo ejecutar el proyecto

1. Clona el repositorio:
   ```bash
   git clone https://github.com/kniaaide/Unidad2_T2.3.git
   ```
2. Abre `Unidad2_T2.3.sln` en Visual Studio.
3. Presiona **F5** (o *Depurar › Iniciar depuración*) para compilar y ejecutar.
4. Elige el número de discos, pulsa **Resolver y simular** y usa la barra para recorrer los movimientos.

También puede compilarse desde la terminal:

```bash
dotnet build Unidad2_T2.3.csproj
dotnet run --project Unidad2_T2.3.csproj
```
