using System;
using System.Collections.Generic;
using System.IO;

namespace SurfaceAreaSolution
{
    /// <summary>
    /// Class to hold parameters of 3D object with methods
    /// to calculate area and minimum spanning volume
    /// </summary>
    class ThreeDObject
    {
        public double Area { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public double MinimumSpanningVolume { get; private set; }
        public List<double> minimumSpanningDimensions;
        public int NoOfVertices { get; private set; }
        public int NoOfFaces { get; private set; }
        public int NoOfEdges { get; private set; }

        private List<List<double>> vertices;
        private List<List<int>> faces;
        private List<List<double>> colours;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path"></param>
        public ThreeDObject(string path)
        {
            Address = path;
            vertices = new List<List<double>>();
            faces = new List<List<int>>();
            colours = new List<List<double>>();
            minimumSpanningDimensions = new List<double>();
        }

        /// <summary>
        /// Read all information from file and calculate properties
        /// </summary>
        /// <returns></returns>
        public bool ReadAndCalculate(out string errorMessage)
        {
            if(readFromFile(out errorMessage))
            {
                getNameFromAddress();
                calculateArea();
                findMinimumSpanningBox();
                errorMessage = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Read all information from OFF file and parse into
        /// relevant lists. Update error reference if read fails.
        /// </summary>
        /// <returns></returns>
        private bool readFromFile(out string errorMessage)
        {
            // Check if file path exists and can be read from
            if (File.Exists(Address))
            {
                // store data from OFF file in string array
                string[] lines = File.ReadAllLines(Address);

                //check that the file is an OFF file type and return false if not
                if (lines[0] != "OFF")
                {
                    errorMessage = "Error. File is not an .OFF at:";
                    return false;
                }

                // split second line of file which contains the number of
                // vertices, faces and edges. Use overload with StringSplitOptions
                // to deal with potential consecutive spaces.
                string[] valuesLine = lines[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // check that there is exactly three values in this line and return false otherwise
                if (valuesLine.Length != 3)
                {
                    errorMessage = "Error. File format. Incorrect number of arguments on line 2 at:";
                    return false;
                }

                // create array to hold int values of valuesLine(no. of vertices, faces and edges)
                int[] shapeProperties = new int[3];

                // fill integer array (shapeproperties) and set fields(NoOfFaces, NoOfVertices, noOfEdeges) with values
                for(int i = 0; i < valuesLine.Length; ++i)
                {
                    //check that the values in string array (valuesLine) are integers
                    bool success = int.TryParse(valuesLine[i], out shapeProperties[i]);
                    if (!success)
                    {
                        errorMessage = "Error. Arguments in line 2 are not of integer type at:";
                        return false;
                    }
                }

                NoOfVertices = shapeProperties[0];
                NoOfFaces = shapeProperties[1];
                NoOfEdges = shapeProperties[2];
                
                // convert each line of vertices from strings and add them to vetices list
                for(int i = 0; i < NoOfVertices; ++i)
                {
                    // split the current line starting at i + 2 as the first line containig vertex co-ordinates
                    string[] tmpLine = lines[i + 2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    List<double> tmpList = new List<double>();
                    for(int j = 0; j < tmpLine.Length; ++j)
                    {
                        double tmpDouble;
                        // check that the line contains double values and add them to list
                        bool success = double.TryParse(tmpLine[j], out tmpDouble);
                        if (!success)
                        {
                            // added 1 to line number as text files count lines from 1
                            errorMessage = "Error. Arguments in line " + (i + 2 + 1) + " cannot be parsed to double at:";
                            return false;
                        }
                        tmpList.Add(tmpDouble);
                    }

                    // add list of co-ordinates to vertices list
                    vertices.Add(tmpList);
                }
                
                // convert each line containing faces and store them in faces list
                // also check for the presence of colour values and convert and store them if present
                for(int i = 0; i < NoOfFaces; ++i)
                {
                    // split current line starting with line + 2 + NoOfVertices as this is first line of faces values
                    string[] tmpLine = lines[i + 2 + NoOfVertices].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    // extract the first value from the current line and convert to an integer
                    // giving the amount of integers that should follow this number
                    int verticesInFace;
                    bool success = int.TryParse(tmpLine[0], out verticesInFace);
                    if (!success)
                    {
                        errorMessage = "Error. Arguments in line " + (i + 2 + NoOfVertices + 1) + " cannot be parsed to int at:";
                        return false;
                    }
                    
                    // check for the presence of colour values by checking whether the
                    // list is longer than the number of vertices (denoted by the value
                    // at the beginning of the list) plus one space for the value at the beginning
                    bool hasColourValues = tmpLine.Length > verticesInFace + 1;

                    // create a list to add vertex values for face
                    List<int> tmpList = new List<int>();

                    // add all vertex references to face list
                    for(int j = 1; j <= verticesInFace; ++j)
                    {
                        int tmpInt;
                        success = int.TryParse(tmpLine[j], out tmpInt);
                        if (!success)
                        {
                            errorMessage = "Error. Arguments in line " + (i + 2 + NoOfVertices + 1) + " cannot be parsed to int at:";
                            return false;
                        }
                        tmpList.Add(tmpInt);
                    }

                    // add list of vertices to faces list
                    faces.Add(tmpList);

                    // if colours are present convert them and add them to colours list
                    if(hasColourValues)
                    {
                        // create list to hold colour values
                        List<double> tmpColourList = new List<double>();

                        // convert colour values to double and store in list
                        for(int j = verticesInFace + 1; j < tmpLine.Length; ++j)
                        {
                            double tmpDouble;

                            // check that the colour values in current line are doubles
                            success = double.TryParse(tmpLine[j], out tmpDouble);
                            if (!success)
                            {
                                errorMessage = "Error. Arguments in line " + (i + 2 + NoOfVertices + 1) + " cannot be parsed to double at:";
                                return false;
                            }

                            // add colour value to list
                            tmpColourList.Add(tmpDouble);
                        }

                        // add list of colour (rgb) values to colours list
                        colours.Add(tmpColourList);
                    }
                }
                errorMessage = null;
                return true;
            }
            errorMessage = "File does not exist at:";
            return false;
        }

        /// <summary>
        /// Extract name of shape from the end of the file path
        /// </summary>
        private void getNameFromAddress()
        {
            // variables to check for the final dot in the address and the preceding backslash
            int dotIndex = -1;
            int startNameIndex = -1;

            // work backwards through the address looking for '.'
            for (int i = Address.Length - 1; i >= 0; --i)
            {
                if (Address[i] == '.')
                {
                    dotIndex = i;
                    break;
                }
            }

            // work backwards from the previously found '.' to find the next backslash '\'
            if(dotIndex != -1)
            {
                for(int i = dotIndex - 1; i >= 0; --i)
                {
                    if(Address[i] == '\\')
                    {
                        startNameIndex = i + 1;
                        break;
                    }
                }
            }

            // if both indexes have been found find the substring that is the name of the shape
            if(startNameIndex > -1 && dotIndex > -1 && dotIndex >= startNameIndex)
                Name = Address.Substring(startNameIndex, dotIndex - startNameIndex);
        }

        /// <summary>
        /// Calculate the total area of all faces
        /// </summary>
        private void calculateArea()
        {
            // start a running total at zero for the area of the object
            double total = 0.0;

            // call appropriate area method dependant on number of vertices and
            // add to total. Set area to -1 if invalid no. of vertices
            foreach(List<int> vertList in faces)
            {
                if (vertList.Count == 3)
                    total += areaOfTriangle(vertList);
                else if (vertList.Count == 4)
                    total += areaOfQuadilateral(vertList);
                else
                {
                    Area = -1;
                    return;
                }
            }

            Area = total;
        }     

        /// <summary>
        /// Calculate area of triangle defined by list of indexes of vertices
        /// in vertices list.
        /// </summary>
        /// <param name="thisFace"></param>
        /// <returns></returns>
        private double areaOfTriangle(List<int> thisFace)
        {
            List<List<double>> triangleVertices = getVerticesForFace(thisFace);
            List<List<double>> vectors = createTwoVectors(triangleVertices);
            List<double> crossProd = crossProduct(vectors[0], vectors[1]);
            double crossMagnitude = magnitudeOfCrossProduct(crossProd);
            double triArea = crossMagnitude / 2;

            return triArea;
        }

        /// <summary>
        /// Get lists of coordinates of vertices referenced by integers in face list
        /// </summary>
        /// <param name="aFace"></param>
        /// <returns></returns>
        private List<List<double>> getVerticesForFace(List<int> aFace)
        {
            List<List<double>> vertexList = new List<List<double>>();
            foreach(int vertexIndex in aFace)
                vertexList.Add(vertices[vertexIndex]);
            return vertexList;  
        }

        /// <summary>
        /// Create a list of two vectors AB and AC from a list of vertices
        /// representing the triangle ABC.
        /// </summary>
        /// <param name="verticesOfTriangle"></param>
        /// <returns></returns>
        private List<List<double>> createTwoVectors(List<List<double>> verticesOfTriangle)
        {
            List<List<double>> vectorList = new List<List<double>>();
            vectorList.Add(calculateVector(verticesOfTriangle[0], verticesOfTriangle[1]));
            vectorList.Add(calculateVector(verticesOfTriangle[0], verticesOfTriangle[2]));

            return vectorList;
        }

        /// <summary>
        /// Calculate vector from two lists of coordinates for two points.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private List<double> calculateVector(List<double> point1, List<double> point2)
        {
            List<double> vector = new List<double>();
            for (int i = 0; i < point1.Count; ++i)
                vector.Add(point1[i] - point2[i]);

            return vector;
        }

        /// <summary>
        /// Calculate the cross product of two vectors
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        private List<double> crossProduct(List<double> vector1, List<double> vector2)
        {
            List<double> product = new List<double>();

            product.Add(vector1[1] * vector2[2] - vector1[2] * vector2[1]);
            product.Add(vector1[2] * vector2[0] - vector1[0] * vector2[2]);
            product.Add(vector1[0] * vector2[1] - vector1[1] * vector2[0]);

            return product;
        }

        /// <summary>
        /// Calculate the magnitude of a vector
        /// </summary>
        /// <param name="crossProd"></param>
        /// <returns></returns>
        private double magnitudeOfCrossProduct(List<double> crossProd)
        {
            return Math.Sqrt(crossProd[0] * crossProd[0] + crossProd[1] * crossProd[1] + crossProd[2] * crossProd[2]);
        }

        /// <summary>
        /// Calculate area of quadilateral
        /// </summary>
        /// <param name="thisFace"></param>
        /// <returns></returns>
        private double areaOfQuadilateral(List<int> thisFace)
        {
            // create 2 triangular faces from the square face
            List<int> triangle1 = new List<int> { thisFace[0], thisFace[1], thisFace[2] };
            List<int> triangle2 = new List<int> { thisFace[2], thisFace[3], thisFace[0] };

            return areaOfTriangle(triangle1) + areaOfTriangle(triangle2);
        }

        /// <summary>
        /// Calculate minimum axis aligned spanning box dimensions and volume
        /// </summary>
        private void findMinimumSpanningBox()
        {
            // set initial values of min and max for each axis to first vertex in list
            double minX, maxX, minY, maxY, minZ, maxZ;
            minX = vertices[0][0];
            maxX = vertices[0][0];
            minY = vertices[0][1];
            maxY = vertices[0][1];
            minZ = vertices[0][2];
            maxZ = vertices[0][2];

            // search all vertices and update minmum and maximum values
            // for each axis
            foreach(List<double> coords in vertices)
            {
                if (coords[0] > maxX)
                    maxX = coords[0];
                if (coords[0] < minX)
                    minX = coords[0];
                if (coords[1] > maxY)
                    maxY = coords[1];
                if (coords[1] < minY)
                    minY = coords[1];
                if (coords[2] > maxZ)
                    maxZ = coords[2];
                if (coords[2] < minZ)
                    minZ = coords[2];
            }

            // Add dimensions of box to minimumSpanningDimensions List
            minimumSpanningDimensions.Add(maxX - minX);
            minimumSpanningDimensions.Add(maxY - minY);
            minimumSpanningDimensions.Add(maxZ - minZ);

            // calculate and store volume of minimum spanning box
            MinimumSpanningVolume = minimumSpanningDimensions[0] * minimumSpanningDimensions[1] * minimumSpanningDimensions[2];
        }
    }
}
