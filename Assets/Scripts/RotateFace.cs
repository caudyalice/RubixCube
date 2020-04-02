using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RotateFace : MonoBehaviour
{
    public GameObject faceBlue;
    public GameObject faceRed;
    public GameObject faceYellow;
    public GameObject faceWhite;
    public GameObject faceGreen;
    public GameObject faceOrange;

    int numFaces;
    GameObject faceToRotate;
    List<GameObject> cubes;
    List<GameObject> cubes_sorted;
    float min = 3;
    float rotation = 0;
    bool rotate = false;
    float rotateSpeed = 10;
    float mousePosX;
    float mousePosY;
    float rotateSpeedFace = 50;
    int sens = 1;

    List<Vector3> cubePos; //position cubes
    List<Quaternion> cubeRot; //rotation cubes
    List<Material []> cubeMaterial; //materials cubes
    List<GameObject> cubeMiddle; // cubes in middle of faces

    //Get cubes on face
    void GetCubes(GameObject faceMiddle)
    {
        cubes_sorted = cubes.OrderBy(
            x => (faceMiddle.transform.position - x.transform.position).magnitude
            ).ToList();
    }

    private void Start()
    {
        cubeMiddle = new List<GameObject>(new GameObject[] { faceBlue, faceRed, faceOrange, faceGreen, faceWhite, faceYellow});
        init_listcube();
    }

    //init list
    void init_listcube()
    {
        numFaces = transform.childCount;
        cubes = new List<GameObject>();
        cubePos = new List<Vector3>();
        cubeRot = new List<Quaternion>();
        cubeMaterial = new List<Material[]>();
        for (int i = 0; i < numFaces; i++)
        {
            cubes.Add(transform.GetChild(i).gameObject);
            cubePos.Add(transform.GetChild(i).localPosition);
            cubeRot.Add(transform.GetChild(i).localRotation);
            cubeMaterial.Add(transform.GetChild(i).GetComponent<Renderer>().materials);
        }
    }

    public void RandomColors()
    {
        resetRubixcube(); //set cube to original places
        List<Material[]> NewcubeMaterial = cubeMaterial;
        for (int i = 0; i < cubes.Count; i++)
        {
            if (!cubeMiddle.Contains(cubes[i]))
            {
                var firstOddNumberIndex = NewcubeMaterial.Select((f, j) => new { f, j })
                .Where(x => x.f.Length == cubes[i].GetComponent<Renderer>().materials.Length && x.j > i && !cubeMiddle.Contains(cubes[x.j]))
                .Select(x => x.j);
                List<int> indexs = firstOddNumberIndex.ToList();
                if (indexs.Count >= 1)
                {
                    int randomIndex = Random.Range(0, indexs.Count);
                    cubes[i].GetComponent<Renderer>().materials = NewcubeMaterial[indexs[randomIndex]];
                    NewcubeMaterial[indexs[randomIndex]] = NewcubeMaterial[i];
                    NewcubeMaterial[i] = cubes[i].GetComponent<Renderer>().materials;
                    cubes[indexs[randomIndex]].GetComponent<Renderer>().materials = NewcubeMaterial[indexs[randomIndex]];
                    
                }
            }
        }
    }


    public void resetRubixcube()
    {
        for (int i = 0; i < numFaces; i++)
        {
            cubes[i].transform.localPosition = cubePos[i];
            cubes[i].transform.localRotation = cubeRot[i];
        }
    }

    //Rotate face
    void rotateFace(GameObject faceMiddle)
    {
        if (rotate)
        {
            if (rotation < 90)
            {
                rotation += Time.deltaTime * rotateSpeedFace;
                foreach (GameObject c in cubes_sorted.GetRange(0, 9))
                {
                    c.transform.RotateAround(faceMiddle.transform.parent.position, faceMiddle.transform.forward, rotateSpeedFace * Time.deltaTime);
                }
            }
            else
            {
                foreach (GameObject c in cubes_sorted.GetRange(0, 9))
                {
                    c.transform.RotateAround(faceMiddle.transform.parent.position, faceMiddle.transform.forward, 90-rotation); //Round to 90°
                }
                rotate = false;
                rotation = 0;
            }
        }
    }

    private void Update()
    {
        if (rotate)
        {
            rotateFace(faceToRotate); //face rotation
        } else
        {

            if (Input.GetMouseButtonDown(0))
            {
                mousePosX = Input.GetAxis("Mouse X");
                mousePosY = Input.GetAxis("Mouse Y");
            }

            if (Input.GetMouseButton(0))
            {
                transform.parent.Rotate((mousePosY - Input.GetAxis("Mouse Y")) * Time.deltaTime * -rotateSpeed, (mousePosX - Input.GetAxis("Mouse X")) * Time.deltaTime * rotateSpeed, 0, Space.World); //Rotate rubix'cube
                mousePosX += -Input.GetAxis("Mouse X");
                mousePosY += -Input.GetAxis("Mouse Y");
            }

            if (Input.GetKey(KeyCode.R))
            {
                resetRubixcube();
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if ((Physics.Raycast(ray, out hit, Mathf.Infinity)) && (Input.GetMouseButtonDown(0)))
            {
                if ((LayerMask.LayerToName(hit.transform.gameObject.layer) == "Default"))
                {
                    faceToRotate = hit.transform.GetChild(0).gameObject;
                    Debug.Log(faceToRotate.transform.forward);
                    rotate = true;
                    GetCubes(faceToRotate);
                }
            }
        }
    }
}
