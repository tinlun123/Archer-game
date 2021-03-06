﻿using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
    public float speed;
    private Vector2 mouse_pos; //Current mouse position
    private bool mouse_drag = false; //Is the mouse being dragged or not
    private Vector2 mouse_press_pos; //Position of the mouse when pressed initially
    private Vector2 move_toward_pos; //The position for the player to move toward
    public float drag_deadzone; //How far you can drag the mouse before it registers as a drag
    

    private LineRenderer lineRenderer;
    public float drag_arrow_width;
    public float drag_arrow_sensitivity; //Affect arrow length when dragging
    public Color drag_arrow_color;

    public GameObject arrow;
    public float arrow_shoot_speed;
	// Use this for initialization
	void Start () {
        

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetWidth(drag_arrow_width, drag_arrow_width);
        lineRenderer.SetVertexCount(2);
        lineRenderer.SetColors(drag_arrow_color, drag_arrow_color);
	}
	
	// Update is called once per frame
	void Update () {
        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        look_at(gameObject, mouse_pos);

        if (Input.GetMouseButtonDown(0)) { //Button press
            mouse_press_pos = mouse_pos;

        }
        if (Input.GetMouseButton(0)) { //Button hold
            if (Vector2.Distance(mouse_pos, mouse_press_pos) > drag_deadzone) { //If the mouse move from its initial position beyond the deadzone
                mouse_drag = true;
            }
            if (mouse_drag) {
                //Drawing arrow line
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, transform.position + ((Vector3)mouse_press_pos - (Vector3)mouse_pos)/(1/drag_arrow_sensitivity));

                look_at(gameObject, transform.position + ((Vector3)mouse_press_pos - (Vector3)mouse_pos)); //Look at where player is shooting
            }
        }
        if (Input.GetMouseButtonUp(0)) { //Button up
            look_at(gameObject, transform.position + ((Vector3)mouse_press_pos - (Vector3)mouse_pos)); //Look at where player is shooting
            if (!mouse_drag) {
                move_toward_pos = mouse_press_pos; //Set destination
            }
            else {
                //Arrow spawning
                GameObject t_arrow = Instantiate(arrow, transform.position, transform.rotation) as GameObject;
                t_arrow.rigidbody2D.velocity = t_arrow.transform.right * arrow_shoot_speed;

                //Drag arrow disassembly code
                lineRenderer.SetPosition(0, Vector3.zero);
                lineRenderer.SetPosition(1, Vector3.zero);
                mouse_drag = false;
            }

        }

        if (Input.GetButton("Stop")) { //Stop the player from moving
            move_toward_pos = transform.position;
        }
        move_to(move_toward_pos, speed);

	}

    void look_at(GameObject obj, Vector3 target) {
        /*Makes the object rotate toward the given point
         *Example usage: lookAt(gameObject, Camera.main.ScreenToWorldPoint(Input.mousePosition)); //Object looks at mouse
         * 
         */

        //Vector2.angle here is used to get the angle between the (1,0) vector(the horizontal line) and the vector between the object and the mouse
        if (transform.position.y < target.y) { //If the mouse is above the object

            //Make the angle negative (e.g. if the mouse position relative to the object is (1,1), vector2.angle((0,1),(1,1)) would return 45, which is facing the left side.
            //If we make that number negative, it would face the right side.
            transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(new Vector2(1, 0), target - obj.transform.position));
        }
        if (transform.position.y > target.y) { //If the mouse is below the object
            transform.rotation = Quaternion.Euler(0, 0, -Vector2.Angle(new Vector2(1, 0), target - obj.transform.position));
        }
    }

    void move_to(Vector2 destination, float speed) {
        /* Move toward a given destination
         * 
         */
        if (new Vector2(transform.position.x, transform.position.y) != destination) { //If destination isn't reached yet
            Vector2 movement = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            transform.position = new Vector3(movement.x, movement.y, transform.position.z); //Move toward destination
        }
    }
}
