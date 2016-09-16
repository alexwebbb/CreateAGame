using UnityEngine;
using System.Collections;

public class MuzzleFlash : MonoBehaviour {

    public GameObject flashObject;
    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderers;

    public float flashTime;

	void Start () {
        Deactivate();
	}
	
    public void Activate() {
        flashObject.SetActive(true);

        int flashSpriteIndex = Random.Range(0, flashSprites.Length - 1);
        for(int i = 0; i < spriteRenderers.Length; i++) {
            spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
        }

        Invoke("Deactivate", flashTime);
    }

    void Deactivate() {
        flashObject.SetActive(false);
    }
}
