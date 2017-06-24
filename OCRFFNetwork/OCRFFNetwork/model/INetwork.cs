using System;
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
        void TrainNetwork();
        void UpdateNetworkWeights();
        bool CheckElement(Example elemtToTest);
        void SaveCurrentWeights();
        void BackwardSted(Example currentExample);
    }
}
