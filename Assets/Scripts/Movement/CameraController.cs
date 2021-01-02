using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 offset;
    public string[] finderTags = { "Player", "ghost" };

    public int current = 0;
    public int currentTag = 0;
    private GameObject[] objects;

    // Start is called before the first frame update
    void Start()
    {
        offset = gameObject.transform.position;
        selectTags();
    }

    void selectTags()
    {
        current = 0;
        objects = GameObject.FindGameObjectsWithTag(finderTags[currentTag]);
    }

    void Update()
    {
        if ( Input.GetKeyDown("space"))
        {
            currentTag = (currentTag + 1) % finderTags.Length;
            selectTags();
        }

        if ( Input.GetKeyDown("tab") ) {
            current = (current + 1) % objects.Length;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if ( objects[current] != null)
            gameObject.transform.position = objects[current].transform.position + offset;
    }
}
