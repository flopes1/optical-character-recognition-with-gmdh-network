﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OCRFFNetwork.model;
using System.Text;
using System.Linq;

namespace OCRFFNetwork.model
{
    // TODO
    public class MultiLayerNetwork : INetwork
    {

        public MultiLayerNetwork(ObservableCollection<Cycle> cycles = null)
        {
            this.Cycles = cycles;
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


        public void TrainNetwork()
        {
            // Precisa fazer a condição de parada de treinamento não precisa treinar por todos os ciclos
            // se treina apenas enquanto o resultado não for bom. (validação cruzada)
            foreach (var cycle in this.Cycles)
            {
                foreach (var example in cycle.Examples)
                {
                    //Treina exemplos do ciclo
                    this.SaveCurrentWeights();

                    var exampleResult = this.ForwardStep(example);

                    //Se o resultado não for o esperado, precisa fazer a fase backward e atualizar os pesos
                    if (!this.CheckResult(exampleResult, example.WantedValues))
                    {
                        this.SetOutputLayerError(example.WantedValues);
                        this.BackwardSted(example);
                        this.UpdateNetworkWeights();
                    }
                }
            }
        }

        private void SetOutputLayerError(ObservableCollection<double> wantedValues)
        {
            //Calcula o erros dos neuronios da camada de saída apenas
            // Na fase backward tem que fazer o mesmo para as camadas intermediarias
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

        public void UpdateNetworkWeights()
        {
            throw new NotImplementedException();
        }

        /**
         *  Metodo que vai usar a rede já treinada e testar o resultado para o exemplo passado
         * 
         * */
        public bool CheckElement(Example elemtToTest)
        {
            throw new NotImplementedException();
        }

        public void SaveCurrentWeights()
        {
            //Pode salvar em txt mesmo separando os pesos por ponto e virgula
            //Salva no pacote dataset, cria uma pasta weigths
            throw new NotImplementedException();
        }

        public void BackwardSted(Example currentExample)
        {
            //TODO ...

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

        #endregion // Properties
    }
}