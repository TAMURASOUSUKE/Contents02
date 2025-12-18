using System;
using UnityEngine;


/*
    スキルの管理をするクラス
*/

public class SkillManager : MonoBehaviour
{
    // スキルのタイプ種別enum
    public enum SkillType
    {
        None = -1,  // 基準
        Buff, // バフ(攻撃力や、足の速度を上げるなど)
        Flash, // フラッシュ(スタン)
        Smoke, // (相手の視界を奪う)
        Bomb, // 爆発
        Max, // ここが種類の最後の番号になる
    }



    int skillCount = (int)SkillType.Max; // スキルの種類の数
    static int currentSkillIndex = -1; // 現在の選んでいるスキルは何かを保存する


    // 選択してるスキルを引数に入れた数の方向にずらす(正なら右、負なら左、0なら動かさない)
    public void MoveSelection(int direction)
    {
        switch (direction)
        {
            case >= 1:
                direction = 1;
                break;
            case <= -1:
                direction = -1;
                break;
            case 0:
                return; // 0の場合は動かさない
        }
        // 降順にも対応できるようにスキルの長さを足す
        currentSkillIndex = (currentSkillIndex + direction + skillCount) % skillCount;
    }


    // キーボード操作時のみ数字キーで直接ショートカットできるようにする
    public void ChooseSkill(int index)
    {
        currentSkillIndex = index;
        if(currentSkillIndex < 0) currentSkillIndex = 0; // 0より下に行かないようにする
        if(currentSkillIndex >= skillCount) currentSkillIndex = (skillCount - 1); // 種類の最後に行く(Maxの一つ前が最後のスキルになっているので計算に含める)
    }

    // currentSkillIndexから現在のスキルに変換して返す関数です
    public static SkillType GetSkill()
    {

        // 安全にキャストできるようにIsDefinedで値が型の中にあるかどうかを確認してからキャストする
        if(Enum.IsDefined(typeof(SkillType), currentSkillIndex))
        {
            Debug.Log($"現在のスキルタイプは{(SkillType)currentSkillIndex}でナンバーは{currentSkillIndex}です");
            return (SkillType)currentSkillIndex;
        }
        Debug.Log("安全にキャストできませんでした");
        return SkillType.None;
    }

    // スキルの情報をインプットする(基本的にスキル使用者側が呼び出す)
    public void InputSkillContext(SkillContext skillContext_)
    {
        if (skillContext_.target == null) return;

        ISkillReceiver receiver = skillContext_.target.GetComponent<ISkillReceiver>(); // スキルの情報の構造体内にあるターゲットに受け取る用のインターフェースを取得する

        // 持っていれば受け取り用の関数に使用者側が設定した情報を渡す
        if (receiver != null)
        {
            receiver.OnReceiveSkill(skillContext_);
        }
    }
}
