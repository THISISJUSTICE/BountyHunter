using UnityEngine;

[System.Serializable]
public class Status
{
    public int maxHealthPoint; //최대 체력
    public int armor; //방어력
    public int magicRegistant; //마법저항력
}

[System.Serializable]
public class MonsterStatus:Status{
    public float speed; //기본 속도 거리(몬스터의 경우 이동 시간)
    public float acceleration; //돌진 속도 거리
    public int maxMagicPoint; //최대 마나
    public int attackDamage; //공격력
    public int magicDamage; //마법공격력
}

[System.Serializable]
public class PlayerStatus:MonsterStatus{
    public int level; //플레이어 레벨
}

public class PlayData{
    public string nickname; //플레이어 닉네임
    PlayerStatus playerStatus;

    //각종 퀘스트, 행적
}

public class UserData{
    public string id;
    public string password;
    PlayData playData;
}