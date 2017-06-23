using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OCRFFNetwork.model
{
    public class Layer
    {
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
	}
}
