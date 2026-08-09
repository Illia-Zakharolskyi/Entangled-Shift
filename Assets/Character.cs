using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] private string characterName;
    [SerializeField] public float moveSpeed;
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int attackDamage;
    [SerializeField] private float maxColdLevel;
    [SerializeField] private float coldIncreaseRate;
    [SerializeField] private Slider coldSlider;
    [SerializeField] private GameObject characterModel;
    [SerializeField] private float shakeIntensity;

    private float currentColdLevel;
    private bool isInColdZone;
    private Vector3 originalModelPosition;

    private void Start()
    {
        currentHealth = maxHealth;
        currentColdLevel = 0f;

        if (coldSlider != null)
        {
            coldSlider.maxValue = maxColdLevel;
            coldSlider.value = 0f;
        }

        if (characterModel != null)
        {
            originalModelPosition = characterModel.transform.localPosition;
        }
    }

    private void Update()
    {
        if (isInColdZone)
        {
            currentColdLevel += coldIncreaseRate * Time.deltaTime;
            if (currentColdLevel > maxColdLevel) currentColdLevel = maxColdLevel;

            if (coldSlider != null)
            {
                coldSlider.value = currentColdLevel;
            }
        }
        else if (currentColdLevel > 0f)
        {
            currentColdLevel -= coldIncreaseRate * Time.deltaTime;
            if (currentColdLevel < 0f) currentColdLevel = 0f;

            if (coldSlider != null)
            {
                coldSlider.value = currentColdLevel;
            }
        }

        if (characterModel != null)
        {
            if (currentColdLevel > 0f)
            {
                float currentShakeForce = (currentColdLevel / maxColdLevel) * shakeIntensity;
                float offsetX = Random.Range(-1f, 1f) * currentShakeForce;
                float offsetZ = Random.Range(-1f, 1f) * currentShakeForce;

                characterModel.transform.localPosition = new Vector3(
                    originalModelPosition.x + offsetX,
                    originalModelPosition.y,
                    originalModelPosition.z + offsetZ
                );
            }
            else
            {
                characterModel.transform.localPosition = originalModelPosition;
            }
        }
    }

    public void Run(bool isRunning, ref float currentMoveSpeed, float normalSpeed, float bonusRunSpeed)
    {
        if (currentHealth <= 0)
        {
            currentMoveSpeed = 0;
            return;
        }

        if (isRunning)
        {
            currentMoveSpeed = normalSpeed + bonusRunSpeed;
        }
        else
        {
            currentMoveSpeed = normalSpeed;
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
    }

    public void DealDamage(Character target)
    {
        if (currentHealth <= 0 || target == null) return;
        target.TakeDamage(attackDamage);
    }

    public void Heal(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ColdZone"))
        {
            isInColdZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ColdZone"))
        {
            isInColdZone = false;
        }
    }
}