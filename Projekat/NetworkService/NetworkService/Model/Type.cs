using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Type
    {
        private string name;
        private string imgSrc;

        // "Prazan" konstruktor koji ce da za default samo stavi prazne stringove.
        public Type() { Name = string.Empty; ImgSrc = string.Empty; }

        public Type(string _name, string _imgSrc)
        {
            Name = _name;
            ImgSrc = _imgSrc;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public string Name
        {
            get { return name; }
            set{ if (name != value) { name = value; RaisePropertyChanged("Name");  } }
        }

        public string ImgSrc
        {
            get { return imgSrc; }
            set { if (imgSrc != value) { imgSrc = value; RaisePropertyChanged("ImgSrc"); } }
        }


    }
}
