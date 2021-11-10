using System;
using System.IO;
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
    }
    
    class Program
    {
        static Vector[] vertices = new Vector[] {
            //Triangle one
            new Vector(-.1f, -.1f),
            new Vector(.1f, -.1f),
            new Vector(0f, .1f),
            
            //Triangle two
            new Vector(.4f, .4f),
            new Vector(.6f, .4f),
            new Vector(.5f, .6f)
        };

        private const int VertexX = 0;
        private const int VertexSize = 3;
        
        static void Main(string[] args) {
            Window window = CreateWindow();
            LoadTriangleIntoBuffer();
            CreateShaderProgram();

            //Engine rendering loop
            while (!Glfw.WindowShouldClose(window)) {
                Glfw.PollEvents();
                ClearScreen();
                Render(window);
                
                for (int i = VertexX; i < vertices.Length; i++) {
                    vertices[i].x += 0.001f;
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