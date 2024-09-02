using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using System.Threading;

namespace water.scripts.world
{

    public partial class LogicHandler : Node2D
    {
        private static LogicHandler instance;

        public static LogicHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LogicHandler();
                }
                return instance;
            }
        }
        public LogicHandler()
        {
            /*
            type=0 air
            type=1 solid
            type=2 liquid
            */
            allTiles = new TileType[]
            {
                new TileType { id = 0, name = "Air", type = 0, texture = null },
                new TileType { id = 1, name = "Dirt", type = 1, texture = null },
                new TileType { id = 2, name = "Water", type = 2, texture = null },
                new TileType { id = 3, name = "Sand", type = 3, texture = null },
                // new TileType { id = 3, name = "Stone", type = 1, texture = null },
            };
            //load textures
            // for (int i = 0; i < allTiles.Length; i++)
            // {
            //     try
            //     {
            //         allTiles[i].texture = (Texture2D)GD.Load("res://" + allTiles[i].name.ToLower() + ".png");
            //     }
            //     catch (Exception e)
            //     {
            //         GD.Print(e);
            //     }
            // }
        }
        private TileType[] allTiles = new TileType[0];


        public TileType GetTileType(int id)
        {
            return allTiles[id];
        }
        public bool IsLiquid(TileMeta tile)
        {
            return tile is LiquidMeta;
        }
        public bool IsEmpty(TileMeta tile)
        {
            return allTiles[tile.id].type == 0;
        }
        public bool IsSolid(TileMeta tile)
        {
            return allTiles[tile.id].type == 1;
        }
        public bool IsGranular(TileMeta tile)
        {
            return allTiles[tile.id].type == 3;
        }

        public void SwapTiles(ref TileMeta a, ref TileMeta b)
        {
            TileMeta temp = a.Clone() as TileMeta;
            a = b.Clone() as TileMeta;
            b = temp;
        }
        public TileMeta[][] GranularLogic(TileMeta[][] map)
        {
            GD.Print("Granular Logic");
            TileMeta[][] newMap = new TileMeta[map.Length][];
            for (int i = 0; i < map.Length; i++)
            {
                newMap[i] = new TileMeta[map[i].Length];
                for (int j = 0; j < map[i].Length; j++)
                {
                    newMap[i][j] = map[i][j].Clone() as TileMeta;
                }
            }
            for (int row = 0; row < map.Length; row++)
            {
                for (int column = 0; column < map[row].Length; column++)
                {
                    if (row == 0 || !IsGranular(newMap[row][column]))
                    {
                        continue;
                    }
                    //move down if empty
                    if (IsEmpty(newMap[row - 1][column]))
                    {
                        SwapTiles(ref newMap[row][column], ref newMap[row - 1][column]);
                        // TileMeta tile = newMap[row][column].Clone() as TileMeta;
                        // TileMeta fallspace = newMap[row - 1][column].Clone() as TileMeta;
                        // newMap[row - 1][column] = tile;
                        // newMap[row][column] = fallspace;
                        continue;
                    }
                    Random random = new Random();
                    int direction = random.Next(0, 2);
                    direction = (2 * direction) - 1;
                    if (IsEmpty(newMap[row - 1][column + direction]))
                    {
                        TileMeta myTile = newMap[row][column].Clone() as TileMeta;
                        TileMeta fallspaceTile = newMap[row - 1][column + direction].Clone() as TileMeta;
                        newMap[row - 1][column + direction] = myTile;
                        newMap[row][column] = fallspaceTile;
                        continue;
                    }
                    direction *= -1;
                    if (IsEmpty(newMap[row - 1][column + direction]))
                    {
                        TileMeta myTile = newMap[row][column].Clone() as TileMeta;
                        TileMeta fallspaceTile = newMap[row - 1][column + direction].Clone() as TileMeta;
                        newMap[row - 1][column + direction] = myTile;
                        newMap[row][column] = fallspaceTile;
                        continue;
                    }
                }
            }
            return newMap;
        }
        public TileMeta[][] LiquidLogic(TileMeta[][] map)
        {
            GD.Print("Liquid Logic");
            Random random = new Random();
            TileMeta[][] oldMap = map;
            TileMeta[][] newMap = new TileMeta[map.Length][];
            for (int i = 0; i < map.Length; i++)
            {
                newMap[i] = new TileMeta[map[i].Length];
                for (int j = 0; j < map[i].Length; j++)
                {
                    newMap[i][j] = map[i][j].Clone() as TileMeta;
                }
            }
            for (int row = 0; row < map.Length; row++)
            {
                for (int column = 0; column < map[row].Length; column++)
                {


                    //Check if this is  water
                    if (!IsLiquid(newMap[row][column]))
                    {
                        continue;
                    }
                    //check if there is a tile bellow
                    if (row != 0 && IsEmpty(newMap[row - 1][column]))
                    {
                        TileMeta fallspace = newMap[row - 1][column].Clone() as TileMeta;
                        LiquidMeta liquid = newMap[row][column] as LiquidMeta;

                        newMap[row][column] = fallspace;
                        newMap[row - 1][column] = liquid;
                        liquid.SetFlowDirection(0);

                        // GD.Print("Water is falling from " + row + " " + column + " to " + (row - 1) + " " + column);
                        continue;

                    }
                    //0 = left 1 = right
                    int direction = Gamemanager.Instance.forceRightFlow ? 1 : random.Next(0, 2);
                    //-1 = left 1 = right
                    direction = (2 * direction) - 1;
                    //Invert direction if we are at the edge of map
                    direction *= (column == 0 || column == map[row].Length - 1) ? -1 : 1;
                    //Amount of liquid in the to right (including the current tile). 
                    int liquidTrain = 0;
                    // bool leftOfTrainEmpty = column == 0 ? false : IsEmpty(map[row][column - 1]);

                    bool canMoveRight = false;//direction == -1 && leftOfTrainEmpty;

                    for (int i = column; i < map[row].Length; i++)
                    {
                        GD.Print("Checking: " + i);
                        //TODO: ADD IF RIGHT WILL DROP
                        //TODO CHECK IF SOMETHING WILL DROP INTO RIGHT/LEFT EMPTY (THEN COMPLETE THAT ACTION)

                        //Check if at mapedge or solid
                        if (i == map[row].Length - 1 || IsSolid(map[row][i + 1]))
                        {
                            //We hit solid/edge stop the train, TODO:Invert direction
                            liquidTrain++;
                            direction = -1; //Invert direction incase left is a option
                            canMoveRight = false;
                            break;
                        }
                        //Check if another liquid
                        if (IsLiquid(map[row][i + 1]))
                        {
                            //Continue checking
                            liquidTrain++;
                            if (row != 0 && IsEmpty(map[row - 1][i]))
                            {
                                GD.Print("Dropping down from: " + row + " " + i + " to " + (row - 1) + " " + i);
                                //TODO: Make the liquid fall, and move train
                                LiquidMeta liquid = map[row][i].Clone() as LiquidMeta;
                                TileMeta fallspace = map[row - 1][i].Clone() as TileMeta;
                                map[row][i] = fallspace;
                                map[row - 1][i] = liquid;
                                canMoveRight = true;
                                break;
                            }
                            continue;

                        }
                        if (IsEmpty(map[row][i + 1]))
                        {
                            //It's free Push the train
                            liquidTrain++;
                            canMoveRight = true; // if we wanna go right we know we can
                            break;
                        }
                    }
                    if (direction == 1 && canMoveRight)
                    {
                        GD.Print("Liquid Train: " + liquidTrain + " Can Move: " + canMoveRight);
                        GD.Print("From: " + column + " to: " + (column + liquidTrain - 1));

                        for (int i = (column + liquidTrain - 1); i > (column - 1); i--)
                        {
                            TileMeta nextTileMeta = newMap[row][i + 1].Clone() as TileMeta;
                            TileMeta myTileMeta = newMap[row][i].Clone() as TileMeta;
                            newMap[row][i] = nextTileMeta;
                            newMap[row][i + 1] = myTileMeta;
                            if (IsLiquid(myTileMeta))
                                (myTileMeta as LiquidMeta).SetFlowDirection(1);
                            // GD.Print("Swapped at col" + i + " " + GetTileType(myTileMeta.id).name + " to " + (i + 1) + " " + GetTileType(nextTileMeta.id).name);
                        }
                        GD.Print("Moved right from: " + column + " to: " + (column + liquidTrain - 1));
                        //Skip the train as those have been checked
                        column += liquidTrain;
                        continue;
                    }
                    if (direction == 1)
                    {
                        GD.Print("Couldn't move right, shouldnt be here");
                        //Tried to move right but couldn't, nothing inverted our direction
                        column += liquidTrain;
                        continue;
                    }
                    //If we are here we are moving left
                    bool canMoveLeft = false;
                    if (IsEmpty(newMap[row][column - 1]))
                    {
                        canMoveLeft = true;
                    }
                    else if (IsEmpty(newMap[row - 1][column - 1]))
                    {
                        //Drop down then move left
                        //canMoveLeft = true;
                    }
                    if (canMoveLeft)
                    {

                        for (int i = column; i < column + liquidTrain; i++)
                        {
                            TileMeta nextTileMeta = newMap[row][i - 1].Clone() as TileMeta;
                            TileMeta myTileMeta = newMap[row][i].Clone() as TileMeta;
                            newMap[row][i] = nextTileMeta;
                            newMap[row][i - 1] = myTileMeta;
                            if (IsLiquid(myTileMeta))
                                (myTileMeta as LiquidMeta).SetFlowDirection(-1);
                            GD.Print("Swapped at col" + i + " " + GetTileType(myTileMeta.id).name + " to " + (i - 1) + " " + GetTileType(nextTileMeta.id).name);
                        }
                        column += liquidTrain;
                        continue;
                    }

                    //Skip the train as those have been checked, if here couldn't move left or right
                    column += liquidTrain;



                }
            }

            return newMap;
        }


    }
}