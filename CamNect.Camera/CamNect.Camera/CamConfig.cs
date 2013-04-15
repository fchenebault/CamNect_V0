﻿using System;
using System.Collections.Generic;
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
        public string Uri { get; set; }
        public bool isPtz { get; set; }
        private bool ptzOn = false;
        public bool Zoom { get; set; }
        public string Modele { get; set; }
        public string Serie { get; set; }
        public int Rank { get; set; }
        private bool show = false;

        public bool Show
        {
            get;
            set;
        }

        public bool PtzOn
        {
            get;
            set;
        }

        public CamConfig(bool afficher, string nom, string id, string pass, string uri, bool isPtz, bool zoom, string serie, int rank, bool show)
        {

            this.Afficher = afficher;
            this.Nom = nom;
            this.Id = id;
            this.Serie = serie;
            this.Pass = pass;
            this.Uri = uri;
            this.isPtz = isPtz;
            this.Zoom = zoom;
            this.Rank = rank;
            this.show = show;
        }
    }
}
