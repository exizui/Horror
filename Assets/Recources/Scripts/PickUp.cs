using UnityEngine;

public class PickupUp : MonoBehaviour
{
    public GameObject camera;
    public float distance = 2.5f;
    public GameObject currentWeapon;
    bool canPickUp;
    private KeyCode e_ = KeyCode.E;

    public static PickupUp singleton;

    private void Awake()
    {
        singleton = this;
        if (camera == null && Camera.main != null)
            camera = Camera.main.gameObject;
    }

    void Update()
    {
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, distance))
        {
            if (hit.transform.tag == "Pickable" && Input.GetKeyDown(e_))
                PickUp(hit);
        }
    }


    void PickUp(RaycastHit hit)
    {
        if (canPickUp) Drop();
        {
            GameLogic.Instance.HidePressButton();
            currentWeapon = hit.transform.gameObject;
            //currentWeapon.GetComponent<Rigidbody>().isKinematic = true;
            currentWeapon.transform.parent = transform;
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localEulerAngles = new Vector3(10f, 0f, 0f);
            currentWeapon.GetComponent<BoxCollider>().isTrigger = true;
            canPickUp = true;
            currentWeapon.GetComponent<BoxCollider>().enabled = false;
            MountTranform();
        }
    }

    public void Drop()
    {
        if (currentWeapon != null)
        {
            GameLogic.Instance.HidePressButton();
            currentWeapon.transform.parent = null;
            //currentWeapon.GetComponent<Rigidbody>().isKinematic = false;
            currentWeapon.GetComponent<BoxCollider>().isTrigger = false;
            currentWeapon.GetComponent<BoxCollider>().enabled = true;
            currentWeapon = null;
            canPickUp = true;
        }

    }

    //public void EnableAndDisableCollider(bool a)
    //{
    //    currentWeapon.GetComponent<MeshCollider>().enabled = a;
    //}
    public void MountTranform()
    {
        if (currentWeapon.name == "mount")
        {
            currentWeapon.transform.localPosition = new Vector3(x: 0f, y: 0.403f, z: 0f);
            currentWeapon.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        }
    }
}