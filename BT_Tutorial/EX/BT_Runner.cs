using UnityEngine;
using demonviglu.bt;
using UnityEditor;

public class BT_Runner : MonoBehaviour
{

    public BehaviorTree Tree; 
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Tree.Tick();
    }
}
