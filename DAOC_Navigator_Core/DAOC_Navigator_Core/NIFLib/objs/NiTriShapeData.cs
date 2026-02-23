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
    /// Class NiTriShapeData.
    /// </summary>
    public class NiTriShapeData : NiTriBasedGeomData
	{
        /// <summary>
        /// The number triangle points
        /// </summary>
        public uint NumTrianglePoints;

        /// <summary>
        /// The has triangles
        /// </summary>
        public bool HasTriangles;

        /// <summary>
        /// The triangles
        /// </summary>
        public Triangle[]? Triangles;

        /// <summary>
        /// The match groups
        /// </summary>
        public ushort[][]? MatchGroups;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiTriShapeData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiTriShapeData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.NumTrianglePoints = reader.ReadUInt32();
			if (base.Version >= eNifVersion.VER_10_1_0_0)
			{
				this.HasTriangles = reader.ReadBoolean(Version);
			}
			if (base.Version <= eNifVersion.VER_10_0_1_2 || this.HasTriangles || base.Version >= eNifVersion.VER_10_0_1_3)
			{
				this.Triangles = new Triangle[(int)this.NumTriangles];
				this.HasTriangles = this.Triangles.Length > 0;
				for (int i = 0; i < (int)this.NumTriangles; i++)
				{
					this.Triangles[i] = new Triangle(reader);
				}
			}
			if (base.Version >= eNifVersion.VER_3_1)
			{
				ushort num = reader.ReadUInt16();
				this.MatchGroups = new ushort[(int)num][];
				for (int j = 0; j < (int)num; j++)
				{
					ushort num2 = reader.ReadUInt16();
					this.MatchGroups[j] = new ushort[(int)num2];
					for (int k = 0; k < (int)num2; k++)
					{
						this.MatchGroups[j][k] = reader.ReadUInt16();
					}
				}
			}
		}
	}
}
