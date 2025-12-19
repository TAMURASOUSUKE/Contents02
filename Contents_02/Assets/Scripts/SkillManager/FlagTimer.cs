using System;
using System.Collections;
using UnityEngine;


/*
    コルーチンを使って呼び出す側が与えたい効果を一定時間Onにするツールのような役割を持つ
*/

public static class FlagTimer
{

    class CoroutineRunner : MonoBehaviour { } // コルーチンを走らせるための空クラス
    static CoroutineRunner runner;

    // シングルトン化してコルーチン起動をできるようにする
    static CoroutineRunner Runner
    {
        get
        {
            if (runner == null)
            {
                var go = new GameObject("[Flagtimer_Runner]"); // ゲームオブジェクトを作る
                GameObject.DontDestroyOnLoad(go); // シーンをまたいでも消えないようにする
                runner = go.AddComponent<CoroutineRunner>(); // コルーチン起動するためのコンポーネントを追加
            }
            return runner; // もしすでにrunnerがあるならそのまま返して、もしないならif文を通って生成してから返す
        }
    }

    // コルーチンを走らせるための関数
    public static void ActivateCoroutine(SkillMasks targetFlag, float duration, Func<SkillMasks> getter, Action<SkillMasks> setter)
    {
        Runner.StartCoroutine(WaitAndTurnOff(targetFlag, duration, getter, setter));
    }

    // コルーチン処理を使って指定した時間後にフラグをOFFにする
    public static IEnumerator WaitAndTurnOff(SkillMasks targetFlag, float duration, Func<SkillMasks> getter, Action<SkillMasks> setter)
    {
        // 現在の状態を取得しOR演算によってフラグを混ぜる(足す)
        SkillMasks current = getter(); // getterによって現在の状態を取得
        setter(current | targetFlag); // 現在の状態と指定したフラグをOR演算することで適用する

        yield return new WaitForSeconds(duration); // 指定した時間待つ

        // 待っている間にほかのフラグが変わっている可能性を考慮して現在のフラグを再取得する
        current = getter();
        // AND NOT演算によって指定したフラグだけを解除する
        setter(current & ~targetFlag);
    }
}
