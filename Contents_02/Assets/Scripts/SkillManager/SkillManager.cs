using UnityEngine;




public class SkillManager : MonoBehaviour
{

    // スキルのタイプ種別enum
    public enum SkillType
    {
        None, // 基準
        Buff, // バフ(攻撃力や、足の速度を上げるなど)
        Flash, // フラッシュ(スタン)
        Smoke, // (相手の視界を奪う)
        Bomb, // 爆発
    }

    SkillContext skillContext;

    void Start()
    {
        
    }

   
    void Update()
    {
        
    }

    void InputSkillContext(SkillContext skillContext_)
    {
        skillContext = skillContext_;
    }
}
