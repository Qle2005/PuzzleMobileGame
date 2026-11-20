using UnityEngine;

public class BudgetManager : MonoBehaviour
{
    public static BudgetManager Instance;

    private int maxBudget;
    private int currentBudget;

    void Awake()
    {
         if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public int SetUpBudget(int levelDataBudget)
    {
        maxBudget=levelDataBudget;
        currentBudget=maxBudget;
        return maxBudget;
    }

    public int CalculateBudget(int amount)
    {
        currentBudget=Mathf.Max(currentBudget+amount,0);
        return currentBudget;
    }


}
