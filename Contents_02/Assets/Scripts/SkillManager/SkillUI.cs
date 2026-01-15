using UnityEngine;
using UnityEngine.EventSystems;

public class SkillUI : MonoBehaviour, ISkillChanger
{
    [SerializeField] SkillMasks skillMasks;

    public void OnChangedSkill(SkillContext skillContext)
    {
        if(skillContext.condition == skillMasks)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameObject); // ©g‚ğ‘I‘ğó‘Ô‚É‚·‚é
        }
    }
}
