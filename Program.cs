using System.IO;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine
{
    class Program {
        static Vertex[] vertices = new Vertex[] {
            new Vertex(new Vector(-0.5f, 0f), Color.Red),
            new Vertex(new Vector(0.5f, 0f), Color.Green),
            new Vertex(new Vector(0f, 1f), Color.Blue)
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
                if (scale <= 0.2f) {
                    multiplier = 1.008f;
                }
                
                if (scale >= 0.6f) {
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
            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(Vertex), NULL);
            glVertexAttribPointer(1, 4, GL_FLOAT, false, sizeof(Vertex), (void*)sizeof(Vector));
            glEnableVertexAttribArray(0);
            glEnableVertexAttribArray(1);
        }
        
        static unsafe void UpdateTriangleBuffer() {
            fixed (Vertex* vertex = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * vertices.Length, vertex, GL_DYNAMIC_DRAW);
            }
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
            glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
            Glfw.SwapBuffers(window);
        }
    }
}