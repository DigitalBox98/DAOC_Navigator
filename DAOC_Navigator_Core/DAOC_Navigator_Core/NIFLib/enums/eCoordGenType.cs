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
    /// Enum eCoordGenType
    /// </summary>
    public enum eCoordGenType : uint
	{
        /// <summary>
        /// The c g_ worl d_ parallel
        /// </summary>
        CG_WORLD_PARALLEL,
        /// <summary>
        /// The c g_ worl d_ perspective
        /// </summary>
        CG_WORLD_PERSPECTIVE,
        /// <summary>
        /// The c g_ spher e_ map
        /// </summary>
        CG_SPHERE_MAP,
        /// <summary>
        /// The c g_ specula r_ cub e_ map
        /// </summary>
        CG_SPECULAR_CUBE_MAP,
        /// <summary>
        /// The c g_ diffus e_ cub e_ map
        /// </summary>
        CG_DIFFUSE_CUBE_MAP
    }
}
