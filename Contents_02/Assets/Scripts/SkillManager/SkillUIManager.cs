using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    [SerializeField] List<SkillUI> skillUIs; // スキル選択時のボタン

    public void SelectSkillContext(SkillContext skillContext_)
    {
        if (skillContext_.condition == SkillMasks.None) return;

        foreach (var skillUI in skillUIs)
        {
            skillUI.OnChangedSkill(skillContext_);
        }
    }

    // ポーズやフェード中などの時にスキルのUI等を隠す
    public void OffObject()
    {
        foreach(var skill in skillUIs)
        {
            skill.gameObject.SetActive(false);
        }
    }


    // すべてのUIを見えるようにする
    public void ActiveObjet()
    {
        foreach (var skill in skillUIs)
        {
            skill.gameObject.SetActive(true);
        }
    }
}
