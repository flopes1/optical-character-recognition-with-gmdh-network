using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRFFNetwork.model
{
    public class AlphabetDataset
    {

        public AlphabetDataset(string alphabetType)
        {
            this.AlphabetType = alphabetType;
            this.LoadAlphabetLetters();
        }

        #region Internal Methods

        private void LoadAlphabetLetters()
        {
            var lettersDirectoryPath = Network.Default.DatasetDirectory;
            var alphabetType = "";

            switch(this.AlphabetType)
            {
                case EnumDatasetType.Train:
                    alphabetType = Network.Default.TrainDirectory;
                    break;
                case EnumDatasetType.Test:
                    alphabetType = Network.Default.TrainDirectory;
                    break;
                case EnumDatasetType.Validation:
                    alphabetType = Network.Default.TrainDirectory;
                    break;
            }

            var lettersDirectory = Directory.GetDirectories(lettersDirectoryPath);

            foreach(var letterDirPath in lettersDirectory)
            {
                var letterFullPath = letterDirPath + alphabetType;

                var letter = new Letter()
                {
                    Name = Path.GetFileName(letterDirPath),
                    Path = letterFullPath,
                    ImagesPath = new ObservableCollection<string>(Directory.GetFiles(letterFullPath))
                };

                this.Letters.Add(letter);
            }

        }

        #endregion // Internal Methods

        #region Alphabet Type

        private string _alphabetType;

        public string AlphabetType
        {
            get
            {
                return _alphabetType;
            }
            set
            {
                if(value == _alphabetType)
                {
                    return;
                }

                _alphabetType = value;
            }

        }

        #endregion //Alphabet Type

        #region Alphabet

        private ObservableCollection<Letter> _letters = new ObservableCollection<Letter>();

        public ObservableCollection<Letter> Letters
        {
            get
            {
                return _letters;
            }

            set
            {
                if (value == _letters)
                {
                    return;
                }

                _letters = value;
            }

        }

        #endregion //Alphabet
    }
    
}
