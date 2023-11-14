//공통적으로 사용하는 것의 집합

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
}

//던전 종류를 열거형으로 표현
public enum DungeonKind{Rock};
