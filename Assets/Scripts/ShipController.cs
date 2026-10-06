// using System.Numerics;
using System.Collections;
// using System.IO.Compression;
// using Unity.Android.Gradle;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class ShipController : BoundedEntity
{

    [Header("Sound Effects")]
    [SerializeField] private SoundEffectHandler m_boostSoundFX;
    [SerializeField] private SoundEffectHandler m_thrustSoundFX;
    [SerializeField] private SoundEffectHandler m_fireSoundFX;
    [SerializeField] private SoundEffectHandler m_deathSoundFX;
    [SerializeField] private SoundEffectHandler m_hitSoundFX;

    [Header("Other References")]
    [SerializeField] private bool m_isDead;
    [SerializeField] private GameObject m_bulletPrefab;
    [SerializeField] private GameObject m_shield;
    [SerializeField] private VisualEffect m_rightThruster;
    [SerializeField] private VisualEffect m_leftThruster;
    [SerializeField] private GameObject m_deathVFXPrefab;
    [SerializeField] private float m_fireDelay;
    [SerializeField] private float m_fireCount;
    [SerializeField] private bool m_isFiring;
    [SerializeField] private bool m_isBoosting;
    private float m_turnInput;
    private float m_forwardInput;
    [SerializeField] private float m_turnSpeed;
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_boostSpeed;
    [SerializeField] private float m_stoppingPower;
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_maxBoostSpeed;
    private bool m_waitingForAstroids;
    private bool m_isInvulnerable;
    private Coroutine m_invulnerabilityRoutine;
    private float m_nextAsteroidHitTime;
    private Coroutine m_hitRoutine;
    private Material m_shipHitMaterial;

    // private void FadeIn(){

    // }

    // private void FadeOut(){

    // }

    protected override void OnEnable(){
        GameEvents.Instance.onRetry += OnRetry;
        GameEvents.Instance.onInitialAstroidsSpawned += OnInitialAstroidsSpawned;
        BeginSpawnInvulnerability();
        EnsureShipHitMaterial();
        SetHitFlashIntensity(0f);
        base.OnEnable();
    }

    protected override void OnDisable(){
        GameEvents.Instance.onRetry -= OnRetry;
        GameEvents.Instance.onInitialAstroidsSpawned -= OnInitialAstroidsSpawned;
        SetHitFlashIntensity(0f);
        base.OnDisable();
    }

    private void OnRetry(){
        BeginSpawnInvulnerability();
        ResetHealth();
        SetPlayerDead();
        if (m_spriteRenderer != null)
        {
            m_spriteRenderer.enabled = false;
        }
        SetHitFlashIntensity(0f);
        StartCoroutine(RespawnPlayer());
    }

    private void BeginSpawnInvulnerability(){
        m_waitingForAstroids = true;
        SetInvulnerable(true);
        if (m_collider != null)
        {
            m_collider.enabled = false;
        }
    }

    private void OnInitialAstroidsSpawned(){
        m_waitingForAstroids = false;
        if (m_isDead)
        {
            return;
        }

        m_collider.enabled = true;
        // First start / retry never go through RespawnPlayer's timed shield-off,
        // so end invulnerability here once the opening wave is ready.
        StartInvulnerabilityTimer();
    }

    private void SetInvulnerable(bool invulnerable){
        m_isInvulnerable = invulnerable;
        if (m_shield != null)
        {
            m_shield.SetActive(invulnerable);
        }
    }

    private void StartInvulnerabilityTimer(){
        if (m_invulnerabilityRoutine != null)
        {
            StopCoroutine(m_invulnerabilityRoutine);
        }
        m_invulnerabilityRoutine = StartCoroutine(EndInvulnerabilityAfterDelay());
    }

    private IEnumerator EndInvulnerabilityAfterDelay(){
        yield return new WaitForSeconds(3f);
        SetInvulnerable(false);
        m_invulnerabilityRoutine = null;
    }

    // Update is called once per frame
    void Update()
    {

        // OnMove(new InputValue());
    }

    private void OnBoost(InputValue value){
        m_isBoosting = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;

        if(m_isBoosting){
            EnableThrusters(true, true);
            m_boostSoundFX.FadeIn();
        }
        else {
            m_boostSoundFX.FadeOut();
        }

        if(!m_isBoosting && m_forwardInput == 0){
            EnableThrusters(false, false);
        }

    }

    void OnMove(InputValue value)
    {
        // PlayerInput sends a real value on started/performed/canceled.
        // LateUpdate passes an empty InputValue so we can keep applying held input.
        if (TryReadMoveInput(value, out Vector2 moveInputDirection))
        {
            m_turnInput = moveInputDirection.x;
            m_forwardInput = moveInputDirection.y;

            if(m_forwardInput >= 0)
            {
                EnableThrusters(true, true);
                m_thrustSoundFX.FadeIn();
            }
            else
            {
                EnableThrusters(false, false);
                m_thrustSoundFX.FadeOut();
            }
        }

        if(m_isDead){return;}

        if(m_rigidbody.linearVelocity.magnitude > m_maxSpeed){
            m_rigidbody.linearVelocity = m_rigidbody.linearVelocity.normalized * m_maxSpeed;
        }

        m_rigidbody.rotation -= m_turnInput * (10f * m_turnSpeed) * Time.deltaTime;

        if(m_isBoosting){
            m_rigidbody.AddRelativeForceY(100f * m_boostSpeed * Time.deltaTime);
        }

        if(m_forwardInput > 0)
        {
            m_rigidbody.AddRelativeForceY(m_forwardInput * (100f * m_moveSpeed) * Time.deltaTime);
        }
        else if(m_forwardInput < 0)
        {
            m_rigidbody.linearVelocity = Vector2.Lerp(m_rigidbody.linearVelocity, Vector2.zero, m_stoppingPower * Time.deltaTime);
        }

        if(m_isBoosting && m_rigidbody.linearVelocity.magnitude > m_maxBoostSpeed)
        {
            m_rigidbody.linearVelocity = m_rigidbody.linearVelocity.normalized * m_maxBoostSpeed;
        }
        else if(!m_isBoosting && m_rigidbody.linearVelocity.magnitude > m_maxSpeed)
        {
            m_rigidbody.linearVelocity = m_rigidbody.linearVelocity.normalized * m_maxSpeed;
        }
    }

    private bool TryReadMoveInput(InputValue value, out Vector2 moveInput)
    {
        moveInput = Vector2.zero;
        try
        {
            moveInput = value.Get<Vector2>();
            return true;
        }
        catch (System.InvalidOperationException)
        {
            return false;
        }
    }

    private void EnableThrusters(bool  right, bool left){
        m_leftThruster.enabled = left;
        m_rightThruster.enabled = right;
    }

    protected override void LateUpdate()
    {
        OnBoost(new InputValue());
        OnMove(new InputValue());

        if(m_isDead){return;}

        base.LateUpdate();

        m_isFiring = Mouse.current != null && Mouse.current.leftButton.isPressed;
        if(m_isFiring){
            TrySpawnBullet();
        }
        else{
            m_fireCount = m_fireDelay;
        }

    }

    private void TrySpawnBullet(){
        if(m_fireCount > m_fireDelay){
            m_fireCount = 0;
            GameObject bullet = Instantiate<GameObject>(m_bulletPrefab);
            bullet.transform.position = transform.position + (transform.up * 3f);
            bullet.transform.up = transform.up;

            m_fireSoundFX.Play();
        }
        else{
            m_fireCount += Time.deltaTime;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryHitAsteroid(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Backup for fast overlaps that miss Enter under discrete steps.
        TryHitAsteroid(collision);
    }

    private void TryHitAsteroid(Collider2D collision)
    {
        if (m_isDead || m_isInvulnerable || Time.time < m_nextAsteroidHitTime) { return; }

        if (collision.TryGetComponent(out AstroidController _))
        {
            // Prevent Stay from draining all health in a few FixedUpdates.
            m_nextAsteroidHitTime = Time.time + 0.35f;
            // Flash before LoseHealth so the killing blow still shows a brief hit effect.
            PlayHitFlash();
            m_hitSoundFX.Play();
            LoseHealth();
        }
    }

    protected void SetPlayerDead(){
        m_isDead = true;
        m_collider.enabled = false;
        m_rigidbody.simulated = false;
        // Keep the sprite visible until the ship-local hit flash finishes.
    }

    protected override void OnDie()
    {
        GameEvents.Instance.PlayerDied();
        SpawnVFX(m_deathVFXPrefab);
        m_deathSoundFX.Play();
        SetPlayerDead();
        StartCoroutine(HideShipAfterHitFlash());
    }

    private IEnumerator HideShipAfterHitFlash()
    {
        // Killing blow starts PlayHitFlash() before LoseHealth/OnDie.
        // Wait for that flash so it is visible on the ship.
        if (m_hitRoutine != null)
        {
            yield return m_hitRoutine;
        }
        else
        {
            PlayHitFlash();
            if (m_hitRoutine != null)
            {
                yield return m_hitRoutine;
            }
        }

        if (m_spriteRenderer != null)
        {
            m_spriteRenderer.enabled = false;
        }
        SetHitFlashIntensity(0f);
        StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer(){
        yield return new WaitForSeconds(0.5f);
        m_rigidbody.position = Vector3.zero;
        m_rigidbody.transform.position = Vector3.zero;
        m_rigidbody.linearVelocity = Vector2.zero;
        m_rigidbody.rotation = 0;
        m_spriteRenderer.enabled = true;
        ResetHealth();
        SetInvulnerable(true);
        m_rigidbody.simulated = true;
        m_isDead = false;
        SetHitFlashIntensity(0f);

        EnableThrusters(true, true);
        m_rightThruster.SetBool("isBoosting", false);
        m_leftThruster.SetBool("isBoosting", false);

        // Stay invulnerable until the initial asteroid wave finishes spawning.
        // OnInitialAstroidsSpawned will start the shield timer.
        if (m_waitingForAstroids)
        {
            yield break;
        }

        m_collider.enabled = true;
        StartInvulnerabilityTimer();
    }

    private void PlayHitFlash(){
        EnsureShipHitMaterial();
        if (m_shipHitMaterial == null || m_spriteRenderer == null) { return; }

        m_spriteRenderer.enabled = true;

        if (m_hitRoutine != null)
        {
            StopCoroutine(m_hitRoutine);
        }
        m_hitRoutine = StartCoroutine(HitRoutine());
    }

    private void EnsureShipHitMaterial(){
        if (m_spriteRenderer == null) { return; }

        // Creates a per-renderer instance so _Intensity only affects this ship.
        if (m_shipHitMaterial == null)
        {
            m_shipHitMaterial = m_spriteRenderer.material;
        }
    }

    private void SetHitFlashIntensity(float intensity){
        EnsureShipHitMaterial();
        if (m_shipHitMaterial == null) { return; }

        m_shipHitMaterial.SetFloat("_Intensity", Mathf.Clamp01(intensity));
    }

    private IEnumerator HitRoutine(){
        float intensity = 0f;
        float duration = 0.15f;

        while (intensity < 1f)
        {
            intensity = Mathf.MoveTowards(intensity, 1f, Time.deltaTime / duration);
            SetHitFlashIntensity(intensity);
            yield return null;
        }

        while (intensity > 0f)
        {
            intensity = Mathf.MoveTowards(intensity, 0f, Time.deltaTime / duration);
            SetHitFlashIntensity(intensity);
            yield return null;
        }

        SetHitFlashIntensity(0f);
        m_hitRoutine = null;
    }
}
