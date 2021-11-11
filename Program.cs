using System;
using System.IO;
using System.Numerics;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine
{
    public struct Vertex {
        public Vector position;

        public Vertex(Vector position) {
            this.position = position;
        }
    }
    
    class Program {
        private static Vertex[] vertices = new Vertex[] {
            /*
            new Vertex(new Vector(0f, 0f)),
            new Vertex(new Vector(1f, 0f)),
            new Vertex(new Vector(0f, 1f)),*/

            //Triangle two
            new Vertex(new Vector(.4f, .3f)),
            new Vertex(new Vector(.6f, .3f)),
            new Vertex(new Vector(.5f, 5f))
        };
        
        private const int VertexSize = 3;

        static void Main(string[] args) {
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
                    vertices[i].position += direction;
                }

                Vector min = vertices[0].position;
                for (int i = 0; i < vertices.Length; i++) {
                    min = Vector.Min(min, vertices[i].position);
                }

                Vector max = vertices[0].position;
                for (int i = 0; i < vertices.Length; i++) {
                    max = Vector.Max(max, vertices[i].position);
                }

                Vector center = (min + max) / 2;
                
                for (int i = 0; i < vertices.Length; i++) {
                    vertices[i].position -= center;
                }
                
                for (int i = 0; i < vertices.Length; i++) {
                    vertices[i].position *= multiplier;
                }
                
                for (int i = 0; i < vertices.Length; i++) {
                    vertices[i].position += center;
                }

                scale *= multiplier;
                if (scale <= 0.5f) {
                    multiplier = 1.008f;
                }
                
                if (scale >= 1.5f) {
                    multiplier = 0.988f;
                }
                
                for (int i = 0; i < vertices.Length; i++) {
                    if (vertices[i].position.x >= 1 && direction.x > 0 || vertices[i].position.x <= -1 && direction.x < 0) {
                        direction.x *= -1;
                        break;
                    }
                }

                for (int i = 0; i < vertices.Length; i++) {
                    if (vertices[i].position.y >= 1 && direction.y > 0 || vertices[i].position.y <= -1 && direction.y < 0) {
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
            fixed (Vertex* vertex = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * vertices.Length, vertex, GL_STATIC_DRAW);
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