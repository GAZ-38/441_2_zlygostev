using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GeneticAlgorithmApp2
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class ImportDialogueWindow : Window
    {
        public List<string> FileNames = new();
        public string? Selection = null;
        public ImportDialogueWindow(List<string> fileNames)
        {
            //this.InitializeComponent();
            FileNames = fileNames;
            InitializeComponent();
            ModelComboBox.ItemsSource = FileNames;
            ModelComboBox.SelectedIndex = 0;
            Selection = ModelComboBox.SelectedItem as string;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Selection = null;
            Close();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            Selection = ModelComboBox.SelectedItem as string;
            Close();
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            Selection = ModelComboBox.SelectedItem as string;
        }
    }
}
