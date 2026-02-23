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
    /// Class NiParticleMeshModifier.
    /// </summary>
    public class NiParticleMeshModifier : NiParticleModifier
	{
        /// <summary>
        /// The particle meshes
        /// </summary>
        public NiRef<NiAVObject>[] ParticleMeshes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiParticleMeshModifier" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiParticleMeshModifier(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.ParticleMeshes = new NiRef<NiAVObject>[reader.ReadUInt32()];
			for (int i = 0; i < this.ParticleMeshes.Length; i++)
			{
				this.ParticleMeshes[i] = new NiRef<NiAVObject>(reader);
			}
		}
	}
}
