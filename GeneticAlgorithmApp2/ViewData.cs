using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticAlgorithmLib;

namespace GeneticAlgorithmApp2
{
    public class ViewData : IDataErrorInfo, INotifyPropertyChanged
    {
        public enum States
        {
            Settings,
            Solving,
            Paused,
            Waiting
        }

        private States _state;
        private int _n;
        private int _r;
        private int _k;
        private int _populationSize;
        private int _survivors;
        private double _mutationChance;
        private string _modelInfo;
        public DataView _modelItemSource;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public States State {
            get{
                return _state;
            }
            set{
                _state = value;
                OnPropertyChanged("State");
            }
        }
        public int N {
            get
            {
                return _n;
            }
            set
            {
                _n = value;
                OnPropertyChanged("N");
            }
        }
        public int R {
            get
            {
                return _r;
            }
            set
            {
                _r = value;
                OnPropertyChanged("R");
            }
        }
        public int K {
            get
            {
                return _k;
            }
            set
            {
                _k = value;
                OnPropertyChanged("K");
            }
        }
        public int PopulationSize {
            get
            {
                return _populationSize;
            }
            set
            {
                _populationSize = value;
                OnPropertyChanged("PopulationSize");
            }
        }
        public int Survivors {
            get
            {
                return _survivors;
            }
            set
            {
                _survivors = value;
                OnPropertyChanged("Survivors");
            }
        }
        public double MutationChance {
            get
            {
                return _mutationChance;
            }
            set
            {
                _mutationChance = value;
                OnPropertyChanged("MutationChance");
            }
        }
        public string ModelInfo {
            get
            {
                return _modelInfo;
            }
            set
            {
                _modelInfo = value;
                OnPropertyChanged("ModelInfo");
            }
        }
        public DataView ModelItemSource
        {
            get
            {
                return _modelItemSource;
            }
            set
            {
                _modelItemSource = value;
                OnPropertyChanged("ModelItemSource");
            }
        }


        public AlgorithmState AlgorithmState { get; set; }

        public ViewData() {
            State = States.Settings;
            N = 8;
            R = 4;
            K = 10;
            PopulationSize = 100;
            Survivors = 40;
            MutationChance = 0.03;
            ModelInfo = "Set up a new model or import an old one :3";
        }
        public void UpdateAppData()
        {
        }

        string ErrorMsg = null;
        public string Error => ErrorMsg;
        public string this[string columnName]
        {
            get
            {
                ErrorMsg = null;
                switch (columnName) {
                    case nameof(R):
                        if (R < 1)
                        {
                            ErrorMsg = "must be at least 1 round";
                        }
                        break;
                    case nameof(N):
                        if (N <= R)
                        {
                            ErrorMsg = "must be more participants than rounds";
                        }
                        break;
                    case nameof(K):
                        if (K < N)
                        {
                            ErrorMsg = "must be at least as many venues as participants";
                        }
                        break;
                    case nameof(PopulationSize):
                        if (PopulationSize < 50)
                        {
                            ErrorMsg = "population size must be at least 50";
                        }
                        break;
                    case nameof(Survivors):
                        if (Survivors <= 0)
                        {
                            ErrorMsg = "survivors cannot be negative number";
                        }
                        break;
                    case nameof(MutationChance):
                        if (MutationChance < 0 || MutationChance > 1)
                        {
                            ErrorMsg = "mutation chance must be between 0 and 1";
                        }
                        break;
                    default:
                        break;
                }
                return ErrorMsg;
            }
        }
    }
}
