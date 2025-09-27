using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using DG.Tweening;

/// <summary>
/// タイルマップを水平方向または垂直方向にスライスして、
/// プレイヤーがいない側のタイルグループを移動させるクラス。
/// </summary>
public class TilemapSlicer : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Tilemap _originalTilemap;
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private Transform _playerTransform;

    [Space]

    [SerializeField] private float _sliceOffset = 2f;
    [SerializeField] private float _sliceDuration = 0.5f;
    #endregion

    #region Private Variables
    private Tilemap _cloneA;
    private Tilemap _cloneB;
    private Tilemap _movedClone;
    private Vector3 _originalLocalPos;

    private bool _sliceState = false;
    private bool _isSliceInProgress = false;

    private InputAction _m_slashRightAction;
    private InputAction _m_slashLeftAction;
    private InputAction _m_slashUpAction;
    private InputAction _m_slashDownAction;

    private enum SliceMode { None, Horizontal, Vertical }
    private SliceMode _lastMode = SliceMode.None;
    #endregion

    #region Start & Updates
    void Start()
    {
        _m_slashRightAction = _inputActions.FindAction("SlashRight");
        _m_slashLeftAction = _inputActions.FindAction("SlashLeft");
        _m_slashUpAction = _inputActions.FindAction("SlashUp");
        _m_slashDownAction = _inputActions.FindAction("SlashDown");

        if (_m_slashRightAction != null) _m_slashRightAction.Enable();
        if (_m_slashLeftAction != null) _m_slashLeftAction.Enable();
        if (_m_slashUpAction != null) _m_slashUpAction.Enable();
        if (_m_slashDownAction != null) _m_slashDownAction.Enable();

        _originalLocalPos = _originalTilemap.transform.localPosition;
    }

    void Update()
    {
        if (_isSliceInProgress) return;

        if (_m_slashRightAction != null && _m_slashRightAction.WasPressedThisFrame())
            HandleSlash(SliceMode.Horizontal, true);

        else if (_m_slashLeftAction != null && _m_slashLeftAction.WasPressedThisFrame())
            HandleSlash(SliceMode.Horizontal, false);

        else if (_m_slashUpAction != null && _m_slashUpAction.WasPressedThisFrame())
            HandleSlash(SliceMode.Vertical, true);

        else if (_m_slashDownAction != null && _m_slashDownAction.WasPressedThisFrame())
            HandleSlash(SliceMode.Vertical, false);
    }

    #endregion

    #region Custom Functions
    private void HandleSlash(SliceMode mode, bool positiveDirection)
    {
        if (!_sliceState)
        {
            _isSliceInProgress = true;
            _sliceState = true;
            if (mode == SliceMode.Horizontal)
                SliceHorizontal(positiveDirection);
            else
                SliceVertical(positiveDirection);

            _lastMode = mode;
        }
        else
        {
            Uncut();
        }
    }

    /// <summary>
    /// 水平方向にタイルをスライスする。
    /// プレイヤーがいない側のタイルを左または右に移動させる。
    /// </summary>
    private void SliceHorizontal(bool slashRight)
    {
        _cloneA = Instantiate(_originalTilemap, _originalTilemap.transform.parent);
        _cloneB = Instantiate(_originalTilemap, _originalTilemap.transform.parent);

        _originalTilemap.gameObject.SetActive(false);

        BoundsInt bounds = _originalTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = _originalTilemap.GetTile(pos);
                if (tile == null) continue;

                if (y >= 0)
                {
                    _cloneB.SetTile(pos, null);
                }
                else
                {
                    _cloneA.SetTile(pos, null);
                }
            }
        }

        float direction = slashRight ? 1f : -1f;

        if (_playerTransform.position.y >= 0)
        {
            _movedClone = _cloneB;
        }
        else
        {
            _movedClone = _cloneA;
        }

        _movedClone.transform.DOLocalMoveX(_movedClone.transform.localPosition.x + direction * _sliceOffset, _sliceDuration)
            .OnComplete(() => _isSliceInProgress = false);
    }

    /// <summary>
    /// 垂直方向にタイルをスライスする。
    /// プレイヤーがいない側のタイルを上または下に移動させる。
    /// </summary>
    private void SliceVertical(bool slashUp)
    {
        _cloneA = Instantiate(_originalTilemap, _originalTilemap.transform.parent);
        _cloneB = Instantiate(_originalTilemap, _originalTilemap.transform.parent);

        _originalTilemap.gameObject.SetActive(false);

        BoundsInt bounds = _originalTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = _originalTilemap.GetTile(pos);
                if (tile == null) continue;

                if (x >= 0)
                {
                    _cloneB.SetTile(pos, null);
                }
                else
                {
                    _cloneA.SetTile(pos, null);
                }
            }
        }

        float direction = slashUp ? 1f : -1f;

        if (_playerTransform.position.x >= 0)
        {
            _movedClone = _cloneB;
        }
        else
        {
            _movedClone = _cloneA;
        }

        _movedClone.transform.DOLocalMoveY(_movedClone.transform.localPosition.y + direction * _sliceOffset, _sliceDuration)
            .OnComplete(() => _isSliceInProgress = false);
    }

    /// <summary>
    /// スライスして移動したタイルを元に戻す。
    /// </summary>
    private void Uncut()
    {
        _isSliceInProgress = true;
        _sliceState = false;

        _movedClone.transform.DOLocalMove(_originalLocalPos, _sliceDuration)
            .OnComplete(() =>
            {
                Destroy(_cloneA.gameObject);
                Destroy(_cloneB.gameObject);
                _originalTilemap.gameObject.SetActive(true);
                _isSliceInProgress = false;
                _lastMode = SliceMode.None;
                _movedClone = null;
            });
    }
    #endregion
}