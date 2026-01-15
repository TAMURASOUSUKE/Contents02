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
}
