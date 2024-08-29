using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace water
{
    struct TileJsonData
    {
        public string name;
        public string texturePath;

    }
    public struct TileType
    {
        public byte id; //Will be same as index in array
        public string name;
        public byte type;
        public Texture2D texture;
    }
    public class TileMeta : ICloneable
    {
        public bool lightUp = false;
        public byte id;
        public TileMeta(byte id, bool lightUp = false)
        {
            this.id = id;
            this.lightUp = lightUp;
        }
        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
    public class LiquidMeta : TileMeta
    {
        //Last tick flow direction, 0 if none, -1 if left
        public sbyte lastTickFlowDirection;
        public LiquidMeta(byte id, sbyte flow) : base(id)
        {
            this.lastTickFlowDirection = flow;
        }
        public void SetFlowDirection(sbyte flow)
        {
            this.lastTickFlowDirection = flow;
        }
    }
}