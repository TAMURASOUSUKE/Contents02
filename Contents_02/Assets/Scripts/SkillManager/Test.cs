using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] SkillManager manager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            manager.MoveSelection(-1);
            SkillManager.GetSkill();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            manager.MoveSelection(1);
            SkillManager.GetSkill();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            manager.ChooseSkill(2);
            SkillManager.GetSkill();
        }
    }
}
