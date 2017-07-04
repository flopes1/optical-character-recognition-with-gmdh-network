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
using OCRFFNetwork.dataset.api.image;

namespace OCRFFNetwork
{
    public class StartNetwork
    {
        public static void Main(String[] args)
        {

            //var sourceDir = Network.Default.DatasetDirectory + "\\Base";
            //var outputDir = Network.Default.DatasetDirectory;

            //CharacterDatasetHandler.GenerateDatasetEntry(sourceDir, outputDir);

            var trainAlphabet = new AlphabetDataset(EnumDatasetType.Train);
            var validationAlphabet = new AlphabetDataset(EnumDatasetType.Validation);
            var testAlphabet = new AlphabetDataset(EnumDatasetType.Test);

            var cycles = DatasetUtils.BuildCyclesFromDataset(trainAlphabet, validationAlphabet, testAlphabet);


            var network = new MultiLayerNetwork(cycles);
            network.InitializeNetwork();

            ObservableCollection<string> lettersReturned;
            var likelyResults = "";


            Console.WriteLine("\nThe Network has initialized. The test of the Network will start\n");
            Console.ReadLine();

            var numberOfExamples = 0;
            var numberOfCorrectResults = 0;

            for (int i = 0; i < network.Cycles.Count; i++)
            {

                //network.Cycles[i].ExamplesTest = network.RandomizeList(network.Cycles[i].ExamplesTest);

                foreach (Example example in network.Cycles[i].ExamplesTest)
                {

                    var outputFromTrainedNetwork = network.CheckElement(example);
                    lettersReturned = DatasetUtils.GetLettersFromOutputArray(outputFromTrainedNetwork);

                    likelyResults = string.Join(" ", lettersReturned);

                    Console.WriteLine("Cycle: " + (i + 1) + " Expected result: " + example.Name + ". Likely results: " + likelyResults);

                    numberOfExamples++;

                    if (lettersReturned.Contains(example.Name))
                    {
                        numberOfCorrectResults++;
                    }

                    likelyResults = "";
                }
            }

            var result = ((float)numberOfCorrectResults / (float)numberOfExamples) * 100;

            Console.WriteLine("Accuraty rate of Network: " + result + "%");

            Console.ReadLine();
        }

        
    }
}
