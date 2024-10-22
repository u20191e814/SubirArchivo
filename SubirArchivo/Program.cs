using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubirArchivo
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool instanceCountOne = false;            
            using (Mutex mtex = new Mutex(true, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, out instanceCountOne))
            {
                if (instanceCountOne)
                {
                    try
                    {                        
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        var login = new IniciarSesion();
                        if (login.ShowDialog() == DialogResult.OK)
                        {
                            Application.Run(new Subir(login.user,login. TokenBearer));
                        }

                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(ex.Message, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    mtex.ReleaseMutex();
                }
                else
                {
                    XtraMessageBox.Show("La aplicación ya esta en ejecuciòn", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


           
           
        }
    }
}
