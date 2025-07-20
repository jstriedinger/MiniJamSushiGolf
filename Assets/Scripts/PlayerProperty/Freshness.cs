using UnityEngine;
namespace PlayerProperty
{
    public class Freshness : MonoBehaviour
    {
        public float freshnessAmount;
        public float maxFreshness;
        public float freshnessDecreaseMultiplier;

        public bool isDecreasingFreshness = false;

        private void Awake()
        {
            maxFreshness = freshnessAmount;
        }

        private void Update()
        {
            CheckFreshness();
        }

        private void CheckFreshness()
        {
            if (isDecreasingFreshness)
            {
                if (freshnessAmount > 0)
                {
                    freshnessAmount += Time.deltaTime * freshnessDecreaseMultiplier;
                }
            }
        }

        public float GetFreshnessPercentage()
        {
            if (freshnessAmount >= 0)
            {
                return freshnessAmount / maxFreshness;
            }
            else
            {
                return 0;
            }
        }
        
        //ok im not sure if this is the most efficient way
        //but this is proabbly the easieast way and quickest
        //hard ref to sushigolfhitcontrol
        public void ModifyFreshness(bool isActivePlayer)
        {
            isDecreasingFreshness = isActivePlayer;
            //Debug.Log($"fresshness is  {isDecreasingFreshness}");
        }
        
    }
}