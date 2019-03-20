using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SpriteSheet
{
    [Serializable]
    public class SpriteSheetData
    {
        public SpriteData[] sprites;
    }

    [Serializable]
    public class SpriteData
    {
        public int x = 0;
        public int y = 0;
        public int width = 0;
        public int height = 0;
        public int pivotX = 0;
        public int pivotY = 0;
    }
}
