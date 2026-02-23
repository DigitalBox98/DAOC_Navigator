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

namespace Niflib
{
    /// <summary>
    /// Class Triangle.
    /// </summary>
    public class Triangle
	{
        /// <summary>
        /// The x
        /// </summary>
        public ushort X;

        /// <summary>
        /// The y
        /// </summary>
        public ushort Y;

        /// <summary>
        /// The z
        /// </summary>
        public ushort Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public Triangle(ushort x, ushort y, ushort z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public Triangle(BinaryReader reader)
		{
			this.X = reader.ReadUInt16();
			this.Y = reader.ReadUInt16();
			this.Z = reader.ReadUInt16();
		}
	}
}
