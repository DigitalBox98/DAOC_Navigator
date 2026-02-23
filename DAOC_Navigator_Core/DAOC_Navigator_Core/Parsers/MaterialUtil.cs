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

using DAOC_Navigator_Core.WorldObjects;
using System.Collections;

namespace DAOC_Navigator_Core.Parsers
{
	public class MaterialUtil
	{

		public static uint BuildUniqueMaterial(ShapeMaterial sm, ref uint i, Hashtable hashMaterial, List<ShapeMaterial> uniqueShapeMaterial)
        {
            uint resultid = 0;
            if (!hashMaterial.ContainsKey(sm))
            {
                resultid = i;
                uniqueShapeMaterial.Add(sm);
                hashMaterial.Add(sm, i);
                i++;
            }
            else
            {
                if (hashMaterial[sm] is uint result)
                {
                    resultid = result;
                }
            }
            return resultid;
        }
    }
}
