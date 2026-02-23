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

namespace Niflib
{
    /// <summary>
    /// Class ByteKey.
    /// </summary>
    public class ByteKey
	{
        /// <summary>
        /// The time
        /// </summary>
        public float Time;

        /// <summary>
        /// The value
        /// </summary>
        public byte Value;

        /// <summary>
        /// The TBC
        /// </summary>
        public Vector3 TBC;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteKey"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="Exception">Invalid eKeyType</exception>
        public ByteKey(BinaryReader reader, eKeyType type)
		{
			this.Time = reader.ReadSingle();
			if (type != eKeyType.LINEAR_KEY)
			{
				throw new Exception("Invalid eKeyType");
			}
			this.Value = reader.ReadByte();
			if (type == eKeyType.TBC_KEY)
			{
				this.TBC = reader.ReadVector3();
			}
		}
	}
}
