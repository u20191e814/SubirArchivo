using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubirArchivo.Modelo
{
    public class StructureOutputCameras
    {
        public string Status { get; set; } = "OK";
        public string StatusCode { get; set; } = "0";
        public List<OutputCamera> Data { get; set; }
        public string StatusMessage { get; set; } = "OK";
        public string InternalMessage { get; set; } = "";
    }
    public class OutputCamera
    {
        public int Pk_camera { get; set; }
        public string Nombre { get; set; }
    }
}
