using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CornellBoxWPF.Acceleration
{
    public class BVHSphere : Sphere
    {
        public Sphere _leftChild { get; set; }
        public Sphere _rightChild { get; set; }
        
        public BVHSphere(Vector3 center, float radius, Vector3 color, Sphere leftChild, Sphere rightChild, bool reflection = false, bool procedualTexture = false, bool bitmapTexture = false)
        {
            this._leftChild = leftChild;
            this._rightChild = rightChild;
            this._center = center;
            this._radius = radius;
            this._color = color;
            this._reflection = reflection;
            this._proceduralTexture = procedualTexture;
            this._bitmapTexture = bitmapTexture;
        }
    }
}
