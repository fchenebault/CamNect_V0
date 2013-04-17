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
        private static ObservableCollection<CamConfig> ligne = new ObservableCollection<CamConfig>();



        public static ObservableCollection<CamConfig> Ligne
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
           }
       }



       #region DraggedItem

       /// <summary>
       /// DraggedItem Dependency Property
       /// </summary>
       public static readonly DependencyProperty DraggedItemProperty =
           DependencyProperty.Register("DraggedItem", typeof(CamConfig), typeof(ConfigCamWindow));

       /// <summary>
       /// Gets or sets the DraggedItem property.  This dependency property 
       /// indicates ....
       /// </summary>
       public CamConfig DraggedItem
       {
           get { return (CamConfig)GetValue(DraggedItemProperty); }
           set { SetValue(DraggedItemProperty, value); }
       }

       #endregion




       #region edit mode monitoring

       /// <summary>
       /// State flag which indicates whether the grid is in edit
       /// mode or not.
       /// </summary>
       public bool IsEditing { get; set; }

       private void OnBeginEdit(object sender, DataGridBeginningEditEventArgs e)
       {
           IsEditing = true;
           //in case we are in the middle of a drag/drop operation, cancel it...
           if (IsDragging) ResetDragDrop();
       }

       private void OnEndEdit(object sender, DataGridCellEditEndingEventArgs e)
       {
           IsEditing = false;
       }

       #endregion

       #region Drag and Drop Rows

       /// <summary>
       /// Keeps in mind whether
       /// </summary>
       public bool IsDragging { get; set; }

       /// <summary>
       /// Initiates a drag action if the grid is not in edit mode.
       /// </summary>
       private void OnMouseLeftButtonDownGrid(object sender, MouseButtonEventArgs e)
       {
           if (IsEditing) return;

           var row = UIHelpers.TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition(dgCamConfig));
           if (row == null || row.IsEditing) return;

           //set flag that indicates we're capturing mouse movements
           IsDragging = true;
           DraggedItem = (CamConfig)row.Item;
       }


       /// <summary>
       /// Completes a drag/drop operation.
       /// </summary>
       private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
       {
           if (!IsDragging || IsEditing)
           {
               return;
           }

           //get the target item
           CamConfig targetItem = (CamConfig)dgCamConfig.SelectedItem;

           if (targetItem == null || !ReferenceEquals(DraggedItem, targetItem))
           {
               //remove the source from the list
               ligne.Remove(DraggedItem);

               //get target index
               var targetIndex = ligne.IndexOf(targetItem);

               //move source at the target's location
               ligne.Insert(targetIndex, DraggedItem);

               //select the dropped item
               dgCamConfig.SelectedItem = DraggedItem;
               reindexCam();
           }



           //reset
           ResetDragDrop();
           //reindexCam();
       }


       /// <summary>
       /// Closes the popup and resets the
       /// grid to read-enabled mode.
       /// </summary>
       private void ResetDragDrop()
       {
           IsDragging = false;
           popup1.IsOpen = false;
           dgCamConfig.IsReadOnly = false;
       }


       /// <summary>
       /// Updates the popup's position in case of a drag/drop operation.
       /// </summary>
       private void OnMouseMove(object sender, MouseEventArgs e)
       {
           if (!IsDragging || e.LeftButton != MouseButtonState.Pressed) return;

           //display the popup if it hasn't been opened yet
           if (!popup1.IsOpen)
           {
               //switch to read-only mode
               dgCamConfig.IsReadOnly = true;

               //make sure the popup is visible
               popup1.IsOpen = true;
           }


           Size popupSize = new Size(popup1.ActualWidth, popup1.ActualHeight);
           popup1.PlacementRectangle = new Rect(e.GetPosition(this), popupSize);

           //make sure the row under the grid is being selected
           Point position = e.GetPosition(dgCamConfig);
           var row = UIHelpers.TryFindFromPoint<DataGridRow>(dgCamConfig, position);
           if (row != null) dgCamConfig.SelectedItem = row.Item;
       }

       #endregion



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
            AddCam(new CamConfig(true, "cameraX", "122554", "hidden","URL", true, false, true, "21255", 5, true));
            ligne = new ObservableCollection<CamConfig>(ligne.OrderByDescending(a => a.Plugged).ThenBy(a => a.Fenetre));
            int index;
            foreach (CamConfig l in ligne)
            {
                index = ligne.IndexOf(l);
                l.Fenetre = index + 1;
            }
            //
            dgCam.ItemsSource = ligne;
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


}
