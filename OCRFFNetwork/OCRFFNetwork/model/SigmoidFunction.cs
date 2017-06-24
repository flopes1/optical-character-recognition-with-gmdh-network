using System;
using System.Collections.Generic;
using System.Text;

namespace OCRFFNetwork.model
{

    public class SigmoidFunction
    {
		public double Calculate(double neti)
		{
			return (1 / (1 + Math.Pow(Math.E, -neti)));
		}

		public double CalculateAndDerivative(double neti)
		{
			double y = Calculate(neti);
			return (y * (1 - y));
		}

        public double CalculateDerivate(double yi)
        {
            return yi * (1 - yi);
        }
	}
}
