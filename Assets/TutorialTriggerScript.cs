using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerScript : MonoBehaviour
{
    private bool _oneshotActivated = false;
    private TutorialUIScript _ui;

    public string Title = "Tutorial title";

    [Multiline]
    public string Text = "Tutorial Text Goes Here";
    public Sprite ImageSprite = null;
    public bool OneShot = true;

    // Start is called before the first frame update
    void Start()
    {
        _ui = GameObject.Find("Tutorial Dialog").GetComponent<TutorialUIScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!_oneshotActivated)
            {
                _oneshotActivated = true;
                _ui.ShowTutorial(Title, Text, ImageSprite);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!OneShot && _oneshotActivated)
            {
                _oneshotActivated = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
