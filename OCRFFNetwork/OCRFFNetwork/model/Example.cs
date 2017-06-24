using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRFFNetwork.model
{
    public class Example
    {

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if(_name == value)
                {
                    return;
                }

                _name = value;
            }
        }

        private ObservableCollection<double> _inputValues = new ObservableCollection<double>();

        public ObservableCollection<double> InputValues
        {
            get
            {
                return _inputValues;
            }
            set
            {
                if(_inputValues == value)
                {
                    return;
                }

                _inputValues = value;
            }
        }

        private ObservableCollection<double> _wantedValues = new ObservableCollection<double>();

        public ObservableCollection<double> WantedValues
        {
            get
            {
                return _wantedValues;
            }
            set
            {
                if (_wantedValues == value)
                {
                    return;
                }

                _wantedValues = value;
            }
        }

    }
}
