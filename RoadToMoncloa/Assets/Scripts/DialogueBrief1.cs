using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBrief1 : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private int _index;

    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[_index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[_index];
            }
        }

    }

    void StartDialogue()
    {
        _index = 0;
        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (_index < lines.Length - 1)
        {
            _index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            StartCoroutine(CloseDialog());
        }
    }

    IEnumerator TypeLine()
    {
        //type each character 1 by 1
        foreach (char character in lines[_index].ToCharArray())
        {
            textComponent.text += character;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    IEnumerator CloseDialog()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);

    }


}
