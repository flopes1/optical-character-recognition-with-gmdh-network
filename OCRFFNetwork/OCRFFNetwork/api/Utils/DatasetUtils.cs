using OCRFFNetwork.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRFFNetwork.api.Utils
{
	public class DatasetUtils
	{

		public static ObservableCollection<Cycle> BuildCyclesFromDataset(AlphabetDataset trainAlphabet, AlphabetDataset validationAlphabet, AlphabetDataset testAlphabet)
		{
			var cycles = new ObservableCollection<Cycle>();

			var imagesPerLetterTrain = trainAlphabet.Letters.FirstOrDefault().ImagesPath.Count;
			var imagesPerLetterValidation = validationAlphabet.Letters.FirstOrDefault().ImagesPath.Count;
			var imagesPerLetterTest = testAlphabet.Letters.FirstOrDefault().ImagesPath.Count;

			var letterCount = trainAlphabet.Letters.Count;

			for (int i = 0; i < imagesPerLetterTrain; i++)
			{
				var cycle = new Cycle();

				for (int j = 0; j < letterCount; j++)
				{
					var letter = trainAlphabet.Letters[j];

					var example = new Example()
					{
						Name = letter.Name,
						InputValues = letter.GetImagePixels(i),
						WantedValues = GetLetterWantedValues(letter.Name)
					};

					cycle.ExamplesTrain.Add(example);

					if (i < imagesPerLetterValidation && i < imagesPerLetterTest)
					{
						var letterValidation = validationAlphabet.Letters[j];

						var exampleValidation = new Example()
						{
							Name = letterValidation.Name,
							InputValues = letterValidation.GetImagePixels(i),
							WantedValues = GetLetterWantedValues(letterValidation.Name)
						};

						cycle.ExamplesValidation.Add(exampleValidation);

						var letterTest = testAlphabet.Letters[j];

						var exampleTest = new Example()
						{
							Name = letterTest.Name,
							InputValues = letterTest.GetImagePixels(i),
							WantedValues = GetLetterWantedValues(letterTest.Name)
						};

						cycle.ExamplesTest.Add(exampleTest);

					}

				}
				cycles.Add(cycle);
			}

			return cycles;
		}

		public static string GetLetterFromArray(ObservableCollection<double> array)
		{
			string letter = "";

			for (int i = 0; i < array.Count; i++)
			{
				if (array[i] == array.Max())
				{
					switch (i)
					{
						case 0:
							letter = "A";
							break;
						case 1:
							letter = "B";
							break;
						case 2:
							letter = "C";
							break;
						case 3:
							letter = "D";
							break;
						case 4:
							letter = "E";
							break;
						case 5:
							letter = "F";
							break;
						case 6:
							letter = "G";
							break;
						case 7:
							letter = "H";
							break;
						case 8:
							letter = "I";
							break;
						case 9:
							letter = "J";
							break;
						case 10:
							letter = "K";
							break;
						case 11:
							letter = "L";
							break;
						case 12:
							letter = "M";
							break;
						case 13:
							letter = "N";
							break;
						case 14:
							letter = "O";
							break;
						case 15:
							letter = "P";
							break;
						case 16:
							letter = "Q";
							break;
						case 17:
							letter = "R";
							break;
						case 18:
							letter = "S";
							break;
						case 19:
							letter = "T";
							break;
						case 20:
							letter = "U";
							break;
						case 21:
							letter = "V";
							break;
						case 22:
							letter = "W";
							break;
						case 23:
							letter = "X";
							break;
						case 24:
							letter = "Y";
							break;
						case 25:
							letter = "Z";
							break;
						default:
							break;
					}

				}

			}

			return letter;
		}

		private static ObservableCollection<double> GetLetterWantedValues(string letter)
		{
			var values = new ObservableCollection<double>();

			switch (letter)
			{
				case "A":
					values = GetLetterArrayByIndex(0);
					break;
				case "B":
					values = GetLetterArrayByIndex(1);
					break;
				case "C":
					values = GetLetterArrayByIndex(2);
					break;
				case "D":
					values = GetLetterArrayByIndex(3);
					break;
				case "E":
					values = GetLetterArrayByIndex(4);
					break;
				case "F":
					values = GetLetterArrayByIndex(5);
					break;
				case "G":
					values = GetLetterArrayByIndex(6);
					break;
				case "H":
					values = GetLetterArrayByIndex(7);
					break;
				case "I":
					values = GetLetterArrayByIndex(8);
					break;
				case "J":
					values = GetLetterArrayByIndex(9);
					break;
				case "K":
					values = GetLetterArrayByIndex(10);
					break;
				case "L":
					values = GetLetterArrayByIndex(11);
					break;
				case "M":
					values = GetLetterArrayByIndex(12);
					break;
				case "N":
					values = GetLetterArrayByIndex(13);
					break;
				case "O":
					values = GetLetterArrayByIndex(14);
					break;
				case "P":
					values = GetLetterArrayByIndex(15);
					break;
				case "Q":
					values = GetLetterArrayByIndex(16);
					break;
				case "R":
					values = GetLetterArrayByIndex(17);
					break;
				case "S":
					values = GetLetterArrayByIndex(18);
					break;
				case "T":
					values = GetLetterArrayByIndex(19);
					break;
				case "U":
					values = GetLetterArrayByIndex(20);
					break;
				case "V":
					values = GetLetterArrayByIndex(21);
					break;
				case "W":
					values = GetLetterArrayByIndex(22);
					break;
				case "X":
					values = GetLetterArrayByIndex(23);
					break;
				case "Y":
					values = GetLetterArrayByIndex(24);
					break;
				case "Z":
					values = GetLetterArrayByIndex(25);
					break;

			}

			return values;
		}

		private static ObservableCollection<double> GetLetterArrayByIndex(int index)
		{
			var array = new ObservableCollection<double>(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
			array[index] = 1;
			return array;
		}
	}
}
