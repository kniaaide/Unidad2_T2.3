using System;
using System.Windows.Forms;

namespace Unidad2
{
    /// <summary>Punto de entrada de la aplicación de la Tarea 2.3.</summary>
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
