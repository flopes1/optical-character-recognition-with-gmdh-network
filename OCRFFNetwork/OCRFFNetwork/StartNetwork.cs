﻿using System;
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

            var cycles = DatasetUtils.BuildCyclesFromDataset(trainAlphabet, validationAlphabet, testAlphabet);


            var network = new MultiLayerNetwork(cycles);
			//network.TrainNetwork();
			network.InitializeNetwork();

			var letterReturned = "";
			var matchString = "";

			for (int i = 0; i < network.Cycles.Count; i++)
			{
				foreach (Example example in network.Cycles[i].ExamplesTest)
				{
					var outputFromTrainedNetwork = network.CheckElement(example);
					letterReturned = DatasetUtils.GetLetterFromOutputArray(outputFromTrainedNetwork);

					matchString = example.Name == letterReturned ? " ACERTOU" : " ERROU";

					Console.WriteLine("Expected result: " + example.Name + ". --- Obtained result: " + letterReturned + matchString);
				}
			}


		}
	}
}
