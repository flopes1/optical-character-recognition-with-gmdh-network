using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OCRFFNetwork.model
{
    public class Layer
    {
		protected Layer (int neuronsCount, int inputsCount, SigmoidFunction sigmoid)
		{
			for (int i = 0; i < neuronsCount; i++)
				_neuronList[i] = new Neuron(inputsCount, sigmoid);
		}

		public ObservableCollection<Double> Compute(double[] input)
		{
			// compute each neuron
			for (int i = 0; i < _neuronList.Count; i++)
				_outputValues[i] = _neuronList[i].Compute(input);

			return _outputValues;
		}


		private ObservableCollection<Neuron> _neuronList = new ObservableCollection<Neuron>();

		public ObservableCollection<Neuron> NeuronList
		{
			get
			{
				return _neuronList;
			}

			set
			{
				if (value != _neuronList)
				{
					_neuronList = value;
				}
			}
		}

		public ObservableCollection<Double> _outputValues = new ObservableCollection<Double>();

		public ObservableCollection<Double> OutputValues
		{
			get
			{
				return _outputValues;
			}
		}
	}
}
