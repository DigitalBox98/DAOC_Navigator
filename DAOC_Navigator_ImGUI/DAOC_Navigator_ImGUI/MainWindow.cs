/*
 * DAOC Navigator - The free open DAOC game navigator
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, see <https://www.gnu.org/licenses/>
 *
 */

using System;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using NativeFileDialogSharp;
using System.IO;
using DAOC_Navigator_Core;
using DAOC_Navigator_Core.GL_Rendering;
using DAOC_Navigator_Core.WorldObjects;
using DAOC_Navigator_Core.Game;

namespace DAOC_Navigator_ImGUI
{
    public class MainWindow : GameWindow
    {

        // ImGUI part
        //

        // Controller 
        ImGuiController _controller;

        // Window/Menus status
        bool options_chooser = true;
        bool about = false;
        bool camera_window = true;
        bool app_quit = false;
        bool monster = true;

        // Camera for ImGUI
        System.Numerics.Vector3 camera_position;
        float camera_yaw;
        float camera_pitch;
        float camera_speed;
        float camera_sensitivity;

        // Others for ImGUI
        string file = new string("");
        private bool firstMove = true;
        private Vector2 lastPos;
        private bool wireframe = false;
        private bool wireframeBefore = false;

        // Initialise the Scale Factor for ImGUI controller 
        float scaleFactorX = 1.0f;
        float scaleFactorY = 1.0f;


        // Main part 
        //

        // Button textures
        private int idButtonPrev;
        private int idButtonNext;

        int monsterNo = 815;
        string status = new string("Initial status");


        System.Numerics.Vector3 backgroundColor = new System.Numerics.Vector3(0.2f,0.2f,0.2f);

        bool flip = false;

        private GL_Model simpleModel;
        private GL_Shader simpleShader;

        private Camera camera;

        GameData gamedata;

        /// <summary>
        /// Create the application window
        /// </summary>
        public MainWindow(NativeWindowSettings nativeWindowSettings) : base(GameWindowSettings.Default, nativeWindowSettings)
        {

            InitGameData();

        }

        public void InitGameData()
        {

            NavigatorSettings.ReadAllSettings();

            // Load gamedata.mpk 
            gamedata = new GameData(NavigatorSettings.CONFIG_DAOC_LOCATION);

        }

        /// <summary>
        /// OnLoad when window is loaded for the first time
        /// </summary>
        /// 
        protected override void OnLoad()
        {
            base.OnLoad();

            updateTitle();

            GL.ClearColor(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            //simpleModel = new GL_Model("Resources/Models/penguin/penguin.nif");
            simpleModel = new GL_Model(new Model("Resources/Models/barrel/barrel.nif"), false, false);

            initCamera();

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            simpleShader = new GL_Shader("Shaders/shader.vert", "Shaders/shader.frag");


            // Now, enable the shader.
            // Just like the VBO, this is global, so every function that uses a shader will modify this one until a new one is bound instead.
            simpleShader.Use();

            // Setup is now complete! Now we move to the OnRenderFrame function to finally draw the triangle.

            idButtonPrev = GL_Texture.LoadTexture("Resources/images/prev.png");
            idButtonNext = GL_Texture.LoadTexture("Resources/images/next.png");

            // ImGUI controller init
            initImGUIController();
            applyImGUITheme();

        }

        private void initCamera()
        {
            camera = new Camera(new Vector3(25, -70, 80), Size.X / (float)Size.Y);

            camera.Yaw = -250;
            camera.Pitch = -32;
            camera_position.X = camera.Position.X;
            camera_position.Y = camera.Position.Y;
            camera_position.Z = camera.Position.Z;
            camera.Speed = 100.5f;
            camera.Sensitivity = 0.2f;
        }

        private void updateTitle()
        {
            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);
        }

        /// <summary>
        /// OnRenderFrame for the rendering loop
        /// </summary>
        /// 
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Enable(EnableCap.DepthTest);

            //Clear the previous frame
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Render the object(s)
            DoRender(e);

            SwapBuffers();

            //Clean up the opengl state back to how we got it
            GL.Disable(EnableCap.DepthTest);
        }

        private void DoRender(FrameEventArgs e)
        {
            // This clears the image, using what you set as GL.ClearColor earlier.
            GL.ClearColor(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // First we setup the shader, including the texture uniform and then call the Draw() method on the imported model to draw all the contained meshes
            simpleShader.Use();
            Matrix4 model = Matrix4.Identity;

            simpleShader.SetMatrix4("model", model);
            simpleShader.SetMatrix4("view", camera.GetViewMatrix());
            simpleShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            if (simpleModel != null)
                simpleModel.Draw(simpleShader);

            // ImGUI
            handleImGUIInterface(e);


        }

        /// <summary>
        /// OnResize to handle the window resizing 
        /// </summary>
        /// 
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            //GLFW.GetFramebufferSize(WindowPtr, out int width, out int height);

            // Update the opengl viewport
            Vector2i fb = this.FramebufferSize;
            GL.Viewport(0, 0, fb.X, fb.Y);

            // Tell ImGui of the new size
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        private void initImGUIController()
        {
            // Get the Scale Factor of the Monitor
            Vector2i fb = this.FramebufferSize;
            scaleFactorX = fb.X / ClientSize.X;
            scaleFactorY = fb.Y / ClientSize.Y;

            // Instanciate the ImGuiController with the right Scale Factor
            _controller = new ImGuiController(ClientSize.X, ClientSize.Y, scaleFactorX, scaleFactorY);
        }


        /// <summary>
        /// applyImGUITheme
        /// </summary>
        ///
        private void applyImGUITheme()
        {
            ImGuiStylePtr style = ImGui.GetStyle();
            style.Colors[(int)ImGuiCol.Button] = new System.Numerics.Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new System.Numerics.Vector4(0.5f, 0.5f, 0.5f, 0.2f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new System.Numerics.Vector4(0.5f, 0.5f, 0.5f, 0.4f);
        }


        private void handleImGUIInterface(FrameEventArgs e)
        {
            // ImGUI controller call for GUI 
            _controller.Update(this, (float)e.Time);

            // ImGUI Menus
            ImGui.BeginMainMenuBar();
            if (ImGui.BeginMenu("File"))
            {
                // Open Model 
                if (ImGui.MenuItem("Open"))
                {
                    openNIF();
                }

                // Open NPK file  
                if (ImGui.MenuItem("Open NPK"))
                {
                    openNPK();
                }
                if (ImGui.MenuItem("About"))
                {
                    about = true;
                }
                ImGui.Separator();
                if (ImGui.MenuItem("Quit"))
                {
                    app_quit = true;
                }

                ImGui.EndMenu();
            }

            // Setup DAOC folder
            if (ImGui.BeginMenu("Setup"))
            {
                if (ImGui.MenuItem("Define DAOC folder"))
                {
                    defineDAOCFolder();
                }
                ImGui.EndMenu();

            }


            // View windows
            if (ImGui.BeginMenu("View"))
            {
                if (ImGui.MenuItem("Camera"))
                {
                    camera_window = true;
                }
                if (ImGui.MenuItem("Options"))
                {
                    options_chooser = true;
                }
                if (ImGui.MenuItem("Monster"))
                {
                    monster = true;
                }
                ImGui.EndMenu();

            }
            ImGui.EndMainMenuBar();


            // GUI over rendering 
            if (about)
            {
                ImGui.OpenPopup("About");
                about = true;

                if (ImGui.BeginPopupModal("About", ref about))
                {

                    ImGui.Text("DAOC Navigator v1.0");
                    ImGui.Text("(c) 2026 DigitalBox");
                    if (ImGui.Button("Ok", new System.Numerics.Vector2(100, 20)))
                    {
                        about = false;
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.EndPopup();
                }
            }


            // Camera 
            if (camera_window)
            {
                camera_position.X = camera.Position.X;
                camera_position.Y = camera.Position.Y;
                camera_position.Z = camera.Position.Z;
                camera_yaw = camera.Yaw;
                camera_pitch = camera.Pitch;
                camera_speed = camera.Speed;
                camera_sensitivity = camera.Sensitivity;

                ImGui.Begin("Camera", ref camera_window);
                ImGui.InputFloat3("Camera position", ref camera_position);
                ImGui.SliderFloat("Yaw", ref camera_yaw, 0, 180);
                ImGui.SliderFloat("Pitch", ref camera_pitch, 0, 180);
                ImGui.SliderFloat("Sensitivity", ref camera_sensitivity, 0, 4);
                ImGui.SliderFloat("Speed", ref camera_speed, 0, 1000);

                ImGui.End();

                camera.Position = new Vector3(camera_position.X, camera_position.Y, camera_position.Z);
                camera.Yaw = camera_yaw;
                camera.Pitch = camera_pitch;
                camera.Speed = camera_speed;
                camera.Sensitivity = camera_sensitivity;
            }

            // Options 
            if (options_chooser)
            {
                ImGui.Begin("Options", ref options_chooser);
                ImGui.ColorEdit3("Background color", ref backgroundColor);
                //ImGui.Checkbox("Flip texture", ref flip);
                ImGui.Checkbox("Wireframe", ref wireframe);
                if (wireframe != wireframeBefore )
                {
                    updateWireframe();
                    wireframeBefore = wireframe;
                }

                ImGui.End();
            }


            // Monster Id
            if (monster)
            {
                ImGui.Begin("Monster Id", ref monster);
                ImGui.InputInt("No", ref monsterNo);

                // Action prev 
                if (ImGui.ImageButton("prev", new IntPtr(idButtonPrev), new System.Numerics.Vector2(72, 72)))
                {
                    PrevMonster();
                }

                // Action next 
                ImGui.SameLine();
                if (ImGui.ImageButton("next", new IntPtr(idButtonNext), new System.Numerics.Vector2(72, 72)))
                {
                    NextMonster();
                }
                ImGui.End();
            }

            // Status bar
            statusBar();

            //ImGui.ShowDemoWindow();
            //ImGui.ShowMetricsWindow();

            _controller.Render();

            ImGuiController.CheckGLError("End of frame");
        }


        private void statusBar()
        {
            ImGuiViewportPtr v = ImGui.GetMainViewport();
            System.Numerics.Vector2 bottom = new System.Numerics.Vector2(0, v.Size.Y - 20);
            ImGui.SetNextWindowPos(bottom);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(v.Size.X, 20));

            // Create window
            ImGui.SetNextWindowViewport(v.ID); // Enforce viewport so we don't create our own viewport when ImGuiConfigFlags_ViewportsNoMerge is set.
            ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.MenuBar;
            bool status_bar = true;
            //bool is_open = ImGui.Begin("##MainStatusBar", ref bbb, window_flags);
            ImGui.Begin("##MainStatusBar", ref status_bar, window_flags);
            ImGui.BeginMenuBar();
            ImGui.PopStyleVar(2);
            if (status != null)
                ImGui.Text("Status : " + status);
            ImGui.EndMenuBar();
        }





        /// <summary>
        /// OnTextInput
        /// </summary>
        ///
        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            _controller.PressChar((char)e.Unicode);
        }

        /// <summary>
        /// OnUpdateFrame
        /// </summary>
        ///
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if(!IsFocused)
            {
                return;
            }

            //Check keyboard 
            CheckKeyboard(e);

            //Check mouse 
            CheckMouse();

        }

        private void CheckKeyboard(FrameEventArgs e)
        {
            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape) || app_quit == true)
            {
                Close();
            }

            // Actions prev / next
            if (input.IsKeyPressed(Keys.Left))
            {
                if (monsterNo > 0)
                {
                    monsterNo--;
                    getMonster(monsterNo);
                }
            }
            if (input.IsKeyPressed(Keys.Right))
            {
                if (monsterNo < gamedata.getMaxMonsterId())
                {
                    monsterNo++;
                    getMonster(monsterNo);
                }
            }

            if (input.IsKeyDown(Keys.W))
            {
                camera.Position += camera.Front * camera.Speed * (float)e.Time; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                camera.Position -= camera.Front * camera.Speed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                camera.Position -= camera.Right * camera.Speed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                camera.Position += camera.Right * camera.Speed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                camera.Position += camera.Up * camera.Speed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.B))
            {
                camera.Position -= camera.Up * camera.Speed * (float)e.Time; // Down
            }

        }


        private void CheckMouse()
        {
            var mouse = MouseState;

            if (mouse.IsButtonDown(MouseButton.Right))
            {
                if (firstMove)
                {
                    lastPos = new Vector2(mouse.X, mouse.Y);
                    firstMove = false;
                }
                else
                {
                    var deltaX = mouse.X - lastPos.X;
                    var deltaY = - (mouse.Y - lastPos.Y);
                    lastPos = new Vector2(mouse.X, mouse.Y);

                    camera.Yaw += deltaX * camera.Sensitivity;
                    camera.Pitch -= deltaY * camera.Sensitivity;
                }
            }

            if (mouse.IsButtonReleased(MouseButton.Right))
            {
                firstMove = true;

                CursorState = CursorState.Normal;
            }
        }



        private void updateWireframe()
        {
            if (wireframe == false)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
        }

        /// <summary>
        /// OnMouseWheel
        /// </summary>
        ///
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            camera.Fov -= e.OffsetY;

            _controller.MouseScroll(e.Offset);
        }



        /// <summary>
        /// getMonster 
        /// </summary>
        /// 
        private void getMonster(int monsterNo)
        {
            Monster m = (Monster)gamedata.MonstersMapping["" + monsterNo];

            if (m != null)
            {
                // Etract the texture from the gamedata.mpk
                string file = Path.Combine(NavigatorSettings.CONFIG_DAOC_LOCATION, "figures", m.NIFFilename + ".nif");

                SkinSet figureMaterial = m.SkinSet;
                if (figureMaterial != null)
                    Console.WriteLine("monster skin archive =" + figureMaterial.BodySkin.ArchiveNum + ", skin file = " + figureMaterial.BodySkin.Filename);
                try
                {
                    simpleModel = new GL_Model(new Model(file, figureMaterial), flip, true);

                    status = Path.GetFileName(file) + " loaded";
                }
                catch (Exception ex)
                {
                    status = ex.ToString();
                }
            }
        }

        /// <summary>
        /// defineDAOCFolder 
        /// </summary>
        /// 
        private void defineDAOCFolder()
        {
            DialogResult result = Dialog.FolderPicker();
            if (result != null)
            {
                Console.WriteLine("Folder : " + result.Path);
                NavigatorSettings.AddUpdateAppSettings( NavigatorSettings.CONFIGNAME_DAOC_LOCATION, result.Path);
                status = "DAOC folder : " + NavigatorSettings.CONFIG_DAOC_LOCATION;
            }
        }

        /// <summary>
        /// openNIF 
        /// </summary>
        /// 
        private void openNIF()
        {
            DialogResult result = Dialog.FileOpen();
            if (result != null)
            {
                file = result.Path;
                if (file != null)
                {

                    try
                    {
                        simpleModel = new GL_Model(new Model(file), flip, false);
                        status = Path.GetFileName(file) + " loaded / DAOC folder = " + NavigatorSettings.CONFIG_DAOC_LOCATION;
                    }
                    catch (Exception ex)
                    {
                        status = ex.ToString();
                    }
                }

            }
        }

        /// <summary>
        /// openNPK 
        /// </summary>
        /// 
        private void openNPK()
        {
            DialogResult result = Dialog.FileOpen("npk");
            if (result != null)
            {
                file = result.Path;
                if (file != null)
                {
                    // Testing NPK reading 
                    string initialPath = Path.GetDirectoryName(file);
                    string curPath = initialPath;
                    string[] components = file.ToLower().Split(Path.DirectorySeparatorChar);
                    string nifComponent = null;

                    foreach (var comp in components)
                    {
                        if (nifComponent != null)
                        {
                            nifComponent = Path.Combine(nifComponent, comp);
                        }
                        else
                        {
                            curPath = Path.Combine(curPath, comp);
                            if (comp.EndsWith(".npk") || comp.EndsWith(".mpk"))
                                nifComponent = comp; 
                        }
                    }
                    string fileNifComponent = nifComponent.Replace("npk", "nif");

                    PAKFile mpk = new PAKFile(file);
                    MemoryStream fstream = new MemoryStream();
                    mpk.ExtractFile(fileNifComponent, fstream);

                    simpleModel = new GL_Model(new Model(fstream, initialPath), false, false);

                }

            }
        }

        /// <summary>
        /// NextMonster 
        /// </summary>
        /// 
        private void NextMonster()
        {
            if (monsterNo < gamedata.getMaxMonsterId())
            {
                monsterNo++;
                getMonster(monsterNo);
            }
        }

        /// <summary>
        /// PrevMonster 
        /// </summary>
        /// 
        private void PrevMonster()
        {
            if (monsterNo > 0)
            {
                monsterNo--;
                getMonster(monsterNo);
            }
        }



    }
}
