using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    // selected object
    public int objectList = 1;
    public List<GameObject> selectedObject = new List<GameObject>();
    public int objectNum = 0;

    // color
    public List<float> r = new List<float>(), g = new List<float>(), b = new List<float>();

    // size
    public List<float> baseSize = new List<float>(),
        sizeX = new List<float>(), sizeY = new List<float>(), sizeZ = new List<float>();

    // grid
    public bool mirrorAxisX = false, mirrorAxisY = false, mirrorAxisZ = false;
    public int row = 1, column = 1, layer = 1;

    // space
    public float baseSpace = 3;
    public float spaceX = 1, spaceY = 1, spaceZ = 1;

    // changing space
    public float baseSpaceChange;
    public float spaceXChange, spaceZChange, spaceYChangeX, spaceYChangeZ;

    // rotate
    public float rotateX, rotateY, rotateZ;
    // check if rotation is identical
    public bool rotateXIdentical = false, rotateYIdentical = false, rotateZIdentical = false;

    // object queue
    public bool queueAxisX, queueAxisY, queueAxisZ;

    // check material
    [HideInInspector]
    public List<bool> haveMaterial = new List<bool>();

    private void Update()
    {
        if (gameObject.transform.hasChanged)
        {
            CreateGrid();
        }
    }

    public void CreateGrid()
    {
        // check if object list is emepty
        bool checkEmpty = true;

        for (int i = 0; i < selectedObject.Count; i++)
        {
            if (selectedObject[i] != null)
            {
                checkEmpty = false;
                break;
            }
        }

        if (row < 1 || column < 1 || layer < 1)
        {
            Debug.LogWarning("Grid value cannot changed to below 1.");
        }
        else if (checkEmpty == true)
        {
            Debug.LogError("The game object have not been selected.");
        }
        else
        {
            // group up the grid
            for (int i = 0; i < selectedObject.Count; i++)
            {
                if (selectedObject[i] != null)
                {
                    while (GameObject.Find(selectedObject[i].name + " Grid") != null)
                        DestroyImmediate(GameObject.Find(selectedObject[i].name + " Grid"));

                    if (GameObject.Find("Grid Group") != null)
                        DestroyImmediate(GameObject.Find("Grid Group"));
                    DestroyImmediate(GameObject.Find(selectedObject[i].name + " Grid"));
                }
            }
            GameObject objectGrid = new GameObject(" Grid");
        
            if (selectedObject[objectNum] != null)
            {
                objectGrid.name = selectedObject[objectNum].name + " Grid";
            }

            // set up object number
            int objectNumber = objectNum;

            // create grid
            for (int gridLayer = 1; gridLayer <= layer; gridLayer++)
            {
                for (int gridColumn = 1; gridColumn <= column; gridColumn++)
                {
                    for (int gridRow = 1; gridRow <= row; gridRow++)
                    {
                        // check if selected object exist
                        if (selectedObject[objectNumber] != null)
                        {
                            // set material
                            if (selectedObject[objectNumber].GetComponent<Renderer>() != null)
                                if (selectedObject[objectNumber].GetComponent<Renderer>().sharedMaterial != null)
                                {
                                    selectedObject[objectNumber].GetComponent<Renderer>().sharedMaterial.color =
                                        new Color(r[objectNumber], g[objectNumber], b[objectNumber]);
                                    haveMaterial[objectNumber] = true;
                                }

                            // set size
                            selectedObject[objectNumber].transform.localScale =
                                new Vector3(sizeX[objectNumber], sizeY[objectNumber], 
                                sizeZ[objectNumber]) * baseSize[objectNumber];

                            // set rotation
                            var rotationX = rotateX; var rotationY = rotateY; var rotationZ = rotateZ;

                            // create object
                            var selectedPrefab = Instantiate(selectedObject[objectNumber],
                                transform.position + new Vector3((gridRow - 1) * spaceX, (gridLayer - 1) * spaceY,
                               (gridColumn - 1) * spaceZ) * baseSpace, transform.rotation);

                            // add more space change
                            selectedPrefab.transform.position += new Vector3(
                                (gridLayer - 1) * spaceXChange * baseSpaceChange,
                                ((gridRow - 1) * spaceYChangeX * baseSpaceChange) +
                                ((gridColumn - 1) * spaceYChangeZ * baseSpaceChange),
                                (gridLayer - 1) * spaceZChange * baseSpaceChange);

                            // check if rotation is identical
                            if (!rotateXIdentical)
                            {
                                rotationX = (gridRow - 1) * rotateX;
                            }
                            if (!rotateYIdentical)
                            {
                                rotationY = (gridLayer - 1) * rotateY;
                            }
                            if (!rotateZIdentical)
                            {
                                rotationZ = (gridColumn - 1) * rotateZ;
                            }

                            // set rotation
                            selectedPrefab.transform.Rotate(new Vector3(rotationX, rotationY, rotationZ));

                            // group up object
                            selectedPrefab.transform.parent = objectGrid.transform;

                            // mirror
                            if (mirrorAxisX)
                            {
                                var mirrorX = Instantiate(selectedObject[objectNumber], transform.position +
                                new Vector3((gridRow) * -spaceX, (gridLayer - 1) * spaceY,
                                (gridColumn - 1) * spaceZ) * baseSpace, transform.rotation);

                                // add more space change
                                mirrorX.transform.position += new Vector3(
                                    (gridLayer - 1) * -spaceXChange * baseSpaceChange,
                                    ((gridRow - 1) * spaceYChangeX * baseSpaceChange) +
                                    ((gridColumn - 1) * spaceYChangeZ * baseSpaceChange),
                                    (gridLayer - 1) * spaceZChange * baseSpaceChange);

                                // check if rotation is identical
                                if (!rotateXIdentical)
                                {
                                    rotationX = (gridRow - 1) * rotateX;
                                }
                                if (!rotateYIdentical)
                                {
                                    rotationY = (gridLayer - 1) * rotateY;
                                }
                                if (!rotateZIdentical)
                                {
                                    rotationZ = (gridColumn - 1) * rotateZ;
                                }

                                // set rotation
                                mirrorX.transform.Rotate(new Vector3(rotationX, -rotationY, -rotationZ));

                                // group up object
                                mirrorX.transform.parent = objectGrid.transform;
                            }

                            // mirror object
                            if (mirrorAxisY)
                            {
                                var mirrorY = Instantiate(selectedObject[objectNumber], transform.position +
                               new Vector3((gridRow - 1) * spaceX, (gridLayer) * -spaceY,
                               (gridColumn - 1) * spaceZ) * baseSpace, transform.rotation);

                                // add more space change
                                mirrorY.transform.position += new Vector3(
                                    (gridLayer - 1) * spaceXChange * baseSpaceChange,
                                    ((gridRow - 1) * -spaceYChangeX * baseSpaceChange) +
                                    ((gridColumn - 1) * -spaceYChangeZ * baseSpaceChange),
                                    (gridLayer - 1) * spaceZChange * baseSpaceChange);

                                // check if rotation is identical
                                if (!rotateXIdentical)
                                {
                                    rotationX = (gridRow - 1) * rotateX;
                                }
                                if (!rotateYIdentical)
                                {
                                    rotationY = (gridLayer - 1) * rotateY;
                                }
                                if (!rotateZIdentical)
                                {
                                    rotationZ = (gridColumn - 1) * rotateZ;
                                }


                                mirrorY.transform.Rotate(new Vector3(-rotationX, rotationY, -rotationZ));

                                mirrorY.transform.parent = objectGrid.transform;

                                // match mirror
                                if (mirrorAxisX)
                                {
                                    var mirrorX = Instantiate(selectedObject[objectNumber], transform.position +
                                    new Vector3((gridRow) * -spaceX, (gridLayer) * -spaceY,
                                    (gridColumn - 1) * spaceZ) * baseSpace, transform.rotation);

                                    // add more space change
                                    mirrorX.transform.position += new Vector3(
                                        (gridLayer - 1) * -spaceXChange * baseSpaceChange,
                                        ((gridRow - 1) * -spaceYChangeX * baseSpaceChange) +
                                        ((gridColumn - 1) * -spaceYChangeZ * baseSpaceChange),
                                        (gridLayer - 1) * spaceZChange * baseSpaceChange);

                                    // check if rotation is identical
                                    if (!rotateXIdentical)
                                    {
                                        rotationX = (gridRow - 1) * rotateX;
                                    }
                                    if (!rotateYIdentical)
                                    {
                                        rotationY = (gridLayer - 1) * rotateY;
                                    }
                                    if (!rotateZIdentical)
                                    {
                                        rotationZ = (gridColumn - 1) * rotateZ;
                                    }

                                    mirrorX.transform.Rotate(new Vector3(-rotationX, -rotationY, rotationZ));

                                    mirrorX.transform.parent = objectGrid.transform;
                                }
                            }

                            if (mirrorAxisZ)
                            {
                                var mirrorZ = Instantiate(selectedObject[objectNumber], transform.position +
                               new Vector3((gridRow - 1) * spaceX, (gridLayer - 1) * spaceY,
                               (gridColumn) * -spaceZ) * baseSpace, transform.rotation);

                                // add more space change
                                mirrorZ.transform.position += new Vector3(
                                    (gridLayer - 1) * spaceXChange * baseSpaceChange,
                                    ((gridRow - 1) * spaceYChangeX * baseSpaceChange) +
                                    ((gridColumn - 1) * spaceYChangeZ * baseSpaceChange),
                                    (gridLayer - 1) * -spaceZChange * baseSpaceChange);

                                // check if rotation is identical
                                if (!rotateXIdentical)
                                {
                                    rotationX = (gridRow - 1) * rotateX;
                                }
                                if (!rotateYIdentical)
                                {
                                    rotationY = (gridLayer - 1) * rotateY;
                                }
                                if (!rotateZIdentical)
                                {
                                    rotationZ = (gridColumn - 1) * rotateZ;
                                }


                                mirrorZ.transform.Rotate(new Vector3(-rotationX, -rotationY, rotationZ));

                                mirrorZ.transform.parent = objectGrid.transform;

                                // match mirror
                                if (mirrorAxisX)
                                {
                                    var mirrorX = Instantiate(selectedObject[objectNumber], transform.position +
                                    new Vector3((gridRow) * -spaceX, (gridLayer - 1) * spaceY,
                                    (gridColumn) * -spaceZ) * baseSpace, transform.rotation);

                                    // add more space change
                                    mirrorX.transform.position += new Vector3(
                                        (gridLayer - 1) * -spaceXChange * baseSpaceChange,
                                        ((gridRow - 1) * spaceYChangeX * baseSpaceChange) +
                                        ((gridColumn - 1) * spaceYChangeZ * baseSpaceChange),
                                        (gridLayer - 1) * -spaceZChange * baseSpaceChange);

                                    // check if rotation is identical
                                    if (!rotateXIdentical)
                                    {
                                        rotationX = (gridRow - 1) * rotateX;
                                    }
                                    if (!rotateYIdentical)
                                    {
                                        rotationY = (gridLayer - 1) * rotateY;
                                    }
                                    if (!rotateZIdentical)
                                    {
                                        rotationZ = (gridColumn - 1) * rotateZ;
                                    }

                                    mirrorX.transform.Rotate(new Vector3(-rotationX, rotationY, -rotationZ));

                                    mirrorX.transform.parent = objectGrid.transform;
                                }

                                if (mirrorAxisY)
                                {
                                    var mirrorY = Instantiate(selectedObject[objectNumber], transform.position +
                                    new Vector3((gridRow - 1) * spaceX, (gridLayer) * -spaceY,
                                    (gridColumn) * -spaceZ) * baseSpace, transform.rotation);

                                    // add more space change
                                    mirrorY.transform.position += new Vector3(
                                        (gridLayer - 1) * spaceXChange * baseSpaceChange,
                                        ((gridRow - 1) * -spaceYChangeX * baseSpaceChange) +
                                        ((gridColumn - 1) * -spaceYChangeZ * baseSpaceChange),
                                        (gridLayer - 1) * -spaceZChange * baseSpaceChange);

                                    // check if rotation is identical
                                    if (!rotateXIdentical)
                                    {
                                        rotationX = (gridRow - 1) * rotateX;
                                    }
                                    if (!rotateYIdentical)
                                    {
                                        rotationY = (gridLayer - 1) * rotateY;
                                    }
                                    if (!rotateZIdentical)
                                    {
                                        rotationZ = (gridColumn - 1) * rotateZ;
                                    }

                                    mirrorY.transform.Rotate(new Vector3(rotationX, -rotationY, -rotationZ));

                                    mirrorY.transform.parent = objectGrid.transform;
                                }

                                if (mirrorAxisX && mirrorAxisY)
                                {
                                    var mirrorY = Instantiate(selectedObject[objectNumber], transform.position +
                                    new Vector3((gridRow) * -spaceX, (gridLayer) * -spaceY,
                                    (gridColumn) * -spaceZ) * baseSpace, transform.rotation);

                                    // add more space change
                                    mirrorY.transform.position += new Vector3(
                                        (gridLayer - 1) * -spaceXChange * baseSpaceChange,
                                        ((gridRow - 1) * -spaceYChangeX * baseSpaceChange) +
                                        ((gridColumn - 1) * -spaceYChangeZ * baseSpaceChange),
                                        (gridLayer - 1) * -spaceZChange * baseSpaceChange);

                                    // check if rotation is identical
                                    if (!rotateXIdentical)
                                    {
                                        rotationX = (gridRow - 1) * rotateX;
                                    }
                                    if (!rotateYIdentical)
                                    {
                                        rotationY = (gridLayer - 1) * rotateY;
                                    }
                                    if (!rotateZIdentical)
                                    {
                                        rotationZ = (gridColumn - 1) * rotateZ;
                                    }

                                    mirrorY.transform.Rotate(new Vector3(rotationX, rotationY, rotationZ));

                                    mirrorY.transform.parent = objectGrid.transform;
                                }
                            }
                        }
                        // queue X
                        if (queueAxisX)
                        {
                            if (objectNumber < selectedObject.Count - 1)
                                objectNumber++;
                            else
                            {
                                objectNumber = 0;
                            }
                        }
                    }
                    // queue Z
                    if (queueAxisZ)
                    {
                        if (objectNumber < selectedObject.Count - 1)
                            objectNumber++;
                        else
                        {
                            objectNumber = 0;
                        }
                    }
                }
                // queue Y
                if (queueAxisY)
                {
                    if (objectNumber < selectedObject.Count - 1)
                        objectNumber++;
                    else
                    {
                        objectNumber = 0;
                    }
                }
            }
            // grid group
            if (objectList > 0)
            {
                GameObject gridGroup = new GameObject("Grid Group");
                objectGrid.transform.parent = gridGroup.transform;
            }
        }

    }
}