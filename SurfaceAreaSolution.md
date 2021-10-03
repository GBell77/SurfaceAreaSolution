# Actify Programming Task: .off Model Analysis - Garry Bell

### Assumptions
1. All faces are represented by 3 or 4 points.
2. Sorted by surface area is in ascending order.

### Summary

The program reads input from the files and then stores the data
in type appropriate collections inside an instantiation of a 3D Object class. 
This data is then used to calculate the area of the shapes by calculating
the areas of all triangles making up the shape. The area of each triangle
is calculated by creating two vectors using the vertices of the triangle. 
The cross product is then calculated. The area of the triangle is equal to 
half of the magnitude of the cross product. The total of these triangle areas
is then stored in the object. The objects are then passed to a file output class
which exports the details of the objects and any errors to an output file.

### Unit Testing

The Assert class methods could be used to unit test the ThreeDObject methods
* areaOfTriangle
* areaOfQuadilateral
* magnitudeOfCrossProduct

The CollectionAssert class methods could be used to unit test the ThreeDObject methods
* getVerticesForFace
* createTwoVectors
* calculateVector
* crossProduct

These tests would consist of comparisons of expected and actual returns
from the methods.

For the methods ThreeDObject.getNameFromAddress, ThreeDObject.calculateArea,
ThreeDObject.findMinimumSpanningBox and ShapesOutputController.buildOutput
test methods which filled the appropriate member varisble Lists with data and then checked the values of the member variables that are updated in the method would need to be created.

For the file input function ThreeDObject.readFromFile a number of input files could be created and an expected internal state of the object after reading the file. This could then be compared with the object after inititalising the member variables.

The output method ShapesOutputController.outputToFile would require a test method that populated stringOutputList and an expected output for the file (a checksum perhaps) which could be compared wiht the actual output.
 