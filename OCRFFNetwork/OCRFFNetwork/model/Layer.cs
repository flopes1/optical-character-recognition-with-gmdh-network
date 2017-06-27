using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OCRFFNetwork.model
{
    public class Layer
    {
        public Layer(int number, int neuronsCount, int previousLayerNeuronsCount, SigmoidFunction sigmoid, bool isLastLayer = false)
        {
            this.Number = number;
            if (!isLastLayer)
            {
                neuronsCount++;
            }
            this.GenerateLayerNeurons(neuronsCount, previousLayerNeuronsCount, sigmoid, isLastLayer);
        }

        private void GenerateLayerNeurons(int neuronsCount, int previousLayerNeuronsCount, SigmoidFunction sigmoid, bool isLastLayer)
        {
            var maxIndex = neuronsCount;
            var isFirstLayer = this.Number == 1;

            if (!isLastLayer)
            {
                var biasNeuron = new Neuron(0, previousLayerNeuronsCount, sigmoid, true, isFirstLayer);
                this.Neurons.Add(biasNeuron);
                maxIndex--;
            }

            for (var i = 0; i < maxIndex; i++)
            {
                var neuron = new Neuron(i + 1, previousLayerNeuronsCount, sigmoid, false, isFirstLayer);
                this.Neurons.Add(neuron);
            }

        }

        public void CalculateLayerOutput(ObservableCollection<double> inputs)
        {
            foreach (var neuron in this.Neurons)
            {
                neuron.CalculateNeuronOutput(inputs);
                this.OutputValues.Add(neuron.Output);
                this.InputValues.Add(neuron.Input);
            }
        }

        public void CalculateLayerError(ObservableCollection<double> wantedValues)
        {
            
            for(int i = 0; i < this.Neurons.Count; i++)
            {
                this.Neurons[i].UpdateError(this.OutputValues[i], wantedValues[i]);
            }

        }

        public bool IsFirstLayer
        {
            get
            {
                return this.Number == 1;
            }
        }

		#region Properties
		private int _number;

        public int Number
        {
            get
            {
                return _number;
            }

            set
            {
                if (value == _number)
                {
                    return;
                }

                _number = value;
            }
        }

        private ObservableCollection<Neuron> _neurons = new ObservableCollection<Neuron>();

        public ObservableCollection<Neuron> Neurons
        {
            get
            {
                return _neurons;
            }

            set
            {
                if (value == _neurons)
                {
                    return;
                }
                _neurons = value;
            }
        }

        /**
         * List of output (y=f(net i)) values of each neuron of the layer. If first Layer y is xi
         * */
        public ObservableCollection<Double> _outputValues = new ObservableCollection<Double>();

        public ObservableCollection<Double> OutputValues
        {
            get
            {
                return _outputValues;
            }

            set
            {
                if (value == _outputValues)
                {
                    return;
                }

                _outputValues = value;
            }
        }

        /**
         * List of input values of the layer. If is the first layer the values are Xi, if is
         * another layer the values are net i
         * */
        public ObservableCollection<double> _inputValues = new ObservableCollection<double>();

        public ObservableCollection<double> InputValues
        {
            get
            {
                return _inputValues;
            }

            set
            {
                if (value == _inputValues)
                {
                    return;
                }

                _inputValues = value;
            }
        }

    }

	#endregion
}
