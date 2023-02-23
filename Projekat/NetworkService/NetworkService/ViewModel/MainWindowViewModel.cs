using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        //private int count = 15; // Inicijalna vrednost broja objekata u sistemu
                                  // ######### ZAMENITI stvarnim brojem elemenata
                                  //           zavisno od broja entiteta u listi
        //private int count = DisplayViewModel.Generators.Count(); // ===> Zbog nekog razloga ne radi kada ovo stavim u count, ali radi ako direktno stavim u kod dole.

        /*
        public int Count
        {
            get { return count; }
        }*/


        public MyICommand<string> NavCommand { get; private set; }

        private SluzbeniciGraphViewModel sluzbeniciGraphViewModel = new SluzbeniciGraphViewModel();
        private DisplayViewModel displayViewModel = new DisplayViewModel();
        private GraphViewModel graphViewModel = new GraphViewModel();
        private GridViewModel gridViewModel = new GridViewModel();

        private BindableBase currentViewModel;

        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "view1":
                    CurrentViewModel = displayViewModel;
                    break;
                case "view2":
                    CurrentViewModel = gridViewModel;
                    break;
                case "view3":
                    CurrentViewModel = graphViewModel;
                    break;
                case "view4":
                    CurrentViewModel = sluzbeniciGraphViewModel;
                    break;
            }
        }

        private int id;
        private int value;
        //private bool file = false; -> Mozda ovo ne treba.
        private Uri path = new Uri("LogFile.txt", UriKind.Relative);

        public MainWindowViewModel()
        {
            NavCommand = new MyICommand<string>(OnNav);
            CurrentViewModel = displayViewModel;

            createListener(); //Povezivanje sa serverskom aplikacijom
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25565);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(DisplayViewModel.Generators.Count().ToString());    // Uzeto koliko elemenata ima trenutno u tabeli.
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji

                            string[] split = incomming.Split('_', ':'); // Zbog _ i : u nazivu entiteta.
                            int index = int.Parse(split[1]);

                            if (DisplayViewModel.Generators.Count() > index) { id = DisplayViewModel.Generators[index].ID;  }
                            else { id = -1; }

                            value = int.Parse(split[2]);
                            Generator gen = new Generator(id);

                            if (id != -1)
                            {
                                DisplayViewModel.Generators[index].Value = value;
                            }
                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }
    }
}
