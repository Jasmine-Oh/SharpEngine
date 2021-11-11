using System.IO;
using System.Runtime.InteropServices;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine
{
    class Program {
        public class Triangle {
            private Vertex[] vertices;
            public float CurrentScale {
                get;
                private set;
            }
            
            public Triangle(Vertex[] vertices) {
                this.vertices = vertices;
                CurrentScale = 1;
            }

            public Vector GetMaxBounds() {
                Vector max = vertices[0].position;
                for (int i = 0; i < vertices.Length; i++) {
                    max = Vector.Max(max, vertices[i].position);
                }

                return max;
            }

            public Vector GetMinBounds() {
                Vector min = vertices[0].position;
                for (int i = 0; i < vertices.Length; i++) {
                    min = Vector.Min(min, vertices[i].position);
                }

                return min;
            }

            public void Scale(float multiplier) {
                Vector center = (GetMinBounds() + GetMaxBounds()) / 2;
                Move(center * - 1);

                for (int i = 0; i < vertices.Length; i++) {
                    vertices[i].position *= multiplier;
                }
                
                Move(center);
                CurrentScale *= multiplier;
            }

            public void Move(Vector direction) {
                for (int i = 0; i < vertices.Length; i++) {
                    vertices[i].position += direction;
                }
            }
            
            public unsafe void Render() {
                fixed (Vertex* vertex = &vertices[0]) {
                    glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * vertices.Length, vertex, GL_DYNAMIC_DRAW);
                }
                
                glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
            }
        }

        private static Triangle triangle = new Triangle(
            new Vertex[] {
            new Vertex(new Vector(-0.5f, 0f), Color.Red),
            new Vertex(new Vector(0.5f, 0f), Color.Green),
            new Vertex(new Vector(0f, 1f), Color.Blue)
            }
        );

        static void Main(string[] args) {
            Window window = CreateWindow();
            LoadTriangleIntoBuffer();
            CreateShaderProgram();

            Vector direction = new Vector(.005f, .005f);
            float multiplier = 0.888f;

            //Engine rendering loop
            while (!Glfw.WindowShouldClose(window)) {
                Glfw.PollEvents();
                ClearScreen();
                Render(window);
                
                triangle.Move(direction);

                if (triangle.GetMaxBounds().x >= 1 && direction.x > 0 || triangle.GetMinBounds().x <= -1 && direction.x < 0) {
                    direction.x *= -1;
                }
                
                if (triangle.GetMaxBounds().y >= 1 && direction.y > 0 || triangle.GetMinBounds().y <= -1 && direction.y < 0) {
                    direction.y *= -1;
                }

                triangle.Scale(multiplier);
                
                if (triangle.CurrentScale <= 0.2f) {
                    multiplier = 1.008f;
                }
                
                if (triangle.CurrentScale >= 0.6f) {
                    multiplier = 0.988f;
                }
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
        
        //Clean up homework
        static unsafe void LoadTriangleIntoBuffer() {
            uint vertexArray = glGenVertexArray();
            uint vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.position)));
            glVertexAttribPointer(1, 4, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.color)));
            glEnableVertexAttribArray(0);
            glEnableVertexAttribArray(1);
        }

        static void CreateShaderProgram() {
            //Create vertex shader
            uint vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("shaders/position-color.vert"));
            glCompileShader(vertexShader);

            //Create fragment shader
            uint fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("shaders/vertex-color.frag"));
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
            triangle.Render();
            Glfw.SwapBuffers(window);
        }
    }
}