using CamNect.Camera;
using ManagedUPnP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamNect.Camera
{
    public delegate void CameraListModifiedEventHandler(List<CameraUtils> cameraList);

    public class CameraDiscovery
    {
        private static CameraDiscovery _instance;
        public Boolean isSearching = false;

        private Discovery _discovery;
        private List<CamConfig> _defaultConfig;
        private List<CameraUtils> _cameraList;

        private CameraDiscovery()
        {

            _defaultConfig = new List<CamConfig>();
            _cameraList = new List<CameraUtils>();

            _discovery = new Discovery(null, AddressFamilyFlags.IPv4, false);
            _discovery.DeviceAdded += new DeviceAddedEventHandler(discDeviceAdded);
            _discovery.DeviceRemoved += new DeviceRemovedEventHandler(discovery_DeviceRemoved);
            _discovery.SearchComplete += new SearchCompleteEventHandler(discovery_SearchComplete);
            loadDatabase();
        }


        public static CameraDiscovery Instance()
        {
            if (_instance == null) _instance = new CameraDiscovery();
            return _instance;
        }

        public void Start()
        {
            //if (!_discovery.Searching)
            if (!isSearching)
            {
                isSearching = true;
                _discovery.Start();
            }
        }

        public void loadDatabase()
        {
            String json = null;

            if (!File.Exists("../../Ressources/Config/defaultconfig.json"))
            {
                StreamWriter jsonfile = new StreamWriter("../../Ressources/Config/defaultconfig.json", false);
                jsonfile.WriteLine("[]");
                jsonfile.Close();
            }
            else
            {
                json = File.ReadAllText("../../Ressources/Config/defaultconfig.json");
                _defaultConfig = JsonConvert.DeserializeObject<List<CamConfig>>(json);
            }
        }

        private void discovery_SearchComplete(object sender, SearchCompleteEventArgs e)
        {
            Console.WriteLine("-------------- discovery_SearchComplete");
            isSearching = false;
        }

        private void discovery_DeviceRemoved(object sender, DeviceRemovedEventArgs e)
        {
            Console.WriteLine("------------------ discovery_DeviceRemoved");
        }

        public event CameraListModifiedEventHandler CameraListModified;



        public bool IsCameraPlugged(string id)
        {
            return _defaultConfig.Where(c => c.Id == id).Count() != 0;
        }


        public void discDeviceAdded(object sender, DeviceAddedEventArgs a)
        {

            bool camExist = false;

            /* Find a configuration */
            foreach (CameraUtils cfg in _cameraList)
            {
                if (a.Device.SerialNumber.ToString() == cfg.Config.Serie)
                {
                    camExist = true;


                    _cameraList.Add(new CameraUtils(a.Device.RootHostAddresses[0].ToString(), cfg.Config));
                    break;
                }
            }
               
            if (!camExist)
            {
                foreach (CamConfig cfg in _defaultConfig)
                {
                    if (a.Device.FriendlyName.Contains(cfg.Modele))
                    {
                        System.Console.WriteLine("-- NOUVELLE CAMERA --");
                        System.Console.WriteLine(cfg.Modele);
                        System.Console.WriteLine(a.Device.SerialNumber.ToString());

                        cfg.Serie = a.Device.SerialNumber.ToString();

                        CamConfig cfgAux = new CamConfig();
                        cfgAux.Clone(cfg);

                        //ConfigCamWindow.AddCam(cfgAux);

                        _cameraList.Add(new CameraUtils(a.Device.RootHostAddresses[0].ToString(), cfgAux));
                        break;
                    }
                }
            }
          

            CameraListModified(_cameraList);
        }





        public List<CameraUtils> CameraList { get { return _cameraList; } }
    }
}
