using System.Drawing;

namespace SharpEngine {
    public struct Vertex {
        public Vector position;
        public Color color;

        public Vertex(Vector position, Color color) {
            this.position = position;
            this.color = color;
        }
    }

    public struct Color {
        public float r, g, b, a;
        public static readonly Color Red = new Color(1, 0, 0, 1);
        public static readonly Color Green = new Color(0, 1, 0, 1);
        public static readonly Color Blue = new Color(0, 0, 1, 1);
        

        public Color(float r, float g, float b, float a) {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }
}