using System;
using System.IO;
using System.Collections.Generic;

namespace SurfaceAreaSolution
{
    class Program
    {
        /// <summary>
        /// main function which takes file addresses, creates the 3D objects from
        /// those files calculates area and minimum spanning boxes and exports them
        /// to a specified file. Errors in reading the file are also eported to the file.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string outputDestination = @"C:\location_of_output_file.txt"; // add details here
            List<ThreeDObject> shapeList = new List<ThreeDObject>();
            List<string> errorList = new List<string>();
            List<string> filePathList = new List<string>();
            filePathList.Add(@"C:\location_of_model_data.off"); // add details here

            //filePathList.Add(@".off");
            //filePathList.Add(@".off");
            //filePathList.Add(@".off");

            foreach(string filePath in filePathList)
            {
                ThreeDObject shapeInFilePath = new ThreeDObject(filePath);
                string error;
                if(shapeInFilePath.ReadAndCalculate(out error))
                    shapeList.Add(shapeInFilePath);
                else
                {
                    error += "\n" + filePath;
                    errorList.Add(error);
                }
            }

            shapeList.Sort((x, y) => x.Area.CompareTo(y.Area));

            ShapesOutputController shapesOutputController = new ShapesOutputController(shapeList, errorList, outputDestination);
            shapesOutputController.createAndDeliverOutput();
        }
    }
}
