using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public GameObject dialoguePanel;      // 拖入 Dialog 对象
    public GameObject interactionPrompt;  // 拖入交互提示 UI

    private bool isDialogueActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (isDialogueActive) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
            {
                interactionPrompt.SetActive(false);
                dialoguePanel.SetActive(true);
                DialogManager dialogManager = dialoguePanel.GetComponent<DialogManager>();
                if (dialogManager != null)
                {
                    dialogManager.StartDialog(this);
                }
                isDialogueActive = true;
                break;
            }
        }
    }

    public void OnDialogueFinished()
    {
        isDialogueActive = false;
    }
}