using _scripts.PlayerCharacter;
using UnityEngine;

public class Visual_Proj_Tomahawk : Proj_Visual
{
    [SerializeField] float _spinSpeed = 750;

    [Header("Scene References")]
    [SerializeField] LineRenderer _lineRenderer;

    private PlayerCharacter _playerCharacter;
    private Tomahawk _weapon;
    /// <summary> le point d'accroche de la corde sur le joueur </summary>
    private Transform _anchorTransform;

    [Header("Settings")]
    [SerializeField]private float _sineScrollSpeed = 3;
    [SerializeField]private float _sineFrequency = 8;
    [SerializeField]private float _sineMagnitude = .5f;

    bool _isLastThrowTomahawk = true;
    
    protected override void Start()
    {
        base.Start();

        //fetch references
        _playerCharacter = GameManager.Instance.localPlayerCharacter;
        _anchorTransform = _playerCharacter.transform;

        _weapon = ((Tomahawk)_playerCharacter.playerHands.rightHand.equippedItem);
        print(_weapon.name);
        
        //disable the rope line renderer when the player shoots another tomahawk
        _weapon.OnShoot += DisableLineRenderer;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _weapon.OnShoot -= DisableLineRenderer;
    }
    
    void DisableLineRenderer()
    {
        _isLastThrowTomahawk = false;
        print("disable");
    }
    
    protected override void Update()
    {
        //rotate tomahawk
        visuals.transform.Rotate(_spinSpeed * Time.deltaTime, 0, 0);
        
        base.Update();

        //update magic rope visuals
        _lineRenderer.enabled = _isLastThrowTomahawk && _weapon.CanDash && _playerCharacter.playerHands.rightHand.equippedItem == _weapon;
        if(_lineRenderer.enabled)
            UpdateLineRenderer();
    }

    void UpdateLineRenderer()
    {
        const float animDuration = 1f;
        float timeSinceAnimStart = Time.time - spawnTime;
        float animAlpha = Mathf.Clamp01(timeSinceAnimStart / animDuration);
        float invertAnimAlpha = 1f-animAlpha;
        
        Vector3 a = transform.position;
        //Vector3 b = _handTransform.position + Vector3.down*.05f;
        Vector3 b = _anchorTransform.position + Vector3.down*.3f;
        
        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            float positionAlpha = i / (float)(_lineRenderer.positionCount-1);
            float parabola = (positionAlpha * (1 - positionAlpha)) * 4 * invertAnimAlpha;
            float sineWave = Mathf.Sin(Time.time * _sineScrollSpeed + positionAlpha * _sineFrequency) * _sineMagnitude * invertAnimAlpha *(positionAlpha * (1f-positionAlpha)*-4);
            
            _lineRenderer.SetPosition(i,Vector3.Lerp(a, b, positionAlpha) + Vector3.down * parabola + transform.right * sineWave  );
        }
    }
}
