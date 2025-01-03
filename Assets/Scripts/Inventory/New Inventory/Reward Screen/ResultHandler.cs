using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using System.Collections;
public class ResultHandler : MonoBehaviour
{
    public GameObject resultPanel;
    public Image resultIcon;
        public TextMeshProUGUI resultCount;

    private void Awake()
    {
        // Initialization if needed
    }

    public void ShowResult(Sprite icon, string countText)
    {
        if (resultPanel)
        {
            resultPanel.SetActive(true);
            resultIcon.sprite = icon;
            resultCount.text = countText;
            resultPanel.GetComponent<Animator>().Play("clip");
        }
        StartCoroutine(HideResultAfterDelay(3.3f));
    }

    private IEnumerator HideResultAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideResult();
    }
    public void HideResult()
    {
        if (resultPanel)
        {
            resultPanel.SetActive(false);
        }
    }
}