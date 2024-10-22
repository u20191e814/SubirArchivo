using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubirArchivo.Modelo
{
    public class StructureOutputFtp
    {
        public string Status { get; set; } = "OK";
        public string StatusCode { get; set; } = "0";
        public OutputFtp Data { get; set; }
        public string StatusMessage { get; set; } = "OK";
        public string InternalMessage { get; set; } = "";
    }
    public class OutputFtp
    {
        public string servidorFtp { get; set; }
        public string usuarioFtp { get; set; }
        public string passwordFtp { get; set; }
        public string directoryFtp { get; set; }
    }
}
