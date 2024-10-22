using DevExpress.XtraEditors;
using Newtonsoft.Json;
using SubirArchivo.Controlador;
using SubirArchivo.Modelo;
using SubirArchivo.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubirArchivo.Vista
{
    public partial class Nuevo : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private string TokenBearer {  get; set; }
        public FolderSettings folder {  get; set; } 
        private EventViewer evento { get; set; }
        public Nuevo(string TokenBearer)
        {
            this.TokenBearer = TokenBearer;
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog()==DialogResult.OK)
            {
                memoEdit1.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void Nuevo_Load(object sender, EventArgs e)
        {
            evento = new EventViewer();
            
            string url = Settings.Default.Api_url + "/api/camera/getCameras";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TokenBearer);
            var tarea1 = client.GetAsync(url);
            tarea1.Wait();

            var tarea2 = tarea1.Result.Content.ReadAsStringAsync();
            tarea2.Wait();

            StructureOutputCameras structu = JsonConvert.DeserializeObject<StructureOutputCameras>(tarea2.Result);
            if (structu!= null )
            {
                if (structu.Status=="OK")
                {
                    lookUpEdit1.Properties.DataSource = structu.Data;
                    lookUpEdit1.Properties.DisplayMember = "Nombre";
                }
                else
                {
                    if (!string.IsNullOrEmpty(structu.InternalMessage))
                    {
                        evento.WriteErrorLog(structu.InternalMessage);
                    }
                  
                    XtraMessageBox.Show(structu.StatusCode + ": "+ structu.StatusMessage, "Cámaras");
                }
            }
            else
            {
                XtraMessageBox.Show("Error en el servicio", "Cámara");
            }

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
               
                if (lookUpEdit1.EditValue == null)
                {
                    XtraMessageBox.Show("Debe elegir una camara ", "Guardar");
                    return;
                }
                string pathfolder = memoEdit1.Text;
                if (string.IsNullOrEmpty(pathfolder))
                {
                    XtraMessageBox.Show("Debe elegir una carpeta valida ", "Guardar");
                    return;
                }
                if (!Directory.Exists(pathfolder))
                {
                    XtraMessageBox.Show("Debe elegir una carpeta valida ", "Guardar");
                    return;
                }
                folder = new FolderSettings();
                OutputCamera cam = lookUpEdit1.EditValue as OutputCamera;
                folder.folderLocal = pathfolder;
                folder.PK_Camara= cam.Pk_camera;
                folder.Nombre = cam.Nombre;
                this.Close();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Guardar");
            }
        }
    }
}
