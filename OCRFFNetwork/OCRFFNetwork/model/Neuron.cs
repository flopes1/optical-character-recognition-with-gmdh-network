using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OCRFFNetwork.model
{
    public class Neuron
    {

        public Neuron()
        {

        }

        public Neuron(int index, int numberOfConnections, SigmoidFunction function, bool isbias, bool isFirstLayer = false)
        {
            this.ActivationFunction = function;
            this.NumberConnections = numberOfConnections;
            this.Index = index;
            this.IsFirstLayerNeuron = isFirstLayer;
            this.IsBias = isbias;

            this.LoadWeightsConfigurations();
            if (!IsFirstLayerNeuron)
            {
                this.Weights = new double[NumberConnections];
                this.Randomize();
            }
        }


        #region Internal Methods

        private void LoadWeightsConfigurations()
        {
            this.Rand = new Random(Seed);
            Seed++;
        }

        private void Randomize()
        {
            double d = _range.Length;

            for (int i = 0; i < NumberConnections; i++)
            {
                _weights[i] = _rand.NextDouble() * d + _range.Min;
            }
        }

        public void CalculateNeuronOutput(ObservableCollection<double> input)
        {
            if (this.IsFirstLayerNeuron)
            {
                if (this.IsBias)
                {
                    this.Output = 1;
                    this.Input = 1;
                }
                else
                {
                    this.Input = input[this.Index - 1];
                    this.Output = input[this.Index - 1];
                }
            }
            else
            {
                if (!this.IsBias)
                {
                    var net = this.CalculateLiquidInput(input);
                    this.Input = net;
                    this.Output = this.ActivationFunction.Calculate(this.Input);
                }
                else
                {
                    this.Output = 1;
                    this.Input = 1;
                }
            }
        }

        public void UpdateError(double yi, double di)
        {
            this.Error = (di - yi);
        }

        private double CalculateLiquidInput(ObservableCollection<double> inputs)
        {

            // initial sum value
            double net = 0.0;

            // compute weighted sum of inputs
            for (int i = 0; i < this.NumberConnections; i++)
            {
                net += this.Weights[i] * inputs[i];
            }

            return net;
        }

        #endregion //Internal Methods

        #region Properties

        private double[] _weights;

        public double[] Weights
        {
            get
            {
                return _weights;
            }

            set
            {
                if (value != _weights)
                {
                    _weights = value;
                }

            }
        }

        private double _input;

        public double Input
        {
            get
            {
                return _input;
            }

            set
            {
                if (value != _input)
                {
                    _input = value;
                }
            }
        }

        private double _output;

        public double Output
        {
            get
            {
                return _output;
            }

            set
            {
                if (value != _output)
                {
                    _output = value;
                }
            }
        }

        private int _numberConnections;

        public int NumberConnections
        {
            get
            {
                return _numberConnections;
            }

            set
            {
                if (value != _numberConnections)
                {
                    _numberConnections = value;
                }
            }
        }

        private int _index;
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (value != _index)
                {
                    _index = value;
                }
            }
        }

        private SigmoidFunction _activationFunction;

        public SigmoidFunction ActivationFunction
        {
            get
            {
                return _activationFunction;
            }

            set
            {
                if (value == _activationFunction)
                {
                    return;
                }

                _activationFunction = value;
            }

        }

        private Random _rand;

        private Random Rand
        {
            get
            {
                return _rand;
            }

            set
            {
                if(value != _rand)
                {
                    _rand = value;
                }
            }
        }

        private DoubleRange _range = new DoubleRange(-1.0, 1.0);

        private bool _isFirstLayerNeuron;
        public bool IsFirstLayerNeuron
        {
            get
            {
                return _isFirstLayerNeuron;
            }

            set
            {
                if (value != _isFirstLayerNeuron)
                {
                    _isFirstLayerNeuron = value;
                }
            }
        }

        private static int _seed = Network.Default.Seed;
        public static int Seed
        {
            get
            {
                return _seed;
            }
            private set
            {
                if (value != _seed)
                {
                    _seed = value;
                }
            }
        }

        public double Error { get; private set; }

        public bool IsBias { get; private set; }

        #endregion


    }
}
