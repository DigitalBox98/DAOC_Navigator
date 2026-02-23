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
    /// Class NiGravity.
    /// </summary>
    public class NiGravity : NiParticleModifier
	{
        /// <summary>
        /// The unkown float1
        /// </summary>
        public float UnkownFloat1;

        /// <summary>
        /// The force
        /// </summary>
        public float Force;

        /// <summary>
        /// The type
        /// </summary>
        public uint Type;

        /// <summary>
        /// The position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The direction
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiGravity" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiGravity(NiFile file, BinaryReader reader) : base(file, reader)
		{
			if (this.File.Header.Version >= eNifVersion.VER_4_0_0_2)
			{
				this.UnkownFloat1 = reader.ReadSingle();
			}
			this.Force = reader.ReadSingle();
			this.Type = reader.ReadUInt32();
			this.Position = reader.ReadVector3();
			this.Direction = reader.ReadVector3();
		}
	}
}
