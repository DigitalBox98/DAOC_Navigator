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

using Assimp;
using DAOC_Navigator_Core.WorldObjects;
using System.Collections;
using System.Diagnostics;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;

// Assimp is only used to load models other than NIF format 
using AssimpMesh = Assimp.Mesh;
using AssimpMatrix4x4 = Assimp.Matrix4x4;

namespace DAOC_Navigator_Core.Parsers
{
    public static class Extensions
    {
        public static Vector3 ConvertAssimpVector3(this Vector3D AssimpVector)
        {
            // Reinterpret the assimp vector into an OpenTK vector.
            return Unsafe.As<Vector3D, Vector3>(ref AssimpVector);
        }

        public static Matrix4 ConvertAssimpMatrix4(this AssimpMatrix4x4 AssimpMatrix)
        {
            // Take the column-major assimp matrix and convert it to a row-major OpenTK matrix.
            return Matrix4.Transpose(Unsafe.As<AssimpMatrix4x4, Matrix4>(ref AssimpMatrix));
        }
    }

    public class AssimpParser
	{

        /// <value>
        /// Property <c>assimpImporter</c> represents the AssimpContext used to read the Assimp model file
        /// </value>
        private AssimpContext? assimpImporter;

        /// <value>
        /// Property <c>uniqueShapeMaterial</c> represents the unique list of ShapeMaterial retrieved
        /// </value>
        private List<ShapeMaterial> uniqueShapeMaterial = new List<ShapeMaterial>();
        private Hashtable hashMaterial = new Hashtable(); // (ShapeMaterial => int)
        private uint materialId = 0;

        /// <value>
        /// Property <c>shapes</c> represents the list of Shapes retrieved from NIF
        /// </value>
        private List<Shape> shapes = new List<Shape>();


        public List<Shape> Shapes
        {
            get { return shapes; }
        }

        public List<ShapeMaterial> ShapeMaterials
        {
            get { return uniqueShapeMaterial; }
        }


        public AssimpParser(string assimpFilename)
		{
            string targetFilename = assimpFilename;
            if (!File.Exists(assimpFilename))
            {
                targetFilename = FileUtil.GetActualCaseForFileName(assimpFilename);
            }
            ReadAssimpFile(targetFilename);
        }

        /// <summary>
        /// Reads the assimp File
        /// </summary>
        private void ReadAssimpFile(string filename)
        {
            // Create a new importer
            assimpImporter = new AssimpContext();


            Scene scene = assimpImporter.ImportFile(filename, PostProcessSteps.Triangulate);

            // Convert Scene to Model/Shapes 
            processAssimpModel(scene);

            // Once we are done with the importer, we release the resources since all the data we need
            // is now contained within our list of processed meshes
            assimpImporter.Dispose();
        }


        /// <summary>
        /// processAssimpModel
        /// </summary>
        public void processAssimpModel(Scene scene)
        {
            // Check for errors
            if (scene == null || scene.SceneFlags.HasFlag(SceneFlags.Incomplete) || scene.RootNode == null)
            {
                Debug.Write("Unable to load model from scene ");
                return;
            }

            // Create an empty list to be filled with meshes in the ProcessNode method
            shapes = new List<Shape>();


            // Process ASSIMP's root node recursively. We pass in the scaling matrix as the first transform
            ProcessAssimpNode(scene.RootNode, scene);

        }

        /// <summary>
        /// ProcessAssimpNode
        /// </summary>
        private void ProcessAssimpNode(Node node, Scene scene)
        {
            // Process each mesh located at the current node
            for (int i = 0; i < node.MeshCount; i++)
            {
                // Nodes don't actually carry any of the mesh data, but rather give an index to the corresponding Mesh
                // within the scene.Meshes List. The Nodes form the hierarchy of the model so that we can establish 
                // parent-child relationships, which are important for passing along transformations.
                AssimpMesh mesh = scene.Meshes[node.MeshIndices[i]];
                shapes.Add(ProcessAssimpMesh(mesh, scene));
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessAssimpNode(node.Children[i], scene);
            }
        }


        /// <summary>
        /// This method processes an Assimp mesh to a Shape
        /// </summary>
        ///
        private Shape ProcessAssimpMesh(AssimpMesh mesh, Scene scene)
        {
            Shape shape = new Shape();

            // the vertices of the shapes
            List<Vector3> verticesList = new List<Vector3>();
            Vector3[] vertices;
            // triangles set defined by indexes <idx1, idx2, idx3> referencing vertices
            List<Polygon> trianglesList = new List<Polygon>();
            Polygon[] triangles;
            // represents the material UV sets
            List<Vector2> uvsetsList = new List<Vector2>();
            Vector2[] uvsets;

            // Walk through each of the mesh's vertices
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vector3 vertex = new Vector3();

                // Positions
                vertex = mesh.Vertices[i].ConvertAssimpVector3(); //transformedPosition;

                // Normals
                /*if (mesh.HasNormals)
                {
                    vertex.Normal = mesh.Normals[i].ConvertAssimpVector3(); //transformedNormal;
                }*/

                // Texture coordinates 
                if (mesh.HasTextureCoords(0)) // Does the mesh contain texture coordinates?
                {
                    Vector2 vec;
                    vec.X = mesh.TextureCoordinateChannels[0][i].X;
                    vec.Y = mesh.TextureCoordinateChannels[0][i].Y;
                    uvsetsList.Add(vec);
                }
                else
                {
                    uvsetsList.Add(new Vector2(0.0f, 0.0f));
                }

                verticesList.Add(vertex);
            }
            // convert to array
            vertices = verticesList.ToArray();
            uvsets = uvsetsList.ToArray();

            // Now walk through each of the mesh's faces (a face is a group of vertices that form a triangle, quadrilateral, or ngon) and retrieve the corresponding vertex indices.
            // All of the faces should be triangles since we used PostProcessSteps.Triangulate during the Assimp import
            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                List<uint> idxList = new List<uint>();
                for (int j = 0; j < face.IndexCount; j++)
                {
                    idxList.Add((uint)face.Indices[j]);
                }
                uint[] idx = idxList.ToArray();
                if (idx.Length == 3)
                {
                    trianglesList.Add(new Polygon((ushort)idx[0], (ushort)idx[1], (ushort)idx[2]));
                }
            }
            // convert to array
            triangles = trianglesList.ToArray();

            // process materials
            Material material = scene.Materials[mesh.MaterialIndex];

            // Set Material Type
            shape.MaterialType = ShapeMaterial.SCENE;

            // Get filename of the material : diffuse
            if (material.HasTextureDiffuse == true)
            {
                //Console.WriteLine("diffuse texture = "+material.TextureDiffuse.FilePath);

                ShapeMaterial sm = new ShapeMaterial(material.TextureDiffuse.FilePath);
                materialId = MaterialUtil.BuildUniqueMaterial(sm, ref materialId, hashMaterial, uniqueShapeMaterial);

            }

            shape = new Shape(vertices, triangles, materialId, uvsets);
            return shape;

        }

        public List<Shape> ConvertToShapes()
        {
            return shapes;
        }

    }
}
