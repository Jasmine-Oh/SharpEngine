using System;

namespace SharpEngine {
    public struct Vector {
        public float x, y, z;

        public Vector(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector(float x, float y) {
            this.x = x;
            this.y = y;
            z = 0;
        }
        
        public static Vector operator +(Vector vecOne, Vector vecTwo) {
            return new Vector(vecOne.x + vecTwo.x, vecOne.y + vecTwo.y, vecOne.z + vecTwo.z);
        }
        
        public static Vector operator -(Vector vecOne, Vector vecTwo) {
            return new Vector(vecOne.x + -vecTwo.x, vecOne.y + -vecTwo.y, vecOne.z + -vecTwo.z);
        }

        public static Vector operator *(Vector vec, float f) {
            return new Vector(vec.x * f, vec.y * f, vec.z * f);
        }

        public static Vector operator /(Vector vec, float f) {
            return new Vector(vec.x / f, vec.y / f, vec.z / f);
        }
        
        public static Vector Max(Vector a, Vector b) {
            return new Vector(MathF.Max(a.x, b.x), MathF.Max(a.y, b.y), MathF.Max(a.z, b.z));
        }

        public static Vector Min(Vector a, Vector b) {
            return new Vector(MathF.Min(a.x, b.x), MathF.Min(a.y, b.y), MathF.Min(a.z, b.z));
        }
    }
}