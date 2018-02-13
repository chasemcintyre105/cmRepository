using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class controller : MonoBehaviour 
{
	public float speed;
    public Text countText;
    public Text winText;
	private Rigidbody rb;
    private int count;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		speed = 10;
        count = 0;
        setCountText();
        winText.text = "";
    }
	
	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");
		Vector3 movement = new Vector3( moveHorizontal, 0.0f, moveVertical );
		rb.AddForce(movement * speed);
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count++;
            setCountText();
        }
    }

    void setCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 9)
        {
            winText.text = "You Win!";
        }
    }


    //Destroy(other.gameObject);
    //if (other.gameObject.CompareTag("Player"))
    //gameObject.setActive(false)
}
