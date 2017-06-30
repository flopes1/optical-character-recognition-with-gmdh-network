using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OCRFFNetwork.model;
using System.Text;
using System.Linq;
using System.IO;
using OCRFFNetwork.api.Utils;

namespace OCRFFNetwork.model
{
	// TODO
	public class MultiLayerNetwork : INetwork
	{

		public MultiLayerNetwork(ObservableCollection<Cycle> cycles = null)
		{
			this.Cycles = cycles;
			//this.MeanSquareErrosFromCycles = new double[this.Cycles.Count];
			this.MeanSquareErrorsFromCycles = new ObservableCollection<double>();
			this.LoadNetworkConfigurations();
			this.BuildNetwork();
		}

		#region Internal Methods

		private void BuildNetwork()
		{

			if (this.Cycles != null)
			{
				var exampleModel = this.Cycles.FirstOrDefault().Examples.FirstOrDefault();

				for (int i = 0; i < Network.Default.NumberOfLayers; i++)
				{
					var previousLayerNeuronsCount = i > 0 ? this.Layers[i - 1].Neurons.Count : 0;
					var isLastLayer = i == Network.Default.NumberOfLayers - 1;
					var numberOfNeurons = isLastLayer ? Network.Default.LettersInAlphabet : exampleModel.InputValues.Count + i;

					var layer = new Layer(i + 1, numberOfNeurons, previousLayerNeuronsCount, new SigmoidFunction(), isLastLayer);
					this.Layers.Add(layer);
				}

			}
		}


		public void TrainNetwork(ObservableCollection<double> meanSquaredErrors)
		{
			for (int i = 0; i < this.Cycles.Count; i++)
			{
				//Somatório das diferenças entre calculados e desejados.
				double sum = 0, countWantedValues = 0;

				foreach (var example in this.Cycles[i].Examples)
				{
					//Treina exemplos do ciclo

					var exampleResult = this.ForwardStep(example);
					countWantedValues = example.WantedValues.Count;

					//Se o resultado não for o esperado, precisa fazer a fase backward e atualizar os pesos
					if (!this.CheckResult(exampleResult, example.WantedValues))
					{
						this.SetOutputLayerError(example.WantedValues);
						this.BackwardStep(example);
						//O reajuste dos pesos está no final da backwardStep
						this.SaveCurrentWeights();
					}

					for (int j = 0; j < exampleResult.Count; j++)
					{
						sum += Math.Pow(exampleResult[j] - example.WantedValues[j], 2);
					}

				}

				//Calculando o EMQ - Erro médio quadrático.
				this.MeanSquareErrorsFromCycles.Add(sum / countWantedValues);

				if (meanSquaredErrors != null)
				{
					if (meanSquaredErrors[i] > this.MeanSquareErrorsFromCycles[i])
					{
						//O EMQ na validação cruzada foi maior que o calculado. O treinamento deve parar.
						break;
					} 
				}
			}

		}

		private void SetOutputLayerError(ObservableCollection<double> wantedValues)
		{
			//Calcula o erros dos neuronios da camada de saída
			this.Layers.LastOrDefault().CalculateLayerError(wantedValues);
		}

		private bool CheckResult(ObservableCollection<double> exampleResult, ObservableCollection<double> wantedValues)
		{
			for (int i = 0; i < wantedValues.Count; i++)
			{
				if (wantedValues[i] != exampleResult[i])
				{
					return false;
				}
			}

			return true;
		}

		public ObservableCollection<double> ForwardStep(Example currentExample)
		{
			var previousLayerOutput = new ObservableCollection<double>();

			foreach (var layer in this.Layers)
			{
				if (layer.IsFirstLayer)
				{
					layer.CalculateLayerOutput(currentExample.InputValues);
				}
				else
				{
					layer.CalculateLayerOutput(previousLayerOutput);
				}
				previousLayerOutput = layer.OutputValues;
			}

			return previousLayerOutput;
		}

		public void UpdateNetworkWeights(double[] sensibilitiesOfHiddenLayer, double[] sensibilitiesOfOutputLayer)
		{
			//Reajuste dos pesos que ligam à camada de saída para a camada escondida.

			//gambiarra
			Layer hiddenLayer;
			foreach (Layer layer in this.Layers)
			{
				if (layer.Number == 2)
				{
					hiddenLayer = layer;

					for (int i = 0; i < hiddenLayer.Neurons.Count; i++)
					{
						for (int j = 0; j < sensibilitiesOfOutputLayer.Length; j++)
						{
							hiddenLayer.Neurons[i].Weights[j] = hiddenLayer.Neurons[i].Weights[j] + this.LearningRate * sensibilitiesOfOutputLayer[j] * hiddenLayer.Neurons[i].Output;
						}
					}
				}
			}
			//fim da gambiarra


			//Reajuste dos pesos que ligam à camada escondida para a camada de entrada.
			Layer firstLayer = this.Layers.FirstOrDefault();

			/* ##############################Pesos estão nulos para todos os neurônios!!!!########################################
			 * ##############################Pesos estão nulos para todos os neurônios!!!!########################################
			 * ##############################Pesos estão nulos para todos os neurônios!!!!########################################
			 * Pesos estão nulos para todos os neurônios!!!! Pesos estão nulos para todos os neurônios!!!!
			 * firstLayer.Neurons[i].Weights[j] nulo.
			 * ##############################Pesos estão nulos para todos os neurônios!!!!########################################
			 * ##############################Pesos estão nulos para todos os neurônios!!!!########################################
			 * ##############################Pesos estão nulos para todos os neurônios!!!!########################################
			 */

			for (int i = 0; i < firstLayer.Neurons.Count; i++)
			{
				for (int j = 0; j < sensibilitiesOfHiddenLayer.Length; j++)
				{
					firstLayer.Neurons[i].Weights[j] = firstLayer.Neurons[i].Weights[j] + this.LearningRate * sensibilitiesOfHiddenLayer[j] * firstLayer.Neurons[i].Input;
				}
			}

			//foreach (Neuron neuron in firstLayer.Neurons)
			//{
			//	for (int i = 0; i < firstLayer.Neurons.Count; i++)
			//	{
			//		for (int j = 0; j < firstLayer.Neurons[i].Weights.Length; j++)
			//		{
			//			if (firstLayer.Neurons[i].Input != 0)
			//			{
			//				firstLayer.Neurons[i].Weights[j] = firstLayer.Neurons[i].Weights[j] + this.LearningRate * sensibilitiesOfHiddenLayer[j] * firstLayer.Neurons[i].Input;
			//			}
			//		}
			//	}
			//}
		}

		/**
         *  Metodo que vai usar a rede já treinada e testar o resultado para o exemplo passado
         * */
		public bool CheckElement(Example elemtToTest)
		{
			throw new NotImplementedException();
		}

		public void SaveCurrentWeights()
		{
			//Pode salvar em txt mesmo separando os pesos por ponto e virgula
			//Salva no pacote dataset, cria uma pasta weigths

			string path = @"../weights/weightsSaved.txt";

			//Cria o arquivo se este nao existir (no caso de não querer dar append, e realmente sobrescrever, retirar o parâmetro true.
			TextWriter tw = new StreamWriter(path, true);

			foreach (Layer layer in this._layers)
			{
				tw.WriteLine("Layer: ");
				foreach (Neuron neuron in layer.Neurons)
				{
					tw.WriteLine($"Neuron {neuron.Index}:");
					foreach (var weight in neuron.Weights)
					{
						tw.WriteLine($"{weight};");
					}
				}
			}
		}

		public void BackwardStep(Example currentExample)
		{
			//A fase backword inicia com o cálculo da sensibilidade para os neurônios da camada de saída.
			//sensibilidade = derivada da função de ativação vezes o erro.

			Layer lastLayer = this.Layers.LastOrDefault();
			double[] outputErrors = new double[lastLayer.Neurons.Count];
			double[] sensibilitiesOfOutputLayer = new double[lastLayer.Neurons.Count];

			//Obtendo a sensibilidade dos neurônios da camada de saída
			for (int i = 0; i < lastLayer.Neurons.Count; i++)
			{
				outputErrors[i] = lastLayer.Neurons[i].Error;
				sensibilitiesOfOutputLayer[i] = outputErrors[i] * lastLayer.Neurons[i].ActivationFunction.CalculateDerivate(lastLayer.Neurons[i].Output);
			}

			//gambiarra
			Layer hiddenLayer;
			foreach (Layer layer in this.Layers)
			{
				if (layer.Number == 2)
				{
					hiddenLayer = layer;
					double[] sensibilitiesOfHiddenLayer = new double[hiddenLayer.Neurons.Count];

					//obtendo a sensibilidade dos neurônios da camada escondida
					// f¹'(net¹) * Somatorio ((W²ij).d²i )
					double sum = 0;

					for (int i = 0; i < hiddenLayer.Neurons.Count; i++)
					{
						//hiddenLayer.Neurons[i].Weights.Length
						for (int j = 0; j < lastLayer.Neurons.Count; j++)
						{
							sum += hiddenLayer.Neurons[i].Weights[j] * sensibilitiesOfOutputLayer[j];
						}
						sensibilitiesOfHiddenLayer[i] = hiddenLayer.Neurons[i].ActivationFunction.CalculateDerivate(hiddenLayer.Neurons[i].Output) * sum;
						sum = 0;
					}

					//Agora, atualizar os pesos.
					this.UpdateNetworkWeights(sensibilitiesOfHiddenLayer, sensibilitiesOfOutputLayer);
				}
			}
			//fim da gambiarra

		}

		//TODO adicionar configurações do calculo da sensibilidade
		private void LoadNetworkConfigurations()
		{
			this.LearningRate = Network.Default.LearningRate;
		}

		#endregion //Internal Methods

		#region Properties

		private ObservableCollection<Example> _examples = new ObservableCollection<Example>();

		//public ObservableCollection<Example> Examples
		//{
		//    get
		//    {
		//        return _examples;
		//    }
		//    set
		//    {
		//        if (value == _examples)
		//        {
		//            return;
		//        }

		//        _examples = value;
		//    }
		//}

		private ObservableCollection<Cycle> _cycles = new ObservableCollection<Cycle>();

		public ObservableCollection<Cycle> Cycles
		{
			get
			{
				return _cycles;
			}
			set
			{
				if (value == _cycles)
				{
					return;
				}

				_cycles = value;
			}
		}

		private ObservableCollection<Layer> _layers = new ObservableCollection<Layer>();

		public ObservableCollection<Layer> Layers
		{
			get
			{
				return _layers;
			}

			set
			{
				if (value != _layers)
				{
					_layers = value;
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

		private ObservableCollection<double> _meanSquareErrorsFromCycles;

		public ObservableCollection<double> MeanSquareErrorsFromCycles
		{
			get
			{
				return _meanSquareErrorsFromCycles;
			}
			set
			{
				if (value != _meanSquareErrorsFromCycles)
				{
					_meanSquareErrorsFromCycles = value;
				}
			}
		}

		#endregion // Properties
	}
}
