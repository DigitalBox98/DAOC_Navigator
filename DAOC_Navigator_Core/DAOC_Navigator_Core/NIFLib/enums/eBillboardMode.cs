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
    /// Enum eBillboardMode
    /// </summary>
    public enum eBillboardMode : ushort
	{
        /// <summary>
        /// The alway s_ fac e_ camera
        /// </summary>
        ALWAYS_FACE_CAMERA,
        /// <summary>
        /// The rotat e_ abou t_ up
        /// </summary>
        ROTATE_ABOUT_UP,
        /// <summary>
        /// The rigi d_ fac e_ camera
        /// </summary>
        RIGID_FACE_CAMERA,
        /// <summary>
        /// The alway s_ fac e_ center
        /// </summary>
        ALWAYS_FACE_CENTER,
        /// <summary>
        /// The rigi d_ fac e_ center
        /// </summary>
        RIGID_FACE_CENTER,
        /// <summary>
        /// The rotat e_ abou t_ u p2
        /// </summary>
        ROTATE_ABOUT_UP2 = 9
	}
}
