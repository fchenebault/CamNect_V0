using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamNect.Camera
{
    public class CamConfig
    {
        public bool Afficher { get; set; }
        public string Nom { get; set; }
        public string Id { get; set; }
        public string Pass { get; set; }
        public string Resolution { get; set; }
        public string Uri { get; set; }
        public bool isPtz { get; set; }
        public bool PtzOn = true;
        public bool Zoom { get; set; }
        public string Modele { get; set; }
        public string Serie { get; set; }
        public string UDN { get; set; }
        public int Fenetre { get; set; }
        public bool Plugged  { get; set; }
     
       

        public CamConfig()
        {
            Afficher = true;
            Resolution = "640x480";
        }

        public CamConfig(bool afficher, string nom, string id, string pass, string uri, bool isPtz, bool zoom, string serie, string udn, int fenetre, bool plugged)
        {
            this.Afficher = afficher;
            this.Nom = nom;
            this.Id = id;
            this.Serie = serie;
            this.UDN = udn;
            this.Pass = pass;
            this.Uri = uri;
            this.isPtz = isPtz;
            this.Zoom = zoom;
            this.Fenetre = fenetre;
            this.Plugged = plugged;
        }

        public void Clone(CamConfig cfg)
        {
            this.Afficher = cfg.Afficher;
            this.Nom = cfg.Nom;
            this.Id = cfg.Id;
            this.Pass = cfg.Pass;
            this.Resolution = cfg.Resolution;
            this.Uri = cfg.Uri;
            this.isPtz = cfg.isPtz;
            this.PtzOn = cfg.PtzOn;
            this.Zoom = cfg.Zoom;
            this.Modele = cfg.Modele;
            this.Serie = cfg.Serie;
            this.Fenetre = cfg.Fenetre;
            this.Plugged = cfg.Plugged;
        }
    }
}
