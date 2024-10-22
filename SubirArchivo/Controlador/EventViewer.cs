using SubirArchivo.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubirArchivo.Controlador
{
    public  class EventViewer
    {
       

        public void WriteErrorLog(string Message)
        {
            try
            {
                if (Settings.Default.WriteLogInDirectory)
                {
                    StreamWriter sw = null;

                    string LogDirectory = string.Empty;
                    if (string.IsNullOrEmpty(Settings.Default.LogDirectory))
                        LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                    else
                        LogDirectory = Path.Combine(Settings.Default.LogDirectory);

                    if (!Directory.Exists(LogDirectory))
                        Directory.CreateDirectory(LogDirectory);
                    string rutaLog = string.Format("{0}\\{1}Log {2}.txt", LogDirectory, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, DateTime.Now.ToString("dd-MM-yyyy"));
                    bool existenciaArchivo = File.Exists(rutaLog);
                    sw = new StreamWriter(rutaLog, true);
                    if (!existenciaArchivo)
                    {
                        string mensaje = "Log creado con nombre " + Path.GetFileName(rutaLog);
                        sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss K") + ": " + mensaje);
                       

                    }
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss K") + ": " + Message);
                    sw.Flush();
                    sw.Close();

                    DirectoryInfo info = new DirectoryInfo(LogDirectory);
                    List<FileInfo> files = info.GetFiles("*.txt", SearchOption.AllDirectories).OrderBy(p => p.CreationTime).ToList();
                    if (files.Count > Settings.Default.LogDirectoryMaxDays)
                    {
                        foreach (FileInfo item in files)
                        {
                            if (DateTime.Now.Subtract(item.CreationTime).TotalDays > Settings.Default.LogDirectoryMaxDays)
                                item.Delete();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Se necesita permisos para guardar los logs en directorio ");
            }



        }
    }
}
