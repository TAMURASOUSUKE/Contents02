using System;
using System.Linq;
using UnityEngine;


/*
    スキルの管理をするクラス
*/

// バフや敵に対してのコマンド効果をまとめたマスク群
[Flags]
public enum SkillMasks
{
    None = 0, // ビットフラグ基準のため0にする
    SpeedUp = 1 << 0, // 1 (2進数 : 0001)
    Flash = 1 << 1,　// 2 (2進数 : 0010)
    Smoke = 1 << 2, // 4 (2進数 : 0100)
    Bomb = 1 << 3, // 8 (2進数 : 1000)
}


public class SkillManager : MonoBehaviour
{
    SkillMasks[] skillArray; // スキルの種類をまとめる用の配列
    int currentSkillIndex = -1; // 現在の選んでいるスキルは何かを保存する

    private void Awake()
    {
        // 配列にスキルの種類を格納するcastでSkillMask型に変換してNoneを排除して配列を形成する
       skillArray = Enum.GetValues(typeof(SkillMasks)).Cast<SkillMasks>().Where(s => s != SkillMasks.None).ToArray();
        // 初期選択は0にする
        if (skillArray.Length > 0) currentSkillIndex = 0;
    }

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
        int len = skillArray.Length;
        currentSkillIndex = (currentSkillIndex + direction + len) % len;
    }


    // キーボード操作時のみ数字キーで直接ショートカットできるようにする
    public void ChooseSkill(int index)
    {
        if(skillArray == null) return;
        // 選択できる範囲にclampする
        currentSkillIndex = Mathf.Clamp(index, 0, skillArray.Length - 1);
    }


    // 現在選択中のスキルを返す
    public SkillMasks GetCurrentSkill()
    {
        if(skillArray == null || skillArray.Length == 0) return SkillMasks.None;
        Debug.Log($"現在選択中のスキルは{skillArray[currentSkillIndex]}");
        return skillArray[currentSkillIndex];
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
