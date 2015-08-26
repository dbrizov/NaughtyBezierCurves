Unity-BezierCurves
==================

Unity-BezierCurves is plugin for Unity which provides you with a BezierCurve GameObject.

System Requirements
-------------------

Unity 5.1 or later versions.<br />
I actually created the plugin with Unity 5.1 and haven't tried to run it with an older version. If you are using an older version of Unity, feel free to try it out. I don't think there are any reasons for it not to run in Unity 4.6.

Features
--------

- Curve modification directly in the Scene View via Control Points with Handles
- Custom editor for adding, deleting and reordering control points very easily
- Full Undo/Redo integration

Ways to create a BezierCurve GameObject
---------------------------------------

- From the Top Menu -> GameObjects -> Create Other -> Bezier Curve
- Right click in the Hierarchy View -> Create Other -> Bezier Curve
- Drag the prefab in the Scene (The prefab is located in "Bezier Curves/Assets/Prefabs" folder)

Known Issues
------------

- There is a bug with the Undo/Redo when reordering control points with the editor. When you want to do an undo, you actually have to press Ctrl+Z / Cmd+Z twice for the operation to be undone.

License
-------

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
