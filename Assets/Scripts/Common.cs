//공통적으로 사용하는 것의 집합

using System.Numerics;
using UnityEngine;

public static class Common{
    public static float floorHorizontal{ //가로 1칸 당 길이
        get{
            return 1.8f;
        }
    }
    public static float floorVertical{ //세로 1칸 당 길이
        get{
            return 3.5f;
        }
    }
    public static UnityEngine.Vector2 CoorToVec(int row, int col){ //좌표 인덱스를 받아 포지션값을 반환
        UnityEngine.Vector2 res;
        int mapSpace = GameManager.Inst.curDungeonInfo[0].Count;
        float posx = floorHorizontal*(mapSpace / 2) - col*floorHorizontal;
        float posz = 10 + row*floorVertical;
        res = new UnityEngine.Vector2(posx, posz);
        return res;
    }

    public static int[] VecToCoor(float posx, float posz){ //포지션값을 받아 가장 가까운 좌표 인덱스로 반환
        int[] res = new int[2];
        int mapSpace = GameManager.Inst.curDungeonInfo[0].Count;
        
        res[0] = (int)Mathf.Round(posx / floorHorizontal) + (mapSpace / 2);
        if(res[0] < 0) res[0] = 0;
        else if(res[0] >= mapSpace) res[0] = mapSpace-1;

        res[1] = (int)Mathf.Round((posz-10) / floorVertical);

        return res;
    }
}

//던전 종류를 열거형으로 표현
public enum DungeonKind{Rock};
