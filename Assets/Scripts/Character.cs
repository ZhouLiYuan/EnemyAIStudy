using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpSpeed = 10f;

    [SerializeField] private Rigidbody2D _rigidbody;

    private GroundDetect _groundDetect;
    private bool IsGrounded { get { return _groundDetect.IsGrounded; } }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _groundDetect = GetComponentInChildren<GroundDetect>();

        SoundManager.PlaySound("BGM");
    }

    private float horizontal;
    private float vertical;

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = new Vector2(horizontal * _speed, _rigidbody.velocity.y);
    }

    #region 角色

    private void Jump()
    {
        // 播放跳跃音效
        SoundManager.PlaySound("audio");

        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpSpeed);
    }



    #endregion
}
