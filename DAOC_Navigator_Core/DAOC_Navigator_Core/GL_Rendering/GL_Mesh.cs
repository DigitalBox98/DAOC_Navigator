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

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAOC_Navigator_Core.GL_Rendering
{
    /// <summary>
    /// The Vertex structure represents a Vertex in the 3D space.
    /// This structure is composed of a Position vector, a Normal vector and a Texture coordinate vector.
    /// </summary>
    /// 
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoords;

        public override string ToString()
        {
            return " Pos(X,Y,Z)=(" + Position.X + ", " + Position.Y + "," + Position.Z + ")"
                    + " Norm(X,Y,Z)=(" + Normal.X + ", " + Normal.Y + "," + Normal.Z + ")"
                    + " Tex(X,Y)=(" + TexCoords.X + ", " + TexCoords.Y + ")";
        }

        public static Vertex VectorToVertex(Vector3 vec)
        {
            Vertex v = new Vertex();
            v.Position.X = vec.X;
            v.Position.Y = vec.Y;
            v.Position.Z = vec.Z;

            return v;
        }
    }

    /// <summary>
    /// Class <c>GL_Mesh</c> represents an OpenGL Mesh.
    /// <c>GL_Mesh</c> is composed of VAOs (Vertex Array Objects), VBOs (Vertex Buffer Objects)
    /// and EBOs (Element Buffer Objects) based on indices, and a list of GL_Texture.
    /// </summary>
    /// 
    public class GL_Mesh
    {
        /// <value>
        /// Property <c>VAO</c> represents the OpenGL VAO id (Vertex Array Object)
        /// </value>
        public int VAO;

        /// <value>
        /// Property <c>indicesCount</c> represents the number of indices of vertex
        /// </value>
        public int indicesCount;

        /// <value>
        /// Property <c>textures</c> represents the GL_Texture list
        /// </value>
        public List<GL_Texture> textures;

        /// <value>
        /// Attribute <c>VBO</c> represents the OpenGL VBO id (Vertex Buffer Object)
        /// </value>
        private int VBO;

        /// <value>
        /// Attribute <c>EBO</c> represents the OpenGL EBO id (Element Buffer Object)
        /// </value>
        private int EBO;

        /// <summary>
        /// This constructor initializes the new GL_Mesh with 
        /// (<paramref name="vertices"/>,<paramref name="indices"/>,<paramref name="textures"/>).
        /// </summary>
        /// <param name="vertices">The new GL_Mesh's array of vertices (Vertex).</param>
        /// <param name="indices">The new GL_Mesh's array of indices.</param>
        /// <param name="textures">The new GL_Mesh's list of textures (GL_Texture).</param>
        public GL_Mesh(Span<Vertex> vertices, Span<uint> indices, List<GL_Texture> textures)
        {
            this.textures = textures;
            indicesCount = indices.Length;

            setupMesh(vertices, indices);

        }

        /// <summary>
        /// This method draw the GL_Mesh into the screen
        /// </summary>
        /// <param name="shader">The GL_Shader used to draw the GL_Mesh</param>
        ///
        public void Draw(GL_Shader shader)
        {
            // Bind appropriate textures
            int diffuseNr = 1;
            int specularNr = 1;
            int normalNr = 1;
            int heightNr = 1;

            for (int i = 0; i < textures.Count; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i); // active proper texture unit before binding
                                                  // retrieve texture number (the N in diffuse_textureN)
                string number = new string("0");
                string name = textures[i].type;
                // Diffuse texture 
                if (name == "texture_diffuse")
                    number = new string(""+diffuseNr++);
                // Specular texture
                else if (name == "texture_specular")
                    number = new string("" + specularNr++);
                // Normal texture
                else if (name == "texture_normal")
                    number = new string("" + normalNr++);
                // Height texture (for bump mapping, etc.)
                else if (name == "texture_height")
                    number = new string("" + heightNr++);

                // now set the sampler to the correct texture unit
                GL.Uniform1(GL.GetUniformLocation(shader.ID, (name + number)), i);
                // and finally bind the texture
                GL.BindTexture(TextureTarget.Texture2D, textures[i].ID);
            }

            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            // always good practice to set everything back to defaults once configured.
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        /// <summary>
        /// This method set up the Mesh
        /// </summary>
        /// <param name="vertices">The new GL_Mesh's array of vertices.</param>
        /// <param name="indices">The new GL_Mesh's array of indices.</param>
        ///
        private void setupMesh(Span<Vertex> vertices, Span<uint> indices)
        {
            // Set up Mesh
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            // Vertex Array Object
            GL.BindVertexArray(VAO);
            // Vertex Buffer Object
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Unsafe.SizeOf<Vertex>(), ref MemoryMarshal.GetReference(vertices), BufferUsageHint.StaticDraw);

            // Element Buffer Object
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), ref MemoryMarshal.GetReference(indices), BufferUsageHint.StaticDraw);

            // Vertex positions
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>(nameof(Vertex.Position)));

            // Vertex normals
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>(nameof(Vertex.Normal)));

            // Vertex texture coords
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>(nameof(Vertex.TexCoords)));

            GL.BindVertexArray(0);
        }


    }
}
