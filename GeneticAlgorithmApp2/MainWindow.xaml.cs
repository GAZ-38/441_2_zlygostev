using GeneticAlgorithmLib;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.DirectoryServices;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using System.IO;

namespace GeneticAlgorithmApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewData _viewData = new();
        Task SolverTask;
        bool DBIsEmpty, ExportIsEnabled = false;
        public List<string> DB;

        string DBPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Database");

        public MainWindow()
        {
            DataContext = _viewData;
            if (!System.IO.Directory.Exists(DBPath))
            {
                System.IO.Directory.CreateDirectory(DBPath);
            }
            if (System.IO.Directory.EnumerateFiles(DBPath).Count() == 0)
            {
                DBIsEmpty = true;
                DB = new();
                File.WriteAllText(System.IO.Path.Combine(DBPath, "DB.json") , JsonConvert.SerializeObject(DB));
            }
            else
            {
                DB = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(System.IO.Path.Combine(DBPath, "DB.json")));
            }
            InitializeComponent();
        }
        public void SetCheckboxLock(bool isLocked)
        {
            N_Box.IsReadOnly = R_Box.IsReadOnly = K_Box.IsReadOnly = isLocked;
            PopulationSize_Box.IsReadOnly = Survivors_Box.IsReadOnly = Mutation_Box.IsReadOnly = isLocked;
        }

        public void UpdateModelView()
        {
            var bestSolution = _viewData.AlgorithmState.Best();
            _viewData.ModelInfo = $"Epoch: {_viewData.AlgorithmState._epoch}; Opponents-metric: {bestSolution.MetricOpponents};" +
                $" Venues-metric {bestSolution.MetricLocations}";
            DataTable dataTable = new DataTable("Venues for each participant");
            dataTable.Columns.Add(new DataColumn("rounds", typeof(string)));
            for(int i = 1; i <= bestSolution.N; ++i)
            {
                dataTable.Columns.Add(new DataColumn($"{i}", typeof(int)));
            }
            for (int i = 0; i < bestSolution.R; ++i)
            {
                DataRow row = dataTable.NewRow();
                row[0] = $"round {i + 1}";
                for (int j = 0; j < bestSolution.N; ++j)
                {
                    row[j + 1] = bestSolution.SolutionMatrix[i, j];
                }
                dataTable.Rows.Add(row);
            }
            _viewData.ModelItemSource = dataTable.DefaultView;
        }

        public void StartSolving()
        {
            SolverTask = Task.Factory.StartNew(() =>
            {
                GeneticAlgorithm solver = new(_viewData.AlgorithmState);
                AlgorithmState newState;
                while (true)
                {
                    newState = solver.IterateSolutions();
                    if (newState._epoch % Constants.SOLUTION_DISPLAY_INTERVAL == 0)
                    {
                        _viewData.AlgorithmState = newState;
                        UpdateModelView();
                    }
                    if(_viewData.State != ViewData.States.Solving)
                    {
                        _viewData.AlgorithmState = newState;
                        UpdateModelView();
                        break;
                    }
                }
            });
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            if(_viewData.State == ViewData.States.Solving)
            {
                ExportIsEnabled = true;
                _viewData.State = ViewData.States.Waiting;
                SolverTask.Wait();
                UpdateModelView();
                _viewData.State = ViewData.States.Paused;
            }
            else
            {
                if(_viewData.State == ViewData.States.Settings)
                {
                    SetCheckboxLock(true);
                    _viewData.AlgorithmState = new(_viewData.N, _viewData.K, _viewData.R, _viewData.PopulationSize,
                        _viewData.Survivors, 0, _viewData.MutationChance);
                    UpdateModelView();
                }
                StartSolving();
                _viewData.State = ViewData.States.Solving;

            }
        }
        private void CanSolveHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Validation.GetHasError(N_Box) ||
                Validation.GetHasError(R_Box) ||
                Validation.GetHasError(K_Box) ||
                Validation.GetHasError(PopulationSize_Box) ||
                Validation.GetHasError(Survivors_Box) ||
                Validation.GetHasError(Mutation_Box) ||
                _viewData.State == ViewData.States.Waiting)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void Terminate_Click(object sender, RoutedEventArgs e)
        {
            //reset everything
            _viewData.ModelItemSource = new();
            _viewData.ModelInfo = "Model is terminated. Set up a new model or import an old one";
            SetCheckboxLock(false);
            ExportIsEnabled = false; ;
            _viewData.State = ViewData.States.Settings;
        }
        private void CanTerminateHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_viewData.State != ViewData.States.Paused || _viewData.State == ViewData.States.Waiting)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            DBIsEmpty = false;
            string newJSONName = $"{DB.Count() + 1}.json";
            string JSON = JsonConvert.SerializeObject(_viewData.AlgorithmState);
            File.WriteAllText(System.IO.Path.Combine(DBPath, newJSONName), JSON);
            DB.Add(newJSONName);
            File.WriteAllText(System.IO.Path.Combine(DBPath, "DB.json"), JsonConvert.SerializeObject(DB));
            ExportIsEnabled = false;
        }
        private void CanExportHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_viewData.State != ViewData.States.Paused || _viewData.State == ViewData.States.Waiting || !ExportIsEnabled)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            _viewData.State = ViewData.States.Waiting;
            var importDialog = new ImportDialogueWindow(DB);
            importDialog.ShowDialog();
            if(importDialog.Selection != null)
            {
                _viewData.AlgorithmState = JsonConvert.DeserializeObject<AlgorithmState>(File.ReadAllText(
                    System.IO.Path.Combine(DBPath, importDialog.Selection)));
                UpdateModelView();
                //_viewData.ModelInfo = File.ReadAllText(System.IO.Path.Combine(DBPath, importDialog.Selection));
                //ExportIsEnabled = false;
            }
            _viewData.State = ViewData.States.Paused;
            //import a model, update parameters
        }
        private void CanImportHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!(_viewData.State == ViewData.States.Paused || _viewData.State == ViewData.States.Settings)
                || _viewData.State == ViewData.States.Waiting || DBIsEmpty)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }
    }
}