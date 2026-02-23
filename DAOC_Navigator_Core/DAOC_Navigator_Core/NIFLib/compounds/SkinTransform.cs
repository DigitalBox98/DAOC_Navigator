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
using Matrix = OpenTK.Mathematics.Matrix4;

namespace Niflib
{
    /// <summary>
    /// Class SkinTransform.
    /// </summary>
    public class SkinTransform
	{
        /// <summary>
        /// The rotation
        /// </summary>
        public Matrix Rotation;

        /// <summary>
        /// The translation
        /// </summary>
        public Vector3 Translation;

        /// <summary>
        /// The scale
        /// </summary>
        public float Scale;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinTransform"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public SkinTransform(NiFile file, BinaryReader reader)
		{
			this.Rotation = reader.ReadMatrix33();
			this.Translation = reader.ReadVector3();
			this.Scale = reader.ReadSingle();
		}
	}
}
