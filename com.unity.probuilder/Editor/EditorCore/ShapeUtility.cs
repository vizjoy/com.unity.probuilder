using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;

namespace UnityEditor.ProBuilder
{
    public static class ShapeUtility
    {
        public static Shape GetShapeGenerator(ShapeType type)
        {
            switch (type)
            {
                case ShapeType.Cube:
                    return new Cube();
                case ShapeType.Stair:
                    throw new System.NotImplementedException();
                case ShapeType.CurvedStair:
                    throw new System.NotImplementedException();
                case ShapeType.Prism:
                    throw new System.NotImplementedException();
                case ShapeType.Cylinder:
                    throw new System.NotImplementedException();
                case ShapeType.Plane:
                    throw new System.NotImplementedException();
                case ShapeType.Door:
                    throw new System.NotImplementedException();
                case ShapeType.Pipe:
                    throw new System.NotImplementedException();
                case ShapeType.Cone:
                    throw new System.NotImplementedException();
                case ShapeType.Sprite:
                    throw new System.NotImplementedException();
                case ShapeType.Arch:
                    throw new System.NotImplementedException();
                case ShapeType.Sphere:
                    throw new System.NotImplementedException();
                case ShapeType.Torus:
                    throw new System.NotImplementedException();
            }

            throw new System.NotImplementedException();
        }
    }
}
