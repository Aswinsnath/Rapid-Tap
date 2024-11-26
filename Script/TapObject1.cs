using UnityEngine;

public class TapObject1 : MonoBehaviour
{
    public bool isTarget;  
    public bool isBonus;   
    public float moveSpeed = 2f;  
    public Vector2 moveDirection = Vector2.right;  

    private void Update()
    {
        if (isBonus || moveDirection != Vector2.zero)
        {
            
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    void OnMouseDown()
    {
        
        FindObjectOfType<Level>().HandleTap(isTarget, isBonus);
    }
}
