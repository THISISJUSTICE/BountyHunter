using UnityEngine;

[System.Serializable]
public class Status
{
    public int maxHealthPoint; //최대 체력
    public int armor; //방어력
    public int magicRegistant; //마법저항력

    //받는 피해량 계산
    public int CalculateDamage(int attackDamage, int magicDamage){
        int dmg = (int)Mathf.Ceil((float)attackDamage/armor) + (int)Mathf.Ceil((float)magicDamage/magicRegistant);
        return dmg * 5;
    }

    //돌진 시 받는 데미지 계산
    public int RushDamaged(int myArmor, int matchArmor, float acceleration){
        if(myArmor / 2 < matchArmor){
            return matchArmor/myArmor * (int)(acceleration *20);
            //HPDecrese(armor / obstacleStatus.armor * (int)(acceleration * 20));
        }
        return 0;
    }

}

[System.Serializable]
public class MonsterStatus:Status{
    public float speed; //기본 속도
    public int maxMagicPoint; //최대 마나
    public int attackDamage; //공격력
    public int magicDamage; //마법공격력
}

[System.Serializable]
public class PlayerStatus:MonsterStatus{
    public int level; //플레이어 레벨
    public float acceleration; //플레이어 돌진 속도
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