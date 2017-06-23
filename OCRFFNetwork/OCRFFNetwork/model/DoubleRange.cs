using System;
using System.Collections.Generic;
using System.Text;

namespace OCRFFNetwork.model
{
	public class DoubleRange
	{
		private double min, max;

		public double Min
		{
			get
			{
				return min;
			}

			set
			{
				if (value != min)
				{
					min = value;
				}
			}
		}

		public double Max
		{
			get
			{
				return max;
			}

			set
			{
				if (value != max)
				{
					max = value;
				}
			}
		}

		public double Length
		{
			get { return max - min; }
		}

		public DoubleRange(double min, double max)
		{
			this.min = min;
			this.max = max;
		}
	}
}
