using System;
using System.IO;
using System.Numerics;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine
{
    struct Vector {
        public float x, y, z;

        public Vector(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = y;
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

    class Program {
        static Vector[] vertices = new Vector[] {
            //Triangle one
            //new Vector(-.1f, -.1f),
            //new Vector(.1f, -.1f),
            //new Vector(0f, .1f),

            //Triangle two
            new Vector(.4f, .3f),
            new Vector(.6f, .3f),
            new Vector(.5f, .5f)
        };

        private const int VertexX = 0;
        private const int VertexSize = 3;

        static void Main(string[] args) {
            Console.WriteLine(Vector.Max(new Vector(1, 3), new Vector(4, 5)).x); // Output: 4
            Console.WriteLine(Vector.Max(new Vector(1, 3), new Vector(4, 5)).y); // Output: 5
            Console.WriteLine(Vector.Min(new Vector(1, 3), new Vector(4, 5)).x); // Output: 1
            Console.WriteLine(Vector.Min(new Vector(1, 3), new Vector(4, 5)).y); // Output: 3

            Console.WriteLine(Vector.Max(new Vector(3, 1), new Vector(2, 4)).x); // Output: 3
            Console.WriteLine(Vector.Max(new Vector(3, 1), new Vector(2, 4)).y); // Output: 4
            Console.WriteLine(Vector.Min(new Vector(3, 1), new Vector(2, 4)).x); // Output: 2
            Console.WriteLine(Vector.Min(new Vector(3, 1), new Vector(2, 4)).y); // Output: 1
            
            Window window = CreateWindow();
            LoadTriangleIntoBuffer();
            CreateShaderProgram();

            Vector direction = new Vector(.005f, .005f);
            float multiplier = 0.888f;
            float scale = 1f;
            //Engine rendering loop
            while (!Glfw.WindowShouldClose(window)) {
                Glfw.PollEvents();
                ClearScreen();
                Render(window);
                
                for (int i = 0; i < vertices.Length; i++) {
                    vertices[i] += direction;
                }

                Vector min = vertices[0];
                for (int i = 0; i < vertices.Length; i++) {
                    min = Vector.Min(min, vertices[i]);
                }

                Vector max = vertices[0];
                for (int i = 0; i < vertices.Length; i++) {
                    max = Vector.Max(max, vertices[i]);
                }

                Vector center = (min + max) / 2;
                
                for (int i = 0; i < vertices.Length; i++) {
                    vertices[i] -= center;
                }
                
                for (int i = 0; i < vertices.Length; i++) {
                    vertices[i] *= multiplier;
                }
                
                for (int i = 0; i < vertices.Length; i++) {
                    vertices[i] += center;
                }

                scale *= multiplier;
                if (scale <= 0.5f) {
                    multiplier = 1.008f;
                }
                
                if (scale >= 1.5f) {
                    multiplier = 0.988f;
                }
                
                for (int i = 0; i < vertices.Length; i++) {
                    if (vertices[i].x >= 1 && direction.x > 0 || vertices[i].x <= -1 && direction.x < 0) {
                        direction.x *= -1;
                        break;
                    }
                }

                for (int i = 0; i < vertices.Length; i++) {
                    if (vertices[i].y >= 1 && direction.y > 0 || vertices[i].y <= -1 && direction.y < 0) {
                        direction.y *= -1;
                        break;
                    }
                }

                UpdateTriangleBuffer();
            }
        }
        
        static Window CreateWindow() {
            //Initialize and configure
            Glfw.Init();
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.Decorated, true);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.OpenglForwardCompatible, Constants.True);
            Glfw.WindowHint(Hint.Doublebuffer, Constants.True);

            //Create and launch a window
            Window window = Glfw.CreateWindow(1024, 768, "SharpEngine", Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);
            return window;
        }
        
        static unsafe void LoadTriangleIntoBuffer() {
            uint vertexArray = glGenVertexArray();
            uint vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            UpdateTriangleBuffer();
            glVertexAttribPointer(0, VertexSize, GL_FLOAT, false, sizeof(Vector), NULL);
            glEnableVertexAttribArray(0);
        }
        
        static unsafe void UpdateTriangleBuffer() {
            fixed (Vector* vertex = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vector) * vertices.Length, vertex, GL_STATIC_DRAW);
            }
        }
        
        static void CreateShaderProgram() {
            //Create vertex shader
            uint vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("shaders/screen-coordinates.vert"));
            glCompileShader(vertexShader);

            //Create fragment shader
            uint fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("shaders/green.frag"));
            glCompileShader(fragmentShader);

            //Create shader program - rendering pipeline
            uint program = glCreateProgram();
            glAttachShader(program, vertexShader);
            glAttachShader(program, fragmentShader);
            glLinkProgram(program);
            glUseProgram(program);
        }

        private static void ClearScreen() {
            glClearColor(.1f, .1f, .1f, 1);
            glClear(GL_COLOR_BUFFER_BIT);
        }

        private static void Render(Window window) {
            glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
            Glfw.SwapBuffers(window);
        }
    }
}