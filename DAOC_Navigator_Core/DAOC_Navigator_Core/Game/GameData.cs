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

using System.Collections;
using System.Diagnostics;

namespace DAOC_Navigator_Core.Game
{
    public class GameData
    {
        public static string GAMEDATA_FILE      = "gamedata.mpk";
        public static string MONSTERS_FILE      = "monsters.csv";
        public static string MONNIFS_FILE       = "monnifs.csv";
        public static string SKINS_FILE         = "skins.csv";
        public static string FIG3_FIGMAIN_FILE  = "fig3tofigmain.csv";
        public static string FIG3_FACEMAP_FILE  = "fig3facemap.csv";
        public static string FIG3_PARTS_FILE    = "fig3parts.csv";
        public static string FIGURES_FOLDER     = "figures";
        public static string FIG3_FOLDER        = "fig3";

        private Hashtable monsterIDToSkinIDS  = new();
        private Hashtable monsterIDToNifID    = new();
        private Hashtable nifNameToNifID      = new();
        private Hashtable nifIDToNifName      = new();
        private Hashtable skinIDToSkin        = new();
        private Hashtable idToDesc            = new();
        private Hashtable descToSkindID       = new();
        private Hashtable textToFigurePart    = new();
        private Hashtable monsterIdToMonster  = new();

        private string[]? figuresFiles;
        private int maxMonsterId = 0;
        private bool isDefined   = false;

        public string RemoveWhitespace(string str)
            => string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

        public int       getMaxMonsterId()    => maxMonsterId;
        public Hashtable MonstersMapping      { get => monsterIdToMonster; set => monsterIdToMonster = value; }
        public string[]? FiguresFiles         { get => figuresFiles;       set => figuresFiles       = value; }
        public bool      IsDefined            { get => isDefined;          set => isDefined          = value; }

        public GameData(string daocLocation)
        {
            if (string.IsNullOrEmpty(daocLocation)) return;

            // FIX #13: Path.Combine replaces manual separator concatenation.
            string gamedatafile = Path.Combine(daocLocation, GAMEDATA_FILE);
            if (!File.Exists(gamedatafile)) return;

            // ---- fig3tofigmain.csv  (NifID → Desc) ----
            using (var mpk = new PAKFile(gamedatafile))
            {
                var fstream = new MemoryStream();
                mpk.ExtractFile(FIG3_FIGMAIN_FILE, fstream);
                using var dataReader = new StreamReader(fstream, System.Text.Encoding.UTF8, true);
                int lineno = 1;
                string? line;
                while ((line = dataReader.ReadLine()) != null)
                {
                    if (lineno > 2)
                    {
                        string[] subs = line.Split(',');
                        string ID   = subs[5];
                        string Desc = RemoveWhitespace(subs[0]);
                        if (!idToDesc.Contains(ID))
                            idToDesc.Add(ID, Desc.ToLower());
                    }
                    lineno++;
                }
            }

            // ---- fig3facemap.csv  (Desc → SkinID) ----
            using (var mpk = new PAKFile(gamedatafile))
            {
                var fstream = new MemoryStream();
                mpk.ExtractFile(FIG3_FACEMAP_FILE, fstream);
                using var dataReader = new StreamReader(fstream, System.Text.Encoding.UTF8, true);
                int lineno = 1;
                string? line;
                while ((line = dataReader.ReadLine()) != null)
                {
                    if (lineno > 2)
                    {
                        string[] subs = line.Split(',');
                        string Desc   = RemoveWhitespace(subs[1]);
                        string SkinID = subs[2];
                        if (!descToSkindID.Contains(Desc.ToLower()))
                            descToSkindID.Add(Desc.ToLower(), SkinID);
                    }
                    lineno++;
                }
            }

            // ---- fig3parts.csv  (Text → FigurePart) ----
            using (var mpk = new PAKFile(gamedatafile))
            {
                var fstream = new MemoryStream();
                mpk.ExtractFile(FIG3_PARTS_FILE, fstream);
                using var dataReader = new StreamReader(fstream, System.Text.Encoding.UTF8, true);
                int lineno = 1;
                string? line;
                while ((line = dataReader.ReadLine()) != null)
                {
                    if (lineno > 2)
                    {
                        string[] subs = line.Split(',');
                        string Text       = RemoveWhitespace(subs[1]);
                        string NIFHead    = subs[2];
                        string ArchiveNum = subs[5];
                        if (!textToFigurePart.Contains(Text.ToLower()))
                            textToFigurePart.Add(Text.ToLower(), new FigurePart(NIFHead, ArchiveNum));
                    }
                    lineno++;
                }
            }

            // ---- monsters.csv  (MonsterID → NifID + SkinIDs) ----
            PAKFile? lastMpk = null;
            try
            {
                lastMpk = new PAKFile(gamedatafile);
                var fstream = new MemoryStream();
                lastMpk.ExtractFile(MONSTERS_FILE, fstream);
                using var dataReader = new StreamReader(fstream, System.Text.Encoding.UTF8, true);
                int lineno = 1;
                string? line;
                while ((line = dataReader.ReadLine()) != null)
                {
                    if (lineno > 2)
                    {
                        string[] subs = line.Split(',');
                        string ID            = subs[0];
                        string NifID         = subs[2];
                        string SkinBodyID    = subs[3];
                        string SkinHeadID    = subs[4];
                        string SkinArmsID    = subs[5];
                        string SkinGlovesID  = subs[6];
                        string SkinLbodyID   = subs[7];
                        string SkinLegsID    = subs[8];
                        string SkinBootsID   = subs[9];
                        string SkinCloakID   = subs[10];
                        string FaceID        = subs[18];
                        string CataNifID     = subs[67];
                        string CataSkinBody  = subs[68];
                        string CataSkinHead  = subs[69];
                        string CataSkinArms  = subs[70];
                        string CataSkinGlv   = subs[71];
                        string CataSkinLBody = subs[72];
                        string CataSkinLegs  = subs[73];
                        string CataSkinBoots = subs[74];
                        string CataSkinCloak = subs[75];

                        if (!string.IsNullOrEmpty(CataNifID) &&
                            (FaceID == "0" || string.IsNullOrEmpty(FaceID)))
                        {
                            NifID       = CataNifID;
                            SkinBodyID  = CataSkinBody;
                            SkinHeadID  = CataSkinHead;
                            SkinArmsID  = CataSkinArms;
                            SkinGlovesID= CataSkinGlv;
                            SkinLbodyID = CataSkinLBody;
                            SkinLegsID  = CataSkinLegs;
                            SkinBootsID = CataSkinBoots;
                            SkinCloakID = CataSkinCloak;
                        }

                        string SkinFaceID = string.Empty;
                        if (FaceID != "0" && !string.IsNullOrEmpty(FaceID))
                        {
                            var desc = idToDesc[ID] as string ?? string.Empty;
                            SkinFaceID = descToSkindID[desc] as string ?? string.Empty;
                        }

                        if (!monsterIDToSkinIDS.Contains(ID))
                            monsterIDToSkinIDS.Add(ID, new SkinID(SkinBodyID, SkinHeadID, SkinArmsID,
                                SkinGlovesID, SkinLbodyID, SkinLegsID, SkinBootsID, SkinCloakID, SkinFaceID));

                        if (!monsterIDToNifID.Contains(ID))
                            monsterIDToNifID.Add(ID, NifID);
                    }
                    lineno++;
                }

                // ---- monnifs.csv  (NifID → NifName) ----
                fstream = new MemoryStream();
                lastMpk.ExtractFile(MONNIFS_FILE, fstream);
                using (var dataReader2 = new StreamReader(fstream, System.Text.Encoding.UTF8, true))
                {
                    lineno = 1;
                    string? line2;
                    while ((line2 = dataReader2.ReadLine()) != null)
                    {
                        if (lineno > 2)
                        {
                            string[] subs = line2.Split(',');
                            string NifID  = subs[0];
                            string NifName= subs[2];
                            if (!nifNameToNifID.Contains(NifName))
                                nifNameToNifID.Add(NifName, NifID);
                            if (!nifIDToNifName.Contains(NifID))
                                nifIDToNifName.Add(NifID, NifName);
                        }
                        lineno++;
                    }
                }

                // ---- skins.csv  (SkinID → Skin) ----
                fstream = new MemoryStream();
                lastMpk.ExtractFile(SKINS_FILE, fstream);
                using (var dataReader3 = new StreamReader(fstream, System.Text.Encoding.UTF8, true))
                {
                    lineno = 1;
                    string? line3;
                    while ((line3 = dataReader3.ReadLine()) != null)
                    {
                        if (lineno > 2)
                        {
                            string[] subs = line3.Split(',');
                            string SkinID    = subs[0];
                            string SkinName  = subs[2];
                            string ArchiveNum= subs[4];
                            int num = 0;
                            if (ArchiveNum != " " && ArchiveNum != "")
                                int.TryParse(ArchiveNum, out num);

                            if (!skinIDToSkin.Contains(SkinID))
                                skinIDToSkin.Add(SkinID, new Skin(SkinName, num.ToString("000")));
                        }
                        lineno++;
                    }
                }
            }
            finally
            {
                lastMpk?.Dispose();
            }

            // ---- Build monsterId → Monster mapping ----
            foreach (object monsterId in monsterIDToNifID.Keys)
            {
                if (monsterIDToNifID[monsterId] is not string keyNifId)    continue;
                if (monsterIDToSkinIDS[monsterId] is not SkinID keySkinId) continue;
                if (!skinIDToSkin.ContainsKey(keySkinId.BodyId))           continue;

                var bodySkin   = skinIDToSkin[keySkinId.BodyId]   as Skin;
                if (bodySkin == null) continue;

                var headSkin   = skinIDToSkin[keySkinId.HeadId]   as Skin;
                var armsSkin   = skinIDToSkin[keySkinId.ArmsId]   as Skin;
                var glovesSkin = skinIDToSkin[keySkinId.GlovesId] as Skin;
                var lbodySkin  = skinIDToSkin[keySkinId.LbodyId]  as Skin;
                var legsSkin   = skinIDToSkin[keySkinId.LegsId]   as Skin;
                var bootsSkin  = skinIDToSkin[keySkinId.BootsId]  as Skin;
                var cloakSkin  = skinIDToSkin[keySkinId.CloakId]  as Skin;
                var faceSkin   = skinIDToSkin[keySkinId.FaceId]   as Skin;

                var skinSet = new SkinSet(bodySkin, headSkin, armsSkin, glovesSkin,
                                          lbodySkin, legsSkin, bootsSkin, cloakSkin, faceSkin);

                if (!nifIDToNifName.ContainsKey(keyNifId)) continue;
                if (nifIDToNifName[keyNifId] is not string nifName) continue;

                Monster mons;
                if (idToDesc.Contains(monsterId) &&
                    idToDesc[monsterId] is string desc &&
                    textToFigurePart.Contains(desc + "head") &&
                    textToFigurePart[desc + "head"] is FigurePart figureHead &&
                    skinSet.FaceSkin != null)
                {
                    var newSkinSet = new SkinSet(skinSet.FaceSkin);
                    mons = new Monster(nifName, newSkinSet, figureHead);
                }
                else
                {
                    mons = new Monster(nifName, skinSet);
                }

                monsterIdToMonster.Add(monsterId, mons);

                if (int.TryParse(monsterId.ToString(), out int currentId) && maxMonsterId < currentId)
                    maxMonsterId = currentId;

                Debug.Write($"Id:{monsterId}, Monster: {mons}");
            }

            Debug.Write("Gamedata loaded");

            // FIX #13: Path.Combine
            figuresFiles = Directory.GetFiles(Path.Combine(daocLocation, FIGURES_FOLDER));
            isDefined    = true;
        }
    }
}
