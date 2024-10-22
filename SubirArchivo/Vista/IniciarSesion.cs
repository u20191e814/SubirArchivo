using DevExpress.XtraEditors;
using Newtonsoft.Json;
using SubirArchivo.Controlador;
using SubirArchivo.Entidades;
using SubirArchivo.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace SubirArchivo
{
    public partial class IniciarSesion : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public IniciarSesion()
        {
            InitializeComponent();
        }
        private EventViewer evento {  get; set; }
        public OutputUsers user { get;set; }
        public string TokenBearer { get; set; }
        private  void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            Iniciar_Sesion();
            
           

        }

        private void Iniciar_Sesion()
        {
            try
            {
                if (string.IsNullOrEmpty(txtusuario.Text))
                {
                    XtraMessageBox.Show("Debe ingresar el usuario", "Correo");
                    return;
                }
                if (string.IsNullOrEmpty(txtContrasenia.Text))
                {
                    XtraMessageBox.Show("Debe ingresar la contraseña", "Correo");
                    return;
                }
                string encryptar = AE.Encrypt(txtContrasenia.Text);
                dynamic expando = new ExpandoObject();
                expando.Correo = txtusuario.Text;
                expando.Clave = encryptar;



                string json = JsonConvert.SerializeObject(expando);
                HttpContent queryString = new StringContent(json, Encoding.UTF8, "application/json");
                string url = Settings.Default.Api_url + "/api/user/login";
                HttpClient client = new HttpClient();
                var tarea1 = client.PostAsync(url, queryString);
                tarea1.Wait();

                var tarea2 = tarea1.Result.Content.ReadAsStringAsync();
                tarea2.Wait();

                client.Dispose();

                StructureOutputUserLogin user = JsonConvert.DeserializeObject<StructureOutputUserLogin>(tarea2.Result);
                if (user != null)
                {
                    if (user.Data != null)
                    {
                        if (user.Data.Habilitado)
                        {
                            this.TokenBearer = user.TokenBearer;
                            this.user = user.Data;
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                    if (user.Status == "400")
                    {
                        XtraMessageBox.Show(user.Status + ": " + user.StatusMessage);
                        if (!string.IsNullOrEmpty(user.InternalMessage))
                        {
                            evento.WriteErrorLog(user.InternalMessage);
                        }
                    }


                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Iniciar sesión");
            }
        }

        private void IniciarSesion_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                Iniciar_Sesion();
            }
        }

        private void IniciarSesion_Load(object sender, EventArgs e)
        {
            evento= new EventViewer();
        }

        private void txtContrasenia_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                Iniciar_Sesion();
            }
        }
    }
}
