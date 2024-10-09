using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health; //HP
    public PlayerMove player;

    public void NextStage() //PlayerMove script에서 OnTriggerEnter2D 함수 쪽 Next Stage에서 사용.
    {
        stageIndex++;
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
        }
        else
        {
            // Player Die Effect
            player.OnDie();
            // Result UI
            Debug.Log("DIE!");
            // Retry Button UI
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Health Down
            HealthDown();
            // Player Reposition, 충돌 후 이동하는 좌표가 Player의 시작 좌표와 같아야 함.
            collision.attachedRigidbody.velocity = Vector2.zero; //낙하 속도 0
            collision.transform.position = new Vector3(0, 0.5f, -1);

        }
    }

    public class PlayerMove
    {
        internal void OnDie()
        {
            throw new NotImplementedException();
        }
    }
}