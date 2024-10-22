using DevExpress.Xpo.Logger;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraPrinting;
using Newtonsoft.Json;
using SubirArchivo.Controlador;
using SubirArchivo.Entidades;
using SubirArchivo.Modelo;
using SubirArchivo.Properties;
using SubirArchivo.Vista;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubirArchivo
{
    public partial class Subir : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private string TokenBearer {  get; set; }   
        private EventViewer evento {  get; set; }

        public Subir(OutputUsers user, string TokenBearer)
        {
            User = user;
            this.TokenBearer = TokenBearer;
            InitializeComponent();
        }

        public OutputUsers User { get; }
        private List<FolderSettings> listaFolders {  get; set; }
        private string ArchivoJsonLocal = "FolderSettings.json";
        private void Subir_Load(object sender, EventArgs e)
        {
            evento = new EventViewer();
            dateEdit1.DateTime = DateTime.Now;
            listaFolders= new List<FolderSettings>();
            CargarLista();
            gridControl1.Focus();
        }

        private void trackBarControl1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                lblporcentaje.Text = trackBarControl1.Value.ToString() + "%";
            }
            catch (Exception ex)
            {

            }
        }

        private void btnNuevaCarpeta_Click(object sender, EventArgs e)
        {
            Nuevo nuevo = new Nuevo(TokenBearer);
            nuevo.ShowDialog();
            if (nuevo.folder!=null)
            {
                if (listaFolders==null)
                {
                    listaFolders = new List<FolderSettings>();
                }
                listaFolders.Add(nuevo.folder);
                string nuevoJson = JsonConvert.SerializeObject(listaFolders, Formatting.Indented);
                string cifrarJson = AE.Encrypt(nuevoJson);
                if (File.Exists(ArchivoJsonLocal))
                {
                    File.Delete(ArchivoJsonLocal);
                }
                File.WriteAllText(ArchivoJsonLocal, cifrarJson);
                CargarLista();
            }
        }

        private void CargarLista()
        {
            try
            {
                listaFolders= new List<FolderSettings>();
                if (File.Exists(ArchivoJsonLocal))
                {
                    string lect = File.ReadAllText(ArchivoJsonLocal);
                    string deseria= AE.Decrypt(lect);
                    listaFolders= JsonConvert.DeserializeObject<List<FolderSettings>>(deseria);
                }
                gridControl1.DataSource = listaFolders;
                if (listaFolders== null || listaFolders.Count==0)
                {
                    btnIniciarCarga.Enabled = false;
                }
                if (listaFolders!=null && listaFolders.Count>0)
                {
                    btnIniciarCarga.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Lista");
            }
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var main = gridControl1.MainView as ColumnView;
                FolderSettings fol = main.GetFocusedRow () as FolderSettings;
                if (fol!= null)
                {
                    if (XtraMessageBox.Show("¿Seguro que desea eliminar la carpeta? ", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
                    {
                        listaFolders.Remove(fol);
                        string nuevoJson = JsonConvert.SerializeObject(listaFolders, Formatting.Indented);
                        string cifrarJson = AE.Encrypt(nuevoJson);
                        if (File.Exists(ArchivoJsonLocal))
                        {
                            File.Delete(ArchivoJsonLocal);
                        }
                        File.WriteAllText(ArchivoJsonLocal, cifrarJson);
                        CargarLista();
                    }
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Eliminar");
            }
        }

        private async void btnIniciarCarga_Click(object sender, EventArgs e)
        {
            try
            {
               
                string url = Settings.Default.Api_url + "/api/ftp/getFtp?id="+Settings.Default.Id_Ftp;
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TokenBearer);
                var tarea1 = await client.GetAsync(url);
               
                var tarea2 = await tarea1.Content.ReadAsStringAsync();

                OutputFtp ftp = null;
                StructureOutputFtp structu = JsonConvert.DeserializeObject<StructureOutputFtp>(tarea2);
                if (structu != null)
                {
                    if (structu.Status == "OK")
                    {
                       ftp = structu.Data;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(structu.InternalMessage))
                        {
                            evento.WriteErrorLog(structu.InternalMessage);
                        }
                        CerrarSplat();
                        XtraMessageBox.Show(structu.StatusCode + ": " + structu.StatusMessage, "Cámaras");
                        return;
                    }
                }
                else
                {
                    CerrarSplat();
                    XtraMessageBox.Show("Error en el servicio", "Cámara");
                    return;
                }


                List<FolderSettings> lista = gridControl1.DataSource as List<FolderSettings>;
                foreach (FolderSettings fol in lista) 
                {
                    if (!Directory.Exists(fol.folderLocal))
                    {
                        continue;
                    }
                    DirectoryInfo dir = new DirectoryInfo(fol.folderLocal);
                    var archivos = dir.GetFiles("*.mp4");
                    fol.total_archivos = archivos.Length;
                    fol.archivos_enviados = 0;
                    fol.archivos_no_enviados = 0;
                    
                }
                

                gridControl2.DataSource = lista;
                if (lista != null)
                {
                    var lista2 = lista.Where(v => v.total_archivos > 0).ToList();
                    if (lista2 != null && lista2.Count==0)
                    {
                        CerrarSplat();
                        XtraMessageBox.Show("No existen archivos a subir ", "Subir");
                        return;
                    }
                }
                if (XtraMessageBox.Show("Los videos que esten en las carpetas una vez cargado se van a eliminar. Responde Si para continuar", "Iniciar carga", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    splashScreenManager1.ShowWaitForm();
                    splashScreenManager1.SetWaitFormCaption("Subiendo");
                    splashScreenManager1.SetWaitFormDescription("Cargando archivos ...");
                    int score = trackBarControl1.Value;
                    DateTime fecha = dateEdit1.DateTime.Date;
                    string folder = Path.Combine(fecha.Year.ToString(), fecha.Month.ToString().PadLeft(2, '0'), fecha.Day.ToString().PadLeft(2, '0'));
                    var estado = FTP.CrearCarpetaFtp(ftp.servidorFtp, AE.Decrypt(ftp.usuarioFtp), AE.Decrypt(ftp.passwordFtp), folder);
                    if (estado!= System.Net.FtpStatusCode.PathnameCreated)
                    {
                        XtraMessageBox.Show("No se pudo crear el directorio para subir", "Subir archivo");
                        evento.WriteErrorLog(estado.ToString());
                        CerrarSplat();
                        return;
                    }
                    
                    SemaphoreSlim subprocess = new SemaphoreSlim(Settings.Default.NTaskUpload);
                    List<Task> tareas = new List<Task>();
                    Task tarea = null;
                    foreach (FolderSettings fol in lista)
                    {
                        var tv = fol;
                        //ProcesarCarga(tv, folder, ftp, fecha, score);
                        tarea = Task.Factory.StartNew(() =>
                        {
                            subprocess.Wait();
                            ProcesarCarga(tv, folder, ftp, fecha, score);
                            subprocess.Release();
                        });
                        tareas.Add(tarea);

                    }
                    Task.WaitAll(tareas.ToArray());
                    foreach (var item in tareas)
                    {
                        item.Dispose();
                    }
                    if (gridControl2.InvokeRequired) 
                    {
                        gridControl2.Invoke(new Action(() => 
                        {
                            gridControl2.DataSource = null;
                            gridControl2.DataSource = lista;
                        }));
                    }
                    else
                    {
                        gridControl2.DataSource = null;
                        gridControl2.DataSource = lista;
                    }
                  
                }
                CerrarSplat();
            }
            catch (Exception ex)
            {
                CerrarSplat();
                XtraMessageBox.Show(ex.Message, "Iniciar carpeta");
            }
        }
        public void CerrarSplat()
        {
            if (splashScreenManager1.IsSplashFormVisible)
            { 
                splashScreenManager1.CloseWaitForm();
            }
        }

        private void ProcesarCarga(FolderSettings fol , string folder, OutputFtp ftp, DateTime fecha, int score)
        {
            string nuevofolder = Path.Combine(folder,fol.PK_Camara.ToString() );
            var estado = FTP.CrearCarpetaFtp(ftp.servidorFtp, AE.Decrypt(ftp.usuarioFtp), AE.Decrypt(ftp.passwordFtp), nuevofolder);
            if (estado != System.Net.FtpStatusCode.PathnameCreated)
            {               
                evento.WriteErrorLog(estado.ToString());
                return;
            }
            DirectoryInfo dir = new DirectoryInfo(fol.folderLocal);
            var archivos = dir.GetFiles("*.mp4");
            if (archivos.Length > 0)
            {
               
                foreach (var item in archivos)
                {
                    try
                    {
                        string nuevoarchivo = Path.Combine(nuevofolder, item.Name);
                        var carga= FTP.EnviarArchivoFtp(item.FullName,  nuevoarchivo, ftp.servidorFtp, AE.Decrypt(ftp.usuarioFtp), AE.Decrypt(ftp.passwordFtp));
                        if (carga==FtpStatusCode.ClosingData)
                        {
                            string[] nombre = item.Name.Replace(".mp4", "").Split('_');
                            int hora = int.Parse(nombre[0]);
                            int minuto = int.Parse(nombre[1]);
                            DateTime nuevafecha = fecha.Date.AddHours(hora).AddMinutes(minuto);
                            dynamic input = new ExpandoObject();
                            input.unixDateTime = Utils.ToUnixTime(nuevafecha);
                            input.file = Path.Combine( ftp.directoryFtp, nuevoarchivo);
                            input.fk_camara = fol.PK_Camara;
                            input.fk_user= User.Pk_user;
                            input.score= score;

                            string json = JsonConvert.SerializeObject(input);
                            HttpContent queryString = new StringContent(json, Encoding.UTF8, "application/json");
                            string url = Settings.Default.Api_url + "/api/objectInVideoFolder/create";
                            HttpClient client = new HttpClient();
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TokenBearer);
                            var tarea1 = client.PostAsync(url, queryString);
                            tarea1.Wait();

                            var tarea2 = tarea1.Result.Content.ReadAsStringAsync();
                            tarea2.Wait();

                            client.Dispose();

                            if (tarea2.Result.Contains("400"))
                            {
                                evento.WriteErrorLog(tarea2.Result);
                                fol.archivos_no_enviados = fol.archivos_no_enviados + 1;
                            }
                            else
                            {
                                if (tarea2.Result.Contains("OK"))
                                {
                                    fol.archivos_enviados = fol.archivos_enviados + 1;
                                    File.Delete(item.FullName);
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                 
                }
            }
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var main = gridControl1.MainView as ColumnView;
                var tt = main.GetFocusedRow() as FolderSettings;
                if (tt != null) 
                {
                    Process.Start("explorer.exe", tt.folderLocal);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
