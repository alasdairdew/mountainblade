using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBlade
{
    interface Entity
    {
        Vector2 Position { get; }
        float Size { get; }

        void Draw(SpriteBatch spriteBatch);
    }
}
