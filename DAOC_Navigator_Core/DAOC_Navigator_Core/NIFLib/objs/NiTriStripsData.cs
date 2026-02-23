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
    /// Class NiTriStripsData.
    /// </summary>
    public class NiTriStripsData : NiTriBasedGeomData
	{
        /// <summary>
        /// The has points
        /// </summary>
        public bool HasPoints;

        /// <summary>
        /// The points
        /// </summary>
        public ushort[][]? Points;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiTriStripsData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiTriStripsData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			ushort[] array = new ushort[(int)reader.ReadUInt16()];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = reader.ReadUInt16();
			}
			if (base.Version >= eNifVersion.VER_10_0_1_3)
			{
				this.HasPoints = reader.ReadBoolean(Version);
			}
			else
			{
				this.HasPoints = array.Length > 0;
			}
			
			if (base.Version < eNifVersion.VER_10_0_1_3 || this.HasPoints)
			{
				this.Points = new ushort[array.Length][];
				for (int j = 0; j < array.Length; j++)
				{
					this.Points[j] = new ushort[(int)array[j]];
					for (ushort num = 0; num < array[j]; num++)
					{
						this.Points[j][(int)num] = reader.ReadUInt16();
					}
				}
			}
		}
	}
}
