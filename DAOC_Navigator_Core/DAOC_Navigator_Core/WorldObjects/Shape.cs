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

namespace DAOC_Navigator_Core.WorldObjects
{
    // Shape is corresponding to a GL_Mesh for the rendering part 
    //
    public class Shape
    {
        /// <value>
        /// Property <c>vertices</c> represents the vertices of the shapes
        /// </value>
        private Vector3[] vertices;

        /// <value>
        /// Property <c>triangles</c> represents the triangles set defined by indexes <idx1, idx2, idx3> referencing vertices
        /// </value>
        private Polygon[] triangles;

        /// <value>
        /// Property <c>material_id</c> represents the material id of the shape / only 1 material is handled
        /// </value>
        private uint material_id = 0;

        /// <value>
        /// Property <c>materialType</c> represents the material type of the shape 
        /// </value>
        private string materialType = ShapeMaterial.BODY;

        /// <value>
        /// Property <c>uvsets</c> represents the material UV sets
        /// </value>
        private Vector2[] uvsets; 

        public Polygon[] Triangles
        {
            get { return triangles; }
            set { triangles = value; }
        }

        public uint MaterialId
        {
            get { return material_id; }
            set { material_id = value; }
        }

        public string MaterialType
        {
            get { return materialType; }
            set { materialType = value; }
        }

        public Vector3[] Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }

        public Vector2[] UVSets
        {
            get { return uvsets; }
            set { uvsets = value; }
        }

        public Shape()
        {
            this.vertices = new Vector3[0];
            this.triangles = new Polygon[0];
            this.uvsets = new Vector2[0];
        }

        public Shape(Vector3[] vertices, Polygon[] triangles)
        {
            this.vertices = vertices;
            this.triangles = triangles;
            this.uvsets = new Vector2[0];
        }

        public Shape(Vector3[] vertices, Polygon[] triangles, uint material_id)
        {
            this.vertices = vertices;
            this.triangles = triangles;
            this.material_id = material_id;
            this.uvsets = new Vector2[0];
        }

        public Shape(Vector3[] vertices, Polygon[] triangles, uint material_id, Vector2[] uvsets)
        {
            this.vertices = vertices;
            this.triangles = triangles;
            this.material_id = material_id;
            this.uvsets = uvsets;
            this.materialType = ShapeMaterial.BODY;
        }
    }
}
