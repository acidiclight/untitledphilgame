using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIScript : MonoBehaviour
{
    private GameObject _panel;

    private TextMeshProUGUI _title;
    private TextMeshProUGUI _message;
    private Image _image;

    // Start is called before the first frame update
    void Start()
    {
        _panel = this.gameObject.transform.Find("Panel").gameObject;

        _title = _panel.transform.Find("Title").gameObject.GetComponent<TextMeshProUGUI>();
        _message = _panel.transform.Find("Tutorial Text").gameObject.GetComponent<TextMeshProUGUI>();

        _image = _panel.transform.Find("Tutorial Image").gameObject.GetComponent<Image>();

        _panel.SetActive(false);
    }

    public void ShowTutorial(string title, string text, Sprite imageSprite)
    {
        this._panel.SetActive(true);
        this._title.text = title;
        this._message.text = text;
        this._image.sprite = imageSprite;

        var player = GameObject.FindGameObjectWithTag("Player");
        var playerScript = player.GetComponent<PhilipController>();
        playerScript.SetControlsActive(false);
    }

    public void HideTutorial()
    {
        _panel.SetActive(false);
        var player = GameObject.FindGameObjectWithTag("Player");
        var playerScript = player.GetComponent<PhilipController>();
        playerScript.SetControlsActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
