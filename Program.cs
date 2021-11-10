using System;
using System.IO;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine
{
    class Program
    {
        static float[] vertices = new float[] {
            // vertex 1 x, y, z
            -.5f, -.5f, 0f,
            // vertex 2 x, y, z
            .5f, -.5f, 0f,
            // vertex 3 x, y, z
            0f, .5f, 0f
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
                Render();
                
                for (int i = VertexX; i < vertices.Length; i ++) {
                    vertices[i] *= 0.99f;
                }
                
                UpdateTriangleBuffer();
            }
        }

        private static void ClearScreen() {
            glClearColor(.1f, .1f, .1f, 1);
            glClear(GL_COLOR_BUFFER_BIT);
        }

        private static void Render() {
            glDrawArrays(GL_TRIANGLES, 0, vertices.Length / 3);
            glFlush();
        }

        static void ExpandTriangle() {
            vertices[0] -= 0.001f;
            vertices[1] -= 0.001f;
            vertices[3] += 0.001f;
            vertices[4] -= 0.001f;
            vertices[7] += 0.0019f;
        }

        static unsafe void LoadTriangleIntoBuffer() {
            //Load the vertices into a buffer
            var vertexArray = glGenVertexArray();
            var vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            UpdateTriangleBuffer();
            glVertexAttribPointer(0, VertexSize, GL_FLOAT, false, VertexSize * sizeof(float), NULL);
            glEnableVertexAttribArray(0);
        }
        
        static void CreateShaderProgram() {
            //Create vertex shader
            var vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("shaders/screen-coordinates.vert"));
            glCompileShader(vertexShader);

            //Create fragment shader
            var fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("shaders/green.frag"));
            glCompileShader(fragmentShader);

            //Create shader program - rendering pipeline
            var program = glCreateProgram();
            glAttachShader(program, vertexShader);
            glAttachShader(program, fragmentShader);
            glLinkProgram(program);
            glUseProgram(program);
        }
        
        static unsafe void UpdateTriangleBuffer() {
            fixed (float* vertex = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, vertex, GL_STATIC_DRAW);
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
            Glfw.WindowHint(Hint.Doublebuffer, Constants.False);

            //Create and launch a window
            var window = Glfw.CreateWindow(1024, 768, "SharpEngine", Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);
            return window;
        }
    }
}