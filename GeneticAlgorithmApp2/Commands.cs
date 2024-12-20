using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GeneticAlgorithmApp2
{
    class Commands
    {
        public static RoutedCommand SolveCommand = new("SolveCommand", typeof(Commands));
        public static RoutedCommand TerminateCommand = new("TerminateCommand", typeof(Commands));
        public static RoutedCommand ExportCommand = new("ExportCommand", typeof(Commands));
        public static RoutedCommand ImportCommand = new("ImportCommand", typeof(Commands));
    }
}
