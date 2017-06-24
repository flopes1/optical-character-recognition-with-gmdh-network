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
                var trainFolder = Directory.GetDirectories(letterFolder);
                var allFolders = new ObservableCollection<String>(trainFolder);
                var trainFolderPath = allFolders.Where(f => f.Contains("train")).FirstOrDefault();
                var trainFolderFiles = Directory.GetFiles(trainFolderPath);

                var currentLetter = Path.GetFileName(letterFolder);

                var trainPath = outputDatasetDirectory + "\\" + currentLetter + "\\" + "train\\";
                var validationPath = outputDatasetDirectory + "\\" + currentLetter + "\\" + "validation\\";
                var testPath = outputDatasetDirectory + "\\" + currentLetter + "\\" + "test\\";

                for (int i = 0; i < 200; i++)
                {
                    var outputImage = "";
                    if (i < 100)
                    {
                        Directory.CreateDirectory(trainPath);
                        outputImage = trainPath + Path.GetFileName(trainFolderFiles[i]);

                    }
                    else if (i < 150)
                    {
                        Directory.CreateDirectory(validationPath);
                        outputImage = validationPath + Path.GetFileName(trainFolderFiles[i]);
                    }
                    else
                    {
                        Directory.CreateDirectory(testPath);
                        outputImage = testPath + Path.GetFileName(trainFolderFiles[i]);
                    }
                    if (!string.IsNullOrEmpty(outputImage))
                    {
                        File.Copy(trainFolderFiles[i], outputImage);
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
