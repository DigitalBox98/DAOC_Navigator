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

using OpenTK.Mathematics;
using DAOC_Navigator_Core.Game;
using DAOC_Navigator_Core.WorldObjects;

namespace DAOC_Navigator_Core.GL_Rendering
{

    /// <summary>
    /// Class <c>GL_Model</c> represents an OpenGL Model
    /// composed of a list of GL_Mesh and a list of GL_Texture.
    /// </summary>
    /// 
    public class GL_Model
    {
        /// <value>
        /// Property <c>meshes</c> represents the list of meshes for the model 
        /// </value>
        private List<GL_Mesh> meshes = new List<GL_Mesh>();

        /// <value>
        /// Property <c>textures_loaded</c> represents the list of loaded textures for the model
        /// optimization is to make sure textures aren't loaded more than once
        /// </value>
        private List<GL_Texture> textures_loaded = new List<GL_Texture>();

        bool flip_vertically = false;


        /// <summary>
        /// This constructor initializes the new GL_Model with 
        /// (<paramref name="path"/>,<paramref name="flip"/>).
        /// </summary>
        /// <param name="model">The model to convert to GL_Model.</param>
        /// <param name="flip">To flip the textures horizontally.</param>
        public GL_Model(Model model, bool flip, bool materialType)
        {
            setupModel(flip);

            processModelforOpenGL(model, materialType);
        }


        /// <summary>
        /// This method setup the Model
        /// </summary>
        /// <param name="flip">To flip the textures horizontally.</param>
        ///
        private void setupModel(bool flip)
        {
            flip_vertically = flip;
        }


        /// <summary>
        /// This method process the model up to the NIFParser for OpenGL 
        /// </summary>
        /// <param name="parser">Parser used to read the model</param>
        /// <param name="figureMaterial">The Skin used for the model</param>
        ///
        public void processModelforOpenGL(Model model, bool materialTypeMode)
        {
            // Convert Shapes to Mesh / Texture
            List<GL_Mesh> resultMeshes = new List<GL_Mesh>();
            ShapeMaterial[] mater = model.Materials; // TODO Convert SkinSet to Material outside ? 

            foreach (Shape shape in model.Shapes)
            {
                List<Vertex> uniqueVertices = new List<Vertex>();
                List<uint> vertexIndices = new List<uint>();

                uint matid = shape.MaterialId;
                List<GL_Texture> texturesList = new List<GL_Texture>();

                // Standard NIF case : texture indicated in file's model, each shape having a material
                if (mater != null && mater.Length != 0 && model.SkinSet == null)
                {
                    ShapeMaterial sMaterial = new ShapeMaterial(mater[matid].Filename);
                    texturesList = loadMaterialToTextures(sMaterial, model.Directory);
                }
                // Game NIF case : texture retrieved from skin MPK archive file (skinxxx.mpk) 
                else if (model.SkinSet != null)
                {
                    string skinDir = Path.Combine(NavigatorSettings.CONFIG_DAOC_LOCATION, "figures") + Path.DirectorySeparatorChar;
                    if (shape.MaterialType.Equals(ShapeMaterial.SCENE) && model.SkinSet.BodySkin != null)
                        texturesList = loadSkinToTextures(model.SkinSet.BodySkin, skinDir);
                    else if (shape.MaterialType.Equals(ShapeMaterial.BODY) && model.SkinSet.BodySkin != null)
                        texturesList = loadSkinToTextures(model.SkinSet.BodySkin, model.Directory);

                    else if (shape.MaterialType.Equals(ShapeMaterial.HEAD) && model.SkinSet.BodySkin != null)
                        texturesList = loadSkinToTextures(model.SkinSet.BodySkin, model.Directory);

                    else if (shape.MaterialType.Equals(ShapeMaterial.ARMS) && model.SkinSet.ArmsSkin != null)
                        texturesList = loadSkinToTextures(model.SkinSet.ArmsSkin, model.Directory);
                    else if (shape.MaterialType.Equals(ShapeMaterial.GLOVES) && model.SkinSet.GlovesSkin != null)
                        texturesList = loadSkinToTextures(model.SkinSet.GlovesSkin, model.Directory);
                    else if (shape.MaterialType.Equals(ShapeMaterial.LBODY) && model.SkinSet.LbodySkin != null)
                        texturesList = loadSkinToTextures(model.SkinSet.LbodySkin, model.Directory);
                    else if (shape.MaterialType.Equals(ShapeMaterial.LEGS) && model.SkinSet.LegsSkin != null)
                        texturesList = loadSkinToTextures(model.SkinSet.LegsSkin, model.Directory);
                    else if (shape.MaterialType.Equals(ShapeMaterial.BOOTS) && model.SkinSet.BootsSkin != null)
                        texturesList = loadSkinToTextures(model.SkinSet.BootsSkin, model.Directory);
                    else if (shape.MaterialType.Equals(ShapeMaterial.CLOAK) && model.SkinSet.CloakSkin != null)
                        texturesList = loadSkinToTextures(model.SkinSet.CloakSkin, model.Directory);
                }

                //Convert s.Triangles to List<uint> (vertexIndices)
                foreach (Polygon p in shape.Triangles)
                {
                    vertexIndices.Add(p.idx1);
                    vertexIndices.Add(p.idx2);
                    vertexIndices.Add(p.idx3);
                }

                // Convert s.Vertices to List<Vertex> (uniqueVertices)
                // Convert s.UVSets to List<Vertex> 
                int vi = 0;
                foreach (Vector3 v in shape.Vertices)
                {
                    Vertex vx = Vertex.VectorToVertex(v);
                    //TODO: review if this use case is normal
                    if (shape.UVSets.Length == 1)
                        vx.TexCoords = new Vector2(shape.UVSets[0].X, shape.UVSets[0].Y);
                    else 
                        vx.TexCoords = new Vector2(shape.UVSets[vi].X, shape.UVSets[vi].Y);
                    uniqueVertices.Add(vx);
                    vi++;
                }

                if (materialTypeMode == true && (shape.MaterialType.Equals(ShapeMaterial.OTHER) || shape.MaterialType.Equals(ShapeMaterial.CLOAK) )) // || shape.MaterialType.Equals(ShapeMaterial.HEAD) ))
                    ;  // skip this kind of material
                else
                    resultMeshes.Add(new GL_Mesh(uniqueVertices.ToArray(), vertexIndices.ToArray(), texturesList));
            }

            meshes = resultMeshes;
        }


        /// <summary>
        /// This method load the material for a given material and returns the GL_Texture in a list
        /// Checks all NIF material textures of a given type and loads the textures if they're not loaded yet.
        /// </summary>
        /// <param name="mat">ShapeMaterial to read</param>
        ///
        private List<GL_Texture> loadMaterialToTextures(ShapeMaterial mat, string directory)
        {
            string typeName = "texture_diffuse";

            List<GL_Texture> textures = new List<GL_Texture>();

            // check if texture was loaded before and if so, continue to next iteration: skip loading a new texture
            bool skip = false;
            for (int j = 0; j < textures_loaded.Count; j++)
            {
                if (textures_loaded[j].path.CompareTo(mat.Filename) == 0)
                {
                    textures.Add(textures_loaded[j]);
                    skip = true; // a texture with the same filepath has already been loaded, continue to next one. (optimization)
                    break;
                }
            }
            if (!skip && mat != null)
            {
                // if texture hasn't been loaded already, load it
                GL_Texture texture = GL_Texture.LoadFromFile(mat.Filename, directory, typeName, flip_vertically);
                textures.Add(texture);
                textures_loaded.Add(texture);  // store it as texture loaded for entire model, to ensure we won't unnecessary load duplicate textures.
            }

            return textures;
        }

        /// <summary>
        /// This method load the material for a given skin and returns the GL_Texture in a list
        /// Checks all NIF material textures of a given type and loads the textures if they're not loaded yet.
        /// </summary>
        /// <param name="skin">Skin to read with the archive num</param>
        ///
        private List<GL_Texture> loadSkinToTextures(Skin skin, string directory)
        {
            string typeName = "texture_diffuse";

            List<GL_Texture> textures = new List<GL_Texture>();

            // check if texture was loaded before and if so, continue to next iteration: skip loading a new texture
            bool skip = false;
            for (int j = 0; j < textures_loaded.Count; j++)
            {

                if (textures_loaded[j].path.CompareTo(skin.Filename) == 0)
                {
                    textures.Add(textures_loaded[j]);
                    skip = true; // a texture with the same filepath has already been loaded, continue to next one. (optimization)
                    break;
                }
            }
            if (!skip && skin != null)
            {
                // if texture hasn't been loaded already, load it
                GL_Texture texture = GL_Texture.LoadFromArchive(skin, directory, typeName, flip_vertically);
                textures.Add(texture);
                textures_loaded.Add(texture);  // store it as texture loaded for entire model, to ensure we won't unnecessary load duplicate textures.
            }

            return textures;
        }



        /// <summary>
        /// This method draw the model with the corresponding shader 
        /// </summary>
        /// <param name="shader">GL_Shader used to draw the model</param>
        ///
        public void Draw(GL_Shader shader)
        {
            foreach (GL_Mesh mesh in meshes)
            {
                mesh.Draw(shader);
            }
        }


        /// <summary>
        /// This method processes an Assimp mesh
        /// </summary>
        /// <param name="mesh">AssimpMesh to process</param>
        /// <param name="scene">Scene to process</param>
        ///
        /*private GL_Mesh ProcessAssimpMeshForOpenGL(AssimpMesh mesh, Scene scene)
        {
            // Data to fill
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            List<GL_Texture> textures = new List<GL_Texture>();


            // Walk through each of the mesh's vertices
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vertex vertex = new Vertex();

                // Positions
                vertex.Position = mesh.Vertices[i].ConvertAssimpVector3(); //transformedPosition;

                // Normals
                if (mesh.HasNormals)
                {
                    vertex.Normal = mesh.Normals[i].ConvertAssimpVector3(); //transformedNormal;
                }

                // Texture coordinates
                if (mesh.HasTextureCoords(0)) // Does the mesh contain texture coordinates?
                {
                    Vector2 vec;
                    vec.X = mesh.TextureCoordinateChannels[0][i].X;
                    vec.Y = mesh.TextureCoordinateChannels[0][i].Y;
                    vertex.TexCoords = vec;

                }
                else vertex.TexCoords = new Vector2(0.0f, 0.0f);

                vertices.Add(vertex);
            }

            // Now walk through each of the mesh's faces (a face is a group of vertices that form a triangle, quadrilateral, or ngon) and retrieve the corresponding vertex indices.
            // All of the faces should be triangles since we used PostProcessSteps.Triangulate during the Assimp import
            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                    indices.Add((uint)face.Indices[j]);
            }

            // process materials
            Material material = scene.Materials[mesh.MaterialIndex];

            // we assume a convention for sampler names in the shaders. Each diffuse texture should be named
            // as 'texture_diffuseN' where N is a sequential number ranging from 1 to MAX_SAMPLER_NUMBER. 
            // Same applies to other texture as the following list summarizes:
            // diffuse: texture_diffuseN
            // specular: texture_specularN
            // normal: texture_normalN

            // 1. diffuse maps
            List<GL_Texture> diffuseMaps = loadAssimpMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
            textures.AddRange(diffuseMaps);
            // 2. specular maps
            List<GL_Texture> specularMaps = loadAssimpMaterialTextures(material, TextureType.Specular, "texture_specular");
            textures.AddRange(specularMaps);
            // 3. normal maps
            List<GL_Texture> normalMaps = loadAssimpMaterialTextures(material, TextureType.Height, "texture_normal");
            textures.AddRange(normalMaps);
            // 4. height maps
            List<GL_Texture> heightMaps = loadAssimpMaterialTextures(material, TextureType.Ambient, "texture_height");
            textures.AddRange(heightMaps);

            // If we were targeting .net 5+ we could use
            //      return new Mesh(CollectionsMarshal.AsSpan(vertices), CollectionsMarshal.AsSpan(indices));
            // to avoid making a copy of all the vertex data.
            return new GL_Mesh(vertices.ToArray(), indices.ToArray(), textures);
        }*/


        /// <summary>
        /// This method load the Assimp textures material 
        /// </summary>
        /// <param name="mat">Material to load</param>
        /// <param name="type">TextureType to load</param>
        /// <param name="typeName">Type name of the texture (texture_diffuse, texture_specular, texture_normal)</param>
        ///
        /// Checks all material textures of a given type and loads the textures if they're not loaded yet.
        /// The required info is returned as a GL_Texture.
        /// 
        /*private List<GL_Texture> loadAssimpMaterialTextures(Material mat, TextureType type, string typeName)
        {
            List<GL_Texture> textures = new List<GL_Texture>();

            
            for (int i = 0; i < mat.GetMaterialTextureCount(type); i++)
            {
                TextureSlot str;
                mat.GetMaterialTexture(type, i, out str);
                // check if texture was loaded before and if so, continue to next iteration: skip loading a new texture
                bool skip = false;
                for (int j = 0; j < textures_loaded.Count; j++)
                {
                    
                    if (textures_loaded[j].path.CompareTo(str.FilePath)==0)
                    {
                        textures.Add(textures_loaded[j]);
                        skip = true; // a texture with the same filepath has already been loaded, continue to next one. (optimization)
                        break;
                    }
                }
                if (!skip)
                {   // if texture hasn't been loaded already, load it
                    //TODO directory to setup correctly
                    GL_Texture texture = GL_Texture.LoadFromFile(str.FilePath, "directory", typeName, flip_vertically);
                    textures.Add(texture);
                    textures_loaded.Add(texture);  // store it as texture loaded for entire model, to ensure we won't unnecessary load duplicate textures.
                }
            }
            return textures;
        }*/


    }
}
