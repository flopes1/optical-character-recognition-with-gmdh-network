using System;
using System.Collections.Generic;
using System.Text;

namespace OCRFFNetwork.model
{
	//Essa classe representa a função de ativação Sigmóide Logística.
    public class SigmoidFunction
    {
		public double Calculate(double x)
		{
			return (1 / (1 + Math.Pow(Math.E, -x)));
		}

		public double CalculateDerivative(double x)
		{
			double y = Calculate(x);
			return (y * (1 - y));
		}
	}
}
