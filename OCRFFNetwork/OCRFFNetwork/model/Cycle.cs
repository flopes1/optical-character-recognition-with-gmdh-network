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
            this.Examples = new ObservableCollection<Example>();
        }

        public ObservableCollection<Example> Examples { get; set; }
    }
}
