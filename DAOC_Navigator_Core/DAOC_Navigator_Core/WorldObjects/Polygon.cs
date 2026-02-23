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

namespace DAOC_Navigator_Core.WorldObjects
{
    // Polygon is a triangle here 
    public struct Polygon
    {
        public ushort idx1
        {
            get { return indexes[0]; }
            set { indexes[0] = value; }
        }

        public ushort idx2
        {
            get { return indexes[1]; }
            set { indexes[1] = value; }
        }

        public ushort idx3
        {
            get { return indexes[2]; }
            set { indexes[2] = value; }
        }

        // Vertices index for the 3 point of the triangle 
        private ushort[] indexes;

        public ushort[] Indexes
        {
            get { return indexes; }
            set { indexes = value; }
        }

        public Polygon(Polygon p)
        {
            indexes = new ushort[3];
            indexes[0] = p.idx1;
            indexes[1] = p.idx1;
            indexes[2] = p.idx1;
        }

        public Polygon(ushort i1, ushort i2, ushort i3)
        {
            indexes = new ushort[3];
            indexes[0] = i1;
            indexes[1] = i2;
            indexes[2] = i3;
        }
    }
}
