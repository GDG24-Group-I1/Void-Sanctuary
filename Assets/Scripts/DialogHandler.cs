using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
public class DialogHandler : MonoBehaviour
{
    private TMP_Text textBox;
    private GameObject dismissButton;
    private string currentDialog;
    private int charsPerSecond;
    private Coroutine runningDialog;
    private Coroutine closingDialog;
    private Animator animator;
    private float waitTime;
    private float? waitStartTime;
    public bool IsInDialog { get; set; }
    public bool IsDialogDismissable { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        textBox = GetComponentInChildren<TMP_Text>();
        dismissButton = GameObject.Find("DismissDialogButton");
        var icon = GameObject.Find("ActivateButtonIcon").GetComponent<Image>();
        animator = GetComponent<Animator>();
        IsInDialog = false;
        IsDialogDismissable = false;
        dismissButton.GetComponent<Button>().onClick.AddListener(DismissDialog);
        var gameInput = GameObject.FindWithTag("InputHandler").GetComponent<GameInput>();
        gameInput.CurrentControl.Changed += (sender, args) =>
        {
            var control = args.NewValue;
            icon.sprite = ControlIconSelector.GetIconForControl(control, control == ControlType.Mouse ? ButtonIcon.MouseLeft : ButtonIcon.ButtonSouth);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (waitStartTime != null)
        {
            dismissButton.GetComponent<Image>().fillAmount = (Time.time - waitStartTime.Value) / waitTime;
        }
    }

    public void SetDialog(string dialog, float writeTimeSeconds, float? lingerTime = null)
    {
        IsInDialog = true;
        dismissButton.GetComponent<Button>().interactable = false;
        dismissButton.GetComponent<Image>().fillAmount = 0;
        if (runningDialog != null)
        {
            StopCoroutine(runningDialog);
            runningDialog = null;
            if (closingDialog != null)
            {
                StopCoroutine(closingDialog);
                closingDialog = null;
            }
        }
        animator.ResetTrigger("Close");
        animator.SetTrigger("Expand");
        textBox.text = "";
        currentDialog = dialog;
        charsPerSecond = (int)(dialog.Length / writeTimeSeconds);
        runningDialog = StartCoroutine(WriteDialog());
        waitTime = lingerTime ?? writeTimeSeconds * 1.5f;
    }
    private IEnumerator WriteDialog()
    {
        while (textBox.text.Length < currentDialog.Length)
        {
            textBox.text += currentDialog[textBox.text.Length];
            yield return new WaitForSeconds(1f / charsPerSecond);
        }
        runningDialog = null;
        closingDialog = StartCoroutine(EnableDismissButton());
    }
    private IEnumerator EnableDismissButton()
    {
        waitStartTime = Time.time;
        yield return new WaitForSeconds(waitTime);
        dismissButton.GetComponent<Button>().interactable = true;
        IsDialogDismissable = true;
        waitStartTime = null;
    }

    public void DismissDialog()
    {
        IsDialogDismissable = false;
        IsInDialog = false;
        animator.ResetTrigger("Expand");
        animator.SetTrigger("Close");
    }

    public void DismissDialogForced()
    {
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        DismissDialog();
    }

    public void RestoreUpdateMode()
    {
        animator.updateMode = AnimatorUpdateMode.Normal;
    }
}
