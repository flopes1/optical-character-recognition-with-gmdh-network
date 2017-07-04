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
            this.MeanSquareErrorsFromCycles = new ObservableCollection<double>();
            this.LoadNetworkConfigurations();
            this.BuildNetwork();
        }

        #region Internal Methods

        private void BuildNetwork()
        {

            if (this.Cycles != null)
            {
                var exampleModel = this.Cycles.FirstOrDefault().ExamplesTrain.FirstOrDefault();

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


        public void InitializeNetwork()
        {

            if (this.IsNetworkTrained())
            {
                Console.WriteLine("The network was already trained. The saved weights will be loaded");
                this.LoadNetWorkWeights();
            }
            else
            {
                this.TrainNetwork();
            }

        }

        private void LoadNetWorkWeights()
        {

            var hiddenLayerWeights = Network.Default.WeightsHidden;
            var outputLayerWeights = Network.Default.WeightsOutput;

            var hiddentWeights = File.ReadAllText(hiddenLayerWeights);
            hiddentWeights = hiddentWeights.Remove(hiddentWeights.LastIndexOf(';'));

            var splitedHiddenWeights = hiddentWeights.Split(';');
            hiddentWeights = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            this.PopulateNetWorkWeights(Array.ConvertAll(splitedHiddenWeights, Double.Parse), 2);

            splitedHiddenWeights = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();


            var outputWeights = File.ReadAllText(outputLayerWeights);
            outputWeights = outputWeights.Remove(outputWeights.LastIndexOf(';'));

            var splitedOutputWeights = outputWeights.Split(';');
            outputWeights = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            this.PopulateNetWorkWeights(Array.ConvertAll(splitedOutputWeights, Double.Parse), 3);
            splitedOutputWeights = null;

        }

        private void PopulateNetWorkWeights(double[] weights, int layer)
        {
            var layerToPopulate = this.Layers.Where(l => l.Number == layer).FirstOrDefault();

            Console.WriteLine("Loading weights for layer: " + layerToPopulate.Number);

            if (layerToPopulate != null)
            {
                var index = 0;
                foreach (var neuron in layerToPopulate.Neurons)
                {
                    for (var i = 0; i < neuron.Weights.Length; i++)
                    {
                        neuron.Weights[i] = weights[index];
                        index++;

                    }
                }
            }
        }

        private bool IsNetworkTrained()
        {
            var hiddenLayerWeights = Network.Default.WeightsHidden;
            var outputLayerWeights = Network.Default.WeightsOutput;

            return File.Exists(hiddenLayerWeights) && File.Exists(outputLayerWeights);
        }

        public void TrainNetwork()
        {
            double meanSquareErrorFromValidation = 0;

            for (int i = 0; i < this.Cycles.Count; i++)
            {
                double sumTrain = 0, sumValidate = 0, countWantedValues = 0;

                this.Cycles[i].ExamplesTrain = this.RandomizeList(this.Cycles[i].ExamplesTrain);

                //Treinamento
                foreach (Example example in this.Cycles[i].ExamplesTrain)
                {
                    Console.WriteLine("\nTraining example: " + example.Name + " in cycle: " + (i + 1));

                    Console.WriteLine("Executing forward");
                    var exampleResult = this.ForwardStep(example);

                    var normalizedResults = DatasetUtils.GetLettersFromOutputArray(exampleResult);

                    Console.WriteLine("Results from forward: " + string.Join(" ", normalizedResults) + " -- Max Result: " + exampleResult.Max());
                    countWantedValues = example.WantedValues.Count;

                    //Se o resultado não for o esperado, precisa fazer a fase backward e atualizar os pesos
                    if (!this.CheckResult(exampleResult, example.WantedValues))
                    {
                        Console.WriteLine("Executing backward and updating weights");

                        this.SetOutputLayerError(example.WantedValues);
                        this.BackwardStep(example);
                        //O reajuste dos pesos está no final da backwardStep
                    }

                    for (int j = 0; j < countWantedValues; j++)
                    {
                        sumTrain += Math.Pow(example.WantedValues[j] - exampleResult[j], 2);
                    }
                }


                Console.WriteLine("\nCalculating EMQ members");

                //Adicionando EMQ do treinamento
                this.MeanSquareErrorsFromCycles.Add(sumTrain / Math.Pow(countWantedValues, 2));

                Console.WriteLine("EMQ Train: " + this.MeanSquareErrorsFromCycles.LastOrDefault());

                //Validação
                if (this.Cycles[i].ExamplesValidation.Count > 0)
                {

                    Console.WriteLine("Calculating results for validation cycle: " + (i + 1));

                    this.Cycles[i].ExamplesValidation = this.RandomizeList(this.Cycles[i].ExamplesValidation);

                    foreach (Example example in this.Cycles[i].ExamplesValidation)
                    {
                        var exampleResult = this.ForwardStep(example);
                        countWantedValues = example.WantedValues.Count;

                        for (int j = 0; j < countWantedValues; j++)
                        {
                            sumValidate += Math.Pow(example.WantedValues[j] - exampleResult[j], 2);
                        }
                    }

                    //EMQ do ciclo de validação.
                    meanSquareErrorFromValidation = (sumValidate / Math.Pow(countWantedValues, 2));

                    Console.WriteLine("EMQ validation:" + meanSquareErrorFromValidation);

                    //O EMQ na validação cruzada foi maior que o calculado. O treinamento deve parar.
                    if (i > Network.Default.IgnoreValidationNumber && meanSquareErrorFromValidation >= this.MeanSquareErrorsFromCycles[i])
                    {
                        Console.WriteLine("Finishing training cause cross validation method in cycle: " + (i + 1));
                        break;
                    }
                }
                else
                {
                    if (i > 0 && (this.MeanSquareErrorsFromCycles[i - 1] <= Network.Default.AcceptanceRatio * this.MeanSquareErrorsFromCycles[i]))
                    {
                        Console.WriteLine("Finishing training cause EMQ method in cycle: " + (i + 1));
                        break;
                    }
                }

            }
            this.SaveCurrentWeights();
        }

        public ObservableCollection<Example> RandomizeList(ObservableCollection<Example> examplesTrain)
        {
            var r = new Random();
            return new ObservableCollection<Example>(examplesTrain.OrderBy(k => r.Next()).ToArray());
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
                var acceptableVale = wantedValues[i];
                var calculedResult = exampleResult[i];

                var theshhold = Network.Default.Theshhold;

                if (!(calculedResult >= (acceptableVale * (1 - theshhold)) && calculedResult <= (acceptableVale * (1 + theshhold))))
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
            Layer firstLayer = this.Layers.FirstOrDefault();
            var hiddenLayer = this.Layers.Where(l => l.Number == 2).FirstOrDefault();
            Layer lastLayer = this.Layers.LastOrDefault();

            //Reajuste dos pesos que ligam à camada de saída para a camada escondida.
            for (int i = 0; i < lastLayer.Neurons.Count; i++)
            {
                for (int j = 0; j < lastLayer.Neurons.FirstOrDefault().Weights.Length; j++)
                {
                    lastLayer.Neurons[i].Weights[j] = lastLayer.Neurons[i].Weights[j] + this.LearningRate * sensibilitiesOfOutputLayer[i] * hiddenLayer.Neurons[j].Output;
                }
            }

            //Reajuste dos pesos que ligam à camada escondida para a camada de entrada.
            for (int i = 0; i < hiddenLayer.Neurons.Count; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                for (int j = 0; j < hiddenLayer.Neurons.FirstOrDefault().Weights.Length; j++)
                {
                    hiddenLayer.Neurons[i].Weights[j] = hiddenLayer.Neurons[i].Weights[j] + this.LearningRate * sensibilitiesOfHiddenLayer[i - 1] * firstLayer.Neurons[j].Input;
                }

            }

        }

        /**
         *  Metodo que vai usar a rede já treinada e testar o resultado para o exemplo passado
         * */
        public ObservableCollection<double> CheckElement(Example elemtToTest)
        {
            return this.ForwardStep(elemtToTest);
        }

        public void SaveCurrentWeights()
        {
            var filePathHiddenLayer = Network.Default.WeightsHidden;
            var filePathOutputLayer = Network.Default.WeightsOutput;


            if (File.Exists(filePathHiddenLayer))
            {
                File.Delete(filePathHiddenLayer);
            }

            if (File.Exists(filePathOutputLayer))
            {
                File.Delete(filePathOutputLayer);
            }

            var writerHidden = File.CreateText(filePathHiddenLayer);
            var writerOutput = File.CreateText(filePathOutputLayer);

            foreach (Layer layer in this.Layers)
            {
                if (!layer.IsFirstLayer)
                {
                    var writer = layer.Number == 2 ? writerHidden : writerOutput;

                    foreach (Neuron neuron in layer.Neurons)
                    {
                        foreach (var weight in neuron.Weights)
                        {
                            writer.Write(weight.ToString("0.#####") + ";");
                        }
                    }
                }
            }

            writerHidden.Close();
            writerOutput.Close();
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
            var hiddenLayer = this.Layers.Where(l => l.Number == 2).FirstOrDefault();

            double[] sensibilitiesOfHiddenLayer = new double[hiddenLayer.Neurons.Count - 1];

            //obtendo a sensibilidade dos neurônios da camada escondida
            // f¹'(net¹) * Somatorio ((W²ij).d²i )
            double sum = 0;

            for (int i = 0; i < hiddenLayer.Neurons.Count; i++)
            {
                //Não há sensibilidade para o neurônio BIAS.
                if (i != 0)
                {
                    for (int j = 0; j < lastLayer.Neurons.Count; j++)
                    {
                        sum += lastLayer.Neurons[j].Weights[i] * sensibilitiesOfOutputLayer[j];
                    }
                    sensibilitiesOfHiddenLayer[i - 1] = hiddenLayer.Neurons[i].ActivationFunction.CalculateDerivate(hiddenLayer.Neurons[i].Output) * sum;
                    sum = 0;
                }
            }

            //Agora, atualizar os pesos.
            this.UpdateNetworkWeights(sensibilitiesOfHiddenLayer, sensibilitiesOfOutputLayer);
            //fim da gambiarra
        }

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
