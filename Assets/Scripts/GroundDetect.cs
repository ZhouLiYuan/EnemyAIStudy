using UnityEngine;

public class GroundDetect : MonoBehaviour
{
    public bool IsGrounded { get; private set; }

    //private bool _isGrounded;
    //public bool GetIsGrounded() { return _isGrounded; }
    //private bool SetIsGrounded(bool value) { __isGrounded = value; }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 碰到符合条件的Collider，认为落地
        if (true)
        {
            IsGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (true)
        {
            IsGrounded = false;
        }
    }
}
