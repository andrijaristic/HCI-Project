using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NetworkService.ViewModel
{
    public class DisplayViewModel : BindableBase
    {
        public static List<string> GeneratorTypes { get; set; } = new List<string> { "Solarni generator", "Vetrogenerator" };
        public static ObservableCollection<Generator> Generators { get; set; } = new ObservableCollection<Generator>();
        public static ObservableCollection<Generator> GeneratorsCopy { get; set; } = new ObservableCollection<Generator>();
        public static ObservableCollection<Generator> GeneratorsSearch { get; set; } = new ObservableCollection<Generator>();
        public static ObservableCollection<Generator> GeneratorsSearchNew { get; set; } = new ObservableCollection<Generator>();

        //private Generator filledGenerator = new Generator();    // Generator koji unosimo.
        private Generator selectedGenerator;                    // Generator na koji smo kliknuli u tabeli.

        // https://docs.microsoft.com/en-us/dotnet/desktop/wpf/app-development/pack-uris-in-wpf?view=netframeworkdesktop-4.8
        private string pathUri = "pack://application:,,,/Images/";
        private string pathFull = string.Empty;

        private string idTextBox;
        private string nameTextBox;
        private string selectedGeneratorType = string.Empty;    // Pamcenje tipa generatora koji smo izabrali. 
        private string searchTextBox;

        private bool nameChecked = true;
        private bool canCancel = false;
        private bool canSearch = true;

        private int inOrOut = -1;
        private bool canCancelFilter = false;
        private bool canFilter = true;

        public int InOrOut
        {
            get { return inOrOut; }
            set { inOrOut = value; }
        }

        public string SearchTextBox
        {
            get { return searchTextBox; }
            set 
            {
                if (searchTextBox != value)
                {
                    searchTextBox = value;
                    OnPropertyChanged("SearchTextBox");
                    SearchCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string IDTextBox
        {
            get { return idTextBox; }
            set
            {
                if (idTextBox != value)
                {
                    idTextBox = value;
                    OnPropertyChanged("IDTextBox");
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string NameTextBox
        {
            get { return nameTextBox; }
            set
            {
                if (nameTextBox != value)
                {
                    nameTextBox = value;
                    OnPropertyChanged("NameTextBox");
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string PathFull
        {
            get { return pathFull; }
            set {  pathFull = value; OnPropertyChanged("PathFull"); }
        }

        /*
        public Generator FilledGenerator
        {
            get { return FilledGenerator; }
            set { if (filledGenerator != value) { filledGenerator = value; OnPropertyChanged("FilledGenerator"); } }
        }*/

        public Generator SelectedGenerator
        {
            get { return selectedGenerator; }
            set { if (selectedGenerator != value) { selectedGenerator = value; DeleteCommand.RaiseCanExecuteChanged(); } }
        }

        public string SelectedGeneratorType
        {
            get { return selectedGeneratorType; }
            set
            {
                if (selectedGeneratorType != value)
                {
                    selectedGeneratorType = value;
                    PathFull = pathUri + value.ToString() + ".png";
                    OnPropertyChanged("PathFull");
                    OnPropertyChanged("SelectedGeneratorType");
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // Spisak komandi.
        public MyICommand AddCommand { get; set; }
        public MyICommand DeleteCommand { get; set; }
        public MyICommand SearchCommand { get; set; }
        public MyICommand FilterCommand { get; set; }
        public MyICommand CancelSearchCommand { get; set; }
        public MyICommand CancelFilterCommand { get; set; }
        public MyICommand NameCheckedCommand { get; set; }
        public MyICommand TypeCheckedCommand { get; set; }
        public MyICommand InRangeCommand { get; set; }
        public MyICommand OutOfRangeCommand { get; set; }
       //public MyICommand<UserControl> TableLoadedCommand { get; set; }

        public DisplayViewModel()
        {
            GeneratorsSearch = new ObservableCollection<Generator>();

            foreach (Generator gen in Generators)
            {
                GeneratorsSearch.Add(gen);
            }

            AddCommand = new MyICommand(OnAdd, CanAdd);
            DeleteCommand = new MyICommand(OnDelete, CanDelete); // (Funkcija koja ce se obaviti, Uslov za obavljanje)
            SearchCommand = new MyICommand(OnSearch, CanSearch);
            CancelSearchCommand = new MyICommand(OnCancel, CanCancel);
            NameCheckedCommand = new MyICommand(OnNameChecked);
            TypeCheckedCommand = new MyICommand(OnTypeChecked);

            InRangeCommand = new MyICommand(OnInRange);
            OutOfRangeCommand = new MyICommand(OnOutOfRange);
            FilterCommand = new MyICommand(OnFilter, CanFilter);
            CancelFilterCommand = new MyICommand(OnCancelFilter, CanCancelFilter);

            //TableLoadedCommand = new MyICommand<UserControl>(OnTableLoad);
        }

        /*
        private void OnTableLoad(UserControl uc)
        {
            //GeneratorsSearch = Generators;
            uc.Focus();
        }*/

        private void OnInRange()
        {
            InOrOut = 1;
            FilterCommand.RaiseCanExecuteChanged();
        }

        private void OnOutOfRange()
        {
            InOrOut = 2;
            FilterCommand.RaiseCanExecuteChanged();
        }

        public bool CanCancelFilter()
        {
            return canCancelFilter;
        }

        public void OnCancelFilter()
        {
            GeneratorsSearch.Clear();
            foreach (Generator gen in GeneratorsCopy)
            {
                GeneratorsSearch.Add(gen);
            }

            canCancelFilter = false;
            canFilter = true;
            canSearch = true;
            GeneratorsCopy.Clear();
            FilterCommand.RaiseCanExecuteChanged();
            SearchCommand.RaiseCanExecuteChanged();
            CancelFilterCommand.RaiseCanExecuteChanged();
        }

        private bool CanFilter()
        {
            bool filter = false;

            if (InOrOut != -1) { filter = true; }
            else { filter = false; }

            return filter;
        }

        // Isto kao za Search, ne zaboravi da onesposobis Search i da dodas Cancel Filter komande i tastere obavezno!
        // Popravi na Drag&Drop da mozes da radis prebacivanje isto.
        private void OnFilter()
        {
            GeneratorsSearchNew.Clear();
            GeneratorsCopy.Clear();

            foreach (Generator gen in GeneratorsSearch)
            {
                GeneratorsCopy.Add(gen);
            }


            if (InOrOut != -1)
            {
                if (InOrOut == 1)
                {
                    foreach (Generator gen in GeneratorsSearch)
                    {
                        if (gen.Value >= 1 && gen.Value <= 5)
                        {
                            GeneratorsSearchNew.Add(gen);
                        }
                    }
                }
                else if (InOrOut == 2)
                {
                    foreach (Generator gen in GeneratorsSearch)
                    {
                        if (gen.Value < 1 || gen.Value > 5)
                        {
                            GeneratorsSearchNew.Add(gen);
                        }
                    }
                }
            }

            canFilter = false;
            canSearch = false;
            canCancelFilter = true;
            InOrOut = -1;

            AddSearchList();
            CancelFilterCommand.RaiseCanExecuteChanged();
            FilterCommand.RaiseCanExecuteChanged();
            SearchCommand.RaiseCanExecuteChanged();
        }

        private bool CanCancel()
        {
            return canCancel;
        }

        private void OnCancel()
        {
            GeneratorsSearch.Clear();
            foreach (Generator gen in GeneratorsCopy)
            {
                GeneratorsSearch.Add(gen);
            }

            canCancel = false;
            canSearch = true;
            GeneratorsCopy.Clear();
            SearchCommand.RaiseCanExecuteChanged();
        }

        private void OnNameChecked()
        {
            nameChecked = true;
        }

        private void OnTypeChecked()
        {
            nameChecked = false;
        }

        private bool CanSearch()
        {
            bool search = false;

            if (SearchTextBox != string.Empty && canSearch) { search = true; }
            else { search = false; }

            return search;
        }

        private void OnSearch()
        {
            GeneratorsSearchNew.Clear();
            GeneratorsCopy.Clear();

            foreach (Generator gen in GeneratorsSearch)
            {
                GeneratorsCopy.Add(gen);
            }


            if (SearchTextBox != string.Empty)
            {
                if (nameChecked)
                {
                    foreach (Generator gen in GeneratorsSearch)
                    {
                        if (SearchTextBox.Equals(gen.Name))
                        {
                            GeneratorsSearchNew.Add(gen);
                        }
                    }
                } else
                {
                    foreach (Generator gen in GeneratorsSearch)
                    {
                        if (SearchTextBox.Equals(gen.Type.Name))
                        {
                            GeneratorsSearchNew.Add(gen);
                        }
                    }
                }

                canSearch = false;
                canCancel = true;
                AddSearchList();
                CancelSearchCommand.RaiseCanExecuteChanged();
                SearchCommand.RaiseCanExecuteChanged();
            }

        }

        private void AddSearchList()
        {
            GeneratorsSearch.Clear();
            foreach (Generator gen in GeneratorsSearchNew)
            {
                GeneratorsSearch.Add(gen);
            }
            GeneratorsSearchNew.Clear();
        }

        // Provera da li uopste moze da se pokusa dodavanje objekata. Ako nema sve ovo dugme se ne moze pritisnuti.
        private bool CanAdd()
        {
            return (IDTextBox != null && NameTextBox != null && SelectedGeneratorType != null);
        }

        private void OnAdd()
        {
            int tempID = 0;
            string tempName = NameTextBox;

            // Validacija za ID;
            try { tempID = int.Parse(IDTextBox); }
            catch
            {
                MessageBox.Show("ID mora biti broj.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validacija za Name;
            if (string.IsNullOrWhiteSpace(tempName))
            {
                MessageBox.Show("Naziv ne sme biti prazan.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedGeneratorType.Equals(string.Empty))
            {
                MessageBox.Show("Type ne sme biti prazan.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validacija da li vec postoji ID koji se trenutno unosi;
            foreach (Generator gen in Generators)
            {
                if (gen.ID == tempID) 
                {
                    MessageBox.Show("ID vec postoji, unesite drugi.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Pravljenje objekta klase Type i postavljanje slike i samo dodavanje novog objekta klase Generator. 

            Model.Type temp = new Model.Type(SelectedGeneratorType, pathUri + SelectedGeneratorType.ToString() + ".png");

            if (SelectedGeneratorType.Equals(GeneratorTypes[0]))
            {
                SluzbeniciGraphViewModel.ElementHeights.FirstBindingPointSluzbenici = SluzbeniciGraphViewModel.CalculateElementLength(0, "add");
            }
            else
            {
                SluzbeniciGraphViewModel.ElementHeights.SecondBindingPointSluzbenici = SluzbeniciGraphViewModel.CalculateElementLength(1, "add");
            }

            Generators.Add(new Generator { ID = tempID, Name = NameTextBox, Value = 0, Type = temp});
            GeneratorsSearch.Add(new Generator { ID = tempID, Name = NameTextBox, Value = 0, Type = temp });
        }

        // Proverava da li postoji generator (Selected je onaj koji trenutno pritiskamo.)
        private bool CanDelete()
        {
            return (SelectedGenerator != null);
        }

        // Stvarno brise element.
        private void OnDelete()
        {
            if (SelectedGenerator.Type.Name.Equals(GeneratorTypes[0])) 
            {
                SluzbeniciGraphViewModel.ElementHeights.FirstBindingPointSluzbenici = SluzbeniciGraphViewModel.CalculateElementLength(0, "delete");
            } else
            {
                SluzbeniciGraphViewModel.ElementHeights.SecondBindingPointSluzbenici = SluzbeniciGraphViewModel.CalculateElementLength(1, "delete");
            }

            Generator gen = SelectedGenerator;

            Generators.Remove(gen);
            GeneratorsSearch.Remove(SelectedGenerator);

            /*
            for (int i = 0; i <= Generators.Count(); i++)
            {
                if (Generators[i].ID == SelectedGenerator.ID)
                {
                    Generators.RemoveAt(i);
                    break;
                }
            }*/
        }
    }
}
