using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject tile;         //地图瓦片预制体
    public GameObject roadblock;    //障碍物预制体

    [Header("MapPam")]
    public Vector2 mapsize;
    [Range(0f,1f)]public float tileInterval;        //地图瓦片间距
    public int roadblockCount;                      //障碍物数量
    public Color foregroundColor, backgroundColor;  //渐变色
    public Vector2 blockHeight;                     //障碍物高度范围，x为min，y为max

    [Header("GameObject")]
    public GameObject map;

    public List<Coord> coords = new List<Coord>();

    private void Start()
    {
        CreateMap();
    }

    void CreateMap()
    {
        for (int i = 0; i < mapsize.x; i++)
        {
            for (int j = 0; j < mapsize.y; j++)
            {
                Vector3 pos = new Vector3((-mapsize.x / 2) + i, 0, (-mapsize.y / 2) + j);
                GameObject tileObj = Instantiate(tile, pos, Quaternion.Euler(90, 0, 0));
                tileObj.transform.parent = map.transform;
                tileObj.transform.localScale *= (1 - tileInterval);
                coords.Add(new Coord(i, j));
            }
        }

        Coord[] queue = Shuffle(coords.ToArray());      //洗牌后得到的队列
        //创建障碍物
        for (int i = 0; i < roadblockCount; i++)
        {
            Coord coo = queue[i];
            int ran = (int)Random.Range(blockHeight.x, blockHeight.y);
            Vector3 pos = new Vector3((-mapsize.x / 2) + coo.x, ran/2.0f, (-mapsize.y / 2) + coo.y);
            GameObject blockObj = Instantiate(roadblock, pos, Quaternion.Euler(0, 0, 0));
            blockObj.transform.parent = map.transform;
            Vector3 scale = blockObj.transform.localScale;
            scale.x *= (1 - tileInterval);
            scale.z *= (1 - tileInterval);
            scale.y *= ran;
            blockObj.transform.localScale = scale;

            //渐变色的实现
            MeshRenderer meshR = blockObj.GetComponent<MeshRenderer>();
            Material material = meshR.material;
            float offset = coo.y / mapsize.y;
            material.color = Color.Lerp(foregroundColor, backgroundColor, offset);
        }
    }

    //洗牌算法
    T[] Shuffle<T>(T[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            var tem = array[i];
            int ran = Random.Range(i, array.Length);
            array[i] = array[ran];
            array[ran] = tem;
        }
        return array;
    }
}

[System.Serializable]
public struct Coord
{
    public int x;
    public int y;

    public Coord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}