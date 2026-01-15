using UnityEngine;

/*
    スキルを受け取ることができるようにするインターフェース
*/

// 発動時
public interface ISkillReceiver
{
    void OnReceiveSkill(SkillContext skillContext_); // 受け取る際に使う関数
}


// 選択切り替え時
public interface ISkillChanger
{
    void OnChangedSkill(SkillContext skillContext); // 受け取るときに使う関数
}