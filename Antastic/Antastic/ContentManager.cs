using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Antastic
{
    public class ContentHolder
    {
        public string ContentName;
        public Texture2D Texture;
        public Color[] TextureData;
    }

    class InternalContentManager
    {
        static List<ContentHolder> contentList = new List<ContentHolder>();

        public InternalContentManager()
        {

        }

        static public void Load()
        {
            // load all of the sprites
            LoadContentHolder("Blank", "Blank_Sprite");
            LoadContentHolder("Clear", "Clear_Sprite");
            LoadContentHolder("Gray", "greyout");
            LoadContentHolder("fingerDown", "fingerDown");
            LoadContentHolder("Apple", "Apple");
            LoadContentHolder("GreenApple", "GreenApple");
            LoadContentHolder("YellowApple", "YellowApple");
            LoadContentHolder("BraeburnApple", "BraeburnApple");
            LoadContentHolder("Ant", "ant");
            LoadContentHolder("RedAnt", "redant");
            LoadContentHolder("Bucket", "bucket");
            LoadContentHolder("Points", "Points");
            LoadContentHolder("Bird", "bird");



            // Particles
            LoadContentHolder("ExplosionParticle", "Particles/explosion");
            LoadContentHolder("SmokeParticle", "Particles/smoke");
            LoadContentHolder("StarburstParticle", "Particles/starburst");
        }

        static private void LoadContentHolder(string name, string textureLocation)
        {
            ContentHolder holder = new ContentHolder();
            holder.ContentName = name;
            holder.Texture = Antastic.sigletonGame.Content.Load<Texture2D>(System.IO.Path.Combine(@"Textures", textureLocation));
            holder.TextureData = new Color[holder.Texture.Width * holder.Texture.Height];
            holder.Texture.GetData(holder.TextureData);
            contentList.Add(holder);
        }

        static public Texture2D GetTexture(string Name)
        {
            ContentHolder contentHolder = GetContentHolder(Name);
            return contentHolder.Texture;
        }

        static public Color[] GetTextureData(string Name)
        {
            ContentHolder contentHolder = GetContentHolder(Name);
            return contentHolder.TextureData;
        }

        static private ContentHolder GetContentHolder(string Name)
        {
            ContentHolder contentHolder = contentList[0];
            int i = 0;
            for (; i < contentList.Count; i++)
            {
                if (contentList[i].ContentName == Name)
                {
                    contentHolder = contentList[i];
                    break;
                }
            }
            if (i == contentList.Count)
            {
                throw new Exception("Unfound content");
            }
            return contentHolder;
        }

    }
}