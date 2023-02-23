using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class SluzbeniciGraphViewModel : BindableBase
    {
        public static GraphUpdater ElementHeights { get; set; } = new GraphUpdater();

        /*
        private static int NumOfType0 { get; set; } // Solarni generator
        private static int NumOfType1 { get; set; } // Vetrogenerator

        public void NumOfTypes()
        {
            foreach (Generator gen in DisplayViewModel.Generators)
            {
                if (gen.Type.Name == "Solarni generator") { NumOfType0++; }
                else { NumOfType1++; }
            }
        }*/

        private static double CurrentLengthSolarni;
        private static double CurrentLengthVetro;
        //private static int CurrentLength = 0;

        public static double CalculateElementLength(int id, string command)
        {
            if (command == "add") 
            { 
                if (id == 0) { return CurrentLengthSolarni += 27.5; }
                else { return CurrentLengthVetro += 27.5; }
            }
            else if (command == "delete") 
            {
                if (id == 0) { return CurrentLengthSolarni -= 27.5; }
                else { return CurrentLengthVetro -= 27.5; }
            }

            return 0;
        }
    }
}
