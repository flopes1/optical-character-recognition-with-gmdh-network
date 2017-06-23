using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OCRFFNetwork.model
{
	public class Neuron
	{
		public Neuron(int inputs, SigmoidFunction function, ObservableCollection<Double> weightList)
		{
			_activationFunction = function;
			_inputsCount = Math.Max(1, inputs);
			_weightList = weightList;
			this.Randomize();
		}

		public void Randomize()
		{
			double d = range.Length;

			// randomize weights
			for (int i = 0; i < _inputsCount; i++)
				_weightList[i] = rand.NextDouble() * d + range.Min;
		}

		public double Compute(double[] input)
		{
			// check for corrent input vector
			if (input.Length != _inputsCount)
				throw new ArgumentException();

			// initial sum value
			double sum = 0.0;

			// compute weighted sum of inputs
			for (int i = 0; i < _inputsCount; i++)
			{
				sum += _weightList[i] * input[i];
			}

			sum += _threshold;

			// _outputValue = w10 * x0 + w11 * x1 etc...
			return (_outputValue = _activationFunction.Calculate(sum));
		}

		#region Properties

		private ObservableCollection<Double> _weightList = new ObservableCollection<Double>();

		public ObservableCollection<Double> WeightList
		{
			get
			{
				return _weightList;
			}

			set
			{
				if (value != _weightList)
				{
					_weightList = value;
				}

			}
		}

		private double _inputValue;

		public double InputValue
		{
			get
			{
				return _inputValue;
			}

			set
			{
				if (value != _inputValue)
				{
					_inputValue = value;
				}
			}
		}

		private double _outputValue;

		public double OutputValue
		{
			get
			{
				return _outputValue;
			}

			set
			{
				if (value != _outputValue)
				{
					_inputValue = value;
				}
			}
		}

		private int _inputsCount;

		public int InputsCount
		{
			get
			{
				return _inputsCount;
			}

			set
			{
				if (value != _inputsCount)
				{
					_inputsCount = value;
				}
			}
		}

		protected double _threshold = 0.0f;

		public double Threshold
		{
			get
			{
				return _threshold;
			}

			set
			{
				if (value != _threshold)
				{
					_threshold = value;
				}
			}
		}

		protected SigmoidFunction _activationFunction;

		public SigmoidFunction ActivationFunction
		{
			get
			{
				return _activationFunction;
			}

		}

		protected static Random rand = new Random((int)DateTime.Now.Ticks);

		public static Random Rand
		{
			get
			{
				return rand;
			}

			set
			{
				if (value != rand)
				{
					rand = value;
				}
			}
		}

		protected static DoubleRange range = new DoubleRange(-1.0, 1.0);

		public static DoubleRange Range
		{
			get
			{
				return range;
			}

			set
			{
				if (value != range)
				{
					range = value;
				}
			}
		}

		#endregion


	}
}
