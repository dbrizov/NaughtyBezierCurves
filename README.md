# NaughtyBezierCurves
NaughtyBezierCurves is plugin for Unity which provides you with a BezierCurve GameObject.

<img src="https://github.com/dbrizov/dbrizov.github.io/blob/master/images/project-images/bezier-curve/bezier-curve.gif" alt="gif" width="350" /><br />
<img src="https://github.com/dbrizov/dbrizov.github.io/blob/master/images/project-images/bezier-curve/bc4.png" alt="screenshot" width="350" />
<img src="https://github.com/dbrizov/dbrizov.github.io/blob/master/images/project-images/bezier-curve/bc2.png" alt="screenshot" width="350" />

## System Requirements
Unity 2017.3.0 or later versions.

## Installation
Add this entry in your **manifest.json**
```
"com.dbrizov.naughtybeziercurves": "https://github.com/dbrizov/NaughtyBezierCurves.git#upm"
```

## Features
- Curve modification directly in the Scene View via Key Points with Handles
- Custom editor for Adding, Deleting and Reordering Key Points very easily
- Full Undo/Redo integration

## Ways to create a BezierCurve GameObject
- From the Top Menu -> GameObjects -> Create Other -> Bezier Curve
- Right click in the Hierarchy View -> Create Other -> Bezier Curve
- Drag the prefab in the Scene (The prefab is located in "Assets/Bezier Curves/Prefabs" folder)

## Code Examples
```C#
BezierCurve3D curve = GetComponent<BezierCurve3D>();

// Evaluate a position and rotation along the curve at a given time
float time = 0.5f; // In range [0, 1]
Vector3 position = curve.GetPoint(time);
Quaterion rotation = curve.GetRotation(time, Vector3.up);

// Get the length of the curve
// This operation is not very heavy, but I advise you to cache the length if you are going to use it
// many times and when you know that the curve won't change at runtime.
float length = curve.GetApproximateLength();

// Other methods
Vector3 tangent = curve.GetTangent(time);
Vector3 binormal = curve.GetBinormal(time, Vector3.up);
Vector3 normal = curve.GetNormal(time, Vector3.up);

// Add a key point at the end of the curve
BezierPoint3D keyPoint = curve.AddKeyPoint(); // via fast method
BezierPoint3D keyPoint = curve.AddKeyPointAt(curve.KeyPointsCount); // via specific index

// Remove a key point
bool isRemoved = curve.RemoveKeyPointAt(0); // Remove the first key point

// Foreach all key points
for (int i = 0; i < curve.KeyPointsCount; i++)
{
    Debug.Log(curve.KeyPoints[i].Position);
    Debug.Log(curve.KeyPoints[i].LeftHandleLocalPosition);
    Debug.Log(curve.KeyPoints[i].RightHandleLocalPosition);
}
```

## License
The MIT License (MIT)

Copyright (c) 2015 Denis Rizov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
