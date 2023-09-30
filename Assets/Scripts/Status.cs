[System.Serializable]
public class Status
{
    public int maxHealthPoint; //최대 체력
    public int armor; //방어력
    public int magicRegistant; //마법저항력
}

[System.Serializable]
public class PlayerStatus:Status{
    public int level; //플레이어 레벨
    public float speed; //플레이어 기본 속도
    public float acceleration; //플레이어 돌진 속도
    public int maxMagicPoint; //플레이어 최대 마나
    public int attackDamage; //플레이어 공격력
    public int magicDamage; //플레이어 마법공격력
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
