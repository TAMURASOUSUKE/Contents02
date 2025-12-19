using System;
using UnityEngine;


/*
    ステータスを決める関数を持つ管理者の役割を持つ
*/

public class StatusController
{
    // 現在のステータスの状態を保持する変数(読み取り専用)
    public SkillMasks currentCondition { get; private set; }

    // 現在の状態を決めるコンストラクタ(引数に最初に設定したい状態を指定できるがデフォルトはNone)
    public StatusController(SkillMasks initialize = SkillMasks.None)
    {
        currentCondition = initialize;
    }

    // 特定のフラグをオンにする
    public void AddEffect(SkillMasks state)
    {
        currentCondition |= state;
    }

    // 特定のフラグを解除する
    public void ReleaseEffect(SkillMasks state)
    {
        currentCondition &= ~state;
    }

    // すべてのフラグをオンにする
    public void AddAllEffects()
    {
        // SkillMasksを配列に変換しそれを格納していく
        foreach(SkillMasks s in Enum.GetValues(typeof(SkillMasks))){
            currentCondition |= s; // 足し合わせ
        }
    }

    // すべてのフラグを消す
    public void ReleaseAllEffects()
    {
        currentCondition = SkillMasks.None; // 0代入で消す
    }

    // 指定した状態を指定した時間だけ与える
    public void AddTimedEffect(SkillMasks state, float duration)
    {
        // コルーチン起動関数
        FlagTimer.ActivateCoroutine(
            state, // 指定する状態
            duration, // オフにするまでの時間
            () => currentCondition, // 現在の状態を見せるgetter
            val => currentCondition = val // それをもとに計算を行うsetter
            );

    }


    // 現在そのフラグがオンになっているかを判別するための関数
    public bool Has(SkillMasks state)
    {
        bool hasFlag = (currentCondition & state) == state; // 現在の状態と調べたい状態のANDを行って調べたい状態になるかどうかをチェックする
        return hasFlag;
    }
}
