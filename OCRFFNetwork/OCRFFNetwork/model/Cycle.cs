using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRFFNetwork.model
{
    public class Cycle
    {

        public Cycle()
        {
            this.ExamplesTrain = new ObservableCollection<Example>();
            this.ExamplesValidation = new ObservableCollection<Example>();
            this.ExamplesTest = new ObservableCollection<Example>();
        }

        public ObservableCollection<Example> ExamplesTrain { get; set; }

        public ObservableCollection<Example> ExamplesValidation { get; set; }

        public ObservableCollection<Example> ExamplesTest { get; set; }

    }
}
