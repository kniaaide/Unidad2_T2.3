using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Unidad2.Ejercicios;

namespace Unidad2
{
    /// <summary>
    /// Formulario de la Tarea 2.3. Simula gráficamente las Torres de Hanoi paso
    /// a paso y mide tiempo, llamadas, profundidad y bytes asignados según n.
    /// </summary>
    public class MainForm : Form
    {
        // Movimientos calculados por la versión recursiva: (disco, origen, destino).
        private readonly List<(int Disco, char Origen, char Destino)> _movimientos = new();

        private readonly Label lblDiscos = new Label { Text = "Número de discos (1 a 15):", Location = new Point(20, 22), AutoSize = true };
        private readonly NumericUpDown nudN = new NumericUpDown
        {
            Location = new Point(190, 18),
            Size = new Size(70, 25),
            Minimum = 1,
            Maximum = 15,
            Value = 3
        };
        private readonly Button btnResolver = new Button { Text = "Resolver y simular", Location = new Point(280, 16), Size = new Size(160, 30) };
        private readonly TrackBar trkPaso = new TrackBar
        {
            Location = new Point(20, 58),
            Size = new Size(740, 45),
            Minimum = 0,
            Maximum = 0,
            TickFrequency = 1
        };
        private readonly Label lblPaso = new Label { Location = new Point(20, 102), Size = new Size(740, 20), Text = "Paso 0 de 0" };
        private readonly Panel pnlTorres = new Panel
        {
            Location = new Point(20, 125),
            Size = new Size(740, 220),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White
        };
        private readonly ListBox lstMovimientos = new ListBox { Location = new Point(20, 360), Size = new Size(300, 380) };
        private readonly Button btnMedir = new Button
        {
            Text = "Medir rendimiento (N = 3, 5, 10, 15)",
            Location = new Point(340, 355),
            Size = new Size(420, 30)
        };
        private readonly DataGridView dgv = new DataGridView
        {
            Location = new Point(340, 395),
            Size = new Size(420, 345),
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        };

        public MainForm()
        {
            Text = "Unidad 2 - Tarea 2.3: Simulación de Torres de Hanoi";
            ClientSize = new Size(790, 760);
            StartPosition = FormStartPosition.CenterScreen;

            ConfigurarTabla();
            Controls.AddRange(new Control[]
            {
                lblDiscos, nudN, btnResolver, trkPaso, lblPaso, pnlTorres,
                lstMovimientos, btnMedir, dgv
            });

            pnlTorres.Paint += DibujarTorres;
            btnResolver.Click += (s, e) => Resolver();
            trkPaso.ValueChanged += (s, e) => ActualizarPaso();
            btnMedir.Click += (s, e) => Medir();

            Resolver();
        }

        private void ConfigurarTabla()
        {
            dgv.Columns.Add("colN", "N");
            dgv.Columns.Add("colMov", "Movimientos");
            dgv.Columns.Add("colLlamRec", "Llamadas recursivo");
            dgv.Columns.Add("colLlamIt", "Llamadas iterativo");
            dgv.Columns.Add("colProf", "Profundidad");
            dgv.Columns.Add("colTRec", "Tiempo rec. (ms)");
            dgv.Columns.Add("colTIt", "Tiempo iter. (ms)");
            dgv.Columns.Add("colBRec", "Bytes rec.");
            dgv.Columns.Add("colBIt", "Bytes iter.");
        }

        // ---------- Simulación paso a paso ----------

        private void Resolver()
        {
            int n = (int)nudN.Value;
            _movimientos.Clear();
            TorresHanoi.Recursivo(n, (disco, origen, destino) => _movimientos.Add((disco, origen, destino)));

            lstMovimientos.BeginUpdate();
            lstMovimientos.Items.Clear();
            if (_movimientos.Count <= 2000)
            {
                for (int i = 0; i < _movimientos.Count; i++)
                {
                    var m = _movimientos[i];
                    lstMovimientos.Items.Add($"{i + 1,5}. Disco {m.Disco}: {m.Origen} -> {m.Destino}");
                }
            }
            else
            {
                lstMovimientos.Items.Add($"Listado omitido: {_movimientos.Count} movimientos.");
            }
            lstMovimientos.EndUpdate();

            trkPaso.Maximum = _movimientos.Count;
            trkPaso.Value = 0;
            ActualizarPaso();
        }

        private void ActualizarPaso()
        {
            int paso = trkPaso.Value;
            string detalle = "";
            if (paso > 0 && paso <= _movimientos.Count)
            {
                var m = _movimientos[paso - 1];
                detalle = $": disco {m.Disco} de {m.Origen} a {m.Destino}";
            }
            lblPaso.Text = $"Paso {paso} de {_movimientos.Count}{detalle}";
            pnlTorres.Invalidate();
        }

        /// <summary>Reconstruye el estado de las torres después de los primeros "paso" movimientos.</summary>
        private Dictionary<char, List<int>> EstadoEn(int paso)
        {
            int n = (int)nudN.Value;
            var estado = new Dictionary<char, List<int>>
            {
                ['A'] = new List<int>(),
                ['B'] = new List<int>(),
                ['C'] = new List<int>()
            };

            // Estado inicial: discos de mayor a menor en A; la lista va de base a cima.
            for (int d = n; d >= 1; d--)
            {
                estado['A'].Add(d);
            }

            for (int i = 0; i < paso && i < _movimientos.Count; i++)
            {
                var m = _movimientos[i];
                List<int> origen = estado[m.Origen];
                int disco = origen[origen.Count - 1];
                origen.RemoveAt(origen.Count - 1);
                estado[m.Destino].Add(disco);
            }
            return estado;
        }

        private void DibujarTorres(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int n = (int)nudN.Value;
            var estado = EstadoEn(trkPaso.Value);

            int baseY = pnlTorres.Height - 25;
            int alto = Math.Max(4, Math.Min(18, (baseY - 40) / Math.Max(n, 1)));
            char[] nombres = { 'A', 'B', 'C' };

            for (int i = 0; i < nombres.Length; i++)
            {
                int x = 130 + i * 240;

                // Poste y base de la torre.
                g.FillRectangle(Brushes.DimGray, x - 4, 40, 8, baseY - 40);
                g.FillRectangle(Brushes.Gray, x - 150, baseY, 300, 8);
                g.DrawString(nombres[i].ToString(), Font, Brushes.Black, x - 6, 12);

                // Discos de la torre, desde la base hacia la cima.
                List<int> discos = estado[nombres[i]];
                for (int j = 0; j < discos.Count; j++)
                {
                    int disco = discos[j];
                    int ancho = 20 + disco * 12;
                    int y = baseY - (j + 1) * (alto + 2);

                    using var pincel = new SolidBrush(ColorDisco(disco));
                    g.FillRectangle(pincel, x - ancho / 2, y, ancho, alto);
                    g.DrawRectangle(Pens.Black, x - ancho / 2, y, ancho, alto);
                }
            }
        }

        private static Color ColorDisco(int disco)
        {
            return Color.FromArgb(255, (disco * 67) % 200 + 30, (disco * 113) % 200 + 30, (disco * 29) % 200 + 60);
        }

        // ---------- Medición de rendimiento ----------

        private void Medir()
        {
            int[] tamanios = { 3, 5, 10, 15 };
            const int repeticiones = 20;

            Cursor = Cursors.WaitCursor;
            btnMedir.Enabled = false;
            try
            {
                dgv.Rows.Clear();

                foreach (int n in tamanios)
                {
                    // Calentamiento para que la compilación JIT no afecte la medición.
                    TorresHanoi.Recursivo(n);
                    TorresHanoi.Iterativo(n);

                    ResultadoHanoi rec = TorresHanoi.Recursivo(n);
                    ResultadoHanoi it = TorresHanoi.Iterativo(n);

                    double tiempoRec = TiempoPromedioMs(() => TorresHanoi.Recursivo(n), repeticiones);
                    double tiempoIt = TiempoPromedioMs(() => TorresHanoi.Iterativo(n), repeticiones);

                    long bytesRec = BytesAsignados(() => TorresHanoi.Recursivo(n));
                    long bytesIt = BytesAsignados(() => TorresHanoi.Iterativo(n));

                    dgv.Rows.Add(
                        n,
                        rec.Movimientos,
                        rec.Llamadas,
                        it.Llamadas,
                        rec.ProfundidadMaxima,
                        tiempoRec.ToString("F4"),
                        tiempoIt.ToString("F4"),
                        bytesRec,
                        bytesIt);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
                btnMedir.Enabled = true;
            }
        }

        private static double TiempoPromedioMs(Action accion, int repeticiones)
        {
            var cronometro = Stopwatch.StartNew();
            for (int i = 0; i < repeticiones; i++)
            {
                accion();
            }
            cronometro.Stop();
            return cronometro.Elapsed.TotalMilliseconds / repeticiones;
        }

        /// <summary>
        /// Bytes asignados en el Heap por el hilo actual durante la acción.
        /// La versión recursiva usa la pila de llamadas; la iterativa crea un objeto por marco.
        /// </summary>
        private static long BytesAsignados(Action accion)
        {
            long antes = GC.GetAllocatedBytesForCurrentThread();
            accion();
            return GC.GetAllocatedBytesForCurrentThread() - antes;
        }
    }
}
