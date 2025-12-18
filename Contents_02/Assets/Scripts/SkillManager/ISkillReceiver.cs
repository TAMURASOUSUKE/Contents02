using UnityEngine;

/*
    スキルを受け取ることができるようにするインターフェース
*/

public interface ISkillReceiver
{
    void OnReceiveSkill(SkillContext skillContext_); // 受け取る際に使う関数
}
