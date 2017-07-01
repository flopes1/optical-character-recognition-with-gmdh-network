using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCRFFNetwork.model;
using OCRFFNetwork.dataset;
using OCRFFNetwork.api.Utils;

namespace OCRFFNetwork
{
    public class StartNetwork
    {
        public static void Main(String[] args)
        {

            var trainAlphabet = new AlphabetDataset(EnumDatasetType.Train);
            var validationAlphabet = new AlphabetDataset(EnumDatasetType.Validation);
            var testAlphabet = new AlphabetDataset(EnumDatasetType.Test);

            var cyclesToTrain = DatasetUtils.BuildCyclesFromDataset(trainAlphabet);
			var cyclesToValidate = DatasetUtils.BuildCyclesFromDataset(validationAlphabet);


            var network = new MultiLayerNetwork(cyclesToTrain);
			var validationNetwork = new MultiLayerNetwork(cyclesToTrain, cyclesToValidate);

			validationNetwork.TrainNetwork();
			network.TrainNetwork();


			//var input = "C:\\Users\\Filipe Lopes\\Desktop\\8º Periodo\\RNA\\Dataset\\Maiusculas";
			//var output = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\dataset";

			//var value = Network.Default.DatasetDirectory;
			//var lettes = Directory.GetDirectories(value);

			//CharacterDatasetHandler.GenerateDatasetEntry(input, output);

			//var dataSet = new AlphabetDataset(EnumDatasetType.Train);
			//dataSet.Alphabet.FirstOrDefault().GetImagePixels(0);

			//var example = new Example()
			//{
			//	Name = "A",
			//	InputValues = new ObservableCollection<double>(new double[] { 1, 0 }),
			//	WantedValues = new ObservableCollection<double>(new double[] { 0, 0 })
			//};


			//var inputLayer = new Layer(1, 2, 0, new SigmoidFunction());
			//var hidLayer = new Layer(2, 3, inputLayer.Neurons.Count, new SigmoidFunction());
			//var outputLayer = new Layer(3, 2, hidLayer.Neurons.Count, new SigmoidFunction(), true);

			//var netWorkLayers = new ObservableCollection<Layer>();
			//netWorkLayers.Add(inputLayer);
			//netWorkLayers.Add(hidLayer);
			//netWorkLayers.Add(outputLayer);

			//var previousLayerOutput = new ObservableCollection<double>();

			//foreach (var layer in netWorkLayers)
			//{
			//    if (layer.IsFirstLayer)
			//    {
			//        layer.CalculateLayerOutput(example.InputValues);
			//    }
			//    else
			//    {
			//        layer.CalculateLayerOutput(previousLayerOutput);
			//    }
			//    previousLayerOutput = layer.OutputValues;
			//}

		}

        

    }
}
