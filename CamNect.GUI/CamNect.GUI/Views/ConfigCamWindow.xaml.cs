using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CamNect.Camera;
using ManagedUPnP;


namespace CamNect.GUI.Views
{
    /// <summary>
    /// Logique d'interaction pour ConfigCamWindow.xaml
    /// </summary>
    
   
    public partial class ConfigCamWindow : Window
    {
        private static System.Windows.Controls.DataGrid dgCam;  
        private static List<CamConfig> ligne = new List<CamConfig>();



        public static List<CamConfig> Ligne
        {
            get
            {
                return ligne;
            }
        }
        public static DataGrid getDg
        {
            get
            {
                return getDg;
            }
        }
       private void CamConfigCollection()
       {
           String json=null;
       
           if (!File.Exists("config.json"))
           {
               StreamWriter jsonfile = new StreamWriter("config.json", false);
               jsonfile.WriteLine("[]");
               jsonfile.Close();
           }
           else
           {
               json = File.ReadAllText("config.json");
               ligne = JsonConvert.DeserializeObject<List<CamConfig>>(json);
           }
       }

        

        public ConfigCamWindow()
        {
            InitializeComponent();

            dgCam = dgCamConfig;
            CamConfigCollection();

          
            foreach (CamConfig l in ligne)
            {
                l.Show = false;
            }

            dgCam.ItemsSource = ligne;
        }
        

        //Deplacer la fenetre car sans bordure :
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            StreamWriter jsonfile = new StreamWriter("config.json", false);

           

            string json = JsonConvert.SerializeObject(ligne, Formatting.Indented);
            jsonfile.WriteLine(json);
            jsonfile.Close();

            this.Close();
        }

        public static void AddCam(CamConfig cfg)
        {
            ligne.Add(cfg);
            dgCam.Items.Refresh();  
        }

        public static void CamRefresh()
        {
            dgCam.Items.Refresh();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //AddCam(true, "cameraX","122554","hidden", "url", true, true, "21255",5,true);
        } 
    }


}
