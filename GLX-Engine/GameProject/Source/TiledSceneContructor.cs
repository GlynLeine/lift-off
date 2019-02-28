﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TiledMapParser;
//using GLXEngine;

//namespace GameProject
//{
//    class TiledSceneContructor
//    {
//        static Map map;

//        static public void LoadObjects(ref Scene a_scene, string a_mapFile)
//        {
//            map = tileMapLoad(a_mapFile);
//            int[,] backgroundTiles = tilesLoad(1);
//            int[,] collisionTiles = tilesLoad(0);
//            List<TiledObject> tiledObjects = new List<TiledObject>(objectsLoad());
//            string tileSheetFileName = map.TileSets[0].Source;
//            TileSet tileSet = TiledParser.ReadTileSet(tileSheetFileName);
//            List<string> objectTypes = new List<string>();
//            tiledObjects.ForEach(tiledObject => { objectTypes.Add(tiledObject.Type); });

//            //load collision tiles
//            for (int i = 0; i < collisionTiles.GetLength(1); i++)
//            {
//                for (int j = 0; j < collisionTiles.GetLength(0); j++)
//                {
//                    if (collisionTiles[j, i] == 0) continue;
//                    AnimationSprite sprite = new AnimationSprite(tileSet.Image.FileName, tileSet.Columns, tileSet.Rows);
//                    sprite.SetFrame(collisionTiles[j, i]);
//                    WallTile tile = new WallTile(a_scene, sprite);
//                    tile.x = j * map.TileWidth;
//                    tile.y = i * map.TileHeight;
//                    a_scene.AddChild(tile);
//                }
//            }

//            //load background tiles
//            for (int i = 0; i < backgroundTiles.GetLength(1); i++)
//            {
//                for (int j = 0; j < backgroundTiles.GetLength(0); j++)
//                {
//                    if (backgroundTiles[j, i] == 0) continue;
//                    AnimationSprite sprite = new AnimationSprite(tileSet.Image.FileName, tileSet.Columns, tileSet.Rows);
//                    sprite.SetFrame(backgroundTiles[j, i]);
//                    BackgroundTile tile = new BackgroundTile(a_scene, sprite);
//                    tile.x = j * map.TileWidth;
//                    tile.y = i * map.TileHeight;
//                    a_scene.AddChild(tile);
//                }
//            }

//            //load objectTypes
//            for (int i = 0; i < objectTypes.Count; i++)
//            {
//                if (objectTypes[i] == null) continue;

//                GameObject gameObject = null;
//                /*
//                if (objectTypes[i] == "Player")
//                    gameObject = player = new Player();
//                */
//                if (gameObject == null) continue;

//                a_scene.AddChild(gameObject);
//                gameObject.x = tiledObjects[i].X;
//                gameObject.y = tiledObjects[i].Y;
//            }
//        }

//        static private Map tileMapLoad(string filename)
//        {
//            Map _map = TiledParser.ReadMap(filename);
//            return _map;
//        }

//        static private int[,] tilesLoad(int layer)
//        {
//            int[,] output = new int[map.Width, map.Height];
//            string[] tiles = map.Layers[layer].Data.innerXML.Split(',');
//            for (int i = 0; i < map.Height; i++)
//            {
//                for (int j = 0; j < map.Width; j++)
//                {
//                    output[j, i] = Convert.ToInt32(tiles[i * map.Width + j]);
//                }
//            }
//            return output;
//        }

//        static private TiledObject[] objectsLoad()
//        {
//            return map.ObjectGroups[0].Objects;
//        }
//    }
//}
