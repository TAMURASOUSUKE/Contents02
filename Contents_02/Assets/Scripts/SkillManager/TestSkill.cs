using UnityEngine;

public class TestSkill : MonoBehaviour
{
    [SerializeField] SkillManager manager;
    StatusController statusController; // ステータスのコントローラー
    void Start()
    {
        statusController = new StatusController();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            manager.MoveSelection(-1);
            manager.GetCurrentSkill();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            manager.MoveSelection(1);
            manager.GetCurrentSkill();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            manager.ChooseSkill(2);
            manager.GetCurrentSkill();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            statusController.AddTimedEffect(manager.GetCurrentSkill(), 5.0f);
            Debug.Log($"効果を発動した。効果は{manager.GetCurrentSkill()}です");
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            statusController.AddAllEffects();
            Debug.Log("すべての効果を付与しました。");

            if (statusController.Has(SkillMasks.SpeedUp)) Debug.Log($"{SkillMasks.SpeedUp}この効果は含まれています");
            if (statusController.Has(SkillMasks.Flash)) Debug.Log($"{SkillMasks.Flash}この効果は含まれています");
            if (statusController.Has(SkillMasks.Smoke)) Debug.Log($"{SkillMasks.Smoke}この効果は含まれています");
            if (statusController.Has(SkillMasks.Bomb)) Debug.Log($"{SkillMasks.Bomb}この効果は含まれています");
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            statusController.ReleaseAllEffects();
            Debug.Log("すべての効果を削除しました。");

            if (!statusController.Has(SkillMasks.SpeedUp)) Debug.Log($"{SkillMasks.SpeedUp}この効果は含まれていません");
            if (!statusController.Has(SkillMasks.Flash)) Debug.Log($"{SkillMasks.Flash}この効果は含まれていません");
            if (!statusController.Has(SkillMasks.Smoke)) Debug.Log($"{SkillMasks.Smoke}この効果は含まれていません");
            if (!statusController.Has(SkillMasks.Bomb)) Debug.Log($"{SkillMasks.Bomb}この効果は含まれていません");
        }
    }
}
