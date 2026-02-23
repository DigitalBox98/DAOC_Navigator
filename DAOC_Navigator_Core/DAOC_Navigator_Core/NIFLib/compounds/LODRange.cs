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
    /// Class LODRange.
    /// </summary>
    public class LODRange
	{
        /// <summary>
        /// The near extent
        /// </summary>
        public float NearExtent;

        /// <summary>
        /// The far extent
        /// </summary>
        public float FarExtent;

        /// <summary>
        /// The unkown ints
        /// </summary>
        public uint[]? UnkownInts;

        /// <summary>
        /// Initializes a new instance of the <see cref="LODRange"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public LODRange(NiFile file, BinaryReader reader)
		{
			this.NearExtent = reader.ReadSingle();
			this.FarExtent = reader.ReadSingle();
			if (file.Version <= eNifVersion.VER_3_1)
			{
				this.UnkownInts = new uint[]
				{
					reader.ReadUInt32(),
					reader.ReadUInt32(),
					reader.ReadUInt32()
				};
			}
		}
	}
}
