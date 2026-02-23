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
    /// Class NiParticleGrowFade.
    /// </summary>
    public class NiParticleGrowFade : NiParticleModifier
	{
        /// <summary>
        /// The grow
        /// </summary>
        public float Grow;

        /// <summary>
        /// The fade
        /// </summary>
        public float Fade;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiParticleGrowFade" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiParticleGrowFade(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.Grow = reader.ReadSingle();
			this.Fade = reader.ReadSingle();
		}
	}
}
