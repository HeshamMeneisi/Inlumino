using System;

namespace Inlumino_SHARED
{
    internal class StaticObjectCreator
    {
        internal static StaticObject CreateObject(ObjectType objectType, Tile parent)
        {
            switch (objectType)
            {
                default: return null;

                case ObjectType.LightSource: return new LightSourceObject(DataHandler.ObjectTextureMap[ObjectType.LightSource], parent);

                case ObjectType.Prism: return new Prism(DataHandler.ObjectTextureMap[ObjectType.Prism], parent);

                case ObjectType.Block: return new Block(DataHandler.ObjectTextureMap[ObjectType.Block], parent);

                case ObjectType.Crystal: return new Crystal(DataHandler.ObjectTextureMap[ObjectType.Crystal], parent);

                case ObjectType.Splitter: return new Splitter(DataHandler.ObjectTextureMap[ObjectType.Splitter], parent);

                case ObjectType.Portal:return new Portal(DataHandler.ObjectTextureMap[ObjectType.Portal], parent);
            }
        }
    }
}