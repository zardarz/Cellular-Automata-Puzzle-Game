using UnityEngine;

public class bleh : MonoBehaviour
{
    void Start() {
        SoundManager.Play("yay" + Random.Range(1,3).ToString());
    }

    void FixedUpdate() {
        SoundManager.Play("yay" + Random.Range(1,3).ToString());
    }
}