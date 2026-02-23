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
    /// Class NiParticleBomb.
    /// </summary>
    public class NiParticleBomb : NiParticleModifier
	{
        /// <summary>
        /// The decay
        /// </summary>
        public float Decay;

        /// <summary>
        /// The duration
        /// </summary>
        public float Duration;

        /// <summary>
        /// The delta v
        /// </summary>
        public float DeltaV;

        /// <summary>
        /// The start
        /// </summary>
        public float Start;

        /// <summary>
        /// The decay type
        /// </summary>
        public eDecayType DecayType;

        /// <summary>
        /// The symmetry type
        /// </summary>
        public eSymmetryType SymmetryType;

        /// <summary>
        /// The position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The direction
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiParticleBomb" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiParticleBomb(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.Decay = reader.ReadSingle();
			this.Duration = reader.ReadSingle();
			this.DeltaV = reader.ReadSingle();
			this.Start = reader.ReadSingle();
			this.DecayType = (eDecayType)reader.ReadUInt32();
			if (base.Version >= eNifVersion.VER_4_1_0_12)
			{
				this.SymmetryType = (eSymmetryType)reader.ReadUInt32();
			}
			this.Position = reader.ReadVector3();
			this.Direction = reader.ReadVector3();
		}
	}
}
