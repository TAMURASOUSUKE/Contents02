using UnityEngine;

public class TestTarget : MonoBehaviour
{
    [SerializeField] TargetSystem targetSystem;
    [SerializeField] SkillManager skillManager;

    void Update()
    {
        // ロックオンボタン（例：R3押し込みやTabキー）
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            targetSystem.ToggleLockOn();
        }

        // ロック中のみ左右切り替え（例：矢印キー）
        if (targetSystem.CurrentTarget != null)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)) targetSystem.SwitchTarget(1); // 右へ
            if (Input.GetKeyDown(KeyCode.LeftArrow)) targetSystem.SwitchTarget(-1); // 左へ
        }

        // 攻撃時：ターゲットシステムが捕まえている敵をSkillManagerに渡す
        if (Input.GetKeyDown(KeyCode.Space)) // 攻撃ボタン
        {
            GameObject target = targetSystem.CurrentTarget;

            // もしロックしてなかったら、従来の「一番近い敵」を簡易的に取るか、攻撃失敗にする
            if (target != null)
            {
                // ここでManagerに渡す！
                SkillContext context = new SkillContext
                {
                    user = this.gameObject,
                    target = target,
                    condition = skillManager.GetCurrentSkill()
                };
                skillManager.InputSkillContext(context);
            }
        }
    }
}
