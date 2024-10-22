using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SubirArchivo.Controlador
{
    public  class FTP
    {
        public static FtpStatusCode EnviarArchivoFtp(string localfile,  string remotefile, string servidorFtp, string usuarioFtp, string passwordFtp)
        {
            FtpStatusCode cod = FtpStatusCode.Undefined;
            // Crear una solicitud FTP
            FtpWebRequest solicitud = (FtpWebRequest)WebRequest.Create($"ftp://{servidorFtp}/{remotefile}");
            solicitud.Method = WebRequestMethods.Ftp.UploadFile;
            solicitud.Credentials = new NetworkCredential(usuarioFtp, passwordFtp);

            // Abrir el archivo para leer
            using (FileStream archivo = File.OpenRead(localfile))
            {
                // Obtener el tamaño del archivo
                byte[] buffer = new byte[archivo.Length];
                archivo.Read(buffer, 0, buffer.Length);

                // Enviar el archivo
                using (Stream flujo = solicitud.GetRequestStream())
                {
                    flujo.Write(buffer, 0, buffer.Length);
                }
            }

            // Obtener la respuesta del servidor
            FtpWebResponse respuesta = (FtpWebResponse)solicitud.GetResponse();
            cod = respuesta.StatusCode;
            respuesta.Close();
            return cod;
        }

        public static  FtpStatusCode CrearCarpetaFtp(string servidorFtp, string usuarioFtp, string passwordFtp, string nombreCarpeta)
        {
            FtpStatusCode ftpStatusCode = FtpStatusCode.Undefined;
            try
            {

                // Crear una solicitud FTP
                FtpWebRequest solicitud = (FtpWebRequest)WebRequest.Create($"ftp://{servidorFtp}/{nombreCarpeta}");
                solicitud.Method = WebRequestMethods.Ftp.MakeDirectory;
                solicitud.Credentials = new NetworkCredential(usuarioFtp, passwordFtp);

                // Obtener la respuesta del servidor
                FtpWebResponse respuesta = (FtpWebResponse)solicitud.GetResponse();
                ftpStatusCode = respuesta.StatusCode;
                respuesta.Close();
            }
            catch (WebException ex)
            {
                if (((FtpWebResponse)ex.Response).StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    ftpStatusCode = FtpStatusCode.PathnameCreated;
                    // La carpeta ya existe, no hacer nada
                }
                else
                {
                    ftpStatusCode = ((FtpWebResponse)ex.Response).StatusCode;
                }
            }
            return ftpStatusCode;
        }

    }
}
