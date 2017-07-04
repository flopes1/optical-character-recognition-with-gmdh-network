using OCRFFNetwork.model;
using OCRFFNetwork.model.api.image;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace OCRFFNetwork.dataset.api.image
{

    public class CharacterDatasetHandler
    {

        public static void GenerateDatasetEntry(string sourceDatasetDirectory, string outputDatasetDirectory)
        {
            var inputDirectory = Directory.GetDirectories(sourceDatasetDirectory);

            foreach (var letterFolder in inputDirectory)
            {
                //var trainFolder = Directory.GetDirectories(letterFolder);
                //var allFolders = new ObservableCollection<String>(trainFolder);
                //var trainFolderPath = allFolders.Where(f => f.Contains("train")).FirstOrDefault();
                //var trainFolderFiles = Directory.GetFiles(trainFolderPath);

                var currentLetter = Path.GetFileName(letterFolder);

                var trainPath = outputDatasetDirectory + "\\" + currentLetter + "\\" + "train\\";
                var validationPath = outputDatasetDirectory + "\\" + currentLetter + "\\" + "validation\\";
                var testPath = outputDatasetDirectory + "\\" + currentLetter + "\\" + "test\\";

                var files = Directory.GetFiles(letterFolder);

                for (int i = 0; i < 160; i++)
                {
                    var outputImage = "";
                    if (i < 80)
                    {
                        Directory.CreateDirectory(trainPath);
                        outputImage = trainPath + Path.GetFileName(files[i]);

                    }
                    else if (i < 120)
                    {
                        Directory.CreateDirectory(validationPath);
                        outputImage = validationPath + Path.GetFileName(files[i]);
                    }
                    else if (i < 160)
                    {
                        Directory.CreateDirectory(testPath);
                        outputImage = testPath + Path.GetFileName(files[i]);
                    }

                    if (!string.IsNullOrEmpty(outputImage))
                    {
                        File.Copy(files[i], outputImage);
                        ImageUtils.Resize(outputImage, 64, 64);
                        while(true)
                        {
                            System.GC.Collect();
                            System.GC.WaitForPendingFinalizers();
                            if (!IsFileLocked(new FileInfo(outputImage)))
                            {
                                break;
                            }                            
                            Thread.Sleep(200);
                        }
                        File.Delete(outputImage);
                    }
                }
            }
        }


        private static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }


    }
}
