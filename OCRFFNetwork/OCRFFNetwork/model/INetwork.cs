﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRFFNetwork.model
{
    public interface INetwork
    {
        ObservableCollection<double> ForwardStep(Example currentExample);
        void TrainNetwork(ObservableCollection<double> meanSquaredErrors);
        void UpdateNetworkWeights(double[] sensibilitiesOfHiddenLayer, double[] sensibilitiesOfOutputLayer);
        bool CheckElement(Example elemtToTest);
        void SaveCurrentWeights();
        void BackwardStep(Example currentExample);
    }
}
