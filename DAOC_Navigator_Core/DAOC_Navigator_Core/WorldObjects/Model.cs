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

using DAOC_Navigator_Core.Parsers;
using DAOC_Navigator_Core.Game;

namespace DAOC_Navigator_Core.WorldObjects
{
    /// <summary>
    /// Represents a 3-D model loaded from a NIF or Assimp-compatible file.
    /// </summary>
    public class Model
    {
        private List<Shape>    shapes;
        private ShapeMaterial[] materials;
        private SkinSet?       skinSet;
        private string         directory = string.Empty;
        private string         file      = string.Empty;

        public List<Shape>     Shapes    { get => shapes;    set => shapes    = value; }
        public ShapeMaterial[] Materials { get => materials; set => materials = value; }
        public SkinSet?        SkinSet   { get => skinSet;   set => skinSet   = value; }
        public string          Directory { get => directory; set => directory = value; }
        public string          File      { get => file;      set => file      = value; }

        // -------------------------------------------------------------------------
        // Constructors
        // -------------------------------------------------------------------------

        public Model(string filepath)
        {
            directory = Path.GetDirectoryName(filepath) ?? string.Empty;
            file      = Path.GetFileName(filepath);
            shapes    = new List<Shape>();
            materials = Array.Empty<ShapeMaterial>();
            ParseModel();
        }

        public Model(string filepath, SkinSet figureSkin)
        {
            directory = Path.GetDirectoryName(filepath) ?? string.Empty;
            file      = Path.GetFileName(filepath);
            shapes    = new List<Shape>();
            materials = Array.Empty<ShapeMaterial>();
            ParseModel();
            skinSet = figureSkin;
        }

        public Model(Stream stream, string filepath)
        {
            directory = Path.GetDirectoryName(filepath) ?? string.Empty;
            file      = Path.GetFileName(filepath);
            shapes    = new List<Shape>();
            materials = Array.Empty<ShapeMaterial>();
            ParseModelFromStream(stream);
        }

        public Model(Stream stream, string filepath, SkinSet skinSetArg)
        {
            directory = Path.GetDirectoryName(filepath) ?? string.Empty;
            file      = Path.GetFileName(filepath);
            shapes    = new List<Shape>();
            materials = Array.Empty<ShapeMaterial>();
            ParseModelFromStream(stream);
            skinSet = skinSetArg;
        }

        // -------------------------------------------------------------------------
        // Parsing
        // -------------------------------------------------------------------------

        /// <summary>
        /// Loads the model from disk, dispatching to the NIF or Assimp parser
        /// depending on the file extension.
        /// </summary>
        public void ParseModel()
        {
            string fullPath = Path.Combine(directory, file);

            if (file.EndsWith(".nif", StringComparison.OrdinalIgnoreCase))
            {
                // FIX #13: Path.Combine instead of manual separator concatenation.
                using var parser = new NIFParser(fullPath);
                shapes    = parser.ConvertToShapes();
                materials = parser.ShapeMaterials.ToArray();
            }
            else
            {
                var parser = new AssimpParser(fullPath);
                shapes    = parser.ConvertToShapes();
                materials = parser.ShapeMaterials.ToArray();
            }
        }

        /// <summary>
        /// Loads the model from an already-opened stream (e.g. extracted from an MPK archive).
        /// Only NIF format is supported via this path.
        /// </summary>
        public void ParseModelFromStream(Stream stream)
        {
            using var parser = new NIFParser(stream);
            shapes    = parser.ConvertToShapes();
            materials = parser.ShapeMaterials.ToArray();
        }
    }
}
