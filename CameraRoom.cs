using UnityEngine;


public class CameraRoom : MonoBehaviour
{
    private Cinemachine.CinemachineVirtualCamera vcam;
    private Transform CameraTarget;
    public bool bigRoom;
    public GameObject tileset;

    private void Awake()
    {
        CameraTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;
        tileset.SetActive(false);
        vcam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            CameraTarget.position = transform.position;
            tileset.SetActive(true);
            if (bigRoom)
            {
                CameraTarget.localScale = new Vector3(16, 16, 0);
                vcam.Follow = GameObject.FindGameObjectWithTag("Player").transform;
                vcam.m_Lens.OrthographicSize = 6f;
            }
            else
            {
                CameraTarget.localScale = new Vector3(8, 8, 0);
                vcam.Follow = CameraTarget;
                vcam.m_Lens.OrthographicSize = 4f;
            }
        }
        if (col.CompareTag("CameraTarget"))
        {
            tileset.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            tileset.SetActive(false);
        }
    }
}
