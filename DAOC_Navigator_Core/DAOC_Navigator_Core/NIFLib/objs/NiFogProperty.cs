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

using Color3 = OpenTK.Mathematics.Color4;

namespace Niflib
{
    /// <summary>
    /// Class NiFogProperty.
    /// </summary>
    public class NiFogProperty : NiProperty
	{
        /// <summary>
        /// The flags
        /// </summary>
        public ushort Flags;

        /// <summary>
        /// The depth
        /// </summary>
        public float Depth;

        /// <summary>
        /// The color
        /// </summary>
        public Color3 Color;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiFogProperty" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiFogProperty(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.Flags = reader.ReadUInt16();
			this.Depth = reader.ReadSingle();
			this.Color = reader.ReadColor3();
		}
	}
}
