using OCRFFNetwork.model.api.image;
using System.Collections.ObjectModel;
using System.Drawing;

namespace OCRFFNetwork.model
{
    public class Letter
    {

        #region Internal Metods

        public ObservableCollection<double> GetImagePixels(int imageIndex)
        {
            return ImageUtils.GetImagePixels(this.ImagesPath[imageIndex]);
        }

        #endregion //Internal Metods

        #region Properties

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if(value == _name)
                {
                    return;
                }

                _name = value;
            }
        }

        private string _path;
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                if(value == _path)
                {
                    return;
                }

                _path = value;
            }
        }

        private ObservableCollection<string> _imagesPath = new ObservableCollection<string>();
        public ObservableCollection<string> ImagesPath
        {
            get
            {
                return _imagesPath;
            }
            set
            {
                if(value == _imagesPath)
                {
                    return;
                }

                _imagesPath = value;
            }
        }

        #endregion //Properties
    }
}