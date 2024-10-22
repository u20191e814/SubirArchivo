using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubirArchivo.Entidades
{
    public class StructureOutputUserLogin
    {
        public string Status { get; set; } = "OK";
        public string StatusCode { get; set; } = "0";
        public OutputUsers Data { get; set; }
        public string StatusMessage { get; set; } = "OK";
        public string InternalMessage { get; set; } = "";
        public string TokenBearer { get; set; } = "";
    }
    public class OutputUsers
    {
        public int Pk_user { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Cod_trabajador { get; set; }

        public string Preg_seguridad { get; set; }

        public string Resp_seguridad { get; set; }
        public string Fecha_creado { get; set; }
        public string UltimaConexion { get; set; }
        public bool Habilitado { get; set; }
        public bool Administrador { get; set; }
    }
}
