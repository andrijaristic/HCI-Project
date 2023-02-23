using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkService.ViewModel
{
    public class GridViewModel : BindableBase
    {
        public Generator draggedItem = null;
        public bool dragging = false;
        public bool exists = false;
        private int selectedIndex = 0;  // Indeksiranje objekata u samom Drag & Drop gridu.
        public static Dictionary<string, Generator> CanvasObject { get; set; } = new Dictionary<string, Generator>(); // Svi elementi dodati u Canvas.

        private ListView listView;
        public BindingList<Generator> Items { get; set; }

        public Canvas sourceCanvas;

        // Spisak komandi.
        public MyICommand<Canvas> DropCommand { get; set; }
        public MyICommand<Canvas> FreeCommand { get; set; }
        public MyICommand<Canvas> LoadCommand { get; set; } // Ucitavanje svega sto je bilo, u slucaju da smo promenili view.
        public MyICommand<Canvas> DragOverCommand { get; set; }
        public MyICommand<Canvas> DragLeaveCommand { get; set; }

        public MyICommand<ListView> LoadListViewCommand { get; set; }
        public MyICommand<ListView> LeftMouseButtonUpCommand { get; set; }
        public MyICommand<Generator> SelectionChangedCommand { get; set; }
        public MyICommand<Canvas> LeftMouseButtonDownCommand { get; set; }
        
       // public MyICommand<UserControl> UCLoadedCommand { get; set; }

        public List<Canvas> CanvasList { get; set; } = new List<Canvas>();  // Spisak svih Canvasa.

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { if (selectedIndex != value) { selectedIndex = value; OnPropertyChanged("SelectedIndex"); } }
        }

        public GridViewModel()
        {
            // Provera da li taj element vec postoji u Grid-u. 
            Items = new BindingList<Generator>();
            foreach (Generator gen in DisplayViewModel.Generators)
            {
                exists = false;
                foreach (Generator gen2 in CanvasObject.Values)
                {
                    if (gen.ID == gen2.ID) { exists = true; break; }
                }
                if (exists == false) { Items.Add(new Generator(gen)); }
            }

            // Deklaracija komandi.
            DropCommand = new MyICommand<Canvas>(OnDrop);               // Drop.
            FreeCommand = new MyICommand<Canvas>(OnFree);
            LoadCommand = new MyICommand<Canvas>(OnLoad);
            DragOverCommand = new MyICommand<Canvas>(OnDragOver);       // Drag i postavi.
            DragLeaveCommand = new MyICommand<Canvas>(OnDragLeave);     // Drag i odustani. (Ne smestis nigde nego samo kliknes negde drugde.)

            LoadListViewCommand = new MyICommand<ListView>(OnLoadListView);
            SelectionChangedCommand = new MyICommand<Generator>(OnSelectionChanged);
            LeftMouseButtonUpCommand = new MyICommand<ListView>(OnLeftMouseButtonUp);
            LeftMouseButtonDownCommand = new MyICommand<Canvas>(OnLeftMouseButtonDown);
            //UCLoadedCommand = new MyICommand<UserControl>(OnUCLoad);
        }

        /*
        private void OnUCLoad(UserControl uc)
        {
            uc.Focus();
        }*/

        // Preuzimanje liste iz DisplayView (Nas glavni view).
        public void OnLoadListView(ListView lv)
        {
            listView = lv;
        }

        // Da omoguci smao dragovanje.
        public void OnSelectionChanged(Generator gen)
        {
            if (!dragging)
            {
                dragging = true;
                draggedItem = new Generator(gen);
                DragDrop.DoDragDrop(listView, draggedItem, DragDropEffects.Move);
            }
        }

        // Kada ne pritiskamo LMB, nista nije selektovano. Da ne dodje do toga da program zapamti zadnje sto smo drzali.
        public void OnLeftMouseButtonUp(ListView lv)
        {
            dragging = false;
            draggedItem = null;
            lv.SelectedItem = null;
        }

        public void OnLeftMouseButtonDown(Canvas c)
        {
            if (!dragging && c.Resources["taken"] != null)
            {
                dragging = true;
                //draggedItem = (Generator)((ListView)c.Children[1]).Items[0];
                draggedItem = CanvasObject[c.Name];
                sourceCanvas = c;
                DragDrop.DoDragDrop(c.Parent, draggedItem, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void ValueCheck(Canvas c)
        {
            Dictionary<int, Generator> temp = new Dictionary<int, Generator>();

            foreach (Generator gen in DisplayViewModel.Generators)
            {
                temp.Add(gen.ID, gen);
            }
            Task.Delay(1000).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ((Border)(c).Children[0]).BorderBrush = Brushes.Transparent;

                    if (CanvasObject.Count != 0)
                    {
                        if (CanvasObject.ContainsKey(c.Name))
                        {
                            if (temp[CanvasObject[c.Name].ID].Value < 1 || temp[CanvasObject[c.Name].ID].Value > 5)
                            {
                                ((Border)(c).Children[0]).BorderBrush = Brushes.Red;
                            }
                        }
                    }
                });
                ValueCheck(c);
            });
        }

        public void OnLoad(Canvas c)
        {
            if (CanvasObject.ContainsKey(c.Name))   // Ako Canvas sadrzi OVAJ SPECIFICAN ELEMENT.
            {
                BitmapImage slika = new BitmapImage();

                slika.BeginInit();
                string tempLoc = CanvasObject[c.Name].Type.Name.ToString() + ".png";  // Upis lokacije slike.
                slika.UriSource = new Uri("pack://application:,,,/Images/" + tempLoc, UriKind.Absolute);
                slika.EndInit();

                c.Background = new ImageBrush(slika);
                ((TextBlock)(c).Children[1]).Text = "";
                c.Resources.Add("taken", true);

                ValueCheck(c);
            }

            if (!CanvasList.Contains(c)) { CanvasList.Add(c); }
        }

        public void OnDrop(Canvas c)
        {
            if (((TextBlock)(c).Children[1]).Text == "!")
            {
                ((TextBlock)(c).Children[1]).Text = "";
                ((TextBlock)(c).Children[1]).Foreground = Brushes.White;
            }

            if (draggedItem != null)
            {
                if (c.Resources["taken"] == null)
                {
                    BitmapImage slika = new BitmapImage();  // Pocetak bitmape kako bi ubacili sliku.

                    slika.BeginInit();
                    string tempLoc = draggedItem.Type.Name.ToString() + ".png";  // Upis lokacije slike.
                    slika.UriSource = new Uri("pack://application:,,,/Images/" + tempLoc, UriKind.Absolute);
                    slika.EndInit();

                    c.Background = new ImageBrush(slika); // Postavljamo sliku na Canvas slot.
                    CanvasObject[c.Name] = draggedItem; // Objekat na canvasu ce postati sada objekat koji smo prevukli.
                    c.Resources.Add("taken", true);     // Tag koji nam kaze da ne mozemo da stavimo nista tu.
                    //((ListView)c.Children[1]).Items.Add(draggedItem);
                    //Items.Remove(Items.Single(x => x.ID == draggedItem.ID));   

                    if (sourceCanvas != null)
                    {
                        sourceCanvas.Resources.Remove("taken");
                        //((ListView)sourceCanvas.Children[1]).Items.Remove(draggedItem);
                        CanvasObject.Remove(sourceCanvas.Name);
                        sourceCanvas.Background = Brushes.White;
                    } else
                    {
                        Items.Remove(Items.Single(x => x.ID == draggedItem.ID));
                    }


                    ValueCheck(c);
                    SelectedIndex = 0;
                }

                sourceCanvas = null;
                dragging = false;
            }
        }

        public void OnDragOver(Canvas c)
        {
            if (c.Resources["taken"] != null)
            {
                ((TextBlock)(c).Children[1]).Text = "!";
                ((TextBlock)(c).Children[1]).Background = Brushes.Salmon;
            }
        }

        public void OnDragLeave(Canvas c)
        {
            if (((TextBlock)(c).Children[1]).Text == "!")
            {
                ((TextBlock)(c).Children[1]).Text = "";
                ((TextBlock)(c).Children[1]).Background = Brushes.Transparent;
            }
        }

        public void OnFree(Canvas c)
        {
            try
            {
                if (c.Resources["taken"] != null)
                {
                    c.Background = Brushes.White;
                    foreach (Generator gen in DisplayViewModel.Generators)
                    {
                        if (!Items.Contains(gen) && CanvasObject[c.Name].ID == gen.ID) { Items.Add(new Generator(gen)); }
                    }

                    c.Resources.Remove("taken");
                    CanvasObject.Remove(c.Name);
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }
    }
}
