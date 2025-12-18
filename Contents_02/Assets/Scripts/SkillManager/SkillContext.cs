using UnityEngine;
using UnityEngine.Rendering.Universal;


/*
    使用者や対象等の情報を持つ構造体    
*/
public struct SkillContext
{
    public GameObject user; // 使用者
    public GameObject target; // 対象
    public Vector3 hitPosition; // 当たった位置
    public SkillManager.SkillType skillType; // スキルのタイプ
}
