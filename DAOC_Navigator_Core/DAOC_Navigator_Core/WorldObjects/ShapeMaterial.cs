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

namespace DAOC_Navigator_Core.WorldObjects
{
    /// <summary>
    /// Identifies the DAoC node-name prefixes used to classify geometry parts of
    /// a figure NIF, and holds the filename of the associated texture.
    /// </summary>
    /// <remarks>
    /// DAoC NIF node-name conventions (case-insensitive prefix matching):
    /// <list type="table">
    ///   <item><term>Scene / Body</term><description>Root scene node – uses the body skin texture.</description></item>
    ///   <item><term>Body2</term><description>Body geometry layer 2.</description></item>
    ///   <item><term>Head</term><description>Head geometry.</description></item>
    ///   <item><term>Arms1</term><description>Arms geometry.</description></item>
    ///   <item><term>Gloves1</term><description>Gloves geometry.</description></item>
    ///   <item><term>LBody2</term><description>Lower body geometry.</description></item>
    ///   <item><term>Legs1</term><description>Legs geometry.</description></item>
    ///   <item><term>Boots1</term><description>Boots geometry.</description></item>
    ///   <item><term>Cloak1</term><description>Cloak geometry.</description></item>
    /// </list>
    /// These string values must match the actual node-name prefixes found in the NIF files.
    /// </remarks>
    public class ShapeMaterial
    {
        // ---- Material type constants ----
        // FIX #9: names now clearly map to their actual NIF node-name prefix values.

        /// <summary>
        /// NIF node prefix for the root scene node that renders with the body skin.
        /// Actual node name starts with <c>"Body"</c> (at the scene/root level).
        /// </summary>
        public static readonly string SCENE_BODY = "Body";

        /// <summary>
        /// Logical category label for the scene/root body node.
        /// </summary>
        public static readonly string SCENE = "Scene";

        /// <summary>
        /// NIF node prefix for the secondary body geometry layer.
        /// Actual node name starts with <c>"Body2"</c>.
        /// </summary>
        public static readonly string BODY = "Body2";

        /// <summary>NIF node prefix for head geometry (<c>"Head"</c>).</summary>
        public static readonly string HEAD = "Head";

        /// <summary>NIF node prefix for arms geometry (<c>"Arms1"</c>).</summary>
        public static readonly string ARMS = "Arms1";

        /// <summary>NIF node prefix for gloves geometry (<c>"Gloves1"</c>).</summary>
        public static readonly string GLOVES = "Gloves1";

        /// <summary>NIF node prefix for lower-body geometry (<c>"LBody2"</c>).</summary>
        public static readonly string LBODY = "LBody2";

        /// <summary>NIF node prefix for legs geometry (<c>"Legs1"</c>).</summary>
        public static readonly string LEGS = "Legs1";

        /// <summary>NIF node prefix for boots geometry (<c>"Boots1"</c>).</summary>
        public static readonly string BOOTS = "Boots1";

        /// <summary>NIF node prefix for cloak geometry (<c>"Cloak1"</c>).</summary>
        public static readonly string CLOAK = "Cloak1";

        /// <summary>Fallback category for geometry that does not match any known prefix.</summary>
        public static readonly string OTHER = "Other";

        // ---- Instance members ----

        private string materialFilename;

        public string Filename
        {
            get => materialFilename;
            set => materialFilename = value;
        }

        public ShapeMaterial(string materialFilename)
        {
            this.materialFilename = materialFilename;
        }
    }
}
