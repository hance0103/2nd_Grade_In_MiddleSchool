using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    public static Blink instance = null;
    public static Blink Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void StartBlink()
    {
        StartCoroutine(Blinking());
    }
    public IEnumerator Blinking()
    {
        ScreenGrayscale.SetGrayscale(true, 0.1f);
        yield return new WaitForSeconds(0.1f);
        ScreenGrayscale.SetGrayscale(false, 0.1f);
        yield return new WaitForSeconds(0.1f);
        ScreenGrayscale.SetGrayscale(true, 0.1f);
        yield return new WaitForSeconds(0.1f);
        ScreenGrayscale.SetGrayscale(false, 0.1f);
        yield return new WaitForSeconds(0.1f);
        ScreenGrayscale.SetGrayscale(true, 0.1f);
        yield return new WaitForSeconds(0.1f);
        ScreenGrayscale.SetGrayscale(false, 0.1f);
        yield return new WaitForSeconds(0.1f);
    }
    void Update()
    {
        
    }
}
