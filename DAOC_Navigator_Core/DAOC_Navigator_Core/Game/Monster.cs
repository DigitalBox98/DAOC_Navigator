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

namespace DAOC_Navigator_Core.Game
{
	public class Monster
	{

        private string nifFilename;
        private SkinSet skinSet;
        private FigurePart? head; 

        public string NIFFilename
        {
            get { return nifFilename; }
            set { nifFilename = value; }
        }

        public SkinSet SkinSet
        {
            get { return skinSet; }
            set { skinSet = value; }
        }

        public FigurePart? Head
        {
            get { return head; }
            set { head = value; }
        }

        public Monster()
        {
            this.nifFilename = "";
            this.skinSet = new SkinSet();
        }

        public Monster(string nifFilename, SkinSet skinSet)
        {
            this.nifFilename = nifFilename;
            this.skinSet = skinSet;
        }

        public Monster(string nifFilename, SkinSet skinSet, FigurePart head)
        {
            this.nifFilename = nifFilename;
            this.skinSet = skinSet;
            this.head = head;
        }

        public override string ToString()
        {
            return " nifFile = " + nifFilename + ", skinSet = " + skinSet;
        }
    }
}
