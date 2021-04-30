using UnityEngine;
using UnityEngine.UI;

namespace MenteBacata.ScivoloCharacterControllerDemo
{
    [DefaultExecutionOrder(2000)]
    public class LevelRotator : MonoBehaviour
    {
        public GameObject menuPanel;

        public Text xRotText, yRotText, zRotText;

        public Slider xRotSlider, yRotSlider, zRotSlider;

        public KeyCode showHideMenuKey;

        private Vector3 originalGravity;

        private void Start()
        {
            SetRotationText();
            menuPanel.SetActive(false);
            originalGravity = Physics.gravity;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (Input.GetKeyDown(showHideMenuKey))
                ShowHideMenu();
        }

        public void ShowHideMenu()
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
            SetEnableComponents(!menuPanel.activeSelf);
            Time.timeScale = menuPanel.activeSelf ? 0f : 1f;
            Cursor.lockState = menuPanel.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;
        }

        public void HandleRotationChange()
        {
            SetRotationText();
            Quaternion newRot = Quaternion.Euler(xRotSlider.value, yRotSlider.value, zRotSlider.value);
            transform.rotation = newRot;
            Physics.gravity = newRot * originalGravity;
        }

        private void SetRotationText()
        {
            xRotText.text = $"X: {Mathf.RoundToInt(xRotSlider.value)}°";
            yRotText.text = $"Y: {Mathf.RoundToInt(yRotSlider.value)}°";
            zRotText.text = $"Z: {Mathf.RoundToInt(zRotSlider.value)}°";
        }

        private void SetEnableComponents(bool enabled)
        {
            Camera.main.GetComponent<OrbitingCamera>().enabled = enabled;

            FindObjectOfType<SimpleCharacterController>().enabled = enabled;

            foreach (var m in FindObjectsOfType<MovingPlatform>())
            {
                m.enabled = enabled;
            }
        }
    } 
}
