using Skyworth.Interaction.SafetyArea;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSetPlaneHeightButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SafetyAreaManager.Instance.OnBeginSetSafeArea += OnBeginSetSafeArea;
        SafetyAreaManager.Instance.OnFinishSetSafeArea += OnFinishSetSafeArea;
        this.GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnDestroy()
    {
        this.GetComponent<Button>().onClick.RemoveListener(OnButtonClick);
        SafetyAreaManager.Instance.OnBeginSetSafeArea -= OnBeginSetSafeArea;
        SafetyAreaManager.Instance.OnFinishSetSafeArea -= OnFinishSetSafeArea;
    }

    private void OnButtonClick()
    {
        SafetyAreaManager.Instance.StartSetSafetyAreaHeight();
    }

    private void OnBeginSetSafeArea()
    {
        this.gameObject.SetActive(false);
    }

    private void OnFinishSetSafeArea()
    {
        this.gameObject.SetActive(true);
    }
}
