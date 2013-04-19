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
    /// <summary>
    /// Logique d'interaction pour ConfigCamWindow.xaml
    /// </summary>
    
   
    public partial class ConfigCamWindow : Window
    {
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
               try
               {
                   maxFenetre = ligne.Max(a => a.Fenetre);
               }
               catch { }
           }

       }



     



        public ConfigCamWindow()
        {
            InitializeComponent();


            dgCam = dgCamConfig;
            CamConfigCollection();

            //Trie les objets en deux categirie, true/false, puis en fonction du numero de fenetre
            ligne = new ObservableCollection<CamConfig>(ligne.OrderByDescending(a => a.Plugged).ThenBy(a => a.Fenetre));
            
            //Reindex le numero de fenetre
            int index;
            foreach (CamConfig l in ligne)
            {
                index = ligne.IndexOf(l);
                l.Fenetre = index + 1;
                l.Plugged = false;
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
            this.Hide();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            StreamWriter jsonfile = new StreamWriter("../../Ressources/Config/config.json", false);

           

            string json = JsonConvert.SerializeObject(ligne, Formatting.Indented);
            jsonfile.WriteLine(json);
            jsonfile.Close();

            this.Hide();
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
            foreach (CameraUtils u in CameraOne.cameraList)
            {
                System.Console.WriteLine(u.Config.Nom);
            }
            
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
