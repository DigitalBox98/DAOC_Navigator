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
    /// Class NiPointLight.
    /// </summary>
    public class NiPointLight : NiLight
	{
        /// <summary>
        /// The constant attenuation
        /// </summary>
        public float ConstantAttenuation;

        /// <summary>
        /// The linear attenuation
        /// </summary>
        public float LinearAttenuation;

        /// <summary>
        /// The quadratic attenuation
        /// </summary>
        public float QuadraticAttenuation;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiPointLight"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiPointLight(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.ConstantAttenuation = reader.ReadSingle();
			this.LinearAttenuation = reader.ReadSingle();
			this.QuadraticAttenuation = reader.ReadSingle();
		}
	}
}
