using UnityEngine;

namespace TeaSpoons.UGuiDesignSystem.Samples
{
    public class DisableOnStart : MonoBehaviour
    {
        void Start()
        {
            gameObject.SetActive(false);
        }
    }
}
