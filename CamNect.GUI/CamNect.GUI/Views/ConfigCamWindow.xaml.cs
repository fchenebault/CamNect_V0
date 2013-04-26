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
using System.Windows.Navigation;
using System.Windows.Threading;
using CamNect.Camera;
using ManagedUPnP;
using Hardcodet.Wpf.Util;
using System.Collections.ObjectModel;


namespace CamNect.GUI.Views
{
   
    public partial class ConfigCamWindow : Window
    {
        // Variables
        private static System.Windows.Controls.DataGrid dgCam;
        public static ObservableCollection<CamConfig> ligne { get; set; }
        public static int maxFenetre = 1;

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
           ligne = new ObservableCollection<CamConfig>();

           if (!File.Exists("../../Ressources/Config/config.json"))
           {
               StreamWriter jsonfile = new StreamWriter("../../Ressources/Config/config.json", false);
               jsonfile.WriteLine("[]");
               jsonfile.Close();
           }
           else
           {
               json = File.ReadAllText("../../Ressources/Config/config.json");
               ligne = JsonConvert.DeserializeObject<ObservableCollection<CamConfig>>(json);
               /*try
               {
                   maxFenetre = ligne.Max(a => a.Fenetre);
               }
               catch { }*/
           }

       }


        public ConfigCamWindow()
        {
            InitializeComponent();


            dgCam = dgCamConfig;
            CamConfigCollection();

            //Trie les objets en deux categirie, true/false, puis en fonction du numero de fenetre
            //ligne = new ObservableCollection<CamConfig>(ligne.OrderByDescending(a => a.Plugged).ThenBy(a => a.Fenetre));
            
            //Reindex le numero de fenetre
            /*int index;
            foreach (CamConfig l in ligne)
            {
                index = ligne.IndexOf(l);
                l.Fenetre = index + 1;
                l.Plugged = false;
            }*/




            dgCam.ItemsSource = ligne;

        }
        

        // To move the window 
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }


        private void onCancelClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void onOkayClick(object sender, RoutedEventArgs e)
        {
            StreamWriter jsonfile = new StreamWriter("../../Ressources/Config/config.json", false);

            string json = JsonConvert.SerializeObject(ligne, Formatting.Indented);
            jsonfile.WriteLine(json);
            jsonfile.Close();

            this.Close();

        }

        public static void AddCam(CamConfig cfg)
        {
            ligne.Add(cfg);
            System.Console.WriteLine("-- AJOUT --");
            System.Console.WriteLine(cfg.Modele);
            System.Console.WriteLine(cfg.Serie);

            System.Console.WriteLine("-- LISTE --");
            foreach (CamConfig c in ligne)
            {
                System.Console.WriteLine(c.Modele + " - " + c.Serie);
            }

            dgCam.Items.Refresh();


        }

        public static void CamRefresh()
        {
            dgCam.Items.Refresh();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            /*AddCam(new CamConfig(true, "cameraX", "122554", "hidden","URL", true, false, true, "21255", 5, true));
            ligne = new ObservableCollection<CamConfig>(ligne.OrderByDescending(a => a.Plugged).ThenBy(a => a.Fenetre));
            int index;
            foreach (CamConfig l in ligne)
            {
                index = ligne.IndexOf(l);
                l.Fenetre = index + 1;
            }
            //
            dgCam.ItemsSource = ligne;*/
            /*foreach (CameraUtils u in CameraOne.cameraList)
            {
                System.Console.WriteLine(u.Config.Nom);
            }*/

           // AddCam(new CamConfig(true, "AXIS M1054", "root", "hiddeb", "URL", true, false, true, "1236454215241", 5, true));

            
        }

        public static void reindexCam()
        {

            //ligne = new ObservableCollection<CamConfig>(ligne.OrderByDescending(a => a.Show).ThenBy(a => a.Fenetre));
            //Reindex le numero de fenetre
            int index;
            foreach (CamConfig l in ligne)
            {
                index = ligne.IndexOf(l);
                l.Fenetre = index + 1;
            }
            //
            dgCam.Items.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class ResolutionList : List<string>
    {
        public ResolutionList()
        {
            this.Add("1000x800");
            this.Add("600x300");
            this.Add("640x480");
        }
    }


}
