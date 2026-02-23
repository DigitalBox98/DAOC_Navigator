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
    /// Class NiRotatingParticlesData.
    /// </summary>
    public class NiRotatingParticlesData : NiParticlesData
	{
        /// <summary>
        /// The has rotations2
        /// </summary>
        public bool HasRotations2;

        /// <summary>
        /// The rotations2
        /// </summary>
        public Vector4[]? Rotations2;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiRotatingParticlesData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiRotatingParticlesData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			if (base.Version <= eNifVersion.VER_4_2_2_0)
			{
				this.HasRotations2 = reader.ReadBoolean(Version);
				this.Rotations2 = new Vector4[this.NumVertices];
				int num = 0;
				while ((long)num < (long)((ulong)this.NumVertices))
				{
					this.Rotations2[num] = reader.ReadVector4();
					num++;
				}
			}
		}
	}
}
