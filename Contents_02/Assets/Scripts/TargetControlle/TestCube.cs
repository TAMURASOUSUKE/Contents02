using UnityEditor.SceneManagement;
using UnityEngine;

public class TestCube : MonoBehaviour, ISkillReceiver
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnReceiveSkill(SkillContext skillContext_)
    {
        if(skillContext_.condition == SkillMasks.Flash)
        {
            Destroy(gameObject);
        }

        if(skillContext_.condition == SkillMasks.Bomb)
        {
            gameObject.AddComponent<Rigidbody>();
        }
    }
}
