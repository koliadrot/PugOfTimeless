using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusText : MonoBehaviour
{
    #region Field Declarations

    private TextMeshProUGUI statusText;

    #endregion

    #region Startup
    private void Awake()
    {
        statusText = GetComponent<TextMeshProUGUI>();
        GetComponent<CanvasRenderer>().SetAlpha(0);
    }

    #endregion

    #region Subject Implementation
    public IEnumerator ChangeStatus(string displayText)//Softing vanish text
    {
        statusText.text = displayText;
        statusText.CrossFadeAlpha(1, 1.5f, false);
        yield return new WaitForSeconds(1.51f);
        statusText.CrossFadeAlpha(0, 1.5f, false);
        yield return new WaitForSeconds(1.51f);
        gameObject.SetActive(false);
    }

    #endregion
}
