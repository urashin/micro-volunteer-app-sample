using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JohnTest : MonoBehaviour
{

    public TextMeshProUGUI text;
    public ProcessDeepLinkMngr mngr;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "[deeplink] " + mngr.deeplinkURL;
    }
}
