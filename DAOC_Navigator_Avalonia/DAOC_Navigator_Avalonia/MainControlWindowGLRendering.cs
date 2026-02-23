/*
 * DAOC Navigator - The free open source DAOC game navigator
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
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Avalonia;
using Avalonia.Media;
using ReactiveUI;
using Window = Avalonia.Controls.Window;
using Camera = DAOC_Navigator_Core.WorldObjects.Camera;
using DAOC_Navigator_Core.GL_Rendering;
using Avalonia.Input;
using Avalonia.OpenGL;
using DAOC_Navigator_Core;
using System.IO;
using Avalonia.Controls;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using DAOC_Navigator_Core.WorldObjects;
using DAOC_Navigator_Core.Game;

namespace DAOC_Navigator_Avalonia
{
    public class MainControlWindowGLRendering : BaseTkOpenGlControl
    {
        // Avalonia part
        //

        public static FilePickerFileType ModelALL { get; } = new("All NIF")
        {
            Patterns = new[] { "*.*" }
        };
        public static FilePickerFileType ModelNIF { get; } = new("All NIF")
        {
            Patterns = new[] { "*.nif" }
        };
        public static FilePickerFileType ModelNPK { get; } = new("All NPK")
        {
            Patterns = new[] { "*.npk" }
        };

        string prevNpkFile = new string("");
        string npkFile = new string("");

        string prevNifFile = new string("");
        string nifFile = new string("");

        string prevModelFile = new string("");
        string modelFile = new string("");

        private System.Numerics.Vector3 _cameraFront;

        private bool _isDragging;
        private Point _lastPos;

        // Avalonia properties
        public static readonly StyledProperty<float> YawProperty =
            AvaloniaProperty.Register<MainControlWindowGLRendering, float>(nameof(Yaw));

        public float Yaw
        {
            get => GetValue(YawProperty);
            set => SetValue(YawProperty, value);
        }

        public static readonly StyledProperty<float> PitchProperty =
            AvaloniaProperty.Register<MainControlWindowGLRendering, float>(nameof(Pitch));

        public float Pitch
        {
            get => GetValue(PitchProperty);
            set => SetValue(PitchProperty, value);
        }

        public static readonly StyledProperty<float> SensitivityProperty =
            AvaloniaProperty.Register<MainControlWindowGLRendering, float>(nameof(Sensitivity));

        public float Sensitivity
        {
            get => GetValue(SensitivityProperty);
            set => SetValue(SensitivityProperty, value);
        }

        public static readonly StyledProperty<float> SpeedProperty =
            AvaloniaProperty.Register<MainControlWindowGLRendering, float>(nameof(Speed));

        public float Speed
        {
            get => GetValue(SpeedProperty);
            set => SetValue(SpeedProperty, value);
        }

        public static readonly StyledProperty<float> CameraXProperty =
            AvaloniaProperty.Register<MainControlWindowGLRendering, float>(nameof(CameraX));

        public float CameraX
        {
            get => GetValue(CameraXProperty);
            set => SetValue(CameraXProperty, value);
        }

        public static readonly StyledProperty<float> CameraYProperty =
            AvaloniaProperty.Register<MainControlWindowGLRendering, float>(nameof(CameraY));

        public float CameraY
        {
            get => GetValue(CameraYProperty);
            set => SetValue(CameraYProperty, value);
        }

        public static readonly StyledProperty<float> CameraZProperty =
            AvaloniaProperty.Register<MainControlWindowGLRendering, float>(nameof(CameraZ));

        public float CameraZ
        {
            get => GetValue(CameraZProperty);
            set => SetValue(CameraZProperty, value);
        }

        public static readonly StyledProperty<bool> WireFrameProperty =
            AvaloniaProperty.Register<MainControlWindowGLRendering, bool>(nameof(WireFrame));

        public bool WireFrame
        {
            get => GetValue(WireFrameProperty);
            set => SetValue(WireFrameProperty, value);
        }

        public static readonly StyledProperty<string> MyActionProperty =
    AvaloniaProperty.Register<MainControlWindowGLRendering, string>(nameof(MyAction));

        public string MyAction
        {
            get => GetValue(MyActionProperty);
            set => SetValue(MyActionProperty, value);
        }

        public static readonly StyledProperty<string> StatusProperty =
    AvaloniaProperty.Register<MainControlWindowGLRendering, string>(nameof(Status));

        public string Status
        {
            get => GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public static readonly StyledProperty<int> MonsterIdProperty =
AvaloniaProperty.Register<MainControlWindowGLRendering, int>(nameof(MonsterId));

        public int MonsterId
        {
            get => GetValue(MonsterIdProperty);
            set => SetValue(MonsterIdProperty, value);
        }

        // Handle Avalonia Color Picker property and update 

        public static readonly StyledProperty<Color> SelectedColorProperty =
    AvaloniaProperty.Register<MainControlWindowGLRendering, Color>(nameof(SelectedColor));

        public Color SelectedColor
        {
            get => GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }



        // Main part 
        //

        // Button textures
        private int idButtonPrev;
        private int idButtonNext;

        int monsterNo = 0;
        int prevMonsterNo = -1; // initialised to -1 so the first frame always triggers getMonster()

        System.Numerics.Vector3 backgroundColor;

        bool flip = false;

        private GL_Model? simpleModel;
        private GL_Shader? simpleShader;

        private Camera camera;

        GameData? gamedata;


        /// <summary>
        /// Create the application window
        /// </summary>
        public MainControlWindowGLRendering()
        {
            InitGameData();

            camera = new Camera(new Vector3(CameraX, CameraY, CameraZ), 1.3f); // TODO improve init 

            InitAvaloniaPickerHandler();
        }


        public void InitGameData()
        {
            NavigatorSettings.ReadAllSettings();

            // Load gamedata.mpk 
            gamedata = new GameData(NavigatorSettings.CONFIG_DAOC_LOCATION);

        }


        //OpenTkInit is called once when the control is created
        protected override void OpenTkInit() // OnLoad()
        {
            updateTitle();

            GL.ClearColor(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            simpleModel = new GL_Model(new Model("Resources/Models/barrel/barrel.nif"), false, true);

            initCamera();

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            simpleShader = new GL_Shader("Shaders/shader.vert", "Shaders/shader.frag");

            simpleShader.Use();

            idButtonPrev = GL_Texture.LoadTexture("Resources/images/prev.png");
            idButtonNext = GL_Texture.LoadTexture("Resources/images/next.png");

        }

        private void initCamera()
        {
            camera = new Camera(new Vector3(CameraX, CameraY, CameraZ), 1.3f);

            // TODO improve the default values
            MonsterId = 815;
            monsterNo = MonsterId;

            camera.Yaw = Yaw;
            camera.Pitch = Pitch;
            camera.Speed = Speed;
            camera.Sensitivity = Sensitivity;
        }

        private void updateTitle()
        {
            //TODO
        }


        //OpenTkRender is called once a frame. The aspect ratio and keyboard state are configured prior to this being called.
        protected override void OpenTkRender() // OnRenderFrame()
        {
            GL.Enable(EnableCap.DepthTest);

            //Clear the previous frame
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Check Avalonia actions
            CheckActions();

            //Check Avalonia keyboard 
            CheckAvaloniaKeyboard();

            //Render the object(s)
            DoRender();

            //Clean up the opengl state back to how we got it
            GL.Disable(EnableCap.DepthTest);
        }

        private void DoRender()
        {
            // Important : all logical and call to methods must be done inside the render loop in order to not loose the GL Context from OpenTK

            GL.ClearColor(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (simpleShader != null && simpleModel != null)
            {
                simpleShader.Use();
                Matrix4 model = Matrix4.Identity;

                // Avalonia update camera 
                updateCameraProperties();

                simpleShader.SetMatrix4("model", model);
                simpleShader.SetMatrix4("view", camera.GetViewMatrix());
                simpleShader.SetMatrix4("projection", camera.GetProjectionMatrix());

                simpleModel.Draw(simpleShader);

            }
        }

        private void InitAvaloniaPickerHandler()
        {
            // Avalonia Handle Color Picker event when value is changed
            this.WhenAnyValue(x => x.SelectedColor)
                .Subscribe(color =>
                {
                    float r = SelectedColor.R;
                    float g = SelectedColor.G;
                    float b = SelectedColor.B;

                    backgroundColor = new System.Numerics.Vector3(r / 255, g / 255, b / 255);
                });
        }

        private void CheckActions()
        {

            if (monsterNo != prevMonsterNo)
            {
                getMonster(monsterNo);
                prevMonsterNo = monsterNo;
            }

            if (npkFile != prevNpkFile) // TODO improve
            {
                loadNPK();
                prevNpkFile = npkFile;
            }

            if (nifFile != prevNifFile) // TODO improve
            {
                loadNIF();
                prevNifFile = nifFile;
            }

            if (modelFile != prevModelFile) // TODO improve
            {
                loadModel();
                prevModelFile = modelFile;
            }

            if (WireFrame == false)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
        }

        //OpenTkTeardown is called when the control is being destroyed
        protected override void OpenTkTeardown()
        {
            Console.WriteLine("UI: Tearing down gl component");
        }

        //Demonstrating use of the Avalonia keyboard state provided by OpenTKAvalonia to control the camera 
        private void CheckAvaloniaKeyboard()
        {
            var effectiveSpeed = 1;

            if (KeyboardState.IsKeyDown(Key.LeftShift))
            {
                effectiveSpeed *= 2;
            }

            if (KeyboardState.IsKeyDown(Key.Z))
            {
                camera.Position += camera.Front * camera.Speed; // Forward
            }

            if (KeyboardState.IsKeyDown(Key.S))
            {
                camera.Position -= camera.Front * camera.Speed; //Backwards
            }

            if (KeyboardState.IsKeyDown(Key.Q))
            {
                camera.Position -= camera.Right * camera.Speed; // Left
            }

            if (KeyboardState.IsKeyDown(Key.D))
            {
                camera.Position += camera.Right * camera.Speed; // Right
            }

            if (KeyboardState.IsKeyDown(Key.Space))
            {
                camera.Position += camera.Up * camera.Speed; // Up
            }

            if (KeyboardState.IsKeyDown(Key.B))
            {
                camera.Position -= camera.Up * camera.Speed; // Down
            }

            updateCameraProperty(camera.Position);
        }


        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            _isDragging = true;
            e.Pointer.Capture(this);
            _lastPos = e.GetPosition(null);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            _isDragging = false;
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (!_isDragging)
                return;

            var pos = e.GetPosition(null);

            float deltaX = -(float)(pos.X - _lastPos.X);
            float deltaY = (float)(pos.Y - _lastPos.Y);
            _lastPos = pos;

            Yaw -= deltaX * camera.Sensitivity;

            if (Pitch > 89.0f)
            {
                Pitch = 89.0f;
            }
            else if (Pitch < -89.0f)
            {
                Pitch = -89.0f;
            }
            else
            {
                Pitch += deltaY * camera.Sensitivity;
            }

            UpdateCameraFront();
        }

        private void updateCameraProperty(Vector3 v)
        {
            CameraX = v.X;
            CameraY = v.Y;
            CameraZ = v.Z;
        }

        private void updateCameraProperties()
        {
            camera.Yaw = Yaw;
            camera.Pitch = Pitch;
            camera.Speed = Speed;
            camera.Sensitivity = Sensitivity;
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            camera.Fov -= (float)e.Delta.Y;
        }

        private void UpdateCameraFront()
        {
            _cameraFront.X = (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(Yaw));
            _cameraFront.Y = (float)Math.Sin(MathHelper.DegreesToRadians(Pitch));
            _cameraFront.Z = -(float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(Yaw));
            _cameraFront = System.Numerics.Vector3.Normalize(_cameraFront);
        }

        protected sealed override void OnOpenGlRender(GlInterface gl, int fb)
        {
            UpdateCameraFront();
            base.OnOpenGlRender(gl, fb);
        }


        /// <summary>
        /// getMonster 
        /// </summary>
        private void getMonster(int monsterNo)
        {
            if (gamedata != null && gamedata.MonstersMapping["" + monsterNo] is Monster mons)
            {
                if (mons != null)
                {
                    // Get the NIF model full filename
                    string file = Path.Combine(NavigatorSettings.CONFIG_DAOC_LOCATION, GameData.FIGURES_FOLDER, mons.NIFFilename + ".nif");

                    // In case of NIF model with specific head 
                    if (mons.Head != null)
                    {
                        // Retrieve the Head NIF from MPK archive
                        int num;
                        string archiveNum = "";
                        if (mons.Head.ArchiveNum != " " && mons.Head.ArchiveNum != "")
                        {
                            num = Int32.Parse(mons.Head.ArchiveNum);
                            archiveNum = num.ToString("000");
                        }

                        string headArchiveFile = Path.Combine(NavigatorSettings.CONFIG_DAOC_LOCATION, GameData.FIGURES_FOLDER, GameData.FIG3_FOLDER, "fig" + archiveNum + ".mpk");

                        string? initialPath = Path.GetDirectoryName(headArchiveFile);
                        string? curPath = initialPath;
                        string[] components = headArchiveFile.ToLower().Split(Path.DirectorySeparatorChar);
                        string? nifComponent = null;

                        if (curPath != null)
                        {
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
                                        nifComponent = comp; //TODO check "";
                                }
                            }
                        }

                        if (nifComponent != null && initialPath != null)
                        {
                            string fileNifComponent = mons.Head.NIFFile + ".nif";

                            MemoryStream fstream = new MemoryStream();
                            using (var mpk = new PAKFile(headArchiveFile))
                                mpk.ExtractFile(fileNifComponent, fstream);

                            try
                            {
                                // TODO check the reason why the model face texture is not displayed
                                simpleModel = new GL_Model(new Model(fstream, Path.Combine(initialPath, fileNifComponent), mons.SkinSet), false, false);

                                Status = Path.GetFileName(file) + " loaded";
                            }
                            catch (Exception ex)
                            {
                                Status = ex.ToString();
                            }
                        }
                    }
                    // Model without dedicated head 
                    else
                    {
                        SkinSet figureSkin = mons.SkinSet;
                        if (figureSkin != null)
                        {
                            Console.WriteLine("monster skin archive =" + figureSkin.BodySkin.ArchiveNum + ", skin file = " + figureSkin.BodySkin.Filename);
                            try
                            {
                                simpleModel = new GL_Model(new Model(file, figureSkin), flip, true);

                                Status = Path.GetFileName(file) + " loaded";
                            }
                            catch (Exception ex)
                            {
                                Status = ex.ToString();
                            }
                        }
                    }
                }
            }
        }

        public async Task defineDAOCFolder()
        {
            var win = TopLevel.GetTopLevel(this) as Window;
            string? selectedFolder = null;

            if (win != null)
            {
                var folders = await win.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
                {
                    Title = "Select DAOC folder"
                });

                if (folders != null && folders.Count > 0)
                {
                    IStorageFolder f = folders[0];
                    Uri uriFolder = f.Path;
                    selectedFolder = uriFolder.LocalPath;
                }
            }

            if (!string.IsNullOrEmpty(selectedFolder))
            {
                NavigatorSettings.CONFIG_DAOC_LOCATION = selectedFolder;
                Status = "DAOC folder : " + NavigatorSettings.CONFIG_DAOC_LOCATION;

                NavigatorSettings.AddUpdateAppSettings(NavigatorSettings.CONFIGNAME_DAOC_LOCATION, NavigatorSettings.CONFIG_DAOC_LOCATION);

                InitGameData(); //TODO improve 
            }
            else
            {
                Console.WriteLine("No folder selected");
            }
        }


        public async void openModel()
        {
            var win = TopLevel.GetTopLevel(this) as Window;

            if (win != null)
            {
                var files = await win.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                {
                    Title = "Select model file",
                    FileTypeFilter = new[] { ModelALL }
                });

                if (files != null && files.Count > 0)
                {
                    IStorageFile f = files[0];
                    Uri uriFile = f.Path;
                    modelFile = uriFile.LocalPath;
                }
            }
        }

        private void loadModel()
        {
            if (modelFile != null)
            {
                try
                {
                    simpleModel = new GL_Model(new Model(modelFile), false, false);
                    Status = Path.GetFileName(nifFile) + " loaded";
                }
                catch (Exception ex)
                {
                    Status = ex.ToString();
                }
            }
        }

        public async void openNIF()
        {
            var win = TopLevel.GetTopLevel(this) as Window;

            if (win != null)
            {
                var files = await win.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                {
                    Title = "Select NIF file",
                    FileTypeFilter = new[] { ModelNIF }
                });

                if (files != null && files.Count > 0)
                {
                    IStorageFile f = files[0];
                    Uri uriFile = f.Path;
                    nifFile = uriFile.LocalPath;
                }
            }
        }

        private void loadNIF()
        {
            if (nifFile != null)
            {
                try
                {
                    simpleModel = new GL_Model(new Model(nifFile), false, false);
                    Status = Path.GetFileName(nifFile) + " loaded";
                }
                catch (Exception ex)
                {
                    Status = ex.ToString();
                }
            }
        }


        public async void openNPK()
        {
            var win = TopLevel.GetTopLevel(this) as Window;

            if (win != null)
            {
                var files = await win.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                {
                    Title = "Select NPK file",
                    FileTypeFilter = new[] { ModelNPK }
                });

                if (files != null && files.Count > 0)
                {
                    IStorageFile f = files[0];
                    Uri uriFile = f.Path;
                    npkFile = uriFile.LocalPath;
                }
            }
        }


        private void loadNPK()
        {
            if (npkFile != null)
            {
                string? initialPath = Path.GetDirectoryName(npkFile);
                string? curPath = initialPath;
                string[] components = npkFile.ToLower().Split(Path.DirectorySeparatorChar);
                string? nifComponent = null;

                if (curPath != null)
                {
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
                                nifComponent = comp; //TODO check "";
                        }
                    }
                }

                if (nifComponent != null && initialPath != null)
                {
                    string fileNifComponent = nifComponent.Replace("npk", "nif");

                    MemoryStream fstream = new MemoryStream();
                    using (var mpk = new PAKFile(npkFile))
                        mpk.ExtractFile(fileNifComponent, fstream);

                    simpleModel = new GL_Model(new Model(fstream, Path.Combine(initialPath, fileNifComponent)), false, false);
                }
            }
        }


        public void SetMonster(string monster)
        {
            MonsterId = Int32.Parse(monster);
            monsterNo = MonsterId;
        }

        public void NextMonster()
        {
            if (gamedata != null && monsterNo < gamedata.getMaxMonsterId())
            {
                monsterNo++;
                MonsterId = monsterNo;
                Console.WriteLine("monsterId=" + monsterNo);
            }
        }

        public void PrevMonster()
        {
            if (monsterNo > 0)
            {
                monsterNo--;
                MonsterId = monsterNo;
                Console.WriteLine("monsterId=" + monsterNo);
            }
        }
    }
}