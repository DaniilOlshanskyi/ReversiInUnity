using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceScript : MonoBehaviour
{

    public enum PieceColor { NONE, BLACK, WHITE };

    public PieceColor Color;
    public int Row;
    public int Col;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {



        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit h;
        Physics.Raycast(castPoint, out h);
        int row = (int)(h.point.z);
        int y = (int)(h.point.x);
        int x = 7 - row;
        print(x + " " + y);
        //this.Flip();

        //print(h.collider.gameObject);
        //h.rigidbody.gameObject.GetComponent<PieceScript>().Flip();
    }

    public void Flip()
    {
        print(this.name + "does a flip with color:" + Color);
        
        if (Color == PieceColor.WHITE)
        {
            gameObject.GetComponentInChildren<Animator>().SetTrigger("W2B");
            //gameObject.GetComponent<Animator>().SetTrigger("W2B");
            Color = PieceColor.BLACK;
        }
        else
        {
            gameObject.GetComponentInChildren<Animator>().SetTrigger("B2W");
            //gameObject.GetComponent<Animator>().SetTrigger("B2W");
            Color = PieceColor.WHITE;
        }
    }

    public void setWhite()
    {
        print(this.name + " sets color to WHITE");
        Color = PieceColor.WHITE;
    }

    public void setBlack()
    {
        print(this.name + " sets color to BLACK");
        Color = PieceColor.BLACK;
    }
}
