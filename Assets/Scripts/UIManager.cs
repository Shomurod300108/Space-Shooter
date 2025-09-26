using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Text _scoreText;
    //handle to Text
    void Start()
    {
        //assign text component to the handle
        _scoreText.text = "Score:" + 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
