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

#if OpenTK
	using OpenTK;
#elif SharpDX
	using SharpDX;
#elif MonoGame
	using Microsoft.Xna.Framework;
#endif
using OpenTK.Mathematics;

namespace Niflib
{

    /// <summary>
    /// Class NiParticleRotation.
    /// </summary>
    public class NiParticleRotation : NiParticleModifier
	{
        /// <summary>
        /// The random inital axis
        /// </summary>
        public bool RandomInitalAxis;

        /// <summary>
        /// The initial axis
        /// </summary>
        public Vector3 InitialAxis;

        /// <summary>
        /// The speed
        /// </summary>
        public float Speed;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiParticleRotation"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiParticleRotation(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.RandomInitalAxis = reader.ReadBoolean(Version);
			this.InitialAxis = reader.ReadVector3();
			this.Speed = reader.ReadSingle();
		}
	}
}
