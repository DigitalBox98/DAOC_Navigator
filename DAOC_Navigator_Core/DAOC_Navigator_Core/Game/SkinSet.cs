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
	public class SkinSet
	{
        private Skin bodySkin;
        private Skin? headSkin;
        private Skin? armsSkin;
        private Skin? glovesSkin;
        private Skin? lbodySkin;
        private Skin? legsSkin;
        private Skin? bootsSkin;
        private Skin? cloakSkin;
        private Skin? faceSkin;

        public Skin BodySkin
        {
            get { return bodySkin; }
            set { bodySkin = value; }
        }

        public Skin? HeadSkin
        {
            get { return headSkin; }
            set { headSkin = value; }
        }

        public Skin? ArmsSkin
        {
            get { return armsSkin; }
            set { armsSkin = value; }
        }

        public Skin? GlovesSkin
        {
            get { return glovesSkin; }
            set { glovesSkin = value; }
        }

        public Skin? LbodySkin
        {
            get { return lbodySkin; }
            set { lbodySkin = value; }
        }

        public Skin? LegsSkin
        {
            get { return legsSkin; }
            set { legsSkin = value; }
        }

        public Skin? BootsSkin
        {
            get { return bootsSkin; }
            set { bootsSkin = value; }
        }

        public Skin? CloakSkin
        {
            get { return cloakSkin; }
            set { cloakSkin = value; }
        }

        public Skin? FaceSkin
        {
            get { return faceSkin; }
            set { faceSkin = value; }
        }

        //
        //
        public SkinSet()
        {
            this.bodySkin = new Skin();
        }

        public SkinSet(Skin bodySkin)
		{
            this.bodySkin = bodySkin;
        }

        public SkinSet(Skin bodySkin, Skin? headSkin, Skin? armsSkin, Skin? glovesSkin, Skin? lbodySkin, Skin? legsSkin, Skin? bootsSkin, Skin? cloakSkin, Skin? faceSkin)
        {
            this.bodySkin = bodySkin;
            this.headSkin = headSkin;
            this.armsSkin = armsSkin;
            this.glovesSkin = glovesSkin;
            this.lbodySkin = lbodySkin;
            this.legsSkin = legsSkin;
            this.bootsSkin = bootsSkin;
            this.cloakSkin = cloakSkin;
            this.faceSkin = faceSkin;
        }

        public override string ToString()
        {
            return bodySkin.ToString();
        }
    }
}
