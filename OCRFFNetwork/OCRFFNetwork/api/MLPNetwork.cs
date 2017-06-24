using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OCRFFNetwork.model;
using System.Text;

namespace OCRFFNetwork.api
{
	// TODO
    public class MLPNetwork
    {
		private ObservableCollection<Layer> _layerList = new ObservableCollection<Layer>();

		public ObservableCollection<Layer> LayerList
		{
			get
			{
				return _layerList;
			}

			set
			{
				if (value != _layerList)
				{
					_layerList = value;
				}
			}
		}

		private double _learningRate;

		public double LearningRate
		{
			get
			{
				return _learningRate;
			}
			set
			{
				if (value != _learningRate)
				{
					_learningRate = value;
				}
			}
		}
	}
}
