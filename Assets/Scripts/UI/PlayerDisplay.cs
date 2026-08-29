using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] private Color healthColor;
    [SerializeField] private Color blockColor;

    [SerializeField] private GameObject blockUI;
    [SerializeField] private CanvasGroup blockCanvasGroup;

    [SerializeField] private float colorTransitionTime = 0.2f;
    [SerializeField] private float blockTransitionTime = 0.45f;

    [SerializeField] private float blockDropDistance = 15f;
    [SerializeField] private float blockStartScale = 1.25f;

    public Player player;

    public TMP_Text playerHealthText;
    public TMP_Text playerBlockText;

    public Image playerSprite;
    public Image playerHealthBarFill;

    private bool _blockWasActive;
    private bool _isBlockTransitioning;
    private bool _blockIsAppearing;

    private float _elapsedTime;
    private float _colorProgress;
    private float _blockProgress;

    private Color _startColor;
    private Color _targetColor;

    private Vector3 _blockTargetPosition;
    private Vector3 _blockStartPosition;


    private void Awake()
    {
        player = GetComponent<Player>();

        blockUI.SetActive(false);

        playerHealthBarFill.color = healthColor;

        blockCanvasGroup.alpha = 0f;
    }

    private void Start()
    {
        _blockTargetPosition = blockUI.transform.localPosition;

        UpdatePlayerDisplay();
    }

    private void Update()
    {
        if (!_isBlockTransitioning) return;

        UpdateAnimationProgress();

        UpdateBlockColorTransition();
        UpdateBlockFade();

        if (_blockIsAppearing)
        {
            UpdateBlockPosition();
            UpdateBlockScale();
        }

        CheckBlockTransitionFinished();
    }

    public void UpdatePlayerDisplay()
    {
        playerHealthText.text = player.playerHealth + "/" + player.playerMaxHealth;

        var healthPercent = (float)player.playerHealth / player.playerMaxHealth;

        playerHealthBarFill.fillAmount = healthPercent;

        playerBlockText.text = player.playerBlock.ToString();

        var blockIsActive = player.playerBlock > 0;

        if (blockIsActive != _blockWasActive)
        {
            StartBlockTransition(blockIsActive);
        }

        _blockWasActive = blockIsActive;
    }


    private void StartBlockTransition(bool blockIsActive)
    {
        _elapsedTime = 0f;

        _blockIsAppearing = blockIsActive;
        _isBlockTransitioning = true;

        _startColor = playerHealthBarFill.color;
        _targetColor = blockIsActive ? blockColor : healthColor;

        if (blockIsActive)
        {
            blockUI.SetActive(true);

            blockCanvasGroup.alpha = 0f;

            _blockStartPosition = _blockTargetPosition + Vector3.up * blockDropDistance;

            blockUI.transform.localPosition = _blockStartPosition;

            blockUI.transform.localScale = Vector3.one * blockStartScale;
        }
    }

    private void UpdateAnimationProgress()
    {
        _elapsedTime += Time.deltaTime;

        _colorProgress = Mathf.Clamp01(_elapsedTime / colorTransitionTime);

        _blockProgress = Mathf.Clamp01(_elapsedTime / blockTransitionTime);
    }

    private void UpdateBlockColorTransition()
    {
        playerHealthBarFill.color = Color.Lerp(_startColor, _targetColor, _colorProgress);
    }
    
    private void UpdateBlockFade()
    {
        blockCanvasGroup.alpha = _blockIsAppearing ? _blockProgress : 1f - _blockProgress;
    }
    
    private void UpdateBlockPosition()
    {
        blockUI.transform.localPosition = Vector3.Lerp(_blockStartPosition, _blockTargetPosition, _blockProgress);
    }

    private void UpdateBlockScale()
    {
        blockUI.transform.localScale = Vector3.Lerp(Vector3.one * blockStartScale, Vector3.one, _blockProgress);
    }

    private void CheckBlockTransitionFinished()
    {
        if (_colorProgress < 1f || _blockProgress < 1f)
        {
            return;
        }

        _isBlockTransitioning = false;

        if (!_blockIsAppearing)
        {
            blockUI.SetActive(false);
        }
    }
}