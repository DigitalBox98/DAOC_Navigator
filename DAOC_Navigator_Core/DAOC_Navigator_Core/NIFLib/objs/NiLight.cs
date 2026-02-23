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

using Color3 = OpenTK.Mathematics.Color4;

namespace Niflib
{
    /// <summary>
    /// Class NiLight.
    /// </summary>
    public class NiLight : NiDynamicEffect
	{
        /// <summary>
        /// The dimmer
        /// </summary>
        public float Dimmer;

        /// <summary>
        /// The ambient color
        /// </summary>
        public Color3 AmbientColor;

        /// <summary>
        /// The diffuse color
        /// </summary>
        public Color3 DiffuseColor;

        /// <summary>
        /// The specular color
        /// </summary>
        public Color3 SpecularColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiLight"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiLight(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.Dimmer = reader.ReadSingle();
			this.AmbientColor = reader.ReadColor3();
			this.DiffuseColor = reader.ReadColor3();
			this.SpecularColor = reader.ReadColor3();
		}
	}
}
