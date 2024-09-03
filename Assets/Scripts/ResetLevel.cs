using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(CollectBattery(other.gameObject));
        }
    }

    IEnumerator CollectBattery(GameObject _player)
    {
        Debug.Log($"Level Restarted!");
        yield return null;
        //yield return new WaitForSeconds(disableTime);
        _player.transform.position = new Vector3(-15f, 0.7f, 6.8f);
    }
}
