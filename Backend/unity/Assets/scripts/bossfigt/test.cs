using UnityEngine;

public class BattleTestTrigger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            BattleManager.Instance.StartTestBattle();

            if (Input.GetKeyDown(KeyCode.M))
    CoinManager.Instance.Add(10);
    }
}