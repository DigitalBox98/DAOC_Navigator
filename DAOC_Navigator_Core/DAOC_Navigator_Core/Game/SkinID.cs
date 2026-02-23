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
	public class SkinID 
    {
		private string bodyId = "";
		private string headId = "";
        private string armsId = "";
        private string glovesId = "";
        private string lbodyId = "";
        private string legsId = "";
        private string bootsId = "";
        private string cloakId = "";
        private string faceId = "";

        public string BodyId
        {
            get { return bodyId; }
            set { bodyId = value; }
        }

        public string HeadId
        {
            get { return headId; }
            set { headId = value; }
        }

        public string ArmsId
        {
            get { return armsId; }
            set { armsId = value; }
        }

        public string GlovesId
        {
            get { return glovesId; }
            set { glovesId = value; }
        }

        public string LbodyId
        {
            get { return lbodyId; }
            set { lbodyId = value; }
        }

        public string LegsId
        {
            get { return legsId; }
            set { legsId = value; }
        }

        public string BootsId
        {
            get { return bootsId; }
            set { bootsId = value; }
        }

        public string CloakId
        {
            get { return cloakId; }
            set { cloakId = value; }
        }

        public string FaceId
        {
            get { return faceId; }
            set { faceId = value; }
        }

        public SkinID(string Id)
		{
            bodyId = Id;
        }

        public SkinID(string bodyId, string headId, string armsId, string glovesId, string lbodyId, string legsId, string bootsId, string cloakId, string faceId)
        {
            this.bodyId = bodyId;
            this.headId = headId;
            this.armsId = armsId;
            this.glovesId = glovesId;
            this.lbodyId = lbodyId;
            this.legsId = legsId;
            this.bootsId = bootsId;
            this.cloakId = cloakId;
            this.faceId = faceId;
        }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is SkinID)
            {
                SkinID id = (SkinID) obj;
                return id.BodyId.Equals(this.BodyId);
            }
            return false;

        }

        public override int GetHashCode()
        {
            return bodyId.GetHashCode();
        }
    }
}
