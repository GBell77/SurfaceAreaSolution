using System;
using System.Collections.Generic;
using System.IO;

namespace SurfaceAreaSolution
{
    /// <summary>
    /// Class to build output and then send it to specified file destination
    /// </summary>
    class ShapesOutputController
    {
        public List<ThreeDObject> shapeList;
        private List<string> errorList;
        private List<string> stringOutputList;
        private string outputPath;

        public ShapesOutputController(List<ThreeDObject> shapes, List<string> errors, string filePath)
        {
            shapeList = shapes;
            errorList = errors;
            outputPath = filePath;
            stringOutputList = new List<string>();
        }

        /// <summary>
        /// Creates output and sends to file.
        /// </summary>
        public void createAndDeliverOutput()
        {
            buildOutput();
            outputToFile();
        }

        /// <summary>
        /// Adds data from objects in shapelist to output list parsing 
        /// non strings to string.
        /// </summary>
        private void buildOutput()
        {
            foreach(ThreeDObject currentShape in shapeList)
            {
                stringOutputList.Add("Name:\t\t\t\t" + currentShape.Name);
                stringOutputList.Add("File address:\t\t\t" + currentShape.Address);
                stringOutputList.Add("Number of points:\t\t" + currentShape.NoOfVertices);
                stringOutputList.Add("Number of faces:\t\t" + currentShape.NoOfFaces);

                stringOutputList.Add("Dimensions of spanning box:\t" + 
                                (decimal)currentShape.minimumSpanningDimensions[0] + " " +
                                (decimal)currentShape.minimumSpanningDimensions[1] + " " +
                                (decimal)currentShape.minimumSpanningDimensions[2]);
                stringOutputList.Add("Volume of spanning box:\t\t" +
                                (decimal)currentShape.MinimumSpanningVolume);
                stringOutputList.Add("Total surface area:\t\t" + (decimal)currentShape.Area + "\n");
            }
            foreach(string error in errorList)
            {
                stringOutputList.Add(error + "\n");
            }
        }

        /// <summary>
        /// Writes to file or creates file if it does not exist
        /// and then writes to it.
        /// </summary>
        private void outputToFile()
        {
            File.WriteAllLines(outputPath, stringOutputList.ToArray());
        }
    }
}
