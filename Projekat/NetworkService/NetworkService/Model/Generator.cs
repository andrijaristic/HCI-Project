using NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Generator : INotifyPropertyChanged
    {
        private int id;
        private string name;
        private Type type;
        private int value;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public int ID
        {
            get { return id; }
            set { if (id != value) { id = value; RaisePropertyChanged("ID"); } }
        }

        public string Name
        {
            get { return name; }
            set { if (name != value) { name = value; RaisePropertyChanged("Name"); } }
        }

        public Type Type
        {
            get { return type; }
            set { if (type != value) { type = value; RaisePropertyChanged("Type"); } }
        }

        public int Value
        {
            get { return value; }
            set 
            {
                this.value = value;
                RaisePropertyChanged("Value");
                GraphViewModel.ElementHeights.FirstBindingPoint = GraphViewModel.CalculateElementHeight(value, ID);
            }
        }

        public Generator() { }

        public Generator(int id) { ID = id; }

        public Generator(Generator gen)
        {
            ID = gen.ID;
            Name = gen.Name;
            Type = gen.Type;
            Value = gen.Value;
        }

    }

}
