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

using Niflib;
using System.Collections;
using DAOC_Navigator_Core.WorldObjects;
using System.Diagnostics;
using OpenTK.Mathematics;
using Shape = DAOC_Navigator_Core.WorldObjects.Shape;
using Polygon = DAOC_Navigator_Core.WorldObjects.Polygon;

namespace DAOC_Navigator_Core.Parsers
{
    public delegate bool IsNodeDrawableEventHandler(NiAVObject node);

    /// <summary>
    /// Parses a DAoC NIF file and converts its geometry into engine-friendly
    /// <see cref="Shape"/> and <see cref="ShapeMaterial"/> lists.
    /// </summary>
    public class NIFParser : IDisposable
    {
        // -------------------------------------------------------------------------
        // Fields
        // -------------------------------------------------------------------------

        /// <summary>Underlying binary stream; owned by this instance.</summary>
        private Stream?  nifStream;

        /// <summary>Parsed NIF object graph.</summary>
        private NiFile?  nifFile;

        private readonly List<ShapeMaterial> uniqueShapeMaterial = new();
        private readonly Hashtable           hashMaterial        = new();  // ShapeMaterial → uint id
        private uint                         materialId          = 0;

        private readonly List<Shape>  shapes         = new();
        private          List<string> ignoreNodeNames = new();
        private          bool         disposed;

        // -------------------------------------------------------------------------
        // Properties / Events
        // -------------------------------------------------------------------------

        public event IsNodeDrawableEventHandler? IsNodeDrawable;

        public List<string> IgnoreNodeNames
        {
            get => ignoreNodeNames;
            set => ignoreNodeNames = value;
        }

        public List<Shape>         Shapes         => shapes;
        public List<ShapeMaterial> ShapeMaterials => uniqueShapeMaterial;

        // -------------------------------------------------------------------------
        // Constructors
        // -------------------------------------------------------------------------

        /// <summary>
        /// Parses a NIF file from a raw <see cref="Stream"/>.
        /// The stream ownership is transferred to this instance.
        /// </summary>
        public NIFParser(Stream stream)
        {
            // FIX #7: use BinaryReader directly on the stream, not via StreamReader.
            nifStream = stream;
            ReadNIFFile();
        }

        /// <summary>
        /// Parses a NIF file by path, resolving case-insensitive names on Linux/macOS.
        /// </summary>
        public NIFParser(string nifFilename)
        {
            string targetFilename = File.Exists(nifFilename)
                ? nifFilename
                : FileUtil.GetActualCaseForFileName(nifFilename);

            nifStream = File.OpenRead(targetFilename);
            ReadNIFFile();
        }

        // -------------------------------------------------------------------------
        // Parsing
        // -------------------------------------------------------------------------

        private void ReadNIFFile()
        {
            // Default list of node names to skip during geometry extraction.
            ignoreNodeNames.AddRange(new[]
            {
                "collidee", "bounding", "climb",
                "!lod_cullme", "!visible_damaged",
                "shadowcaster", "far"
            });

            // FIX #7: BinaryReader wraps the stream directly – no StreamReader involved.
            using var breader = new BinaryReader(nifStream!, System.Text.Encoding.UTF8, leaveOpen: true);
            nifFile = new NiFile(breader);
        }

        /// <summary>
        /// Converts the NIF scene graph into a flat list of <see cref="Shape"/> objects.
        /// </summary>
        public List<Shape> ConvertToShapes()
        {
            if (nifFile != null)
                WalkNodes(nifFile.FindRoot());

            return shapes;
        }

        // -------------------------------------------------------------------------
        // IDisposable
        // -------------------------------------------------------------------------

        public void Dispose()
        {
            if (!disposed)
            {
                nifStream?.Dispose();
                nifStream = null;
                disposed  = true;
            }
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------------
        // Node traversal
        // -------------------------------------------------------------------------

        private void WalkNodes(NiAVObject? node)
        {
            if (node == null || !IsValidNode(node)) return;

            string materialType = string.Empty;
            if (node.Parent is NiNode parentNode)
                materialType = RetrieveMaterialType(parentNode.Name.ToString());

            if (node is NiTriShape triShape)
                ParseNiTriShape(triShape, materialType);
            else if (node is NiTriStrips triStrips)
                ParseStrips(triStrips, materialType);

            if (node is NiNode currentNode)
            {
                foreach (var child in currentNode.Children)
                {
                    if (child.IsValid())
                        WalkNodes(child.Object);
                }
            }
        }

        /// <summary>
        /// Returns <c>true</c> when the node should be included in the output geometry.
        /// </summary>
        internal bool IsValidNode(NiAVObject node)
        {
            // Invisible flag (bit 0).
            if ((node.Flags & 1) == 1) return false;

            // Name-based exclusion list (case-insensitive prefix match).
            string nameLower = node.Name.Value.ToLower();
            foreach (string ignore in IgnoreNodeNames)
            {
                if (nameLower.StartsWith(ignore.ToLower(), StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            // Optional external filter callback.
            return IsNodeDrawable == null || IsNodeDrawable(node);
        }

        // -------------------------------------------------------------------------
        // Geometry extraction
        // -------------------------------------------------------------------------

        private void ParseNiTriShape(NiTriShape nishape, string materialType)
        {
            if (nishape.Data.Object is not NiTriShapeData geometry) return;
            if (!geometry.HasVertices || geometry.NumVertices < 3)   return;

            Matrix4 worldMatrix = ComputeWorldMatrix(nishape);
            Shape   shape       = new Shape();

            NiTexturingProperty? tex = FindTexturingProperty(nishape);
            if (tex != null) shape.MaterialId = RetrieveMaterialId(tex);

            shape.MaterialType = materialType;
            shape.Vertices     = ComputeVertices(geometry.Vertices, worldMatrix);
            shape.Triangles    = ConvertTriangles(geometry.Triangles);

            // FIX: UVSets bounds check (mirrors the check already present here).
            shape.UVSets = geometry.UVSets.Length > 0
                ? geometry.UVSets[0]
                : new[] { new Vector2(0, 0) };

            shapes.Add(shape);
        }

        private void ParseStrips(NiTriStrips strips, string materialType)
        {
            if (strips.Data.Object is not NiTriStripsData geometry) return;
            if (!geometry.HasVertices || geometry.NumVertices < 3)   return;

            Matrix4 worldMatrix = ComputeWorldMatrix(strips);
            Shape   shape       = new Shape();

            NiTexturingProperty? tex = FindTexturingProperty(strips);
            if (tex != null) shape.MaterialId = RetrieveMaterialId(tex);

            shape.MaterialType = materialType;
            shape.Vertices     = ComputeVertices(geometry.Vertices, worldMatrix);

            // FIX #10: guard UVSets access.
            shape.UVSets = geometry.UVSets.Length > 0
                ? geometry.UVSets[0]
                : new[] { new Vector2(0, 0) };

            if (geometry.Points != null)
                shape.Triangles = ConvertTriangles(ComputeTrianglesFromPoints(geometry.Points).ToArray());

            shapes.Add(shape);
        }

        // -------------------------------------------------------------------------
        // Material helpers
        // -------------------------------------------------------------------------

        /// <summary>
        /// Returns (or creates) the unique material id for the texture property.
        /// FIX #8: uses TryGetValue to avoid KeyNotFoundException.
        /// </summary>
        public uint RetrieveMaterialId(NiTexturingProperty objTexture)
        {
            if (objTexture.BaseTexture == null) return 0;

            uint texRefId = objTexture.BaseTexture.Source.RefId;
            if (nifFile?.ObjectsByRef == null) return 0;

            if (!nifFile.ObjectsByRef.TryGetValue(texRefId, out NiObject? sourceObj))
                return 0;

            if (sourceObj is not NiSourceTexture source || source.FileName == null)
                return 0;

            Debug.Write($"Material id/filename = {materialId}/{source.FileName}");

            var sm = new ShapeMaterial(source.FileName.ToString());
            return MaterialUtil.BuildUniqueMaterial(sm, ref materialId, hashMaterial, uniqueShapeMaterial);
        }

        /// <summary>
        /// Maps a node name prefix to one of the well-known DAoC material type strings.
        /// </summary>
        public string RetrieveMaterialType(string materialName)
        {
            // FIX #9: compare with OrdinalIgnoreCase instead of duplicating
            // lower-case and upper-case checks.
            if (materialName.StartsWith(ShapeMaterial.SCENE_BODY, StringComparison.OrdinalIgnoreCase) ||
                materialName.StartsWith(ShapeMaterial.SCENE,      StringComparison.OrdinalIgnoreCase))
                return ShapeMaterial.SCENE;
            if (materialName.StartsWith(ShapeMaterial.BODY,   StringComparison.OrdinalIgnoreCase)) return ShapeMaterial.BODY;
            if (materialName.StartsWith(ShapeMaterial.HEAD,   StringComparison.OrdinalIgnoreCase)) return ShapeMaterial.HEAD;
            if (materialName.StartsWith(ShapeMaterial.ARMS,   StringComparison.OrdinalIgnoreCase)) return ShapeMaterial.ARMS;
            if (materialName.StartsWith(ShapeMaterial.GLOVES, StringComparison.OrdinalIgnoreCase)) return ShapeMaterial.GLOVES;
            if (materialName.StartsWith(ShapeMaterial.LBODY,  StringComparison.OrdinalIgnoreCase)) return ShapeMaterial.LBODY;
            if (materialName.StartsWith(ShapeMaterial.LEGS,   StringComparison.OrdinalIgnoreCase)) return ShapeMaterial.LEGS;
            if (materialName.StartsWith(ShapeMaterial.BOOTS,  StringComparison.OrdinalIgnoreCase)) return ShapeMaterial.BOOTS;
            if (materialName.StartsWith(ShapeMaterial.CLOAK,  StringComparison.OrdinalIgnoreCase)) return ShapeMaterial.CLOAK;
            return ShapeMaterial.OTHER;
        }

        // FIX #12: use NiFile.INVALID_REF constant instead of the magic literal.
        private NiMaterialProperty? FindMaterialProperty(NiTriBasedGeometry nishape)
        {
            if (nishape.Properties == null) return null;
            foreach (var propRef in nishape.Properties)
            {
                if (propRef.RefId == NiFile.INVALID_REF) continue;
                if (nifFile?.ObjectsByRef != null &&
                    nifFile.ObjectsByRef.TryGetValue(propRef.RefId, out var obj) &&
                    obj is NiMaterialProperty mat)
                    return mat;
            }
            return null;
        }

        // FIX #12: use NiFile.INVALID_REF constant.
        private NiTexturingProperty? FindTexturingProperty(NiTriBasedGeometry nishape)
        {
            if (nishape.Properties == null) return null;
            foreach (var propRef in nishape.Properties)
            {
                if (propRef.RefId == NiFile.INVALID_REF) continue;
                if (nifFile?.ObjectsByRef != null &&
                    nifFile.ObjectsByRef.TryGetValue(propRef.RefId, out var obj) &&
                    obj is NiTexturingProperty tex)
                    return tex;
            }
            return null;
        }

        // -------------------------------------------------------------------------
        // Geometry helpers
        // -------------------------------------------------------------------------

        private static List<Triangle> ComputeTrianglesFromPoints(ushort[][] gpoints)
        {
            var triangles = new List<Triangle>();
            foreach (ushort[] points in gpoints)
            {
                if (points.Length < 3) continue;

                ushort p1 = points[0];
                ushort p2 = points[1];
                bool   flip = false;

                for (int j = 1; j < points.Length - 1; j++)
                {
                    ushort p3 = points[j + 1];
                    if (p1 != p2 && p1 != p3 && p2 != p3)
                        triangles.Add(flip ? new Triangle(p1, p3, p2) : new Triangle(p1, p2, p3));

                    p1   = p2;
                    p2   = p3;
                    flip = !flip;
                }
            }
            return triangles;
        }

        private static Polygon[] ConvertTriangles(Triangle[]? triangles)
        {
            if (triangles == null || triangles.Length == 0)
                return Array.Empty<Polygon>();

            var polygons = new Polygon[triangles.Length];
            for (int i = 0; i < triangles.Length; i++)
                polygons[i] = new Polygon(triangles[i].X, triangles[i].Y, triangles[i].Z);
            return polygons;
        }

        private static Vector3[] ComputeVertices(Vector3[]? vertices, Matrix4 transform)
        {
            if (vertices == null) return Array.Empty<Vector3>();
            var result = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
                result[i] = Vector3.TransformPosition(vertices[i], transform);
            return result;
        }

        /// <summary>
        /// Computes the accumulated world-space transform for <paramref name="obj"/> by
        /// walking up the parent chain and composing transforms from root → leaf.
        /// FIX #3: path is traversed from root to leaf (reversed) so child transforms
        /// are applied after parent transforms, matching the NIF hierarchy convention.
        /// </summary>
        private static Matrix4 ComputeWorldMatrix(NiAVObject obj)
        {
            // Build path from the node up to the root.
            var path    = new List<NiAVObject>();
            var current = obj;
            while (current != null)
            {
                path.Add(current);
                current = current.Parent;
            }

            // Apply transforms from root → leaf (reverse of the collected order).
            var worldMatrix = Matrix4.Identity;
            for (int i = path.Count - 1; i >= 0; i--)
            {
                var node = path[i];
                worldMatrix *= node.Rotation;
                worldMatrix *= Matrix4.CreateScale(node.Scale, node.Scale, node.Scale);
                worldMatrix *= Matrix4.CreateTranslation(node.Translation.X, node.Translation.Y, node.Translation.Z);
            }
            return worldMatrix;
        }
    }
}
