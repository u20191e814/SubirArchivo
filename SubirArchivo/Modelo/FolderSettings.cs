using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubirArchivo.Modelo
{
    public  class FolderSettings
    {
        public string Nombre { get; set; }
        public int PK_Camara { get; set; }
        public string folderLocal { get; set; }
        public int total_archivos { get; set; } = 0;
        public int archivos_enviados { get; set; } = 0;
        public int archivos_no_enviados { get; set; } = 0;
    }
}
